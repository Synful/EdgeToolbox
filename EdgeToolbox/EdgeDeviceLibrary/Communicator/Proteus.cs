#define TRACE
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using Edge.Common.DataStructures;
using Edge.Common.Events;
using Edge.Common.IO;
using EdgeDeviceLibrary.FusionService;
using EdgeDeviceLibrary.Models;
using EncryptorAssembly;
using MXZipLibrary;
using ProteusCommand;
using ProteusCommand.MiniAppCommands;
using USBAssembly;

namespace EdgeDeviceLibrary.Communicator {
	public class Proteus : Communicator_Base {
		private class FileTreeComparer : IComparer<TreeNode<F_FIND>> {
			private static FileTreeComparer _comparer = null;

			public static FileTreeComparer Instance {
				get {
					if (_comparer == null) {
						_comparer = new FileTreeComparer();
					}
					return _comparer;
				}
			}

			public int Compare(TreeNode<F_FIND> lhs, TreeNode<F_FIND> rhs) {
				if (lhs.Value.IsDirectory != rhs.Value.IsDirectory) {
					if (lhs.Value.IsDirectory) {
						return -1;
					}
					return 1;
				}
				return string.Compare(lhs.Value.Filename, rhs.Value.Filename);
			}
		}

		private class FileListBuilder {
			private List<string> _fileNames = new List<string>();

			public string[] FileNames => _fileNames.ToArray();

			public void WalkHandler(TreeNode<F_FIND> node) {
				if (!node.Value.IsDirectory) {
					TreeNode<F_FIND> parent = node.Parent;
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Length = 0;
					while (parent != null) {
						stringBuilder.Insert(0, parent.Value.Filename + "\\");
						parent = parent.Parent;
					}
					stringBuilder.Append(node.Value.Filename);
					stringBuilder.Append(", " + node.Value.FileSize + " bytes");
					stringBuilder.Append(", " + node.Value.CreationDateTime.ToString());
					stringBuilder.Append(", ver. " + node.Value.Security.FileVersion);
					_fileNames.Add(stringBuilder.ToString());
				}
			}
		}

		private struct FileAddressLength {
			public uint _address;

			public uint _length;

			public FileAddressLength(uint address, uint length) {
				_address = address;
				_length = length;
			}
		}

		public delegate void DeviceSlotResetProgress();

		public delegate void DeviceSlotProgress(string task, string status, int percentageComplete);

		public delegate void DeviceSlotOverallProgress(string task, int percentageComplete);

		private enum VersionIndex {
			MSP430,
			FPGA,
			Firmware
		}

		public delegate bool DownloadDelegate(string RelativePathAndName, ref MemoryStream ms, bool showProgress);

		public enum OEMType {
			OEMType_None,
			OEMType_Test,
			OEMType_GM,
			OEMType_Ford,
			OEMType_Dodge,
			OEMType_Nissan
		}

		public static string[] UnsupportedSkus = new string[4]
		{
			"87000",
			"87100",
			"86000",
			"86100"
		};

		private static Proteus proteus;

		public USBCommunicator _comm = null;

		private DeviceConnector _deviceConnector = null;

		private bool _reprogrammedMSP430 = false;

		private GeneralEvent.GeneralEventHandler _generalEventDelegate = null;

		public string _serialNumber = "";

		private string _description = "";

		private int _showGeneralEventType = 1;

		private int _progressOffset = 0;

		private bool _encrypt = false;

		private float _progressRatio = 1f;

		private uint _readPacketTotal = 0u;

		private uint _writePacketTotal = 0u;

		private Dictionary<string, FileAddressLength> _fileList = null;

		private uint _nextFileAddress = 1u;

		private OEMType _oemType = OEMType.OEMType_None;

		private MemoryStream _deviceFilesXML;

		public DeviceSlotResetProgress _deviceSlotResetProgress = null;

		public DeviceSlotProgress _deviceSlotProgress = null;

		public DeviceSlotOverallProgress _deviceSlotOverallProgress = null;

		public DownloadDelegate _downloadDelegate = null;

		private string _evoModel = "UNIDENTIFIED";

		private uint _productSerialNumber = 0u;

		public string _FPGAVersionString = "";

		private string _FirmwareVersionString = "";

		private string _MSP430VersionString = "";

		private string _CalibrationVersionString = "";

		private bool _appAlreadyLoaded = false;

		private bool _hasHotLevels = false;

		private byte[] _settings = null;

		public override bool HasUnlockableUpgrades => _hasHotLevels;

		public override string FirmwareIDTargetPath {
			get {
				if (_deviceConnector != null) {
					return new EdgeDeviceLibrary.FusionService.FusionService().CreateCustomFirmwareZip(_deviceConnector.Email, _deviceConnector.Password, GetSerialNumber().ToString(), _deviceConnector.FirmwareIDTarget + ".bin", _MSP430VersionString, _FPGAVersionString, _FirmwareVersionString);
				}
				return "none";
			}
		}

		public override string CalibrationIDTargetPath {
			get {
				if (_deviceConnector != null) {
					return GetCalibrationIDTargetPath(_deviceConnector.CalibrationIDTarget, _deviceConnector.Email, _deviceConnector.Password, GetSerialNumber().ToString());
				}
				return "none";
			}
		}

		public override bool IsResetNecessary => _reprogrammedMSP430;

		public override bool IsUnplugResetNecessary {
			get {
				Cmd_GetRecoveryState cmd_GetRecoveryState = new Cmd_GetRecoveryState(encrypted: false);
				try {
					int num = _comm.IssueCommand(_serialNumber, cmd_GetRecoveryState);
					if (cmd_GetRecoveryState.ErrorCode == 0 && cmd_GetRecoveryState.RequestPowerCycle) {
						return true;
					}
				} catch {
				}
				return false;
			}
		}

		public string PlatformFileExtension {
			get {
				PlatformTypes platformType = _PlatformType;
				if ((uint)(platformType - 4) <= 1u) {
					return "__cyclone4";
				}
				return string.Empty;
			}
		}

		public static Proteus Instance() {
			return proteus;
		}
		public static Proteus Instance(USBCommunicator comm, string serialNumber, string description) {
			if (proteus == null) {
				proteus = new Proteus(comm, serialNumber, description, DeviceConnector.Instance());
			}
			return proteus;
		}

		public Proteus(USBCommunicator comm, string serialNumber, string description, DeviceConnector deviceConnector) {
			_comm = comm;
			_serialNumber = serialNumber;
			_description = description;
			_deviceConnector = deviceConnector;
			_comm.OpenUSBDeviceBySerialNumber(serialNumber);
			_generalEventDelegate = OnGeneralEvent;
			GeneralEvent.AddGeneralEventHandler(_generalEventDelegate);
		}

		public override void Cleanup() {
			base.Cleanup();
			if (_comm != null) {
				try {
					_comm.CloseUSBDevice(_serialNumber, 1000.0);
				} catch {
				}
				_comm.Dispose();
				_comm = null;
			}
			if (_generalEventDelegate != null) {
				GeneralEvent.RemoveGeneralEventHandler(_generalEventDelegate);
			}
			_generalEventDelegate = null;
		}

		private void OnGeneralEvent(object sender, GeneralEvent.GeneralEventArgs e) {
			if (_showGeneralEventType == 0) {
				return;
			}
			try {
				if (sender is USBCommunicator) {
					uint num = (uint)e.EventData[0];
					uint num2 = (uint)e.EventData[1];
					if (num2 == 0) {
						num2 = _readPacketTotal;
					} else {
						_writePacketTotal = num2;
					}
					string task = $"Transferred {num} / {num2}";
					int num3 = 0;
					if ((decimal)num2 > 0m) {
						num3 = (int)((decimal)num / (decimal)num2 * 100m);
					}
					if (_showGeneralEventType == 1) {
						ReportProgress(task, "", num3);
					} else if (_showGeneralEventType == 2) {
						ReportOverallProgress(task, num3);
					} else if (_showGeneralEventType == 3) {
						int num4 = (int)((float)num3 * _progressRatio) + _progressOffset;
						num4 = ((num4 > 100) ? 100 : num4);
						ReportProgress("Writing files to flash", num4 + "% Complete", num4);
						ReportOverallProgress(task, num3);
					}
				}
			} catch (Exception) {
			}
		}

		public override uint GetUnlockCode(string codeName) {
			return EncryptDecrypt.GetUnlockCode(GetSerialNumber(), codeName);
		}

		public override bool UnlockFeature(string codeName) {
			bool result = false;
			uint unlockCode = GetUnlockCode(codeName);
			if (unlockCode != 0) {
				MemoryStream memoryStream = new MemoryStream();
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.NewLineChars = "\r\n";
				xmlWriterSettings.Indent = true;
				XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
				xmlWriter.WriteStartDocument();
				xmlWriter.WriteStartElement("root");
				xmlWriter.WriteStartElement("hotlevels");
				xmlWriter.WriteAttributeString("code", unlockCode.ToString());
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndDocument();
				xmlWriter.Flush();
				memoryStream.Seek(0L, SeekOrigin.Begin);
				ChangeMakeDir("\\UIResources\\", "");
				result = WriteFat(memoryStream, "UnlockCodes.xml", fileEncrypted: false);
			}
			return result;
		}

		public void ProcessUploadFolder(ref string files, string currentDir) {
			Cmd_MA_Dir cmd_MA_Dir = new Cmd_MA_Dir(encrypted: false, "*.*");
			Cmd_MA_MkChRmDir cmd_MA_MkChRmDir = null;
			try {
				int num = _comm.IssueCommand(_serialNumber, cmd_MA_Dir);
				if (cmd_MA_Dir == null) {
					throw new Exception("Cannot read dir in ProcessUploadFolder");
				}
				if (cmd_MA_Dir.ErrorCode != 0) {
					throw new Exception("Error reading dir in ProcessUploadFolder: " + cmd_MA_Dir.ErrorCode + " - " + cmd_MA_Dir.ExtendedErrorCode);
				}
				F_FIND[] entries = cmd_MA_Dir.Entries;
				foreach (F_FIND f_FIND in entries) {
					if (f_FIND.Filename.Length <= 0 || f_FIND.Filename[0] == '.' || !(Path.GetExtension(f_FIND.Filename).ToLower() != ".bmp")) {
						continue;
					}
					if (f_FIND.IsDirectory) {
						cmd_MA_MkChRmDir = DirActionOnDevice(DirActions.ChDir, encrypted: false, f_FIND.Filename);
						if (cmd_MA_MkChRmDir == null) {
							throw new Exception("Cannot change dir in ProcessUploadFolder to " + f_FIND.Filename);
						}
						if (cmd_MA_MkChRmDir.ErrorCode != 0) {
							throw new Exception("Error changing dir in ProcessUploadFolder: " + cmd_MA_MkChRmDir.ErrorCode + " - " + cmd_MA_MkChRmDir.ExtendedErrorCode);
						}
						ProcessUploadFolder(ref files, F_FIND.GetFixedFilePath(currentDir, f_FIND.Filename));
						cmd_MA_MkChRmDir = DirActionOnDevice(DirActions.ChDir, encrypted: false, "..");
						if (cmd_MA_MkChRmDir == null) {
							throw new Exception("Cannot change dir in ProcessUploadFolder to ..");
						}
						if (cmd_MA_MkChRmDir.ErrorCode != 0) {
							throw new Exception("Error changing dir to .. in ProcessUploadFolder: " + cmd_MA_MkChRmDir.ErrorCode + " - " + cmd_MA_MkChRmDir.ExtendedErrorCode);
						}
					} else {
						if (files.Length > 0) {
							files += "|";
						}
						files += currentDir;
						if (currentDir != "\\") {
							files += "\\";
						}
						files += f_FIND.Filename;
					}
				}
			} catch (Exception) {
			}
		}

		public override bool NeedsUserInputVIN(out VINFormat vinFormat) {
			XmlDocument xmlDocument = null;
			string currentDir = GetCurrentDir();
			vinFormat = null;
			MemoryStream ms = new MemoryStream();
			ChangeMakeDir("\\UIResources\\", "");
			try {
				if (!ReadFat("GeneralSettings.xml", bSkipError: false, ShowsProgressBar: false, ref ms)) {
					return false;
				}
				ms.Seek(0L, SeekOrigin.Begin);
				xmlDocument = new XmlDocument();
				XmlReader reader = new XmlTextReader(ms);
				xmlDocument.Load(reader);
			} finally {
				ms.Dispose();
			}
			XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("vinrequest");
			if (xmlNode == null) {
				ChangeDirectory(currentDir);
				return false;
			}
			XmlAttribute xmlAttribute = xmlNode.Attributes["value"];
			bool result = false;
			if (xmlAttribute == null || !bool.TryParse(xmlAttribute.Value, out result) || !result) {
				ChangeDirectory(currentDir);
				return false;
			}
			ms = new MemoryStream();
			try {
				ChangeMakeDir(GetStockFolderName(), "");
				if (ReadFat("VIN.xml", bSkipError: false, ShowsProgressBar: false, ref ms)) {
					XmlDocument xmlDocument2 = new XmlDocument();
					ms.Seek(0L, SeekOrigin.Begin);
					using (XmlReader reader2 = new XmlTextReader(ms)) {
						xmlDocument2.Load(reader2);
					}
					vinFormat = new VINFormat(xmlDocument2);
				}
			} finally {
				ms.Dispose();
			}
			ChangeDirectory(currentDir);
			return result;
		}

		public override void SaveUserVIN(string vin) {
			XmlDocument xmlDocument = null;
			MemoryStream ms = new MemoryStream();
			ChangeMakeDir("\\UIResources\\", "");
			try {
				if (!ReadFat("GeneralSettings.xml", bSkipError: false, ShowsProgressBar: false, ref ms)) {
					return;
				}
				ms.Seek(0L, SeekOrigin.Begin);
				xmlDocument = new XmlDocument();
				XmlReader reader = new XmlTextReader(ms);
				xmlDocument.Load(reader);
			} finally {
				ms.Dispose();
			}
			XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("fusionvin");
			XmlAttribute xmlAttribute = null;
			if (xmlNode == null) {
				xmlNode = xmlDocument.CreateElement("fusionvin");
				xmlDocument.DocumentElement.AppendChild(xmlNode);
				xmlAttribute = xmlDocument.CreateAttribute("value");
				xmlNode.Attributes.Append(xmlAttribute);
			} else {
				xmlAttribute = xmlNode.Attributes["value"];
				if (xmlAttribute == null) {
					xmlAttribute = xmlDocument.CreateAttribute("value");
					xmlNode.Attributes.Append(xmlAttribute);
				}
			}
			if (string.IsNullOrEmpty(vin)) {
				xmlAttribute.Value = "none";
			} else {
				xmlAttribute.Value = vin;
			}
			ms = new MemoryStream();
			using (XmlWriter w = new XmlTextWriter(ms, null)) {
				xmlDocument.Save(w);
			}
			byte[] baFileToWrite = ms.ToArray();
			WriteFat(baFileToWrite, "GeneralSettings.xml");
			ms.Dispose();
		}

		public override string[] FATGetFileList() {
			List<TreeNode<F_FIND>> list = new List<TreeNode<F_FIND>>();
			List<string> list2 = new List<string>();
			DirActionOnDevice(DirActions.ChDir, encrypted: false, "\\");
			Cmd_MA_Dir cmd_MA_Dir = LoadDir(encrypted: false, "*.*");
			if (cmd_MA_Dir == null) {
				return list2.ToArray();
			}
			F_FIND[] entries = cmd_MA_Dir.Entries;
			foreach (F_FIND f_FIND in entries) {
				TreeNode<F_FIND> treeNode = null;
				if (f_FIND.IsFile) {
					treeNode = new TreeNode<F_FIND>(f_FIND);
					list.Add(treeNode);
				} else if (f_FIND.IsRealDirectory) {
					treeNode = new TreeNode<F_FIND>(f_FIND);
					list.Add(treeNode);
					RecurseRetrieveFileEntries(treeNode);
				}
			}
			if (list.Count > 0) {
				list.Sort(FileTreeComparer.Instance);
			}
			FileListBuilder fileListBuilder = new FileListBuilder();
			TreeNode<F_FIND>.TraverseTree(list, TreeNode<F_FIND>.TraverseType.PostOrder, (TreeNode<F_FIND>.TraverseHandler)fileListBuilder.WalkHandler);
			return fileListBuilder.FileNames;
		}

		public void RecurseRetrieveFileEntries(TreeNode<F_FIND> parentNode) {
			if (DirActionOnDevice(DirActions.ChDir, encrypted: false, parentNode.Value.Filename).ErrorCode == 0) {
				Cmd_MA_Dir cmd_MA_Dir = LoadDir(encrypted: false, "*.*");
				if (cmd_MA_Dir == null) {
					return;
				}
				F_FIND[] entries = cmd_MA_Dir.Entries;
				foreach (F_FIND f_FIND in entries) {
					TreeNode<F_FIND> treeNode = null;
					if (f_FIND.IsFile) {
						treeNode = new TreeNode<F_FIND>(f_FIND);
						parentNode.AddChild(treeNode);
					} else if (f_FIND.IsRealDirectory) {
						treeNode = new TreeNode<F_FIND>(f_FIND);
						parentNode.AddChild(treeNode);
					}
				}
				DirActionOnDevice(DirActions.ChDir, encrypted: false, "..");
			}
			parentNode.SortChildren(FileTreeComparer.Instance);
		}

		public override void UploadFiles(string files, UploadListType listType) {
			try {
				MemoryStream ms = new MemoryStream();
				ZipOutputStream zipOutputStream = new ZipOutputStream(ms);
				string currentDir = "";
				currentDir = ChangeMakeDir("\\", currentDir);
				string text = "CriticalFileUpload.zip";
				if (listType == UploadListType.SpecificFiles) {
					text = "SpecificFileUpload.zip";
				}
				if (files == null || files == string.Empty) {
					text = "AllFileUpload.zip";
					ProcessUploadFolder(ref files, currentDir);
				}
				if (files != null && files != string.Empty) {
					char[] separator = new char[1]
					{
						'|'
					};
					string[] array = files.Split(separator);
					int num = array.Length;
					int num2 = 0;
					string[] array2 = array;
					foreach (string text2 in array2) {
						currentDir = ChangeMakeDir(text2, currentDir);
						int percentageComplete = 0;
						if (num > 0) {
							percentageComplete = (int)((float)num2 / (float)num * 100f);
						}
						ReportOverallProgress("Overall Files Transfer..." + percentageComplete + "%", percentageComplete);
						Application.DoEvents();
						MemoryStream ms2 = new MemoryStream();
						try {
							if (ReadFat(Path.GetFileName(text2), bSkipError: false, ShowsProgressBar: true, ref ms2)) {
								ms2.Seek(0L, SeekOrigin.Begin);
								byte[] array3 = new byte[ms2.Length];
								ms2.Read(array3, 0, (int)ms2.Length);
								ms2.Close();
								ZipEntry entry = new ZipEntry(ZipEntry.CleanName(text2));
								zipOutputStream.PutNextEntry(entry);
								zipOutputStream.Write(array3, 0, array3.Length);
							}
						} catch {
						} finally {
							ms2?.Dispose();
						}
						num2++;
					}
				}
				if (_deviceConnector != null) {
					string RemoteRelativePathAndName = "_uploads\\devices\\" + _deviceConnector.communicator.GetDevicesTableID() + "\\" + text;
					zipOutputStream.Finish();
					_deviceConnector.UploadStockFile(ref ms, ref RemoteRelativePathAndName);
					zipOutputStream.Close();
				}
			} catch (Exception ex) {
				throw ex;
			}
		}

		public static string AppendDateTimeFileName(string filename) {
			string extension = Path.GetExtension(filename);
			filename = Path.GetFileNameWithoutExtension(filename);
			filename = filename + "_" + DateTime.Now.ToString("o");
			filename = filename.Substring(0, filename.LastIndexOf('.')) + extension;
			filename = filename.Replace(':', '-');
			return filename;
		}

		public Cmd_MA_Dir LoadDir(bool encrypted, string filename) {
			Cmd_MA_Dir cmd_MA_Dir = null;
			try {
				cmd_MA_Dir = new Cmd_MA_Dir(encrypted, filename);
				int num = _comm.IssueCommand(_serialNumber, cmd_MA_Dir);
			} catch (ProteusCmdException) {
				return null;
			} catch (Exception) {
				return null;
			}
			return cmd_MA_Dir;
		}

		public Cmd_MA_MkChRmDir DirActionOnDevice(DirActions dirAction, bool encrypted, string dirName) {
			Cmd_MA_MkChRmDir cmd_MA_MkChRmDir = null;
			try {
				cmd_MA_MkChRmDir = new Cmd_MA_MkChRmDir(dirAction, encrypted, dirName);
				DateTime now = DateTime.Now;
				int num = _comm.IssueCommand(_serialNumber, cmd_MA_MkChRmDir);
				DateTime now2 = DateTime.Now;
				TimeSpan timeSpan = now2 - now;
			} catch (ProteusCmdException ex) {
				Trace.WriteLine(">> Exception: " + ex.Message);
				throw new Exception(ex.Message);
			} catch (Exception ex2) {
				Trace.WriteLine(">> Exception: " + ex2.Message);
				throw ex2;
			}
			return cmd_MA_MkChRmDir;
		}

		public Cmd_MA_ReadFile ReadFileFromDevice(bool encrypted, string filename) {
			Cmd_MA_GetFileInfo cmd_MA_GetFileInfo = new Cmd_MA_GetFileInfo(encrypted: false, filename);
			_comm.IssueCommand(_serialNumber, cmd_MA_GetFileInfo);
			Cmd_MA_ReadFile cmd_MA_ReadFile = null;
			if (cmd_MA_GetFileInfo != null && cmd_MA_GetFileInfo.FileSize != 0) {
				try {
					cmd_MA_ReadFile = new Cmd_MA_ReadFile(encrypted, filename);
					_readPacketTotal = 0u;
					if (cmd_MA_ReadFile.PacketDataLength > 0) {
						_readPacketTotal = cmd_MA_GetFileInfo.FileSize / cmd_MA_ReadFile.PacketDataLength;
					}
					int num = _comm.IssueCommand(_serialNumber, cmd_MA_ReadFile);
					string task = $"Transferred {_readPacketTotal} / {_readPacketTotal}";
					ReportProgress(task, "", 100);
				} catch (ProteusCmdException ex) {
					throw new Exception(ex.Message);
				} catch (Exception ex2) {
					throw ex2;
				}
			}
			return cmd_MA_ReadFile;
		}

		private Cmd_MA_WriteFile WriteFileToDevice(bool encrypted, MemoryStream ms, string filename, out bool success) {
			success = false;
			byte[] array = null;
			uint fileSize = 0u;
			if (ms != null && ms.Length > 0) {
				array = new byte[(int)ms.Length];
				ms.Read(array, 0, (int)ms.Length);
				fileSize = (uint)array.Length;
			}
			Cmd_MA_WriteFile cmd_MA_WriteFile = null;
			try {
				cmd_MA_WriteFile = new Cmd_MA_WriteFile(encrypted, filename, fileSize, array);
				int num = _comm.IssueCommand(_serialNumber, cmd_MA_WriteFile, 3000.0, 5000.0);
				if (_showGeneralEventType == 1) {
					string task = $"Transferred {_writePacketTotal} / {_writePacketTotal}";
					ReportProgress(task, "", 100);
				} else if (_showGeneralEventType == 2) {
					string task2 = $"Transferred {_writePacketTotal} / {_writePacketTotal}";
					ReportOverallProgress(task2, 100);
				}
				success = true;
			} catch (ProteusCmdException ex) {
				Trace.WriteLine(" >> Failed to write " + filename + "; " + ex.Message);
				success = false;
			} catch (Exception ex2) {
				Trace.WriteLine(" >> Failed to write " + filename + "; " + ex2.Message);
				success = false;
			}
			return cmd_MA_WriteFile;
		}

		public bool ChangeToActiveStockFolder() {
			string stockFolderName = GetStockFolderName();
			if (stockFolderName.Length > 0) {
				return ChangeDirectory("\\" + stockFolderName);
			}
			return false;
		}

		public override string GetStockFolderName() {
			string result = "";
			switch (GetOemType()) {
				case OEMType.OEMType_None:
					result = GetPartNumber(cachedOk: false);
					break;
				case OEMType.OEMType_Test:
					result = "test";
					break;
				case OEMType.OEMType_GM:
					result = "gm";
					break;
				case OEMType.OEMType_Ford:
					result = "ford";
					break;
				case OEMType.OEMType_Dodge:
					result = "dodge";
					break;
				case OEMType.OEMType_Nissan:
					result = "nissan";
					break;
			}
			return result;
		}

		private OEMType GetOemType() {
			if (_oemType == OEMType.OEMType_None) {
				string currentDir = GetCurrentDir();
				byte[] array = ReadSettings();
				if (array.Length >= 8) {
					_oemType = (OEMType)array[8];
				}
				ChangeDirectory(currentDir);
			}
			return _oemType;
		}

		public override uint FATGetFileAddressOrLength(string filenameToRead, bool IsAddress) {
			uint result = 0u;
			if (string.Compare(filenameToRead, "cs_info.bin", ignoreCase: true) == 0 || string.Compare(filenameToRead, "vid.bin", ignoreCase: true) == 0) {
				byte[] array = ReadSettings();
				if (array.Length >= 8) {
					string dir = "";
					_oemType = (OEMType)array[8];
					switch (_oemType) {
						case OEMType.OEMType_None:
							dir = "\\";
							break;
						case OEMType.OEMType_Test:
							dir = "\\Test\\";
							break;
						case OEMType.OEMType_GM:
							dir = "\\GM\\";
							break;
						case OEMType.OEMType_Ford:
							dir = "\\Ford\\";
							break;
						case OEMType.OEMType_Dodge:
							dir = "\\Dodge\\";
							break;
						case OEMType.OEMType_Nissan:
							dir = "\\Nissan\\";
							break;
					}
					ChangeMakeDir(dir, "");
				}
			}
			Cmd_MA_Dir cmd_MA_Dir = LoadDir(encrypted: false, filenameToRead);
			if (cmd_MA_Dir != null && cmd_MA_Dir.Entries.Length == 1) {
				if (_fileList == null) {
					_fileList = new Dictionary<string, FileAddressLength>();
				}
				FileAddressLength value;
				if (_fileList.ContainsKey(filenameToRead)) {
					value = _fileList[filenameToRead];
				} else {
					value = new FileAddressLength(_nextFileAddress++, 0u);
					_fileList[filenameToRead] = value;
				}
				value._length = cmd_MA_Dir.Entries[0].FileSize;
				result = (IsAddress ? value._address : value._length);
			}
			return result;
		}

		public override bool ReadFat(string filenameToRead, bool bSkipError, bool ShowsProgressBar, ref MemoryStream ms) {
			bool result = false;
			string currentDir = GetCurrentDir();
			if (currentDir == "\\" && (string.Compare(filenameToRead, "cs_info.bin", ignoreCase: true) == 0 || string.Compare(filenameToRead, "vid.bin", ignoreCase: true) == 0) && GetOemType() != 0) {
				ChangeDirectory("\\" + GetStockFolderName());
			}
			Cmd_MA_ReadFile cmd_MA_ReadFile = ReadFileFromDevice(_encrypt, filenameToRead);
			if (cmd_MA_ReadFile != null && cmd_MA_ReadFile.ErrorCode == 0) {
				ms.Write(cmd_MA_ReadFile.FileData, 0, cmd_MA_ReadFile.FileData.Length);
				ms.Seek(0L, SeekOrigin.Begin);
				result = true;
			}
			if (currentDir != GetCurrentDir()) {
				ChangeDirectory(currentDir);
			}
			return result;
		}

		public override void RebootDevice(bool IsHardReset) {
		}

		public override void RebootDevice(bool IsHardReset, bool doConnectEvent) {
		}

		public override bool WriteFat(byte[] baFileToWrite, string DeviceFileName, bool fileEncrypted) {
			MemoryStream memoryStream = null;
			if (baFileToWrite != null) {
				memoryStream = new MemoryStream();
				memoryStream.Write(baFileToWrite, 0, baFileToWrite.Length);
			}
			return WriteFat(memoryStream, DeviceFileName, fileEncrypted);
		}

		public bool WriteFat(MemoryStream ms, string DeviceFileName, bool fileEncrypted) {
			bool success = false;
			string fileName = Path.GetFileName(DeviceFileName);
			ms?.Seek(0L, SeekOrigin.Begin);
			Cmd_MA_WriteFile cmd_MA_WriteFile = WriteFileToDevice(fileEncrypted, ms, fileName, out success);
			return success;
		}

		protected override void GetBootLoaderStageStrings(string fullString, ref string stage1, ref string stage2) {
			base.GetBootLoaderStageStrings(fullString, ref stage1, ref stage2);
		}

		public override double GetBootLoaderVersionDouble(string bootloaderVer) {
			return base.GetBootLoaderVersionDouble(bootloaderVer);
		}

		protected override byte[] ReadByteArrayFromPort(int nByteCount, int msecTimeout, bool VerifyChecksum) {
			return base.ReadByteArrayFromPort(nByteCount, msecTimeout, VerifyChecksum);
		}

		internal override byte[] ReadFlash(MemoryLocation location, uint InAddr, ushort InSize) {
			return base.ReadFlash(location, InAddr, InSize);
		}

		protected override bool WriteFlash(byte SecID, uint Address, byte[] dataToWrite) {
			return base.WriteFlash(SecID, Address, dataToWrite);
		}

		protected override bool WriteFlash(byte SecID, uint Address, byte[] dataToWrite, bool normalWrite) {
			return base.WriteFlash(SecID, Address, dataToWrite, normalWrite);
		}

		public override void UpdateStage2(string webStage2) {
			base.UpdateStage2(webStage2);
		}

		protected override bool VerifyStage2IsGood(string bootLoaderVersion) {
			return base.VerifyStage2IsGood(bootLoaderVersion);
		}

		private byte[] ReadZipEntry(ZipFile file, ZipEntry entry) {
			Stream inputStream = file.GetInputStream(entry);
			byte[] array = new byte[entry.Size];
			int num = 0;
			int num2 = inputStream.Read(array, 0, array.Length);
			while (num + num2 < array.Length) {
				num += num2;
				num2 = inputStream.Read(array, num, array.Length - num);
			}
			return array;
		}

		public bool ChangeDirectory(string targetDirectory) {
			Cmd_MA_MkChRmDir cmd_MA_MkChRmDir = DirActionOnDevice(DirActions.ChDir, encrypted: false, targetDirectory);
			if (cmd_MA_MkChRmDir == null) {
				return false;
			}
			if (cmd_MA_MkChRmDir.ErrorCode != 0) {
				return false;
			}
			return true;
		}

		public string ChangeMakeDir(string dir, string currentDir) {
			string text = currentDir;
			Cmd_MA_MkChRmDir cmd_MA_MkChRmDir = null;
			if (Path.GetDirectoryName(dir) != currentDir) {
				text = Path.GetDirectoryName(dir);
				if (text == null) {
					text = "";
				}
				if (text.Length == 0 || text[0] != '\\') {
					text = "\\" + text;
				}
				cmd_MA_MkChRmDir = DirActionOnDevice(DirActions.ChDir, encrypted: false, text);
				if (cmd_MA_MkChRmDir == null) {
					throw new Exception("Can't change device current directory to " + text);
				}
				if (cmd_MA_MkChRmDir.ErrorCode != 0 && (cmd_MA_MkChRmDir.ExtendedErrorCode == 5 || cmd_MA_MkChRmDir.ExtendedErrorCode == 3)) {
					cmd_MA_MkChRmDir = DirActionOnDevice(DirActions.MkDir, encrypted: false, text);
					if (cmd_MA_MkChRmDir == null) {
						throw new Exception("Can't make new directory " + text);
					}
					if (cmd_MA_MkChRmDir.ErrorCode == 0) {
						cmd_MA_MkChRmDir = DirActionOnDevice(DirActions.ChDir, encrypted: false, text);
					}
				}
				if (cmd_MA_MkChRmDir == null) {
					throw new Exception("Can't change device current directory to " + text);
				}
				if (cmd_MA_MkChRmDir.ErrorCode != 0) {
					throw new Exception("Change / Make Dir failed: " + cmd_MA_MkChRmDir.ErrorCode + " - " + cmd_MA_MkChRmDir.ExtendedErrorCode);
				}
			}
			return text;
		}

		public bool DeleteFile(string name) {
			string fileName = Path.GetFileName(name);
			fileName = fileName.Replace("DELETED_", "");
			Cmd_MA_DeleteFile cmd_MA_DeleteFile = new Cmd_MA_DeleteFile(encrypted: false, fileName);
			int num = _comm.IssueCommand(_serialNumber, cmd_MA_DeleteFile);
			return cmd_MA_DeleteFile.ErrorCode == 0;
		}

		private bool CmdEnforceDecompressedFileRule(string name) {
			bool flag = false;
			string fileName = Path.GetFileName(name);
			Cmd_MA_EnforceDecompressedFileRule cmd_MA_EnforceDecompressedFileRule = new Cmd_MA_EnforceDecompressedFileRule(encrypted: false, fileName);
			int num = _comm.IssueCommand(_serialNumber, cmd_MA_EnforceDecompressedFileRule);
			return cmd_MA_EnforceDecompressedFileRule.ErrorCode == 0;
		}

		private void WriteXmlDir(XmlWriter writer, string currentDir) {
			Cmd_MA_Dir cmd_MA_Dir = new Cmd_MA_Dir(encrypted: false, "*.*");
			Cmd_MA_MkChRmDir cmd_MA_MkChRmDir = null;
			try {
				int num = _comm.IssueCommand(_serialNumber, cmd_MA_Dir);
				if (cmd_MA_Dir == null) {
					throw new Exception("Cannot read dir in WriteXmlDir");
				}
				if (cmd_MA_Dir.ErrorCode != 0) {
					throw new Exception("Error reading dir in WriteXmlDir: " + cmd_MA_Dir.ErrorCode + " - " + cmd_MA_Dir.ExtendedErrorCode);
				}
				F_FIND[] entries = cmd_MA_Dir.Entries;
				foreach (F_FIND f_FIND in entries) {
					if (f_FIND.Filename.Length <= 0 || f_FIND.Filename[0] == '.') {
						continue;
					}
					if (f_FIND.Filename.Contains("~~") || f_FIND.Filename.ToLower().Contains("fs_image.7z")) {
						Cmd_MA_DeleteFile cmd = new Cmd_MA_DeleteFile(encrypted: false, f_FIND.Filename);
						_comm.IssueCommand(_serialNumber, cmd);
					} else if (f_FIND.IsDirectory) {
						cmd_MA_MkChRmDir = DirActionOnDevice(DirActions.ChDir, encrypted: false, f_FIND.Filename);
						if (cmd_MA_MkChRmDir == null) {
							throw new Exception("Cannot change dir in WriteXmlDir to " + f_FIND.Filename);
						}
						if (cmd_MA_MkChRmDir.ErrorCode != 0) {
							throw new Exception("Error changing dir in WriteXmlDir: " + cmd_MA_MkChRmDir.ErrorCode + " - " + cmd_MA_MkChRmDir.ExtendedErrorCode);
						}
						WriteXmlDir(writer, F_FIND.GetFixedFilePath(currentDir, f_FIND.Filename));
						cmd_MA_MkChRmDir = DirActionOnDevice(DirActions.ChDir, encrypted: false, "..");
						if (cmd_MA_MkChRmDir == null) {
							throw new Exception("Cannot change dir in WriteXmlDir to ..");
						}
						if (cmd_MA_MkChRmDir.ErrorCode != 0) {
							throw new Exception("Error changing dir to .. in WriteXmlDir: " + cmd_MA_MkChRmDir.ErrorCode + " - " + cmd_MA_MkChRmDir.ExtendedErrorCode);
						}
					} else {
						f_FIND.XmlWrite(writer, currentDir);
					}
				}
			} catch (Exception) {
			}
		}

		public string GetCalibrationIDTargetPath(long calibrationTargetID, string email, string password, string serialnumber) {
			_deviceFilesXML = new MemoryStream();
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.NewLineChars = "\r\n";
			xmlWriterSettings.Indent = true;
			XmlWriter xmlWriter = XmlWriter.Create(_deviceFilesXML, xmlWriterSettings);
			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement("root");
			ChangeMakeDir("\\", "");
			WriteXmlDir(xmlWriter, "\\");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();
			xmlWriter.Flush();
			_deviceFilesXML.Seek(0L, SeekOrigin.Begin);
			byte[] array = new byte[_deviceFilesXML.Length];
			_deviceFilesXML.Read(array, 0, array.Length);
			EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
			fusionService.Timeout = 300000;
			return fusionService.NewCreateCustomCalibraionZip(email, password, serialnumber, calibrationTargetID + ".bin", array);
		}

		public void UpdateFileInfo(XPathNavigator navigator, string fileName) {
			try {
				Cmd_MA_GetFileInfo cmd_MA_GetFileInfo = new Cmd_MA_GetFileInfo(encrypted: false, fileName);
				int num = _comm.IssueCommand(_serialNumber, cmd_MA_GetFileInfo);
				if (cmd_MA_GetFileInfo == null) {
					throw new Exception("Error Updating File Info (Get): " + fileName);
				}
				if (cmd_MA_GetFileInfo.ErrorCode != 0) {
					throw new Exception("Error Updating File Info (Get): " + fileName + " - " + cmd_MA_GetFileInfo.ErrorCode + " - " + cmd_MA_GetFileInfo.ExtendedErrorCode);
				}
				DateTime value = cmd_MA_GetFileInfo.FileDateTime;
				FileInfoSecurity securityRaw = cmd_MA_GetFileInfo.SecurityRaw;
				XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)navigator.Evaluate("/root/fileinfo[@name='" + fileName + "']");
				if (xPathNodeIterator.MoveNext() && xPathNodeIterator.Current.IsNode) {
					value = Convert.ToDateTime(xPathNodeIterator.Current.GetAttribute("creationdatetime", navigator.BaseURI));
					securityRaw.FileVersion = Convert.ToUInt16(xPathNodeIterator.Current.GetAttribute("fileversion", navigator.BaseURI));
				}
				Cmd_MA_SetFileInfo cmd_MA_SetFileInfo = new Cmd_MA_SetFileInfo(encrypted: false, fileName, setDateTime: true, value, setSecurity: true, securityRaw);
				num = _comm.IssueCommand(_serialNumber, cmd_MA_SetFileInfo);
				if (cmd_MA_SetFileInfo == null) {
					throw new Exception("Error Updating File Info (Set): " + fileName);
				}
				if (cmd_MA_SetFileInfo.ErrorCode != 0) {
					throw new Exception("Error Updating File Info (Set): " + fileName + " - " + cmd_MA_SetFileInfo.ErrorCode + " - " + cmd_MA_SetFileInfo.ExtendedErrorCode);
				}
			} catch (Exception) {
			}
		}

		public void UpdateFileInfo(XPathNavigator navigator, string xmlFileName, string deviceFileName) {
			try {
				Cmd_MA_GetFileInfo cmd_MA_GetFileInfo = new Cmd_MA_GetFileInfo(encrypted: false, deviceFileName);
				int num = _comm.IssueCommand(_serialNumber, cmd_MA_GetFileInfo);
				if (cmd_MA_GetFileInfo == null) {
					throw new Exception("Error Retrieving File Info From Device (Get): " + deviceFileName);
				}
				if (cmd_MA_GetFileInfo.ErrorCode != 0) {
					throw new Exception("Error Retrieving File Info From Device (Get): " + deviceFileName + " - " + cmd_MA_GetFileInfo.ErrorCode + " - " + cmd_MA_GetFileInfo.ExtendedErrorCode);
				}
				DateTime value = cmd_MA_GetFileInfo.FileDateTime;
				FileInfoSecurity securityRaw = cmd_MA_GetFileInfo.SecurityRaw;
				XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)navigator.Evaluate("/root/fileinfo[@name='" + xmlFileName + "']");
				if (xPathNodeIterator.MoveNext() && xPathNodeIterator.Current.IsNode) {
					value = Convert.ToDateTime(xPathNodeIterator.Current.GetAttribute("creationdatetime", navigator.BaseURI));
					securityRaw.FileVersion = Convert.ToUInt16(xPathNodeIterator.Current.GetAttribute("fileversion", navigator.BaseURI));
				}
				Cmd_MA_SetFileInfo cmd_MA_SetFileInfo = new Cmd_MA_SetFileInfo(encrypted: false, deviceFileName, setDateTime: true, value, setSecurity: true, securityRaw);
				num = _comm.IssueCommand(_serialNumber, cmd_MA_SetFileInfo);
				if (cmd_MA_SetFileInfo == null) {
					throw new Exception("Error Updating File Info (Set): " + deviceFileName);
				}
				if (cmd_MA_SetFileInfo.ErrorCode != 0) {
					throw new Exception("Error Updating File Info (Set): " + deviceFileName + " - " + cmd_MA_SetFileInfo.ErrorCode + " - " + cmd_MA_SetFileInfo.ExtendedErrorCode);
				}
			} catch (Exception) {
			}
		}

		private bool WaitForDeviceReconnect(int pollDelay, int? maxWait) {
			int num = 0;
			bool flag = _comm.DeviceCount == 0;
			int num2 = 0;
			while (_comm.DeviceCount != 0) {
				Thread.Sleep(200);
				if (++num2 % 5 == 0) {
					Application.DoEvents();
				}
			}
			if (!flag) {
				DeviceConnector.ConnectionArgs connectionArgs = new DeviceConnector.ConnectionArgs();
				connectionArgs.EventType = DeviceConnector.ConnectionArgs.ConnectionType.Disconnected;
				_deviceConnector.SendDeviceDisconnectionEvent(connectionArgs);
			}
			bool flag2 = true;
			bool result = false;
			num2 = 0;
			while (flag2) {
				if (_comm.DeviceCount != 0) {
					if (_comm.Devices.Length > 1) {
						throw new Exception("Only one CS / CTS can connect at a time...");
					}
					_serialNumber = _comm.Devices[0].DeviceSerialNumber;
					_description = _comm.Devices[0].DeviceDescription;
					Thread.Sleep(10000);
					result = true;
					flag2 = false;
				} else {
					if (maxWait.HasValue && num > maxWait) {
						return false;
					}
					Thread.Sleep(pollDelay);
					num += pollDelay;
					if (++num2 % 5 == 0) {
						Application.DoEvents();
					}
				}
			}
			return result;
		}

		private bool ResetDeviceAndReconnect(bool resetForMSP430, int maxWait) {
			bool result = false;
			if (_comm != null) {
				ReportProgress("Resetting Device", "Please wait...", 0);
				Cmd_Reset cmd_Reset = null;
				bool flag = true;
				while (flag) {
					try {
						cmd_Reset = new Cmd_Reset(encrypted: false);
						int num = _comm.IssueCommand(_serialNumber, cmd_Reset);
						if (cmd_Reset == null) {
							throw new Exception("Can't reset device");
						}
						_comm.CloseUSBDevice(_serialNumber, 500.0);
						if (WaitForDeviceReconnect(resetForMSP430 ? 1000 : 200, maxWait)) {
							_comm.OpenUSBDeviceBySerialNumber(_serialNumber);
							result = true;
							break;
						}
						throw new UsbException("Device not reconnected before timeout");
					} catch (ProteusCmdException ex) {
						if (ex.Error == ProteusCmdError.InvalidCommand) {
							StopMiniApp();
							Thread.Sleep(100);
							continue;
						}
						throw ex;
					} catch (Exception ex2) {
						throw ex2;
					}
				}
			}
			return result;
		}

		private bool ResetDeviceAndReconnect(bool resetForMSP430, int maxWait, int progressPercent) {
			bool result = false;
			if (_comm != null) {
				ReportProgress("Resetting Device", "Please wait...", progressPercent);
				Cmd_Reset cmd_Reset = null;
				bool flag = true;
				while (flag) {
					try {
						cmd_Reset = new Cmd_Reset(encrypted: false);
						int num = _comm.IssueCommand(_serialNumber, cmd_Reset);
						if (cmd_Reset == null) {
							throw new Exception("Can't reset device");
						}
						_comm.CloseUSBDevice(_serialNumber, 500.0);
						if (WaitForDeviceReconnect(resetForMSP430 ? 1000 : 200, maxWait)) {
							_comm.OpenUSBDeviceBySerialNumber(_serialNumber);
							result = true;
							break;
						}
						throw new UsbException("Device not reconnected before timeout");
					} catch (ProteusCmdException ex) {
						if (ex.Error == ProteusCmdError.InvalidCommand) {
							StopMiniApp();
							Thread.Sleep(100);
							continue;
						}
						throw ex;
					} catch (Exception ex2) {
						throw ex2;
					}
				}
			}
			return result;
		}

		public void ResetProgressBar() {
			if (_deviceConnector != null) {
				_deviceConnector.ResetProgressBar();
			}
			if (_deviceSlotResetProgress != null) {
				_deviceSlotResetProgress();
			}
		}

		public void ReportProgress(string task, string status, int percentageComplete) {
			if (_deviceConnector != null) {
				_deviceConnector.ReportProgress(task, status, percentageComplete);
			}
			if (_deviceSlotProgress != null) {
				_deviceSlotProgress(task, status, percentageComplete);
			}
		}

		public void ReportOverallProgress(string task, int percentageComplete) {
			if (_deviceConnector != null) {
				_deviceConnector.ReportOverallProgress(task, percentageComplete);
			}
			if (_deviceSlotOverallProgress != null) {
				_deviceSlotOverallProgress(task, percentageComplete);
			}
		}

		public override bool SendEncryptedBinFile(ref MemoryStream ms, uint Address, byte SecID) {
			return SendEncryptedBinFile(_deviceConnector, ref ms, Address, SecID);
		}

		public override bool ResetDevice(int maxWait) {
			ChangeDirectory("\\_Updates\\");
			WriteFat(new byte[1]
			{
				255
			}, "MSP430Reboot.txt", fileEncrypted: false);
			bool result = ResetDeviceAndReconnect(resetForMSP430: true, maxWait);
			ChangeDirectory("\\_Updates\\");
			if (!DeleteFile("MSP430Reboot.txt")) {
				throw new Exception("Unable to delete 'MSP430Reboot.txt' file after a MSP430 update");
			}
			return result;
		}

		public override bool WaitForDeviceReconnect(int? maxWait) {
			bool flag = false;
			try {
				try {
					_comm.CloseUSBDevice(_serialNumber, 500.0);
				} catch {
				}
				flag = WaitForDeviceReconnect(1000, maxWait);
				if (flag) {
					_comm.OpenUSBDeviceBySerialNumber(_serialNumber);
				}
			} catch {
			}
			return flag;
		}

		public void LoadAndRunMiniApp(string miniAppName) {
			LoadMiniApp(_encrypt, "_miniapps\\" + _FPGAVersionString + "\\" + miniAppName, null);
			ExecMiniApp(_encrypt, miniAppName);
		}

		public byte[] ReadCriticalLog() {
			try {
				Cmd_MA_ReadFlash cmd_MA_ReadFlash = new Cmd_MA_ReadFlash(encrypted: false, FlashArea.ErrorLog, 0u, 0u);
				_comm.IssueCommand(_serialNumber, cmd_MA_ReadFlash);
				if (cmd_MA_ReadFlash.ErrorCode == 0) {
					return cmd_MA_ReadFlash.FlashData;
				}
			} catch (Exception ex) {
				Trace.WriteLine(ex.Message);
			}
			return null;
		}

		public bool SendEncryptedBinFile(DeviceConnector dc, ref MemoryStream ms, uint Address, byte SecID) {
			bool result = false;
			ms = PreProcessFileImage(ms);
			ZipFile zipFile = new ZipFile(ms);
			_showGeneralEventType = 2;
			ResetProgressBar();
			ReportProgress("Preparing flash memory", "Please wait...", 0);
			ReportProgress("Writing files to flash", "0% Complete", 0);
			int num = 0;
			try {
				string text = "";
				string text2 = "";
				string[] array = null;
				switch (SecID) {
					case 70:
						text = "MiniApp_FlashAccess.elf.bin" + PlatformFileExtension;
						text2 = "FWFileVerList.xml";
						array = new string[3]
						{
						_MSP430VersionString,
						_FPGAVersionString,
						_FirmwareVersionString
						};
						break;
					case 67:
						text = "MiniApp_FileAccess.elf.bin" + PlatformFileExtension;
						text2 = "CalFileVerList.xml";
						ReportOverallProgress("", 0);
						break;
					default:
						throw new Exception("Invalid binary file SecID");
				}
				LoadMiniApp(encrypted: false, "_miniapps\\" + _FPGAVersionString + "\\" + text, null);
				ExecMiniApp(encrypted: false, text);
				ZipEntry entry = zipFile.GetEntry(text2);
				if (entry != null) {
					Stream inputStream = zipFile.GetInputStream(entry);
					if (inputStream != null) {
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.Load(inputStream);
						XPathNavigator xPathNavigator = xmlDocument.CreateNavigator();
						bool flag = true;
						if (_deviceConnector != null) {
							flag = IsForceUpdateSet();
						}
						string currentDir = "";
						if (SecID == 67) {
							currentDir = ChangeMakeDir("\\", currentDir);
							WriteFat(new byte[1]
							{
								255
							}, "updating.txt", fileEncrypted: false);
						} else {
							currentDir = ChangeMakeDir("\\_Updates\\", currentDir);
						}
						ZipEntry sevenZipImage = GetSevenZipImage(zipFile);
						if (sevenZipImage != null) {
							_showGeneralEventType = 3;
							DeleteFilesToBeUpdated(xPathNavigator);
						}
						_progressOffset = 0;
						foreach (ZipEntry item in zipFile) {
							if (item != null && item.Name != text2 && !item.IsDirectory) {
								if (SecID == 67) {
									currentDir = ChangeMakeDir(item.Name, currentDir);
								}
								string fileName = Path.GetFileName(item.Name);
								if (fileName.Length < 9 || fileName.Substring(0, 8).ToUpper().CompareTo("DELETED_") != 0) {
									byte[] array2 = null;
									if (SecID == 70 || item.Size > 0) {
										array2 = ReadZipEntry(zipFile, item);
										_progressOffset += (int)(array2.Length / ms.Length * 100);
										if (array2 == null) {
											throw new Exception("Can't read zip entry bytes");
										}
									}
									switch (SecID) {
										case 70: {
											XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)xPathNavigator.Evaluate("/root/fileinfo[@name='" + F_FIND.GetFixedFilePath(currentDir, fileName) + "']");
											if (!xPathNodeIterator.MoveNext() || !xPathNodeIterator.Current.IsNode) {
												break;
											}
											string attribute = xPathNodeIterator.Current.GetAttribute("fileversion", xPathNavigator.BaseURI);
											uint num2 = Convert.ToUInt32(xPathNodeIterator.Current.GetAttribute("fileversionindex", xPathNavigator.BaseURI));
											if (num2 > array.Length) {
												throw new Exception("Version Index out of bounds: " + num2 + " - " + fileName);
											}
											if (!(attribute.CompareTo(array[num2]) != 0 || flag)) {
												break;
											}
											uint num3 = Convert.ToUInt32(xPathNodeIterator.Current.GetAttribute("flashindex", xPathNavigator.BaseURI));
											uint num4 = Convert.ToUInt32(xPathNodeIterator.Current.GetAttribute("startaddress", xPathNavigator.BaseURI));
											uint num5 = Convert.ToUInt32(xPathNodeIterator.Current.GetAttribute("length", xPathNavigator.BaseURI));
											bool flag2 = Convert.ToBoolean(xPathNodeIterator.Current.GetAttribute("bulkerase", xPathNavigator.BaseURI));
											bool prependHeader = Convert.ToBoolean(xPathNodeIterator.Current.GetAttribute("appendheader", xPathNavigator.BaseURI));
											bool flag3 = false;
											bool flag4 = false;
											bool flag5 = false;
											switch (num3) {
												case 0u:
													flag3 = true;
													break;
												case 1u:
													flag4 = true;
													break;
												case 2u:
													flag5 = true;
													break;
											}
											if (flag5) {
												WriteFat(array2, item.Name, fileEncrypted: false);
												break;
											}
											int num6 = 0;
											for (num6 = 0; num6 < 5; num6++) {
												if (num2 != 1) {
													byte b = (byte)(num4 >> 16);
													byte b2 = (byte)(num5 >> 16);
													try {
														if (!flag2) {
															ReportOverallProgress("Erasing Flash", 0);
															for (byte b3 = 0; b3 <= b2; b3 = (byte)(b3 + 1)) {
																Cmd_MA_EraseFlash cmd_MA_EraseFlash = new Cmd_MA_EraseFlash(encrypted: false, flag3, flag4, (byte)(b + b3));
																int num7 = _comm.IssueCommand(_serialNumber, cmd_MA_EraseFlash, 3000.0, 5000.0);
																if (cmd_MA_EraseFlash.ErrorCode != 0) {
																	throw new Exception("Can't erase flash: " + cmd_MA_EraseFlash.ErrorCode + " - " + cmd_MA_EraseFlash.ExtendedErrorCode);
																}
																string task = $"Erasing {b3} / {b2}";
																double num8 = (int)b3;
																num8 = ((b2 == 0) ? 0.0 : (num8 / (double)(int)b2));
																num8 *= 100.0;
																ReportOverallProgress(task, (int)num8);
															}
														} else {
															ReportOverallProgress("Erasing Flash " + (num3 + 1), 1);
															Cmd_MA_EraseFlash cmd_MA_EraseFlash = new Cmd_MA_EraseFlash(encrypted: false, flag3, flag4);
															int num9 = _comm.IssueCommand(_serialNumber, cmd_MA_EraseFlash, 2500.0, 55000.0);
															ReportOverallProgress("Erasing Complete", 100);
															if (cmd_MA_EraseFlash.ErrorCode != 0) {
																throw new Exception("Can't erase flash: " + cmd_MA_EraseFlash.ErrorCode + " - " + cmd_MA_EraseFlash.ExtendedErrorCode);
															}
														}
													} catch (Exception ex) {
														if (num6 + 1 >= 5) {
															throw new Exception("Cannot erase flash: " + ex.Message, ex);
														}
														Thread.Sleep(100);
														continue;
													}
												}
												try {
													FlashArea flashAreeToWrite = FlashArea.NotSet;
													if (flag4) {
														flashAreeToWrite = FlashArea.SecondFlash;
													}
													if (flag3) {
														flashAreeToWrite = FlashArea.MainFlash;
													}
													Cmd_MA_WriteFlash cmd_MA_WriteFlash = new Cmd_MA_WriteFlash(encrypted: false, flashAreeToWrite, num4, num5, array2, prependHeader);
													int num10 = _comm.IssueCommand(_serialNumber, cmd_MA_WriteFlash, 3000.0, 5000.0, 60000.0);
													if (cmd_MA_WriteFlash.ErrorCode != 0) {
														throw new Exception("Can't erase flash: " + cmd_MA_WriteFlash.ErrorCode + " - " + cmd_MA_WriteFlash.ExtendedErrorCode);
													}
													if (num2 == 0) {
														_reprogrammedMSP430 = true;
													}
													if (_deviceConnector != null && _deviceConnector.communicator != null) {
														DeviceConnector deviceConnector = _deviceConnector;
														VersionIndex versionIndex = (VersionIndex)num2;
														deviceConnector.Log("Updated " + versionIndex.ToString() + " to version " + attribute);
													}
												} catch (UsbException ex2) {
													Cmd_AbortCommand cmd = new Cmd_AbortCommand(encrypted: false, 50);
													_comm.IssueCommand(_serialNumber, cmd);
													if (num6 + 1 >= 5) {
														throw new Exception("Cannot update device: " + ex2.Message, ex2);
													}
													continue;
												} catch (Exception ex3) {
													if (num6 + 1 >= 5) {
														throw new Exception("Cannot update device: " + ex3.Message, ex3);
													}
													Thread.Sleep(100);
													continue;
												}
												break;
											}
											break;
										}
										case 67:
											if (ReadOnlyFile(item.Name)) {
												continue;
											}
											if (WriteFat(array2, item.Name, _encrypt)) {
												UpdateFileInfo(xPathNavigator, F_FIND.GetFixedFilePath(currentDir, fileName));
												break;
											}
											throw new Exception("Error writing file: " + item.Name);
										default:
											throw new Exception("Invalid SecID");
									}
								}
							}
							int num11 = 0;
							if (zipFile.Size > 0 && sevenZipImage == null) {
								num11 = (int)((float)(++num) / (float)zipFile.Size * 100f);
								ReportProgress("Writing files to flash", num11 + "% Complete", num11);
							}
						}
						_showGeneralEventType = 2;
						if (SecID == 67) {
							if (sevenZipImage != null) {
								bool flag6 = false;
								StopMiniApp();
								flag6 = TryUnpackSevenZip(sevenZipImage, (int)(_progressRatio * 100f));
								int num12 = 0;
								while (!flag6 && num12 < 1) {
									ReportProgress("Updating decomp module", "Updating decompression module.", 0);
									byte[] app = RestoreSevenZippedApp(zipFile.GetInputStream(sevenZipImage));
									InstallAndRebootApp(text, app);
									DisplayDeviceMessage("Updating. Do not disconnect!");
									flag6 = TryUnpackSevenZip(sevenZipImage, (int)(_progressRatio * 100f));
									num12++;
								}
								if (!flag6) {
									throw new Exception("An error occured during decompression");
								}
								LoadMiniApp(encrypted: false, "_miniapps\\" + _FPGAVersionString + "\\" + text, null);
								ExecMiniApp(encrypted: false, text);
								UpdateFileInfo(xPathNavigator);
							}
							currentDir = ChangeMakeDir("\\UIResources\\", "");
							DeleteFile("TestGeneralSettings.xml");
							currentDir = DeleteMarkedFiles(xPathNavigator, currentDir);
							currentDir = ChangeMakeDir("\\", currentDir);
							if (!DeleteFile("updating.txt")) {
								throw new Exception("Unable to delete updating.txt");
							}
						}
						ReportProgress("Writing files to flash", "100% Complete", 100);
						result = true;
					}
				}
			} finally {
				zipFile.Close();
				_showGeneralEventType = 1;
			}
			return result;
		}

		private MemoryStream PreProcessFileImage(MemoryStream ms) {
			MemoryStream memoryStream = new MemoryStream();
			ZipFile zipFile = new ZipFile(ms);
			List<string> fileList = GetFileList(zipFile);
			List<string> cycloneFiles = GetCycloneFiles(fileList);
			if (PlatformFileExtension != string.Empty && cycloneFiles.FindAll((string f) => f.Contains(PlatformFileExtension)).Count <= 0) {
				throw new Exception("The update image does not support the " + PlatformFileExtension.Replace("_", "") + " platform");
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<string> list = ExcludedPlatformSpecificFiles(fileList, dictionary);
			ZipOutputStream zipOutputStream = new ZipOutputStream(memoryStream);
			foreach (ZipEntry item in zipFile) {
				if (list.Contains(item.Name)) {
					continue;
				}
				if (dictionary.ContainsKey(item.Name)) {
					if (fileList.Contains(dictionary[item.Name])) {
						WriteZipEntry(zipOutputStream, zipFile, item, dictionary[item.Name]);
					}
				} else {
					WriteZipEntry(zipOutputStream, zipFile, item, item.Name);
				}
			}
			zipOutputStream.Flush();
			zipOutputStream.Finish();
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return memoryStream;
		}

		private void WriteZipEntry(ZipOutputStream zipOutStream, ZipFile zipFile, ZipEntry zipEntry, string fileName) {
			if (zipEntry != null) {
				byte[] array = null;
				if (zipEntry.Size > 0) {
					Stream inputStream = zipFile.GetInputStream(zipEntry);
					array = new byte[zipEntry.Size];
					inputStream.Read(array, 0, array.Length);
				}
				zipOutStream.PutNextEntry(new ZipEntry(ZipEntry.CleanName(fileName)));
				if (array != null) {
					zipOutStream.Write(array, 0, array.Length);
				}
			}
		}

		private List<string> GetCycloneFiles(List<string> fileList) {
			return fileList.FindAll((string f) => f.Contains("__cyclone"));
		}

		private List<string> ExcludedPlatformSpecificFiles(List<string> fileList, Dictionary<string, string> substituteFiles) {
			List<string> excludedFiles = new List<string>();
			List<string> cycloneFiles = GetCycloneFiles(fileList);
			if (cycloneFiles.Count > 0) {
				Dictionary<string, int> uniqueBaseFileNames = new Dictionary<string, int>();
				cycloneFiles.ForEach(delegate (string f) {
					uniqueBaseFileNames[f.Remove(f.IndexOf("__"))] = 0;
				});
				fileList.ForEach(delegate (string f) {
					foreach (KeyValuePair<string, int> item in uniqueBaseFileNames) {
						if (f.Contains(item.Key)) {
							if (PlatformSupportedFile(f)) {
								if (PlatformFileExtension != string.Empty) {
									substituteFiles.Add(f, f.Replace(PlatformFileExtension, ""));
								}
							} else {
								excludedFiles.Add(f);
							}
						}
					}
				});
			}
			return excludedFiles;
		}

		private bool PlatformSupportedFile(string file) {
			bool flag = false;
			if (PlatformFileExtension == string.Empty) {
				return !file.Contains("__");
			}
			return file.Contains(PlatformFileExtension);
		}

		private List<string> GetFileList(ZipFile zipFile) {
			List<string> list = new List<string>();
			foreach (ZipEntry item in zipFile) {
				list.Add(item.Name);
			}
			return list;
		}

		private bool ReadOnlyFile(string fileName) {
			Cmd_MA_GetFileInfo cmd_MA_GetFileInfo = new Cmd_MA_GetFileInfo(encrypted: false, Path.GetFileName(fileName));
			if (_comm.IssueCommand(_serialNumber, cmd_MA_GetFileInfo) > 0) {
				if (cmd_MA_GetFileInfo.ExtendedErrorCode == 5 && fileName.Contains(".lzma")) {
					return ReadOnlyFile(fileName.Replace(".lzma", ""));
				}
				if (cmd_MA_GetFileInfo.SecurityRaw.Attr_ReadOnly) {
					return true;
				}
			}
			return false;
		}

		private string DeleteMarkedFiles(XPathNavigator fileVerNav, string currentDir) {
			currentDir = ChangeMakeDir("\\", currentDir);
			if (_deviceFilesXML == null) {
				return currentDir;
			}
			_deviceFilesXML.Seek(0L, SeekOrigin.Begin);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(_deviceFilesXML);
			XPathNavigator xPathNavigator = xmlDocument.CreateNavigator();
			ReportProgress("Removing Old Files", "Removing Old Files", 0);
			XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)fileVerNav.Evaluate("/root/fileinfo[contains(@name, 'DELETED_')]");
			while (xPathNodeIterator.MoveNext()) {
				if (!xPathNodeIterator.Current.IsNode) {
					continue;
				}
				string text = xPathNodeIterator.Current.GetAttribute("name", fileVerNav.BaseURI).Replace("DELETED_", "");
				string fileName = Path.GetFileName(text);
				XPathNodeIterator xPathNodeIterator2 = xPathNavigator.Select("/root/fileinfo[contains(@name, '" + text + "')]");
				if (xPathNodeIterator2.Count <= 0) {
					continue;
				}
				xPathNodeIterator2.MoveNext();
				if (xPathNodeIterator2.Current.IsNode) {
					string text2 = xPathNodeIterator2.Current.GetAttribute("name", xPathNavigator.BaseURI);
					if (xPathNodeIterator2.Current.GetAttribute("decompressed", xPathNavigator.BaseURI) == "true") {
						text2 = text2.Replace(".lzma", "");
					}
					if (!(text != text2)) {
						currentDir = ChangeMakeDir(text, currentDir);
						DeleteFile(F_FIND.GetFixedFilePath(currentDir, fileName));
						ReportProgress("Removing Old Files", "Removing Old Files", (int)((float)xPathNodeIterator.CurrentPosition / (float)xPathNodeIterator.Count * 100f));
					}
				}
			}
			return currentDir;
		}

		private void DeleteFilesToBeUpdated(XPathNavigator fileVerNav) {
			DeleteChunkedFiles(fileVerNav);
			EnforceCompressedFileRule(fileVerNav);
		}

		private void EnforceCompressedFileRule(XPathNavigator fileVerNav) {
			string currentDir = string.Empty;
			ReportProgress("Deleting Update Files", "Deleting Update Files.", 0);
			XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)fileVerNav.Evaluate("/root/fileinfo[not(contains(@name, 'DELETED_'))]");
			while (xPathNodeIterator.MoveNext()) {
				if (xPathNodeIterator.Current.IsNode) {
					string attribute = xPathNodeIterator.Current.GetAttribute("name", fileVerNav.BaseURI);
					string fileName = Path.GetFileName(attribute);
					if (!attribute.Contains("CHUNKED_")) {
						currentDir = ChangeMakeDir(attribute, currentDir);
						CmdEnforceDecompressedFileRule(F_FIND.GetFixedFilePath(currentDir, fileName));
						ReportProgress("Delete updating files", "Delete updating files.", (int)((float)xPathNodeIterator.CurrentPosition / (float)xPathNodeIterator.Count * 100f));
					}
				}
			}
		}

		private void DeleteChunkedFiles(XPathNavigator fileVerNav) {
			string currentDir = string.Empty;
			ReportProgress("Deleting Update Files", "Deleting Update Files.", 0);
			XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)fileVerNav.Evaluate("/root/fileinfo[contains(@name, 'CHUNKED_')]");
			while (xPathNodeIterator.MoveNext()) {
				if (xPathNodeIterator.Current.IsNode) {
					string attribute = xPathNodeIterator.Current.GetAttribute("name", fileVerNav.BaseURI);
					attribute = attribute.Remove(attribute.IndexOf("CHUNKED_"), "CHUNKED_".Length);
					string fileName = Path.GetFileName(attribute);
					if (!fileName.Contains("app") && !fileName.Contains("App") && HasUpdatingFile(fileVerNav, fileName)) {
						currentDir = ChangeMakeDir(attribute, currentDir);
						DeleteFile(F_FIND.GetFixedFilePath(currentDir, fileName));
						CmdEnforceDecompressedFileRule(F_FIND.GetFixedFilePath(currentDir, fileName));
						ReportProgress("Delete updating files", "Delete updating files.", (int)((float)xPathNodeIterator.CurrentPosition / (float)xPathNodeIterator.Count * 100f));
					}
				}
			}
		}

		private bool HasUpdatingFile(XPathNavigator fileVerNav, string fileNameOnly) {
			XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)fileVerNav.Evaluate("/root/fileinfo[contains(@name, '" + fileNameOnly + "')]");
			while (xPathNodeIterator.MoveNext()) {
				if (xPathNodeIterator.Current.IsNode) {
					string attribute = xPathNodeIterator.Current.GetAttribute("name", fileVerNav.BaseURI);
					if (!attribute.Contains("CHUNKED_") && !attribute.Contains("DELETED_")) {
						return true;
					}
				}
			}
			return false;
		}

		private void UpdateFileInfo(XPathNavigator fileVerNav) {
			ReportProgress("Updating file info", "Updating file information.", 0);
			XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)fileVerNav.Evaluate("/root/fileinfo[not(contains(@name, 'DELETED_'))]");
			while (xPathNodeIterator.MoveNext()) {
				if (xPathNodeIterator.Current.IsNode) {
					string attribute = xPathNodeIterator.Current.GetAttribute("name", fileVerNav.BaseURI);
					if (attribute.Contains("CHUNKED_")) {
						UpdateFileInfo(fileVerNav, attribute, attribute.Remove(attribute.IndexOf("CHUNKED_"), "CHUNKED_".Length));
					} else {
						UpdateFileInfo(fileVerNav, attribute);
					}
					ReportProgress("Updating file info", "Updating file information.", (int)((float)xPathNodeIterator.CurrentPosition / (float)xPathNodeIterator.Count * 100f));
				}
			}
		}

		private void InstallAndRebootApp(string miniappName, byte[] app) {
			if (app != null) {
				LoadMiniApp(encrypted: false, "_miniapps\\" + _FPGAVersionString + "\\" + miniappName, null);
				ExecMiniApp(encrypted: false, miniappName);
				string targetDirectory = "\\_Updates\\";
				ChangeDirectory(targetDirectory);
				if (!WriteFat(app, "App.bin", fileEncrypted: false)) {
					throw new Exception("Error writing App.bin for Seven Zip update");
				}
			}
			ReportProgress("Rebooting the device", "Rebooting the device.\r\nDo not disconnect!", 0);
			ResetDeviceAndReconnect(resetForMSP430: true, 60000, (int)(_progressRatio * 100f));
		}

		private bool TryUnpackSevenZip(ZipEntry sevenZippedImageEntry, int progressSection1) {
			bool flag = false;
			ReportProgress("Decompressing 7zip", "Decompressing system files.\r\nThis process will take up to 15 minutes.\r\nDo not disconnect the device!", 0);
			Cmd_Unpack7z cmd_Unpack7z = new Cmd_Unpack7z(encrypted: false);
			cmd_Unpack7z.SavePages = true;
			try {
				_comm.IssueCommand(_serialNumber, cmd_Unpack7z, 60000.0, 1000000.0);
				if (!cmd_Unpack7z.RespSuccess) {
					_deviceConnector.Log("Error response to Cmd_Unpack7z command. Command Packet: " + BitConverter.ToString(cmd_Unpack7z.CommandPacket).Replace("-", " ") + "Response Pages: " + PagesToString(cmd_Unpack7z.ResponsePages));
					return false;
				}
				return true;
			} catch (ProteusCmdException ex) {
				if (ex.Error == ProteusCmdError.InvalidCommand) {
					return false;
				}
				if (cmd_Unpack7z != null) {
					_deviceConnector.Log("Exception for Cmd_Unpack7z command. Command Packet: " + BitConverter.ToString(cmd_Unpack7z.CommandPacket).Replace("-", " ") + "Response Pages: " + PagesToString(cmd_Unpack7z.ResponsePages));
				}
				throw ex;
			}
		}

		private string PagesToString(byte[][] pages) {
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (byte[] value in pages) {
				stringBuilder.Append("Page " + num + ":  ");
				stringBuilder.Append(BitConverter.ToString(value).Replace("-", " "));
				num++;
			}
			return stringBuilder.ToString();
		}

		private byte[] RestoreSevenZippedApp(Stream zipStream) {
			byte[] array = null;
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			DirectoryInfo directoryInfo = System.IO.Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PowerteqFusion"));
			if (!directoryInfo.Exists) {
				throw new Exception("Unable to create the PowerteqFusion directory");
			}
			string text = Path.Combine(directoryInfo.FullName, "fs_image.7z");
			string sevenZipExePath = Path.Combine(directoryName, "7za.exe");
			FileStream writeStream = new FileStream(text, FileMode.Create, FileAccess.Write);
			ReadWriteStream(zipStream, writeStream);
			if (!File.Exists(text)) {
				throw new Exception("fs_image was not extracted from zip file");
			}
			using (TempFileCollection tempFileCollection = new TempFileCollection(directoryInfo.FullName, keepFiles: false)) {
				ExtractDeviceImage(text, directoryInfo, sevenZipExePath);
				string[] files = System.IO.Directory.GetFiles(directoryInfo.FullName, "*.*", SearchOption.AllDirectories);
				string[] array2 = files;
				foreach (string fileName in array2) {
					tempFileCollection.AddFile(fileName, keepFile: false);
				}
				if (files.Length == 1 && System.IO.Directory.GetDirectories(directoryInfo.FullName).Length == 0) {
					throw new Exception("fs_image extraction failed");
				}
				int numAppChunks = 0;
				int num = 0;
				string[] files2 = System.IO.Directory.GetFiles(directoryInfo.FullName, "App.bin*", SearchOption.AllDirectories);
				List<FileInfo> list = new List<FileInfo>();
				string[] array3 = files2;
				foreach (string text2 in array3) {
					string text3 = text2.Split(new string[1]
					{
						"~~"
					}, StringSplitOptions.RemoveEmptyEntries)[1];
					if (numAppChunks == 0) {
						numAppChunks = int.Parse(text3.Substring(2));
					} else if (numAppChunks != int.Parse(text3.Substring(2))) {
						_deviceConnector.Log("Parsing App.bin~~ file names failed.   appChunk array:  " + StringArraytoString(files2));
						throw new Exception("The App.bin file name: " + text2 + " is not formed correctly in the image");
					}
					FileInfo fileInfo = new FileInfo(text2);
					num += (int)fileInfo.Length;
					list.Add(fileInfo);
				}
				if (numAppChunks == 0 || numAppChunks > 99) {
					throw new Exception("The number of App.bin chunks is invalid.  Number of chunks: " + numAppChunks);
				}
				array = new byte[num];
				int num2 = 0;
				try {
					int i;
					for (i = 1; i <= numAppChunks; i++) {
						FileInfo fileInfo2 = list.Find((FileInfo e) => e.Name.Contains("App.bin~~" + i.ToString("D2") + numAppChunks.ToString("D2")));
						using (FileStream fileStream = File.OpenRead(fileInfo2.FullName)) {
							fileStream.Read(array, num2, (int)fileInfo2.Length);
						}
						num2 += (int)fileInfo2.Length;
					}
				} catch (Exception ex) {
					_deviceConnector.Log("Restoring App.bin chunks failed.  appChunk array:  " + StringArraytoString(files2));
					array = null;
					throw new Exception("Unable to restore app from chunks. " + ex.Message);
				}
			}
			System.IO.Directory.Delete(directoryInfo.FullName, recursive: true);
			return array;
		}

		private string StringArraytoString(string[] stringArray) {
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in stringArray) {
				stringBuilder.Append(value);
				stringBuilder.Append(", ");
			}
			return stringBuilder.ToString();
		}

		private void ReadWriteStream(Stream inStream, FileStream writeStream) {
			int num = 256;
			byte[] buffer = new byte[num];
			for (int num2 = inStream.Read(buffer, 0, num); num2 > 0; num2 = inStream.Read(buffer, 0, num)) {
				writeStream.Write(buffer, 0, num2);
			}
			inStream.Close();
			writeStream.Close();
		}

		private void ExtractDeviceImage(string fs_imagePath, DirectoryInfo outputDir, string sevenZipExePath) {
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = sevenZipExePath;
			processStartInfo.Arguments = "x \"" + fs_imagePath + "\" -o\"" + outputDir.FullName + "\" -r -aoa";
			processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			Process process = Process.Start(processStartInfo);
			process.WaitForExit();
		}

		public override bool SendEncryptedS19File(ref MemoryStream ms) {
			return false;
		}

		private bool EncryptionCheck() {
			Cmd_EncryptCheck cmd_EncryptCheck = null;
			try {
				cmd_EncryptCheck = new Cmd_EncryptCheck();
				DateTime now = DateTime.Now;
				int num = _comm.IssueCommand(_serialNumber, cmd_EncryptCheck);
				DateTime now2 = DateTime.Now;
				TimeSpan timeSpan = now2 - now;
			} catch (ProteusCmdException ex) {
				throw new Exception(ex.Message);
			} catch (Exception ex2) {
				throw ex2;
			}
			return cmd_EncryptCheck.Encrypt;
		}

		private Cmd_LoadApp LoadMiniApp(bool encrypted, string fileMiniApp, byte[] bytes) {
			byte[] array = null;
			string text = "<Unknown>";
			Cmd_LoadApp cmd_LoadApp = null;
			try {
				if (bytes == null) {
					base.IsFileFormatEncryptedS19 = false;
					MemoryStream ms = new MemoryStream();
					if (_downloadDelegate != null) {
						_downloadDelegate(fileMiniApp, ref ms, showProgress: false);
					} else if (_deviceConnector != null) {
						new DownloadDelegate(_deviceConnector.Download)(fileMiniApp, ref ms, showProgress: false);
					}
					ms.Seek(0L, SeekOrigin.Begin);
					array = new byte[(int)ms.Length];
					ms.Read(array, 0, (int)ms.Length);
					ms.Close();
					ms.Dispose();
				} else {
					array = bytes;
				}
				text = Edge.Common.IO.Directory.GetOnlyFilename(fileMiniApp);
				text = text.Substring(0, text.IndexOf("."));
				Cmd_StopApp cmd = new Cmd_StopApp(encrypted: false);
				int num = _comm.IssueCommand(_serialNumber, cmd);
				cmd_LoadApp = new Cmd_LoadApp(encrypted, text, (uint)array.Length, array);
				DateTime now = DateTime.Now;
				num = _comm.IssueCommand(_serialNumber, cmd_LoadApp);
				DateTime now2 = DateTime.Now;
				TimeSpan timeSpan = now2 - now;
			} catch (ProteusCmdException ex) {
				throw new Exception(ex.Message);
			} catch (Exception ex2) {
				throw ex2;
			}
			return cmd_LoadApp;
		}

		private Cmd_ExecApp ExecMiniApp(bool encrypted, string appName) {
			Cmd_ExecApp cmd_ExecApp = null;
			try {
				cmd_ExecApp = new Cmd_ExecApp(encrypted, appName);
				DateTime now = DateTime.Now;
				int num = _comm.IssueCommand(_serialNumber, cmd_ExecApp);
				DateTime now2 = DateTime.Now;
				TimeSpan timeSpan = now2 - now;
			} catch (ProteusCmdException ex) {
				throw new Exception(ex.Message);
			} catch (Exception ex2) {
				throw ex2;
			}
			return cmd_ExecApp;
		}

		public static ZipEntry GetSevenZipImage(ZipFile zipFile) {
			ZipEntry result = null;
			foreach (ZipEntry item in zipFile) {
				if (!item.IsDirectory && item.Name.Contains("fs_image.7z")) {
					result = item;
					break;
				}
			}
			return result;
		}

		public override void VerifyBootloader() {
			base.VerifyBootloader();
			if (_FPGAVersionString.Length > 0) {
				_encrypt = EncryptionCheck();
				LoadMiniApp(_encrypt, "_miniapps\\" + _FPGAVersionString + "\\MiniApp_FileAccess.elf.bin" + PlatformFileExtension, null);
				ExecMiniApp(_encrypt, "MiniApp_FileAccess.elf.bin");
			}
		}

		public bool FormatDrive() {
			if (_FPGAVersionString.Length > 0) {
				try {
					LoadMiniApp(encrypted: false, "_miniapps\\" + _FPGAVersionString + "\\MiniApp_FileAccess.elf.bin" + PlatformFileExtension, null);
					ExecMiniApp(encrypted: false, "MiniApp_FileAccess.elf.bin");
					Cmd_MA_FormatDrive cmd = new Cmd_MA_FormatDrive(encrypted: false);
					int num = _comm.IssueCommand(_serialNumber, cmd, 10000.0, 10000.0);
					StopMiniApp();
					return true;
				} catch (Exception) {
					return false;
				}
			}
			return false;
		}

		public string GetCustomModelPart(bool cachedOk) {
			int num = nProductIdentifier;
			if(_evoModel.Equals("UNIDENTIFIED") || !cachedOk) {
				try {
					if(_deviceConnector != null) {
						num = new EdgeDeviceLibrary.FusionService.FusionService().GetModelFromUserSerialNum(_deviceConnector.Email, _deviceConnector.Password, GetSerialNumber());
					}
				} catch(WebException) {
					num = 0;
				}
				if(num == 0) {
					return string.Empty;
				}
				string[] array = null;
				try {
					if(_deviceConnector != null) {
						nProductIdentifier = num;
						_evoModel = new EdgeDeviceLibrary.FusionService.FusionService().GetModelString(_deviceConnector.Email, _deviceConnector.Password, num);
						array = new EdgeDeviceLibrary.FusionService.FusionService().GetBootloaderInfo(_deviceConnector.Email, _deviceConnector.Password, nProductIdentifier);
					}
				} catch(SoapException) {
					_evoModel = "UNIDENTIFIED";
					array = null;
				} catch(WebException) {
					_evoModel = "UNIDENTIFIED_NO_WEB";
					array = null;
				}
				if(array != null && array[3] == "Encrypted_S19") {
					base.IsFileFormatEncryptedS19 = true;
				} else {
					base.IsFileFormatEncryptedS19 = false;
				}
			}
			return $"[{num}] {_evoModel}";
		}

		public override string GetPartNumber(bool cachedOk) {
			if (_evoModel.Equals("UNIDENTIFIED") || !cachedOk) {
				ushort num = 0;
				try {
					if (_deviceConnector != null) {
						num = (ushort)new EdgeDeviceLibrary.FusionService.FusionService().GetModelFromUserSerialNum(_deviceConnector.Email, _deviceConnector.Password, GetSerialNumber());
					}
				} catch (WebException) {
					num = 0;
				}
				if (num == 0) {
					return string.Empty;
				}
				string[] array = null;
				try {
					if (_deviceConnector != null) {
						nProductIdentifier = num;
						_evoModel = new EdgeDeviceLibrary.FusionService.FusionService().GetModelString(_deviceConnector.Email, _deviceConnector.Password, num);
						array = new EdgeDeviceLibrary.FusionService.FusionService().GetBootloaderInfo(_deviceConnector.Email, _deviceConnector.Password, nProductIdentifier);
					}
				} catch (SoapException) {
					_evoModel = "UNIDENTIFIED";
					array = null;
				} catch (WebException) {
					_evoModel = "UNIDENTIFIED_NO_WEB";
					array = null;
				}
				if (array != null && array[3] == "Encrypted_S19") {
					base.IsFileFormatEncryptedS19 = true;
				} else {
					base.IsFileFormatEncryptedS19 = false;
				}
			}
			return _evoModel;
		}

		public override uint GetSerialNumber() {
			if (_productSerialNumber == 0 && _deviceConnector != null) {
				for (int i = 0; i < 2; i++) {
					Cmd_GetSerial cmd_GetSerial = new Cmd_GetSerial(encrypted: false);
					try {
						int num = _comm.IssueCommand(_serialNumber, cmd_GetSerial);
						if (cmd_GetSerial.SerialNumber.Length > 0) {
							_productSerialNumber = Convert.ToUInt32(cmd_GetSerial.SerialNumber);
							goto IL_03f9;
						}
					} catch (ProteusCmdException ex) {
						if (ex.Error == ProteusCmdError.InvalidCommand_RecoveryMode) {
							_showGeneralEventType = 1;
							Cmd_GetRecoveryState cmd_GetRecoveryState = new Cmd_GetRecoveryState(encrypted: false);
							try {
								ChangeDirectory("\\_Updates\\");
								WriteFat(File.ReadAllBytes($"{AppDomain.CurrentDomain.BaseDirectory}Files/85300-CS_CycloneIVE/_Updates/App.bin"), "App.bin");
								int num2 = _comm.IssueCommand(_serialNumber, cmd_GetRecoveryState);
								if (cmd_GetRecoveryState.ErrorCode == 0) {
									Cmd_GetFPGAVersion cmd_GetFPGAVersion = new Cmd_GetFPGAVersion(encrypted: false);
									num2 = _comm.IssueCommand(_serialNumber, cmd_GetFPGAVersion);
									if (cmd_GetFPGAVersion.ErrorCode == 0) {
										_PlatformType = cmd_GetFPGAVersion.PlatformType;
										if (cmd_GetRecoveryState.RecoverMSP430App) {
											ResetProgressBar();
											ReportOverallProgress("Recovery Mode - MSP430", 33);
											ReportProgress("", "", 0);
											base.IsFileFormatEncryptedS19 = false;
											MemoryStream ms = new MemoryStream();
											_deviceConnector.Download("_miniapps\\" + cmd_GetFPGAVersion.Version + "\\MSP430Firmware.bin", ref ms, showProgress: true);
											ms.Seek(0L, SeekOrigin.Begin);
											byte[] array = new byte[(int)ms.Length];
											ms.Read(array, 0, (int)ms.Length);
											ms.Close();
											ms.Dispose();
											ReportOverallProgress("Recovery Mode - Erasing", 66);
											ReportProgress("", "", 0);
											Cmd_EraseAppFlash cmd_EraseAppFlash = new Cmd_EraseAppFlash(encrypted: false, AppFlashAreas.MSP430App);
											num2 = _comm.IssueCommand(_serialNumber, cmd_EraseAppFlash, 2500.0, 55000.0);
											if (cmd_EraseAppFlash.ErrorCode != 0) {
												throw new Exception("Error erasing MSP430 during recovery");
											}
											ReportOverallProgress("Recovery Mode - Writing", 100);
											ReportProgress("", "", 1);
											Cmd_WriteAppFlash cmd_WriteAppFlash = new Cmd_WriteAppFlash(encrypted: false, AppFlashAreas.MSP430App, (uint)array.Length, array, prependHeader: false);
											num2 = _comm.IssueCommand(_serialNumber, cmd_WriteAppFlash);
											if (cmd_WriteAppFlash.ErrorCode != 0) {
												throw new Exception("Error writing MSP430 during recovery");
											}
										}
										if (cmd_GetRecoveryState.RecoverNIOSApp) {
											ResetProgressBar();
											ReportOverallProgress("Recovery Mode - Application", 33);
											ReportProgress("", "", 0);
											RecoverApplication(cmd_GetFPGAVersion);
										}
										ReportOverallProgress("Recovery Mode - Resetting", 100);
										ReportProgress("", "", 0);
										if (ResetDeviceAndReconnect(cmd_GetRecoveryState.RecoverMSP430App, 60000)) {
											i = 0;
										}
										continue;
									}
									throw new Exception("Error getting FPGA version during recovery");
								}
								throw new Exception("Error getting recovery state");
							} catch (Exception ex2) {
								throw ex2;
							} finally {
								ReportProgress("", "", 0);
								ReportOverallProgress("", 0);
								_showGeneralEventType = 1;
							}
						}
						if (i == 0 && ex.Message.IndexOf("0x02") != -1) {
							_appAlreadyLoaded = true;
							StopMiniApp();
							Thread.Sleep(100);
							continue;
						}
						throw new Exception(ex.Message);
					} catch (Exception ex3) {
						if (i == 0 && ex3.Message.IndexOf("0x02") != -1) {
							_appAlreadyLoaded = true;
							StopMiniApp();
							Thread.Sleep(100);
							continue;
						}
						throw ex3;
					}
				}
			}
			goto IL_03f9;
		IL_03f9:
			return _productSerialNumber;
		}

		private void RecoverApplication(Cmd_GetFPGAVersion cmdFPGAVersion) {
			base.IsFileFormatEncryptedS19 = false;
			MemoryStream ms = new MemoryStream();
			_deviceConnector.Download(string.Format("_miniapps\\{0}\\Firmware.bin" + PlatformFileExtension, cmdFPGAVersion.Version), ref ms, showProgress: true);
			ms.Seek(0L, SeekOrigin.Begin);
			byte[] array = new byte[(int)ms.Length];
			ms.Read(array, 0, (int)ms.Length);
			ms.Close();
			ms.Dispose();
			ReportOverallProgress("Recovery Mode - Erasing", 66);
			ReportProgress("", "", 0);
			Cmd_EraseAppFlash cmd_EraseAppFlash = new Cmd_EraseAppFlash(encrypted: false, AppFlashAreas.NIOSApp);
			int num = _comm.IssueCommand(_serialNumber, cmd_EraseAppFlash, 25000.0, 80000.0);
			if (cmd_EraseAppFlash.ErrorCode == 0) {
				ReportOverallProgress("Recovery Mode - Writing", 100);
				ReportProgress("", "", 1);
				Cmd_WriteAppFlash cmd_WriteAppFlash = new Cmd_WriteAppFlash(encrypted: false, AppFlashAreas.NIOSApp, (uint)array.Length, array, prependHeader: true);
				num = _comm.IssueCommand(_serialNumber, cmd_WriteAppFlash);
				if (cmd_WriteAppFlash.ErrorCode == 80) {
					Cmd_MA_FormatDrive cmd_MA_FormatDrive = new Cmd_MA_FormatDrive(encrypted: false);
					num = _comm.IssueCommand(_serialNumber, cmd_MA_FormatDrive, 60000.0, 60000.0, 60000.0);
					if (cmd_MA_FormatDrive.ErrorCode != 0) {
						throw new Exception("Error formatting NAND Flash during recovery");
					}
					cmd_WriteAppFlash = new Cmd_WriteAppFlash(encrypted: false, AppFlashAreas.NIOSApp, (uint)array.Length, array, prependHeader: true);
					num = _comm.IssueCommand(_serialNumber, cmd_WriteAppFlash);
				}
				if (cmd_WriteAppFlash.ErrorCode != 0) {
					throw new Exception("Error writing Application during recovery");
				}
				return;
			}
			throw new Exception("Error erasing Application during recovery");
		}

		private bool StopMiniApp() {
			bool result = false;
			Cmd_StopApp cmd_StopApp = null;
			try {
				cmd_StopApp = new Cmd_StopApp(encrypted: false);
				cmd_StopApp.SavePages = true;
				int num = _comm.IssueCommand(_serialNumber, cmd_StopApp);
				if (!cmd_StopApp.RespSuccess) {
					_deviceConnector.Log("Error response to Cmd_StopApp command. Command Packet: " + BitConverter.ToString(cmd_StopApp.CommandPacket).Replace("-", " ") + "Response Pages: " + PagesToString(cmd_StopApp.ResponsePages));
				}
			} catch (ProteusCmdException ex) {
				if (cmd_StopApp != null) {
					_deviceConnector.Log("ProteusCmdException for Cmd_StopApp command. Command Packet: " + BitConverter.ToString(cmd_StopApp.CommandPacket).Replace("-", " ") + "Response Pages: " + PagesToString(cmd_StopApp.ResponsePages));
				}
				throw new Exception(ex.Message);
			} catch (Exception ex2) {
				if (cmd_StopApp != null) {
					_deviceConnector.Log("Exception for Cmd_StopApp command. Command Packet: " + BitConverter.ToString(cmd_StopApp.CommandPacket).Replace("-", " ") + "Response Pages: " + PagesToString(cmd_StopApp.ResponsePages));
				}
				throw new Exception(ex2.Message);
			}
			return result;
		}

		protected bool DecodeStringVersion(string version, ref byte[] bytes, uint offset) {
			bool result = false;
			if (version.Length > 0) {
				char[] separator = new char[1]
				{
					'.'
				};
				string[] array = version.Split(separator);
				if (array.Length >= 2) {
					bytes[offset] = (byte)array[0][0];
					bytes[offset + 1] = 46;
					bytes[offset + 2] = (byte)array[1][0];
					if (array.Length >= 3) {
						int num = Convert.ToInt32(array[2]);
						bytes[offset + 4] = (byte)((num & 0xFF00) >> 8);
						bytes[offset + 5] = (byte)((uint)num & 0xFFu);
					} else {
						bytes[offset + 4] = 0;
						bytes[offset + 5] = 0;
					}
					result = true;
				}
			}
			return result;
		}

		protected override byte[] GetBootLoaderVersion() {
			byte[] bytes = null;
			if (_FPGAVersionString.Length == 0) {
				for (int i = 0; i < 2; i++) {
					Cmd_GetVersion cmd_GetVersion = null;
					try {
						cmd_GetVersion = new Cmd_GetVersion(encrypted: false, platformType: true, appFPGA: false, calibration: true, firmware_NIOS: true, fpga_NIOS: true, firmware_MSP430: true);
						int num = _comm.IssueCommand(_serialNumber, cmd_GetVersion);
						if (cmd_GetVersion.Version_FPGA_NIOS != null) {
							_FPGAVersionString = cmd_GetVersion.Version_FPGA_NIOS;
						}
						if (cmd_GetVersion.Version_Firmware_NIOS != null) {
							_FirmwareVersionString = cmd_GetVersion.Version_Firmware_NIOS;
						}
						if (cmd_GetVersion.Version_Firmware_MSP430 != null) {
							_MSP430VersionString = cmd_GetVersion.Version_Firmware_MSP430;
						}
						if (cmd_GetVersion.Version_Calibration != null) {
							_CalibrationVersionString = cmd_GetVersion.Version_Calibration;
						}
						_PlatformType = cmd_GetVersion.PlatformType;
						GetSerialNumber();
						DisplayDeviceMessage("Updating. Do not disconnect!");
					} catch (ProteusCmdException ex) {
						if (i == 0 && ex.Message.IndexOf("0x01") != -1) {
							_appAlreadyLoaded = true;
							StopMiniApp();
							Thread.Sleep(100);
							continue;
						}
						throw new Exception(ex.Message);
					} catch (Exception ex2) {
						if (i == 0 && ex2.Message.IndexOf("0x01") != -1) {
							_appAlreadyLoaded = true;
							StopMiniApp();
							Thread.Sleep(100);
							continue;
						}
						throw ex2;
					}
					break;
				}
			}
			if (_FirmwareVersionString.Length > 0) {
				bytes = new byte[8]
				{
					118,
					118,
					0,
					0,
					0,
					0,
					0,
					0
				};
				if (!DecodeStringVersion(_FirmwareVersionString, ref bytes, 2u)) {
					bytes = null;
				}
			}
			return bytes;
		}

		public void DisplayDeviceMessage(string message) {
			Cmd_DisplayMessage cmd_DisplayMessage = null;
			try {
				cmd_DisplayMessage = new Cmd_DisplayMessage(encrypted: false, "messagewindow", message);
				_comm.IssueCommand(_serialNumber, cmd_DisplayMessage);
			} catch (Exception) {
			}
		}

		public override void UpdateComplete() {
			DisplayDeviceMessage("You may now disconnect your device...");
			_settings = null;
			base.UpdateComplete();
		}

		public override void FinalizeUpdate() {
			new EdgeDeviceLibrary.FusionService.FusionService().ClearSKUChangedFromUserSerialNum(_deviceConnector.Email, _deviceConnector.Password, GetSerialNumber());
		}

		public override string GetBootloaderVersionString() {
			byte[] bootLoaderVersion = GetBootLoaderVersion();
			string text = "";
			if (bootLoaderVersion != null) {
				for (int i = 1; i < bootLoaderVersion.Length - 2; i++) {
					if (bootLoaderVersion[i] != 0) {
						string str = text;
						char c = (char)bootLoaderVersion[i];
						text = str + c;
					}
				}
			}
			return text;
		}

		public override void ReadVersionFile(ref MemoryStream ms) {
			ReadFat("version.bin", bSkipError: false, ShowsProgressBar: false, ref ms);
		}

		public override string GetCalibrationVersionString() {
			short[] calibrationVersion = GetCalibrationVersion();
			return calibrationVersion[0] + "." + calibrationVersion[1] + "." + calibrationVersion[2];
		}

		public override string GetFirmwareVersionString() {
			GetBootLoaderVersion();
			return _FirmwareVersionString + "-" + _MSP430VersionString + "-" + _FPGAVersionString;
		}

		public override short[] GetFirmwareVersion() {
			short[] array = new short[3];
			try {
				byte[] bootLoaderVersion = GetBootLoaderVersion();
				array[0] = Convert.ToInt16(bootLoaderVersion[2] - 48);
				array[1] = Convert.ToInt16(bootLoaderVersion[4] - 48);
				array[2] = Convert.ToInt16(bootLoaderVersion[6] << 8);
				array[2] += Convert.ToInt16(bootLoaderVersion[7]);
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
			return array;
		}

		public override short[] GetCalibrationVersion() {
			short[] array = new short[3];
			try {
				byte[] array2 = ReadSettings();
				if (array2 != null) {
					array[0] = array2[4];
					array[1] = array2[5];
					array[2] = Convert.ToInt16(array2[6] << 8);
					ref short reference = ref array[2];
					reference = (short)(reference + array2[7]);
					_hasHotLevels = array2[9] != 0;
				}
				MemoryStream ms = new MemoryStream();
				if (ReadFat("\\UIResources\\Version.txt", bSkipError: false, ShowsProgressBar: false, ref ms)) {
					string text = Encoding.ASCII.GetString(ms.ToArray()).Trim();
					string[] array3 = text.Split('.');
					for (int i = 0; i < 3; i++) {
						if (array3.Length <= i || !short.TryParse(array3[i], out array[i])) {
							array[i] = 0;
						}
					}
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
			return array;
		}

		private string GetCurrentDir() {
			string result = "";
			Cmd_MA_GetCWD cmd_MA_GetCWD = null;
			try {
				cmd_MA_GetCWD = new Cmd_MA_GetCWD(encrypted: false);
				int num = _comm.IssueCommand(_serialNumber, cmd_MA_GetCWD);
				if (cmd_MA_GetCWD.ErrorCode == 0) {
					int num2 = cmd_MA_GetCWD.CurrentDir.IndexOf('\0');
					result = ((num2 < 0) ? cmd_MA_GetCWD.CurrentDir : cmd_MA_GetCWD.CurrentDir.Substring(0, num2));
				}
			} catch (Exception ex) {
				throw ex;
			}
			return result;
		}

		private bool LoadSettingRoot(string name, ref string value, string defaultValue) {
			bool result = false;
			Cmd_MA_MkChRmDir cmd_MA_MkChRmDir = DirActionOnDevice(DirActions.ChDir, encrypted: false, "/UIResources");
			if (cmd_MA_MkChRmDir == null) {
				throw new Exception("Can't change to /UIResources dir");
			}
			try {
				MemoryStream ms = new MemoryStream();
				if (ReadFat("GeneralSettings.xml", bSkipError: false, ShowsProgressBar: false, ref ms)) {
					ms.Seek(0L, SeekOrigin.Begin);
					XmlReader reader = XmlReader.Create(ms);
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(reader);
					XPathNavigator xPathNavigator = xmlDocument.CreateNavigator();
					try {
						XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)xPathNavigator.Evaluate("/root/" + name + "/@value");
						if (xPathNodeIterator.MoveNext()) {
							if (xPathNodeIterator.Current.IsNode) {
								value = xPathNodeIterator.Current.Value;
								result = true;
							}
						} else {
							xPathNodeIterator = (XPathNodeIterator)xPathNavigator.Evaluate("/root");
							if (xPathNodeIterator.MoveNext() && xPathNodeIterator.Current.IsNode) {
								xPathNodeIterator.Current.AppendChild("<" + name + " value=\"" + defaultValue + "\" />");
								value = defaultValue;
								MemoryStream memoryStream = new MemoryStream();
								XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, null);
								xmlDocument.Save(memoryStream);
								byte[] array = new byte[memoryStream.Length];
								memoryStream.Seek(0L, SeekOrigin.Begin);
								memoryStream.Read(array, 0, (int)memoryStream.Length);
								result = WriteFat(array, "GeneralSettings.xml");
								memoryStream.Close();
								memoryStream.Dispose();
							}
						}
					} catch (Exception) {
						result = false;
					} finally {
						ms.Close();
					}
				}
			} catch {
				value = defaultValue;
			}
			return result;
		}

		private bool LoadSettingRoot(string name, ref int value, int defaultValue) {
			bool flag = false;
			string value2 = "";
			flag = LoadSettingRoot(name, ref value2, defaultValue.ToString());
			try {
				if (name.CompareTo("majorversion") == 0) {
					value = 0;
				}
				value = Convert.ToInt32(value2);
			} catch {
				value = 0;
			}
			return flag;
		}

		private bool LoadSettingRoot(string name, ref bool value, bool defaultValue) {
			bool flag = false;
			string value2 = "";
			flag = LoadSettingRoot(name, ref value2, defaultValue.ToString());
			value = value2.ToLower().CompareTo("true") == 0;
			return flag;
		}

		private bool SaveSettingRoot(string name, string value) {
			bool result = false;
			Cmd_MA_MkChRmDir cmd_MA_MkChRmDir = DirActionOnDevice(DirActions.ChDir, encrypted: false, "/UIResources");
			if (cmd_MA_MkChRmDir == null) {
				throw new Exception("Can't change to /UIResources dir");
			}
			MemoryStream ms = new MemoryStream();
			if (ReadFat("GeneralSettings.xml", bSkipError: false, ShowsProgressBar: false, ref ms)) {
				ms.Seek(0L, SeekOrigin.Begin);
				XmlReader reader = XmlReader.Create(ms);
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(reader);
				XPathNavigator xPathNavigator = xmlDocument.CreateNavigator();
				try {
					XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)xPathNavigator.Evaluate("/root/" + name + "/@value");
					if (xPathNodeIterator.MoveNext()) {
						if (xPathNodeIterator.Current.IsNode) {
							xPathNodeIterator.Current.SetTypedValue(value);
							result = true;
						}
					} else {
						xPathNodeIterator = (XPathNodeIterator)xPathNavigator.Evaluate("/root");
						if (xPathNodeIterator.MoveNext() && xPathNodeIterator.Current.IsNode) {
							xPathNodeIterator.Current.AppendChild("<" + name + " value=\"" + value + "\" />");
						}
					}
					MemoryStream memoryStream = new MemoryStream();
					XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, null);
					xmlDocument.Save(memoryStream);
					byte[] array = new byte[memoryStream.Length];
					memoryStream.Seek(0L, SeekOrigin.Begin);
					memoryStream.Read(array, 0, (int)memoryStream.Length);
					result = WriteFat(array, "GeneralSettings.xml");
					memoryStream.Close();
					memoryStream.Dispose();
				} catch (Exception) {
					result = false;
				} finally {
					ms.Close();
				}
			}
			return result;
		}

		private bool SaveSettingRoot(string name, int value) {
			return SaveSettingRoot(name, value.ToString());
		}

		private bool SaveSettingRoot(string name, bool value) {
			string value2 = "false";
			if (value) {
				value2 = "true";
			}
			return SaveSettingRoot(name, value2);
		}

		public override byte[] ReadSettings() {
			if (_settings == null) {
				bool value = false;
				LoadSettingRoot("stockwriter", ref value, defaultValue: false);
				int value2 = 0;
				LoadSettingRoot("currentlevel", ref value2, 0);
				string value3 = "";
				LoadSettingRoot("lastoem", ref value3, "none");
				byte b = 0;
				switch (value3) {
					case "GMCAN":
					case "GMJ1850":
						b = 2;
						break;
					case "FordCAN":
					case "FordJ1850":
						b = 3;
						break;
					case "Dodge":
						b = 4;
						break;
				}
				bool value4 = false;
				LoadSettingRoot("hotlevelsavailable", ref value4, defaultValue: false);
				_settings = new byte[10];
				_settings[0] = 9;
				_settings[1] = 0;
				_settings[2] = (byte)value2;
				_settings[3] = (byte)(value ? 1u : 0u);
				_settings[4] = 0;
				_settings[5] = 0;
				_settings[6] = 0;
				_settings[7] = 0;
				_settings[8] = b;
				_settings[9] = (byte)(value4 ? 1u : 0u);
			}
			return _settings;
		}

		public override bool EraseSettings() {
			byte[] array = ReadSettings();
			if (array == null) {
				return false;
			}
			array[2] = 0;
			WriteSettings(array);
			DeleteVehicleSettings();
			return true;
		}

		private void DeleteVehicleSettings() {
			if (ChangeDirectory("\\GM\\")) {
				DeleteFile("Settings.xml");
			}
			if (ChangeDirectory("\\Ford\\")) {
				DeleteFile("Settings.xml");
			}
			if (ChangeDirectory("\\Dodge\\")) {
				DeleteFile("Settings.xml");
			}
			if (ChangeDirectory("\\Nissan\\")) {
				DeleteFile("Settings.xml");
			}
			if (ChangeDirectory("\\EAS\\")) {
				DeleteFile("Settings.xml");
			}
			if (!ChangeDirectory("\\")) {
				return;
			}
			Cmd_MA_Dir cmd_MA_Dir = LoadDir(encrypted: false, "*.xml");
			if (cmd_MA_Dir == null || cmd_MA_Dir.Entries == null) {
				return;
			}
			for (int i = 0; i < cmd_MA_Dir.Entries.Length; i++) {
				if (cmd_MA_Dir.Entries[i].Filename.EndsWith(".xml") && (cmd_MA_Dir.Entries[i].Filename.StartsWith("eeprom") || cmd_MA_Dir.Entries[i].Filename.StartsWith("EE_"))) {
					DeleteFile(cmd_MA_Dir.Entries[i].Filename);
				}
			}
		}

		public override bool ResetToFactorySettings() {
			if (ChangeDirectory("\\UIResources\\")) {
				Cmd_MA_Dir cmd_MA_Dir = LoadDir(encrypted: false, "*.xml");
				if (cmd_MA_Dir != null && cmd_MA_Dir.Entries != null) {
					for (int i = 0; i < cmd_MA_Dir.Entries.Length; i++) {
						if (cmd_MA_Dir.Entries[i].Filename.EndsWith(".xml") && (cmd_MA_Dir.Entries[i].Filename.StartsWith("GeneralSettings") || cmd_MA_Dir.Entries[i].Filename.StartsWith("GS_"))) {
							DeleteFile(cmd_MA_Dir.Entries[i].Filename);
						}
					}
				}
			}
			_settings = null;
			DeleteVehicleSettings();
			return true;
		}

		public override void WriteSettings(byte[] baSettings) {
			if (baSettings.Length >= 9) {
				SaveSettingRoot("stockwriter", baSettings[3] == 1);
				SaveSettingRoot("currentlevel", baSettings[2].ToString());
			}
		}

		public override bool IsProgrammingInterrupted() {
			byte[] array = ReadSettings();
			if (array == null) {
				return false;
			}
			if (array[3] > 0) {
				return true;
			}
			return false;
		}

		public override int GetProgrammedToLevel() {
			byte[] array = ReadSettings();
			if (array == null || _deviceConnector == null) {
				return -1;
			}
			int num = 0;
			try {
				num = new EdgeDeviceLibrary.FusionService.FusionService().GetModelLevelOffsetInSettings(_deviceConnector.Email, _deviceConnector.Password, base.ProductIdentifier);
			} catch {
				num = 2;
			}
			return array[num];
		}

		public override void ClearInternalFlash(uint address, uint endaddress) {
		}

		public override bool Send_S19_File(string SendData) {
			return false;
		}

		protected override bool Send_S19_Line(string OutData) {
			return false;
		}

		public override bool SendSupportFilesToDevice(bool lastStep) {
			string text;
			try {
				EdgeDeviceLibrary.FusionService.FusionService fusionService = new EdgeDeviceLibrary.FusionService.FusionService();
				text = fusionService.GetListOfFilesToSendToDevice2(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber().ToString(), lastStep);
			} catch {
				text = string.Empty;
			}
			if (text.Length == 0) {
				return false;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(text);
			foreach (XmlNode item in xmlDocument.DocumentElement.SelectNodes("sendFile")) {
				MemoryStream ms = new MemoryStream();
				string text2 = string.Empty;
				try {
					text2 = item.SelectSingleNode("serverFileName").InnerText;
					int result = 0;
					if (!int.TryParse(item.SelectSingleNode("deviceSendFileId").InnerText, out result)) {
						continue;
					}
					if (DeviceConnector.Instance().Download(text2, ref ms, showProgress: true)) {
						string innerText = item.SelectSingleNode("targetFileName").InnerText;
						StringBuilder stringBuilder = new StringBuilder(item.SelectSingleNode("targetPath").InnerText);
						if (string.Compare(Path.GetExtension(innerText), ".zip", ignoreCase: true) == 0) {
							ZipFile zipFile = new ZipFile(ms);
							for (int i = 0; i < zipFile.Size; i++) {
								ZipEntry zipEntry = zipFile[i];
								stringBuilder.Length = 0;
								stringBuilder.Append(zipEntry.Name.Replace("/", "\\"));
								if (zipEntry.IsDirectory) {
									ReformatPath(stringBuilder);
									ChangeMakeDir(stringBuilder.ToString(), string.Empty);
									continue;
								}
								innerText = Path.GetFileName(stringBuilder.ToString());
								string directoryName = Path.GetDirectoryName(stringBuilder.ToString());
								stringBuilder.Length = 0;
								stringBuilder.Append(directoryName);
								ReformatPath(stringBuilder);
								Stream inputStream = zipFile.GetInputStream(zipEntry);
								byte[] buffer = new byte[2048];
								int num = inputStream.Read(buffer, 0, 2048);
								MemoryStream memoryStream = new MemoryStream();
								while (num > 0) {
									memoryStream.Write(buffer, 0, num);
									num = inputStream.Read(buffer, 0, 2048);
								}
								memoryStream.Flush();
								ChangeMakeDir(stringBuilder.ToString(), string.Empty);
								WriteFat(memoryStream, innerText, fileEncrypted: false);
								memoryStream.Dispose();
								ResetFileVersionToZero(innerText);
							}
							zipFile.Close();
						} else {
							stringBuilder.Replace("/", "\\");
							stringBuilder.Replace("\\\\", "\\");
							ReformatPath(stringBuilder);
							ChangeMakeDir(stringBuilder.ToString(), string.Empty);
							WriteFat(ms, innerText, fileEncrypted: false);
							ResetFileVersionToZero(innerText);
						}
					}
					EdgeDeviceLibrary.FusionService.FusionService fusionService2 = new EdgeDeviceLibrary.FusionService.FusionService();
					fusionService2.SetFileAsSent(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, result);
				} catch (Exception ex) {
					_deviceConnector.Log("Failed to send file '" + text2 + "' to device; Exception Information: " + ex.Message);
					return false;
				} finally {
					ms?.Dispose();
				}
			}
			return true;
		}

		public override string GetUpgradeOptions() {
			return new EdgeDeviceLibrary.FusionService.FusionService().GetCanUpgradeSKUFromUserSerialNum(_deviceConnector.Email, _deviceConnector.Password, GetSerialNumber());
		}

		public override bool HasSkuChanged() {
			return new EdgeDeviceLibrary.FusionService.FusionService().GetSKUChangedFromUserSerialNum(_deviceConnector.Email, _deviceConnector.Password, GetSerialNumber());
		}

		public override void PrepareForSkuChange() {
			int showGeneralEventType = _showGeneralEventType;
			_showGeneralEventType = 0;
			if (!FormatDrive()) {
				throw new Exception("Error formatting drive");
			}
			_deviceConnector.Log("Formatted for SKU change.");
			_showGeneralEventType = showGeneralEventType;
		}

		public override string[] GetUnsupportedSkus() {
			return UnsupportedSkus;
		}

		protected bool TryReadGmskSettings(out List<GmskRequest> gmskRequests) {
			bool result = false;
			gmskRequests = new List<GmskRequest>();
			try {
				MemoryStream ms = new MemoryStream();
				if (ReadFat("Settings.xml", bSkipError: false, ShowsProgressBar: false, ref ms)) {
					ms.Seek(0L, SeekOrigin.Begin);
					XmlReader reader = XmlReader.Create(ms);
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(reader);
					XPathNavigator xPathNavigator = xmlDocument.CreateNavigator();
					try {
						XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)xPathNavigator.Evaluate("/root/vehicles/vehicle/fivebyte");
						while (xPathNodeIterator.MoveNext()) {
							if (xPathNodeIterator.Current.IsNode) {
								gmskRequests.Add(new GmskRequest {
									Module = xPathNodeIterator.Current.GetAttribute("module", ""),
									Request = xPathNodeIterator.Current.GetAttribute("request", ""),
									Seed = xPathNodeIterator.Current.GetAttribute("seed", "")
								});
								result = true;
							}
						}
					} catch (Exception) {
						result = false;
					} finally {
						ms.Close();
					}
				}
			} catch {
			}
			return result;
		}

		protected bool TryReadOemSettings(string xpath, out string value) {
			bool result = false;
			value = null;
			try {
				MemoryStream ms = new MemoryStream();
				if (ReadFat("Settings.xml", bSkipError: false, ShowsProgressBar: false, ref ms)) {
					ms.Seek(0L, SeekOrigin.Begin);
					XmlReader reader = XmlReader.Create(ms);
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(reader);
					XPathNavigator xPathNavigator = xmlDocument.CreateNavigator();
					try {
						XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)xPathNavigator.Evaluate(xpath);
						if (xPathNodeIterator.MoveNext() && xPathNodeIterator.Current.IsNode) {
							value = xPathNodeIterator.Current.Value;
							result = true;
						}
					} catch (Exception) {
						result = false;
					} finally {
						ms.Close();
					}
				}
			} catch {
			}
			return result;
		}

		public override bool NeedsGmsk() {
			bool result = false;
			OEMType oemType = GetOemType();
			if (oemType != OEMType.OEMType_GM || !ChangeDirectory("/GM/")) {
				return false;
			}
			List<GmskRequest> gmskRequests = null;
			if (TryReadGmskSettings(out gmskRequests)) {
				foreach (GmskRequest item in gmskRequests) {
					if (item.Request.CompareTo("1") == 0) {
						result = true;
					}
				}
			}
			return result;
		}

		public override bool TryGetGmskRequests(out List<GmskRequest> requests) {
			requests = null;
			bool result = false;
			OEMType oemType = GetOemType();
			if (oemType != OEMType.OEMType_GM || !ChangeDirectory("/GM/")) {
				return false;
			}
			string value = null;
			if (TryReadOemSettings("/root/currentvin/@value", out value) && TryReadGmskSettings(out requests)) {
				string serialNumber = GetSerialNumber().ToString();
				foreach (GmskRequest request in requests) {
					request.SerialNumber = serialNumber;
					request.Vin = value;
				}
				result = true;
			}
			return result;
		}

		public override bool SetGmsk(string key, GmskRequest request) {
			bool result = false;
			OEMType oemType = GetOemType();
			if (oemType != OEMType.OEMType_GM || !ChangeDirectory("/GM/")) {
				return false;
			}
			try {
				MemoryStream ms = new MemoryStream();
				if (ReadFat("Settings.xml", bSkipError: false, ShowsProgressBar: false, ref ms)) {
					ms.Seek(0L, SeekOrigin.Begin);
					XmlReader reader = XmlReader.Create(ms);
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(reader);
					XPathNavigator xPathNavigator = xmlDocument.CreateNavigator();
					try {
						XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)xPathNavigator.Evaluate("/root/vehicles/vehicle/fivebyte");
						while (xPathNodeIterator.MoveNext()) {
							if (xPathNodeIterator.Current.IsNode) {
								string attribute = xPathNodeIterator.Current.GetAttribute("module", "");
								string attribute2 = xPathNodeIterator.Current.GetAttribute("seed", "");
								if (request.Module.CompareTo(attribute) == 0 && request.Seed.CompareTo(attribute2) == 0) {
									xPathNodeIterator.Current.MoveToAttribute("key", xPathNodeIterator.Current.NamespaceURI);
									xPathNodeIterator.Current.SetValue(key);
									xPathNodeIterator.Current.MoveToParent();
									xPathNodeIterator.Current.MoveToAttribute("request", xPathNodeIterator.Current.NamespaceURI);
									xPathNodeIterator.Current.SetValue("0");
									xPathNodeIterator.Current.MoveToParent();
								}
							}
						}
						MemoryStream memoryStream = new MemoryStream();
						XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, null);
						xmlDocument.Save(memoryStream);
						byte[] array = new byte[memoryStream.Length];
						memoryStream.Seek(0L, SeekOrigin.Begin);
						memoryStream.Read(array, 0, (int)memoryStream.Length);
						result = WriteFat(array, "Settings.xml");
						memoryStream.Close();
						memoryStream.Dispose();
					} catch (Exception) {
						result = false;
					} finally {
						ms.Close();
					}
				}
			} catch {
			}
			return result;
		}

		private void ResetFileVersionToZero(string fileName) {
			try {
				Cmd_MA_GetFileInfo cmd_MA_GetFileInfo = new Cmd_MA_GetFileInfo(encrypted: false, fileName);
				int num = _comm.IssueCommand(_serialNumber, cmd_MA_GetFileInfo);
				if (cmd_MA_GetFileInfo == null) {
					throw new Exception("Error Updating File Info (Get): " + fileName);
				}
				if (cmd_MA_GetFileInfo.ErrorCode != 0) {
					throw new Exception("Error Updating File Info (Get): " + fileName + " - " + cmd_MA_GetFileInfo.ErrorCode + " - " + cmd_MA_GetFileInfo.ExtendedErrorCode);
				}
				FileInfoSecurity securityRaw = cmd_MA_GetFileInfo.SecurityRaw;
				securityRaw.FileVersion = 0;
				Cmd_MA_SetFileInfo cmd_MA_SetFileInfo = new Cmd_MA_SetFileInfo(encrypted: false, fileName, setDateTime: true, DateTime.Now, setSecurity: true, securityRaw);
				num = _comm.IssueCommand(_serialNumber, cmd_MA_SetFileInfo);
				if (cmd_MA_SetFileInfo == null) {
					throw new Exception("Error Updating File Info (Set): " + fileName);
				}
				if (cmd_MA_SetFileInfo.ErrorCode != 0) {
					throw new Exception("Error Updating File Info (Set): " + fileName + " - " + cmd_MA_SetFileInfo.ErrorCode + " - " + cmd_MA_SetFileInfo.ExtendedErrorCode);
				}
			} catch (Exception) {
			}
		}

		private static void ReformatPath(StringBuilder targetPath) {
			if (targetPath.Length > 0) {
				if (targetPath[0] != '\\') {
					targetPath.Insert(0, '\\');
				}
				if (targetPath[targetPath.Length - 1] != '\\') {
					targetPath.Append('\\');
				}
			} else {
				targetPath.Append('\\');
			}
		}
	}
}
