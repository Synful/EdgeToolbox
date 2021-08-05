using EdgeToolbox.Properties;

namespace EdgeToolbox {
    partial class MainForm {
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
            this.menuMain = new DarkUI.Controls.DarkMenuStrip();
            this.deviceTools = new System.Windows.Forms.ToolStripMenuItem();
            this.dt_Wait = new System.Windows.Forms.ToolStripMenuItem();
            this.dt_Connect = new System.Windows.Forms.ToolStripMenuItem();
            this.dt_Info = new System.Windows.Forms.ToolStripMenuItem();
            this.dt_Update = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.stripMain = new DarkUI.Controls.DarkStatusStrip();
            this.dStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.DockPanel = new DarkUI.Docking.DarkDockPanel();
            this.ConnectionWorker = new System.ComponentModel.BackgroundWorker();
            this.DeviceWaiter = new System.ComponentModel.BackgroundWorker();
            this.dt_DumpFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMain.SuspendLayout();
            this.stripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMain
            // 
            this.menuMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.menuMain.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deviceTools,
            this.aboutBtn});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Padding = new System.Windows.Forms.Padding(3, 2, 0, 2);
            this.menuMain.Size = new System.Drawing.Size(992, 24);
            this.menuMain.TabIndex = 0;
            this.menuMain.Text = "darkMenuStrip1";
            // 
            // deviceTools
            // 
            this.deviceTools.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.deviceTools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.deviceTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dt_Wait,
            this.dt_Connect,
            this.dt_Info,
            this.dt_DumpFiles,
            this.dt_Update});
            this.deviceTools.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.deviceTools.Name = "deviceTools";
            this.deviceTools.Size = new System.Drawing.Size(54, 20);
            this.deviceTools.Text = "Device";
            // 
            // dt_Wait
            // 
            this.dt_Wait.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.dt_Wait.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.dt_Wait.Name = "dt_Wait";
            this.dt_Wait.Size = new System.Drawing.Size(180, 22);
            this.dt_Wait.Text = "Connect Device...";
            // 
            // dt_Connect
            // 
            this.dt_Connect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.dt_Connect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.dt_Connect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.dt_Connect.Name = "dt_Connect";
            this.dt_Connect.Size = new System.Drawing.Size(180, 22);
            this.dt_Connect.Text = "Connect";
            this.dt_Connect.Visible = false;
            this.dt_Connect.Click += new System.EventHandler(this.dt_Connect_Click);
            // 
            // dt_Info
            // 
            this.dt_Info.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.dt_Info.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.dt_Info.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.dt_Info.Name = "dt_Info";
            this.dt_Info.Size = new System.Drawing.Size(180, 22);
            this.dt_Info.Text = "Info";
            this.dt_Info.Visible = false;
            this.dt_Info.Click += new System.EventHandler(this.dt_Info_Click);
            // 
            // dt_Update
            // 
            this.dt_Update.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.dt_Update.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.dt_Update.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.dt_Update.Name = "dt_Update";
            this.dt_Update.Size = new System.Drawing.Size(180, 22);
            this.dt_Update.Text = "Update";
            this.dt_Update.Visible = false;
            this.dt_Update.Click += new System.EventHandler(this.dt_Update_Click);
            // 
            // aboutBtn
            // 
            this.aboutBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.aboutBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.Size = new System.Drawing.Size(52, 20);
            this.aboutBtn.Text = "About";
            this.aboutBtn.Click += new System.EventHandler(this.aboutBtn_Click);
            // 
            // stripMain
            // 
            this.stripMain.AutoSize = false;
            this.stripMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.stripMain.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.stripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dStatusLabel});
            this.stripMain.Location = new System.Drawing.Point(0, 543);
            this.stripMain.Name = "stripMain";
            this.stripMain.Padding = new System.Windows.Forms.Padding(0, 5, 0, 3);
            this.stripMain.Size = new System.Drawing.Size(992, 24);
            this.stripMain.SizingGrip = false;
            this.stripMain.TabIndex = 3;
            this.stripMain.Text = "darkStatusStrip1";
            // 
            // dStatusLabel
            // 
            this.dStatusLabel.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.dStatusLabel.Name = "dStatusLabel";
            this.dStatusLabel.Size = new System.Drawing.Size(48, 16);
            this.dStatusLabel.Text = "Waiting";
            // 
            // DockPanel
            // 
            this.DockPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.DockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DockPanel.Location = new System.Drawing.Point(0, 24);
            this.DockPanel.Name = "DockPanel";
            this.DockPanel.Size = new System.Drawing.Size(992, 519);
            this.DockPanel.TabIndex = 4;
            // 
            // ConnectionWorker
            // 
            this.ConnectionWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ConnectionWorker_DoWork);
            this.ConnectionWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ConnectionWorker_RunWorkerCompleted);
            // 
            // DeviceWaiter
            // 
            this.DeviceWaiter.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DeviceWaiter_DoWork);
            // 
            // dt_DumpFiles
            // 
            this.dt_DumpFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.dt_DumpFiles.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.dt_DumpFiles.Name = "dt_DumpFiles";
            this.dt_DumpFiles.Size = new System.Drawing.Size(180, 22);
            this.dt_DumpFiles.Text = "Dump All Files";
            this.dt_DumpFiles.Click += new System.EventHandler(this.dt_DumpFiles_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 567);
            this.Controls.Add(this.DockPanel);
            this.Controls.Add(this.stripMain);
            this.Controls.Add(this.menuMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuMain;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Edge Toolbox";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.stripMain.ResumeLayout(false);
            this.stripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DarkUI.Controls.DarkMenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem deviceTools;
        private System.Windows.Forms.ToolStripMenuItem aboutBtn;
        private DarkUI.Controls.DarkStatusStrip stripMain;
        private System.Windows.Forms.ToolStripStatusLabel dStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem dt_Connect;
        private System.ComponentModel.BackgroundWorker ConnectionWorker;
        private System.Windows.Forms.ToolStripMenuItem dt_Info;
        private System.Windows.Forms.ToolStripMenuItem dt_Update;
        public DarkUI.Docking.DarkDockPanel DockPanel;
        private System.ComponentModel.BackgroundWorker DeviceWaiter;
        private System.Windows.Forms.ToolStripMenuItem dt_Wait;
        private System.Windows.Forms.ToolStripMenuItem dt_DumpFiles;
    }
}

