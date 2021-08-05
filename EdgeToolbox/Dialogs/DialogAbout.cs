using DarkUI.Forms;
using System.Windows.Forms;

namespace EdgeToolbox {
    public partial class DialogAbout : DarkDialog {
        public DialogAbout() {
            InitializeComponent();

            lblVersion.Text = $"Version: {Application.ProductVersion.ToString()}";
            btnOk.Text = "Close";
        }
    }
}
