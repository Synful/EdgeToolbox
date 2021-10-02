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
using System.Threading;
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

		int sel_product_id;
		string sel_model;

        public UpdateForm() {
			Control.CheckForIllegalCrossThreadCalls = false;
			InitializeComponent();
            firmData.AutoGenerateColumns = false;
            calData.AutoGenerateColumns = false;
			info_lb.Text = "";
			file_lb.Text = "";
			info_lb.Text = "Setting Product ID...";
			update_product_bw.RunWorkerAsync();
		}

		private void update_product_bw_DoWork(object sender, DoWorkEventArgs e) {
			Thread.Sleep(1000);
			Invoke((MethodInvoker)delegate {
				List<string> models = product_id_cb.Items.Cast<string>().ToList();
				int index = models.FindIndex(x => x == Proteus.Instance().GetCustomModelPart(false));
				product_id_cb.SelectedIndex = index;
				info_lb.Text = "";
				product_id_cb.Enabled = true;
			});
		}

		private void product_id_cb_SelectedIndexChanged(object sender, EventArgs e) {
			string[] sel = product_id_cb.SelectedItem.ToString().Replace("[", "").Replace("]", "").Split(' ');
			sel_product_id = int.Parse(sel[0]);
			sel_model = sel[1];
		}

		public string[] AvailableFirmware {
			get {
				EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
				return fusionService.GetAvailableFirmware(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, sel_product_id);
			}
		}
		public string[] AvailableCalibrations {
			get {
				EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
				string[] availableCalibrations = fusionService.GetAvailableCalibrations(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, sel_product_id, DeviceConnector.Instance().FirmwareIDTarget);
				if(availableCalibrations == null) {
					availableCalibrations = fusionService.GetAvailableCalibrations(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, sel_product_id, 0L);
				} else if(availableCalibrations[0] == null) {
					availableCalibrations = fusionService.GetAvailableCalibrations(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, sel_product_id, 0L);
				}
				return availableCalibrations;
			}
		}

		private void load_all_bw_DoWork(object sender, DoWorkEventArgs e) {
			Invoke((MethodInvoker)delegate {
				firmInfo.Clear();
				string[] availableFirmwares = AvailableFirmware;
				for (int i = 0; i < availableFirmwares.Length / 4; i++) {
					string id = availableFirmwares[i * 4];
					string type = availableFirmwares[i * 4 + 2];
					if (id != "0" && type != "0")
						firmInfo.Add(new UpdateInfo(id, availableFirmwares[i * 4 + 1], type, availableFirmwares[i * 4 + 3]));
				}
				firmData.DataSource = firmInfo;

				calInfo.Clear();
				string[] availableCalibrations = AvailableCalibrations;
				for (int i = 0; i < availableCalibrations.Length / 4; i++) {
					string id = availableCalibrations[i * 4];
					string type = availableCalibrations[i * 4 + 2];
					if (id != "0" && type != "0")
						calInfo.Add(new UpdateInfo(id, availableCalibrations[i * 4 + 1], type, availableCalibrations[i * 4 + 3]));
				}
				calData.DataSource = calInfo;
			});
		}
        private void load_all_btn_Click(object sender, EventArgs e) {
			load_all_bw.RunWorkerAsync();
		}

		private void update_downloader_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			if(base.InvokeRequired) {
				BeginInvoke((MethodInvoker)delegate {
					update_downloader_ProgressChanged(sender, e);
				});
			} else {
				file_lb.Text = (string)e.UserState;
			}
		}
		private bool Download(string filename, long offset, ref MemoryStream ms) {
			update_downloader.AutoSetChunkSize = false;
			update_downloader.RemoteFileName = filename;
			update_downloader.LocalSaveFolder = "Data";
			update_downloader.IncludeHashVerification = false;
			update_downloader.RunWorkerSync(ref ms, 9999999);
			return !update_downloader.CancellationPending;
		}
		private void download_all_bw_DoWork(object sender, DoWorkEventArgs e) {
			Invoke((MethodInvoker)delegate {
				DeviceConnector dc = DeviceConnector.Instance();

				file_download_pb.Maximum = firmInfo.Count;
				string firm_dir = $"{AppDomain.CurrentDomain.BaseDirectory}Files/{sel_model}/Firmwares/";
				if(!Directory.Exists($"{firm_dir}")) {
					Directory.CreateDirectory($"{firm_dir}");
				}
				foreach(UpdateInfo fInfo in firmInfo) {
					info_lb.Text = $"{fInfo.id} | {fInfo.version}";
					string fTargetPath = $"_frm_cal\\{fInfo.id}.bin";
					MemoryStream firmMs = new MemoryStream();
					if(!Download(fTargetPath, 0L, ref firmMs)) {
						return;
					}
					File.WriteAllBytes($"{firm_dir}/{fInfo.version}.zip", firmMs.ToArray());
					file_download_pb.Value++;
				}

				file_download_pb.Maximum = calInfo.Count;
				string cal_dur = $"{AppDomain.CurrentDomain.BaseDirectory}Files/{sel_model}/Calibrations/";
				if(!Directory.Exists($"{cal_dur}")) {
					Directory.CreateDirectory($"{cal_dur}");
				}
				foreach(UpdateInfo cInfo in calInfo) {
					info_lb.Text = $"{cInfo.id} | {cInfo.version}";
					string cTargetPath = $"_frm_cal\\{cInfo.id}.bin";
					MemoryStream calMs = new MemoryStream();
					if(!Download(cTargetPath, 0L, ref calMs)) {
						return;
					}
					File.WriteAllBytes($"{cal_dur}/{cInfo.version}.zip", calMs.ToArray());
					file_download_pb.Value++;
				}
			});
		}
		private void download_all_btn_Click(object sender, EventArgs e) {
			download_all_bw.RunWorkerAsync();
		}
    }
}
