namespace Getting_Win_Devices
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DeviceInfo = new System.Windows.Forms.TextBox();
            this.EnableB = new System.Windows.Forms.Button();
            this.DisableB = new System.Windows.Forms.Button();
            this.DeviceList = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // DeviceInfo
            // 
            this.DeviceInfo.Location = new System.Drawing.Point(363, 8);
            this.DeviceInfo.Multiline = true;
            this.DeviceInfo.Name = "DeviceInfo";
            this.DeviceInfo.Size = new System.Drawing.Size(351, 203);
            this.DeviceInfo.TabIndex = 1;
            // 
            // EnableB
            // 
            this.EnableB.Location = new System.Drawing.Point(363, 242);
            this.EnableB.Name = "EnableB";
            this.EnableB.Size = new System.Drawing.Size(75, 32);
            this.EnableB.TabIndex = 2;
            this.EnableB.Text = "Enable";
            this.EnableB.UseVisualStyleBackColor = true;
            this.EnableB.Click += new System.EventHandler(this.EnableB_Click);
            // 
            // DisableB
            // 
            this.DisableB.Location = new System.Drawing.Point(464, 242);
            this.DisableB.Name = "DisableB";
            this.DisableB.Size = new System.Drawing.Size(75, 32);
            this.DisableB.TabIndex = 3;
            this.DisableB.Text = "Disable";
            this.DisableB.UseVisualStyleBackColor = true;
            this.DisableB.Click += new System.EventHandler(this.DisableB_Click);
            // 
            // DeviceList
            // 
            this.DeviceList.FullRowSelect = true;
            this.DeviceList.Location = new System.Drawing.Point(12, 8);
            this.DeviceList.MultiSelect = false;
            this.DeviceList.Name = "DeviceList";
            this.DeviceList.Size = new System.Drawing.Size(337, 266);
            this.DeviceList.TabIndex = 4;
            this.DeviceList.UseCompatibleStateImageBehavior = false;
            this.DeviceList.View = System.Windows.Forms.View.Tile;
            this.DeviceList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DeviceList_MouseClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 288);
            this.Controls.Add(this.DeviceList);
            this.Controls.Add(this.DisableB);
            this.Controls.Add(this.EnableB);
            this.Controls.Add(this.DeviceInfo);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox DeviceInfo;
        private System.Windows.Forms.Button EnableB;
        private System.Windows.Forms.Button DisableB;
        private System.Windows.Forms.ListView DeviceList;
    }
}

