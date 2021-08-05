using DarkUI.Config;
using DarkUI.Controls;
using DarkUI.Docking;

namespace EdgeToolbox {
    partial class DockFileSystem {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.fileView = new DarkUI.Controls.DarkTreeView();
            this.refreshBtn = new DarkUI.Controls.DarkButton();
            this.LoadListWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // fileView
            // 
            this.fileView.Dock = System.Windows.Forms.DockStyle.Top;
            this.fileView.Location = new System.Drawing.Point(0, 25);
            this.fileView.MaxDragChange = 20;
            this.fileView.Name = "fileView";
            this.fileView.ShowIcons = true;
            this.fileView.Size = new System.Drawing.Size(280, 396);
            this.fileView.TabIndex = 0;
            this.fileView.Text = "fileView";
            this.fileView.SelectedNodesChanged += new System.EventHandler(this.fileView_SelectedNodesChanged);
            // 
            // refreshBtn
            // 
            this.refreshBtn.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.refreshBtn.Location = new System.Drawing.Point(0, 427);
            this.refreshBtn.Name = "refreshBtn";
            this.refreshBtn.Padding = new System.Windows.Forms.Padding(5);
            this.refreshBtn.Size = new System.Drawing.Size(280, 23);
            this.refreshBtn.TabIndex = 1;
            this.refreshBtn.Text = "Refresh";
            this.refreshBtn.Click += new System.EventHandler(this.refreshBtn_Click);
            // 
            // LoadListWorker
            // 
            this.LoadListWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.LoadListWorker_DoWork);
            this.LoadListWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.LoadListWorker_RunWorkerCompleted);
            // 
            // DockFileSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.refreshBtn);
            this.Controls.Add(this.fileView);
            this.DefaultDockArea = DarkUI.Docking.DarkDockArea.Left;
            this.DockText = "File System";
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = global::EdgeToolbox.Properties.Resources.files;
            this.Name = "DockFileSystem";
            this.SerializationKey = "DockProject";
            this.Size = new System.Drawing.Size(280, 450);
            this.ResumeLayout(false);

        }

        #endregion

        private DarkTreeView fileView;
        private DarkButton refreshBtn;
        private System.ComponentModel.BackgroundWorker LoadListWorker;
    }
}
