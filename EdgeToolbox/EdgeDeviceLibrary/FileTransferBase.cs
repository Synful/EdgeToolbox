using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using EdgeDeviceLibrary.FusionService;

namespace EdgeDeviceLibrary
{
	public class FileTransferBase : BackgroundWorker
	{
		protected EdgeDeviceLibrary.FusionService.FusionService ws;

		public bool AutoSetChunkSize = true;

		public int ChunkSize = 16384;

		public int MaxRetries = 50;

		protected int NumRetries = 0;

		public int ChunkSizeSampleInterval = 15;

		public bool IncludeHashVerification = true;

		public int PreferredTransferDuration = 800;

		protected long Offset = 0L;

		protected string LocalFileName;

		protected DateTime StartTime;

		protected Thread HashThread;

		public string LocalFileHash;

		public string RemoteFileHash;

		public string LocalFilePath;

		public long MaxRequestLength = 4096L;

		private CookieContainer cookies = new CookieContainer();

		public EdgeDeviceLibrary.FusionService.FusionService WebService
		{
			get
			{
				if (ws == null)
				{
					ws = new EdgeDeviceLibrary.FusionService.FusionService();
					ws.Credentials = CredentialCache.DefaultCredentials;
					ws.CookieContainer = cookies;
				}
				return ws;
			}
		}

		public event EventHandler ChunkSizeChanged;

		public FileTransferBase()
		{
			base.WorkerReportsProgress = true;
			base.WorkerSupportsCancellation = true;
		}

		protected override void Dispose(bool disposing)
		{
			if (HashThread != null && HashThread.IsAlive)
			{
				HashThread.Abort();
			}
			base.Dispose(disposing);
		}

		protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
		{
			if (HashThread != null && HashThread.IsAlive)
			{
				HashThread.Abort();
			}
			base.OnRunWorkerCompleted(e);
		}

		protected override void OnDoWork(DoWorkEventArgs e)
		{
			MaxRequestLength = Math.Max(1L, WebService.GetMaxRequestLength() * 1024 - 2048);
			base.OnDoWork(e);
		}

		public static string CalcFileSize(long numBytes)
		{
			string text = "";
			text = ((numBytes > 1073741824) ? $"{(double)numBytes / 1073741824.0:0.00} Gb" : ((numBytes <= 1048576) ? $"{(double)numBytes / 1024.0:0} Kb" : $"{(double)numBytes / 1048576.0:0.00} Mb"));
			if (text == "0 Kb")
			{
				text = "1 Kb";
			}
			return text;
		}

		public static string CalcFileHash(ref MemoryStream ms)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] value = mD5CryptoServiceProvider.ComputeHash(ms.ToArray());
			return BitConverter.ToString(value);
		}

		protected void CheckLocalFileHash(ref MemoryStream ms)
		{
			LocalFileHash = CalcFileHash(ref ms);
		}

		protected void CalcAndSetChunkSize()
		{
			double totalMilliseconds = DateTime.Now.Subtract(StartTime).TotalMilliseconds;
			double num = (double)ChunkSize / totalMilliseconds;
			double val = num * (double)PreferredTransferDuration;
			ChunkSize = (int)Math.Min(MaxRequestLength, Math.Max(4096.0, val));
			string sender = string.Format($"Chunk size: {CalcFileSize(ChunkSize)} {((ChunkSize == MaxRequestLength) ? " (max)" :  $"{ChunkSize}")}");
			if (this.ChunkSizeChanged != null)
			{
				this.ChunkSizeChanged(sender, EventArgs.Empty);
			}
		}
	}
}
