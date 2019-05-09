using FangPage.Common;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace FangPage.MVC
{
	public class FPResponse
	{
		public static void End()
		{
			HttpContext.Current.Response.End();
		}

		public static void Redirect(string url)
		{
			HttpContext.Current.Response.Redirect(url);
		}

		public static void Redirect(string url, string target)
		{
			if (target == "" || target == "_self" || target == "self")
			{
				HttpContext.Current.Response.Redirect(url);
				return;
			}
			target = target.TrimStart('_');
			HttpContext.Current.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
			HttpContext.Current.Response.Write("<script type=\"text/javascript\">");
			HttpContext.Current.Response.Write("window." + target + ".location='" + url + "';");
			HttpContext.Current.Response.Write("</script>");
			HttpContext.Current.Response.End();
		}

		public static void Write(string s)
		{
			HttpContext.Current.Response.Write(s);
		}

		public static void Write(string s, bool endResponse)
		{
			HttpContext.Current.Response.Write(s);
			if (endResponse)
			{
				HttpContext.Current.Response.End();
			}
		}

		public static void Write(object obj)
		{
			HttpContext.Current.Response.Write(obj);
		}

		public static void Write(object obj, bool endResponse)
		{
			HttpContext.Current.Response.Write(obj);
			if (endResponse)
			{
				HttpContext.Current.Response.End();
			}
		}

		public static void WriteEnd(object obj)
		{
			HttpContext.Current.Response.Write(obj);
			HttpContext.Current.Response.End();
		}

		public static void WriteDown(string filePath)
		{
			string fileName = Path.GetFileName(filePath);
			HttpContext.Current.Response.Buffer = true;
			HttpContext.Current.Response.Clear();
			HttpContext.Current.Response.ContentType = GetResponseContentType(Path.GetExtension(filePath));
			HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + FPUtils.UrlEncode(fileName));
			HttpContext.Current.Response.WriteFile(filePath);
			HttpContext.Current.Response.Flush();
			HttpContext.Current.Response.End();
		}

		public static void WriteDown(string filePath, string filename)
		{
			HttpContext.Current.Response.Buffer = true;
			HttpContext.Current.Response.Clear();
			HttpContext.Current.Response.ContentType = GetResponseContentType(Path.GetExtension(filePath));
			HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + FPUtils.UrlEncode(filename));
			HttpContext.Current.Response.WriteFile(filePath);
			HttpContext.Current.Response.Flush();
			HttpContext.Current.Response.End();
		}

		public static void WriteJson(object obj)
		{
			SendData(FPJson.ToJson(obj));
		}

		public static void WriteXml<T>(T model) where T : new()
		{
			SendData(FPXml.ToXml(model));
		}

		public static void WriteXml<T>(List<T> list) where T : new()
		{
			SendData(FPXml.ToXml(list));
		}

		public static void SendData(string data)
		{
			HttpContext.Current.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
			HttpContext.Current.Response.Write(data);
			HttpContext.Current.Response.End();
		}

		public static void SendData(object obj)
		{
			HttpContext.Current.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
			HttpContext.Current.Response.Write(obj);
			HttpContext.Current.Response.End();
		}

		public static void Log(object obj)
		{
			if (obj != null)
			{
				FPFile.AppendFile(FPFile.GetMapPath(WebConfig.WebPath + "log/sys.log"), obj.ToString());
			}
		}

		public static void Log(string logfile, object obj)
		{
			if (obj != null)
			{
				FPFile.AppendFile(FPFile.GetMapPath(WebConfig.WebPath + "log/" + logfile), obj.ToString());
			}
		}

		private static string GetResponseContentType(string type)
		{
			if (type == ".htm")
			{
				return "text/html";
			}
			if (type == ".html")
			{
				return "text/html";
			}
			if (type == ".txt")
			{
				return "text/plain";
			}
			if (type == ".xml")
			{
				return "text/plain";
			}
			if (type == ".js")
			{
				return "application/x-javascript";
			}
			if (type == ".css")
			{
				return "text/css";
			}
			if (type == ".jpg")
			{
				return "image/jpeg";
			}
			if (type == "gif")
			{
				return "image/gif";
			}
			if (type == "png")
			{
				return "image/png";
			}
			if (type == ".swf")
			{
				return "application/x-shockwave-flash";
			}
			if (type == ".flv")
			{
				return "application/x-shockwave-flash";
			}
			if (type == "doc")
			{
				return "application/msword";
			}
			if (type == ".xls")
			{
				return "application/vnd.ms-excel";
			}
			if (type == ".ppt")
			{
				return "application/vnd.ms-powerpoint";
			}
			if (type == ".mp3")
			{
				return "audio/mpeg";
			}
			if (type == ".mpg")
			{
				return "video/mpeg";
			}
			if (type == ".rar")
			{
				return "application/zip";
			}
			if (type == ".zip")
			{
				return "application/zip";
			}
			return "application/octet-stream";
		}
	}
}
