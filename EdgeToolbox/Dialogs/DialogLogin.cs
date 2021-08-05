using DarkUI.Forms;
using EdgeDeviceLibrary;
using EdgeDeviceLibrary.Communicator;
using EdgeToolbox.Classes;
using System.Windows.Forms;

namespace EdgeToolbox {
    public partial class DialogLogin : DarkDialog {
        public DialogLogin() {
            InitializeComponent();
            Proteus p = Proteus.Instance();
        }

        private void DialogLogin_FormClosing(object sender, FormClosingEventArgs e) {
            Settings s = Settings.Instance();
            s.email = emailTxt.Text;
            s.pass = passTxt.Text;
            s.Save();
        }
    }
}
