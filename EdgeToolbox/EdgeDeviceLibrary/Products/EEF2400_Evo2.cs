using System;
using System.ComponentModel;

namespace EdgeDeviceLibrary.Products
{
	internal class EEF2400_Evo2 : Product_Base
	{
		public override void PerformDeviceConnectionTasks(BackgroundWorker bw)
		{
			UploadVehicleUserIdentifierFile("vid.bin");
			UploadVehicleUserIdentifierFile("cs_info.bin");
		}

		public override void ValidateStockFiles()
		{
			throw new Exception("Old EVO2's don't support stock file validation");
		}
	}
}
