namespace EdgeToolbox.Forms {
    partial class UpdateForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
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
            this.loaderBG = new System.ComponentModel.BackgroundWorker();
            this.calData = new System.Windows.Forms.DataGridView();
            this.firmData = new System.Windows.Forms.DataGridView();
            this.darkButton1 = new DarkUI.Controls.DarkButton();
            this.UpdateDownloader = new EdgeDeviceLibrary.FileTransferDownload();
            this.firmUpdateId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.firmUpdateType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.firmUpdateVer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.firmUpdateDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.calUpdateId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.calUpdateType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.calUpdateVer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.calUpdateDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tempBar = new System.Windows.Forms.ProgressBar();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.calData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.firmData)).BeginInit();
            this.SuspendLayout();
            // 
            // loaderBG
            // 
            this.loaderBG.DoWork += new System.ComponentModel.DoWorkEventHandler(this.loaderBG_DoWork);
            // 
            // calData
            // 
            this.calData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.calData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.calData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.calUpdateId,
            this.calUpdateType,
            this.calUpdateVer,
            this.calUpdateDesc});
            this.calData.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.calData.Location = new System.Drawing.Point(0, 355);
            this.calData.MultiSelect = false;
            this.calData.Name = "calData";
            this.calData.ReadOnly = true;
            this.calData.RowHeadersVisible = false;
            this.calData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.calData.Size = new System.Drawing.Size(1001, 327);
            this.calData.TabIndex = 1;
            // 
            // firmData
            // 
            this.firmData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.firmData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.firmData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.firmUpdateId,
            this.firmUpdateType,
            this.firmUpdateVer,
            this.firmUpdateDesc});
            this.firmData.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.firmData.Location = new System.Drawing.Point(0, 109);
            this.firmData.MultiSelect = false;
            this.firmData.Name = "firmData";
            this.firmData.ReadOnly = true;
            this.firmData.RowHeadersVisible = false;
            this.firmData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.firmData.Size = new System.Drawing.Size(1001, 246);
            this.firmData.TabIndex = 2;
            // 
            // darkButton1
            // 
            this.darkButton1.Location = new System.Drawing.Point(103, 32);
            this.darkButton1.Name = "darkButton1";
            this.darkButton1.Padding = new System.Windows.Forms.Padding(5);
            this.darkButton1.Size = new System.Drawing.Size(144, 36);
            this.darkButton1.TabIndex = 3;
            this.darkButton1.Text = "darkButton1";
            this.darkButton1.Click += new System.EventHandler(this.darkButton1_Click);
            // 
            // UpdateDownloader
            // 
            this.UpdateDownloader.WorkerReportsProgress = true;
            this.UpdateDownloader.WorkerSupportsCancellation = true;
            this.UpdateDownloader.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.UpdateDownloader_ProgressChanged);
            // 
            // firmUpdateId
            // 
            this.firmUpdateId.DataPropertyName = "id";
            this.firmUpdateId.HeaderText = "Id";
            this.firmUpdateId.Name = "firmUpdateId";
            this.firmUpdateId.ReadOnly = true;
            this.firmUpdateId.Width = 41;
            // 
            // firmUpdateType
            // 
            this.firmUpdateType.DataPropertyName = "type";
            this.firmUpdateType.HeaderText = "Type";
            this.firmUpdateType.Name = "firmUpdateType";
            this.firmUpdateType.ReadOnly = true;
            this.firmUpdateType.Width = 56;
            // 
            // firmUpdateVer
            // 
            this.firmUpdateVer.DataPropertyName = "version";
            this.firmUpdateVer.HeaderText = "Version";
            this.firmUpdateVer.Name = "firmUpdateVer";
            this.firmUpdateVer.ReadOnly = true;
            this.firmUpdateVer.Width = 67;
            // 
            // firmUpdateDesc
            // 
            this.firmUpdateDesc.DataPropertyName = "Description";
            this.firmUpdateDesc.HeaderText = "Description";
            this.firmUpdateDesc.Name = "firmUpdateDesc";
            this.firmUpdateDesc.ReadOnly = true;
            this.firmUpdateDesc.Width = 85;
            // 
            // calUpdateId
            // 
            this.calUpdateId.DataPropertyName = "id";
            this.calUpdateId.HeaderText = "Id";
            this.calUpdateId.Name = "calUpdateId";
            this.calUpdateId.ReadOnly = true;
            this.calUpdateId.Width = 41;
            // 
            // calUpdateType
            // 
            this.calUpdateType.DataPropertyName = "type";
            this.calUpdateType.HeaderText = "Type";
            this.calUpdateType.Name = "calUpdateType";
            this.calUpdateType.ReadOnly = true;
            this.calUpdateType.Width = 56;
            // 
            // calUpdateVer
            // 
            this.calUpdateVer.DataPropertyName = "version";
            this.calUpdateVer.HeaderText = "Version";
            this.calUpdateVer.Name = "calUpdateVer";
            this.calUpdateVer.ReadOnly = true;
            this.calUpdateVer.Width = 67;
            // 
            // calUpdateDesc
            // 
            this.calUpdateDesc.DataPropertyName = "description";
            this.calUpdateDesc.HeaderText = "Description";
            this.calUpdateDesc.Name = "calUpdateDesc";
            this.calUpdateDesc.ReadOnly = true;
            this.calUpdateDesc.Width = 85;
            // 
            // tempBar
            // 
            this.tempBar.Location = new System.Drawing.Point(382, 80);
            this.tempBar.Name = "tempBar";
            this.tempBar.Size = new System.Drawing.Size(304, 23);
            this.tempBar.TabIndex = 4;
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(500, 44);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(60, 13);
            this.darkLabel1.TabIndex = 5;
            this.darkLabel1.Text = "darkLabel1";
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(500, 64);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(60, 13);
            this.darkLabel2.TabIndex = 6;
            this.darkLabel2.Text = "darkLabel2";
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1001, 682);
            this.Controls.Add(this.darkLabel2);
            this.Controls.Add(this.darkLabel1);
            this.Controls.Add(this.tempBar);
            this.Controls.Add(this.darkButton1);
            this.Controls.Add(this.firmData);
            this.Controls.Add(this.calData);
            this.Name = "UpdateForm";
            this.Text = "UpdateForm";
            ((System.ComponentModel.ISupportInitialize)(this.calData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.firmData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EdgeDeviceLibrary.FileTransferDownload UpdateDownloader;
        private System.ComponentModel.BackgroundWorker loaderBG;
        private System.Windows.Forms.DataGridView calData;
        private System.Windows.Forms.DataGridView firmData;
        private DarkUI.Controls.DarkButton darkButton1;
        private System.Windows.Forms.DataGridViewTextBoxColumn calUpdateId;
        private System.Windows.Forms.DataGridViewTextBoxColumn calUpdateType;
        private System.Windows.Forms.DataGridViewTextBoxColumn calUpdateVer;
        private System.Windows.Forms.DataGridViewTextBoxColumn calUpdateDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn firmUpdateId;
        private System.Windows.Forms.DataGridViewTextBoxColumn firmUpdateType;
        private System.Windows.Forms.DataGridViewTextBoxColumn firmUpdateVer;
        private System.Windows.Forms.DataGridViewTextBoxColumn firmUpdateDesc;
        private System.Windows.Forms.ProgressBar tempBar;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkLabel darkLabel2;
    }
}