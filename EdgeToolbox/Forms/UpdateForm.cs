using DarkUI.Controls;
using DarkUI.Forms;
using EdgeDeviceLibrary;
using EdgeDeviceLibrary.Communicator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EdgeToolbox.Forms {
    public partial class UpdateForm : DarkForm {
		public class UpdateInfo {

			public string id { get; set; }
			public string type { get; set; }
			public string version { get; set; }
			public string description { get; set; }

			public UpdateInfo(string ID, string Description, string Type, string VersionString) {
				id = ID;
				description = Description;
				type = Type;
				version = VersionString;
			}
		}

		List<UpdateInfo> firmInfo = new List<UpdateInfo>();
		List<UpdateInfo> calInfo = new List<UpdateInfo>();

        public UpdateForm() {
			Control.CheckForIllegalCrossThreadCalls = false;
			InitializeComponent();
            firmData.AutoGenerateColumns = false;
            calData.AutoGenerateColumns = false;
        }

		private void UpdateDownloader_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			if (base.InvokeRequired) {
				BeginInvoke((MethodInvoker)delegate {
					UpdateDownloader_ProgressChanged(sender, e);
				});
			} else {
				darkLabel2.Text = (string)e.UserState;
				tempBar.Value = e.ProgressPercentage;
			}
		}
		private bool Download(string filename, long offset, ref MemoryStream ms) {
			UpdateDownloader.AutoSetChunkSize = false;
			UpdateDownloader.RemoteFileName = filename;
			UpdateDownloader.LocalSaveFolder = "Data";
			UpdateDownloader.IncludeHashVerification = false;
			UpdateDownloader.RunWorkerSync(ref ms, 9999999);
			return !UpdateDownloader.CancellationPending;
		}

		private void DownloadAll() {
			DeviceConnector dc = DeviceConnector.Instance();
            string workDir = $"{AppDomain.CurrentDomain.BaseDirectory}Files/Firms_Cals/{Proteus.Instance().GetPartNumber(false)}-{Proteus.Instance().PlatformType}";
            if (!Directory.Exists($"{workDir}")) {
                Directory.CreateDirectory($"{workDir}");
            }

            foreach (UpdateInfo fInfo in firmInfo) {
				darkLabel1.Text = $"{fInfo.id} | {fInfo.version}";
				string fTargetPath = $"_frm_cal\\{fInfo.id}.bin";
                MemoryStream firmMs = new MemoryStream();
                if (!Download(fTargetPath, 0L, ref firmMs)) {
                    return;
                }
                File.WriteAllBytes($"{workDir}/{fInfo.version}_Fw.zip", firmMs.ToArray());
            }

            foreach (UpdateInfo cInfo in calInfo) {
				darkLabel1.Text = $"{cInfo.id} | {cInfo.version}";
				string cTargetPath = $"_frm_cal\\{cInfo.id}.bin";
				MemoryStream calMs = new MemoryStream();
				if (!Download(cTargetPath, 0L, ref calMs)) {
					return;
				}
				File.WriteAllBytes($"{workDir}/{cInfo.version}_Cal.zip", calMs.ToArray());
			}
		}

        private void loaderBG_DoWork(object sender, DoWorkEventArgs e) {
			Invoke((MethodInvoker)delegate {
				firmInfo.Clear();
				string[] availableFirmwares = DeviceConnector.Instance().communicator.AvailableFirmware;
				for (int i = 0; i < availableFirmwares.Length / 4; i++) {
					string id = availableFirmwares[i * 4];
					string type = availableFirmwares[i * 4 + 2];
					if (id != "0" && type != "0")
						firmInfo.Add(new UpdateInfo(id, availableFirmwares[i * 4 + 1], type, availableFirmwares[i * 4 + 3]));
				}
				firmData.DataSource = firmInfo;

				calInfo.Clear();
				string[] availableCalibrations = DeviceConnector.Instance().communicator.AvailableCalibrations;
				for (int i = 0; i < availableCalibrations.Length / 4; i++) {
					string id = availableCalibrations[i * 4];
					string type = availableCalibrations[i * 4 + 2];
					if (id != "0" && type != "0")
						calInfo.Add(new UpdateInfo(id, availableCalibrations[i * 4 + 1], type, availableCalibrations[i * 4 + 3]));
				}
				calData.DataSource = calInfo;

				DownloadAll();
			});
		}

        private void darkButton1_Click(object sender, EventArgs e) {
			loaderBG.RunWorkerAsync();
		}
    }
}
