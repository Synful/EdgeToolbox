#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using EdgeDeviceLibrary.FusionService;
using EdgeDeviceLibrary.Models;
using ProteusCommand;

namespace EdgeDeviceLibrary.Communicator {
	public abstract class Communicator_Base {
		public enum MemoryLocation {
			OnProcessor,
			External,
			EEPROM
		}

		protected byte SecID = 0;

		protected uint ErasedSector = 0u;

		protected const byte SecID_IFlash = 70;

		protected const byte SecID_EEPROM = 69;

		protected const byte SecID_EFlash = 67;

		private bool isFileFormatEncryptedS19;

		protected int nProductIdentifier;

		protected long nProductDatabaseID;

		protected SerialPort port;

		protected PlatformTypes _PlatformType = PlatformTypes.NotSet;

		private string addresses;

		private byte[] savedSettings;

		public bool IsFileFormatEncryptedS19 {
			get {
				return isFileFormatEncryptedS19;
			}
			set {
				isFileFormatEncryptedS19 = value;
			}
		}

		public PlatformTypes PlatformType => _PlatformType;

		public virtual string FirmwareIDTargetPath {
			get {
				DeviceConnector deviceConnector = DeviceConnector.Instance();
				if (deviceConnector != null) {
					return "_frm_cal\\" + deviceConnector.FirmwareIDTarget + ".bin";
				}
				return "none";
			}
		}

		public virtual string CalibrationIDTargetPath {
			get {
				DeviceConnector deviceConnector = DeviceConnector.Instance();
				if (deviceConnector != null) {
					return "_frm_cal\\" + deviceConnector.CalibrationIDTarget + ".bin";
				}
				return "none";
			}
		}

		public int ProductIdentifier => nProductIdentifier;

		public virtual bool IsResetNecessary => false;

		public virtual bool IsUnplugResetNecessary => false;

		public string[] AvailableFirmware {
			get {
				EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
				return fusionService.GetAvailableFirmware(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, ProductIdentifier);
			}
		}

		public string[] AllCalibrations {
			get {
				EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
				return fusionService.GetAvailableCalibrations(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, ProductIdentifier, 0L);
			}
		}

		public string[] AvailableCalibrations {
			get {
				EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
				string[] availableCalibrations = fusionService.GetAvailableCalibrations(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, ProductIdentifier, DeviceConnector.Instance().FirmwareIDTarget);
				if (availableCalibrations == null) {
					availableCalibrations = fusionService.GetAvailableCalibrations(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, ProductIdentifier, 0L);
				} else if (availableCalibrations[0] == null) {
					availableCalibrations = fusionService.GetAvailableCalibrations(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, ProductIdentifier, 0L);
				}
				return availableCalibrations;
			}
		}

		public long[] UpdateFirmwareCalibrationIDs => GetFirmwareCalibrationIDs(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);

		public virtual bool HasUnlockableUpgrades => false;

		protected abstract byte[] GetBootLoaderVersion();

		public abstract string GetPartNumber(bool cachedOk = false);

		public virtual string GetStockFolderName() {
			return GetPartNumber();
		}

		public abstract void ReadVersionFile(ref MemoryStream ms);

		public abstract string GetBootloaderVersionString();

		public abstract string GetFirmwareVersionString();

		public abstract string GetCalibrationVersionString();

		public abstract short[] GetFirmwareVersion();

		public abstract short[] GetCalibrationVersion();

		public virtual void Cleanup() {
		}

		protected virtual byte[] ReadByteArrayFromPort(int nByteCount, int msecTimeout, bool VerifyChecksum) {
			DateTime now = DateTime.Now;
			TimeSpan t = new TimeSpan(0, 0, 0, 0, msecTimeout);
			if (nByteCount > 0) {
				while (port.BytesToRead < nByteCount) {
					Application.DoEvents();
					if (DateTime.Now - now > t) {
						byte[] array = new byte[port.BytesToRead];
						for (int i = 0; i < port.BytesToRead; i++) {
							array[i] = (byte)port.ReadByte();
						}
						throw new Exception("Timed out on read");
					}
				}
			} else {
				int bytesToRead = port.BytesToRead;
				while (DateTime.Now - now < t) {
					if (bytesToRead != port.BytesToRead) {
						bytesToRead = port.BytesToRead;
						now = DateTime.Now;
					}
					Thread.Sleep(10);
				}
			}
			byte[] array2 = new byte[port.BytesToRead];
			int num = 0;
			while (port.BytesToRead > 0) {
				array2[num] = (byte)port.ReadByte();
				num++;
			}
			bool flag = true;
			if (VerifyChecksum) {
				int num2 = 0;
				for (int j = 1; j < array2.Length - 2; j++) {
					num2 += array2[j];
				}
				num2 ^= 0xFFFF;
				if (array2.Length < 3) {
					return array2;
				}
				if (array2[array2.Length - 2] != (0xFF00 & num2) >> 8) {
					flag = false;
				}
				if (array2[array2.Length - 1] != (0xFF & num2)) {
					flag = false;
				}
			}
			if (flag) {
				return array2;
			}
			return null;
		}

		public virtual void RebootDevice(bool IsHardReset) {
			RebootDevice(IsHardReset, doConnectEvent: true);
		}

		public virtual void RebootDevice(bool IsHardReset, bool doConnectEvent) {
			byte[] array = new byte[1];
			if (IsHardReset) {
				array[0] = 72;
			} else {
				array[0] = 66;
			}
			port.Write(array, 0, 1);
			DeviceConnector.Instance().Close();
			Thread.Sleep(3000);
			DeviceConnector.Instance().IsDeviceConnected(doConnectEvent);
		}

		internal virtual byte[] ReadFlash(MemoryLocation location, uint InAddr, ushort InSize) {
			byte[] bytes = BitConverter.GetBytes(InAddr);
			byte[] bytes2 = BitConverter.GetBytes(InSize);
			byte[] array = new byte[7]
			{
				82,
				0,
				0,
				0,
				0,
				0,
				0
			};
			switch (location) {
				case MemoryLocation.EEPROM:
					if (DeviceConnector.Instance().communicator.GetType() != typeof(Evo2)) {
						throw new Exception("Can't read EEPROM on a Chameleon");
					}
					array[1] = 69;
					break;
				case MemoryLocation.External:
					array[1] = 67;
					break;
				case MemoryLocation.OnProcessor:
					array[1] = 70;
					break;
			}
			array[2] = bytes[2];
			array[3] = bytes[1];
			array[4] = bytes[0];
			array[5] = bytes2[1];
			array[6] = bytes2[0];
			port.Write(array, 0, array.Length);
			byte[] array2 = ReadByteArrayFromPort(InSize + 3, 10000, VerifyChecksum: true);
			byte[] array3 = new byte[array2.Length - 3];
			for (int i = 1; i < array2.Length - 2; i++) {
				array3[i - 1] = array2[i];
			}
			return array3;
		}

		protected virtual bool WriteFlash(byte SecID, uint Address, byte[] dataToWrite) {
			return WriteFlash(SecID, Address, dataToWrite, normalWrite: true);
		}

		protected virtual bool WriteFlash(byte SecID, uint Address, byte[] dataToWrite, bool normalWrite) {
			bool flag = false;
			while (!flag) {
				if (dataToWrite.Length > 512 && SecID != 66) {
					throw new Exception("Only call WriteFlash with dataToWrite length <=0x200 (512 decimal)");
				}
				ushort num = 0;
				foreach (byte b in dataToWrite) {
					num = (ushort)(num + b);
				}
				num = (ushort)(65535 - num);
				byte[] array = new byte[1];
				if (normalWrite) {
					array[0] = 87;
				} else {
					array[0] = 65;
				}
				port.Write(array, 0, 1);
				array[0] = SecID;
				port.Write(array, 0, 1);
				byte[] buffer = SwitchEndianness(BitConverter.GetBytes(Address));
				port.Write(buffer, 1, 3);
				byte[] bytes = BitConverter.GetBytes(dataToWrite.Length);
				array = new byte[2];
				array[1] = bytes[0];
				array[0] = bytes[1];
				port.Write(array, 0, 2);
				port.Write(dataToWrite, 0, dataToWrite.Length);
				bytes = BitConverter.GetBytes(num);
				array[1] = bytes[0];
				array[0] = bytes[1];
				port.Write(array, 0, 2);
				array = ReadByteArrayFromPort(2, 10000, VerifyChecksum: false);
				switch (array[1]) {
					case 111:
						flag = true;
						continue;
					case 99:
						flag = false;
						continue;
					case 102:
						throw new Exception("Flash access violation in Communicator Base");
				}
				char c = (char)array[1];
				string text = c + "(" + array[1].ToString("X") + ")";
				switch (array[1]) {
					case 0:
						text = "IAP_SUCCESS";
						break;
					case 1:
						text = "IAP_INVALID_COMMAND";
						break;
					case 2:
						text = "IAP_SRC_ADDR_ERR";
						break;
					case 3:
						text = "IAP_DST_ADDR_ERR";
						break;
					case 4:
						text = "IAP_SRC_ADDR_NOMAP";
						break;
					case 5:
						text = "IAP_DST_ADDR_NOMAP";
						break;
					case 6:
						text = "IAP_COUNT_ERR";
						break;
					case 7:
						text = "IAP_INVALID_SECTOR";
						break;
					case 9:
						text = "IAP_SECTOR_LOCKED";
						break;
					case 11:
						text = "IAP_BUSY";
						break;
				}
				string[] obj = new string[8]
				{
					"Writeflash failed ",
					Address.ToString("X"),
					" (wrong endianess) with error ",
					null,
					null,
					null,
					null,
					null
				};
				c = (char)array[0];
				obj[3] = c.ToString();
				obj[4] = "(";
				obj[5] = array[0].ToString("X");
				obj[6] = ") - ";
				obj[7] = text;
				throw new Exception(string.Concat(obj));
			}
			return flag;
		}

		public byte[] SwitchEndianness(byte[] ba) {
			byte[] array = new byte[ba.Length];
			int num = 0;
			for (int num2 = ba.Length - 1; num2 >= 0; num2--) {
				array[num] = ba[num2];
				num++;
			}
			return array;
		}

		public abstract uint GetSerialNumber();

		public void FATClearFile(string DeviceFileName) {
			byte[] array = new byte[20]
			{
				67,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};
			byte[] bytes = Encoding.Default.GetBytes(DeviceFileName);
			for (int i = 1; i < bytes.Length + 1; i++) {
				array[i] = bytes[i - 1];
			}
			array[bytes.Length + 1] = 0;
			port.Write(array, 0, bytes.Length + 2);
			byte[] array2 = new byte[1]
			{
				1
			};
			while (array2[0] != 99) {
				array2 = ReadByteArrayFromPort(1, 10000, VerifyChecksum: true);
				if (array2[0] == 117) {
					throw new Exception("\"Unknown\" response when attempting to erase file " + DeviceFileName + ".");
				}
			}
		}

		public void UploadFileList() {
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			MemoryStream ms = null;
			StreamWriter streamWriter = null;
			try {
				string[] array = FATGetFileList();
				if (array.Length != 0) {
					ms = new MemoryStream();
					streamWriter = new StreamWriter(ms, Encoding.ASCII);
					string[] array2 = array;
					foreach (string value in array2) {
						streamWriter.WriteLine(value);
					}
					streamWriter.Flush();
					ms.Position = 0L;
					string RemoteRelativePathAndName = "_uploads\\devices\\" + DeviceConnector.Instance().communicator.GetDevicesTableID() + "\\DeviceFileList.txt";
					deviceConnector.UploadStockFile(ref ms, ref RemoteRelativePathAndName);
				}
			} catch (Exception value2) {
				Trace.WriteLine(value2);
			} finally {
				streamWriter?.Dispose();
				ms?.Dispose();
			}
		}

		public virtual bool IsUpdateEnabledForDevice(out string updateMessage) {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.IsUpdateEnabledForDevice(ProductIdentifier, out updateMessage);
		}

		public virtual bool NeedsUserInputVIN(out VINFormat vinFormat) {
			vinFormat = null;
			return false;
		}

		public virtual void SaveUserVIN(string vin) {
		}

		public virtual string[] FATGetFileList() {
			byte[] ba = ReadFlash(MemoryLocation.External, 0u, 4);
			ba = SwitchEndianness(ba);
			uint num = BitConverter.ToUInt32(ba, 0);
			if (num >= 65535) {
				throw new InvalidDataException("FATCount over 65535, is " + num);
			}
			string[] array = new string[num];
			uint num2 = 4u;
			byte[] array2 = new byte[0];
			for (int i = 1; i < num + 1; i++) {
				Application.DoEvents();
				byte[] bytes = ReadFlash(MemoryLocation.External, num2, 24);
				string @string = Encoding.ASCII.GetString(bytes);
				@string = @string.Remove(@string.IndexOf('\0'));
				@string = (array[i - 1] = @string.Substring(0, @string.Length));
				num2 += 24;
			}
			return array;
		}

		public virtual uint FATGetFileAddressOrLength(string filenameToRead, bool IsAddress) {
			bool flag = false;
			uint result = 0u;
			uint result2 = 0u;
			byte[] ba = ReadFlash(MemoryLocation.External, 0u, 4);
			ba = SwitchEndianness(ba);
			uint num = BitConverter.ToUInt32(ba, 0);
			if (num >= 65535) {
				throw new InvalidDataException("FATCount over 65535, is " + num);
			}
			uint num2 = 4u;
			byte[] array = new byte[0];
			for (int i = 1; i < num + 1; i++) {
				Application.DoEvents();
				byte[] array2 = ReadFlash(MemoryLocation.External, num2, 24);
				string @string = Encoding.ASCII.GetString(array2);
				@string = @string.Remove(@string.IndexOf('\0'));
				@string = @string.Substring(0, @string.Length);
				if (@string == filenameToRead) {
					flag = true;
					byte[] array3 = new byte[4];
					Array.Copy(array2, 16, array3, 0, 4);
					array3 = SwitchEndianness(array3);
					result = BitConverter.ToUInt32(array3, 0);
					Array.Copy(array2, 20, array3, 0, 4);
					array3 = SwitchEndianness(array3);
					result2 = BitConverter.ToUInt32(array3, 0);
					break;
				}
				num2 += 24;
			}
			if (flag) {
				if (IsAddress) {
					return result;
				}
				return result2;
			}
			return 0u;
		}

		public virtual bool ReadFat(string filenameToRead, bool bSkipError, bool ShowsProgressBar, ref MemoryStream ms) {
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			port.ReadExisting();
			uint num = FATGetFileAddressOrLength(filenameToRead, IsAddress: true);
			uint num2 = FATGetFileAddressOrLength(filenameToRead, IsAddress: false);
			if (num == 0 && num2 == 0) {
				return false;
			}
			int num3 = 128;
			byte[] array = new byte[num2];
			for (int i = 0; i < num2; i += num3) {
				int percentageComplete = 0;
				if (num2 != 0) {
					percentageComplete = (int)((float)i / (float)num2 * 100f);
				}
				deviceConnector.ReportProgress("Transferring from device...", percentageComplete + "% Complete", percentageComplete);
				Application.DoEvents();
				if (num2 - i < num3) {
					num3 = (int)(num2 - i);
				}
				byte[] array2 = ReadFlash(MemoryLocation.External, num, (ushort)num3);
				array2.CopyTo(array, i);
				num += (uint)num3;
			}
			ms.Write(array, 0, array.Length);
			deviceConnector.ReportProgress("", "", 0);
			ms.Seek(0L, SeekOrigin.Begin);
			return true;
		}

		public virtual bool WriteFat(byte[] baFileToWrite, string DeviceFileName) {
			return WriteFat(baFileToWrite, DeviceFileName, fileEncrypted: false);
		}

		public virtual bool WriteFat(byte[] baFileToWrite, string DeviceFileName, bool fileEncrypted) {
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			int num = 256;
			uint num2 = FATGetFileAddressOrLength(DeviceFileName, IsAddress: true);
			FATClearFile(DeviceFileName);
			bool normalWrite = deviceConnector.communicator.GetBootLoaderVersionDouble(null) < 2.7 || (fileEncrypted ? true : false);
			for (int i = 0; i < baFileToWrite.Length; i += num) {
				int percentageComplete = 0;
				if (baFileToWrite.Length != 0) {
					percentageComplete = (int)((float)i / (float)baFileToWrite.Length * 100f);
				}
				deviceConnector.ReportProgress("Transferring to device...", percentageComplete + "% Complete", percentageComplete);
				Application.DoEvents();
				if (baFileToWrite.Length - i < num) {
					num = baFileToWrite.Length - i;
				}
				byte[] array = new byte[num];
				for (int j = 0; j < array.Length; j++) {
					array[j] = baFileToWrite[i + j];
				}
				if (!WriteFlash(67, num2 + (uint)i, array, normalWrite)) {
					return false;
				}
			}
			deviceConnector.ReportProgress("", "", 0);
			return true;
		}

		public virtual bool SendEncryptedS19File(ref MemoryStream ms) {
			byte[] bytes = DeviceConnector.Instance().Decrypt(ms.ToArray());
			return Send_S19_File(Encoding.Default.GetString(bytes));
		}

		public virtual bool ResetDevice(int maxWait) {
			return false;
		}

		public virtual bool WaitForDeviceReconnect(int? maxWait) {
			return false;
		}

		public virtual bool SendEncryptedBinFile(ref MemoryStream ms, uint Address, byte SecID) {
			ms.Seek(0L, SeekOrigin.Begin);
			int num;
			switch (SecID) {
				case 70:
					if (Address == 8192) {
						ClearInternalFlash(8192u, 24575u);
					} else {
						ClearInternalFlash(24576u, 253951u);
					}
					num = 512;
					break;
				case 66:
					num = 8192;
					break;
				default:
					ClearExternalFlash();
					num = 128;
					break;
			}
			byte[] array = new byte[num];
			for (int i = 0; i < ms.Length; i += num) {
				int percentageComplete = 0;
				if (ms.Length > 0) {
					percentageComplete = (int)((float)i / (float)ms.Length * 100f);
				}
				DeviceConnector.Instance().ReportProgress("Writing flash memory", "", percentageComplete);
				int num2 = ms.Read(array, 0, num);
				byte[] array2 = new byte[num2];
				Array.Copy((Array)array, (Array)array2, (long)num2);
				WriteFlash(SecID, Address, array2);
				Address += (uint)num2;
			}
			return true;
		}

		public abstract bool Send_S19_File(string SendData);

		protected string GetNextLine(string SendData, ref int startIndex) {
			int num = 2;
			int num2 = SendData.IndexOf("\r\n", startIndex);
			if (num2 == -1) {
				num2 = SendData.IndexOf("\n", startIndex);
				num = 1;
			}
			string result = SendData.Substring(startIndex, num2 - startIndex);
			startIndex = num2 + num;
			return result;
		}

		protected abstract bool Send_S19_Line(string OutData);

		public void SectorErase(byte SectorID, uint baAddress) {
			addresses = addresses + $"{baAddress:X}" + " ";
			port.Write("E");
			byte[] buffer = new byte[1]
			{
				SectorID
			};
			port.Write(buffer, 0, 1);
			buffer = new byte[3]
			{
				BitConverter.GetBytes(baAddress)[2],
				BitConverter.GetBytes(baAddress)[1],
				BitConverter.GetBytes(baAddress)[0]
			};
			port.Write(buffer, 0, 3);
			bool flag = false;
			while (!flag) {
				byte[] array = ReadByteArrayFromPort(2, 1000, VerifyChecksum: false);
				if (array[1] == 112) {
					flag = false;
					continue;
				}
				if (array[1] == 99) {
					flag = true;
					continue;
				}
				throw new Exception("Failed to erase sector at address " + baAddress + ", result was " + BitConverter.ToString(array));
			}
		}

		public string GetFwCalDescription(long fwCalID) {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.GetFwCalDescription(fwCalID);
		}

		public string GetFirmwareDescription(long firmwareIDTarget) {
			try {
				return FixCRLF(GetFwCalDescription(firmwareIDTarget));
			} catch {
				return "";
			}
		}

		private string FixCRLF(string s) {
			if (s.IndexOf('\r') == -1) {
				s = s.Replace("\n", "\r\n");
			}
			return s;
		}

		public string GetCalibrationDescription(long calibrationIDTarget) {
			try {
				return FixCRLF(GetFwCalDescription(calibrationIDTarget));
			} catch {
				return "";
			}
		}

		public ushort GetModelFromUserSerialNum(string email, string password, long serialNum) {
			ushort num = 0;
			try {
				num = (ushort)new EdgeDeviceLibrary.FusionService.FusionService().GetModelFromUserSerialNum(email, password, serialNum);
			} catch (WebException) {
				num = 0;
			}
			nProductIdentifier = num;
			return num;
		}

		public long[] GetFirmwareCalibrationIDs(string email, string password, string serialnumber, int productid) {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			if (productid == 0) {
				productid = fusionService.GetModelFromUserSerialNum(email, password, Convert.ToUInt32(serialnumber));
			}
			return fusionService.GetTargetFirmCalIDs(email, password, serialnumber, productid);
		}

		public long[] GetCurrentFwCalIDs(string email, string password, string serialnumber, int productid) {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			if (productid == 0) {
				productid = fusionService.GetModelFromUserSerialNum(email, password, Convert.ToUInt32(serialnumber));
			}
			return fusionService.GetCurrentFwCal(email, password, productid);
		}

		public long[] GetUpdateFirmwareAndCalibrationIDs(string serialNumber, string existingFirmwareVersion, string existingCalibrationVersion) {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.GetUpdateFirmwareAndCalibrationIDs(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, nProductIdentifier, GetSerialNumber().ToString(), existingFirmwareVersion, existingCalibrationVersion);
		}

		public bool IsGetAllFilesSet() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.IsGetAllFilesSet(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public bool IsGetAllCriticalFilesSet() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.IsGetAllCriticalFilesSet(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public string GetCriticalFilesList() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.GetCriticalFilesList(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, nProductIdentifier);
		}

		public string[] GetSpecificFilesList() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.GetSpecificFilesList(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public void ClearGetSpecificFiles() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			fusionService.ClearGetSpecificFiles(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public bool IsMakeStockWriterSet() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.IsMakeStockWriterSet(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public bool IsSetStockFlagSet() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.IsSetStockFlagSet(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public bool IsEraseSettingsSet() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.IsEraseSettingsSet(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public virtual bool SendSupportFilesToDevice(bool lastStep) {
			return false;
		}

		public bool IsResetToFactorySet() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.IsResetToFactorySet(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public bool DeviceCanBeUpdated() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.DeviceCanBeUpdated(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public bool IsForceUpdateSet() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.IsForceUpdateSet(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public long GetDevicesTableID() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			return fusionService.GetDeviceID(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public abstract int GetProgrammedToLevel();

		public bool IsProgrammedToLevel() {
			//int programmedToLevel = GetProgrammedToLevel();
			//if (programmedToLevel < 0)
			//{
			//	return false;
			//}
			//if (programmedToLevel > 0)
			//{
			//	return true;
			//}
			return false;
		}

		public abstract bool IsProgrammingInterrupted();

		public bool SetProgrammingInterruptedFlag() {
			byte[] array = ReadSettings();
			if (array == null) {
				return false;
			}
			array[3] = 1;
			WriteSettings(array);
			return true;
		}

		public virtual void WriteSettings(byte[] baSettings) {
			SectorErase(67, 8257536u);
			if (GetBootLoaderVersionDouble(null) >= 2.7) {
				WriteFlash(67, 8257536u, baSettings, normalWrite: false);
			} else {
				WriteFlash(67, 8257536u, baSettings, normalWrite: true);
			}
		}

		public abstract byte[] ReadSettings();

		public virtual bool EraseSettings() {
			byte[] array = ReadSettings();
			if (array == null) {
				return false;
			}
			for (int i = 0; i < array.Length; i++) {
				array[i] = byte.MaxValue;
			}
			WriteSettings(array);
			return true;
		}

		public virtual bool ResetToFactorySettings() {
			return false;
		}

		public bool SaveSettings() {
			savedSettings = ReadSettings();
			if (savedSettings == null) {
				return false;
			}
			return true;
		}

		public bool RestoreSettings() {
			if (savedSettings == null) {
				return false;
			}
			WriteSettings(savedSettings);
			return true;
		}

		public virtual uint GetUnlockCode(string codeName) {
			return 0u;
		}

		public virtual bool UnlockFeature(string codeName) {
			return false;
		}

		public virtual void UpdateComplete() {
		}

		public virtual void FinalizeUpdate() {
		}

		public void UploadAllFiles() {
			UploadFiles(string.Empty, UploadListType.AllFiles);
		}

		public virtual void UploadFiles(string files, UploadListType listType) {
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			string[] array = FATGetFileList();
			List<string> list = new List<string>();
			bool flag = false;
			int num = 0;
			int num2 = 0;
			if (files == null || files == string.Empty || listType == UploadListType.AllFiles) {
				flag = true;
				num = array.Length;
			} else {
				string[] array2 = files.Split('|');
				string[] array3 = array2;
				foreach (string text in array3) {
					list.Add(text.ToLower());
				}
				num = list.Count;
			}
			for (int j = 0; j < array.Length; j++) {
				if (flag || list.Contains(array[j].ToLower())) {
					string text2 = array[j];
					int percentageComplete = 0;
					if (num > 0) {
						percentageComplete = (int)((float)num2 / (float)num * 100f);
					}
					deviceConnector.ReportOverallProgress("Overall Files Transfer..." + percentageComplete + "%", percentageComplete);
					Application.DoEvents();
					MemoryStream ms = new MemoryStream();
					ReadFat(text2, bSkipError: false, ShowsProgressBar: false, ref ms);
					string RemoteRelativePathAndName = "_uploads\\devices\\" + deviceConnector.communicator.GetDevicesTableID() + "\\" + text2;
					DeviceConnector.Instance().UploadStockFile(ref ms, ref RemoteRelativePathAndName);
					num2++;
				}
			}
			deviceConnector.ReportOverallProgress(string.Empty, 0);
			Application.DoEvents();
			if (flag) {
				DeviceConnector.Instance().Log("Uploaded all files");
			} else {
				DeviceConnector.Instance().Log("Uploaded requested files");
			}
		}

		public void ClearMakeStockWriter() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			fusionService.ClearMakeStockWriter(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public void ClearSetStock() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			fusionService.ClearSetStock(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public void ClearGetAllFiles() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			fusionService.ClearGetAllFiles(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public void ClearGetAllCriticalFiles() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			fusionService.ClearGetAllCriticalFiles(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public void ClearEraseSettings() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			fusionService.ClearEraseSettings(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public void ClearResetToFactory() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			fusionService.ClearResetToFactory(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), nProductIdentifier);
		}

		public virtual double GetBootLoaderVersionDouble(string bootloaderVer) {
			if (bootloaderVer == null) {
				bootloaderVer = GetBootloaderVersionString();
			}
			double result = 0.0;
			if (bootloaderVer.Length > 0 && bootloaderVer.IndexOf('v') == 0 && bootloaderVer.IndexOf('.') != -1) {
				result = Convert.ToDouble(bootloaderVer.Substring(1), CultureInfo.CreateSpecificCulture("en-US"));
			}
			return result;
		}

		protected virtual void GetBootLoaderStageStrings(string fullString, ref string stage1, ref string stage2) {
			stage1 = fullString.Trim();
			stage2 = stage1;
			string[] array = stage1.Split(',');
			if (array.Length != 0) {
				stage1 = array[0].Trim();
			}
			if (array.Length > 1) {
				stage2 = array[1].Trim();
			}
		}

		protected virtual bool VerifyStage2IsGood(string bootLoaderVersion) {
			bool result = true;
			if (GetBootLoaderVersionDouble(bootLoaderVersion) >= 2.61) {
				byte[] array = ReadFlash(MemoryLocation.OnProcessor, 8192u, 4);
				if (array.Length < 4) {
					throw new Exception("Cannot read stage 2 flash area");
				}
				result = array[0] == 172 && array[1] == 0 && array[2] == 159 && array[3] == 229;
			}
			return result;
		}

		public virtual void UpdateStage2(string webStage2) {
			IsFileFormatEncryptedS19 = false;
			MemoryStream ms = new MemoryStream();
			DeviceConnector.Instance().Download("_bootloaders\\BL" + webStage2.Substring(1).Replace(".", "") + ".bin", ref ms);
			SendEncryptedBinFile(ref ms, 8192u, 70);
		}

		public virtual string[] GetFusionBootloaderInfo() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			if (nProductIdentifier == 65535) {
				throw new Exception("Product Identifier not set!");
			}
			return fusionService.GetBootloaderInfo(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, nProductIdentifier);
		}

		public virtual void VerifyBootloader() {
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			if (nProductIdentifier == 65535) {
				throw new Exception("Product Identifier not set!");
			}
			string[] bootloaderInfo = fusionService.GetBootloaderInfo(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, nProductIdentifier);
			if (bootloaderInfo[0] == null || bootloaderInfo[0].Trim().Length <= 0) {
				return;
			}
			string stage = "";
			string stage2 = "";
			GetBootLoaderStageStrings(bootloaderInfo[0], ref stage, ref stage2);
			bool flag = stage != stage2 && GetBootLoaderVersionDouble(stage2) >= 2.7;
			string bootloaderVersionString = GetBootloaderVersionString();
			if (!flag) {
				return;
			}
			int i = 0;
			bool flag2 = false;
			string empty = string.Empty;
			for (; i < 4; i++) {
				if (flag2) {
					break;
				}
				RebootDevice(IsHardReset: false, doConnectEvent: false);
				empty = GetBootloaderVersionString();
				flag2 = ((GetBootLoaderVersionDouble(empty) >= 2.7) ? true : false);
			}
			if (!flag2) {
				throw new Exception("Unable to boot to stage 2");
			}
		}

		public void ClearExternalFlash() {
			SecID = 67;
			port.DiscardInBuffer();
			port.Write("M");
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			while (num2 < 300) {
				num3++;
				if (num3 > 100) {
					num3 = 0;
				}
				DeviceConnector.Instance().ReportProgress("Preparing flash memory", "Please wait...", num3);
				byte[] array = ReadByteArrayFromPort(2, 2000, VerifyChecksum: false);
				if (array[1] == 112) {
					num2++;
					num++;
					Application.DoEvents();
					continue;
				}
				if ((array[0] != 77 && array[0] != 109) || array[1] != 99) {
					throw new Exception("Mass erase appears to have failed.");
				}
				break;
			}
		}

		public abstract void ClearInternalFlash(uint address, uint endaddress);

		public uint GetFwCalChecksum(string serverPathFwCal) {
			uint num = 0u;
			try {
				return new EdgeDeviceLibrary.FusionService.FusionService().GetFwCalChecksum(serverPathFwCal);
			} catch (Exception) {
				return 0u;
			}
		}

		public virtual string GetUpgradeOptions() {
			return null;
		}

		public virtual bool HasSkuChanged() {
			return false;
		}

		public virtual void PrepareForSkuChange() {
		}

		public virtual string[] GetUnsupportedSkus() {
			return new string[0];
		}

		public virtual bool NeedsGmsk() {
			return false;
		}

		public virtual bool TryGetGmskRequests(out List<GmskRequest> requests) {
			requests = null;
			return false;
		}

		public virtual bool SetGmsk(string key, GmskRequest request) {
			return false;
		}
	}
}
