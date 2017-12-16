using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using IMAPI2.Interop;
using IMAPI2.MediaItem;
using System.Linq;
using CD_Burn.Resources;

namespace CD_Burn
{
    public partial class Form1 : Form
    {
        private const string ClientName = "CD Burn";

        private DevicesManager devicesManger;
        private FileManager filesManager;

        private bool _isBurning;
        private bool _isFormatting;
        private bool _closeMedia;
        private bool _ejectMedia;

        public Form1()
        {
            InitializeComponent();
            devicesManger = new DevicesManager();
            filesManager = new FileManager(backgroundBurnWorker);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            devicesComboBox.Items.AddRange(devicesManger.DevicesList.Cast<object>().ToArray());
            if (devicesComboBox.Items.Count > 0)
            {
                devicesComboBox.SelectedIndex = 0;
            }

            DateTime now = DateTime.Now;
            textBoxLabel.Text = now.Year + "_" + now.Month + "_" + now.Day;

            labelStatusText.Text = string.Empty;
            labelFormatStatusText.Text = string.Empty;

            UpdateCapacity(devicesComboBox.SelectedIndex);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            devicesManger.ReleaseDiscRecorderItems();
        }

        private void DevicesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (devicesComboBox.SelectedIndex == -1)
            {
                return;
            }

            var discRecorder = devicesManger.DevicesList[devicesComboBox.SelectedIndex];

            if (!devicesManger.RecorderIsSupported(discRecorder))
            {
                MessageBox.Show("Recorder not supported", ClientName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void DevicesComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = DevicesManager.GetDevicePath((IDiscRecorder2)e.ListItem);
        }

        private void ButtonDetectMedia_Click(object sender, EventArgs e)
        {
            if (devicesComboBox.SelectedIndex == -1)
            {
                return;
            }

            labelMediaType.Text = devicesManger.GetNameOfCurrentMedia(devicesComboBox.SelectedIndex);
            UpdateCapacity(devicesComboBox.SelectedIndex);
        }

        // Updates the capacity progressbar
        private void UpdateCapacity(int index)
        {
            long totalDiscSize = devicesManger.GetTotalDiscSizeOFCurrentMedia(index);

            if (totalDiscSize == 0)
            {
                labelTotalSize.Text = "0MB";
                return;
            }

            labelTotalSize.Text = totalDiscSize < 1000000000 ?
                string.Format("{0}MB", totalDiscSize / 1000000) :
                string.Format("{0:F2}GB", (float)totalDiscSize / 1000000000.0);


            if (filesManager.GetFilesSize() == 0)
            {
                progressBarCapacity.Value = 0;
                progressBarCapacity.ForeColor = SystemColors.Highlight;
            }
            else
            {
                var percent = (int)((filesManager.GetFilesSize() * 100) / totalDiscSize);
                if (percent > 100)
                {
                    progressBarCapacity.Value = 100;
                    progressBarCapacity.ForeColor = Color.Red;
                }
                else
                {
                    progressBarCapacity.Value = percent;
                    progressBarCapacity.ForeColor = SystemColors.Highlight;
                }
            }
        }

        // User clicked the "Burn" button
        private void ButtonBurn_Click(object sender, EventArgs e)
        {
            if (devicesComboBox.SelectedIndex == -1)
            {
                return;
            }

            if (_isBurning)
            {
                buttonBurn.Enabled = false;
                backgroundBurnWorker.CancelAsync();
            }
            else
            {
                _isBurning = true;
                _closeMedia = checkBoxCloseMedia.Checked;
                _ejectMedia = checkBoxEject.Checked;

                EnableBurnUI(false);

                var discRecorder = devicesManger.DevicesList[devicesComboBox.SelectedIndex];
                filesManager.BurnData.uniqueRecorderId = discRecorder.ActiveDiscRecorder;

                backgroundBurnWorker.RunWorkerAsync(filesManager.BurnData);
            }
        }

        /// The thread that does the burning of the media
        private void BackgroundBurnWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            MsftDiscRecorder2 discRecorder = null;
            MsftDiscFormat2Data discFormatData = null;

            try
            {
                //
                // Create and initialize the IDiscRecorder2 object
                //
                discRecorder = new MsftDiscRecorder2();
                var burnData = (BurnData)e.Argument;
                discRecorder.InitializeDiscRecorder(burnData.uniqueRecorderId);

                //
                // Create and initialize the IDiscFormat2Data
                //
                discFormatData = new MsftDiscFormat2Data
                {
                    Recorder = discRecorder,
                    ClientName = ClientName,
                    ForceMediaToBeClosed = _closeMedia
                };

                //
                // Set the verification level
                //
                var burnVerification = (IBurnVerification)discFormatData;
                burnVerification.BurnVerificationLevel = IMAPI_BURN_VERIFICATION_LEVEL.IMAPI_BURN_VERIFICATION_NONE;

                //
                // Check if media is blank, (for RW media)
                //
                object[] multisessionInterfaces = null;
                if (!discFormatData.MediaHeuristicallyBlank)
                {
                    multisessionInterfaces = discFormatData.MultisessionInterfaces;
                }

                //
                // Create the file system
                //
                IStream fileSystem;
                if (!filesManager.CreateMediaFileSystem(discRecorder, multisessionInterfaces, out fileSystem, textBoxLabel.Text, listBoxFiles.Items))
                {
                    e.Result = -1;
                    return;
                }

                //
                // add the Update event handler
                //
                discFormatData.Update += DiscFormatData_Update;

                //
                // Write the data here
                //
                try
                {
                    discFormatData.Write(fileSystem);
                    e.Result = 0;
                }
                catch (COMException ex)
                {
                    e.Result = ex.ErrorCode;
                    MessageBox.Show(ex.Message, "IDiscFormat2Data.Write failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    if (fileSystem != null)
                    {
                        Marshal.FinalReleaseComObject(fileSystem);
                    }
                }

                //
                // remove the Update event handler
                //
                discFormatData.Update -= DiscFormatData_Update;

                if (_ejectMedia)
                {
                    discRecorder.EjectMedia();
                }
            }
            catch (COMException exception)
            {
                //
                // If anything happens during the format, show the message
                //
                MessageBox.Show(exception.Message);
                e.Result = exception.ErrorCode;
            }
            finally
            {
                if (discRecorder != null)
                {
                    Marshal.ReleaseComObject(discRecorder);
                }

                if (discFormatData != null)
                {
                    Marshal.ReleaseComObject(discFormatData);
                }
            }
        }

        void DiscFormatData_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress)
        {
            //
            // Check if we've cancelled
            //
            if (backgroundBurnWorker.CancellationPending)
            {
                var format2Data = (IDiscFormat2Data)sender;
                format2Data.CancelWrite();
                return;
            }

            var eventArgs = (IDiscFormat2DataEventArgs)progress;

            filesManager.InitBurnDate(eventArgs);

            //
            // Report back to the UI
            //
            backgroundBurnWorker.ReportProgress(0, filesManager.BurnData);
        }


        /// Completed the "Burn" thread
        private void BackgroundBurnWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            labelStatusText.Text = (int)e.Result == 0 ? "Finished Burning Disc!" : "Error Burning Disc!";
            statusProgressBar.Value = 0;

            _isBurning = false;
            EnableBurnUI(true);
            buttonBurn.Enabled = true;
        }

        /// Enables/Disables the "Burn" User Interface
        void EnableBurnUI(bool enable)
        {
            buttonBurn.Text = enable ? "&Burn" : "&Cancel";
            buttonDetectMedia.Enabled = enable;

            devicesComboBox.Enabled = enable;
            listBoxFiles.Enabled = enable;

            buttonAddFiles.Enabled = enable;
            buttonAddFolders.Enabled = enable;
            buttonRemoveFiles.Enabled = enable;
            checkBoxEject.Enabled = enable;
            checkBoxCloseMedia.Enabled = enable;
            textBoxLabel.Enabled = enable;
        }


        /// Event receives notification from the Burn thread of an event
        private void BackgroundBurnWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //int percent = e.ProgressPercentage;
            var burnData = (BurnData)e.UserState;

            if (burnData.task == BURN_MEDIA_TASK.BURN_MEDIA_TASK_FILE_SYSTEM)
            {
                labelStatusText.Text = burnData.statusMessage;
            }
            else if (burnData.task == BURN_MEDIA_TASK.BURN_MEDIA_TASK_WRITING)
            {
                switch (burnData.currentAction)
                {
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_VALIDATING_MEDIA:
                        labelStatusText.Text = "Validating current media...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FORMATTING_MEDIA:
                        labelStatusText.Text = "Formatting media...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_INITIALIZING_HARDWARE:
                        labelStatusText.Text = "Initializing hardware...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_CALIBRATING_POWER:
                        labelStatusText.Text = "Optimizing laser intensity...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_WRITING_DATA:
                        long writtenSectors = burnData.lastWrittenLba - burnData.startLba;

                        if (writtenSectors > 0 && burnData.sectorCount > 0)
                        {
                            var percent = (int)((100 * writtenSectors) / burnData.sectorCount);
                            labelStatusText.Text = string.Format("Progress: {0}%", percent);
                            statusProgressBar.Value = percent;
                        }
                        else
                        {
                            labelStatusText.Text = "Progress 0%";
                            statusProgressBar.Value = 0;
                        }
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FINALIZATION:
                        labelStatusText.Text = "Finalizing writing...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_COMPLETED:
                        labelStatusText.Text = "Completed!";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_VERIFYING:
                        labelStatusText.Text = "Verifying";
                        break;
                }
            }
        }

        /// Enable the Burn Button if items in the file listbox
        private void EnableBurnButton()
        {
            buttonBurn.Enabled = (listBoxFiles.Items.Count > 0);
        }

        /// The user has clicked the "Format" button
        private void ButtonFormat_Click(object sender, EventArgs e)
        {
            if (devicesComboBox.SelectedIndex == -1)
            {
                return;
            }

            _isFormatting = true;
            EnableFormatUI(false);

            var discRecorder = devicesManger.DevicesList[devicesComboBox.SelectedIndex];
            backgroundFormatWorker.RunWorkerAsync(discRecorder.ActiveDiscRecorder);
        }

        /// Enables/Disables the "Burn" User Interface
        void EnableFormatUI(bool enable)
        {
            buttonFormat.Enabled = enable;
            checkBoxEjectFormat.Enabled = enable;
            checkBoxQuickFormat.Enabled = enable;
        }

        /// Worker thread that Formats the Disc
        private void BackgroundFormatWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            MsftDiscRecorder2 discRecorder = null;
            MsftDiscFormat2Erase discFormatErase = null;

            try
            {
                //
                // Create and initialize the IDiscRecorder2
                //
                discRecorder = new MsftDiscRecorder2();
                var activeDiscRecorder = (string)e.Argument;
                discRecorder.InitializeDiscRecorder(activeDiscRecorder);

                //
                // Create the IDiscFormat2Erase and set properties
                //
                discFormatErase = new MsftDiscFormat2Erase
                {
                    Recorder = discRecorder,
                    ClientName = ClientName,
                    FullErase = !checkBoxQuickFormat.Checked
                };

                //
                // Setup the Update progress event handler
                //
                discFormatErase.Update += DiscFormatErase_Update;

                //
                // Erase the media here
                //
                try
                {
                    discFormatErase.EraseMedia();
                    e.Result = 0;
                }
                catch (COMException ex)
                {
                    e.Result = ex.ErrorCode;
                    MessageBox.Show(ex.Message, "IDiscFormat2.EraseMedia failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                //
                // Remove the Update progress event handler
                //
                discFormatErase.Update -= DiscFormatErase_Update;

                //
                // Eject the media 
                //
                if (checkBoxEjectFormat.Checked)
                {
                    discRecorder.EjectMedia();
                }

            }
            catch (COMException exception)
            {
                //
                // If anything happens during the format, show the message
                //
                MessageBox.Show(exception.Message);
            }
            finally
            {
                if (discRecorder != null)
                {
                    Marshal.ReleaseComObject(discRecorder);
                }

                if (discFormatErase != null)
                {
                    Marshal.ReleaseComObject(discFormatErase);
                }
            }
        }

        /// Event Handler for the Erase Progress Updates
        void DiscFormatErase_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, int elapsedSeconds, int estimatedTotalSeconds)
        {
            var percent = elapsedSeconds * 100 / estimatedTotalSeconds;
            //
            // Report back to the UI
            //
            backgroundFormatWorker.ReportProgress(percent);
        }

        private void BackgroundFormatWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            labelFormatStatusText.Text = string.Format("Formatting {0}%...", e.ProgressPercentage);
            formatProgressBar.Value = e.ProgressPercentage;
        }

        private void BackgroundFormatWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            labelFormatStatusText.Text = (int)e.Result == 0 ?
                "Finished Formatting Disc!" : "Error Formatting Disc!";

            formatProgressBar.Value = 0;

            _isFormatting = false;
            EnableFormatUI(true);
        }

        // Adds a file to the burn list
        private void ButtonAddFiles_Click(object sender, EventArgs e)
        {
            filesManager.AddedFile();

            listBoxFiles.Items.Clear();
            listBoxFiles.Items.AddRange(filesManager.Files.Cast<object>().ToArray());

            UpdateCapacity(devicesComboBox.SelectedIndex);
            EnableBurnButton();
        }

        // Adds a folder to the burn list
        private void ButtonAddFolders_Click(object sender, EventArgs e)
        {
            filesManager.AddedFolder();
            listBoxFiles.Items.Clear();
            listBoxFiles.Items.AddRange(filesManager.Files.Cast<object>().ToArray());

            UpdateCapacity(devicesComboBox.SelectedIndex);
            EnableBurnButton();
        }

        // User wants to remove a file or folder
        private void ButtonRemoveFiles_Click(object sender, EventArgs e)
        {
            filesManager.RemoveFileFolder(listBoxFiles.SelectedIndex);
            listBoxFiles.Items.Clear();
            listBoxFiles.Items.AddRange(filesManager.Files.Cast<object>().ToArray());

            UpdateCapacity(devicesComboBox.SelectedIndex);
            EnableBurnButton();
        }

        // The user has selected a file or folder
        private void ListBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonRemoveFiles.Enabled = (listBoxFiles.SelectedIndex != -1);
        }

        private void ListBoxFiles_DrawItem(object sender, DrawItemEventArgs e)
        {
            try { 
                IMediaItem mediaItem = filesManager.Files[e.Index];
                if (mediaItem == null)
                {
                    return;
                }

                e.DrawBackground();

                if ((e.State & DrawItemState.Focus) != 0)
                {
                    e.DrawFocusRectangle();
                }

                if (mediaItem.FileIconImage != null)
                {
                    e.Graphics.DrawImage(mediaItem.FileIconImage, new Rectangle(4, e.Bounds.Y + 4, 16, 16));
                }

                var rectF = new RectangleF(e.Bounds.X + 24, e.Bounds.Y,
                    e.Bounds.Width - 24, e.Bounds.Height);

                var font = new Font(FontFamily.GenericSansSerif, 11);

                var stringFormat = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                e.Graphics.DrawString(mediaItem.ToString(), font, new SolidBrush(e.ForeColor),
                    rectF, stringFormat);
            }
            catch (ArgumentOutOfRangeException ex)
            { }
        }

        // Called when user selects a new tab
        private void TabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (_isBurning || _isFormatting)
            {
                e.Cancel = true;
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();

                notifyIcon1.BalloonTipTitle = "Программа свёрнута в трей";
                notifyIcon1.BalloonTipText = "Обратите внимание что программа была спрятана в трей и продолжит свою работу.";
                notifyIcon1.ShowBalloonTip(5000);
            }
        }
    }
}