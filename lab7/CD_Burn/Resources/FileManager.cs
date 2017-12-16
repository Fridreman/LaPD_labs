using IMAPI2.Interop;
using IMAPI2.MediaItem;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;

namespace CD_Burn.Resources
{
    class FileManager
    {

        private List<IMediaItem> files;

        private BurnData _burnData;
        private BackgroundWorker backgroundBurnWorker;

        public List<IMediaItem> Files
        {
            get { return this.files; }
        }
        public BurnData BurnData
        {
            get { return this._burnData; }
        }


        public FileManager(BackgroundWorker backgroundBurnWorke)
        {
            _burnData = new BurnData();
            this.backgroundBurnWorker = backgroundBurnWorke;
            this.files = new List<IMediaItem>();
        }

        public bool CreateMediaFileSystem(IDiscRecorder2 discRecorder, object[] multisessionInterfaces, out IStream dataStream, string volumeName,
            ListBox.ObjectCollection listFiles)
        {
            MsftFileSystemImage fileSystemImage = null;
            try
            {
                fileSystemImage = new MsftFileSystemImage();
                fileSystemImage.ChooseImageDefaults(discRecorder);
                fileSystemImage.FileSystemsToCreate =
                    FsiFileSystems.FsiFileSystemJoliet | FsiFileSystems.FsiFileSystemISO9660;
                fileSystemImage.VolumeName = volumeName;

                fileSystemImage.Update += fileSystemImage_Update;

                //
                // If multisessions, then import previous sessions
                //
                if (multisessionInterfaces != null)
                {
                    fileSystemImage.MultisessionInterfaces = multisessionInterfaces;
                    fileSystemImage.ImportFileSystem();
                }

                //
                // Get the image root
                //
                IFsiDirectoryItem rootItem = fileSystemImage.Root;

                //
                // Add Files and Directories to File System Image
                //
                foreach (IMediaItem mediaItem in listFiles)
                {
                    //
                    // Check if we've cancelled
                    //
                    if (backgroundBurnWorker.CancellationPending)
                    {
                        break;
                    }

                    //
                    // Add to File System
                    //
                    mediaItem.AddToFileSystem(rootItem);
                }

                fileSystemImage.Update -= fileSystemImage_Update;

                //
                // did we cancel?
                //
                if (backgroundBurnWorker.CancellationPending)
                {
                    dataStream = null;
                    return false;
                }

                dataStream = fileSystemImage.CreateResultImage().ImageStream;
            }
            catch (COMException)
            {
                dataStream = null;
                return false;
            }
            finally
            {
                if (fileSystemImage != null)
                {
                    Marshal.ReleaseComObject(fileSystemImage);
                }
            }

            return true;
        }

        // Event Handler for File System Progress Updates
        void fileSystemImage_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender,
            [In, MarshalAs(UnmanagedType.BStr)]string currentFile, [In] int copiedSectors, [In] int totalSectors)
        {
            var percentProgress = 0;
            if (copiedSectors > 0 && totalSectors > 0)
            {
                percentProgress = (copiedSectors * 100) / totalSectors;
            }

            if (!string.IsNullOrEmpty(currentFile))
            {
                var fileInfo = new FileInfo(currentFile);
                _burnData.statusMessage = "Adding \"" + fileInfo.Name + "\" to image...";

                //
                // report back to the ui
                //
                _burnData.task = BURN_MEDIA_TASK.BURN_MEDIA_TASK_FILE_SYSTEM;
                backgroundBurnWorker.ReportProgress(percentProgress, _burnData);
            }

        }

        public void InitBurnDate(IDiscFormat2DataEventArgs eventArgs)
        {
            _burnData.task = BURN_MEDIA_TASK.BURN_MEDIA_TASK_WRITING;

            // IDiscFormat2DataEventArgs Interface
            _burnData.elapsedTime = eventArgs.ElapsedTime;
            _burnData.remainingTime = eventArgs.RemainingTime;
            _burnData.totalTime = eventArgs.TotalTime;

            // IWriteEngine2EventArgs Interface
            _burnData.currentAction = eventArgs.CurrentAction;
            _burnData.startLba = eventArgs.StartLba;
            _burnData.sectorCount = eventArgs.SectorCount;
            _burnData.lastReadLba = eventArgs.LastReadLba;
            _burnData.lastWrittenLba = eventArgs.LastWrittenLba;
            _burnData.totalSystemBuffer = eventArgs.TotalSystemBuffer;
            _burnData.usedSystemBuffer = eventArgs.UsedSystemBuffer;
            _burnData.freeSystemBuffer = eventArgs.FreeSystemBuffer;
        }

        public void AddedFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.files.Add(new FileItem(openFileDialog.FileName));
            }
        }

        public void AddedFolder()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.files.Add(new DirectoryItem(folderBrowserDialog.SelectedPath));
            }
        }

        public void RemoveFileFolder(int index)
        {
            IMediaItem mediaItem = files[index];
            if (mediaItem == null)
                return;

            files.Remove(mediaItem);
        }

        public long GetFilesSize()
        {
            long answer = 0;
            foreach (IMediaItem mediaItem in files)
            {
                answer += mediaItem.SizeOnDisc;
            }

            return answer;
        }
    }
}
