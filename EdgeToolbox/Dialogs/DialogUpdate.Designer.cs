using DarkUI.Controls;

namespace EdgeToolbox {
    partial class DialogUpdate {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogUpdate));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.progBar2 = new System.Windows.Forms.ProgressBar();
            this.fileLabel = new DarkUI.Controls.DarkLabel();
            this.progBar = new System.Windows.Forms.ProgressBar();
            this.sActL = new DarkUI.Controls.DarkLabel();
            this.pActL = new DarkUI.Controls.DarkLabel();
            this.UpdaterBG = new System.ComponentModel.BackgroundWorker();
            this.UpdateDownloader = new EdgeDeviceLibrary.FileTransferDownload();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.progBar2);
            this.pnlMain.Controls.Add(this.fileLabel);
            this.pnlMain.Controls.Add(this.progBar);
            this.pnlMain.Controls.Add(this.sActL);
            this.pnlMain.Controls.Add(this.pActL);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(15, 15, 15, 5);
            this.pnlMain.Size = new System.Drawing.Size(367, 122);
            this.pnlMain.TabIndex = 2;
            // 
            // progBar2
            // 
            this.progBar2.Dock = System.Windows.Forms.DockStyle.Top;
            this.progBar2.Location = new System.Drawing.Point(15, 89);
            this.progBar2.Name = "progBar2";
            this.progBar2.Size = new System.Drawing.Size(337, 23);
            this.progBar2.TabIndex = 4;
            // 
            // fileLabel
            // 
            this.fileLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.fileLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.fileLabel.Location = new System.Drawing.Point(15, 71);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(337, 18);
            this.fileLabel.TabIndex = 3;
            this.fileLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // progBar
            // 
            this.progBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.progBar.Location = new System.Drawing.Point(15, 49);
            this.progBar.Name = "progBar";
            this.progBar.Size = new System.Drawing.Size(337, 22);
            this.progBar.TabIndex = 0;
            // 
            // sActL
            // 
            this.sActL.Dock = System.Windows.Forms.DockStyle.Top;
            this.sActL.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.sActL.Location = new System.Drawing.Point(15, 32);
            this.sActL.Name = "sActL";
            this.sActL.Size = new System.Drawing.Size(337, 17);
            this.sActL.TabIndex = 2;
            this.sActL.Text = "Secondary Action Label";
            this.sActL.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pActL
            // 
            this.pActL.Dock = System.Windows.Forms.DockStyle.Top;
            this.pActL.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.pActL.Location = new System.Drawing.Point(15, 15);
            this.pActL.Name = "pActL";
            this.pActL.Size = new System.Drawing.Size(337, 17);
            this.pActL.TabIndex = 1;
            this.pActL.Text = "Primary Action Label";
            this.pActL.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // UpdaterBG
            // 
            this.UpdaterBG.DoWork += new System.ComponentModel.DoWorkEventHandler(this.UpdaterBG_DoWork);
            this.UpdaterBG.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.UpdaterBG_RunWorkerCompleted);
            // 
            // UpdateDownloader
            // 
            this.UpdateDownloader.WorkerReportsProgress = true;
            this.UpdateDownloader.WorkerSupportsCancellation = true;
            this.UpdateDownloader.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.UpdateDownloader_ProgressChanged);
            // 
            // DialogUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 167);
            this.Controls.Add(this.pnlMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogUpdate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Updating Device...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DialogUpdate_FormClosing);
            this.Controls.SetChildIndex(this.pnlMain, 0);
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.ProgressBar progBar;
        private DarkLabel pActL;
        private DarkLabel sActL;
        private System.Windows.Forms.ProgressBar progBar2;
        private DarkLabel fileLabel;
        private System.ComponentModel.BackgroundWorker UpdaterBG;
        private EdgeDeviceLibrary.FileTransferDownload UpdateDownloader;
    }
}