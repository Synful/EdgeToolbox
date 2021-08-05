namespace EdgeToolbox.Docks {
    partial class DockSettings {
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
            this.passTxt = new DarkUI.Controls.DarkTextBox();
            this.emailTxt = new DarkUI.Controls.DarkTextBox();
            this.saveBtn = new DarkUI.Controls.DarkButton();
            this.emailLabel = new DarkUI.Controls.DarkLabel();
            this.passLabel = new DarkUI.Controls.DarkLabel();
            this.loadBtn = new DarkUI.Controls.DarkButton();
            this.devModeCB = new DarkUI.Controls.DarkCheckBox();
            this.SuspendLayout();
            // 
            // passTxt
            // 
            this.passTxt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.passTxt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passTxt.Dock = System.Windows.Forms.DockStyle.Top;
            this.passTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.passTxt.Location = new System.Drawing.Point(0, 83);
            this.passTxt.Name = "passTxt";
            this.passTxt.Size = new System.Drawing.Size(201, 20);
            this.passTxt.TabIndex = 8;
            this.passTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // emailTxt
            // 
            this.emailTxt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.emailTxt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.emailTxt.Dock = System.Windows.Forms.DockStyle.Top;
            this.emailTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.emailTxt.Location = new System.Drawing.Point(0, 44);
            this.emailTxt.Name = "emailTxt";
            this.emailTxt.Size = new System.Drawing.Size(201, 20);
            this.emailTxt.TabIndex = 7;
            this.emailTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(102, 132);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Padding = new System.Windows.Forms.Padding(5);
            this.saveBtn.Size = new System.Drawing.Size(94, 23);
            this.saveBtn.TabIndex = 9;
            this.saveBtn.Text = "Save";
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // emailLabel
            // 
            this.emailLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.emailLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.emailLabel.Location = new System.Drawing.Point(0, 25);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(201, 19);
            this.emailLabel.TabIndex = 10;
            this.emailLabel.Text = "Email";
            this.emailLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // passLabel
            // 
            this.passLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.passLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.passLabel.Location = new System.Drawing.Point(0, 64);
            this.passLabel.Name = "passLabel";
            this.passLabel.Size = new System.Drawing.Size(201, 19);
            this.passLabel.TabIndex = 11;
            this.passLabel.Text = "Password";
            this.passLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // loadBtn
            // 
            this.loadBtn.Location = new System.Drawing.Point(3, 132);
            this.loadBtn.Name = "loadBtn";
            this.loadBtn.Padding = new System.Windows.Forms.Padding(5);
            this.loadBtn.Size = new System.Drawing.Size(90, 23);
            this.loadBtn.TabIndex = 12;
            this.loadBtn.Text = "Load";
            this.loadBtn.Click += new System.EventHandler(this.loadBtn_Click);
            // 
            // devModeCB
            // 
            this.devModeCB.AutoSize = true;
            this.devModeCB.Location = new System.Drawing.Point(64, 109);
            this.devModeCB.Name = "devModeCB";
            this.devModeCB.Size = new System.Drawing.Size(76, 17);
            this.devModeCB.TabIndex = 13;
            this.devModeCB.Text = "Dev Mode";
            this.devModeCB.CheckedChanged += new System.EventHandler(this.devModeCB_CheckedChanged);
            // 
            // DockSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.devModeCB);
            this.Controls.Add(this.loadBtn);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.passTxt);
            this.Controls.Add(this.passLabel);
            this.Controls.Add(this.emailTxt);
            this.Controls.Add(this.emailLabel);
            this.DefaultDockArea = DarkUI.Docking.DarkDockArea.Right;
            this.DockText = "Settings";
            this.Icon = global::EdgeToolbox.Properties.Resources.properties_16xLG;
            this.Name = "DockSettings";
            this.SerializationKey = "DockSettings";
            this.Size = new System.Drawing.Size(201, 442);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DarkUI.Controls.DarkTextBox passTxt;
        private DarkUI.Controls.DarkTextBox emailTxt;
        private DarkUI.Controls.DarkButton saveBtn;
        private DarkUI.Controls.DarkLabel emailLabel;
        private DarkUI.Controls.DarkLabel passLabel;
        private DarkUI.Controls.DarkButton loadBtn;
        private DarkUI.Controls.DarkCheckBox devModeCB;
    }
}
