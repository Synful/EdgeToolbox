using DarkUI.Controls;

namespace EdgeToolbox {
    partial class DialogLogin {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogLogin));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.emailLabel = new DarkUI.Controls.DarkLabel();
            this.emailTxt = new DarkUI.Controls.DarkTextBox();
            this.passTxt = new DarkUI.Controls.DarkTextBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.passTxt);
            this.pnlMain.Controls.Add(this.darkLabel1);
            this.pnlMain.Controls.Add(this.emailTxt);
            this.pnlMain.Controls.Add(this.emailLabel);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(15, 15, 15, 5);
            this.pnlMain.Size = new System.Drawing.Size(343, 156);
            this.pnlMain.TabIndex = 2;
            // 
            // emailLabel
            // 
            this.emailLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.emailLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.emailLabel.Location = new System.Drawing.Point(15, 15);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(313, 20);
            this.emailLabel.TabIndex = 0;
            this.emailLabel.Text = "Email:";
            this.emailLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // emailTxt
            // 
            this.emailTxt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.emailTxt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.emailTxt.Dock = System.Windows.Forms.DockStyle.Top;
            this.emailTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.emailTxt.Location = new System.Drawing.Point(15, 35);
            this.emailTxt.Name = "emailTxt";
            this.emailTxt.Size = new System.Drawing.Size(313, 23);
            this.emailTxt.TabIndex = 1;
            this.emailTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // passTxt
            // 
            this.passTxt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.passTxt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passTxt.Dock = System.Windows.Forms.DockStyle.Top;
            this.passTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.passTxt.Location = new System.Drawing.Point(15, 78);
            this.passTxt.Name = "passTxt";
            this.passTxt.Size = new System.Drawing.Size(313, 23);
            this.passTxt.TabIndex = 3;
            this.passTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.passTxt.UseSystemPasswordChar = true;
            // 
            // darkLabel1
            // 
            this.darkLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(15, 58);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(313, 20);
            this.darkLabel1.TabIndex = 2;
            this.darkLabel1.Text = "Password:";
            this.darkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DialogLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 156);
            this.Controls.Add(this.pnlMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DialogLogin_FormClosing);
            this.Controls.SetChildIndex(this.pnlMain, 0);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private DarkTextBox passTxt;
        private DarkLabel darkLabel1;
        private DarkTextBox emailTxt;
        private DarkLabel emailLabel;
    }
}