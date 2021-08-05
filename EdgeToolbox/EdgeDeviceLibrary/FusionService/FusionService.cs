using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace EdgeDeviceLibrary.FusionService
{
	[GeneratedCode("System.Web.Services", "4.7.2556.0")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[WebServiceBinding(Name = "FusionServiceSoap", Namespace = "http://edgeproducts.inc/")]
	public class FusionService : SoapHttpClientProtocol
	{
		private SendOrPostCallback GetStampedFileNameOperationCompleted;

		private SendOrPostCallback AppendChunkOperationCompleted;

		private SendOrPostCallback FinalizeUploadTransferOperationCompleted;

		private SendOrPostCallback DownloadChunkOperationCompleted;

		private SendOrPostCallback GetFileSizeOperationCompleted;

		private SendOrPostCallback CheckFileHashOperationCompleted;

		private SendOrPostCallback Notify_ReceivedNewStockFilesOperationCompleted;

		private SendOrPostCallback GetModelStringOperationCompleted;

		private SendOrPostCallback CreateCustomFirmwareZipOperationCompleted;

		private SendOrPostCallback CreateCustomCalibraionZipOperationCompleted;

		private SendOrPostCallback NewCreateCustomCalibraionZipOperationCompleted;

		private SendOrPostCallback GetModelLevelOffsetInSettingsOperationCompleted;

		private SendOrPostCallback GetProductIDFromModelStringOperationCompleted;

		private SendOrPostCallback GetModelFromUserSerialNumOperationCompleted;

		private SendOrPostCallback GetCanUpgradeSKUFromUserSerialNumOperationCompleted;

		private SendOrPostCallback GetSKUChangedFromUserSerialNumOperationCompleted;

		private SendOrPostCallback ClearSKUChangedFromUserSerialNumOperationCompleted;

		private SendOrPostCallback CreatingUpdateUserForRacingOperationCompleted;

		private SendOrPostCallback CreatingUpdateUserForTPMSControlOperationCompleted;

		private SendOrPostCallback CreatingUserForHPTunersOperationCompleted;

		private SendOrPostCallback CreatingUpdateUserForHotUnlockOperationCompleted;

		private SendOrPostCallback UpdateUserForHotUnlockOperationCompleted;

		private SendOrPostCallback RetrieveUserRacingCodeOperationCompleted;

		private SendOrPostCallback RetrieveUserHotUnlockCodeOperationCompleted;

		private SendOrPostCallback GetBootloaderInfoOperationCompleted;

		private SendOrPostCallback GetFwCalDescriptionOperationCompleted;

		private SendOrPostCallback GetFwCalChecksumOperationCompleted;

		private SendOrPostCallback GetAvailableFirmwareOperationCompleted;

		private SendOrPostCallback IsUpdateEnabledForDeviceOperationCompleted;

		private SendOrPostCallback GetAvailableCalibrationsOperationCompleted;

		private SendOrPostCallback SetUpdateCompleteOperationCompleted;

		private SendOrPostCallback SetPlatformTypeOperationCompleted;

		private SendOrPostCallback LogOperationCompleted;

		private SendOrPostCallback GetTargetFirmCalIDsOperationCompleted;

		private SendOrPostCallback GetCurrentFwCalOperationCompleted;

		private SendOrPostCallback IsGetAllFilesSetOperationCompleted;

		private SendOrPostCallback IsGetAllCriticalFilesSetOperationCompleted;

		private SendOrPostCallback GetCriticalFilesListOperationCompleted;

		private SendOrPostCallback GetSpecificFilesListOperationCompleted;

		private SendOrPostCallback ClearGetSpecificFilesOperationCompleted;

		private SendOrPostCallback IsMakeStockWriterSetOperationCompleted;

		private SendOrPostCallback IsSetStockFlagSetOperationCompleted;

		private SendOrPostCallback IsEraseSettingsSetOperationCompleted;

		private SendOrPostCallback IsResetToFactorySetOperationCompleted;

		private SendOrPostCallback DeviceCanBeUpdatedOperationCompleted;

		private SendOrPostCallback IsForceUpdateSetOperationCompleted;

		private SendOrPostCallback ClearEraseSettingsOperationCompleted;

		private SendOrPostCallback ClearResetToFactoryOperationCompleted;

		private SendOrPostCallback ClearMakeStockWriterOperationCompleted;

		private SendOrPostCallback ClearSetStockOperationCompleted;

		private SendOrPostCallback ClearGetAllFilesOperationCompleted;

		private SendOrPostCallback ClearGetAllCriticalFilesOperationCompleted;

		private SendOrPostCallback GetDeviceIDOperationCompleted;

		private SendOrPostCallback GetDeviceIDForSerialOperationCompleted;

		private SendOrPostCallback GetUpdateFirmwareAndCalibrationIDsOperationCompleted;

		private SendOrPostCallback GetListOfFilesToSendToDeviceOperationCompleted;

		private SendOrPostCallback GetListOfFilesToSendToDevice2OperationCompleted;

		private SendOrPostCallback SetFileAsSentOperationCompleted;

		private SendOrPostCallback SetDeviceHasCustomTuningOperationCompleted;

		private SendOrPostCallback GetMaxRequestLengthOperationCompleted;

		private bool useDefaultCredentialsSetExplicitly;

		public new string Url
		{
			get
			{
				return base.Url;
			}
			set
			{
				if (IsLocalFileSystemWebService(base.Url) && !useDefaultCredentialsSetExplicitly && !IsLocalFileSystemWebService(value))
				{
					base.UseDefaultCredentials = false;
				}
				base.Url = value;
			}
		}

		public new bool UseDefaultCredentials
		{
			get
			{
				return base.UseDefaultCredentials;
			}
			set
			{
				base.UseDefaultCredentials = value;
				useDefaultCredentialsSetExplicitly = true;
			}
		}

		public event GetStampedFileNameCompletedEventHandler GetStampedFileNameCompleted;

		public event AppendChunkCompletedEventHandler AppendChunkCompleted;

		public event FinalizeUploadTransferCompletedEventHandler FinalizeUploadTransferCompleted;

		public event DownloadChunkCompletedEventHandler DownloadChunkCompleted;

		public event GetFileSizeCompletedEventHandler GetFileSizeCompleted;

		public event CheckFileHashCompletedEventHandler CheckFileHashCompleted;

		public event Notify_ReceivedNewStockFilesCompletedEventHandler Notify_ReceivedNewStockFilesCompleted;

		public event GetModelStringCompletedEventHandler GetModelStringCompleted;

		public event CreateCustomFirmwareZipCompletedEventHandler CreateCustomFirmwareZipCompleted;

		public event CreateCustomCalibraionZipCompletedEventHandler CreateCustomCalibraionZipCompleted;

		public event NewCreateCustomCalibraionZipCompletedEventHandler NewCreateCustomCalibraionZipCompleted;

		public event GetModelLevelOffsetInSettingsCompletedEventHandler GetModelLevelOffsetInSettingsCompleted;

		public event GetProductIDFromModelStringCompletedEventHandler GetProductIDFromModelStringCompleted;

		public event GetModelFromUserSerialNumCompletedEventHandler GetModelFromUserSerialNumCompleted;

		public event GetCanUpgradeSKUFromUserSerialNumCompletedEventHandler GetCanUpgradeSKUFromUserSerialNumCompleted;

		public event GetSKUChangedFromUserSerialNumCompletedEventHandler GetSKUChangedFromUserSerialNumCompleted;

		public event ClearSKUChangedFromUserSerialNumCompletedEventHandler ClearSKUChangedFromUserSerialNumCompleted;

		public event CreatingUpdateUserForRacingCompletedEventHandler CreatingUpdateUserForRacingCompleted;

		public event CreatingUpdateUserForTPMSControlCompletedEventHandler CreatingUpdateUserForTPMSControlCompleted;

		public event CreatingUserForHPTunersCompletedEventHandler CreatingUserForHPTunersCompleted;

		public event CreatingUpdateUserForHotUnlockCompletedEventHandler CreatingUpdateUserForHotUnlockCompleted;

		public event UpdateUserForHotUnlockCompletedEventHandler UpdateUserForHotUnlockCompleted;

		public event RetrieveUserRacingCodeCompletedEventHandler RetrieveUserRacingCodeCompleted;

		public event RetrieveUserHotUnlockCodeCompletedEventHandler RetrieveUserHotUnlockCodeCompleted;

		public event GetBootloaderInfoCompletedEventHandler GetBootloaderInfoCompleted;

		public event GetFwCalDescriptionCompletedEventHandler GetFwCalDescriptionCompleted;

		public event GetFwCalChecksumCompletedEventHandler GetFwCalChecksumCompleted;

		public event GetAvailableFirmwareCompletedEventHandler GetAvailableFirmwareCompleted;

		public event IsUpdateEnabledForDeviceCompletedEventHandler IsUpdateEnabledForDeviceCompleted;

		public event GetAvailableCalibrationsCompletedEventHandler GetAvailableCalibrationsCompleted;

		public event SetUpdateCompleteCompletedEventHandler SetUpdateCompleteCompleted;

		public event SetPlatformTypeCompletedEventHandler SetPlatformTypeCompleted;

		public event LogCompletedEventHandler LogCompleted;

		public event GetTargetFirmCalIDsCompletedEventHandler GetTargetFirmCalIDsCompleted;

		public event GetCurrentFwCalCompletedEventHandler GetCurrentFwCalCompleted;

		public event IsGetAllFilesSetCompletedEventHandler IsGetAllFilesSetCompleted;

		public event IsGetAllCriticalFilesSetCompletedEventHandler IsGetAllCriticalFilesSetCompleted;

		public event GetCriticalFilesListCompletedEventHandler GetCriticalFilesListCompleted;

		public event GetSpecificFilesListCompletedEventHandler GetSpecificFilesListCompleted;

		public event ClearGetSpecificFilesCompletedEventHandler ClearGetSpecificFilesCompleted;

		public event IsMakeStockWriterSetCompletedEventHandler IsMakeStockWriterSetCompleted;

		public event IsSetStockFlagSetCompletedEventHandler IsSetStockFlagSetCompleted;

		public event IsEraseSettingsSetCompletedEventHandler IsEraseSettingsSetCompleted;

		public event IsResetToFactorySetCompletedEventHandler IsResetToFactorySetCompleted;

		public event DeviceCanBeUpdatedCompletedEventHandler DeviceCanBeUpdatedCompleted;

		public event IsForceUpdateSetCompletedEventHandler IsForceUpdateSetCompleted;

		public event ClearEraseSettingsCompletedEventHandler ClearEraseSettingsCompleted;

		public event ClearResetToFactoryCompletedEventHandler ClearResetToFactoryCompleted;

		public event ClearMakeStockWriterCompletedEventHandler ClearMakeStockWriterCompleted;

		public event ClearSetStockCompletedEventHandler ClearSetStockCompleted;

		public event ClearGetAllFilesCompletedEventHandler ClearGetAllFilesCompleted;

		public event ClearGetAllCriticalFilesCompletedEventHandler ClearGetAllCriticalFilesCompleted;

		public event GetDeviceIDCompletedEventHandler GetDeviceIDCompleted;

		public event GetDeviceIDForSerialCompletedEventHandler GetDeviceIDForSerialCompleted;

		public event GetUpdateFirmwareAndCalibrationIDsCompletedEventHandler GetUpdateFirmwareAndCalibrationIDsCompleted;

		public event GetListOfFilesToSendToDeviceCompletedEventHandler GetListOfFilesToSendToDeviceCompleted;

		public event GetListOfFilesToSendToDevice2CompletedEventHandler GetListOfFilesToSendToDevice2Completed;

		public event SetFileAsSentCompletedEventHandler SetFileAsSentCompleted;

		public event SetDeviceHasCustomTuningCompletedEventHandler SetDeviceHasCustomTuningCompleted;

		public event GetMaxRequestLengthCompletedEventHandler GetMaxRequestLengthCompleted;

		public FusionService()
		{
			Url = "https://fusion.edgeproducts.com/FusionService.asmx";
			if (IsLocalFileSystemWebService(Url))
			{
				UseDefaultCredentials = true;
				useDefaultCredentialsSetExplicitly = false;
			}
			else
			{
				useDefaultCredentialsSetExplicitly = true;
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetStampedFileName", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string GetStampedFileName(string email, string password, string fileName)
		{
			object[] array = Invoke("GetStampedFileName", new object[3]
			{
				email,
				password,
				fileName
			});
			return (string)array[0];
		}

		public void GetStampedFileNameAsync(string email, string password, string fileName)
		{
			GetStampedFileNameAsync(email, password, fileName, null);
		}

		public void GetStampedFileNameAsync(string email, string password, string fileName, object userState)
		{
			if (GetStampedFileNameOperationCompleted == null)
			{
				GetStampedFileNameOperationCompleted = OnGetStampedFileNameOperationCompleted;
			}
			InvokeAsync("GetStampedFileName", new object[3]
			{
				email,
				password,
				fileName
			}, GetStampedFileNameOperationCompleted, userState);
		}

		private void OnGetStampedFileNameOperationCompleted(object arg)
		{
			if (this.GetStampedFileNameCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetStampedFileNameCompleted(this, new GetStampedFileNameCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/AppendChunk", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void AppendChunk(string Email, string Password, string FileName, [XmlElement(DataType = "base64Binary")] byte[] buffer, long Offset)
		{
			Invoke("AppendChunk", new object[5]
			{
				Email,
				Password,
				FileName,
				buffer,
				Offset
			});
		}

		public void AppendChunkAsync(string Email, string Password, string FileName, byte[] buffer, long Offset)
		{
			AppendChunkAsync(Email, Password, FileName, buffer, Offset, null);
		}

		public void AppendChunkAsync(string Email, string Password, string FileName, byte[] buffer, long Offset, object userState)
		{
			if (AppendChunkOperationCompleted == null)
			{
				AppendChunkOperationCompleted = OnAppendChunkOperationCompleted;
			}
			InvokeAsync("AppendChunk", new object[5]
			{
				Email,
				Password,
				FileName,
				buffer,
				Offset
			}, AppendChunkOperationCompleted, userState);
		}

		private void OnAppendChunkOperationCompleted(object arg)
		{
			if (this.AppendChunkCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AppendChunkCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/FinalizeUploadTransfer", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string FinalizeUploadTransfer(string email, string password, string fileName)
		{
			object[] array = Invoke("FinalizeUploadTransfer", new object[3]
			{
				email,
				password,
				fileName
			});
			return (string)array[0];
		}

		public void FinalizeUploadTransferAsync(string email, string password, string fileName)
		{
			FinalizeUploadTransferAsync(email, password, fileName, null);
		}

		public void FinalizeUploadTransferAsync(string email, string password, string fileName, object userState)
		{
			if (FinalizeUploadTransferOperationCompleted == null)
			{
				FinalizeUploadTransferOperationCompleted = OnFinalizeUploadTransferOperationCompleted;
			}
			InvokeAsync("FinalizeUploadTransfer", new object[3]
			{
				email,
				password,
				fileName
			}, FinalizeUploadTransferOperationCompleted, userState);
		}

		private void OnFinalizeUploadTransferOperationCompleted(object arg)
		{
			if (this.FinalizeUploadTransferCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.FinalizeUploadTransferCompleted(this, new FinalizeUploadTransferCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/DownloadChunk", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement(DataType = "base64Binary")]
		public byte[] DownloadChunk(string Email, string Password, string FileName, long Offset, int BufferSize)
		{
			object[] array = Invoke("DownloadChunk", new object[5]
			{
				Email,
				Password,
				FileName,
				Offset,
				BufferSize
			});
			return (byte[])array[0];
		}

		public void DownloadChunkAsync(string Email, string Password, string FileName, long Offset, int BufferSize)
		{
			DownloadChunkAsync(Email, Password, FileName, Offset, BufferSize, null);
		}

		public void DownloadChunkAsync(string Email, string Password, string FileName, long Offset, int BufferSize, object userState)
		{
			if (DownloadChunkOperationCompleted == null)
			{
				DownloadChunkOperationCompleted = OnDownloadChunkOperationCompleted;
			}
			InvokeAsync("DownloadChunk", new object[5]
			{
				Email,
				Password,
				FileName,
				Offset,
				BufferSize
			}, DownloadChunkOperationCompleted, userState);
		}

		private void OnDownloadChunkOperationCompleted(object arg)
		{
			if (this.DownloadChunkCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DownloadChunkCompleted(this, new DownloadChunkCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetFileSize", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public long GetFileSize(string FileName)
		{
			object[] array = Invoke("GetFileSize", new object[1]
			{
				FileName
			});
			return (long)array[0];
		}

		public void GetFileSizeAsync(string FileName)
		{
			GetFileSizeAsync(FileName, null);
		}

		public void GetFileSizeAsync(string FileName, object userState)
		{
			if (GetFileSizeOperationCompleted == null)
			{
				GetFileSizeOperationCompleted = OnGetFileSizeOperationCompleted;
			}
			InvokeAsync("GetFileSize", new object[1]
			{
				FileName
			}, GetFileSizeOperationCompleted, userState);
		}

		private void OnGetFileSizeOperationCompleted(object arg)
		{
			if (this.GetFileSizeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetFileSizeCompleted(this, new GetFileSizeCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/CheckFileHash", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string CheckFileHash(string FileName)
		{
			object[] array = Invoke("CheckFileHash", new object[1]
			{
				FileName
			});
			return (string)array[0];
		}

		public void CheckFileHashAsync(string FileName)
		{
			CheckFileHashAsync(FileName, null);
		}

		public void CheckFileHashAsync(string FileName, object userState)
		{
			if (CheckFileHashOperationCompleted == null)
			{
				CheckFileHashOperationCompleted = OnCheckFileHashOperationCompleted;
			}
			InvokeAsync("CheckFileHash", new object[1]
			{
				FileName
			}, CheckFileHashOperationCompleted, userState);
		}

		private void OnCheckFileHashOperationCompleted(object arg)
		{
			if (this.CheckFileHashCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CheckFileHashCompleted(this, new CheckFileHashCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/Notify_ReceivedNewStockFiles", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void Notify_ReceivedNewStockFiles(long deviceID, string uploadedFiles)
		{
			Invoke("Notify_ReceivedNewStockFiles", new object[2]
			{
				deviceID,
				uploadedFiles
			});
		}

		public void Notify_ReceivedNewStockFilesAsync(long deviceID, string uploadedFiles)
		{
			Notify_ReceivedNewStockFilesAsync(deviceID, uploadedFiles, null);
		}

		public void Notify_ReceivedNewStockFilesAsync(long deviceID, string uploadedFiles, object userState)
		{
			if (Notify_ReceivedNewStockFilesOperationCompleted == null)
			{
				Notify_ReceivedNewStockFilesOperationCompleted = OnNotify_ReceivedNewStockFilesOperationCompleted;
			}
			InvokeAsync("Notify_ReceivedNewStockFiles", new object[2]
			{
				deviceID,
				uploadedFiles
			}, Notify_ReceivedNewStockFilesOperationCompleted, userState);
		}

		private void OnNotify_ReceivedNewStockFilesOperationCompleted(object arg)
		{
			if (this.Notify_ReceivedNewStockFilesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.Notify_ReceivedNewStockFilesCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetModelString", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string GetModelString(string Email, string Password, int ProductIdentifier)
		{
			object[] array = Invoke("GetModelString", new object[3]
			{
				Email,
				Password,
				ProductIdentifier
			});
			return (string)array[0];
		}

		public void GetModelStringAsync(string Email, string Password, int ProductIdentifier)
		{
			GetModelStringAsync(Email, Password, ProductIdentifier, null);
		}

		public void GetModelStringAsync(string Email, string Password, int ProductIdentifier, object userState)
		{
			if (GetModelStringOperationCompleted == null)
			{
				GetModelStringOperationCompleted = OnGetModelStringOperationCompleted;
			}
			InvokeAsync("GetModelString", new object[3]
			{
				Email,
				Password,
				ProductIdentifier
			}, GetModelStringOperationCompleted, userState);
		}

		private void OnGetModelStringOperationCompleted(object arg)
		{
			if (this.GetModelStringCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetModelStringCompleted(this, new GetModelStringCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/CreateCustomFirmwareZip", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string CreateCustomFirmwareZip(string Email, string Password, string SerialNumber, string InputFileName, string MSP430Version, string FPGAVersion, string FirmwareVersion)
		{
			object[] array = Invoke("CreateCustomFirmwareZip", new object[7]
			{
				Email,
				Password,
				SerialNumber,
				InputFileName,
				MSP430Version,
				FPGAVersion,
				FirmwareVersion
			});
			return (string)array[0];
		}

		public void CreateCustomFirmwareZipAsync(string Email, string Password, string SerialNumber, string InputFileName, string MSP430Version, string FPGAVersion, string FirmwareVersion)
		{
			CreateCustomFirmwareZipAsync(Email, Password, SerialNumber, InputFileName, MSP430Version, FPGAVersion, FirmwareVersion, null);
		}

		public void CreateCustomFirmwareZipAsync(string Email, string Password, string SerialNumber, string InputFileName, string MSP430Version, string FPGAVersion, string FirmwareVersion, object userState)
		{
			if (CreateCustomFirmwareZipOperationCompleted == null)
			{
				CreateCustomFirmwareZipOperationCompleted = OnCreateCustomFirmwareZipOperationCompleted;
			}
			InvokeAsync("CreateCustomFirmwareZip", new object[7]
			{
				Email,
				Password,
				SerialNumber,
				InputFileName,
				MSP430Version,
				FPGAVersion,
				FirmwareVersion
			}, CreateCustomFirmwareZipOperationCompleted, userState);
		}

		private void OnCreateCustomFirmwareZipOperationCompleted(object arg)
		{
			if (this.CreateCustomFirmwareZipCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateCustomFirmwareZipCompleted(this, new CreateCustomFirmwareZipCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/CreateCustomCalibraionZip", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string CreateCustomCalibraionZip(string Email, string Password, string SerialNumber, string InputFileName, [XmlElement(DataType = "base64Binary")] byte[] XmlFileVerList)
		{
			object[] array = Invoke("CreateCustomCalibraionZip", new object[5]
			{
				Email,
				Password,
				SerialNumber,
				InputFileName,
				XmlFileVerList
			});
			return (string)array[0];
		}

		public void CreateCustomCalibraionZipAsync(string Email, string Password, string SerialNumber, string InputFileName, byte[] XmlFileVerList)
		{
			CreateCustomCalibraionZipAsync(Email, Password, SerialNumber, InputFileName, XmlFileVerList, null);
		}

		public void CreateCustomCalibraionZipAsync(string Email, string Password, string SerialNumber, string InputFileName, byte[] XmlFileVerList, object userState)
		{
			if (CreateCustomCalibraionZipOperationCompleted == null)
			{
				CreateCustomCalibraionZipOperationCompleted = OnCreateCustomCalibraionZipOperationCompleted;
			}
			InvokeAsync("CreateCustomCalibraionZip", new object[5]
			{
				Email,
				Password,
				SerialNumber,
				InputFileName,
				XmlFileVerList
			}, CreateCustomCalibraionZipOperationCompleted, userState);
		}

		private void OnCreateCustomCalibraionZipOperationCompleted(object arg)
		{
			if (this.CreateCustomCalibraionZipCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateCustomCalibraionZipCompleted(this, new CreateCustomCalibraionZipCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/NewCreateCustomCalibraionZip", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string NewCreateCustomCalibraionZip(string Email, string Password, string SerialNumber, string InputFileName, [XmlElement(DataType = "base64Binary")] byte[] XmlFileVerList)
		{
			object[] array = Invoke("NewCreateCustomCalibraionZip", new object[5]
			{
				Email,
				Password,
				SerialNumber,
				InputFileName,
				XmlFileVerList
			});
			return (string)array[0];
		}

		public void NewCreateCustomCalibraionZipAsync(string Email, string Password, string SerialNumber, string InputFileName, byte[] XmlFileVerList)
		{
			NewCreateCustomCalibraionZipAsync(Email, Password, SerialNumber, InputFileName, XmlFileVerList, null);
		}

		public void NewCreateCustomCalibraionZipAsync(string Email, string Password, string SerialNumber, string InputFileName, byte[] XmlFileVerList, object userState)
		{
			if (NewCreateCustomCalibraionZipOperationCompleted == null)
			{
				NewCreateCustomCalibraionZipOperationCompleted = OnNewCreateCustomCalibraionZipOperationCompleted;
			}
			InvokeAsync("NewCreateCustomCalibraionZip", new object[5]
			{
				Email,
				Password,
				SerialNumber,
				InputFileName,
				XmlFileVerList
			}, NewCreateCustomCalibraionZipOperationCompleted, userState);
		}

		private void OnNewCreateCustomCalibraionZipOperationCompleted(object arg)
		{
			if (this.NewCreateCustomCalibraionZipCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.NewCreateCustomCalibraionZipCompleted(this, new NewCreateCustomCalibraionZipCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetModelLevelOffsetInSettings", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public int GetModelLevelOffsetInSettings(string Email, string Password, int nProductIdentifier)
		{
			object[] array = Invoke("GetModelLevelOffsetInSettings", new object[3]
			{
				Email,
				Password,
				nProductIdentifier
			});
			return (int)array[0];
		}

		public void GetModelLevelOffsetInSettingsAsync(string Email, string Password, int nProductIdentifier)
		{
			GetModelLevelOffsetInSettingsAsync(Email, Password, nProductIdentifier, null);
		}

		public void GetModelLevelOffsetInSettingsAsync(string Email, string Password, int nProductIdentifier, object userState)
		{
			if (GetModelLevelOffsetInSettingsOperationCompleted == null)
			{
				GetModelLevelOffsetInSettingsOperationCompleted = OnGetModelLevelOffsetInSettingsOperationCompleted;
			}
			InvokeAsync("GetModelLevelOffsetInSettings", new object[3]
			{
				Email,
				Password,
				nProductIdentifier
			}, GetModelLevelOffsetInSettingsOperationCompleted, userState);
		}

		private void OnGetModelLevelOffsetInSettingsOperationCompleted(object arg)
		{
			if (this.GetModelLevelOffsetInSettingsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetModelLevelOffsetInSettingsCompleted(this, new GetModelLevelOffsetInSettingsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetProductIDFromModelString", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public int GetProductIDFromModelString(string Email, string Password, string ProductName)
		{
			object[] array = Invoke("GetProductIDFromModelString", new object[3]
			{
				Email,
				Password,
				ProductName
			});
			return (int)array[0];
		}

		public void GetProductIDFromModelStringAsync(string Email, string Password, string ProductName)
		{
			GetProductIDFromModelStringAsync(Email, Password, ProductName, null);
		}

		public void GetProductIDFromModelStringAsync(string Email, string Password, string ProductName, object userState)
		{
			if (GetProductIDFromModelStringOperationCompleted == null)
			{
				GetProductIDFromModelStringOperationCompleted = OnGetProductIDFromModelStringOperationCompleted;
			}
			InvokeAsync("GetProductIDFromModelString", new object[3]
			{
				Email,
				Password,
				ProductName
			}, GetProductIDFromModelStringOperationCompleted, userState);
		}

		private void OnGetProductIDFromModelStringOperationCompleted(object arg)
		{
			if (this.GetProductIDFromModelStringCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetProductIDFromModelStringCompleted(this, new GetProductIDFromModelStringCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetModelFromUserSerialNum", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public int GetModelFromUserSerialNum(string Email, string Password, long SerialNum)
		{
			object[] array = Invoke("GetModelFromUserSerialNum", new object[3]
			{
				Email,
				Password,
				SerialNum
			});
			return (int)array[0];
		}

		public void GetModelFromUserSerialNumAsync(string Email, string Password, long SerialNum)
		{
			GetModelFromUserSerialNumAsync(Email, Password, SerialNum, null);
		}

		public void GetModelFromUserSerialNumAsync(string Email, string Password, long SerialNum, object userState)
		{
			if (GetModelFromUserSerialNumOperationCompleted == null)
			{
				GetModelFromUserSerialNumOperationCompleted = OnGetModelFromUserSerialNumOperationCompleted;
			}
			InvokeAsync("GetModelFromUserSerialNum", new object[3]
			{
				Email,
				Password,
				SerialNum
			}, GetModelFromUserSerialNumOperationCompleted, userState);
		}

		private void OnGetModelFromUserSerialNumOperationCompleted(object arg)
		{
			if (this.GetModelFromUserSerialNumCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetModelFromUserSerialNumCompleted(this, new GetModelFromUserSerialNumCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetCanUpgradeSKUFromUserSerialNum", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string GetCanUpgradeSKUFromUserSerialNum(string Email, string Password, long SerialNum)
		{
			object[] array = Invoke("GetCanUpgradeSKUFromUserSerialNum", new object[3]
			{
				Email,
				Password,
				SerialNum
			});
			return (string)array[0];
		}

		public void GetCanUpgradeSKUFromUserSerialNumAsync(string Email, string Password, long SerialNum)
		{
			GetCanUpgradeSKUFromUserSerialNumAsync(Email, Password, SerialNum, null);
		}

		public void GetCanUpgradeSKUFromUserSerialNumAsync(string Email, string Password, long SerialNum, object userState)
		{
			if (GetCanUpgradeSKUFromUserSerialNumOperationCompleted == null)
			{
				GetCanUpgradeSKUFromUserSerialNumOperationCompleted = OnGetCanUpgradeSKUFromUserSerialNumOperationCompleted;
			}
			InvokeAsync("GetCanUpgradeSKUFromUserSerialNum", new object[3]
			{
				Email,
				Password,
				SerialNum
			}, GetCanUpgradeSKUFromUserSerialNumOperationCompleted, userState);
		}

		private void OnGetCanUpgradeSKUFromUserSerialNumOperationCompleted(object arg)
		{
			if (this.GetCanUpgradeSKUFromUserSerialNumCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetCanUpgradeSKUFromUserSerialNumCompleted(this, new GetCanUpgradeSKUFromUserSerialNumCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetSKUChangedFromUserSerialNum", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool GetSKUChangedFromUserSerialNum(string Email, string Password, long SerialNum)
		{
			object[] array = Invoke("GetSKUChangedFromUserSerialNum", new object[3]
			{
				Email,
				Password,
				SerialNum
			});
			return (bool)array[0];
		}

		public void GetSKUChangedFromUserSerialNumAsync(string Email, string Password, long SerialNum)
		{
			GetSKUChangedFromUserSerialNumAsync(Email, Password, SerialNum, null);
		}

		public void GetSKUChangedFromUserSerialNumAsync(string Email, string Password, long SerialNum, object userState)
		{
			if (GetSKUChangedFromUserSerialNumOperationCompleted == null)
			{
				GetSKUChangedFromUserSerialNumOperationCompleted = OnGetSKUChangedFromUserSerialNumOperationCompleted;
			}
			InvokeAsync("GetSKUChangedFromUserSerialNum", new object[3]
			{
				Email,
				Password,
				SerialNum
			}, GetSKUChangedFromUserSerialNumOperationCompleted, userState);
		}

		private void OnGetSKUChangedFromUserSerialNumOperationCompleted(object arg)
		{
			if (this.GetSKUChangedFromUserSerialNumCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetSKUChangedFromUserSerialNumCompleted(this, new GetSKUChangedFromUserSerialNumCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/ClearSKUChangedFromUserSerialNum", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string ClearSKUChangedFromUserSerialNum(string Email, string Password, long SerialNum)
		{
			object[] array = Invoke("ClearSKUChangedFromUserSerialNum", new object[3]
			{
				Email,
				Password,
				SerialNum
			});
			return (string)array[0];
		}

		public void ClearSKUChangedFromUserSerialNumAsync(string Email, string Password, long SerialNum)
		{
			ClearSKUChangedFromUserSerialNumAsync(Email, Password, SerialNum, null);
		}

		public void ClearSKUChangedFromUserSerialNumAsync(string Email, string Password, long SerialNum, object userState)
		{
			if (ClearSKUChangedFromUserSerialNumOperationCompleted == null)
			{
				ClearSKUChangedFromUserSerialNumOperationCompleted = OnClearSKUChangedFromUserSerialNumOperationCompleted;
			}
			InvokeAsync("ClearSKUChangedFromUserSerialNum", new object[3]
			{
				Email,
				Password,
				SerialNum
			}, ClearSKUChangedFromUserSerialNumOperationCompleted, userState);
		}

		private void OnClearSKUChangedFromUserSerialNumOperationCompleted(object arg)
		{
			if (this.ClearSKUChangedFromUserSerialNumCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ClearSKUChangedFromUserSerialNumCompleted(this, new ClearSKUChangedFromUserSerialNumCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/CreatingUpdateUserForRacing", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string CreatingUpdateUserForRacing(string Email, long SerialNum, string FirstName, string LastName, string Address1, string Address2, string City, string State, string Zip, string Country, string Phone, string RacingAssociation)
		{
			object[] array = Invoke("CreatingUpdateUserForRacing", new object[12]
			{
				Email,
				SerialNum,
				FirstName,
				LastName,
				Address1,
				Address2,
				City,
				State,
				Zip,
				Country,
				Phone,
				RacingAssociation
			});
			return (string)array[0];
		}

		public void CreatingUpdateUserForRacingAsync(string Email, long SerialNum, string FirstName, string LastName, string Address1, string Address2, string City, string State, string Zip, string Country, string Phone, string RacingAssociation)
		{
			CreatingUpdateUserForRacingAsync(Email, SerialNum, FirstName, LastName, Address1, Address2, City, State, Zip, Country, Phone, RacingAssociation, null);
		}

		public void CreatingUpdateUserForRacingAsync(string Email, long SerialNum, string FirstName, string LastName, string Address1, string Address2, string City, string State, string Zip, string Country, string Phone, string RacingAssociation, object userState)
		{
			if (CreatingUpdateUserForRacingOperationCompleted == null)
			{
				CreatingUpdateUserForRacingOperationCompleted = OnCreatingUpdateUserForRacingOperationCompleted;
			}
			InvokeAsync("CreatingUpdateUserForRacing", new object[12]
			{
				Email,
				SerialNum,
				FirstName,
				LastName,
				Address1,
				Address2,
				City,
				State,
				Zip,
				Country,
				Phone,
				RacingAssociation
			}, CreatingUpdateUserForRacingOperationCompleted, userState);
		}

		private void OnCreatingUpdateUserForRacingOperationCompleted(object arg)
		{
			if (this.CreatingUpdateUserForRacingCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreatingUpdateUserForRacingCompleted(this, new CreatingUpdateUserForRacingCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/CreatingUpdateUserForTPMSControl", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string CreatingUpdateUserForTPMSControl(string Email, long SerialNum, string FirstName, string LastName, string Address1, string Address2, string City, string State, string Zip, string Country, string Phone)
		{
			object[] array = Invoke("CreatingUpdateUserForTPMSControl", new object[11]
			{
				Email,
				SerialNum,
				FirstName,
				LastName,
				Address1,
				Address2,
				City,
				State,
				Zip,
				Country,
				Phone
			});
			return (string)array[0];
		}

		public void CreatingUpdateUserForTPMSControlAsync(string Email, long SerialNum, string FirstName, string LastName, string Address1, string Address2, string City, string State, string Zip, string Country, string Phone)
		{
			CreatingUpdateUserForTPMSControlAsync(Email, SerialNum, FirstName, LastName, Address1, Address2, City, State, Zip, Country, Phone, null);
		}

		public void CreatingUpdateUserForTPMSControlAsync(string Email, long SerialNum, string FirstName, string LastName, string Address1, string Address2, string City, string State, string Zip, string Country, string Phone, object userState)
		{
			if (CreatingUpdateUserForTPMSControlOperationCompleted == null)
			{
				CreatingUpdateUserForTPMSControlOperationCompleted = OnCreatingUpdateUserForTPMSControlOperationCompleted;
			}
			InvokeAsync("CreatingUpdateUserForTPMSControl", new object[11]
			{
				Email,
				SerialNum,
				FirstName,
				LastName,
				Address1,
				Address2,
				City,
				State,
				Zip,
				Country,
				Phone
			}, CreatingUpdateUserForTPMSControlOperationCompleted, userState);
		}

		private void OnCreatingUpdateUserForTPMSControlOperationCompleted(object arg)
		{
			if (this.CreatingUpdateUserForTPMSControlCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreatingUpdateUserForTPMSControlCompleted(this, new CreatingUpdateUserForTPMSControlCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/CreatingUserForHPTuners", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string CreatingUserForHPTuners(string Email, string Password)
		{
			object[] array = Invoke("CreatingUserForHPTuners", new object[2]
			{
				Email,
				Password
			});
			return (string)array[0];
		}

		public void CreatingUserForHPTunersAsync(string Email, string Password)
		{
			CreatingUserForHPTunersAsync(Email, Password, null);
		}

		public void CreatingUserForHPTunersAsync(string Email, string Password, object userState)
		{
			if (CreatingUserForHPTunersOperationCompleted == null)
			{
				CreatingUserForHPTunersOperationCompleted = OnCreatingUserForHPTunersOperationCompleted;
			}
			InvokeAsync("CreatingUserForHPTuners", new object[2]
			{
				Email,
				Password
			}, CreatingUserForHPTunersOperationCompleted, userState);
		}

		private void OnCreatingUserForHPTunersOperationCompleted(object arg)
		{
			if (this.CreatingUserForHPTunersCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreatingUserForHPTunersCompleted(this, new CreatingUserForHPTunersCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/CreatingUpdateUserForHotUnlock", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string CreatingUpdateUserForHotUnlock(string Email, long SerialNum, string FirstName, string LastName, string Address1, string Address2, string City, string State, string Zip, string Country, string Phone, string RacingAssociation)
		{
			object[] array = Invoke("CreatingUpdateUserForHotUnlock", new object[12]
			{
				Email,
				SerialNum,
				FirstName,
				LastName,
				Address1,
				Address2,
				City,
				State,
				Zip,
				Country,
				Phone,
				RacingAssociation
			});
			return (string)array[0];
		}

		public void CreatingUpdateUserForHotUnlockAsync(string Email, long SerialNum, string FirstName, string LastName, string Address1, string Address2, string City, string State, string Zip, string Country, string Phone, string RacingAssociation)
		{
			CreatingUpdateUserForHotUnlockAsync(Email, SerialNum, FirstName, LastName, Address1, Address2, City, State, Zip, Country, Phone, RacingAssociation, null);
		}

		public void CreatingUpdateUserForHotUnlockAsync(string Email, long SerialNum, string FirstName, string LastName, string Address1, string Address2, string City, string State, string Zip, string Country, string Phone, string RacingAssociation, object userState)
		{
			if (CreatingUpdateUserForHotUnlockOperationCompleted == null)
			{
				CreatingUpdateUserForHotUnlockOperationCompleted = OnCreatingUpdateUserForHotUnlockOperationCompleted;
			}
			InvokeAsync("CreatingUpdateUserForHotUnlock", new object[12]
			{
				Email,
				SerialNum,
				FirstName,
				LastName,
				Address1,
				Address2,
				City,
				State,
				Zip,
				Country,
				Phone,
				RacingAssociation
			}, CreatingUpdateUserForHotUnlockOperationCompleted, userState);
		}

		private void OnCreatingUpdateUserForHotUnlockOperationCompleted(object arg)
		{
			if (this.CreatingUpdateUserForHotUnlockCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreatingUpdateUserForHotUnlockCompleted(this, new CreatingUpdateUserForHotUnlockCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/UpdateUserForHotUnlock", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string UpdateUserForHotUnlock(string Email, long SerialNum, bool HotUnlockEnable)
		{
			object[] array = Invoke("UpdateUserForHotUnlock", new object[3]
			{
				Email,
				SerialNum,
				HotUnlockEnable
			});
			return (string)array[0];
		}

		public void UpdateUserForHotUnlockAsync(string Email, long SerialNum, bool HotUnlockEnable)
		{
			UpdateUserForHotUnlockAsync(Email, SerialNum, HotUnlockEnable, null);
		}

		public void UpdateUserForHotUnlockAsync(string Email, long SerialNum, bool HotUnlockEnable, object userState)
		{
			if (UpdateUserForHotUnlockOperationCompleted == null)
			{
				UpdateUserForHotUnlockOperationCompleted = OnUpdateUserForHotUnlockOperationCompleted;
			}
			InvokeAsync("UpdateUserForHotUnlock", new object[3]
			{
				Email,
				SerialNum,
				HotUnlockEnable
			}, UpdateUserForHotUnlockOperationCompleted, userState);
		}

		private void OnUpdateUserForHotUnlockOperationCompleted(object arg)
		{
			if (this.UpdateUserForHotUnlockCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateUserForHotUnlockCompleted(this, new UpdateUserForHotUnlockCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/RetrieveUserRacingCode", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string RetrieveUserRacingCode(string Email, long SerialNum)
		{
			object[] array = Invoke("RetrieveUserRacingCode", new object[2]
			{
				Email,
				SerialNum
			});
			return (string)array[0];
		}

		public void RetrieveUserRacingCodeAsync(string Email, long SerialNum)
		{
			RetrieveUserRacingCodeAsync(Email, SerialNum, null);
		}

		public void RetrieveUserRacingCodeAsync(string Email, long SerialNum, object userState)
		{
			if (RetrieveUserRacingCodeOperationCompleted == null)
			{
				RetrieveUserRacingCodeOperationCompleted = OnRetrieveUserRacingCodeOperationCompleted;
			}
			InvokeAsync("RetrieveUserRacingCode", new object[2]
			{
				Email,
				SerialNum
			}, RetrieveUserRacingCodeOperationCompleted, userState);
		}

		private void OnRetrieveUserRacingCodeOperationCompleted(object arg)
		{
			if (this.RetrieveUserRacingCodeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RetrieveUserRacingCodeCompleted(this, new RetrieveUserRacingCodeCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/RetrieveUserHotUnlockCode", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string RetrieveUserHotUnlockCode(string Email, long SerialNum)
		{
			object[] array = Invoke("RetrieveUserHotUnlockCode", new object[2]
			{
				Email,
				SerialNum
			});
			return (string)array[0];
		}

		public void RetrieveUserHotUnlockCodeAsync(string Email, long SerialNum)
		{
			RetrieveUserHotUnlockCodeAsync(Email, SerialNum, null);
		}

		public void RetrieveUserHotUnlockCodeAsync(string Email, long SerialNum, object userState)
		{
			if (RetrieveUserHotUnlockCodeOperationCompleted == null)
			{
				RetrieveUserHotUnlockCodeOperationCompleted = OnRetrieveUserHotUnlockCodeOperationCompleted;
			}
			InvokeAsync("RetrieveUserHotUnlockCode", new object[2]
			{
				Email,
				SerialNum
			}, RetrieveUserHotUnlockCodeOperationCompleted, userState);
		}

		private void OnRetrieveUserHotUnlockCodeOperationCompleted(object arg)
		{
			if (this.RetrieveUserHotUnlockCodeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RetrieveUserHotUnlockCodeCompleted(this, new RetrieveUserHotUnlockCodeCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetBootloaderInfo", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string[] GetBootloaderInfo(string Email, string Password, int nProductIdentifier)
		{
			object[] array = Invoke("GetBootloaderInfo", new object[3]
			{
				Email,
				Password,
				nProductIdentifier
			});
			return (string[])array[0];
		}

		public void GetBootloaderInfoAsync(string Email, string Password, int nProductIdentifier)
		{
			GetBootloaderInfoAsync(Email, Password, nProductIdentifier, null);
		}

		public void GetBootloaderInfoAsync(string Email, string Password, int nProductIdentifier, object userState)
		{
			if (GetBootloaderInfoOperationCompleted == null)
			{
				GetBootloaderInfoOperationCompleted = OnGetBootloaderInfoOperationCompleted;
			}
			InvokeAsync("GetBootloaderInfo", new object[3]
			{
				Email,
				Password,
				nProductIdentifier
			}, GetBootloaderInfoOperationCompleted, userState);
		}

		private void OnGetBootloaderInfoOperationCompleted(object arg)
		{
			if (this.GetBootloaderInfoCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetBootloaderInfoCompleted(this, new GetBootloaderInfoCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetFwCalDescription", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string GetFwCalDescription(long fwCalID)
		{
			object[] array = Invoke("GetFwCalDescription", new object[1]
			{
				fwCalID
			});
			return (string)array[0];
		}

		public void GetFwCalDescriptionAsync(long fwCalID)
		{
			GetFwCalDescriptionAsync(fwCalID, null);
		}

		public void GetFwCalDescriptionAsync(long fwCalID, object userState)
		{
			if (GetFwCalDescriptionOperationCompleted == null)
			{
				GetFwCalDescriptionOperationCompleted = OnGetFwCalDescriptionOperationCompleted;
			}
			InvokeAsync("GetFwCalDescription", new object[1]
			{
				fwCalID
			}, GetFwCalDescriptionOperationCompleted, userState);
		}

		private void OnGetFwCalDescriptionOperationCompleted(object arg)
		{
			if (this.GetFwCalDescriptionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetFwCalDescriptionCompleted(this, new GetFwCalDescriptionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetFwCalChecksum", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public uint GetFwCalChecksum(string pathFwCal)
		{
			object[] array = Invoke("GetFwCalChecksum", new object[1]
			{
				pathFwCal
			});
			return (uint)array[0];
		}

		public void GetFwCalChecksumAsync(string pathFwCal)
		{
			GetFwCalChecksumAsync(pathFwCal, null);
		}

		public void GetFwCalChecksumAsync(string pathFwCal, object userState)
		{
			if (GetFwCalChecksumOperationCompleted == null)
			{
				GetFwCalChecksumOperationCompleted = OnGetFwCalChecksumOperationCompleted;
			}
			InvokeAsync("GetFwCalChecksum", new object[1]
			{
				pathFwCal
			}, GetFwCalChecksumOperationCompleted, userState);
		}

		private void OnGetFwCalChecksumOperationCompleted(object arg)
		{
			if (this.GetFwCalChecksumCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetFwCalChecksumCompleted(this, new GetFwCalChecksumCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetAvailableFirmware", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string[] GetAvailableFirmware(string email, string password, int nProductID)
		{
			object[] array = Invoke("GetAvailableFirmware", new object[3]
			{
				email,
				password,
				nProductID
			});
			return (string[])array[0];
		}

		public void GetAvailableFirmwareAsync(string email, string password, int nProductID)
		{
			GetAvailableFirmwareAsync(email, password, nProductID, null);
		}

		public void GetAvailableFirmwareAsync(string email, string password, int nProductID, object userState)
		{
			if (GetAvailableFirmwareOperationCompleted == null)
			{
				GetAvailableFirmwareOperationCompleted = OnGetAvailableFirmwareOperationCompleted;
			}
			InvokeAsync("GetAvailableFirmware", new object[3]
			{
				email,
				password,
				nProductID
			}, GetAvailableFirmwareOperationCompleted, userState);
		}

		private void OnGetAvailableFirmwareOperationCompleted(object arg)
		{
			if (this.GetAvailableFirmwareCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetAvailableFirmwareCompleted(this, new GetAvailableFirmwareCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/IsUpdateEnabledForDevice", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool IsUpdateEnabledForDevice(int productId, out string updateMessage)
		{
			object[] array = Invoke("IsUpdateEnabledForDevice", new object[1]
			{
				productId
			});
			updateMessage = (string)array[1];
			return (bool)array[0];
		}

		public void IsUpdateEnabledForDeviceAsync(int productId)
		{
			IsUpdateEnabledForDeviceAsync(productId, null);
		}

		public void IsUpdateEnabledForDeviceAsync(int productId, object userState)
		{
			if (IsUpdateEnabledForDeviceOperationCompleted == null)
			{
				IsUpdateEnabledForDeviceOperationCompleted = OnIsUpdateEnabledForDeviceOperationCompleted;
			}
			InvokeAsync("IsUpdateEnabledForDevice", new object[1]
			{
				productId
			}, IsUpdateEnabledForDeviceOperationCompleted, userState);
		}

		private void OnIsUpdateEnabledForDeviceOperationCompleted(object arg)
		{
			if (this.IsUpdateEnabledForDeviceCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.IsUpdateEnabledForDeviceCompleted(this, new IsUpdateEnabledForDeviceCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetAvailableCalibrations", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string[] GetAvailableCalibrations(string email, string password, int nProductID, long FirmwareID)
		{
			object[] array = Invoke("GetAvailableCalibrations", new object[4]
			{
				email,
				password,
				nProductID,
				FirmwareID
			});
			return (string[])array[0];
		}

		public void GetAvailableCalibrationsAsync(string email, string password, int nProductID, long FirmwareID)
		{
			GetAvailableCalibrationsAsync(email, password, nProductID, FirmwareID, null);
		}

		public void GetAvailableCalibrationsAsync(string email, string password, int nProductID, long FirmwareID, object userState)
		{
			if (GetAvailableCalibrationsOperationCompleted == null)
			{
				GetAvailableCalibrationsOperationCompleted = OnGetAvailableCalibrationsOperationCompleted;
			}
			InvokeAsync("GetAvailableCalibrations", new object[4]
			{
				email,
				password,
				nProductID,
				FirmwareID
			}, GetAvailableCalibrationsOperationCompleted, userState);
		}

		private void OnGetAvailableCalibrationsOperationCompleted(object arg)
		{
			if (this.GetAvailableCalibrationsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetAvailableCalibrationsCompleted(this, new GetAvailableCalibrationsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/SetUpdateComplete", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void SetUpdateComplete(string Email, string Password, string SerialNum, long ProductIdentifier)
		{
			Invoke("SetUpdateComplete", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductIdentifier
			});
		}

		public void SetUpdateCompleteAsync(string Email, string Password, string SerialNum, long ProductIdentifier)
		{
			SetUpdateCompleteAsync(Email, Password, SerialNum, ProductIdentifier, null);
		}

		public void SetUpdateCompleteAsync(string Email, string Password, string SerialNum, long ProductIdentifier, object userState)
		{
			if (SetUpdateCompleteOperationCompleted == null)
			{
				SetUpdateCompleteOperationCompleted = OnSetUpdateCompleteOperationCompleted;
			}
			InvokeAsync("SetUpdateComplete", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductIdentifier
			}, SetUpdateCompleteOperationCompleted, userState);
		}

		private void OnSetUpdateCompleteOperationCompleted(object arg)
		{
			if (this.SetUpdateCompleteCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetUpdateCompleteCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/SetPlatformType", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void SetPlatformType(string Email, string Password, string SerialNum, long ProductIdentifier, int PlatformType)
		{
			Invoke("SetPlatformType", new object[5]
			{
				Email,
				Password,
				SerialNum,
				ProductIdentifier,
				PlatformType
			});
		}

		public void SetPlatformTypeAsync(string Email, string Password, string SerialNum, long ProductIdentifier, int PlatformType)
		{
			SetPlatformTypeAsync(Email, Password, SerialNum, ProductIdentifier, PlatformType, null);
		}

		public void SetPlatformTypeAsync(string Email, string Password, string SerialNum, long ProductIdentifier, int PlatformType, object userState)
		{
			if (SetPlatformTypeOperationCompleted == null)
			{
				SetPlatformTypeOperationCompleted = OnSetPlatformTypeOperationCompleted;
			}
			InvokeAsync("SetPlatformType", new object[5]
			{
				Email,
				Password,
				SerialNum,
				ProductIdentifier,
				PlatformType
			}, SetPlatformTypeOperationCompleted, userState);
		}

		private void OnSetPlatformTypeOperationCompleted(object arg)
		{
			if (this.SetPlatformTypeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetPlatformTypeCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/Log", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void Log(string Email, string Password, string SerialNum, long firmwareID, long calibrationID, long ProductIdentifier, string Description)
		{
			Invoke("Log", new object[7]
			{
				Email,
				Password,
				SerialNum,
				firmwareID,
				calibrationID,
				ProductIdentifier,
				Description
			});
		}

		public void LogAsync(string Email, string Password, string SerialNum, long firmwareID, long calibrationID, long ProductIdentifier, string Description)
		{
			LogAsync(Email, Password, SerialNum, firmwareID, calibrationID, ProductIdentifier, Description, null);
		}

		public void LogAsync(string Email, string Password, string SerialNum, long firmwareID, long calibrationID, long ProductIdentifier, string Description, object userState)
		{
			if (LogOperationCompleted == null)
			{
				LogOperationCompleted = OnLogOperationCompleted;
			}
			InvokeAsync("Log", new object[7]
			{
				Email,
				Password,
				SerialNum,
				firmwareID,
				calibrationID,
				ProductIdentifier,
				Description
			}, LogOperationCompleted, userState);
		}

		private void OnLogOperationCompleted(object arg)
		{
			if (this.LogCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.LogCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetTargetFirmCalIDs", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public long[] GetTargetFirmCalIDs(string Email, string Password, string SerialNum, long ProductID)
		{
			object[] array = Invoke("GetTargetFirmCalIDs", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
			return (long[])array[0];
		}

		public void GetTargetFirmCalIDsAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			GetTargetFirmCalIDsAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void GetTargetFirmCalIDsAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (GetTargetFirmCalIDsOperationCompleted == null)
			{
				GetTargetFirmCalIDsOperationCompleted = OnGetTargetFirmCalIDsOperationCompleted;
			}
			InvokeAsync("GetTargetFirmCalIDs", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, GetTargetFirmCalIDsOperationCompleted, userState);
		}

		private void OnGetTargetFirmCalIDsOperationCompleted(object arg)
		{
			if (this.GetTargetFirmCalIDsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetTargetFirmCalIDsCompleted(this, new GetTargetFirmCalIDsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetCurrentFwCal", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public long[] GetCurrentFwCal(string Email, string Password, long productIdentifier)
		{
			object[] array = Invoke("GetCurrentFwCal", new object[3]
			{
				Email,
				Password,
				productIdentifier
			});
			return (long[])array[0];
		}

		public void GetCurrentFwCalAsync(string Email, string Password, long productIdentifier)
		{
			GetCurrentFwCalAsync(Email, Password, productIdentifier, null);
		}

		public void GetCurrentFwCalAsync(string Email, string Password, long productIdentifier, object userState)
		{
			if (GetCurrentFwCalOperationCompleted == null)
			{
				GetCurrentFwCalOperationCompleted = OnGetCurrentFwCalOperationCompleted;
			}
			InvokeAsync("GetCurrentFwCal", new object[3]
			{
				Email,
				Password,
				productIdentifier
			}, GetCurrentFwCalOperationCompleted, userState);
		}

		private void OnGetCurrentFwCalOperationCompleted(object arg)
		{
			if (this.GetCurrentFwCalCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetCurrentFwCalCompleted(this, new GetCurrentFwCalCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/IsGetAllFilesSet", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool IsGetAllFilesSet(string Email, string Password, string SerialNum, long ProductID)
		{
			object[] array = Invoke("IsGetAllFilesSet", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
			return (bool)array[0];
		}

		public void IsGetAllFilesSetAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			IsGetAllFilesSetAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void IsGetAllFilesSetAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (IsGetAllFilesSetOperationCompleted == null)
			{
				IsGetAllFilesSetOperationCompleted = OnIsGetAllFilesSetOperationCompleted;
			}
			InvokeAsync("IsGetAllFilesSet", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, IsGetAllFilesSetOperationCompleted, userState);
		}

		private void OnIsGetAllFilesSetOperationCompleted(object arg)
		{
			if (this.IsGetAllFilesSetCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.IsGetAllFilesSetCompleted(this, new IsGetAllFilesSetCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/IsGetAllCriticalFilesSet", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool IsGetAllCriticalFilesSet(string Email, string Password, string SerialNum, long ProductID)
		{
			object[] array = Invoke("IsGetAllCriticalFilesSet", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
			return (bool)array[0];
		}

		public void IsGetAllCriticalFilesSetAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			IsGetAllCriticalFilesSetAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void IsGetAllCriticalFilesSetAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (IsGetAllCriticalFilesSetOperationCompleted == null)
			{
				IsGetAllCriticalFilesSetOperationCompleted = OnIsGetAllCriticalFilesSetOperationCompleted;
			}
			InvokeAsync("IsGetAllCriticalFilesSet", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, IsGetAllCriticalFilesSetOperationCompleted, userState);
		}

		private void OnIsGetAllCriticalFilesSetOperationCompleted(object arg)
		{
			if (this.IsGetAllCriticalFilesSetCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.IsGetAllCriticalFilesSetCompleted(this, new IsGetAllCriticalFilesSetCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetCriticalFilesList", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string GetCriticalFilesList(string Email, string Password, long ProductID)
		{
			object[] array = Invoke("GetCriticalFilesList", new object[3]
			{
				Email,
				Password,
				ProductID
			});
			return (string)array[0];
		}

		public void GetCriticalFilesListAsync(string Email, string Password, long ProductID)
		{
			GetCriticalFilesListAsync(Email, Password, ProductID, null);
		}

		public void GetCriticalFilesListAsync(string Email, string Password, long ProductID, object userState)
		{
			if (GetCriticalFilesListOperationCompleted == null)
			{
				GetCriticalFilesListOperationCompleted = OnGetCriticalFilesListOperationCompleted;
			}
			InvokeAsync("GetCriticalFilesList", new object[3]
			{
				Email,
				Password,
				ProductID
			}, GetCriticalFilesListOperationCompleted, userState);
		}

		private void OnGetCriticalFilesListOperationCompleted(object arg)
		{
			if (this.GetCriticalFilesListCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetCriticalFilesListCompleted(this, new GetCriticalFilesListCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetSpecificFilesList", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string[] GetSpecificFilesList(string emailAddress, string password, string serialNumber, long productIdentifier)
		{
			object[] array = Invoke("GetSpecificFilesList", new object[4]
			{
				emailAddress,
				password,
				serialNumber,
				productIdentifier
			});
			return (string[])array[0];
		}

		public void GetSpecificFilesListAsync(string emailAddress, string password, string serialNumber, long productIdentifier)
		{
			GetSpecificFilesListAsync(emailAddress, password, serialNumber, productIdentifier, null);
		}

		public void GetSpecificFilesListAsync(string emailAddress, string password, string serialNumber, long productIdentifier, object userState)
		{
			if (GetSpecificFilesListOperationCompleted == null)
			{
				GetSpecificFilesListOperationCompleted = OnGetSpecificFilesListOperationCompleted;
			}
			InvokeAsync("GetSpecificFilesList", new object[4]
			{
				emailAddress,
				password,
				serialNumber,
				productIdentifier
			}, GetSpecificFilesListOperationCompleted, userState);
		}

		private void OnGetSpecificFilesListOperationCompleted(object arg)
		{
			if (this.GetSpecificFilesListCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetSpecificFilesListCompleted(this, new GetSpecificFilesListCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/ClearGetSpecificFiles", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ClearGetSpecificFiles(string emailAddress, string password, string serialNumber, long productIdentifier)
		{
			Invoke("ClearGetSpecificFiles", new object[4]
			{
				emailAddress,
				password,
				serialNumber,
				productIdentifier
			});
		}

		public void ClearGetSpecificFilesAsync(string emailAddress, string password, string serialNumber, long productIdentifier)
		{
			ClearGetSpecificFilesAsync(emailAddress, password, serialNumber, productIdentifier, null);
		}

		public void ClearGetSpecificFilesAsync(string emailAddress, string password, string serialNumber, long productIdentifier, object userState)
		{
			if (ClearGetSpecificFilesOperationCompleted == null)
			{
				ClearGetSpecificFilesOperationCompleted = OnClearGetSpecificFilesOperationCompleted;
			}
			InvokeAsync("ClearGetSpecificFiles", new object[4]
			{
				emailAddress,
				password,
				serialNumber,
				productIdentifier
			}, ClearGetSpecificFilesOperationCompleted, userState);
		}

		private void OnClearGetSpecificFilesOperationCompleted(object arg)
		{
			if (this.ClearGetSpecificFilesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ClearGetSpecificFilesCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/IsMakeStockWriterSet", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool IsMakeStockWriterSet(string Email, string Password, string SerialNum, long ProductID)
		{
			object[] array = Invoke("IsMakeStockWriterSet", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
			return (bool)array[0];
		}

		public void IsMakeStockWriterSetAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			IsMakeStockWriterSetAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void IsMakeStockWriterSetAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (IsMakeStockWriterSetOperationCompleted == null)
			{
				IsMakeStockWriterSetOperationCompleted = OnIsMakeStockWriterSetOperationCompleted;
			}
			InvokeAsync("IsMakeStockWriterSet", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, IsMakeStockWriterSetOperationCompleted, userState);
		}

		private void OnIsMakeStockWriterSetOperationCompleted(object arg)
		{
			if (this.IsMakeStockWriterSetCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.IsMakeStockWriterSetCompleted(this, new IsMakeStockWriterSetCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/IsSetStockFlagSet", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool IsSetStockFlagSet(string Email, string Password, string SerialNum, long ProductID)
		{
			object[] array = Invoke("IsSetStockFlagSet", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
			return (bool)array[0];
		}

		public void IsSetStockFlagSetAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			IsSetStockFlagSetAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void IsSetStockFlagSetAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (IsSetStockFlagSetOperationCompleted == null)
			{
				IsSetStockFlagSetOperationCompleted = OnIsSetStockFlagSetOperationCompleted;
			}
			InvokeAsync("IsSetStockFlagSet", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, IsSetStockFlagSetOperationCompleted, userState);
		}

		private void OnIsSetStockFlagSetOperationCompleted(object arg)
		{
			if (this.IsSetStockFlagSetCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.IsSetStockFlagSetCompleted(this, new IsSetStockFlagSetCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/IsEraseSettingsSet", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool IsEraseSettingsSet(string Email, string Password, string SerialNum, long ProductID)
		{
			object[] array = Invoke("IsEraseSettingsSet", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
			return (bool)array[0];
		}

		public void IsEraseSettingsSetAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			IsEraseSettingsSetAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void IsEraseSettingsSetAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (IsEraseSettingsSetOperationCompleted == null)
			{
				IsEraseSettingsSetOperationCompleted = OnIsEraseSettingsSetOperationCompleted;
			}
			InvokeAsync("IsEraseSettingsSet", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, IsEraseSettingsSetOperationCompleted, userState);
		}

		private void OnIsEraseSettingsSetOperationCompleted(object arg)
		{
			if (this.IsEraseSettingsSetCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.IsEraseSettingsSetCompleted(this, new IsEraseSettingsSetCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/IsResetToFactorySet", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool IsResetToFactorySet(string emailAddress, string password, string serialNumber, long productIdentifier)
		{
			object[] array = Invoke("IsResetToFactorySet", new object[4]
			{
				emailAddress,
				password,
				serialNumber,
				productIdentifier
			});
			return (bool)array[0];
		}

		public void IsResetToFactorySetAsync(string emailAddress, string password, string serialNumber, long productIdentifier)
		{
			IsResetToFactorySetAsync(emailAddress, password, serialNumber, productIdentifier, null);
		}

		public void IsResetToFactorySetAsync(string emailAddress, string password, string serialNumber, long productIdentifier, object userState)
		{
			if (IsResetToFactorySetOperationCompleted == null)
			{
				IsResetToFactorySetOperationCompleted = OnIsResetToFactorySetOperationCompleted;
			}
			InvokeAsync("IsResetToFactorySet", new object[4]
			{
				emailAddress,
				password,
				serialNumber,
				productIdentifier
			}, IsResetToFactorySetOperationCompleted, userState);
		}

		private void OnIsResetToFactorySetOperationCompleted(object arg)
		{
			if (this.IsResetToFactorySetCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.IsResetToFactorySetCompleted(this, new IsResetToFactorySetCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/DeviceCanBeUpdated", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool DeviceCanBeUpdated(string Email, string Password, string SerialNum, long ProductID)
		{
			object[] array = Invoke("DeviceCanBeUpdated", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
			return (bool)array[0];
		}

		public void DeviceCanBeUpdatedAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			DeviceCanBeUpdatedAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void DeviceCanBeUpdatedAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (DeviceCanBeUpdatedOperationCompleted == null)
			{
				DeviceCanBeUpdatedOperationCompleted = OnDeviceCanBeUpdatedOperationCompleted;
			}
			InvokeAsync("DeviceCanBeUpdated", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, DeviceCanBeUpdatedOperationCompleted, userState);
		}

		private void OnDeviceCanBeUpdatedOperationCompleted(object arg)
		{
			if (this.DeviceCanBeUpdatedCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeviceCanBeUpdatedCompleted(this, new DeviceCanBeUpdatedCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/IsForceUpdateSet", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool IsForceUpdateSet(string Email, string Password, string SerialNum, long ProductID)
		{
			object[] array = Invoke("IsForceUpdateSet", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
			return (bool)array[0];
		}

		public void IsForceUpdateSetAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			IsForceUpdateSetAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void IsForceUpdateSetAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (IsForceUpdateSetOperationCompleted == null)
			{
				IsForceUpdateSetOperationCompleted = OnIsForceUpdateSetOperationCompleted;
			}
			InvokeAsync("IsForceUpdateSet", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, IsForceUpdateSetOperationCompleted, userState);
		}

		private void OnIsForceUpdateSetOperationCompleted(object arg)
		{
			if (this.IsForceUpdateSetCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.IsForceUpdateSetCompleted(this, new IsForceUpdateSetCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/ClearEraseSettings", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ClearEraseSettings(string Email, string Password, string SerialNum, long ProductID)
		{
			Invoke("ClearEraseSettings", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
		}

		public void ClearEraseSettingsAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			ClearEraseSettingsAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void ClearEraseSettingsAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (ClearEraseSettingsOperationCompleted == null)
			{
				ClearEraseSettingsOperationCompleted = OnClearEraseSettingsOperationCompleted;
			}
			InvokeAsync("ClearEraseSettings", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, ClearEraseSettingsOperationCompleted, userState);
		}

		private void OnClearEraseSettingsOperationCompleted(object arg)
		{
			if (this.ClearEraseSettingsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ClearEraseSettingsCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/ClearResetToFactory", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ClearResetToFactory(string emailAddress, string password, string serialNumber, long productIdentifier)
		{
			Invoke("ClearResetToFactory", new object[4]
			{
				emailAddress,
				password,
				serialNumber,
				productIdentifier
			});
		}

		public void ClearResetToFactoryAsync(string emailAddress, string password, string serialNumber, long productIdentifier)
		{
			ClearResetToFactoryAsync(emailAddress, password, serialNumber, productIdentifier, null);
		}

		public void ClearResetToFactoryAsync(string emailAddress, string password, string serialNumber, long productIdentifier, object userState)
		{
			if (ClearResetToFactoryOperationCompleted == null)
			{
				ClearResetToFactoryOperationCompleted = OnClearResetToFactoryOperationCompleted;
			}
			InvokeAsync("ClearResetToFactory", new object[4]
			{
				emailAddress,
				password,
				serialNumber,
				productIdentifier
			}, ClearResetToFactoryOperationCompleted, userState);
		}

		private void OnClearResetToFactoryOperationCompleted(object arg)
		{
			if (this.ClearResetToFactoryCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ClearResetToFactoryCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/ClearMakeStockWriter", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ClearMakeStockWriter(string Email, string Password, string SerialNum, long ProductID)
		{
			Invoke("ClearMakeStockWriter", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
		}

		public void ClearMakeStockWriterAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			ClearMakeStockWriterAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void ClearMakeStockWriterAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (ClearMakeStockWriterOperationCompleted == null)
			{
				ClearMakeStockWriterOperationCompleted = OnClearMakeStockWriterOperationCompleted;
			}
			InvokeAsync("ClearMakeStockWriter", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, ClearMakeStockWriterOperationCompleted, userState);
		}

		private void OnClearMakeStockWriterOperationCompleted(object arg)
		{
			if (this.ClearMakeStockWriterCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ClearMakeStockWriterCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/ClearSetStock", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ClearSetStock(string Email, string Password, string SerialNum, long ProductID)
		{
			Invoke("ClearSetStock", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
		}

		public void ClearSetStockAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			ClearSetStockAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void ClearSetStockAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (ClearSetStockOperationCompleted == null)
			{
				ClearSetStockOperationCompleted = OnClearSetStockOperationCompleted;
			}
			InvokeAsync("ClearSetStock", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, ClearSetStockOperationCompleted, userState);
		}

		private void OnClearSetStockOperationCompleted(object arg)
		{
			if (this.ClearSetStockCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ClearSetStockCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/ClearGetAllFiles", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ClearGetAllFiles(string Email, string Password, string SerialNum, long ProductID)
		{
			Invoke("ClearGetAllFiles", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
		}

		public void ClearGetAllFilesAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			ClearGetAllFilesAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void ClearGetAllFilesAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (ClearGetAllFilesOperationCompleted == null)
			{
				ClearGetAllFilesOperationCompleted = OnClearGetAllFilesOperationCompleted;
			}
			InvokeAsync("ClearGetAllFiles", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, ClearGetAllFilesOperationCompleted, userState);
		}

		private void OnClearGetAllFilesOperationCompleted(object arg)
		{
			if (this.ClearGetAllFilesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ClearGetAllFilesCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/ClearGetAllCriticalFiles", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ClearGetAllCriticalFiles(string Email, string Password, string SerialNum, long ProductID)
		{
			Invoke("ClearGetAllCriticalFiles", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
		}

		public void ClearGetAllCriticalFilesAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			ClearGetAllCriticalFilesAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void ClearGetAllCriticalFilesAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (ClearGetAllCriticalFilesOperationCompleted == null)
			{
				ClearGetAllCriticalFilesOperationCompleted = OnClearGetAllCriticalFilesOperationCompleted;
			}
			InvokeAsync("ClearGetAllCriticalFiles", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, ClearGetAllCriticalFilesOperationCompleted, userState);
		}

		private void OnClearGetAllCriticalFilesOperationCompleted(object arg)
		{
			if (this.ClearGetAllCriticalFilesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ClearGetAllCriticalFilesCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetDeviceID", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public long GetDeviceID(string Email, string Password, string SerialNum, long ProductID)
		{
			object[] array = Invoke("GetDeviceID", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			});
			return (long)array[0];
		}

		public void GetDeviceIDAsync(string Email, string Password, string SerialNum, long ProductID)
		{
			GetDeviceIDAsync(Email, Password, SerialNum, ProductID, null);
		}

		public void GetDeviceIDAsync(string Email, string Password, string SerialNum, long ProductID, object userState)
		{
			if (GetDeviceIDOperationCompleted == null)
			{
				GetDeviceIDOperationCompleted = OnGetDeviceIDOperationCompleted;
			}
			InvokeAsync("GetDeviceID", new object[4]
			{
				Email,
				Password,
				SerialNum,
				ProductID
			}, GetDeviceIDOperationCompleted, userState);
		}

		private void OnGetDeviceIDOperationCompleted(object arg)
		{
			if (this.GetDeviceIDCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetDeviceIDCompleted(this, new GetDeviceIDCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetDeviceIDForSerial", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public long GetDeviceIDForSerial(string Email, string Password, string SerialNum)
		{
			object[] array = Invoke("GetDeviceIDForSerial", new object[3]
			{
				Email,
				Password,
				SerialNum
			});
			return (long)array[0];
		}

		public void GetDeviceIDForSerialAsync(string Email, string Password, string SerialNum)
		{
			GetDeviceIDForSerialAsync(Email, Password, SerialNum, null);
		}

		public void GetDeviceIDForSerialAsync(string Email, string Password, string SerialNum, object userState)
		{
			if (GetDeviceIDForSerialOperationCompleted == null)
			{
				GetDeviceIDForSerialOperationCompleted = OnGetDeviceIDForSerialOperationCompleted;
			}
			InvokeAsync("GetDeviceIDForSerial", new object[3]
			{
				Email,
				Password,
				SerialNum
			}, GetDeviceIDForSerialOperationCompleted, userState);
		}

		private void OnGetDeviceIDForSerialOperationCompleted(object arg)
		{
			if (this.GetDeviceIDForSerialCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetDeviceIDForSerialCompleted(this, new GetDeviceIDForSerialCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetUpdateFirmwareAndCalibrationIDs", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public long[] GetUpdateFirmwareAndCalibrationIDs(string emailAddress, string password, long productIdentifier, string serialNumber, string deviceFirmwareVersion, string deviceCalibrationVersion)
		{
			object[] array = Invoke("GetUpdateFirmwareAndCalibrationIDs", new object[6]
			{
				emailAddress,
				password,
				productIdentifier,
				serialNumber,
				deviceFirmwareVersion,
				deviceCalibrationVersion
			});
			return (long[])array[0];
		}

		public void GetUpdateFirmwareAndCalibrationIDsAsync(string emailAddress, string password, long productIdentifier, string serialNumber, string deviceFirmwareVersion, string deviceCalibrationVersion)
		{
			GetUpdateFirmwareAndCalibrationIDsAsync(emailAddress, password, productIdentifier, serialNumber, deviceFirmwareVersion, deviceCalibrationVersion, null);
		}

		public void GetUpdateFirmwareAndCalibrationIDsAsync(string emailAddress, string password, long productIdentifier, string serialNumber, string deviceFirmwareVersion, string deviceCalibrationVersion, object userState)
		{
			if (GetUpdateFirmwareAndCalibrationIDsOperationCompleted == null)
			{
				GetUpdateFirmwareAndCalibrationIDsOperationCompleted = OnGetUpdateFirmwareAndCalibrationIDsOperationCompleted;
			}
			InvokeAsync("GetUpdateFirmwareAndCalibrationIDs", new object[6]
			{
				emailAddress,
				password,
				productIdentifier,
				serialNumber,
				deviceFirmwareVersion,
				deviceCalibrationVersion
			}, GetUpdateFirmwareAndCalibrationIDsOperationCompleted, userState);
		}

		private void OnGetUpdateFirmwareAndCalibrationIDsOperationCompleted(object arg)
		{
			if (this.GetUpdateFirmwareAndCalibrationIDsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetUpdateFirmwareAndCalibrationIDsCompleted(this, new GetUpdateFirmwareAndCalibrationIDsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetListOfFilesToSendToDevice", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string GetListOfFilesToSendToDevice(string emailAddress, string password, string deviceSerialNumber)
		{
			object[] array = Invoke("GetListOfFilesToSendToDevice", new object[3]
			{
				emailAddress,
				password,
				deviceSerialNumber
			});
			return (string)array[0];
		}

		public void GetListOfFilesToSendToDeviceAsync(string emailAddress, string password, string deviceSerialNumber)
		{
			GetListOfFilesToSendToDeviceAsync(emailAddress, password, deviceSerialNumber, null);
		}

		public void GetListOfFilesToSendToDeviceAsync(string emailAddress, string password, string deviceSerialNumber, object userState)
		{
			if (GetListOfFilesToSendToDeviceOperationCompleted == null)
			{
				GetListOfFilesToSendToDeviceOperationCompleted = OnGetListOfFilesToSendToDeviceOperationCompleted;
			}
			InvokeAsync("GetListOfFilesToSendToDevice", new object[3]
			{
				emailAddress,
				password,
				deviceSerialNumber
			}, GetListOfFilesToSendToDeviceOperationCompleted, userState);
		}

		private void OnGetListOfFilesToSendToDeviceOperationCompleted(object arg)
		{
			if (this.GetListOfFilesToSendToDeviceCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetListOfFilesToSendToDeviceCompleted(this, new GetListOfFilesToSendToDeviceCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetListOfFilesToSendToDevice2", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string GetListOfFilesToSendToDevice2(string emailAddress, string password, string deviceSerialNumber, bool endFiles)
		{
			object[] array = Invoke("GetListOfFilesToSendToDevice2", new object[4]
			{
				emailAddress,
				password,
				deviceSerialNumber,
				endFiles
			});
			return (string)array[0];
		}

		public void GetListOfFilesToSendToDevice2Async(string emailAddress, string password, string deviceSerialNumber, bool endFiles)
		{
			GetListOfFilesToSendToDevice2Async(emailAddress, password, deviceSerialNumber, endFiles, null);
		}

		public void GetListOfFilesToSendToDevice2Async(string emailAddress, string password, string deviceSerialNumber, bool endFiles, object userState)
		{
			if (GetListOfFilesToSendToDevice2OperationCompleted == null)
			{
				GetListOfFilesToSendToDevice2OperationCompleted = OnGetListOfFilesToSendToDevice2OperationCompleted;
			}
			InvokeAsync("GetListOfFilesToSendToDevice2", new object[4]
			{
				emailAddress,
				password,
				deviceSerialNumber,
				endFiles
			}, GetListOfFilesToSendToDevice2OperationCompleted, userState);
		}

		private void OnGetListOfFilesToSendToDevice2OperationCompleted(object arg)
		{
			if (this.GetListOfFilesToSendToDevice2Completed != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetListOfFilesToSendToDevice2Completed(this, new GetListOfFilesToSendToDevice2CompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/SetFileAsSent", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void SetFileAsSent(string emailAddress, string password, int deviceSendFileId)
		{
			Invoke("SetFileAsSent", new object[3]
			{
				emailAddress,
				password,
				deviceSendFileId
			});
		}

		public void SetFileAsSentAsync(string emailAddress, string password, int deviceSendFileId)
		{
			SetFileAsSentAsync(emailAddress, password, deviceSendFileId, null);
		}

		public void SetFileAsSentAsync(string emailAddress, string password, int deviceSendFileId, object userState)
		{
			if (SetFileAsSentOperationCompleted == null)
			{
				SetFileAsSentOperationCompleted = OnSetFileAsSentOperationCompleted;
			}
			InvokeAsync("SetFileAsSent", new object[3]
			{
				emailAddress,
				password,
				deviceSendFileId
			}, SetFileAsSentOperationCompleted, userState);
		}

		private void OnSetFileAsSentOperationCompleted(object arg)
		{
			if (this.SetFileAsSentCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetFileAsSentCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/SetDeviceHasCustomTuning", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void SetDeviceHasCustomTuning(string email, string pass, string serialNumber, string customTuningBrand)
		{
			Invoke("SetDeviceHasCustomTuning", new object[4]
			{
				email,
				pass,
				serialNumber,
				customTuningBrand
			});
		}

		public void SetDeviceHasCustomTuningAsync(string email, string pass, string serialNumber, string customTuningBrand)
		{
			SetDeviceHasCustomTuningAsync(email, pass, serialNumber, customTuningBrand, null);
		}

		public void SetDeviceHasCustomTuningAsync(string email, string pass, string serialNumber, string customTuningBrand, object userState)
		{
			if (SetDeviceHasCustomTuningOperationCompleted == null)
			{
				SetDeviceHasCustomTuningOperationCompleted = OnSetDeviceHasCustomTuningOperationCompleted;
			}
			InvokeAsync("SetDeviceHasCustomTuning", new object[4]
			{
				email,
				pass,
				serialNumber,
				customTuningBrand
			}, SetDeviceHasCustomTuningOperationCompleted, userState);
		}

		private void OnSetDeviceHasCustomTuningOperationCompleted(object arg)
		{
			if (this.SetDeviceHasCustomTuningCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetDeviceHasCustomTuningCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://edgeproducts.inc/GetMaxRequestLength", RequestNamespace = "http://edgeproducts.inc/", ResponseNamespace = "http://edgeproducts.inc/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public long GetMaxRequestLength()
		{
			object[] array = Invoke("GetMaxRequestLength", new object[0]);
			return (long)array[0];
		}

		public void GetMaxRequestLengthAsync()
		{
			GetMaxRequestLengthAsync(null);
		}

		public void GetMaxRequestLengthAsync(object userState)
		{
			if (GetMaxRequestLengthOperationCompleted == null)
			{
				GetMaxRequestLengthOperationCompleted = OnGetMaxRequestLengthOperationCompleted;
			}
			InvokeAsync("GetMaxRequestLength", new object[0], GetMaxRequestLengthOperationCompleted, userState);
		}

		private void OnGetMaxRequestLengthOperationCompleted(object arg)
		{
			if (this.GetMaxRequestLengthCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetMaxRequestLengthCompleted(this, new GetMaxRequestLengthCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		private bool IsLocalFileSystemWebService(string url)
		{
			if (url == null || url == string.Empty)
			{
				return false;
			}
			Uri uri = new Uri(url);
			if (uri.Port >= 1024 && string.Compare(uri.Host, "localHost", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			return false;
		}
	}
}
