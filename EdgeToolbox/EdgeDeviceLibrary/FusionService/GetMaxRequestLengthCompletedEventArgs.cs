using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace EdgeDeviceLibrary.FusionService
{
	[GeneratedCode("System.Web.Services", "4.7.2556.0")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class GetMaxRequestLengthCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public long Result
		{
			get
			{
				RaiseExceptionIfNecessary();
				return (long)results[0];
			}
		}

		internal GetMaxRequestLengthCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
