using System;
using System.IO;
using System.Text;
using ProteusCommand.MiniAppCommands;

namespace EdgeDeviceLibrary
{
	public class Debugging
	{
		public static readonly object SyncRoot = new object();

		public static TextWriter FileWriter;

		public static readonly Debugging Instance = new Debugging();

		public void OpenDebugLog()
		{
			if (FileWriter != null)
			{
				return;
			}
			lock (SyncRoot)
			{
				if (FileWriter == null)
				{
					string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"Fusion\\{DateTime.Now:yyyy-MM-dd}.log");
					Directory.CreateDirectory(Path.GetDirectoryName(path));
					FileStream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
					fileStream.Seek(0L, SeekOrigin.End);
					FileWriter = new StreamWriter(fileStream);
				}
			}
		}

		public void CloseDebugLog()
		{
			if (FileWriter == null)
			{
				return;
			}
			lock (SyncRoot)
			{
				if (FileWriter != null)
				{
					FileWriter.Close();
					FileWriter = null;
				}
			}
		}

		public void WriteLogEntry(string entry)
		{
			if (FileWriter == null)
			{
				OpenDebugLog();
			}
			string arg = string.Format("{0} ::> ", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.FFFF tt zzz"));
			lock (SyncRoot)
			{
				FileWriter.WriteLine("{0}{1}", arg, entry);
				FileWriter.Flush();
			}
		}

		public void DumpCommandData(Cmd_MA_WriteFile command)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(">>>>>>>>> Transfer File: " + command.FileName + "<<<<<<<<<<");
			for (int i = 0; i < command.CommandPages.Length; i++)
			{
				stringBuilder.AppendLine("Command #" + (i + 1));
				stringBuilder.AppendLine(WriteHexValues(command.CommandPages[i]));
				stringBuilder.AppendLine("------------------------------------------------------------------------------------");
				stringBuilder.AppendLine("Response #" + (i + 1));
				stringBuilder.AppendLine(WriteHexValues(command.ResponsePages[i]));
				stringBuilder.AppendLine("------------------------------------------------------------------------------------");
			}
			stringBuilder.AppendLine("<<<<<<<<>>>>>>>>");
			WriteLogEntry(stringBuilder.ToString());
		}

		private string WriteHexValues(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				stringBuilder.Append(bytes[i].ToString("X2") + " ");
				if ((i + 1) % 32 == 0)
				{
					stringBuilder.AppendLine();
				}
			}
			return stringBuilder.ToString();
		}
	}
}
