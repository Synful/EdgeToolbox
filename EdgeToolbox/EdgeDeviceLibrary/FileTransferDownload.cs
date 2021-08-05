using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace EdgeDeviceLibrary {
	public class FileTransferDownload : FileTransferBase {
		public delegate void RunWorkerReportProgress(int percentageComplete, string task);

		public string RemoteFileName;

		public string LocalSaveFolder;

		public void RunWorkerSync(ref MemoryStream ms) {
			RunWorkerSync(ref ms, DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, -1);
		}

		public void RunWorkerSync(ref MemoryStream ms, int BufferSize) {
			RunWorkerSync(ref ms, DeviceConnector.Instance().Email, DeviceConnector.Instance().Password, BufferSize);
		}

		public void RunWorkerSync(ref MemoryStream ms, string email, string password, int BufferSize) {
			RunWorkerSync(base.ReportProgress, ref ms, email, password, BufferSize);
		}

		public void RunWorkerSync(RunWorkerReportProgress reportProgress, ref MemoryStream ms, string email, string password, int BufferSize) {
			base.WebService.CookieContainer = new CookieContainer();
			Offset = 0L;
			int num = 0;
			long fileSize = base.WebService.GetFileSize(RemoteFileName);
			string arg = FileTransferBase.CalcFileSize(fileSize);
			ms.Seek(Offset, SeekOrigin.Begin);
			while (Offset < fileSize && !base.CancellationPending) {
				int bSize = (BufferSize != -1 ? BufferSize : ChunkSize);
				Application.DoEvents();
				if (AutoSetChunkSize) {
					switch (num % ChunkSizeSampleInterval) {
						case 0:
							StartTime = DateTime.Now;
							break;
						case 1:
							CalcAndSetChunkSize();
							break;
					}
				}
				try {
					byte[] array = base.WebService.DownloadChunk(email, password, RemoteFileName, Offset, bSize);
					ms.Write(array, 0, array.Length);
					Offset += array.Length;
				} catch (Exception ex) {
					if (NumRetries++ >= MaxRetries) {
						throw new Exception("Error occurred during download, too many retries.\r\n" + ex.Message);
					}
				}
				string task = $"Transferred {FileTransferBase.CalcFileSize(Offset)} / {arg}";
				int percentageComplete = (int)((decimal)Offset / (decimal)fileSize * 100m);
				reportProgress?.Invoke(percentageComplete, task);
				num++;
			}
			if (IncludeHashVerification && !base.CancellationPending) {
				ReportProgress(99, "Checking file hash...");
				if (ws.Url.ToLower().Contains("localhost")) {
					CheckLocalFileHash(ref ms);
					RemoteFileHash = base.WebService.CheckFileHash(RemoteFileName);
				} else {
					CheckLocalFileHash(ref ms);
					base.WebService.Timeout = 600000;
					RemoteFileHash = base.WebService.CheckFileHash(RemoteFileName);
					base.WebService.Timeout = 30000;
				}
				if (LocalFileHash != RemoteFileHash) {
					throw new Exception("The download of " + RemoteFileName + " failed due to hash mismatch.");
				}
			}
		}
	}
}
