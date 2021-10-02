using Common;
using DarkUI.Forms;
using Edge.Imaging;
using EdgeDeviceLibrary;
using EdgeDeviceLibrary.Communicator;
using ProteusCommand;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace EdgeToolbox {
    public partial class DialogFileDownloader : DarkDialog {

        Proteus p;
		private DeviceConnector.ProgressReportEventHandler _progressHandler;
		private DeviceConnector.ProgressReportEventHandler _overallProgressHandler;

		public DialogFileDownloader() {
            InitializeComponent();
			CheckForIllegalCrossThreadCalls = false;
			p = Proteus.Instance();
            btnOk.Text = "Close";
			_progressHandler = FileDownload_ProgressChanged;
			_overallProgressHandler = FileDownload_FileProgressChanged;
			DeviceConnector.Instance().ProgressReportEvent += _progressHandler;
			DeviceConnector.Instance().OverallProgressReportEvent += _overallProgressHandler;
			DownloaderBG.RunWorkerAsync();
		}
		private void DialogFileDownloader_FormClosing(object sender, FormClosingEventArgs e) {
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			if (_progressHandler != null) {
				deviceConnector.ProgressReportEvent -= _progressHandler;
			}
			if (_overallProgressHandler != null) {
				deviceConnector.OverallProgressReportEvent -= _overallProgressHandler;
			}
		}

		private void FileDownload_FileProgressChanged(object sender, DeviceConnector.ProgressReportEventArgs e) {
			if (base.IsDisposed) {
				return;
			}
			try {
				if (e.percentComplete > 0 || e.task != string.Empty) {
					progressOverall.Value = e.percentComplete;
					if (e.task != null) {
						labelOverallTask.Text = e.task;
					}
				}
				Refresh();
				Application.DoEvents();
			} catch (NullReferenceException) { }
		}
		private void FileDownload_ProgressChanged(object sender, DeviceConnector.ProgressReportEventArgs e) {
			if (base.InvokeRequired) {
				BeginInvoke((MethodInvoker)delegate {
					FileDownload_ProgressChanged(sender, e);
				});
			} else {
				if (base.IsDisposed) {
					return;
				}
				try {
					if (e.percentComplete > 0) {
						progressTask.Value = ((e.percentComplete > 100) ? 100 : e.percentComplete);
						if (e.status != null && e.task != null) {
							labelFileProg.Text = e.task;
						}
					}
					Refresh();
					Application.DoEvents();
				} catch (NullReferenceException) {
				}
			}
		}

		private Bitmap ConvertDDBtoBMP(byte[] fileData) {
			Bitmap result = null;
			if (fileData != null) {
				ushort num = BinaryHelper.ReadLittleEndianShort(fileData, 0);
				BinaryHelper.ReadLittleEndianShort(fileData, 2);
				int num2 = (int)BinaryHelper.ReadLittleEndianShort(fileData, 4);
				IntPtr intPtr = IntPtr.Zero;
				PixelFormat format = PixelFormat.Format16bppRgb565;
				int num3 = 2;
				if ((num2 & 3) == 3) {
					format = PixelFormat.Format32bppArgb;
					num3 = 4;
				}
				try {
					intPtr = Marshal.AllocHGlobal(fileData.Length - 8);
					Marshal.Copy(fileData, 8, intPtr, fileData.Length - 8);
					using (Bitmap bitmap = new Bitmap((int)num, (int)BinaryHelper.ReadLittleEndianShort(fileData, 2), (int)num * num3, format, intPtr)) {
						result = Image.FromHbitmap(bitmap.GetHbitmap());
					}
				} finally {
					if (intPtr != IntPtr.Zero) {
						Marshal.FreeHGlobal(intPtr);
					}
				}
			}
			return result;
		}
		private void ConvertBMPtoDDB(Bitmap bmp, Stream outputStream) {
			if (bmp != null) {
				ushort num = (ushort)bmp.Width;
				ushort num2 = (ushort)bmp.Height;
				ushort value = 32770;
				Rectangle rect = new Rectangle(0, 0, (int)num, (int)num2);
				BitmapData bitmapData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format16bppRgb565);
				int num3 = bitmapData.Stride * bitmapData.Height;
				byte[] array = new byte[num3];
				Marshal.Copy(bitmapData.Scan0, array, 0, num3);
				bmp.UnlockBits(bitmapData);
				byte[] array2 = new byte[array.Length + 8];
				BinaryHelper.WriteLittleEndianShort(array2, 0, num);
				BinaryHelper.WriteLittleEndianShort(array2, 2, num2);
				BinaryHelper.WriteLittleEndianShort(array2, 4, value);
				Buffer.BlockCopy(array, 0, array2, 8, array.Length);
				outputStream.Write(array2, 0, array2.Length);
			}
		}

		void DumpFiles() {
			Invoke((MethodInvoker)delegate {
				if(p != null) {
					progressOverall.Value = 0;
					string workingDir = $"{AppDomain.CurrentDomain.BaseDirectory}Files/{p.GetPartNumber(true)}/Storage";
					p.ChangeDirectory("\\");
					string[] files = p.FATGetFileList();
					progressOverall.Maximum = files.Length;
					foreach(string file in files) {
						labelFilename.Text = file.Split(',')[0];
						MemoryStream mem = new MemoryStream();
						string[] info = file.Split(',')[0].Split('\\');
						if(info.Length > 1) {
							p.ChangeDirectory($"\\{info[0]}\\");
							p.ReadFat($"{info[1]}", true, true, ref mem);
							if(!Directory.Exists($"{workingDir}/{info[0]}")) {
								Directory.CreateDirectory($"{workingDir}/{info[0]}");
							}
							if(info[1].EndsWith(".ddb")) {
								File.WriteAllBytes($"{workingDir}/{info[0]}/{info[1]}", mem.ToArray());
								Bitmap bitmap = ConvertDDBtoBMP(mem.ToArray());
								MemoryStream bitStream = new MemoryStream();
								bitmap.Save(bitStream, ImageFormat.Bmp);
								File.WriteAllBytes($"{workingDir}/{info[0]}/{info[1].Replace(".ddb", ".bmp")}", bitStream.ToArray());
								bitmap.Dispose();
							} else if(info[1].EndsWith(".pim")) {
								File.WriteAllBytes($"{workingDir}/{info[0]}/{info[1]}", mem.ToArray());
								Bitmap bitmap = BitmapToPim.PimToBitmap(mem.ToArray());
								MemoryStream bitStream = new MemoryStream();
								bitmap.Save(bitStream, ImageFormat.Bmp);
								File.WriteAllBytes($"{workingDir}/{info[0]}/{info[1].Replace(".pim", ".bmp")}", bitStream.ToArray());
								bitmap.Dispose();
							} else {
								File.WriteAllBytes($"{workingDir}/{info[0]}/{info[1]}", mem.ToArray());
							}
						} else {
							p.ChangeDirectory("\\");
							p.ReadFat($"{info[0]}", true, true, ref mem);
							File.WriteAllBytes($"{workingDir}/{info[0]}", mem.ToArray());
						}
						progressOverall.Value++;
					}
					Application.DoEvents();
				}
			});
		}
		private void DownloaderBG_DoWork(object sender, DoWorkEventArgs e) {
			DumpFiles();
		}
        private void DownloaderBG_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			labelFilename.Text = "Finalizing...";
			Application.DoEvents();
			Thread.Sleep(5000);
			Close();
		}
    }
}
