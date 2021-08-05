using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace EdgeDeviceLibrary.Products
{
	internal class Generic : Product_Base
	{
		private const int CS_INFO_MAX_VERSION = 2;

		public override void PerformDeviceConnectionTasks(BackgroundWorker bw)
		{
			UploadVehicleUserIdentifierFile("vid.bin");
			UploadVehicleUserIdentifierFile("cs_info.bin");
			if (DeviceConnector.Instance().communicator.FATGetFileAddressOrLength("cs_info.bin", IsAddress: true) != 0)
			{
				ValidateStockFiles();
			}
		}

		public override void ValidateStockFiles()
		{
			byte[] array = new byte[18];
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			MemoryStream ms = new MemoryStream();
			if (!deviceConnector.communicator.ReadFat("cs_info.bin", bSkipError: true, ShowsProgressBar: false, ref ms))
			{
				return;
			}
			ms.Read(array, 0, 16);
			if (array[0] == byte.MaxValue || !Encoding.ASCII.GetString(array).Contains("cs_info.bin"))
			{
				return;
			}
			ms.Read(array, 0, 2);
			Array.Reverse(array, 0, 2);
			ChecksumInfo.cs_infoVersion = BitConverter.ToUInt16(array, 0);
			try
			{
				if (ChecksumInfo.cs_infoVersion > 2)
				{
					throw new Exception("cs_info.bin version newer than this program supports.");
				}
			}
			catch (Exception ex)
			{
				deviceConnector.Log("Exception: " + ex.Message + "  Stack: " + ex.StackTrace);
				return;
			}
			ms.Read(array, 0, 2);
			Array.Reverse(array, 0, 2);
			ushort num = BitConverter.ToUInt16(array, 0);
			ms.Read(array, 0, 18);
			ChecksumInfo[] array2 = new ChecksumInfo[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = new ChecksumInfo();
				array2[i].StockFileName = GetStringEndingAtNull(ms);
				array2[i].StockCSFileName = GetStringEndingAtNull(ms);
				try
				{
					if (ms.Position >= ms.Length)
					{
						throw new Exception("Unable to read vehicle stock file and checksum information.");
					}
				}
				catch (Exception ex2)
				{
					deviceConnector.Log("Exception (E5): " + ex2.Message + "  Stack: " + ex2.StackTrace);
					return;
				}
				ms.Read(array, 0, 4);
				array2[i].StockCSChecksum = BitConverter.ToUInt32(array, 0);
				if (ChecksumInfo.cs_infoVersion == 2)
				{
					ms.Read(array, 0, 1);
					array2[i].ProcessingFlag = (FileProcessFlag)array[0];
				}
			}
			VerifyChecksumsInStockCS(array2);
		}

		private string GetStringEndingAtNull(MemoryStream msCS_INFO)
		{
			long position = msCS_INFO.Position;
			int num = 0;
			while (msCS_INFO.Position < msCS_INFO.Length && msCS_INFO.ReadByte() != 0)
			{
				num++;
			}
			num++;
			msCS_INFO.Position = position;
			byte[] array = new byte[num];
			msCS_INFO.Read(array, 0, num);
			return Encoding.Default.GetString(array, 0, num - 1);
		}
	}
}
