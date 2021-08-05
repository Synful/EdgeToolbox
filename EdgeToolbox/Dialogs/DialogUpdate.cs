using DarkUI.Forms;
using EdgeDeviceLibrary;
using EdgeDeviceLibrary.Communicator;
using ProteusCommand;
using System;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace EdgeToolbox {
    public partial class DialogUpdate : DarkDialog {

        Proteus p;
		bool HasErasedExternalFlash;
		private DeviceConnector.ProgressReportEventHandler _progressHandler;
		private DeviceConnector.ProgressReportEventHandler _overallProgressHandler;

		public DialogUpdate() {
            InitializeComponent();
            p = Proteus.Instance();
            btnOk.Text = "Close";
			pActL.Text = "Step 1 of 4: Downloading firmware from the Internet";
			sActL.Text = "Downloading Firmware...";
			_progressHandler = UploadFmAndCal_ProgressChanged;
			_overallProgressHandler = UploadFmAndCal_FileProgressChanged;
			DeviceConnector.Instance().ProgressReportEvent += _progressHandler;
			DeviceConnector.Instance().OverallProgressReportEvent += _overallProgressHandler;
			UpdaterBG.RunWorkerAsync();
		}
		private void DialogUpdate_FormClosing(object sender, FormClosingEventArgs e) {
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			if (_progressHandler != null) {
				deviceConnector.ProgressReportEvent -= _progressHandler;
			}
			if (_overallProgressHandler != null) {
				deviceConnector.OverallProgressReportEvent -= _overallProgressHandler;
			}
		}

		private void UpdateDownloader_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			if (base.InvokeRequired) {
				BeginInvoke((MethodInvoker)delegate {
					UpdateDownloader_ProgressChanged(sender, e);
				});
			} else {
				progBar.Value = e.ProgressPercentage;
			}
		}

		private bool Download(string filename, long offset, ref MemoryStream ms) {
			UpdateDownloader.AutoSetChunkSize = true;
			UpdateDownloader.RemoteFileName = filename;
			UpdateDownloader.LocalSaveFolder = "Data";
			UpdateDownloader.IncludeHashVerification = true;
			UpdateDownloader.RunWorkerSync(ref ms);
			return !UpdateDownloader.CancellationPending;
		}

		private void UploadFmAndCal(ref MemoryStream ms, bool IsFirmware) {
			DeviceConnector deviceConnector = DeviceConnector.Instance();
			if (deviceConnector.communicator.IsFileFormatEncryptedS19) {
				deviceConnector.communicator.SendEncryptedS19File(ref ms);
				return;
			}
			if (IsFirmware) {
				try {
					if (!deviceConnector.communicator.SendEncryptedBinFile(ref ms, 24576u, 70)) {
						throw new Exception("Unable to update firmware");
					}
				} catch (Exception ex) {
					deviceConnector.Log("Unable to update firmware - " + ex.Message);
					throw new Exception("Unable to update firmware: " + ex.Message, ex);
				}
				return;
			}
			try {
				if (!deviceConnector.communicator.SendEncryptedBinFile(ref ms, 0u, 67)) {
					throw new Exception("Unable to update calibration");
				}
			} catch (Exception ex2) {
				deviceConnector.Log("Unable to update calibration - " + ex2.Message);
				throw new Exception("Unable to update calibration: " + ex2.Message, ex2);
			}
		}
		private void UploadFmAndCal_ProgressChanged(object sender, DeviceConnector.ProgressReportEventArgs e) {
			bool flag = false;
			if (e.status != null && e.task != null && sActL.Text.CompareTo(e.status) != 0) {
				sActL.Text = e.status;
				if (sActL.Text == "Please wait...") {
					progBar.Style = ProgressBarStyle.Marquee;
					HasErasedExternalFlash = true;
				} else {
					progBar.Style = ProgressBarStyle.Blocks;
					if (HasErasedExternalFlash && pActL.Text == "Step 4 of 5: Clearing calibration") {
						pActL.Text = "Step 5 of 5: Uploading calibration";
					}
				}
				Application.DoEvents();
			}
			if (e.percentComplete != progBar.Value) {
				progBar.Value = e.percentComplete;
				Application.DoEvents();
			}
		}
		private void UploadFmAndCal_FileProgressChanged(object sender, DeviceConnector.ProgressReportEventArgs e) {
			if (fileLabel.Text.CompareTo(e.task) != 0) {
				fileLabel.Text = e.task;
				Application.DoEvents();
			}
			if (progBar2.Style != ProgressBarStyle.Continuous) {
				progBar2.Style = ProgressBarStyle.Continuous;
				Application.DoEvents();
			}
			if (progBar2.Value != e.percentComplete) {
				progBar2.Value = e.percentComplete;
				Application.DoEvents();
			}
		}

		private void RunUpdater() {
			DeviceConnector dc = DeviceConnector.Instance();
			try {
				if (dc.communicator.HasSkuChanged()) {
					pActL.Text = "Preparing for upgrade";
					sActL.Text = "Cleaning up files...";
					Refresh();
					progBar2.Style = ProgressBarStyle.Marquee;
					Thread thread = new Thread((ThreadStart)delegate {
						dc.communicator.PrepareForSkuChange();
					});
					thread.Name = "Prepare For Upgrade Thread";
					thread.IsBackground = true;
					thread.Start();
					while (thread.ThreadState != ThreadState.Stopped) {
						Application.DoEvents();
						Thread.Sleep(50);
					}
					thread.Join();
					dc.ResetProgressBar();
				}
				pActL.Text = "Step 1 of 5: Downloading firmware from the Internet";
				sActL.Text = "Downloading Firmware...";
				Refresh();
				MemoryStream ms = new MemoryStream();
				if (!Download(dc.FirmwareIDTargetPath, 0L, ref ms)) {
					return;
				}
				dc.ResetProgressBar();
				pActL.Text = "Step 2 of 5: Downloading calibration from the Internet";
				sActL.Text = "Downloading Calibration...";
				Refresh();
				MemoryStream msCalibration = new MemoryStream();
				progBar2.Style = ProgressBarStyle.Marquee;
				bool CalDownloaded = false;
				Thread thread2 = new Thread((ThreadStart)delegate {
					CalDownloaded = Download(dc.CalibrationIDTargetPath, 0L, ref msCalibration);
				});
				thread2.Name = "Calibration Download Thread";
				thread2.IsBackground = true;
				thread2.Start();
				while (thread2.ThreadState != ThreadState.Stopped) {
					Application.DoEvents();
					Thread.Sleep(50);
				}
				thread2.Join();
				progBar2.Style = ProgressBarStyle.Marquee;
				if (!CalDownloaded) {
					return;
				}
				dc.ResetProgressBar();
				pActL.Text = "Step 3 of 5: Preparing and transferring firmware";
				UploadFmAndCal(ref ms, IsFirmware: true);
				dc.ResetProgressBar();
				pActL.Text = "Step 4 of 5: Clearing calibration";
				Refresh();
				UploadFmAndCal(ref msCalibration, IsFirmware: false);
				dc.Log("Firmware and Calibration");
				if (dc.communicator.IsMakeStockWriterSet()) {
					if (dc.communicator.RestoreSettings()) {
						dc.Log("Settings restored");
					} else {
						dc.Log("Tried to restore settings, but failed.  This is usually caused by blank settings.");
					}
					if (dc.communicator.SetProgrammingInterruptedFlag()) {
						dc.Log("Set ProgrammingInterruptedFlag");
					} else {
						dc.Log("Tried to set the WriteStock flag, but failed.  This is usually caused by blank settings.");
					}
					dc.communicator.ClearMakeStockWriter();
				}
                if (dc.communicator.IsResetNecessary) {
                    bool flag = false;
                        Application.DoEvents();
                        try {
                            flag = dc.communicator.ResetDevice(60000);
                        } catch {
                            flag = false;
                        }
                    for (int i = 0; i < 2; i++) {
                        if (flag && !dc.communicator.IsUnplugResetNecessary) {
                            continue;
                        }
                        Application.DoEvents();
                        SystemSounds.Beep.Play();
                        try {
                            flag = dc.communicator.WaitForDeviceReconnect(flag ? new int?(60000) : null);
                        } catch {
                            flag = false;
                        }
                    }
                }
                dc.SetUpdateComplete();
			} catch (ProteusCmdException ex) {
				if (ex.Category == ProteusCmdErrorCategory.Command && ex.Error == ProteusCmdError.InvalidCommand) {
					throw new ApplicationException("The device may be in demo mode and/or still connected to the vehicle.  If connected to the vehicle, please disconnect the device from the vehicle and the computer then reconnect the device to the computer and reopen Fusion.  If in demo mode, please disconnect and reconnect the device making sure to avoid pressing a button or touching the screen then reopen Fusion.", ex);
				}
			}
		}

        private void UpdaterBG_DoWork(object sender, DoWorkEventArgs e) {
			RunUpdater();
        }
        private void UpdaterBG_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			sActL.Text = "Finalizing...";
			Application.DoEvents();
			Thread.Sleep(5000);
			Close();
		}
    }
}
