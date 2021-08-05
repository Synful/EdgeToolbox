using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace EdgeDeviceLibrary.FusionService
{
	[GeneratedCode("System.Web.Services", "4.7.2556.0")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class GetModelFromUserSerialNumCompletedEventArgs : AsyncCompletedEventArgs
	{
		private object[] results;

		public int Result
		{
			get
			{
				RaiseExceptionIfNecessary();
				return (int)results[0];
			}
		}

		internal GetModelFromUserSerialNumCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState)
			: base(exception, cancelled, userState)
		{
			this.results = results;
		}
	}
}
