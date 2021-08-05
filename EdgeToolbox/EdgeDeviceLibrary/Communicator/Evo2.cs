using System;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Text;
using System.Windows.Forms;
using EdgeDeviceLibrary.FusionService;

namespace EdgeDeviceLibrary.Communicator
{
	internal class Evo2 : Communicator_Base
	{
		private static Evo2 evo2;

		public static Evo2 Instance(SerialPort Port)
		{
			if (evo2 == null)
			{
				evo2 = new Evo2(Port);
			}
			return evo2;
		}

		private Evo2(SerialPort Port)
		{
			port = Port;
			base.IsFileFormatEncryptedS19 = true;
		}

		public override uint GetSerialNumber()
		{
			byte[] bytes = base.ReadFlash(MemoryLocation.EEPROM, 3040u, 32);
			string @string = Encoding.Default.GetString(bytes);
			return uint.Parse(@string.Substring(16, 8));
		}

		public override string GetPartNumber(bool cachedOk)
		{
			string str = "UNIDENTIFIED";
			byte[] bytes = ReadFlash(MemoryLocation.EEPROM, 3040u, 32);
			string @string = Encoding.ASCII.GetString(bytes);
			if (@string.Substring(0, 7) == "EEF2100")
			{
				str = @string.Substring(0, 7);
			}
			else if (@string.Substring(0, 7) == "EEF2400")
			{
				str = @string.Substring(0, 7);
			}
			str += "-EVO2";
			try
			{
				nProductIdentifier = new EdgeDeviceLibrary.FusionService.FusionService().GetProductIDFromModelString(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, str);
			}
			catch (WebException)
			{
				str += "_NO_WEB";
			}
			return str;
		}

		protected override byte[] GetBootLoaderVersion()
		{
			byte[] buffer = new byte[1]
			{
				86
			};
			port.Write(buffer, 0, 1);
			return base.ReadByteArrayFromPort(5, 1000, VerifyChecksum: true);
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
			ReadFat("version.txt", bSkipError: false, ShowsProgressBar: false, ref ms);
		}

		public override string GetFirmwareVersionString()
		{
			short[] firmwareVersion = GetFirmwareVersion();
			return firmwareVersion[0] + "." + firmwareVersion[1] + "." + firmwareVersion[2];
		}

		public override short[] GetFirmwareVersion()
		{
			MemoryStream memoryStream = new MemoryStream();
			short[] array = new short[3];
			try
			{
				memoryStream.Seek(4L, SeekOrigin.Begin);
				array[0] = (short)memoryStream.ReadByte();
				array[1] = (short)memoryStream.ReadByte();
				byte[] array2 = new byte[2];
				memoryStream.Read(array2, 0, 2);
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
				ReadFat("version.bin", bSkipError: false, ShowsProgressBar: false, ref ms);
				ms.Seek(8L, SeekOrigin.Begin);
				array[0] = (short)ms.ReadByte();
				array[1] = (short)ms.ReadByte();
				byte[] array2 = new byte[2];
				ms.Read(array2, 0, 2);
				array[2] = BitConverter.ToInt16(array2, 0);
				ms.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			return array;
		}

		public override string GetCalibrationVersionString()
		{
			short[] calibrationVersion = GetCalibrationVersion();
			return calibrationVersion[0] + "." + calibrationVersion[1] + "." + calibrationVersion[2];
		}

		public override byte[] ReadSettings()
		{
			port.ReadExisting();
			byte[] buffer = new byte[1]
			{
				83
			};
			port.Write(buffer, 0, 1);
			byte[] array = ReadByteArrayFromPort(0, 500, VerifyChecksum: true);
			int num = BitConverter.ToUInt16(array, 1);
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
			return false;
		}

		public override int GetProgrammedToLevel()
		{
			return -1;
		}

		private bool SetFlashSecurity(bool LockFlash, bool VerifyOnly)
		{
			byte[] array = new byte[7]
			{
				82,
				69,
				0,
				1,
				1,
				2,
				0
			};
			port.ReadExisting();
			port.Write(array, 0, array.Length);
			byte[] array2 = ReadByteArrayFromPort(5, 1000, VerifyChecksum: true);
			if (array2[1] != 254)
			{
				return true;
			}
			if (VerifyOnly)
			{
				return true;
			}
			byte[] array3 = new byte[7]
			{
				82,
				70,
				63,
				190,
				0,
				2,
				0
			};
			port.ReadExisting();
			port.Write(array3, 0, array3.Length);
			array2 = ReadByteArrayFromPort(5, 1000, VerifyChecksum: true);
			byte[] array4 = new byte[array2.Length - 3];
			Array.Copy(array2, 1, array4, 0, array2.Length - 3);
			uint num = BitConverter.ToUInt32(array4, 266);
			byte b = array4[271];
			if (num == uint.MaxValue && b == 254)
			{
			}
			num = uint.MaxValue;
			b = (array4[271] = (byte)((!LockFlash) ? 254 : byte.MaxValue));
			Array.Copy(BitConverter.GetBytes(num), 0, array4, 266, 4);
			SectorErase(70, 4177408u);
			try
			{
				WriteFlash(70, 4177408u, array4);
			}
			catch
			{
				byte[] array5 = new byte[7]
				{
					82,
					70,
					63,
					190,
					0,
					2,
					0
				};
				port.ReadExisting();
				port.Write(array5, 0, array5.Length);
				array2 = ReadByteArrayFromPort(5, 1000, VerifyChecksum: true);
			}
			return true;
		}

		public override bool SendEncryptedS19File(ref MemoryStream ms)
		{
			SetFlashSecurity(LockFlash: false, VerifyOnly: true);
			byte[] bytes = DeviceConnector.Instance().Decrypt(ms.ToArray());
			Send_S19_File(Encoding.Default.GetString(bytes));
			SetFlashSecurity(LockFlash: false, VerifyOnly: false);
			return true;
		}

		public override void ClearInternalFlash(uint address, uint endaddress)
		{
			throw new Exception("The method or operation ClearInternalFlash is not implemented.");
		}

		public override bool Send_S19_File(string SendData)
		{
			int startIndex = 0;
			StringBuilder stringBuilder = new StringBuilder();
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			while (startIndex < SendData.Length)
			{
				int percentageComplete = (int)((float)startIndex / (float)SendData.Length * 100f);
				deviceConnector.ReportProgress("Transferring to device...", percentageComplete + "% Complete", percentageComplete);
				string nextLine = GetNextLine(SendData, ref startIndex);
				if (nextLine.Substring(0, 2) == "S8")
				{
					break;
				}
				Send_S19_Line(nextLine);
			}
			return true;
		}

		protected override bool Send_S19_Line(string OutData)
		{
			byte b = 0;
			byte b2 = 0;
			byte b3 = 0;
			ulong num = 0uL;
			ushort num2 = 0;
			ushort num3 = 0;
			byte[] array = new byte[4];
			byte[] array2 = new byte[0];
			Application.DoEvents();
			ushort num4 = ushort.Parse(OutData.Substring(1, 1));
			string text = OutData.Substring(2, 2);
			uint address;
			do
			{
				byte b4;
				switch (num4)
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
					num2 = 8;
					num3 = (ushort)(ushort.Parse(text, NumberStyles.HexNumber) - 3);
					b = byte.Parse(OutData.Substring(4, 2), NumberStyles.HexNumber);
					b2 = byte.Parse(OutData.Substring(6, 2), NumberStyles.HexNumber);
					b4 = b;
					num = (ulong)(b4 * 256) + (ulong)b2;
					if (num >= 16384 && num <= 32767)
					{
						num += 4079616;
					}
					if (num >= 49152 && num <= 65535)
					{
						num += 4112384;
					}
					b = (byte)((num & 0xFF0000) >> 16);
					b2 = (byte)((num & 0xFF00) >> 8);
					b3 = (byte)(num & 0xFF);
					break;
				case 2:
					num2 = 10;
					num3 = (ushort)(ushort.Parse(text, NumberStyles.HexNumber) - 4);
					b = byte.Parse(OutData.Substring(4, 2), NumberStyles.HexNumber);
					b2 = byte.Parse(OutData.Substring(6, 2), NumberStyles.HexNumber);
					b3 = byte.Parse(OutData.Substring(8, 2), NumberStyles.HexNumber);
					break;
				case 3:
					num2 = 12;
					num3 = ushort.Parse(OutData.Substring(2, 4), NumberStyles.HexNumber);
					b = byte.Parse(OutData.Substring(6, 2), NumberStyles.HexNumber);
					b2 = byte.Parse(OutData.Substring(8, 2), NumberStyles.HexNumber);
					b3 = byte.Parse(OutData.Substring(10, 2), NumberStyles.HexNumber);
					break;
				case 8:
				case 9:
					return true;
				}
				if (num3 == 0)
				{
					return false;
				}
				array[0] = b;
				array[1] = b2;
				array[2] = b3;
				array[3] = 0;
				b4 = b2;
				if ((int)b4 % 2 == 1)
				{
					b4 = (byte)(b4 - 1);
				}
				uint num5 = (uint)(b * 65536 + b4 * 256);
				if (SecID == 70 && num5 != ErasedSector)
				{
					SectorErase(SecID, num5);
					ErasedSector = num5;
				}
				Application.DoEvents();
				array2 = new byte[num3];
				int num6 = 0;
				for (int i = num2; i < num3 * 2 + num2; i += 2)
				{
					array2[num6] = byte.Parse(OutData.Substring(i, 2), NumberStyles.HexNumber);
					num6++;
				}
				if (num == 4177888)
				{
					array2[num3 - 2] = 192;
					array2[num3 - 1] = 1;
				}
				address = (uint)((array[3] << 24) + (array[0] << 16) + (array[1] << 8) + array[2]);
			}
			while (!WriteFlash(SecID, address, array2));
			return true;
		}
	}
}
