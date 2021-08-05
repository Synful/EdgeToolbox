using DarkUI.Controls;

namespace EdgeToolbox {
    partial class DialogVIN {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogVIN));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.txtVIN = new DarkUI.Controls.DarkTextBox();
            this.emailLabel = new DarkUI.Controls.DarkLabel();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.txtVIN);
            this.pnlMain.Controls.Add(this.emailLabel);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(15, 15, 15, 5);
            this.pnlMain.Size = new System.Drawing.Size(343, 73);
            this.pnlMain.TabIndex = 2;
            // 
            // txtVIN
            // 
            this.txtVIN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtVIN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtVIN.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtVIN.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtVIN.Location = new System.Drawing.Point(15, 35);
            this.txtVIN.Name = "txtVIN";
            this.txtVIN.Size = new System.Drawing.Size(313, 23);
            this.txtVIN.TabIndex = 1;
            this.txtVIN.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // emailLabel
            // 
            this.emailLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.emailLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.emailLabel.Location = new System.Drawing.Point(15, 15);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(313, 20);
            this.emailLabel.TabIndex = 0;
            this.emailLabel.Text = "VIN:";
            this.emailLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DialogVIN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 118);
            this.Controls.Add(this.pnlMain);
            this.DialogButtons = DarkUI.Forms.DarkDialogButton.OkCancel;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogVIN";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "VIN Prompt";
            this.Controls.SetChildIndex(this.pnlMain, 0);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private DarkTextBox txtVIN;
        private DarkLabel emailLabel;
    }
}