using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace EdgeDeviceLibrary.FusionService
{
	[GeneratedCode("System.Web.Services", "4.7.2556.0")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class FinalizeUploadTransferCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public string Result
		{
			get
			{
				RaiseExceptionIfNecessary();
				return (string)results[0];
			}
		}

		internal FinalizeUploadTransferCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
