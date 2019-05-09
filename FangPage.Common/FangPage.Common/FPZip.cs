using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Web;

namespace FangPage.Common
{
	public class FPZip : IDisposable
	{
		private ZipFile file;

		private MemoryStream ms = new MemoryStream();

		public FPZip()
		{
			file = ZipFile.Create(ms);
			file.BeginUpdate();
		}

		public void AddDir(string dirpath)
		{
			AddDir(dirpath, "");
		}

		public void AddDir(string dirpath, string entryname)
		{
			if (dirpath.EndsWith("\\") || dirpath.EndsWith("/"))
			{
				dirpath = dirpath.Substring(0, dirpath.Length - 1);
			}
			if (Directory.Exists(dirpath))
			{
				if (string.IsNullOrEmpty(entryname))
				{
					entryname = dirpath.Substring(dirpath.LastIndexOf("\\") + 1);
				}
				ZipDir(file, dirpath, entryname);
			}
		}

		public void AddFile(string filepath)
		{
			AddFile(filepath, "");
		}

		public void AddFile(string filepath, string entryname)
		{
			if (File.Exists(filepath))
			{
				if (string.IsNullOrEmpty(entryname))
				{
					entryname = Path.GetFileName(filepath);
				}
				file.Add(filepath, entryname);
			}
		}

		public void ZipDown(string zipname)
		{
			byte[] array = null;
			file.CommitUpdate();
			array = new byte[ms.Length];
			ms.Position = 0L;
			ms.Read(array, 0, array.Length);
			HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + zipname);
			HttpContext.Current.Response.BinaryWrite(array);
			HttpContext.Current.Response.Flush();
			HttpContext.Current.Response.End();
		}

		public void ZipSave(string zippath)
		{
			byte[] array = null;
			file.CommitUpdate();
			array = new byte[ms.Length];
			ms.Position = 0L;
			ms.Read(array, 0, array.Length);
			string directoryName = Path.GetDirectoryName(zippath);
			string fileName = Path.GetFileName(zippath);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			if (File.Exists(zippath))
			{
				File.Delete(zippath);
			}
			using (FileStream fileStream = new FileStream(zippath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				fileStream.Write(array, 0, array.Length);
				fileStream.Close();
			}
		}

		private void ZipDir(ZipFile file, string sitemappath, string entryname)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sitemappath);
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				ZipDir(file, sitemappath + "\\" + directoryInfo2.Name, entryname + "\\" + directoryInfo2.Name);
			}
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				file.Add(fileInfo.FullName, entryname + "\\" + fileInfo.Name);
			}
		}

		public void Close()
		{
			ms.Close();
			file.Close();
		}

		public void Dispose()
		{
			ms.Close();
			file.Close();
		}

		public static bool UnZip(string zipFilePath)
		{
			return UnZip(zipFilePath, "");
		}

		public static bool UnZip(string zipFilePath, string unZipDir)
		{
			if (unZipDir == string.Empty)
			{
				unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
			}
			if (!unZipDir.EndsWith("\\"))
			{
				unZipDir += "\\";
			}
			if (!Directory.Exists(unZipDir))
			{
				Directory.CreateDirectory(unZipDir);
			}
			using (ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(zipFilePath)))
			{
				ZipEntry nextEntry;
				while ((nextEntry = zipInputStream.GetNextEntry()) != null)
				{
					string directoryName = Path.GetDirectoryName(nextEntry.Name);
					string fileName = Path.GetFileName(nextEntry.Name);
					if (directoryName.Length > 0)
					{
						Directory.CreateDirectory(unZipDir + directoryName);
					}
					if (!directoryName.EndsWith("\\"))
					{
						directoryName += "\\";
					}
					if (fileName != string.Empty)
					{
						using (FileStream fileStream = File.Create(unZipDir + nextEntry.Name))
						{
							int num = 2048;
							byte[] array = new byte[2048];
							while (true)
							{
								num = zipInputStream.Read(array, 0, array.Length);
								if (num <= 0)
								{
									break;
								}
								fileStream.Write(array, 0, num);
							}
						}
					}
				}
			}
			return true;
		}

		public static string UnRar(string rarPath, string unPath)
		{
			string mapPath = FPFile.GetMapPath("/bin/WinRAR.exe");
			if (new FileInfo(mapPath).Exists)
			{
				try
				{
					string text = "x -inul -y -o+ -v[t,b]";
					text = text + " " + rarPath + " " + unPath;
					ProcessStartInfo processStartInfo = new ProcessStartInfo();
					processStartInfo.FileName = mapPath;
					processStartInfo.Arguments = text;
					processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
					Process process = new Process();
					processStartInfo.UseShellExecute = false;
					process.StartInfo = processStartInfo;
					process.Start();
					while (!process.HasExited)
					{
					}
					process.WaitForExit();
					return "";
				}
				catch (Exception ex)
				{
					return ex.Message;
				}
			}
			return "请安装WinRar.exe!";
		}
	}
}
