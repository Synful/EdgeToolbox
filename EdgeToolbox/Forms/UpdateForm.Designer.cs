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
            this.load_all_bw = new System.ComponentModel.BackgroundWorker();
            this.calData = new System.Windows.Forms.DataGridView();
            this.calUpdateId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.calUpdateType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.calUpdateVer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.calUpdateDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.firmData = new System.Windows.Forms.DataGridView();
            this.firmUpdateId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.firmUpdateType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.firmUpdateVer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.firmUpdateDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.load_all_btn = new DarkUI.Controls.DarkButton();
            this.download_all_btn = new DarkUI.Controls.DarkButton();
            this.darkStatusStrip1 = new DarkUI.Controls.DarkStatusStrip();
            this.file_download_pb = new System.Windows.Forms.ToolStripProgressBar();
            this.info_lb = new System.Windows.Forms.ToolStripStatusLabel();
            this.file_lb = new System.Windows.Forms.ToolStripStatusLabel();
            this.download_all_bw = new System.ComponentModel.BackgroundWorker();
            this.product_id_cb = new DarkUI.Controls.DarkComboBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.update_product_bw = new System.ComponentModel.BackgroundWorker();
            this.update_downloader = new EdgeDeviceLibrary.FileTransferDownload();
            ((System.ComponentModel.ISupportInitialize)(this.calData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.firmData)).BeginInit();
            this.darkStatusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // load_all_bw
            // 
            this.load_all_bw.DoWork += new System.ComponentModel.DoWorkEventHandler(this.load_all_bw_DoWork);
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
            this.calData.Location = new System.Drawing.Point(0, 306);
            this.calData.MultiSelect = false;
            this.calData.Name = "calData";
            this.calData.ReadOnly = true;
            this.calData.RowHeadersVisible = false;
            this.calData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.calData.Size = new System.Drawing.Size(1001, 241);
            this.calData.TabIndex = 1;
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
            this.firmData.Location = new System.Drawing.Point(0, 60);
            this.firmData.MultiSelect = false;
            this.firmData.Name = "firmData";
            this.firmData.ReadOnly = true;
            this.firmData.RowHeadersVisible = false;
            this.firmData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.firmData.Size = new System.Drawing.Size(1001, 246);
            this.firmData.TabIndex = 2;
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
            // load_all_btn
            // 
            this.load_all_btn.Location = new System.Drawing.Point(12, 12);
            this.load_all_btn.Name = "load_all_btn";
            this.load_all_btn.Padding = new System.Windows.Forms.Padding(5);
            this.load_all_btn.Size = new System.Drawing.Size(144, 36);
            this.load_all_btn.TabIndex = 3;
            this.load_all_btn.Text = "Load All";
            this.load_all_btn.Click += new System.EventHandler(this.load_all_btn_Click);
            // 
            // download_all_btn
            // 
            this.download_all_btn.Location = new System.Drawing.Point(162, 12);
            this.download_all_btn.Name = "download_all_btn";
            this.download_all_btn.Padding = new System.Windows.Forms.Padding(5);
            this.download_all_btn.Size = new System.Drawing.Size(144, 36);
            this.download_all_btn.TabIndex = 7;
            this.download_all_btn.Text = "Download All";
            this.download_all_btn.Click += new System.EventHandler(this.download_all_btn_Click);
            // 
            // darkStatusStrip1
            // 
            this.darkStatusStrip1.AutoSize = false;
            this.darkStatusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.darkStatusStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkStatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.file_download_pb,
            this.info_lb,
            this.file_lb});
            this.darkStatusStrip1.Location = new System.Drawing.Point(0, 547);
            this.darkStatusStrip1.Name = "darkStatusStrip1";
            this.darkStatusStrip1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 3);
            this.darkStatusStrip1.Size = new System.Drawing.Size(1001, 31);
            this.darkStatusStrip1.SizingGrip = false;
            this.darkStatusStrip1.TabIndex = 8;
            this.darkStatusStrip1.Text = "darkStatusStrip1";
            // 
            // file_download_pb
            // 
            this.file_download_pb.Name = "file_download_pb";
            this.file_download_pb.Size = new System.Drawing.Size(100, 17);
            // 
            // info_lb
            // 
            this.info_lb.Name = "info_lb";
            this.info_lb.Size = new System.Drawing.Size(27, 18);
            this.info_lb.Text = "null";
            // 
            // file_lb
            // 
            this.file_lb.Name = "file_lb";
            this.file_lb.Size = new System.Drawing.Size(27, 18);
            this.file_lb.Text = "null";
            // 
            // download_all_bw
            // 
            this.download_all_bw.WorkerReportsProgress = true;
            this.download_all_bw.WorkerSupportsCancellation = true;
            this.download_all_bw.DoWork += new System.ComponentModel.DoWorkEventHandler(this.download_all_bw_DoWork);
            // 
            // product_id_cb
            // 
            this.product_id_cb.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.product_id_cb.Enabled = false;
            this.product_id_cb.FormattingEnabled = true;
            this.product_id_cb.Items.AddRange(new object[] {
            "[1] EEF2100",
            "[2] EEF2400",
            "[3] EEC1000",
            "[4] EEF1100",
            "[5] EEF1200",
            "[6] EEC2200",
            "[7] Test7",
            "[9] 35060",
            "[10] 25003",
            "[12] 10103",
            "[13] 83400",
            "[14] EEF2100-EVO",
            "[16] 15004",
            "[17] 15001",
            "[18] 85100",
            "[19] 85150",
            "[20] 85110",
            "[21] 85160",
            "[22] 85600",
            "[24] 15500",
            "[25] 25500",
            "[26] 85200",
            "[27] 85250",
            "[28] 85700",
            "[29] AttitudeCS",
            "[30] AttitudeCTS",
            "[31] AttitudeRaceCS",
            "[32] AttitudeRaceCTS",
            "[33] InsightCS",
            "[34] InsightCTS",
            "[35] 65231",
            "[36] 65230",
            "[37] 8040",
            "[38] 8030",
            "[39] 8130",
            "[40] 8140",
            "[41] 85101",
            "[42] 85201",
            "[43] 73830",
            "[44] 85210",
            "[45] 85260",
            "[46] 85400",
            "[47] 85450",
            "[48] 85350",
            "[49] 85300",
            "[50] X140X",
            "[51] X150X",
            "[52] 84030",
            "[53] 84130",
            "[54] 88888",
            "[55] 99999",
            "[56] 42050",
            "[58] 85301",
            "[59] 85401",
            "[60] 74130",
            "[61] 75200",
            "[62] 86100",
            "[63] 86000",
            "[64] 85302",
            "[65] 85402",
            "[66] 42051",
            "[67] 41050",
            "[68] 41051",
            "[69] 84031",
            "[70] 84131",
            "[71] 84132",
            "[72] 84032",
            "[73] 1050",
            "[74] 2050",
            "[75] 3050",
            "[76] 1060",
            "[77] 2060",
            "[78] 3060",
            "[79] 65235",
            "[80] 85351",
            "[81] 85451",
            "[82] 25350",
            "[83] 25351",
            "[84] 25450",
            "[85] 25451",
            "[86] 2067-17",
            "[249] 6520x",
            "[250] Test250",
            "[252] DemoCTS",
            "[253] DemoCS"});
            this.product_id_cb.Location = new System.Drawing.Point(312, 27);
            this.product_id_cb.Name = "product_id_cb";
            this.product_id_cb.Size = new System.Drawing.Size(145, 21);
            this.product_id_cb.TabIndex = 9;
            this.product_id_cb.SelectedIndexChanged += new System.EventHandler(this.product_id_cb_SelectedIndexChanged);
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(355, 9);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(58, 13);
            this.darkLabel1.TabIndex = 10;
            this.darkLabel1.Text = "Product ID";
            // 
            // update_product_bw
            // 
            this.update_product_bw.DoWork += new System.ComponentModel.DoWorkEventHandler(this.update_product_bw_DoWork);
            // 
            // update_downloader
            // 
            this.update_downloader.WorkerReportsProgress = true;
            this.update_downloader.WorkerSupportsCancellation = true;
            this.update_downloader.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.update_downloader_ProgressChanged);
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1001, 578);
            this.Controls.Add(this.darkLabel1);
            this.Controls.Add(this.product_id_cb);
            this.Controls.Add(this.download_all_btn);
            this.Controls.Add(this.load_all_btn);
            this.Controls.Add(this.firmData);
            this.Controls.Add(this.calData);
            this.Controls.Add(this.darkStatusStrip1);
            this.Name = "UpdateForm";
            this.Text = "UpdateForm";
            ((System.ComponentModel.ISupportInitialize)(this.calData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.firmData)).EndInit();
            this.darkStatusStrip1.ResumeLayout(false);
            this.darkStatusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private EdgeDeviceLibrary.FileTransferDownload update_downloader;
        private System.ComponentModel.BackgroundWorker load_all_bw;
        private System.Windows.Forms.DataGridView calData;
        private System.Windows.Forms.DataGridView firmData;
        private DarkUI.Controls.DarkButton load_all_btn;
        private System.Windows.Forms.DataGridViewTextBoxColumn calUpdateId;
        private System.Windows.Forms.DataGridViewTextBoxColumn calUpdateType;
        private System.Windows.Forms.DataGridViewTextBoxColumn calUpdateVer;
        private System.Windows.Forms.DataGridViewTextBoxColumn calUpdateDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn firmUpdateId;
        private System.Windows.Forms.DataGridViewTextBoxColumn firmUpdateType;
        private System.Windows.Forms.DataGridViewTextBoxColumn firmUpdateVer;
        private System.Windows.Forms.DataGridViewTextBoxColumn firmUpdateDesc;
        private DarkUI.Controls.DarkButton download_all_btn;
        private DarkUI.Controls.DarkStatusStrip darkStatusStrip1;
        private System.Windows.Forms.ToolStripProgressBar file_download_pb;
        private System.Windows.Forms.ToolStripStatusLabel info_lb;
        private System.Windows.Forms.ToolStripStatusLabel file_lb;
        private System.ComponentModel.BackgroundWorker download_all_bw;
        private DarkUI.Controls.DarkComboBox product_id_cb;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private System.ComponentModel.BackgroundWorker update_product_bw;
    }
}