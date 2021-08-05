using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace EdgeDeviceLibrary.Products
{
	internal class EEC2200 : Product_Base
	{
		public override void PerformDeviceConnectionTasks(BackgroundWorker bw)
		{
			ValidateStockFiles();
		}

		public override void ValidateStockFiles()
		{
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			MemoryStream ms = new MemoryStream();
			if (!deviceConnector.communicator.ReadFat("vid.bin", bSkipError: true, ShowsProgressBar: false, ref ms))
			{
				return;
			}
			File.WriteAllBytes("data\\vid.bin", ms.ToArray());
			string[] array = File.ReadAllLines("data\\vid.bin");
			File.Delete("data\\vid.bin");
			if (array.Length < 12)
			{
				return;
			}
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < 11; i++)
			{
				if (i == 5)
				{
					i = 8;
				}
				array[i + 1] = array[i + 1].Substring(0, array[i + 1].Length - 1);
				arrayList.Add(array[i + 1] + ".bin");
			}
			ChecksumInfo[] array2 = new ChecksumInfo[arrayList.Count];
			for (int j = 0; j < arrayList.Count; j++)
			{
				array2[j] = new ChecksumInfo();
				array2[j].StockFileName = (string)arrayList[j];
			}
			MemoryStream ms2 = new MemoryStream();
			deviceConnector.communicator.ReadFat("stock_cs.bin", bSkipError: true, ShowsProgressBar: false, ref ms2);
			bool flag = false;
			byte[] array3 = ms2.ToArray();
			byte[] array4 = array3;
			for (int k = 0; k < array4.Length; k++)
			{
				if (array4[k] != 0)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			for (int l = 0; l * 20 < array3.Length; l++)
			{
				if (array3[l * 20] == 0)
				{
					for (int m = l; m < array2.Length; m++)
					{
						array2[m].StockCSFileName = "DELETED";
					}
					break;
				}
				array2[l].StockCSFileName = Encoding.Default.GetString(array3, l * 20, 16);
				array2[l].StockCSFileName = array2[l].StockCSFileName.Substring(0, array2[l].StockCSFileName.IndexOf('\0'));
				array2[l].StockCSChecksum = BitConverter.ToUInt32(array3, l * 20 + 16);
			}
			VerifyChecksumsInStockCS(array2);
		}
	}
}
