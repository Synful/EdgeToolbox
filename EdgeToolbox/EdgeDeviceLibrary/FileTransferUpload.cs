using System;
using System.IO;

namespace EdgeDeviceLibrary
{
	public class FileTransferUpload : FileTransferBase
	{
		private string _remoteFileName = null;

		public MemoryStream LocalMemoryStream;

		public string RemoteFileName
		{
			get
			{
				return _remoteFileName;
			}
			set
			{
				_remoteFileName = base.WebService.GetStampedFileName(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, value);
			}
		}

		public void RunWorkerSync(ref MemoryStream ms)
		{
			bool flag = false;
			Offset = 0L;
			int num = 0;
			if (AutoSetChunkSize)
			{
				ChunkSize = 16384;
			}
			long length = ms.Length;
			string arg = FileTransferBase.CalcFileSize(length);
			byte[] array = new byte[ChunkSize];
			string text = RemoteFileName;
			ms.Position = Offset;
			int num2;
			do
			{
				num2 = ms.Read(array, 0, ChunkSize);
				if (num2 != array.Length)
				{
					ChunkSize = num2;
					byte[] array2 = new byte[num2];
					Array.Copy(array, array2, num2);
					array = array2;
				}
				if (array.Length == 0 && flag)
				{
					break;
				}
				try
				{
					flag = true;
					if (string.IsNullOrEmpty(text))
					{
						text = base.WebService.GetStampedFileName(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, LocalFileName);
					}
					base.WebService.AppendChunk(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, text, array, Offset);
					if (AutoSetChunkSize)
					{
						switch (num % ChunkSizeSampleInterval)
						{
						case 0:
							StartTime = DateTime.Now;
							break;
						case 1:
							CalcAndSetChunkSize();
							array = new byte[ChunkSize];
							break;
						}
					}
					Offset += num2;
					string userState = $"Transferred {FileTransferBase.CalcFileSize(Offset)} / {arg}";
					int percentProgress = 100;
					if (length > 0)
					{
						percentProgress = (int)((decimal)Offset / (decimal)length * 100m);
					}
					ReportProgress(percentProgress, userState);
				}
				catch (Exception ex)
				{
					ms.Position -= num2;
					if (NumRetries++ >= MaxRetries)
					{
						throw new Exception($"Error occurred during upload, too many retries. \n{ex.ToString()}");
					}
				}
				num++;
			}
			while (num2 > 0 && !base.CancellationPending);
			string remoteFileHash = base.WebService.FinalizeUploadTransfer(DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, text);
			if (IncludeHashVerification)
			{
				ReportProgress(99, "Checking file hash...");
				LocalMemoryStream = ms;
				CheckLocalFileHash(ref ms);
				base.WebService.Timeout = 600000;
				RemoteFileHash = remoteFileHash;
				base.WebService.Timeout = 30000;
				if (LocalFileHash != RemoteFileHash)
				{
					throw new Exception("Hash check failed for upload of file " + text);
				}
			}
		}
	}
}
