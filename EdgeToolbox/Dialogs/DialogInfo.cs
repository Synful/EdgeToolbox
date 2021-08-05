using DarkUI.Forms;
using EdgeDeviceLibrary;
using EdgeDeviceLibrary.Communicator;
using System.Windows.Forms;

namespace EdgeToolbox {
    public partial class DialogInfo : DarkDialog {
        public DialogInfo() {
            InitializeComponent();
            Proteus p = Proteus.Instance();
            long[] info = p.GetUpdateFirmwareAndCalibrationIDs(p.GetSerialNumber().ToString(), p.GetFirmwareVersionString(), p.GetCalibrationVersionString());

            infoLabel.Text = $"Part #: {p.GetPartNumber(false)}\n" +
                             $"Serial #: {p.GetSerialNumber()}\n" +
                             $"Bootloader Ver: {p.GetBootloaderVersionString()}\n" +
                             $"Calibration Ver: {p.GetCalibrationVersionString()}\n" +
                             $"Firmware Ver: {p.GetFirmwareVersionString()}\n" +
                             $"Vehicle Make: {p.GetStockFolderName()}\n" +
                             $"Tune Level: {p.GetProgrammedToLevel()}\n" +
                             $"Upgrade Options: {p.GetUpgradeOptions()}";
            if (info != null) {
                updateLabel.Text = $"New Firmware Ver: {info[0]}\n" +
                                   $"New Calibration Ver: {info[1]}";
            }
            btnOk.Text = "Close";
        }
    }
}
