using DarkUI.Config;
using DarkUI.Controls;
using DarkUI.Docking;
using DarkUI.Forms;
using EdgeToolbox.Classes;
using EdgeToolbox.Properties;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EdgeToolbox {
    public partial class DockTextEditor : DarkDocument {
        byte[] fileData;

        public DockTextEditor() {
            InitializeComponent();

            txtDocument.SelectionStart = txtDocument.Text.Length;
        }

        public DockTextEditor(string name, byte[] data) : this() {
            DockText = name;
            fileData = data;
            Icon = Resources.document_16xLG;
            txtDocument.Text = Encoding.UTF8.GetString(data);
        }

        public override void Close() {
            var result = DarkMessageBox.ShowWarning(null, @"You will lose any unsaved changes. Continue?", @"Close document", DarkDialogButton.YesNo);
            if(result == DialogResult.No)
                return;

            base.Close();
        }
    }
}
