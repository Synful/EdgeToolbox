using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using MXZipLibrary;

namespace EdgeDeviceLibrary.Products {
	public abstract class Product_Base {
		protected byte[] vidBytes = null;

		public abstract void ValidateStockFiles();

		public virtual void PerformDeviceConnectionTasks(BackgroundWorker bw) {
			UploadVehicleUserIdentifierFile("vid.bin");
			UploadVehicleUserIdentifierFile("cs_info.bin");
			ValidateStockFiles();
		}

		protected void UploadVehicleUserIdentifierFile(string fileName) {
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			MemoryStream ms = new MemoryStream();
			if (deviceConnector.communicator.ReadFat(fileName, bSkipError: false, ShowsProgressBar: false, ref ms)) {
				string RemoteRelativePathAndName = "_uploads\\devices\\" + DeviceConnector.Instance().communicator.GetDevicesTableID() + "\\" + fileName;
				deviceConnector.UploadStockFile(ref ms, ref RemoteRelativePathAndName);
			}
		}

		protected int FindBytes(byte[] source, byte[] search) {
			int result = -1;
			for (int i = 0; i < source.Length; i++) {
				if (source[i] == search[0]) {
					int num = 0;
					for (num = 1; num < search.Length && source[i + num] == search[num]; num++) {
					}
					if (num == search.Length) {
						result = i;
						break;
					}
				}
			}
			return result;
		}

		protected void VerifyChecksumsInStockCS(ChecksumInfo[] checkSumInfos) {
			string relativePathAndName = "_ver\\" + DeviceConnector.Instance().communicator.GetStockFolderName() + "_stock_files.bin";
			MemoryStream ms = new MemoryStream();
			DeviceConnector.Instance().Download(relativePathAndName, ref ms);
			byte[] array = ms.ToArray();
			Hashtable hashtable = new Hashtable();
			for (int i = 0; i < array.Length; i += 20) {
				string @string = Encoding.Default.GetString(array, i, 16);
				@string = @string.Substring(0, @string.IndexOf('\0')).ToLower();
				byte[] array2 = new byte[4];
				Array.Copy(array, i + 16, array2, 0, 4);
				uint num = BitConverter.ToUInt32(array2, 0);
				hashtable[@string] = num;
			}
			bool flag = false;
			bool flag2 = false;
			foreach (ChecksumInfo checksumInfo in checkSumInfos) {
				if (!(checksumInfo.StockCSFileName != "DELETED")) {
					continue;
				}
				if (hashtable[checksumInfo.StockFileName.ToLower()] == null) {
					switch (checksumInfo.ProcessingFlag) {
						case FileProcessFlag.NONE:
						case FileProcessFlag.NORMAL:
						case FileProcessFlag.UPDATE_REQ_ONLY:
							checksumInfo.RequiresUploadToWebsite = true;
							flag = true;
							break;
						case FileProcessFlag.UPDATE_REQ_NO_STOCKFILE:
							flag = true;
							break;
					}
				} else if ((uint)hashtable[checksumInfo.StockFileName.ToLower()] != checksumInfo.StockCSChecksum) {
					switch (checksumInfo.ProcessingFlag) {
						case FileProcessFlag.NONE:
						case FileProcessFlag.NORMAL:
						case FileProcessFlag.CHECKSUM_MISMATCH_ONLY:
							checksumInfo.RequiresDownloadFromWebsite = true;
							flag2 = true;
							break;
					}
				}
			}
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			if (flag || flag2) {
				string firmwareVersionString = deviceConnector.communicator.GetFirmwareVersionString();
				string calibrationVersionString = deviceConnector.communicator.GetCalibrationVersionString();
				uint serialNumber = deviceConnector.communicator.GetSerialNumber();
				long[] updateFirmwareAndCalibrationIDs = deviceConnector.communicator.GetUpdateFirmwareAndCalibrationIDs(serialNumber.ToString(), firmwareVersionString, calibrationVersionString);
				if (updateFirmwareAndCalibrationIDs != null) {
					if (updateFirmwareAndCalibrationIDs[0] == 0L || updateFirmwareAndCalibrationIDs[1] == 0) {
						throw new Exception("Current firmware and calibration id's are zero for this device - " + deviceConnector.communicator.GetPartNumber());
					}
					if (deviceConnector.communicator.DeviceCanBeUpdated()) {
						throw new Exception("UPDATEBEFORESTOCKOPERATION");
					}
				}
			}
			if (flag && MessageBox.Show("Your vehicle contains programming that is new to our Evolution. In order to program your vehicle we need to verify that the files read from your vehicle are correct. Without sending us these files we cannot program your vehicle. Would you like to send us these files now?", "New File Upload", MessageBoxButtons.YesNo) == DialogResult.Yes) {
				long devicesTableID = DeviceConnector.Instance().communicator.GetDevicesTableID();
				StringBuilder stringBuilder = new StringBuilder();
				string uploadedFilesList;
				using (StringWriter stringWriter = new StringWriter()) {
					XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlTextWriter.WriteStartDocument();
					xmlTextWriter.WriteStartElement("root");
					xmlTextWriter.WriteStartElement("cs_info");
					xmlTextWriter.WriteAttributeString("version", ((byte)ChecksumInfo.cs_infoVersion).ToString());
					xmlTextWriter.WriteEndElement();
					string text = "StockUpload.zip";
					MemoryStream ms2 = new MemoryStream();
					ZipOutputStream zipOutputStream = new ZipOutputStream(ms2);
					xmlTextWriter.WriteStartElement("stockFiles");
					foreach (ChecksumInfo checksumInfo2 in checkSumInfos) {
						xmlTextWriter.WriteStartElement("file");
						xmlTextWriter.WriteAttributeString("deviceFile", checksumInfo2.StockFileName);
						xmlTextWriter.WriteAttributeString("fusionFile", checksumInfo2.StockCSFileName);
						xmlTextWriter.WriteAttributeString("checksum", checksumInfo2.StockCSChecksum.ToString());
						xmlTextWriter.WriteAttributeString("requiresDownload", checksumInfo2.RequiresDownloadFromWebsite.ToString());
						xmlTextWriter.WriteAttributeString("requiresUpload", checksumInfo2.RequiresUploadToWebsite.ToString());
						xmlTextWriter.WriteAttributeString("processingFlag", ((byte)checksumInfo2.ProcessingFlag).ToString());
						xmlTextWriter.WriteEndElement();
						if (checksumInfo2.RequiresUploadToWebsite) {
							MemoryStream ms3 = new MemoryStream();
							deviceConnector.communicator.ReadFat(checksumInfo2.StockCSFileName, bSkipError: true, ShowsProgressBar: true, ref ms3);
							ms3.Seek(0L, SeekOrigin.Begin);
							byte[] array3 = new byte[ms3.Length];
							ms3.Read(array3, 0, (int)ms3.Length);
							ms3.Close();
							ZipEntry entry = new ZipEntry(ZipEntry.CleanName(checksumInfo2.StockCSFileName));
							zipOutputStream.PutNextEntry(entry);
							zipOutputStream.Write(array3, 0, array3.Length);
							deviceConnector.Log("Zipped stock file: " + checksumInfo2.StockCSFileName);
							stringBuilder.Append(checksumInfo2.StockFileName + "|-|");
						} else if (checksumInfo2.ProcessingFlag == FileProcessFlag.UPDATE_REQ_NO_STOCKFILE) {
							deviceConnector.Log("Update required with no stock file: " + checksumInfo2.StockCSFileName);
						}
					}
					xmlTextWriter.WriteEndElement();
					WriteFileToStockZip(deviceConnector, "vid.bin", stringBuilder, zipOutputStream);
					WriteFileToStockZip(deviceConnector, "cs_info.bin", stringBuilder, zipOutputStream);
					string RemoteRelativePathAndName = "_uploads\\devices\\" + deviceConnector.communicator.GetDevicesTableID() + "\\" + text;
					zipOutputStream.Finish();
					deviceConnector.UploadStockFile(ref ms2, ref RemoteRelativePathAndName);
					zipOutputStream.Close();
					deviceConnector.Log("Uploaded " + RemoteRelativePathAndName);
					stringBuilder.Append(Path.GetFileName(RemoteRelativePathAndName) + "|-|");
					xmlTextWriter.WriteStartElement("uploadedFilesList");
					xmlTextWriter.WriteString(stringBuilder.ToString());
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndDocument();
					xmlTextWriter.Flush();
					uploadedFilesList = stringWriter.ToString();
				}
				try {
					deviceConnector.Notify_ReceivedNewStockFiles(devicesTableID, uploadedFilesList);
				} catch (Exception) {
				}
			}
			if (!flag2) {
				return;
			}
			bool flag3 = true;
			if (deviceConnector.communicator.ProductIdentifier == 5 && MessageBox.Show("WARNING: Stock files don't match. If this vehicle has ever been programmed with another brand of programmer and we download stock files to it this may cause this vehicle to cease to function.\r\n\r\nThe only way to be sure is to have this vehicle checked at a dealer and recovered by them if it has been programmed before.\r\n\r\nIf you choose to continue and your vehicle was programmed with another programmer it may disable your vehicle. In that case the only method of recovery is to take the vehicle to the dealer to be reprogrammed. Most dealers charge to do that and Edge Products will not pay for it. Would you like to continue?", "Checksum Failure Detected", MessageBoxButtons.YesNo, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button2) != DialogResult.Yes) {
				flag3 = false;
			}
			if (!flag3 || MessageBox.Show("Stock files don't match what was expected.  Download new files?", "Checksum Failure Detected", MessageBoxButtons.YesNo) != DialogResult.Yes) {
				return;
			}
			foreach (ChecksumInfo checksumInfo3 in checkSumInfos) {
				if (!checksumInfo3.RequiresDownloadFromWebsite) {
					continue;
				}
				string relativePathAndName2 = "_stock\\" + deviceConnector.communicator.GetStockFolderName() + "\\" + checksumInfo3.StockFileName;
				MemoryStream ms4 = new MemoryStream();
				deviceConnector.Download(relativePathAndName2, ref ms4);
				deviceConnector.ReportProgress("Preparing device...", "", 51);
				byte[] array4 = ms4.ToArray();
				if (vidBytes != null) {
					byte[] search = new byte[14] {
						67, 111, 112, 121, 114, 105, 103, 104, 116, 32, 70, 111, 114, 100
					};
					int num2 = FindBytes(array4, search);
					if (num2 >= 0) {
						num2 -= 99;
						for (int m = 0; m < 256; m++) {
							array4[num2 + m] = vidBytes[m];
						}
					}
				}
				deviceConnector.communicator.WriteFat(array4, checksumInfo3.StockCSFileName);
				deviceConnector.Log("Stock checksum failed.  Sent " + checksumInfo3.StockFileName + " as " + checksumInfo3.StockCSFileName + " to device.");
			}
			deviceConnector.communicator.SetProgrammingInterruptedFlag();
		}

		private void WriteFileToStockZip(DeviceConnector dc, string fileName, StringBuilder uploadedFilesList, ZipOutputStream zipStream) {
			try {
				MemoryStream ms = new MemoryStream();
				if (dc.communicator.ReadFat(fileName, bSkipError: false, ShowsProgressBar: true, ref ms)) {
					ms.Seek(0L, SeekOrigin.Begin);
					byte[] array = new byte[ms.Length];
					ms.Read(array, 0, (int)ms.Length);
					ms.Close();
					ZipEntry entry = new ZipEntry(ZipEntry.CleanName(fileName));
					zipStream.PutNextEntry(entry);
					zipStream.Write(array, 0, array.Length);
					dc.Log("Zipped " + fileName);
					uploadedFilesList.Append(fileName + "|-|");
				}
			} catch (Exception) {
			}
		}
	}
}
