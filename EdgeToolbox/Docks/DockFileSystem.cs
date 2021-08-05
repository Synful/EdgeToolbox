using Common;
using DarkUI.Controls;
using DarkUI.Docking;
using DarkUI.Forms;
using Edge.Common.DataStructures;
using EdgeDeviceLibrary.Communicator;
using EdgeToolbox.Classes;
using EdgeToolbox.Properties;
using ProteusCommand.MiniAppCommands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace EdgeToolbox {
    public partial class DockFileSystem : DarkToolWindow {
		private class FileTreeComparer : IComparer<TreeNode<F_FIND>> {
			private static FileTreeComparer _comparer = null;

			public static FileTreeComparer Instance {
				get {
					if(_comparer == null) {
						_comparer = new FileTreeComparer();
					}
					return _comparer;
				}
			}

			public int Compare(TreeNode<F_FIND> lhs, TreeNode<F_FIND> rhs) {
				if(lhs.Value.IsDirectory != rhs.Value.IsDirectory) {
					if(lhs.Value.IsDirectory) {
						return -1;
					}
					return 1;
				}
				return string.Compare(lhs.Value.Filename, rhs.Value.Filename);
			}
		}
		public class FileSystemInfo {

			public string Dir;
			public string[] Files;
			public FileSystemInfo(string Dir, string[] Files) {
				this.Dir = Dir;
				this.Files = Files;
			}

		}

		Proteus p;

		private static DockFileSystem _instance;
		public static DockFileSystem Instance() {
			if(_instance == null) {
				_instance = new DockFileSystem();
            }
			return _instance;
		}

		public DockFileSystem() {
			InitializeComponent();

			p = Proteus.Instance();
        }
		public FileSystemInfo RecurseRetrieveFileEntries(string Dir) {
			if(p.DirActionOnDevice(DirActions.ChDir, encrypted: false, Dir).ErrorCode == 0) {
				Cmd_MA_Dir cmd_MA_Dir = p.LoadDir(encrypted: false, "*.*");
				F_FIND[] entries = cmd_MA_Dir.Entries;
				List<string> Files = new List<string>();
				foreach(F_FIND f_FIND in entries) {
					if(f_FIND.IsFile) {
                        Files.Add(f_FIND.Filename);
					}
				}
				p.DirActionOnDevice(DirActions.ChDir, encrypted: false, "..");
				return new FileSystemInfo(Dir, Files.ToArray());
			}
			return null;
		}
		private List<FileSystemInfo> FATGetFileList() {
			List<FileSystemInfo> list = new List<FileSystemInfo>();

			p.DirActionOnDevice(DirActions.ChDir, encrypted: false, "\\");

			Cmd_MA_Dir cmd_MA_Dir = p.LoadDir(encrypted: false, "*.*");
			List<string> Files = new List<string>();
			foreach(F_FIND f_FIND in cmd_MA_Dir.Entries) {
				if(f_FIND.IsFile) {
					Files.Add(f_FIND.Filename);
				} else if(f_FIND.IsRealDirectory) {
					FileSystemInfo FSI = RecurseRetrieveFileEntries(f_FIND.Filename);
					if(FSI != null) {
						list.Add(RecurseRetrieveFileEntries(f_FIND.Filename));
					}
				}
			}
			if(Files.Count > 0) {
				list.Add(new FileSystemInfo("", Files.ToArray()));
			}
			return list;
		}

		public void refreshBtn_Click(object sender, System.EventArgs e) {
			if(p == null) {
				p = Proteus.Instance();
			}
			LoadListWorker.RunWorkerAsync();
        }

        private void LoadListWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
			Invoke((MethodInvoker)delegate {
				p.ChangeDirectory("\\");
				List<FileSystemInfo> files = FATGetFileList();
                foreach(FileSystemInfo file in files) {
                    if(file.Dir != "") {
                        var node = new DarkTreeNode($"{file.Dir}");
						node.ExpandedIcon = Resources.folder_open;
                        node.Icon = Resources.folder_closed;

						foreach(string fileN in file.Files) {
							var childNode = new DarkTreeNode($"{fileN}");
							childNode.Icon = Resources.document_16xLG;
							node.Nodes.Add(childNode);
						}

                        fileView.Nodes.Add(node);
                    } else {
						foreach(string fileN in file.Files) {
							var node = new DarkTreeNode($"{fileN}");
							node.Icon = Resources.document_16xLG;
							fileView.Nodes.Add(node);
						}
                    }
                }
            });
		}
        private void LoadListWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
			//DarkMessageBox.ShowInformation(this.ParentForm, "Finished Loading Files.", "Info");
        }

        private void fileView_SelectedNodesChanged(object sender, System.EventArgs e) {
			DarkTreeView view = (DarkTreeView)sender;
			if(view.SelectedNodes.Count > 0) {
				DarkTreeNode selNode = view.SelectedNodes[0];
				DarkTreeNode parentNode = selNode.ParentNode;
				if(parentNode != null) {
					p.ChangeDirectory($"\\{parentNode.Text}\\");
					Cmd_MA_ReadFile file = p.ReadFileFromDevice(false, selNode.Text);
					if(file != null) {
						if(selNode.Text.EndsWith(".txt")) {
							MainForm.Instance().DockPanel.AddContent(new DockTextEditor(file.Filename, file.FileData));
						}
						if(selNode.Text.EndsWith(".xml")) {
							MainForm.Instance().DockPanel.AddContent(new DockXmlEditor($"\\{parentNode.Text}\\", file.Filename));
                        }
					}
				} else {
					p.ChangeDirectory("\\");
					Cmd_MA_ReadFile file = p.ReadFileFromDevice(false, selNode.Text);
					if(file != null) {
						if (selNode.Text.EndsWith(".txt")) {
							MainForm.Instance().DockPanel.AddContent(new DockTextEditor(file.Filename, file.FileData));
						}
						if (selNode.Text.EndsWith(".xml")) {
							MainForm.Instance().DockPanel.AddContent(new DockXmlEditor("\\", file.Filename));
						}
					}
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
	}
}
