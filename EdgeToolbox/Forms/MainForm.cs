using Common;
using DarkUI.Forms;
using DarkUI.Win32;
using Edge.Common.Math;
using Edge.Imaging;
using EdgeDeviceLibrary;
using EdgeDeviceLibrary.Communicator;
using EdgeDeviceLibrary.Models;
using EdgeToolbox.Classes;
using EdgeToolbox.Docks;
using EdgeToolbox.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProteusCommand;
using ProteusCommand.MiniAppCommands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EdgeToolbox {
	public partial class MainForm : DarkForm {

		Settings settings;
		DeviceConnector deviceCon;
		Proteus p;
		bool canConnect = false;
		static MainForm _instance;
		public static MainForm Instance() {
			return _instance;
		}

		public MainForm() {
			_instance = this;
			CheckForIllegalCrossThreadCalls = false;
			settings = new Settings($"{AppDomain.CurrentDomain.BaseDirectory}");

			InitializeComponent();
			Application.AddMessageFilter(new ControlScrollFilter());
			Application.AddMessageFilter(DockPanel.DockContentDragFilter);
			Application.AddMessageFilter(DockPanel.DockResizeFilter);

			deviceCon = DeviceConnector.Instance();
			deviceCon.DeviceConnectionEvent += dc_DeviceConnectionEvent;
			deviceCon.DeviceDisconnectionEvent += dc_deviceDisconnectEvent;
			if (settings.email.Equals("") || settings.pass.Equals("")) {
				DarkMessageBox.ShowInformation(this, "Please Input Your Edge Login Info To Continue.", "Login Not Found");
				authFail:
				DialogResult dialog = new DialogLogin().ShowDialog(this);
				if (dialog != DialogResult.OK) {
					Environment.Exit(0);
				}
				if (!deviceCon.VerifyEmailAndPassword(settings.email, settings.pass)) {
					DarkMessageBox.ShowError(this, "Please check your username/password and try again", "Authorization failed");
					goto authFail;
				}
			}
			deviceCon.Email = settings.email;
			deviceCon.Password = settings.pass;
			DockPanel.AddContent(new DockSettings());
			DockPanel.AddContent(DockFileSystem.Instance());
			DockPanel.AddContent(new DockConsole());
			DeviceWaiter.RunWorkerAsync();
			UpdateStatus("Awaiting Device...");
		}
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			settings.Save();
		}

		private void DeviceWaiter_DoWork(object sender, DoWorkEventArgs e) {
			while (!deviceCon.IsDeviceConnected()) { }
		}
		private void dc_DeviceConnectionEvent(object sender, EventArgs e) {
			if(base.IsDisposed) {
				return;
			}
			dt_Connect.Visible = true;
			dt_Wait.Visible = false;
			canConnect = true;
		}
		private void dc_deviceDisconnectEvent(object sender, EventArgs e) {
			if(base.IsDisposed) {
				return;
			}
			dt_Wait.Visible = false;
			dt_Connect.Visible = false;
			dt_Info.Visible = false;
			dt_Update.Visible = false;
			canConnect = false;
		}

        private void aboutBtn_Click(object sender, EventArgs e) {
			new DialogAbout().ShowDialog();
        }

		private void dt_Connect_Click(object sender, EventArgs e) {
			if (canConnect) {
				dt_Connect.Visible = false;
				ConnectionWorker.RunWorkerAsync();
			}
		}
        private void ConnectionWorker_DoWork(object sender, DoWorkEventArgs e) {
			UpdateStatus("Booting Device...");
			Refresh();
			if(deviceCon.communicator != null) {
				if(deviceCon.communicator.GetBootLoaderVersionDouble(deviceCon.communicator.GetBootloaderVersionString()) >= 2.6) {
					deviceCon.communicator.RebootDevice(IsHardReset: true);
				}
				deviceCon.SetPlatformType();
			}
			UpdateStatus("Connected...");
			Refresh();
			while(deviceCon.communicator == null) {
				Application.DoEvents();
			}
			UpdateStatus("Validating device boot loader. Please wait...");
			Application.DoEvents();
			try {
				deviceCon.communicator.VerifyBootloader();
			} catch(Exception ex) {
				try {
					if(deviceCon.communicator.GetPartNumber() == string.Empty) {
						DarkMessageBox.ShowError(this, "Product Part Number not set.\nPlease follow the instructions provided by technical support.", "Boot Loader Error");
						return;
					}
				} catch(Exception ex2) {
					DarkMessageBox.ShowError(this, $"{ex2.Message}\nPlease follow the instructions provided by technical support.", "Boot Loader Error");
					return;
				}
				DarkMessageBox.ShowError(this, $"{ex.Message}\nPlease follow the instructions provided by technical support.", "Boot Loader Error");
				return;
			}
			string partNumber = deviceCon.communicator.GetPartNumber();
			if (partNumber.Length == 0) {
				DarkMessageBox.ShowError(this, "Product Part Number not set.\nPlease follow the instructions provided by technical support.", "Device Connection Error");
				return;
			}
			if (!deviceCon.communicator.HasSkuChanged() && deviceCon.communicator.GetUnsupportedSkus().Contains(partNumber)) {
				DarkMessageBox.ShowError(this, "This device is not supported by EdgeToolbox.\nEdgeToolbox will now close.", "Unsupported Device");
				Application.Exit();
				return;
			}
			Application.DoEvents();
			if (deviceCon.communicator.NeedsUserInputVIN(out var vinFormat)) {
				string text = DialogVIN.ShowForm(vinFormat, this);
				if(!string.IsNullOrEmpty(text)) {
					deviceCon.communicator.SaveUserVIN(text);
				}
			}
			string updateMessage;
			if (!deviceCon.communicator.IsUpdateEnabledForDevice(out updateMessage)) {
				DarkMessageBox.ShowError(this, $"{updateMessage}\nUpdate Is Currently Unavailable For This Product", "Update Message");
			} else {
				//DarkMessageBox.ShowError(this, $"Information:\n{updateMessage}", "Update Message");
			}
			Application.DoEvents();
			if (deviceCon.communicator.NeedsGmsk()) {
				ProcessGmsk();
			}
			if (!deviceCon.communicator.IsForceUpdateSet()) {
				try {
					if(!deviceCon.communicator.IsMakeStockWriterSet()) {
						if(deviceCon.communicator.IsProgrammingInterrupted()) {
							DarkMessageBox.ShowError(this, "Connect your device to your vehicle and follow the instructions on its display.\nIf programming fails, contact support.", "Device Status Error");
							return;
						}
						if(deviceCon.communicator.IsProgrammedToLevel()) {
							DarkMessageBox.ShowError(this, "Return vehicle to stock, then try to update again.\nRe-Install the device in your vehicle and return the power program to stock level 0, then reconnect the device to Fusion.", "Device Status Error");
							return;
						}
					}
					deviceCon.product.PerformDeviceConnectionTasks(new BackgroundWorker());
				} catch(InvalidDataException) {
				} catch(Exception ex5) {
					if(!ex5.Message.Contains("UPDATEBEFORESTOCKOPERATION")) {
						throw;
					}
				}
			} else if(deviceCon.communicator.IsProgrammedToLevel()) {
				DarkMessageBox.ShowError(this, "Return vehicle to stock, then try to update again.\nRe-Install the device in your vehicle and return the power program to stock level 0, then reconnect the device to Fusion.", "Device Status Error");
				return;
			}
			if (deviceCon.communicator.SendSupportFilesToDevice(lastStep: true)) { }
		}
        private void ConnectionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			p = Proteus.Instance();
			dt_Info.Visible = true;
			long[] versions = deviceCon.communicator.GetUpdateFirmwareAndCalibrationIDs(deviceCon.communicator.GetSerialNumber().ToString(), deviceCon.communicator.GetFirmwareVersionString(), deviceCon.communicator.GetCalibrationVersionString());
			//if (p.DeviceCanBeUpdated() && versions != null) {
				//deviceCon.FirmwareIDTarget = versions[0];
				//deviceCon.CalibrationIDTarget = versions[1];
				dt_Update.Visible = true;
			//}
			deviceCon.UnlockFeature("hotlevels");
			DockFileSystem.Instance().refreshBtn_Click(null, EventArgs.Empty);
			UpdateStatus("Device Awaiting Commands...");
			DisplayMsg("Device Connected To Edge Toolbox.\nMade By Synful.\nAwaiting Commands...");
			DarkMessageBox.ShowInformation(this, $"Connected To {p.GetPartNumber(false)}", "Connected!");
		}
		private void ProcessGmsk() {
			UpdateStatus("Contacting Powerteq Cloud for processing. Please wait...");
			Application.DoEvents();
			List<GmskRequest> requests = null;
			if(!deviceCon.communicator.TryGetGmskRequests(out requests)) {
				MessageBox.Show("An error occured when reading critical programming data.\r\nPlease contact technical support for assistance at 1-888-360-3343.", "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Application.Exit();
			}
			bool isReady = false;
			bool hasError = false;
			foreach(GmskRequest item in requests) {
				if(item.Request.CompareTo("0") == 0) {
					continue;
				}
				IDictionary<string, JToken> resultDict = null;
				resultDict = RequestSeedKey(item, resultDict, out isReady, out hasError);
				if(isReady && !hasError) {
					string key = resultDict["Key"].Value<string>();
					if(!deviceCon.communicator.SetGmsk(key, item)) {
						//MessageBox.Show("An error occured when writing critical programming data.\r\nPlease contact technical support for assistance at 1-888-360-3343.", "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						Application.Exit();
					}
				} else if(hasError) {
					string text = resultDict["Error"].Value<string>();
					deviceCon.Log("Error processing GMSK.  Error is: " + text);
					if(text == "NeedCredit") {
						//MessageBox.Show("A $40 vehicle license is required to tune your vehicle. Your device will now be updated. After updating, you will be brought to the Fusion online store where your vehicle license may be purchased. Once purchased, your device will receive the required license to tune your vehicle. If you have questions please call 888-360-3343.", "Key Unlock credit required", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						continue;
					}
					//MessageBox.Show("An error occured when requesting GM data (" + text + ").\r\nPlease contact technical support for assistance at 1-888-360-3343.", "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					Application.Exit();
				} else {
					//MessageBox.Show("Contacting Powerteq cloud for vehicle support. This may take 1-2 hours. You will receive an email when your unlock is ready.", "Server Processing", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					Application.Exit();
				}
			}
			if(isReady && !hasError) {
				//MessageBox.Show("Your GM device is now unlocked. We will now check for any additional device updates", "GM Unlocked", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
		}
		private IDictionary<string, JToken> RequestSeedKey(GmskRequest gmskRequest, IDictionary<string, JToken> resultDict, out bool isReady, out bool hasError) {
            DataService dataService = new DataService();
			Dictionary<string, object> obj = new Dictionary<string, object> {
				["GMSeedKey"] = true,
				["VIN"] = gmskRequest.Vin,
				["Seed"] = gmskRequest.Seed,
				["SerialNumber"] = gmskRequest.SerialNumber,
				["SupplierId"] = GetSupplierId(gmskRequest)
			};
			obj["FusionPartNumber"] = deviceCon.communicator.GetPartNumber();
			obj["Email"] = deviceCon.Email;
			string jsonQuery = JsonConvert.SerializeObject(obj);
			resultDict = JsonConvert.DeserializeObject<IDictionary<string, JToken>>(dataService.Query("UpdateGMToken", jsonQuery));
			isReady = resultDict["IsReady"].Value<bool>();
			hasError = resultDict.ContainsKey("Error");
			return resultDict;
		}
		private string GetSupplierId(GmskRequest request) {
			string result = "";
			if(Regex.IsMatch(request.Module, "^0x7E0$", RegexOptions.IgnoreCase)) {
				if(Regex.IsMatch(request.Vin, "^.{7}[G]")) {
					result = "70";
				} else if(Regex.IsMatch(request.Vin, "^.{7}[Y]")) {
					result = "65";
				} else if(Regex.IsMatch(request.Vin, "^.{2}[0146]")) {
					result = "129";
				} else if(Regex.IsMatch(request.Vin, "^.{2}[YNKCT]")) {
					result = "146";
				}
			} else if(Regex.IsMatch(request.Module, "^0x7E2$", RegexOptions.IgnoreCase)) {
				result = "135";
			}
			return result;
		}

		/*
		     public enum FlashArea : byte {
        MainFlash = 0,
        SecondFlash = 1,
        NIOS_App = 10,
        NIOS_FPGA_BC = 11,
        MSP430App = 12,
        ErrorLog = 13,
        NotSet = 255
    }
		 */


		private void dt_Info_Click(object sender, EventArgs e) {

			Cmd_MA_ReadFlash readMainFlash = new Cmd_MA_ReadFlash(false, FlashArea.MainFlash, 0, 9000000);
			Proteus.Instance()._comm.IssueCommand(Proteus.Instance()._serialNumber, readMainFlash);
			File.WriteAllBytes($"{AppDomain.CurrentDomain.BaseDirectory}Files/{Proteus.Instance().GetPartNumber(true)}/Bins/MainFlash.bin", readMainFlash.FlashData);

			//Cmd_MA_ReadFlash readSecondFlash = new Cmd_MA_ReadFlash(false, FlashArea.SecondFlash, 0, 1000000);
			//Proteus.Instance()._comm.IssueCommand(Proteus.Instance()._serialNumber, readSecondFlash);
			//File.WriteAllBytes($"{AppDomain.CurrentDomain.BaseDirectory}Files/{Proteus.Instance().GetPartNumber(true)}/Bins/SecondFlash.bin", readSecondFlash.FlashData);

			Cmd_MA_ReadFlash readNIOS_App = new Cmd_MA_ReadFlash(false, FlashArea.NIOS_App, 0, 1000000);
			Proteus.Instance()._comm.IssueCommand(Proteus.Instance()._serialNumber, readNIOS_App);
			File.WriteAllBytes($"{AppDomain.CurrentDomain.BaseDirectory}Files/{Proteus.Instance().GetPartNumber(true)}/Bins/NIOS_App.bin", readNIOS_App.FlashData);

			Cmd_MA_ReadFlash readNIOS_FPGA_BC = new Cmd_MA_ReadFlash(false, FlashArea.NIOS_FPGA_BC, 0, 1000000);
			Proteus.Instance()._comm.IssueCommand(Proteus.Instance()._serialNumber, readNIOS_FPGA_BC);
			File.WriteAllBytes($"{AppDomain.CurrentDomain.BaseDirectory}Files/{Proteus.Instance().GetPartNumber(true)}/Bins/NIOS_FPGA_BC.bin", readNIOS_FPGA_BC.FlashData);

			Cmd_MA_ReadFlash readMSP430App = new Cmd_MA_ReadFlash(false, FlashArea.MSP430App, 0, 1000000);
			Proteus.Instance()._comm.IssueCommand(Proteus.Instance()._serialNumber, readMSP430App);
			File.WriteAllBytes($"{AppDomain.CurrentDomain.BaseDirectory}Files/{Proteus.Instance().GetPartNumber(true)}/Bins/MSP430App.bin", readMSP430App.FlashData);

			Cmd_MA_ReadFlash readErrorLog = new Cmd_MA_ReadFlash(false, FlashArea.ErrorLog, 0, 0);
			Proteus.Instance()._comm.IssueCommand(Proteus.Instance()._serialNumber, readErrorLog);
			File.WriteAllBytes($"{AppDomain.CurrentDomain.BaseDirectory}Files/{Proteus.Instance().GetPartNumber(true)}/Bins/ErrorLog.bin", readErrorLog.FlashData);

			//new DialogInfo().ShowDialog();
		}

        private void dt_Update_Click(object sender, EventArgs e) {
			//new DialogUpdate().ShowDialog();
			new UpdateForm().ShowDialog();
		}

		private void DisplayMsg(string message) {
			Cmd_DisplayMessage cmd_DisplayMessage = null;
			Proteus p = Proteus.Instance();
			try {
				if (p != null) {
					cmd_DisplayMessage = new Cmd_DisplayMessage(encrypted: false, "messagewindow", message);
					p._comm.IssueCommand(p._serialNumber, cmd_DisplayMessage);
				}
			} catch (Exception) { }
		}
		private void UpdateStatus(string msg) {
			dStatusLabel.Text = msg;
		}

        private void dt_DumpFiles_Click(object sender, EventArgs e) {
			new DialogFileDownloader().ShowDialog();
		}
    }
}
