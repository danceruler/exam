using FangPage.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace FangPage.MVC
{
	public class FPRequest
	{
		public static HttpFileCollection Files => HttpContext.Current.Request.Files;

		public static bool IsPost()
		{
			return HttpContext.Current.Request.HttpMethod.Equals("POST");
		}

		public static bool IsGet()
		{
			return HttpContext.Current.Request.HttpMethod.Equals("GET");
		}

		public static bool IsPostFile()
		{
			for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
			{
				if (HttpContext.Current.Request.Files[i].FileName != "")
				{
					return true;
				}
			}
			return false;
		}

		public static string GetServerString(string strName)
		{
			if (HttpContext.Current.Request.ServerVariables[strName] == null)
			{
				return "";
			}
			return HttpContext.Current.Request.ServerVariables[strName].ToString();
		}

		public static string GetUrlReferrer()
		{
			string text = null;
			try
			{
				text = HttpContext.Current.Request.UrlReferrer.ToString();
			}
			catch
			{
			}
			if (text == null)
			{
				return "";
			}
			return text;
		}

		public static string GetDomain()
		{
			HttpRequest request = HttpContext.Current.Request;
			string text = request.Url.Host;
			if (text == "127.0.0.1")
			{
				text = "localhost";
			}
			if (!request.Url.IsDefaultPort)
			{
				return $"http://{text}:{request.Url.Port.ToString()}";
			}
			return "http://" + text;
		}

		public static int GetPort()
		{
			return HttpContext.Current.Request.Url.Port;
		}

		public static string GetHost()
		{
			return HttpContext.Current.Request.Url.Host;
		}

		public static string GetRawUrl()
		{
			return HttpContext.Current.Request.RawUrl;
		}

		public static bool IsBrowserGet()
		{
			string[] array = new string[6]
			{
				"ie",
				"opera",
				"netscape",
				"mozilla",
				"konqueror",
				"firefox"
			};
			string text = HttpContext.Current.Request.Browser.Type.ToLower();
			for (int i = 0; i < array.Length; i++)
			{
				if (text.IndexOf(array[i]) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		public static string GetUrl()
		{
			return HttpContext.Current.Request.Url.ToString();
		}

		public static string GetQueryString(string strName)
		{
			if (HttpContext.Current.Request.QueryString[strName] == null)
			{
				return "";
			}
			return HttpContext.Current.Request.QueryString[strName];
		}

		public static string GetPageName()
		{
			string[] array = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
			return array[array.Length - 1].ToLower();
		}

		public static int GetParamCount()
		{
			return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count;
		}

		public static string GetFormString(string strName)
		{
			if (HttpContext.Current.Request.Form[strName] == null)
			{
				return "";
			}
			return HttpContext.Current.Request.Form[strName];
		}

		public static string GetString(string strName)
		{
			if ("".Equals(GetFormString(strName)))
			{
				return GetQueryString(strName);
			}
			return GetFormString(strName);
		}

		public static string GetString(string strName, string defValue)
		{
			string @string = GetString(strName);
			if (@string == "")
			{
				return defValue;
			}
			return @string;
		}

		public static int GetQueryInt(string strName, int defValue)
		{
			string queryString = GetQueryString(strName);
			if (FPUtils.IsNumericArray(queryString))
			{
				return FPArray.SplitInt(queryString, 1)[0];
			}
			return FPUtils.StrToInt(queryString, defValue);
		}

		public static int GetFormInt(string strName, int defValue)
		{
			string formString = GetFormString(strName);
			if (FPUtils.IsNumericArray(formString))
			{
				return FPArray.SplitInt(formString, 1)[0];
			}
			return FPUtils.StrToInt(formString, defValue);
		}

		public static int GetInt(string strName, int defValue)
		{
			if ("".Equals(GetFormString(strName)))
			{
				return GetQueryInt(strName, defValue);
			}
			return GetFormInt(strName, defValue);
		}

		public static int GetInt(string strName)
		{
			return GetInt(strName, 0);
		}

		public static string GetIntString(string strName)
		{
			return FPArray.FmatInt(GetString(strName));
		}

		public static DateTime GetDateTime(string strName)
		{
			string @string = GetString(strName);
			@string = ((strName.ToLower() == "startdate") ? FPUtils.FormatDateTime(@string, "yyyy-MM-dd 00:00:00") : ((!(strName.ToLower() == "enddate")) ? FPUtils.FormatDateTime(@string) : FPUtils.FormatDateTime(@string, "yyyy-MM-dd 23:59:59")));
			return FPUtils.StrToDateTime(@string);
		}

		public static DateTime GetDateTime(string strName, string format)
		{
			return FPUtils.StrToDateTime(FPUtils.FormatDateTime(GetString(strName), format));
		}

		public static DateTime? GetDateTime2(string strName)
		{
			string @string = GetString(strName);
			if (string.IsNullOrEmpty(@string))
			{
				return null;
			}
			@string = ((strName.ToLower() == "startdate") ? FPUtils.FormatDateTime(@string, "yyyy-MM-dd 00:00:00") : ((!(strName.ToLower() == "enddate")) ? FPUtils.FormatDateTime(@string) : FPUtils.FormatDateTime(@string, "yyyy-MM-dd 23:59:59")));
			return FPUtils.StrToDateTime2(@string);
		}

		public static DateTime? GetDateTime2(string strName, string format)
		{
			return FPUtils.StrToDateTime2(FPUtils.FormatDateTime(GetString(strName), format));
		}

		public static float GetQueryFloat(string strName, float defValue)
		{
			return FPUtils.StrToFloat(HttpContext.Current.Request.QueryString[strName], defValue);
		}

		public static float GetFormFloat(string strName, float defValue)
		{
			return FPUtils.StrToFloat(HttpContext.Current.Request.Form[strName], defValue);
		}

		public static float GetFloat(string strName, float defValue)
		{
			if ("".Equals(GetFormString(strName)))
			{
				return GetQueryFloat(strName, defValue);
			}
			return GetFormFloat(strName, defValue);
		}

		public static float GetFloat(string strName)
		{
			return GetFloat(strName, 0f);
		}

		public static decimal GetQueryDecimal(string strName, decimal defValue)
		{
			return FPUtils.StrToDecimal(HttpContext.Current.Request.QueryString[strName], defValue);
		}

		public static decimal GetFormDecimal(string strName, decimal defValue)
		{
			return FPUtils.StrToDecimal(HttpContext.Current.Request.Form[strName], defValue);
		}

		public static decimal GetDecimal(string strName, decimal defValue)
		{
			if ("".Equals(GetFormString(strName)))
			{
				return GetQueryDecimal(strName, defValue);
			}
			return GetFormDecimal(strName, defValue);
		}

		public static decimal GetDecimal(string strName)
		{
			return GetDecimal(strName, decimal.Zero);
		}

		public static double GetQueryDouble(string strName, double defValue)
		{
			return FPUtils.StrToDouble(HttpContext.Current.Request.QueryString[strName], defValue);
		}

		public static double GetFormDouble(string strName, double defValue)
		{
			return FPUtils.StrToDouble(HttpContext.Current.Request.Form[strName], defValue);
		}

		public static double GetDouble(string strName, float defValue)
		{
			if ("".Equals(GetFormString(strName)))
			{
				return GetQueryDouble(strName, defValue);
			}
			return GetFormDouble(strName, defValue);
		}

		public static double GetDouble(string strName)
		{
			return GetDouble(strName, 0f);
		}

		public static T GetModel<T>() where T : new()
		{
			return GetModel<T>("");
		}

		public static T GetModel<T>(T model)
		{
			return GetModel(model, "");
		}

		public static T GetModel<T>(string prefix) where T : new()
		{
			T val = new T();
			PropertyInfo[] properties = typeof(T).GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo == null || !propertyInfo.CanWrite)
				{
					continue;
				}
				string text = prefix + propertyInfo.Name;
				if (text.ToLower() == prefix + "pageurl")
				{
					propertyInfo.SetValue(val, GetRawUrl(), null);
				}
				else if (propertyInfo.PropertyType == typeof(FPData))
				{
					FPData fPData = GetFPData(text, propertyInfo.GetValue(val, null) as FPData);
					propertyInfo.SetValue(val, fPData, null);
				}
				else if (propertyInfo.PropertyType == typeof(List<FPData>))
				{
					List<FPData> fPList = GetFPList(text);
					propertyInfo.SetValue(val, fPList, null);
				}
				else if (HttpContext.Current.Request.QueryString[text] != null || HttpContext.Current.Request.Form[text] != null)
				{
					if (propertyInfo.PropertyType == typeof(string))
					{
						propertyInfo.SetValue(val, GetString(text), null);
					}
					else if (propertyInfo.PropertyType == typeof(int))
					{
						propertyInfo.SetValue(val, GetInt(text), null);
					}
					else if (propertyInfo.PropertyType == typeof(short))
					{
						propertyInfo.SetValue(val, short.Parse(GetInt(text).ToString()), null);
					}
					else if (propertyInfo.PropertyType == typeof(DateTime))
					{
						propertyInfo.SetValue(val, GetDateTime(text), null);
					}
					else if (propertyInfo.PropertyType == typeof(decimal))
					{
						propertyInfo.SetValue(val, GetDecimal(text), null);
					}
					else if (propertyInfo.PropertyType == typeof(float))
					{
						propertyInfo.SetValue(val, GetFloat(text), null);
					}
					else if (propertyInfo.PropertyType == typeof(double))
					{
						propertyInfo.SetValue(val, GetDouble(text), null);
					}
					else if (propertyInfo.PropertyType == typeof(DateTime?))
					{
						propertyInfo.SetValue(val, GetDateTime2(text), null);
					}
				}
			}
			return val;
		}

		public static T GetModel<T>(T model, string prefix)
		{
			PropertyInfo[] properties = model.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo == null || !propertyInfo.CanWrite)
				{
					continue;
				}
				string text = prefix + propertyInfo.Name;
				if (text.ToLower() == prefix + "pageurl")
				{
					propertyInfo.SetValue(model, GetRawUrl(), null);
				}
				else if (propertyInfo.PropertyType == typeof(FPData))
				{
					FPData fPData = propertyInfo.GetValue(model, null) as FPData;
					object[] customAttributes = propertyInfo.GetCustomAttributes(true);
					foreach (object obj in customAttributes)
					{
						if (!(obj is CheckBox))
						{
							continue;
						}
						CheckBox checkBox = obj as CheckBox;
						if (!checkBox.IsCheckBox)
						{
							continue;
						}
						if (checkBox.CheckName != "")
						{
							string[] array = FPArray.SplitString(checkBox.CheckName);
							foreach (string key in array)
							{
								fPData[key] = "";
							}
						}
						else
						{
							string[] array = fPData.Keys;
							foreach (string key2 in array)
							{
								fPData[key2] = "";
							}
						}
					}
					fPData = GetFPData(text, fPData);
					propertyInfo.SetValue(model, fPData, null);
				}
				else if (propertyInfo.PropertyType == typeof(List<FPData>))
				{
					List<FPData> fPList = GetFPList(text);
					propertyInfo.SetValue(model, fPList, null);
				}
				else if (HttpContext.Current.Request.QueryString[text] == null && HttpContext.Current.Request.Form[text] == null)
				{
					object[] customAttributes = propertyInfo.GetCustomAttributes(true);
					foreach (object obj2 in customAttributes)
					{
						if (obj2 is CheckBox && (obj2 as CheckBox).IsCheckBox)
						{
							if (propertyInfo.PropertyType == typeof(int))
							{
								propertyInfo.SetValue(model, 0, null);
							}
							else
							{
								propertyInfo.SetValue(model, "", null);
							}
						}
					}
				}
				else if (propertyInfo.PropertyType == typeof(string))
				{
					propertyInfo.SetValue(model, GetString(text), null);
				}
				else if (propertyInfo.PropertyType == typeof(int))
				{
					propertyInfo.SetValue(model, GetInt(text), null);
				}
				else if (propertyInfo.PropertyType == typeof(DateTime))
				{
					propertyInfo.SetValue(model, GetDateTime(text), null);
				}
				else if (propertyInfo.PropertyType == typeof(decimal))
				{
					propertyInfo.SetValue(model, GetDecimal(text), null);
				}
				else if (propertyInfo.PropertyType == typeof(float))
				{
					propertyInfo.SetValue(model, GetFloat(text), null);
				}
				else if (propertyInfo.PropertyType == typeof(double))
				{
					propertyInfo.SetValue(model, GetDouble(text), null);
				}
				else if (propertyInfo.PropertyType == typeof(DateTime?))
				{
					propertyInfo.SetValue(model, GetDateTime2(text), null);
				}
			}
			return model;
		}

		public static List<T> GetList<T>() where T : new()
		{
			return GetList<T>("");
		}

		public static List<T> GetList<T>(string prefix) where T : new()
		{
			List<T> list = new List<T>();
			Type typeFromHandle = typeof(T);
			int num = 0;
			bool flag = true;
			while (flag)
			{
				T val = new T();
				bool flag2 = false;
				PropertyInfo[] properties = typeFromHandle.GetProperties();
				foreach (PropertyInfo propertyInfo in properties)
				{
					string text = prefix + propertyInfo.Name + "[" + num.ToString() + "]";
					if (propertyInfo.PropertyType == typeof(FPData))
					{
						FPData fPData = GetFPData(text, propertyInfo.GetValue(val, null) as FPData);
						if (fPData.Count > 0)
						{
							propertyInfo.SetValue(val, fPData, null);
							flag2 = true;
						}
					}
					else if (propertyInfo.PropertyType == typeof(List<FPData>))
					{
						List<FPData> fPList = GetFPList(text);
						if (fPList.Count > 0)
						{
							propertyInfo.SetValue(val, fPList, null);
							flag2 = true;
						}
					}
					else if (HttpContext.Current.Request.QueryString[text] != null || HttpContext.Current.Request.Form[text] != null)
					{
						flag2 = true;
						if (propertyInfo.PropertyType == typeof(string))
						{
							propertyInfo.SetValue(val, GetString(text), null);
						}
						else if (propertyInfo.PropertyType == typeof(int))
						{
							propertyInfo.SetValue(val, GetInt(text), null);
						}
						else if (propertyInfo.PropertyType == typeof(short))
						{
							propertyInfo.SetValue(val, short.Parse(GetInt(text).ToString()), null);
						}
						else if (propertyInfo.PropertyType == typeof(DateTime))
						{
							propertyInfo.SetValue(val, GetDateTime(text), null);
						}
						else if (propertyInfo.PropertyType == typeof(decimal))
						{
							propertyInfo.SetValue(val, GetDecimal(text), null);
						}
						else if (propertyInfo.PropertyType == typeof(float))
						{
							propertyInfo.SetValue(val, GetFloat(text), null);
						}
						else if (propertyInfo.PropertyType == typeof(double))
						{
							propertyInfo.SetValue(val, GetDouble(text), null);
						}
						else if (propertyInfo.PropertyType == typeof(DateTime?))
						{
							propertyInfo.SetValue(val, GetDateTime2(text), null);
						}
					}
				}
				if (!flag2)
				{
					flag = false;
					break;
				}
				list.Add(val);
				num++;
			}
			return list;
		}

		public static FPData GetFPData(string name)
		{
			FPData fPData = new FPData();
			Regex regex = new Regex("((((?:\\s*)\\[([^\\[\\]\\{\\}\\s]+)\\])(?:\\s*)))", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
			string a = "";
			MatchCollection matchCollection = regex.Matches(name);
			if (matchCollection.Count > 0)
			{
				a = matchCollection[matchCollection.Count - 1].Groups[4].ToString();
				foreach (Match item in matchCollection)
				{
					name = name.Replace(item.Groups[0].ToString(), "");
				}
			}
			Regex regex2 = new Regex("((((?:\\s*)" + name + "\\[([^\\[\\]\\{\\}\\s]+)\\])(?:\\s*)))", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
			foreach (string key in HttpContext.Current.Request.Params.Keys)
			{
				MatchCollection matchCollection2 = regex2.Matches(key);
				if (matchCollection2.Count > 0)
				{
					if (a != "")
					{
						if (matchCollection2.Count > 1)
						{
							string b = matchCollection2[matchCollection2.Count - 2].Groups[4].ToString();
							if (a == b)
							{
								fPData[matchCollection2[matchCollection2.Count - 1].Groups[4].ToString()] = GetString(key);
							}
						}
					}
					else
					{
						fPData[matchCollection2[matchCollection2.Count - 1].Groups[4].ToString()] = GetString(key);
					}
				}
			}
			return fPData;
		}

		public static FPData GetFPData(string name, FPData fpdata)
		{
			Regex regex = new Regex("((((?:\\s*)\\[([^\\[\\]\\{\\}\\s]+)\\])(?:\\s*)))", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
			string a = "";
			MatchCollection matchCollection = regex.Matches(name);
			if (matchCollection.Count > 0)
			{
				a = matchCollection[matchCollection.Count - 1].Groups[4].ToString();
				foreach (Match item in matchCollection)
				{
					name = name.Replace(item.Groups[0].ToString(), "");
				}
			}
			Regex regex2 = new Regex("((((?:\\s*)" + name + "\\[([^\\[\\]\\{\\}\\s]+)\\])(?:\\s*)))", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
			foreach (string key in HttpContext.Current.Request.Params.Keys)
			{
				MatchCollection matchCollection2 = regex2.Matches(key);
				if (matchCollection2.Count > 0)
				{
					if (a != "")
					{
						if (matchCollection2.Count > 1)
						{
							string b = matchCollection2[matchCollection2.Count - 2].Groups[4].ToString();
							if (a == b)
							{
								fpdata[matchCollection2[matchCollection2.Count - 1].Groups[4].ToString()] = GetString(key);
							}
						}
					}
					else
					{
						fpdata[matchCollection2[matchCollection2.Count - 1].Groups[4].ToString()] = GetString(key);
					}
				}
			}
			return fpdata;
		}

		public static FPData GetFPData(FPData fpdata)
		{
			foreach (KeyValuePair<string, string> datum in fpdata.Data)
			{
				string key = datum.Key;
				if (HttpContext.Current.Request.QueryString[key] != null || HttpContext.Current.Request.Form[key] != null)
				{
					fpdata[datum.Key] = GetString(key);
				}
			}
			return fpdata;
		}

		public static List<FPData> GetFPList(string name)
		{
			List<FPData> list = new List<FPData>();
			int num = 0;
			bool flag = true;
			while (flag)
			{
				FPData fPData = GetFPData(name + "[" + num + "]");
				if (fPData.Count == 0)
				{
					flag = false;
					break;
				}
				list.Add(fPData);
				num++;
			}
			return list;
		}

		public static List<FPData> GetFPList(FPData fpdata)
		{
			List<FPData> list = new List<FPData>();
			int num = 0;
			bool flag = true;
			while (flag)
			{
				FPData fPData = new FPData();
				foreach (KeyValuePair<string, string> datum in fpdata.Data)
				{
					string text = datum.Key + "[" + num + "]";
					if (HttpContext.Current.Request.QueryString[text] != null || HttpContext.Current.Request.Form[text] != null)
					{
						fPData[datum.Key] = GetString(text);
					}
				}
				if (fpdata.Count == 0)
				{
					flag = false;
					break;
				}
				list.Add(fpdata);
				num++;
			}
			return list;
		}

		public static string GetContent()
		{
			string text = "";
			HttpContext.Current.Response.ContentType = "application/json";
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			using (StreamReader streamReader = new StreamReader(HttpContext.Current.Request.InputStream))
			{
				return HttpUtility.UrlDecode(streamReader.ReadToEnd());
			}
		}

		public static List<T> GetToList<T>() where T : new()
		{
			return FPJson.ToList<T>(GetContent());
		}

		public static string GetIP()
		{
			try
			{
				string empty = string.Empty;
				empty = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
				if (empty == null || empty == string.Empty)
				{
					empty = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
				}
				if (empty == null || empty == string.Empty)
				{
					empty = HttpContext.Current.Request.UserHostAddress;
				}
				if (empty == null || empty == string.Empty || !FPUtils.IsIP(empty))
				{
					return "0.0.0.0";
				}
				return empty;
			}
			catch
			{
				return "0.0.0.0";
			}
		}

		public static string GetServerIP()
		{
			try
			{
				string empty = string.Empty;
				empty = HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];
				if (empty == null || empty == string.Empty || !FPUtils.IsIP(empty))
				{
					return "0.0.0.0";
				}
				if (empty == "127.0.0.1" || empty == "0.0.0.0")
				{
					empty = FPUtils.GetIntranetIp();
				}
				return empty;
			}
			catch
			{
				return "0.0.0.0";
			}
		}

		public static string GetServerIIS()
		{
			try
			{
				string empty = string.Empty;
				empty = HttpContext.Current.Request.ServerVariables["SERVER_SOFTWARE"];
				if (string.IsNullOrEmpty(empty))
				{
					return "";
				}
				return empty;
			}
			catch
			{
				return "";
			}
		}
	}
}
