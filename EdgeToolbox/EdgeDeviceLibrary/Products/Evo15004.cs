using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace EdgeDeviceLibrary.Products
{
	public class Evo15004 : Product_Base
	{
		public override void PerformDeviceConnectionTasks(BackgroundWorker bw)
		{
			UploadVehicleUserIdentifierFile("vid.bin");
			UploadVehicleUserIdentifierFile("stock_cs.bin");
			UploadVehicleUserIdentifierFile("cs_info.bin");
			ValidateStockFiles();
		}

		public override void ValidateStockFiles()
		{
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			MemoryStream ms = new MemoryStream();
			if (!deviceConnector.communicator.ReadFat("vid.bin", bSkipError: false, ShowsProgressBar: true, ref ms))
			{
				return;
			}
			byte[] data = ms.ToArray();
			string[] array = GetNullTerminatedString(data, 323).Split('-');
			string text = array[0] + array[array.Length - 1];
			if (!text.Contains("."))
			{
				text += ".BIN";
			}
			ChecksumInfo checksumInfo = new ChecksumInfo();
			checksumInfo.StockFileName = text;
			MemoryStream ms2 = new MemoryStream();
			deviceConnector.communicator.ReadFat("stock_cs.bin", bSkipError: true, ShowsProgressBar: false, ref ms2);
			bool flag = false;
			byte[] array2 = ms2.ToArray();
			byte[] array3 = array2;
			for (int i = 0; i < array3.Length; i++)
			{
				if (array3[i] != 0)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				checksumInfo.StockCSFileName = GetNullTerminatedString(array2, 0);
				checksumInfo.StockCSChecksum = BitConverter.ToUInt32(array2, 16);
			}
			VerifyChecksumsInStockCS(new ChecksumInfo[1]
			{
				checksumInfo
			});
		}

		private string GetNullTerminatedString(byte[] data, int pos)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (pos < data.Length && data[pos] != 0)
			{
				stringBuilder.Append((char)data[pos]);
				pos++;
			}
			return stringBuilder.ToString();
		}
	}
}
