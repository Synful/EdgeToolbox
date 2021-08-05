using System;
using System.Collections.Generic;
using System.Xml;

namespace EdgeDeviceLibrary.Products
{
	public class UploadedFilesXml : XmlDocument
	{
		private string _uploadedFiles = "";

		private ushort _cs_infoVersion = 1;

		private List<ChecksumInfo> _stockFiles = new List<ChecksumInfo>();

		private bool _hasUpdatesRequiredFiles = false;

		private bool _hasManualProcessedFiles = false;

		public string UploadedFiles => _uploadedFiles;

		public ushort cs_infoVersion => _cs_infoVersion;

		public List<ChecksumInfo> StockFiles => _stockFiles;

		public bool HasUpdatesRequiredFiles => _hasUpdatesRequiredFiles;

		public bool HasManualProcessedFiles => _hasManualProcessedFiles;

		public bool TryParseXml(string xml)
		{
			try
			{
				ParseXml(xml);
				return true;
			}
			catch (XmlException)
			{
				return false;
			}
		}

		private void ParseXml(string xml)
		{
			LoadXml(xml);
			XmlNode xmlNode = SelectSingleNode("root/cs_info");
			_cs_infoVersion = Convert.ToUInt16(xmlNode.Attributes.GetNamedItem("version").Value);
			XmlNodeList xmlNodeList = SelectNodes("root/stockFiles/file");
			foreach (XmlNode item in xmlNodeList)
			{
				ChecksumInfo checksumInfo = new ChecksumInfo();
				checksumInfo.StockFileName = item.Attributes.GetNamedItem("deviceFile").Value;
				checksumInfo.StockCSFileName = item.Attributes.GetNamedItem("fusionFile").Value;
				checksumInfo.StockCSChecksum = Convert.ToUInt32(item.Attributes.GetNamedItem("checksum").Value);
				checksumInfo.RequiresDownloadFromWebsite = ((item.Attributes.GetNamedItem("requiresDownload").Value == "True") ? true : false);
				checksumInfo.RequiresUploadToWebsite = ((item.Attributes.GetNamedItem("requiresUpload").Value == "True") ? true : false);
				checksumInfo.ProcessingFlag = (FileProcessFlag)Convert.ToByte(item.Attributes.GetNamedItem("processingFlag").Value);
				_stockFiles.Add(checksumInfo);
				if (checksumInfo.RequiresUploadToWebsite)
				{
					_hasUpdatesRequiredFiles = true;
				}
				if (checksumInfo.ProcessingFlag == FileProcessFlag.UPDATE_REQ_NO_STOCKFILE)
				{
					_hasManualProcessedFiles = true;
				}
			}
			XmlNode xmlNode3 = SelectSingleNode("root/uploadedFilesList");
			_uploadedFiles = xmlNode3.InnerText;
		}
	}
}
