namespace Getting_USB_Devices
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
            this.components = new System.ComponentModel.Container();
            this.OutputGrid = new System.Windows.Forms.DataGridView();
            this.RemoveB = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.Message = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.OutputGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // OutputGrid
            // 
            this.OutputGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.OutputGrid.Location = new System.Drawing.Point(12, 12);
            this.OutputGrid.Name = "OutputGrid";
            this.OutputGrid.Size = new System.Drawing.Size(444, 150);
            this.OutputGrid.TabIndex = 0;
            this.OutputGrid.SelectionChanged += new System.EventHandler(this.OutputGrid_SelectionChanged);
            // 
            // RemoveB
            // 
            this.RemoveB.Location = new System.Drawing.Point(356, 180);
            this.RemoveB.Name = "RemoveB";
            this.RemoveB.Size = new System.Drawing.Size(100, 23);
            this.RemoveB.TabIndex = 1;
            this.RemoveB.Text = "Remove disk";
            this.RemoveB.UseVisualStyleBackColor = true;
            this.RemoveB.Click += new System.EventHandler(this.RemoveB_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 2;
            // 
            // Message
            // 
            this.Message.AutoSize = true;
            this.Message.Location = new System.Drawing.Point(13, 179);
            this.Message.Name = "Message";
            this.Message.Size = new System.Drawing.Size(0, 13);
            this.Message.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 211);
            this.Controls.Add(this.Message);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RemoveB);
            this.Controls.Add(this.OutputGrid);
            this.Name = "Form1";
            this.Text = "USB devices";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.OutputGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView OutputGrid;
        private System.Windows.Forms.Button RemoveB;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Message;
    }
}

