using System.ComponentModel;
using System.IO;
using EdgeDeviceLibrary.Communicator;
using MXZipLibrary;

namespace EdgeDeviceLibrary.Products
{
	internal class ProteusProduct : Generic
	{
		public override void PerformDeviceConnectionTasks(BackgroundWorker bw)
		{
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			MemoryStream ms = new MemoryStream();
			byte[] array = null;
			((Proteus)deviceConnector.communicator).ChangeDirectory("\\");
			byte[] array2 = ((!deviceConnector.communicator.ReadFat("vid.bin", bSkipError: false, ShowsProgressBar: false, ref ms)) ? null : ms.ToArray());
			ms.Dispose();
			ms = new MemoryStream();
			byte[] array3 = ((!deviceConnector.communicator.ReadFat("cs_info.bin", bSkipError: false, ShowsProgressBar: false, ref ms)) ? null : ms.ToArray());
			ms.Dispose();
			((Proteus)deviceConnector.communicator).LoadAndRunMiniApp("MiniApp_FileAccess.elf.bin");
			if (array2 != null || array3 != null || array != null)
			{
				MemoryStream ms2 = new MemoryStream();
				ZipOutputStream zipOutputStream = new ZipOutputStream(ms2);
				try
				{
					if (array2 != null)
					{
						WriteZipEntry(zipOutputStream, "vid.bin", array2);
					}
					if (array3 != null)
					{
						WriteZipEntry(zipOutputStream, "cs_info.bin", array3);
					}
					if (array != null)
					{
						WriteZipEntry(zipOutputStream, "CriticalLog.bin", array);
					}
					zipOutputStream.Flush();
					zipOutputStream.Close();
					zipOutputStream = null;
					ms2 = new MemoryStream(ms2.ToArray());
					string RemoteRelativePathAndName = "_uploads\\devices\\" + deviceConnector.communicator.GetDevicesTableID() + "\\StatusFiles.zip";
					deviceConnector.UploadStockFile(ref ms2, ref RemoteRelativePathAndName);
				}
				finally
				{
					zipOutputStream?.Close();
					ms2?.Dispose();
				}
			}
			if (array3 != null)
			{
				((Proteus)deviceConnector.communicator).ChangeToActiveStockFolder();
				ValidateStockFiles();
			}
		}

		private static void WriteZipEntry(ZipOutputStream zipStream, string fileName, byte[] fileData)
		{
			ZipEntry entry = new ZipEntry(fileName);
			zipStream.PutNextEntry(entry);
			zipStream.Write(fileData, 0, fileData.Length);
		}
	}
}
