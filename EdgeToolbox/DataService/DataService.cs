using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using EdgeToolbox.Properties;

namespace EdgeToolbox
{
	[GeneratedCode("System.Web.Services", "4.7.2556.0")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[WebServiceBinding(Name = "DataService", Namespace = "http://tempuri.org/")]
	public class DataService : SoapHttpClientProtocol
	{
		private SendOrPostCallback QueryOperationCompleted;

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

		public event QueryCompletedEventHandler QueryCompleted;

		public DataService()
		{
			Url = "https://cloud.edgeproducts.com/DataService.asmx";
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

		[SoapDocumentMethod("http://tempuri.org/Query", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string Query(string token, string jsonQuery)
		{
			return (string)Invoke("Query", new object[2]
			{
				token,
				jsonQuery
			})[0];
		}

		public void QueryAsync(string token, string jsonQuery)
		{
			QueryAsync(token, jsonQuery, null);
		}

		public void QueryAsync(string token, string jsonQuery, object userState)
		{
			if (QueryOperationCompleted == null)
			{
				QueryOperationCompleted = OnQueryOperationCompleted;
			}
			InvokeAsync("Query", new object[2]
			{
				token,
				jsonQuery
			}, QueryOperationCompleted, userState);
		}

		private void OnQueryOperationCompleted(object arg)
		{
			if (this.QueryCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.QueryCompleted(this, new QueryCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
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
