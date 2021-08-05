using DarkUI.Controls;

namespace EdgeToolbox {
    partial class DialogFileDownloader {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogFileDownloader));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.progressTask = new System.Windows.Forms.ProgressBar();
            this.labelFileProg = new DarkUI.Controls.DarkLabel();
            this.labelFilename = new DarkUI.Controls.DarkLabel();
            this.progressOverall = new System.Windows.Forms.ProgressBar();
            this.labelOverallTask = new DarkUI.Controls.DarkLabel();
            this.DownloaderBG = new System.ComponentModel.BackgroundWorker();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.progressTask);
            this.pnlMain.Controls.Add(this.labelFileProg);
            this.pnlMain.Controls.Add(this.labelFilename);
            this.pnlMain.Controls.Add(this.progressOverall);
            this.pnlMain.Controls.Add(this.labelOverallTask);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(15, 15, 15, 5);
            this.pnlMain.Size = new System.Drawing.Size(367, 122);
            this.pnlMain.TabIndex = 2;
            // 
            // progressTask
            // 
            this.progressTask.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressTask.Location = new System.Drawing.Point(15, 89);
            this.progressTask.Name = "progressTask";
            this.progressTask.Size = new System.Drawing.Size(337, 23);
            this.progressTask.TabIndex = 4;
            // 
            // labelFileProg
            // 
            this.labelFileProg.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelFileProg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.labelFileProg.Location = new System.Drawing.Point(15, 71);
            this.labelFileProg.Name = "labelFileProg";
            this.labelFileProg.Size = new System.Drawing.Size(337, 18);
            this.labelFileProg.TabIndex = 3;
            this.labelFileProg.Text = "File Download Progress";
            this.labelFileProg.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelFilename
            // 
            this.labelFilename.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelFilename.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.labelFilename.Location = new System.Drawing.Point(15, 54);
            this.labelFilename.Name = "labelFilename";
            this.labelFilename.Size = new System.Drawing.Size(337, 17);
            this.labelFilename.TabIndex = 2;
            this.labelFilename.Text = "Current File";
            this.labelFilename.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // progressOverall
            // 
            this.progressOverall.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressOverall.Location = new System.Drawing.Point(15, 32);
            this.progressOverall.Name = "progressOverall";
            this.progressOverall.Size = new System.Drawing.Size(337, 22);
            this.progressOverall.TabIndex = 0;
            // 
            // labelOverallTask
            // 
            this.labelOverallTask.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelOverallTask.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.labelOverallTask.Location = new System.Drawing.Point(15, 15);
            this.labelOverallTask.Name = "labelOverallTask";
            this.labelOverallTask.Size = new System.Drawing.Size(337, 17);
            this.labelOverallTask.TabIndex = 1;
            this.labelOverallTask.Text = "Overall Download Progress";
            this.labelOverallTask.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // DownloaderBG
            // 
            this.DownloaderBG.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DownloaderBG_DoWork);
            this.DownloaderBG.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.DownloaderBG_RunWorkerCompleted);
            // 
            // DialogFileDownloader
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
            this.Name = "DialogFileDownloader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Downloading Files...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DialogFileDownloader_FormClosing);
            this.Controls.SetChildIndex(this.pnlMain, 0);
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.ProgressBar progressOverall;
        private DarkLabel labelOverallTask;
        private DarkLabel labelFilename;
        private System.Windows.Forms.ProgressBar progressTask;
        private DarkLabel labelFileProg;
        private System.ComponentModel.BackgroundWorker DownloaderBG;
    }
}