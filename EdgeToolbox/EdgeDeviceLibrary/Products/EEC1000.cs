using System;
using System.IO;
using System.Text;

namespace EdgeDeviceLibrary.Products
{
	internal class EEC1000 : Product_Base
	{
		public override void ValidateStockFiles()
		{
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			MemoryStream ms = new MemoryStream();
			if (!deviceConnector.communicator.ReadFat("vid.bin", bSkipError: true, ShowsProgressBar: false, ref ms))
			{
				return;
			}
			byte[] array = ms.ToArray();
			if (array[0] == byte.MaxValue)
			{
				return;
			}
			ChecksumInfo[] array2 = new ChecksumInfo[5];
			for (int i = 0; i < 5; i++)
			{
				array2[i] = new ChecksumInfo();
			}
			string[] separator = new string[1]
			{
				"\r\n"
			};
			string[] array3 = Encoding.Default.GetString(array).Split(separator, StringSplitOptions.None);
			if (array3.Length >= 5)
			{
				for (int j = 2; j < 7; j++)
				{
					array3[j] = array3[j].Substring(0, array3[j].Length - 1) + ".bin";
					array2[j - 2].StockFileName = array3[j];
				}
			}
			MemoryStream ms2 = new MemoryStream();
			deviceConnector.communicator.ReadFat("stock_cs.bin", bSkipError: true, ShowsProgressBar: false, ref ms2);
			bool flag = false;
			byte[] array4 = ms2.ToArray();
			byte[] array5 = array4;
			for (int k = 0; k < array5.Length; k++)
			{
				if (array5[k] != 0)
				{
					flag = true;
				}
			}
			if (flag)
			{
				for (int l = 0; l * 20 < array4.Length && array4[l * 20] != 0; l++)
				{
					array2[l].StockCSFileName = Encoding.Default.GetString(array4, l * 20, 16);
					array2[l].StockCSFileName = array2[l].StockCSFileName.Substring(0, array2[l].StockCSFileName.IndexOf('\0'));
					array2[l].StockCSChecksum = BitConverter.ToUInt32(array4, l * 20 + 16);
				}
				VerifyChecksumsInStockCS(array2);
			}
		}
	}
}
