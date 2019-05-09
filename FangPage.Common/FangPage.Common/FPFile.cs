using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;

namespace FangPage.Common
{
	public class FPFile
	{
		public static string GetMapPath(string virPath)
		{
			if (HttpContext.Current != null)
			{
				return HttpContext.Current.Server.MapPath(virPath);
			}
			return GetMapPath(AppDomain.CurrentDomain.BaseDirectory, virPath);
		}

		public static string GetMapPath(string strPath, string virPath)
		{
			virPath = virPath.Replace("/", "\\");
			strPath = strPath.TrimEnd('\\');
			while (virPath.StartsWith("..\\"))
			{
				if (strPath != "")
				{
					strPath = Path.GetDirectoryName(strPath);
				}
				virPath = virPath.Substring(3);
			}
			if (virPath != "")
			{
				strPath = Combine(strPath, virPath).TrimEnd('\\');
			}
			return strPath;
		}

		public static string GetExt(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				return "";
			}
			if (filename.LastIndexOf(".") > 0)
			{
				return filename.Substring(filename.LastIndexOf(".") + 1);
			}
			return "";
		}

		public static string ReadFile(string filename)
		{
			if (!File.Exists(filename))
			{
				return "";
			}
			using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
				{
					return streamReader.ReadToEnd();
				}
			}
		}

		public static void WriteFile(string filename, string content)
		{
			if (!Directory.Exists(Path.GetDirectoryName(filename)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(filename));
			}
			if (File.Exists(filename) && File.GetAttributes(filename).ToString().IndexOf("ReadOnly") != -1)
			{
				File.SetAttributes(filename, FileAttributes.Normal);
			}
			using (FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(content);
				fileStream.Write(bytes, 0, bytes.Length);
				fileStream.Close();
			}
		}

		public static void AppendFile(string filename, string content)
		{
			if (!Directory.Exists(Path.GetDirectoryName(filename)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(filename));
			}
			if (File.Exists(filename) && File.GetAttributes(filename).ToString().IndexOf("ReadOnly") != -1)
			{
				File.SetAttributes(filename, FileAttributes.Normal);
			}
			if (!File.Exists(filename))
			{
				using (FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
				{
					StreamWriter streamWriter = new StreamWriter(fileStream);
					streamWriter.WriteLine(content);
					streamWriter.Close();
					fileStream.Close();
				}
			}
			else
			{
				using (FileStream fileStream2 = new FileStream(filename, FileMode.Append, FileAccess.Write))
				{
					StreamWriter streamWriter2 = new StreamWriter(fileStream2);
					streamWriter2.WriteLine(content);
					streamWriter2.Close();
					fileStream2.Close();
				}
			}
		}

		public static void FileCoppy(string orignFile, string newFile)
		{
			if (!string.IsNullOrEmpty(orignFile) && !string.IsNullOrEmpty(newFile) && File.Exists(orignFile))
			{
				if (!Directory.Exists(Path.GetDirectoryName(newFile)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(newFile));
				}
				File.Copy(orignFile, newFile, true);
			}
		}

		public static void FileDel(string path)
		{
			if (!string.IsNullOrEmpty(path) && File.Exists(path))
			{
				File.Delete(path);
			}
		}

		public static void FileMove(string orignFile, string newFile)
		{
			if (!string.IsNullOrEmpty(orignFile) && !string.IsNullOrEmpty(newFile) && File.Exists(orignFile))
			{
				if (!Directory.Exists(Path.GetDirectoryName(newFile)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(newFile));
				}
				File.Move(orignFile, newFile);
			}
		}

		public static void CreateDir(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException(path);
			}
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public static void CopyDir(string sourcePath, string targetPath)
		{
			if (Directory.Exists(sourcePath))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(sourcePath);
				if (!Directory.Exists(targetPath))
				{
					Directory.CreateDirectory(targetPath);
				}
				FileInfo[] files = directoryInfo.GetFiles();
				foreach (FileInfo fileInfo in files)
				{
					fileInfo.CopyTo(targetPath + "\\" + fileInfo.Name, true);
				}
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					CopyDir(directoryInfo2.FullName, targetPath + "\\" + directoryInfo2.Name);
				}
			}
		}

		public static string Combine(string path1, string path2)
		{
			char c = '/';
			if (path1.IndexOf('\\') >= 0)
			{
				path1 = path1.Replace("/", "\\");
				path2 = path2.Replace("/", "\\");
				c = '\\';
			}
			else
			{
				path1 = path1.Replace("\\", "/");
				path2 = path2.Replace("\\", "/");
			}
			if (path1 == "")
			{
				return path2;
			}
			if (path2 == "")
			{
				return path1;
			}
			path1 = path1.TrimEnd(c);
			path2 = path2.TrimStart(c);
			return path1 + c.ToString() + path2;
		}

		public static long GetDirSize(string dirPath)
		{
			long num = 0L;
			if (!Directory.Exists(dirPath))
			{
				return num;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				num += fileInfo.Length;
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				num += GetDirSize(directoryInfo2.FullName);
			}
			return num;
		}

		public static void DeleteDir(string dir)
		{
			if (string.IsNullOrEmpty(dir))
			{
				throw new ArgumentException(dir);
			}
			if (!Directory.Exists(dir))
			{
				return;
			}
			string[] fileSystemEntries = Directory.GetFileSystemEntries(dir);
			foreach (string text in fileSystemEntries)
			{
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				else
				{
					DeleteDir(text);
				}
			}
			Directory.Delete(dir, true);
		}

		public static void ClearDir(string dirPath)
		{
			if (!Directory.Exists(dirPath))
			{
				return;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (fileInfo.Attributes.ToString().IndexOf("ReadOnly") != -1)
				{
					File.SetAttributes(fileInfo.FullName, FileAttributes.Normal);
				}
				File.Delete(fileInfo.FullName);
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				Directory.Delete(directoryInfo2.FullName, true);
			}
		}

		public static string Execute(string filepath, string args)
		{
			return Execute(filepath, args, true, ProcessWindowStyle.Hidden);
		}

		public static string Execute(string filepath, string args, bool waitexit)
		{
			return Execute(filepath, args, waitexit, ProcessWindowStyle.Hidden);
		}

		public static string Execute(string filepath, string args, bool waitexit, ProcessWindowStyle WindowStyle)
		{
			if (new FileInfo(filepath).Exists)
			{
				try
				{
					ProcessStartInfo processStartInfo = new ProcessStartInfo();
					processStartInfo.FileName = filepath;
					processStartInfo.Arguments = args;
					processStartInfo.WindowStyle = WindowStyle;
					Process process = new Process();
					processStartInfo.UseShellExecute = false;
					process.StartInfo = processStartInfo;
					process.Start();
					while (!process.HasExited)
					{
					}
					if (waitexit)
					{
						process.WaitForExit();
					}
					return "";
				}
				catch (Exception ex)
				{
					return ex.Message;
				}
			}
			return "程序文件不存在";
		}

		public static string FormatBytesStr(long bytes)
		{
			if (bytes >= 1073741824)
			{
				return $"{(double)bytes / 1073741824.0:F}" + " G";
			}
			if (bytes >= 1048576)
			{
				return $"{(double)bytes / 1048576.0:F}" + " M";
			}
			if (bytes >= 1024)
			{
				return $"{(double)bytes / 1024.0:F}" + " K";
			}
			return bytes.ToString() + " B";
		}

		public static bool IsFileUse(string filename)
		{
			bool result = true;
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
				result = false;
			}
			catch
			{
			}
			finally
			{
				fileStream?.Close();
			}
			return result;
		}
	}
}
