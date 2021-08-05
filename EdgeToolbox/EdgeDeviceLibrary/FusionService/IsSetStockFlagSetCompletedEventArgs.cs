using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace EdgeDeviceLibrary.FusionService
{
	[GeneratedCode("System.Web.Services", "4.7.2556.0")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class IsSetStockFlagSetCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public bool Result
		{
			get
			{
				RaiseExceptionIfNecessary();
				return (bool)results[0];
			}
		}

		internal IsSetStockFlagSetCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
