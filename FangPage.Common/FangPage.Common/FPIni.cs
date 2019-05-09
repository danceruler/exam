using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace FangPage.Common
{
	public class FPIni
	{
		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

		public static void WriteIni(string section, string key, string value, string filename)
		{
			if (!Directory.Exists(Path.GetDirectoryName(filename)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(filename));
			}
			if (File.Exists(filename) && File.GetAttributes(filename).ToString().IndexOf("ReadOnly") != -1)
			{
				File.SetAttributes(filename, FileAttributes.Normal);
			}
			WritePrivateProfileString(section, key, value, filename);
		}

		public static void WriteIni<T>(T model, string filename)
		{
			Type typeFromHandle = typeof(T);
			PropertyInfo[] properties = typeFromHandle.GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo != null && propertyInfo.CanWrite)
				{
					string value = string.Empty;
					if (propertyInfo.GetValue(model, null) != null)
					{
						value = propertyInfo.GetValue(model, null).ToString();
					}
					WriteIni(typeFromHandle.Name, propertyInfo.Name, value, filename);
				}
			}
		}

		public static void WriteIni<T>(string key, string value, string filename)
		{
			Type typeFromHandle = typeof(T);
			WriteIni(typeFromHandle.Name, key, value, filename);
		}

		public static string ReadIni(string section, string key, string filename)
		{
			StringBuilder stringBuilder = new StringBuilder(255);
			GetPrivateProfileString(section, key, "", stringBuilder, 255, filename);
			return stringBuilder.ToString();
		}

		public static string ReadIni<T>(string key, string filename)
		{
			Type typeFromHandle = typeof(T);
			return ReadIni(typeFromHandle.Name, key, filename);
		}

		public static T ReadIni<T>(string filename) where T : new()
		{
			T val = new T();
			if (!File.Exists(filename))
			{
				WriteIni(val, filename);
				return val;
			}
			Type typeFromHandle = typeof(T);
			PropertyInfo[] properties = typeFromHandle.GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo != null && propertyInfo.CanWrite)
				{
					string text = ReadIni(typeFromHandle.Name, propertyInfo.Name, filename);
					if (propertyInfo.PropertyType == typeof(string))
					{
						propertyInfo.SetValue(val, text, null);
					}
					else if (propertyInfo.PropertyType == typeof(int))
					{
						propertyInfo.SetValue(val, FPUtils.StrToInt(text), null);
					}
					else if (propertyInfo.PropertyType == typeof(short))
					{
						propertyInfo.SetValue(val, short.Parse(FPUtils.StrToInt(text).ToString()), null);
					}
					else if (propertyInfo.PropertyType == typeof(DateTime))
					{
						propertyInfo.SetValue(val, FPUtils.StrToDateTime(text), null);
					}
					else if (propertyInfo.PropertyType == typeof(decimal))
					{
						propertyInfo.SetValue(val, FPUtils.StrToDecimal(text), null);
					}
					else if (propertyInfo.PropertyType == typeof(float))
					{
						propertyInfo.SetValue(val, FPUtils.StrToFloat(text), null);
					}
					else if (propertyInfo.PropertyType == typeof(double))
					{
						propertyInfo.SetValue(val, FPUtils.StrToDouble(text), null);
					}
					else if (propertyInfo.PropertyType == typeof(DateTime?) && propertyInfo.PropertyType != null)
					{
						propertyInfo.SetValue(val, FPUtils.StrToDateTime(text), null);
					}
				}
			}
			return val;
		}
	}
}
