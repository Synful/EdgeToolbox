namespace EdgeDeviceLibrary.Products
{
	public class ChecksumInfo
	{
		private static ushort _cs_infoVersion = 1;

		private string stockFileName;

		private string stockCSFileName;

		private uint stockCSChecksum;

		private bool requiresDownload;

		private bool requiresUpload;

		private FileProcessFlag _processingFlag = FileProcessFlag.NONE;

		public static ushort cs_infoVersion
		{
			get
			{
				return _cs_infoVersion;
			}
			set
			{
				_cs_infoVersion = value;
			}
		}

		public string StockFileName
		{
			get
			{
				return stockFileName;
			}
			set
			{
				stockFileName = value;
			}
		}

		public string StockCSFileName
		{
			get
			{
				return stockCSFileName;
			}
			set
			{
				stockCSFileName = value;
			}
		}

		public uint StockCSChecksum
		{
			get
			{
				return stockCSChecksum;
			}
			set
			{
				stockCSChecksum = value;
			}
		}

		public bool RequiresDownloadFromWebsite
		{
			get
			{
				return requiresDownload;
			}
			set
			{
				requiresDownload = value;
			}
		}

		public bool RequiresUploadToWebsite
		{
			get
			{
				return requiresUpload;
			}
			set
			{
				requiresUpload = value;
			}
		}

		public FileProcessFlag ProcessingFlag
		{
			get
			{
				return _processingFlag;
			}
			set
			{
				_processingFlag = value;
			}
		}

		public ChecksumInfo()
		{
			stockFileName = "";
			stockCSFileName = "";
			stockCSChecksum = 0u;
			requiresUpload = false;
			requiresDownload = false;
		}
	}
}
