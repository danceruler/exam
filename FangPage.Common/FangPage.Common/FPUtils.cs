using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace FangPage.Common
{
	public class FPUtils
	{
		public static string MD5(string str)
		{
			byte[] bytes = Encoding.Default.GetBytes(str);
			bytes = new MD5CryptoServiceProvider().ComputeHash(bytes);
			string text = "";
			for (int i = 0; i < bytes.Length; i++)
			{
				text += bytes[i].ToString("x").PadLeft(2, '0');
			}
			return text;
		}

		public static string UrlEncode(string str)
		{
			return HttpUtility.UrlEncode(str, Encoding.UTF8);
		}

		public static string UrlDecode(string str)
		{
			return HttpUtility.UrlDecode(str, Encoding.UTF8);
		}

		public static string CutString(string str, int len)
		{
			Regex regex = new Regex("^[\\u4e00-\\u9fa5]+$", RegexOptions.Compiled);
			char[] array = str.ToCharArray();
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				num = ((!regex.IsMatch(array[i].ToString())) ? (num + 1) : (num + 2));
				if (num <= len)
				{
					stringBuilder.Append(array[i]);
					continue;
				}
				break;
			}
			if (stringBuilder.ToString() != str)
			{
				stringBuilder.Append("...");
			}
			return stringBuilder.ToString();
		}

		public static string RemoveHtml(string content)
		{
			string pattern = "<[^>]*>";
			return Regex.Replace(content, pattern, string.Empty, RegexOptions.IgnoreCase).Trim();
		}

		public static string GetTxtFromHTML(string HTML)
		{
			Regex regex = new Regex("</?(?!br|img)[^>]*>", RegexOptions.IgnoreCase);
			return regex.Replace(HTML, "");
		}

		public static string GetDateTime()
		{
			return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static string GetDate()
		{
			return DateTime.Now.ToString("yyyy-MM-dd");
		}

		public static string FormatDateTime(string datetime, string format)
		{
			if (string.IsNullOrEmpty(datetime))
			{
				return "";
			}
			try
			{
				datetime = Convert.ToDateTime(datetime).ToString(format).Replace("1900-01-01", "");
			}
			catch
			{
			}
			return datetime;
		}

		public static string FormatDateTime(string datetime)
		{
			return FormatDateTime(datetime, "yyyy-MM-dd HH:mm:ss");
		}

		public static string FormatDateTime(DateTime datetime)
		{
			return FormatDateTime(datetime, "yyyy-MM-dd HH:mm:ss");
		}

		public static string FormatDateTime(DateTime datetime, string format)
		{
			if (string.IsNullOrEmpty(format))
			{
				return datetime.ToString("yyyy-MM-dd HH:mm:ss");
			}
			return datetime.ToString(format);
		}

		public static string FormatDateTime(DateTime? datetime)
		{
			return FormatDateTime(datetime, "yyyy-MM-dd HH:mm:ss");
		}

		public static string FormatDateTime(DateTime? datetime, string format)
		{
			if (datetime.HasValue)
			{
				return FormatDateTime(datetime.Value, format);
			}
			return "";
		}

		public static string DateDiff(DateTime? starttime, DateTime? endtime)
		{
			if (!starttime.HasValue || !endtime.HasValue)
			{
				return "";
			}
			TimeSpan ts = new TimeSpan(starttime.Value.Ticks);
			TimeSpan timeSpan = new TimeSpan(endtime.Value.Ticks).Subtract(ts).Duration();
			string text = "";
			if (timeSpan.Days > 0)
			{
				text = text + timeSpan.Days.ToString() + "天";
			}
			if (timeSpan.Hours > 0)
			{
				text = text + timeSpan.Hours.ToString() + "时";
			}
			if (timeSpan.Minutes > 0)
			{
				text = text + timeSpan.Minutes.ToString() + "分";
			}
			if (timeSpan.Seconds > 0)
			{
				text = text + timeSpan.Seconds.ToString() + "秒";
			}
			if (text == "")
			{
				text = "1秒";
			}
			return text;
		}

		public static bool StrToBool(object Expression, bool defValue)
		{
			if (Expression != null)
			{
				if (string.Compare(Expression.ToString(), "true", true) == 0)
				{
					return true;
				}
				if (string.Compare(Expression.ToString(), "false", true) == 0)
				{
					return false;
				}
			}
			return defValue;
		}

		public static int StrToInt(object Expression)
		{
			return StrToInt(Expression, 0);
		}

		public static int StrToInt(object Expression, int defValue)
		{
			if (Expression != null)
			{
				string text = Expression.ToString();
				if (text.Length > 0 && text.Length <= 11 && Regex.IsMatch(text, "^[-]?[0-9]*$") && (text.Length < 10 || (text.Length == 10 && text[0] == '1') || (text.Length == 11 && text[0] == '-' && text[1] == '1')))
				{
					return Convert.ToInt32(text);
				}
			}
			return defValue;
		}

		public static float StrToFloat(object strValue)
		{
			return StrToFloat(strValue, 0f);
		}

		public static float StrToFloat(object strValue, float defValue)
		{
			if (strValue == null || strValue.ToString().Length > 10)
			{
				return defValue;
			}
			float result = defValue;
			if (strValue != null && Regex.IsMatch(strValue.ToString(), "^([-]|[0-9])[0-9]*(\\.\\w*)?$"))
			{
				result = Convert.ToSingle(strValue);
			}
			return result;
		}

		public static decimal StrToDecimal(object strValue)
		{
			return StrToDecimal(strValue, 0.00m);
		}

		public static decimal StrToDecimal(object strValue, decimal defValue)
		{
			if (strValue == null || strValue.ToString().Length > 10)
			{
				return defValue;
			}
			decimal result = defValue;
			if (strValue != null && Regex.IsMatch(strValue.ToString(), "^([-]|[0-9])[0-9]*(\\.\\w*)?$"))
			{
				result = Convert.ToDecimal(strValue);
			}
			return result;
		}

		public static double StrToDouble(object strValue)
		{
			return StrToDouble(strValue, 0.0);
		}

		public static double StrToDouble(object strValue, double defValue)
		{
			if (strValue == null || strValue.ToString().Length > 10)
			{
				return defValue;
			}
			double result = defValue;
			if (strValue != null && Regex.IsMatch(strValue.ToString(), "^([-]|[0-9])[0-9]*(\\.\\w*)?$"))
			{
				result = Convert.ToDouble(strValue);
			}
			return result;
		}

		public static DateTime StrToDateTime(string strValue)
		{
			try
			{
				return Convert.ToDateTime(strValue);
			}
			catch
			{
				return Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			}
		}

		public static DateTime StrToDateTime(string strValue, string format)
		{
			try
			{
				strValue = FormatDateTime(strValue, format);
				return Convert.ToDateTime(strValue);
			}
			catch
			{
				if (!string.IsNullOrEmpty(format))
				{
					return Convert.ToDateTime(DateTime.Now.ToString(format));
				}
				return Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			}
		}

		public static DateTime? StrToDateTime2(string strValue)
		{
			if (string.IsNullOrEmpty(strValue))
			{
				return null;
			}
			try
			{
				return Convert.ToDateTime(strValue);
			}
			catch
			{
				return null;
			}
		}

		public static DateTime? StrToDateTime2(string strValue, string format)
		{
			if (string.IsNullOrEmpty(strValue))
			{
				return null;
			}
			try
			{
				strValue = FormatDateTime(strValue, format);
				return Convert.ToDateTime(strValue);
			}
			catch
			{
				return null;
			}
		}

		public static FPData StrToFPData(string strArray)
		{
			FPData fPData = new FPData();
			int num = 0;
			string[] array = FPArray.SplitString(strArray);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = fPData[num] = array[i];
				num++;
			}
			return fPData;
		}

		public static Version StrToVersion(string str)
		{
			int[] array = FPArray.SplitInt(str, ".", 4);
			return new Version(string.Format("{0,1}.{1,1}.{2,1}.{3,1}", array[0].ToString(), array[1].ToString(), array[2].ToString(), array[3].ToString()));
		}

		public static Version GetVersion(string filename)
		{
			Version result = StrToVersion("0.0.0.0");
			if (Path.GetExtension(filename).ToLower() == ".dll" && File.Exists(filename))
			{
				Assembly assembly = GetAssembly(filename);
				if (assembly != null)
				{
					result = assembly.GetName().Version;
				}
			}
			return result;
		}

		public static Assembly GetAssembly(string filename)
		{
			if (!File.Exists(filename))
			{
				return null;
			}
			byte[] rawAssembly = File.ReadAllBytes(filename);
			return Assembly.Load(rawAssembly);
		}

		public static string FormatVersion(string str)
		{
			int[] array = FPArray.SplitInt(str, ".", 4);
			return string.Format("{0,1}.{1,1}.{2,1}", array[0].ToString(), array[1].ToString(), array[2].ToString());
		}

		public static bool IsContain(string content, string value)
		{
			return content.IndexOf(value) >= 0;
		}

		public static bool IsContain(StringBuilder content, string value)
		{
			int length = content.Length;
			content = content.Replace(value, value + "1");
			if (length != content.Length)
			{
				content = content.Replace(value + "1", value);
				return true;
			}
			return false;
		}

		public static bool IsNumeric(object Expression)
		{
			if (Expression != null)
			{
				string text = Expression.ToString();
				if (text.Length > 0 && text.Length <= 11 && Regex.IsMatch(text, "^[-]?[0-9]*[.]?[0-9]*$") && (text.Length < 10 || (text.Length == 10 && text[0] == '1') || (text.Length == 11 && text[0] == '-' && text[1] == '1')))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsNumericArray(string[] strNumber)
		{
			if (strNumber == null)
			{
				return false;
			}
			if (strNumber.Length < 1)
			{
				return false;
			}
			foreach (string expression in strNumber)
			{
				if (!IsNumeric(expression))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsNumericArray(string strNumber)
		{
			if (strNumber == null)
			{
				return false;
			}
			if (strNumber.Length < 1)
			{
				return false;
			}
			string[] array = strNumber.Split(',');
			foreach (string expression in array)
			{
				if (!IsNumeric(expression))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsIP(string ip)
		{
			return Regex.IsMatch(ip, "^((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)$");
		}

		public static bool IsSafeSqlString(string str)
		{
			return !Regex.IsMatch(str, "[-|;|,|\\/|\\(|\\)|\\[|\\]|\\}|\\{|%|@|\\*|!|\\']");
		}

		public static bool IsValidDomain(string host)
		{
			if (host.IndexOf(".") == -1)
			{
				return false;
			}
			return (!new Regex("^\\d+$").IsMatch(host.Replace(".", string.Empty))) ? true : false;
		}

		public static bool IsEmail(string strEmail)
		{
			return Regex.IsMatch(strEmail, "^([\\w-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([\\w-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$");
		}

		public static string ImgToBase64(string filename)
		{
			try
			{
				Bitmap bitmap = new Bitmap(filename);
				MemoryStream memoryStream = new MemoryStream();
				ImageFormat format = ImageFormat.Png;
				string str = "data:image/png;base64,";
				if (Path.GetExtension(filename) == ".jpg")
				{
					format = ImageFormat.Jpeg;
					str = "data:image/jpeg;base64,";
				}
				else if (Path.GetExtension(filename) == ".gif")
				{
					format = ImageFormat.Gif;
					str = "data:image/gif;base64,";
				}
				bitmap.Save(memoryStream, format);
				byte[] array = new byte[memoryStream.Length];
				memoryStream.Position = 0L;
				memoryStream.Read(array, 0, (int)memoryStream.Length);
				memoryStream.Close();
				return str + Convert.ToBase64String(array);
			}
			catch (Exception ex)
			{
				return "转换失败:" + ex.Message;
			}
		}

		public static string Base64ToImg(string base64str, string filename)
		{
			try
			{
				string directoryName = Path.GetDirectoryName(filename);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				if (base64str.StartsWith("data:image/"))
				{
					base64str = FPArray.SplitString(base64str, 2)[1];
				}
				byte[] buffer = Convert.FromBase64String(base64str);
				MemoryStream memoryStream = new MemoryStream(buffer);
				Bitmap bitmap = new Bitmap(memoryStream);
				if (File.Exists(filename))
				{
					File.Delete(filename);
				}
				ImageFormat format = ImageFormat.Png;
				if (base64str.StartsWith("data:image/jpeg"))
				{
					format = ImageFormat.Jpeg;
				}
				else if (base64str.StartsWith("data:image/gif"))
				{
					format = ImageFormat.Gif;
				}
				bitmap.Save(filename, format);
				memoryStream.Close();
				return "";
			}
			catch (Exception ex)
			{
				return "转换失败:" + ex.Message;
			}
		}

		public static string TxtToImg(string txt, int width, int height, string filename)
		{
			try
			{
				if (width == 0)
				{
					width = 500;
				}
				if (height == 0)
				{
					height = 400;
				}
				string directoryName = Path.GetDirectoryName(filename);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				Bitmap image = new Bitmap(width, height);
				Graphics graphics = Graphics.FromImage(image);
				Font font = new Font("宋体", 11f);
				SizeF sizeF = graphics.MeasureString(txt, font, width);
				image = new Bitmap(width, Convert.ToInt32(sizeF.Height));
				graphics = Graphics.FromImage(image);
				graphics.Clear(Color.White);
				graphics.DrawString(txt, font, Brushes.Black, new Rectangle(0, 0, Convert.ToInt32(sizeF.Width), Convert.ToInt32(sizeF.Height)));
				image.Save(filename, ImageFormat.Jpeg);
				return "";
			}
			catch (Exception ex)
			{
				return "转换失败:" + ex.Message;
			}
		}

		public static string GetIntranetIp()
		{
			string text = string.Empty;
			IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress[] addressList = hostEntry.AddressList;
			foreach (IPAddress iPAddress in addressList)
			{
				if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					text = iPAddress.ToString();
					break;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "127.0.0.1";
			}
			return text;
		}

		public static string Trim(string content, string trim)
		{
			if (string.IsNullOrEmpty(trim))
			{
				return content;
			}
			if (content.StartsWith(trim))
			{
				content = content.Remove(0, trim.Length);
			}
			if (content.EndsWith(trim))
			{
				content = content.Remove(content.Length - trim.Length, trim.Length);
			}
			return content;
		}

		public static string TrimStart(string content, string trim)
		{
			if (string.IsNullOrEmpty(trim))
			{
				return content;
			}
			if (content.StartsWith(trim))
			{
				content = content.Remove(0, trim.Length);
			}
			return content;
		}

		public static string TrimEnd(string content, string trim)
		{
			if (string.IsNullOrEmpty(trim))
			{
				return content;
			}
			if (content.EndsWith(trim))
			{
				content = content.Remove(content.Length - trim.Length, trim.Length);
			}
			return content;
		}
	}
}
