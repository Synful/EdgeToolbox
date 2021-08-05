#define TRACE
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Text;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using EdgeDeviceLibrary.FusionService;

namespace EdgeDeviceLibrary.Communicator
{
	public class Chameleon : Communicator_Base
	{
		protected byte[] baRomArea;

		private static Chameleon chamo;

		private string _evoModel;

		public void LogTimerTick(string sMessage)
		{
		}

		public static Chameleon Instance(SerialPort Port)
		{
			if (chamo == null)
			{
				chamo = new Chameleon(Port);
			}
			return chamo;
		}

		public Chameleon(SerialPort Port)
		{
			port = Port;
			baRomArea = ReadFlash(MemoryLocation.OnProcessor, 7680u, 512);
		}

		public override string GetPartNumber(bool cachedOk)
		{
			if (!string.IsNullOrEmpty(_evoModel))
			{
				return _evoModel;
			}
			string text = "UNIDENTIFIED";
			ushort num = (ushort)BitConverter.ToInt16(baRomArea, 0);
			Trace.WriteLine(num.ToString(), "Product ID #");
			if (num == ushort.MaxValue)
			{
				try
				{
					num = (ushort)new EdgeDeviceLibrary.FusionService.FusionService().GetModelFromUserSerialNum(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, GetSerialNumber());
				}
				catch (WebException)
				{
					num = 0;
				}
			}
			if (num == 0)
			{
				return string.Empty;
			}
			string[] array = null;
			try
			{
				nProductIdentifier = num;
				text = new EdgeDeviceLibrary.FusionService.FusionService().GetModelString(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, num);
				array = new EdgeDeviceLibrary.FusionService.FusionService().GetBootloaderInfo(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, nProductIdentifier);
				_evoModel = text;
			}
			catch (SoapException)
			{
				text = "UNIDENTIFIED";
				array = null;
			}
			catch (WebException)
			{
				text = "UNIDENTIFIED_NO_WEB";
				array = null;
			}
			if (array != null && array[3] == "Encrypted_S19")
			{
				base.IsFileFormatEncryptedS19 = true;
			}
			else
			{
				base.IsFileFormatEncryptedS19 = false;
			}
			Trace.WriteLine(text, "Model");
			return text;
		}

		public override uint GetSerialNumber()
		{
			return BitConverter.ToUInt32(baRomArea, 4);
		}

		protected override byte[] GetBootLoaderVersion()
		{
			byte[] buffer = new byte[1]
			{
				86
			};
			port.Write(buffer, 0, 1);
			return base.ReadByteArrayFromPort(0, 1000, VerifyChecksum: true);
		}

		public override string GetBootloaderVersionString()
		{
			byte[] bootLoaderVersion = GetBootLoaderVersion();
			string text = "";
			for (int i = 1; i < bootLoaderVersion.Length - 2; i++)
			{
				string str = text;
				char c = (char)bootLoaderVersion[i];
				text = str + c;
			}
			return text;
		}

		public override void ReadVersionFile(ref MemoryStream ms)
		{
			ReadFat("version.bin", bSkipError: false, ShowsProgressBar: false, ref ms);
		}

		public override string GetCalibrationVersionString()
		{
			short[] calibrationVersion = GetCalibrationVersion();
			return calibrationVersion[0] + "." + calibrationVersion[1] + "." + calibrationVersion[2];
		}

		public override string GetFirmwareVersionString()
		{
			short[] firmwareVersion = GetFirmwareVersion();
			return firmwareVersion[0] + "." + firmwareVersion[1] + "." + firmwareVersion[2];
		}

		public override short[] GetFirmwareVersion()
		{
			MemoryStream ms = new MemoryStream();
			ReadVersionFile(ref ms);
			short[] array = new short[3];
			try
			{
				ms.Seek(4L, SeekOrigin.Begin);
				array[0] = (short)ms.ReadByte();
				array[1] = (short)ms.ReadByte();
				byte[] array2 = new byte[2];
				ms.Read(array2, 0, 2);
				array[2] = BitConverter.ToInt16(array2, 0);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			return array;
		}

		public override short[] GetCalibrationVersion()
		{
			short[] array = new short[3];
			try
			{
				MemoryStream ms = new MemoryStream();
				if (DeviceConnector.Instance().communicator.ReadFat("version.bin", bSkipError: true, ShowsProgressBar: false, ref ms))
				{
					ms.Seek(8L, SeekOrigin.Begin);
					array[0] = (short)ms.ReadByte();
					array[1] = (short)ms.ReadByte();
					byte[] array2 = new byte[2];
					ms.Read(array2, 0, 2);
					array[2] = BitConverter.ToInt16(array2, 0);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			return array;
		}

		public override byte[] ReadSettings()
		{
			port.ReadExisting();
			byte[] buffer = new byte[1]
			{
				83
			};
			port.Write(buffer, 0, 1);
			byte[] array = ReadByteArrayFromPort(0, 1000, VerifyChecksum: true);
			int num = 0;
			try
			{
				num = BitConverter.ToUInt16(array, 1);
				if (num == 65535)
				{
					return null;
				}
			}
			catch
			{
				return null;
			}
			byte[] array2 = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = array[i + 1];
			}
			if (array[0] != 115)
			{
				throw new Exception("Communicator: ReadSettings Failed");
			}
			return array2;
		}

		public override bool IsProgrammingInterrupted()
		{
			byte[] array = ReadSettings();
			if (array == null)
			{
				return false;
			}
			if (array[3] > 0)
			{
				return true;
			}
			return false;
		}

		public override int GetProgrammedToLevel()
		{
			byte[] array = ReadSettings();
			if (array == null)
			{
				return -1;
			}
			int modelLevelOffsetInSettings = new EdgeDeviceLibrary.FusionService.FusionService().GetModelLevelOffsetInSettings(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, base.ProductIdentifier);
			return array[modelLevelOffsetInSettings];
		}

		public override void ClearInternalFlash(uint address, uint endaddress)
		{
			uint num = 8192u;
			for (uint num2 = address; num2 < endaddress; num2 += num)
			{
				DeviceConnector.Instance().communicator.SectorErase(70, num2);
				num = ((num2 >= 65536) ? ((num2 >= 196608) ? 8192u : 65536u) : 8192u);
			}
		}

		public override bool Send_S19_File(string SendData)
		{
			int startIndex = 0;
			bool flag = true;
			bool flag2 = false;
			StringBuilder stringBuilder = new StringBuilder();
			uint num = 0u;
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			deviceConnector.ResetProgressBar();
			while (startIndex < SendData.Length)
			{
				string nextLine = GetNextLine(SendData, ref startIndex);
				if (nextLine.Substring(0, 2) == "S8")
				{
					break;
				}
				if (flag)
				{
					flag2 = ((!(nextLine.Substring(2, 2) == "45")) ? true : false);
					Send_S19_Line(nextLine);
					flag = false;
					continue;
				}
				if (!flag2)
				{
					Send_S19_Line(nextLine);
					int percentageComplete = (int)((float)startIndex / (float)SendData.Length * 100f);
					deviceConnector.ReportProgress("Transferring to device...", percentageComplete + "% Complete", percentageComplete);
					continue;
				}
				if (num == 0)
				{
					num = uint.Parse(nextLine.Substring(4, 6), NumberStyles.HexNumber);
				}
				int num2 = int.Parse(nextLine.Substring(2, 2), NumberStyles.HexNumber);
				num2 -= 4;
				num2 *= 2;
				if (stringBuilder.Length + num2 >= 1024)
				{
					int num3 = 1024 - stringBuilder.Length;
					if (num3 > 0)
					{
						stringBuilder.Append(nextLine.Substring(10, num3));
					}
					if (stringBuilder.Length < 1024)
					{
						throw new Exception("aggBuff is not proper length - Communicator Base");
					}
					byte[] bytes = BitConverter.GetBytes(num);
					string str = bytes[2].ToString("X02") + bytes[1].ToString("X02") + bytes[0].ToString("X02");
					stringBuilder.Insert(0, "S30200" + str);
					Send_S19_Line(stringBuilder.ToString());
					stringBuilder = new StringBuilder();
					stringBuilder.Append(nextLine.Substring(10 + num3, nextLine.Length - 10 - num3 - 2));
					num += (uint)(512 - stringBuilder.Length);
					int percentageComplete2 = (int)((float)startIndex / (float)SendData.Length * 100f);
					deviceConnector.ReportProgress("Transferring to device...", percentageComplete2 + "% Complete", percentageComplete2);
				}
				else
				{
					stringBuilder.Append(nextLine.Substring(10, nextLine.Length - 12));
				}
			}
			if (stringBuilder.Length > 0)
			{
				byte[] bytes2 = BitConverter.GetBytes(num);
				string str2 = bytes2[2].ToString("X02") + bytes2[1].ToString("X02") + bytes2[0].ToString("X02");
				stringBuilder.Insert(0, "S3" + (stringBuilder.Length / 2).ToString("X04") + str2);
				Send_S19_Line(stringBuilder.ToString());
				int percentageComplete3 = (int)((float)startIndex / (float)SendData.Length * 100f);
				deviceConnector.ReportProgress("Transferring to device...", percentageComplete3 + "% Complete", percentageComplete3);
			}
			return true;
		}

		protected override bool Send_S19_Line(string OutData)
		{
			byte b = 0;
			byte b2 = 0;
			byte b3 = 0;
			ushort num = 0;
			ushort num2 = 0;
			byte[] array = new byte[4];
			byte[] array2 = new byte[0];
			Application.DoEvents();
			ushort num3 = ushort.Parse(OutData.Substring(1, 1));
			string text = OutData.Substring(2, 2);
			uint address;
			do
			{
				switch (num3)
				{
				case 0:
					SecID = 70;
					ErasedSector = 0u;
					if (text == "45")
					{
						ClearExternalFlash();
					}
					return true;
				case 1:
					MessageBox.Show("Oops - Hit case 1 in SendUSBLine");
					break;
				case 2:
					num = 10;
					num2 = (ushort)(ushort.Parse(text, NumberStyles.HexNumber) - 4);
					b = byte.Parse(OutData.Substring(4, 2), NumberStyles.HexNumber);
					b2 = byte.Parse(OutData.Substring(6, 2), NumberStyles.HexNumber);
					b3 = byte.Parse(OutData.Substring(8, 2), NumberStyles.HexNumber);
					break;
				case 3:
					num = 12;
					num2 = ushort.Parse(OutData.Substring(2, 4), NumberStyles.HexNumber);
					b = byte.Parse(OutData.Substring(6, 2), NumberStyles.HexNumber);
					b2 = byte.Parse(OutData.Substring(8, 2), NumberStyles.HexNumber);
					b3 = byte.Parse(OutData.Substring(10, 2), NumberStyles.HexNumber);
					break;
				case 8:
				case 9:
					return true;
				}
				if (num2 == 0)
				{
					return false;
				}
				array[0] = b;
				array[1] = b2;
				array[2] = b3;
				array[3] = 0;
				byte b4 = b2;
				if (SecID == 70)
				{
					if (b >= 1 && b <= 3)
					{
						b4 = 0;
					}
				}
				else if ((int)b4 % 2 == 1)
				{
					b4 = (byte)(b4 - 1);
				}
				uint num4 = (uint)(b * 65536 + b4 * 256);
				if (num4 % 8192u == 0 && (num4 < 65537 || num4 == 131072 || num4 >= 196608) && SecID == 70 && num4 != ErasedSector)
				{
					SectorErase(SecID, num4);
					ErasedSector = num4;
				}
				Application.DoEvents();
				array2 = new byte[num2];
				int num5 = 0;
				for (int i = num; i < num2 * 2 + num; i += 2)
				{
					array2[num5] = byte.Parse(OutData.Substring(i, 2), NumberStyles.HexNumber);
					num5++;
				}
				address = (uint)((array[3] << 24) + (array[0] << 16) + (array[1] << 8) + array[2]);
			}
			while (!WriteFlash(SecID, address, array2));
			return true;
		}
	}
}
