using DarkUI.Docking;
using EdgeDeviceLibrary;
using EdgeToolbox.Classes;

namespace EdgeToolbox.Docks {
    public partial class DockSettings : DarkToolWindow {
        Settings settings;

        public DockSettings() {
            InitializeComponent();

            settings = Settings.Instance();
            emailTxt.Text = settings.email;
            passTxt.Text = settings.pass;
            devModeCB.Checked = settings.devmode;
        }

        private void devModeCB_CheckedChanged(object sender, System.EventArgs e) {
            settings.devmode = devModeCB.Checked;
        }

        private void loadBtn_Click(object sender, System.EventArgs e) {
            settings.Load();
        }

        private void saveBtn_Click(object sender, System.EventArgs e) {
            settings.Save();
            DeviceConnector.Instance().Email = emailTxt.Text;
            DeviceConnector.Instance().Password = passTxt.Text;
        }
    }
}
