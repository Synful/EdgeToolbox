using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace EdgeDeviceLibrary.Products
{
	internal class EEF1200 : Product_Base
	{
		public override void PerformDeviceConnectionTasks(BackgroundWorker bw)
		{
			UploadVehicleUserIdentifierFile("vid.bin");
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
			byte[] bytes = ms.ToArray();
			string text = Encoding.Default.GetString(bytes).Substring(70, 8);
			if (!text.Contains("."))
			{
				text += ".";
			}
			text += "BIN";
			ChecksumInfo[] array = new ChecksumInfo[2]
			{
				new ChecksumInfo(),
				null
			};
			array[0].StockFileName = text;
			text = Encoding.Default.GetString(bytes).Substring(518, 8);
			if (!text.Contains("."))
			{
				text += ".";
			}
			text += "BIN";
			array[1] = new ChecksumInfo();
			array[1].StockFileName = text;
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
				}
			}
			if (flag)
			{
				for (int j = 0; j * 20 < array2.Length && array2[j * 20] != 0; j++)
				{
					array[j].StockCSFileName = Encoding.Default.GetString(array2, j * 20, 16);
					array[j].StockCSFileName = array[j].StockCSFileName.Substring(0, array[j].StockCSFileName.IndexOf('\0'));
					array[j].StockCSChecksum = BitConverter.ToUInt32(array2, j * 20 + 16);
				}
				VerifyChecksumsInStockCS(array);
			}
		}
	}
}
