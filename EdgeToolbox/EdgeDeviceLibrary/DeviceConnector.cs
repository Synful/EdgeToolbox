using System;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using EdgeDeviceLibrary.Communicator;
using EdgeDeviceLibrary.FusionService;
using EdgeDeviceLibrary.Products;
using Microsoft.Win32;
using USBAssembly;

namespace EdgeDeviceLibrary {
	public class DeviceConnector {
		public class ConnectionArgs : EventArgs {
			public enum ConnectionType {
				Connected,
				Disconnected
			}

			private ConnectionType _eventType = ConnectionType.Connected;

			public ConnectionType EventType {
				get {
					return _eventType;
				}
				set {
					_eventType = value;
				}
			}
		}

		public delegate void DeviceConnectionEventHandler(object sender, EventArgs e);

		public delegate void DeviceDisconnectionEventHandler(object sender, EventArgs e);

		public delegate void ProgressReportEventHandler(object sender, ProgressReportEventArgs e);

		public class ProgressReportEventArgs : EventArgs {
			public string task;

			public string status;

			public int percentComplete;

			public ProgressReportEventArgs(string task, string status, int percentComplete) {
				this.task = task;
				this.status = status;
				this.percentComplete = percentComplete;
			}
		}

		private string email;

		private string password;

		public long FirmwareIDTarget = 0L;

		public long CalibrationIDTarget = 0L;

		private SerialPort port;

		private USBCommunicator usbComm = null;

		public string filePath;

		public Communicator_Base communicator;

		private string EvoFoundAtRegistryKey;

		public Product_Base product;

		private bool instantiateProductOnConnection = true;

		private bool noDetectionAssumeChameleon = false;

		private FileTransferDownload fileTransferDownload1;

		private FileTransferUpload fileTransferUpload1;

		private static DeviceConnector dc;

		private static readonly object _lock = new object();

		public string Email {
			get {
				return email;
			}
			set {
				email = value;
			}
		}

		public string Password {
			get {
				return password;
			}
			set {
				password = value;
			}
		}

		public string FirmwareIDTargetPath {
			get {
				if (communicator != null) {
					return communicator.FirmwareIDTargetPath;
				}
				return "none";
			}
		}

		public string CalibrationIDTargetPath {
			get {
				if (communicator != null) {
					return communicator.CalibrationIDTargetPath;
				}
				return "none";
			}
		}

		public bool HasUnlockableUpgrades {
			get {
				if (communicator != null) {
					return communicator.HasUnlockableUpgrades;
				}
				return false;
			}
		}

		public bool InstantiateProductOnConnection {
			get {
				return instantiateProductOnConnection;
			}
			set {
				instantiateProductOnConnection = value;
			}
		}

		public bool NoDetectionAssumeChameleon {
			get {
				return noDetectionAssumeChameleon;
			}
			set {
				noDetectionAssumeChameleon = value;
			}
		}

		public event DeviceConnectionEventHandler DeviceConnectionEvent;

		public event DeviceDisconnectionEventHandler DeviceDisconnectionEvent;

		public event ProgressReportEventHandler ProgressReportEvent;

		public event ProgressReportEventHandler OverallProgressReportEvent;

		public uint GetUnlockCode(string codeName) {
			if (communicator != null) {
				return communicator.GetUnlockCode(codeName);
			}
			return 0u;
		}

		public bool UnlockFeature(string codeName) {
			if (communicator != null) {
				return communicator.UnlockFeature(codeName);
			}
			return false;
		}

		public static DeviceConnector Instance() {
			lock (_lock) {
				if (dc == null) {
					dc = new DeviceConnector();
				}
				return dc;
			}
		}

		private DeviceConnector() {
			Random random = new Random();
			byte[] array = new byte[8];
			random.NextBytes(array);
			filePath = string.Format("{1}\\{0}", new Guid(random.Next(), (short)random.Next(), (short)random.Next(), array).ToString(), Environment.ExpandEnvironmentVariables("%temp%"));
			if (!Directory.Exists(filePath)) {
				Directory.CreateDirectory(filePath);
			}
			port = new SerialPort();
			fileTransferDownload1 = new FileTransferDownload();
		}

		public bool IsDeviceConnected() {
			return IsDeviceConnected(doConnectEvent: true);
		}

		public bool IsDeviceConnected(bool doConnectEvent) {
			try {
				if (usbComm == null) {
					usbComm = new USBCommunicator();
				}
			} catch {
				usbComm = null;
			}
			uint num = 0u;
			if (usbComm != null) {
				num = usbComm.EnumDevices();
			}
			if (num != 0) {
				if (!IdentifyAndConnectUSBPort()) {
					return false;
				}
				if (doConnectEvent) {
					OnDeviceConnectionEvent();
				}
				if (usbComm.EnumDevices() != 0) {
					return true;
				}
				return false;
			}
			if (EvoFoundAtRegistryKey == null) {
				if (!IdentifyAndConnectCommPort()) {
					return false;
				}
				if (doConnectEvent) {
					OnDeviceConnectionEvent();
				}
			}
			RegistryKey localMachine = Registry.LocalMachine;
			localMachine = localMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\SERIALCOMM");
			if (localMachine == null) {
				return false;
			}
			string text = (string)localMachine.GetValue(EvoFoundAtRegistryKey);
			if (text != null) {
				return true;
			}
			return false;
		}

		public void Close() {
			if (communicator != null) {
				communicator.UpdateComplete();
				communicator.Cleanup();
				communicator = null;
			}
			try {
				port.Close();
			} catch {
			}
			EvoFoundAtRegistryKey = null;
		}

		private bool IdentifyAndConnectUSBPort() {
			return IdentifyUSBDevice();
		}

		private bool IdentifyAndConnectCommPort() {
			bool result = false;
			RegistryKey localMachine = Registry.LocalMachine;
			localMachine = localMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\SERIALCOMM");
			if (localMachine == null) {
				return false;
			}
			for (int i = 0; i < 20; i++) {
				string text = (string)localMachine.GetValue("\\Device\\slabser" + i);
				if (text == null) {
					continue;
				}
				EvoFoundAtRegistryKey = "\\Device\\slabser" + i;
				result = true;
				if (!port.IsOpen) {
					port.PortName = text;
					if (!IdentifyDevice()) {
						result = false;
					}
					break;
				}
				throw new Exception("Port is already open in IdentifyAndConnectCommPort");
			}
			return result;
		}

		private bool IdentifyUSBDevice() {
			if (usbComm != null) {
				for (int i = 0; i < 5; i++) {
					if (usbComm.Devices.Length == 0) {
						Thread.Sleep(500);
						continue;
					}
					if (usbComm.Devices.Length > 1) {
						throw new Exception("Only one CS / CTS can connect at a time...");
					}
					communicator = Proteus.Instance(usbComm, usbComm.Devices[0].DeviceSerialNumber, usbComm.Devices[0].DeviceDescription);
					if (communicator != null) {
						try {
							Thread.Sleep(2000);
							communicator.GetPartNumber();
							product = new ProteusProduct();
							return true;
						} catch (Exception ex) {
							throw ex;
						}
					}
				}
			}
			throw new Exception("IdentifyDevice failed");
		}

		private bool IdentifyDevice() {
			for (int i = 0; i < 5; i++) {
				Thread.Sleep(500);
				if (port.IsOpen) {
					throw new Exception("Port should never be open when calling IdentifyDevice");
				}
				port.Parity = Parity.None;
				port.DataBits = 8;
				port.StopBits = StopBits.One;
				port.BaudRate = 460800;
				try {
					port.Open();
				} catch (Exception) {
					throw new Exception("Couldn't open the port in IdentifyDevice");
				}
				if (NoDetectionAssumeChameleon) {
					communicator = Chameleon.Instance(port);
					return true;
				}
				if (TestBaudRate()) {
					communicator = Chameleon.Instance(port);
					if (InstantiateProductOnConnection) {
						string partNumber = communicator.GetPartNumber();
						uint num = 0u;
						try {
							num = communicator.FATGetFileAddressOrLength("cs_info.bin", IsAddress: true);
						} catch {
						}
						if (num != 0) {
							product = new Generic();
						} else {
							switch (partNumber) {
								case "EEC1000":
									product = new EEC1000();
									break;
								case "EEC2200":
									product = new EEC2200();
									break;
								case "15001":
								case "EEF1100":
									product = new EEF1100();
									break;
								case "EEF1200":
									product = new EEF1200();
									break;
								case "15004":
								case "15500":
									product = new Evo15004();
									break;
								case "15050":
								case "EEF2100":
									product = new EEF2100();
									break;
								case "EEF2400":
									product = new EEF2400();
									break;
								default:
									product = new Generic();
									break;
							}
						}
					}
					return true;
				}
				port.Close();
				port.BaudRate = 500000;
				try {
					port.Open();
				} catch {
					throw new Exception("Couldn't open the port in IdentifyDevice");
				}
				if (TestBaudRate()) {
					communicator = Evo2.Instance(port);
					if (InstantiateProductOnConnection) {
						if (Instance().communicator.FATGetFileAddressOrLength("cs_info.bin", IsAddress: true) != 0) {
							product = new Generic();
						} else {
							string text = null;
							try {
								text = communicator.GetPartNumber();
							} catch {
								throw new Exception("Unsupported device connected.");
							}
							string a = text;
							if (!(a == "EEF2100-EVO2")) {
								if (a == "EEF2400-EVO2") {
									product = new EEF2400_Evo2();
								} else {
									MessageBox.Show("Didn't recognize Evo2 model!");
								}
							} else {
								product = new EEF2100_Evo2();
							}
						}
					}
					return true;
				}
				port.Close();
			}
			throw new Exception("IdentifyDevice failed");
		}

		private bool TestBaudRate() {
			try {
				byte[] buffer = new byte[1]
				{
					86
				};
				port.ReadExisting();
				port.Write(buffer, 0, 1);
				DateTime now = DateTime.Now;
				TimeSpan t = new TimeSpan(0, 0, 0, 0, 1000);
				while (port.BytesToRead < 6) {
					if (DateTime.Now - now > t) {
						throw new Exception("Timed out on read");
					}
				}
				byte[] array = new byte[port.BytesToRead];
				int num = 0;
				while (port.BytesToRead > 0) {
					array[num] = (byte)port.ReadByte();
					num++;
				}
				int num2 = 0;
				for (int i = 1; i < array.Length - 2; i++) {
					num2 += array[i];
				}
				num2 ^= 0xFFFF;
				if (array.Length < 3) {
					return false;
				}
				if (array[array.Length - 2] != (0xFF00 & num2) >> 8) {
					return false;
				}
				if (array[array.Length - 1] != (0xFF & num2)) {
					return false;
				}
			} catch {
				Thread.Sleep(1000);
				port.Close();
				return false;
			}
			return true;
		}

		public void ResetProgressBar() {
			if (this.ProgressReportEvent != null) {
				this.ProgressReportEvent(this, new ProgressReportEventArgs(null, null, 0));
			}
		}

		public void ReportProgress(string task, string status, int percentageComplete) {
			if (this.ProgressReportEvent != null) {
				this.ProgressReportEvent(this, new ProgressReportEventArgs(task, status, percentageComplete));
			}
		}

		public void ReportOverallProgress(string task, int percentageComplete) {
			if (this.OverallProgressReportEvent != null) {
				this.OverallProgressReportEvent(this, new ProgressReportEventArgs(task, string.Empty, percentageComplete));
			}
		}

		private void Download_ProgressReportEvent(object sender, ProgressChangedEventArgs e) {
			ReportProgress("", e.UserState as string, e.ProgressPercentage);
		}

		private void Upload_ProgressReportEvent(object sender, ProgressChangedEventArgs e) {
			ReportProgress("", e.UserState as string, e.ProgressPercentage);
		}

		private void OnDeviceConnectionEvent() {
			if (this.DeviceConnectionEvent != null) {
				this.DeviceConnectionEvent(this, new EventArgs());
			}
		}

		public void SendDeviceDisconnectionEvent(ConnectionArgs args) {
			if (this.DeviceDisconnectionEvent != null) {
				this.DeviceDisconnectionEvent(this, args);
			}
		}

		public void Dispose() {
			Close();
		}

		public byte[] Decrypt(byte[] baDataToDecrypt) {
			byte[] array = new byte[baDataToDecrypt.Length];
			uint[] array2 = new uint[4]
			{
				1634030625u,
				1766596717u,
				1853122663u,
				560426601u
			};
			uint num = (uint)(baDataToDecrypt.Length / 8);
			if (baDataToDecrypt.Length % 8 > 0) {
				num++;
			}
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < num; i++) {
				num2 = ((num != 0) ? ((int)((float)i / (float)num)) : 0);
				if (num2 != num3) {
					Instance().ReportProgress("Decrypting ", num2 + "% Complete", num2);
					num3 = num2;
				}
				uint num4 = 3337565984u;
				uint num5 = BitConverter.ToUInt32(baDataToDecrypt, i * 8);
				uint num6 = BitConverter.ToUInt32(baDataToDecrypt, i * 8 + 4);
				for (int j = 0; j < 32; j++) {
					num6 -= (((num5 << 4) ^ (num5 >> 5)) + num5) ^ (num4 + array2[(num4 >> 11) & 3]);
					num4 -= 2654435769u;
					num5 -= (((num6 << 4) ^ (num6 >> 5)) + num6) ^ (num4 + array2[num4 & 3]);
				}
				BitConverter.GetBytes(num5).CopyTo(array, i * 8);
				BitConverter.GetBytes(num6).CopyTo(array, i * 8 + 4);
			}
			return array;
		}

		public void SetCustomTuningBrand(string brand) {
			try {
				EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
				fusionService.SetDeviceHasCustomTuning(Instance().Email, Instance().Password, communicator.GetSerialNumber().ToString(), brand);
			} catch (Exception ex) {
				Log("Error setting Custom Tuning : " + ex.Message);
			}
		}

		public void Log(string Comment) {
			try {
				EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
				//fusionService.Log(Instance().Email, Instance().Password, communicator.GetSerialNumber().ToString(), FirmwareIDTarget, CalibrationIDTarget, communicator.ProductIdentifier, Comment);
			} catch {
			}
		}

		public void SetPlatformType() {
			try {
				EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
				fusionService.SetPlatformType(Instance().email, Instance().password, communicator.GetSerialNumber().ToString(), communicator.ProductIdentifier, (int)communicator.PlatformType);
			} catch {
				throw new Exception("Unable to update the Platform Type on Fusion");
			}
		}

		public void Log(string email, string password, string serialNumber, long firmwareIDTarget, long calibrationIDTarget, int productIdentifier, string comment) {
			try {
				EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
				//fusionService.Log(email, password, serialNumber, firmwareIDTarget, calibrationIDTarget, productIdentifier, comment);
			} catch {
			}
		}

		public void SetUpdateComplete() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			fusionService.SetUpdateComplete(Instance().Email, Instance().Password, communicator.GetSerialNumber().ToString(), communicator.ProductIdentifier);
			communicator.FinalizeUpdate();
		}

		public void SetUpdateCancelled() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			//fusionService.Log(Instance().Email, Instance().Password, communicator.GetSerialNumber().ToString(), 0L, 0L, communicator.ProductIdentifier, "Update Cancelled");
		}

		public bool Download(string RelativePathAndName, ref MemoryStream ms) {
			return Download(RelativePathAndName, ref ms, showProgress: true);
		}

		public bool Download(string RelativePathAndName, ref MemoryStream ms, bool showProgress) {
			if (fileTransferDownload1.IsBusy) {
				Thread.Sleep(1000);
				if (fileTransferDownload1.IsBusy) {
					MessageBox.Show("Download worker thread is still busy, please wait", "Busy");
				}
				return false;
			}
			fileTransferDownload1.AutoSetChunkSize = true;
			fileTransferDownload1.RemoteFileName = RelativePathAndName;
			fileTransferDownload1.IncludeHashVerification = true;
			ProgressChangedEventHandler value = Download_ProgressReportEvent;
			if (showProgress) {
				fileTransferDownload1.ProgressChanged += value;
			}
			fileTransferDownload1.RunWorkerSync(ref ms);
			if (showProgress) {
				fileTransferDownload1.ProgressChanged -= value;
			}
			return true;
		}

		public bool UploadStockFile(ref MemoryStream ms, ref string RemoteRelativePathAndName) {
			fileTransferUpload1 = new FileTransferUpload();
			if (fileTransferUpload1.IsBusy) {
				Thread.Sleep(1000);
				if (fileTransferUpload1.IsBusy) {
					MessageBox.Show("Upload worker thread is still busy, please wait", "Busy");
				}
				return false;
			}
			fileTransferUpload1.AutoSetChunkSize = true;
			fileTransferUpload1.RemoteFileName = RemoteRelativePathAndName;
			RemoteRelativePathAndName = fileTransferUpload1.RemoteFileName;
			fileTransferUpload1.IncludeHashVerification = true;
			fileTransferUpload1.ProgressChanged += Upload_ProgressReportEvent;
			fileTransferUpload1.RunWorkerSync(ref ms);
			return true;
		}

		public bool VerifyEmailAndPassword(string email, string password) {
			try {
				string modelString = new EdgeDeviceLibrary.FusionService.FusionService().GetModelString(email, password, 3);
				return true;
			} catch (WebException ex) {
				string text = ex.Message;
				if (ex.InnerException != null) {
					text = text + " **Inner: " + ex.InnerException.Message;
				}
				WriteExceptionLog(text);
				throw new Exception("Fusion was unable to connect to the server. This may be caused by firewalls or other network issues.");
			} catch (Exception ex2) {
				string text2 = ex2.Message;
				if (ex2.InnerException != null) {
					text2 = text2 + " **Inner: " + ex2.InnerException.Message;
				}
				WriteExceptionLog(text2);
				return false;
			}
		}

		private void WriteExceptionLog(string text) {
			try {
				DateTime now = DateTime.Now;
				string text2 = now.Year + "_";
				if (now.Month < 10) {
					text2 += "0";
				}
				text2 = text2 + now.Month + "_";
				if (now.Day < 10) {
					text2 += "0";
				}
				text2 += now.Day;
				FileStream fileStream = new FileStream("ExcLog_" + text2 + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
				StreamWriter streamWriter = new StreamWriter(fileStream);
				fileStream.Seek(0L, SeekOrigin.End);
				streamWriter.WriteLine(now.ToString() + ": " + text);
				streamWriter.Flush();
				streamWriter.Close();
			} catch (Exception) {
			}
		}

		public bool DoServerCommands() {
			bool result = false;
			string[] specificFilesList = communicator.GetSpecificFilesList();
			if (specificFilesList != null && specificFilesList.Length != 0) {
				communicator.UploadFiles(string.Join("|", specificFilesList), UploadListType.SpecificFiles);
				communicator.ClearGetSpecificFiles();
				result = true;
				dc.Log("Get Specific Files Completed");
			}
			if (communicator.IsGetAllCriticalFilesSet()) {
				communicator.UploadFiles(communicator.GetCriticalFilesList(), UploadListType.CriticalFiles);
				communicator.ClearGetAllCriticalFiles();
				result = true;
				dc.Log("Get All Critical Files Completed");
			}
			if (communicator.IsGetAllFilesSet()) {
				communicator.UploadAllFiles();
				communicator.ClearGetAllFiles();
				result = true;
				dc.Log("Get All Files Completed");
			}
			if (communicator.IsMakeStockWriterSet()) {
				if (communicator.SaveSettings()) {
					Log("Settings saved");
					result = false;
				} else {
					Log("Tried to settings saved, but failed.  This is usually caused by blank settings.");
					result = true;
				}
			} else {
				if (communicator.IsSetStockFlagSet()) {
					if (communicator.SetProgrammingInterruptedFlag()) {
						Log("Set ProgrammingInterruptedFlag");
					} else {
						Log("Tried to set the WriteStock flag, but failed.  This is usually caused by blank settings.");
					}
					communicator.ClearSetStock();
					result = true;
				}
				if (communicator.IsEraseSettingsSet()) {
					if (communicator.EraseSettings()) {
						Log("Erased settings.");
					} else {
						Log("Failed to erase settings.  They were probably blank already.");
					}
					communicator.ClearEraseSettings();
					result = true;
				}
				if (communicator.IsResetToFactorySet()) {
					if (communicator.ResetToFactorySettings()) {
						Log("Reset to factory settings.");
					} else {
						Log("Device did not reset to factory settings.");
					}
					communicator.ClearResetToFactory();
					result = true;
				}
			}
			if (communicator.SendSupportFilesToDevice(lastStep: false)) {
				Log("Files Sent To Device (Before Update)");
				result = true;
			}
			return result;
		}

		public void Notify_ReceivedNewStockFiles(long deviceTableID, string uploadedFilesList) {
			try {
				new EdgeDeviceLibrary.FusionService.FusionService().Notify_ReceivedNewStockFiles(deviceTableID, uploadedFilesList);
			} catch (Exception) {
			}
		}
	}
}
