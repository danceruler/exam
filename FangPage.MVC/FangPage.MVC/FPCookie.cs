using System;
using System.Web;

namespace FangPage.MVC
{
	public class FPCookie
	{
		public static void WriteCookie(string strName, string strValue)
		{
			HttpCookie httpCookie = HttpContext.Current.Request.Cookies[strName];
			if (httpCookie == null)
			{
				httpCookie = new HttpCookie(strName);
			}
			httpCookie.Value = strValue;
			HttpContext.Current.Response.AppendCookie(httpCookie);
		}

		public static void WriteCookie(string strName, string key, string strValue)
		{
			HttpCookie httpCookie = HttpContext.Current.Request.Cookies[strName];
			if (httpCookie == null)
			{
				httpCookie = new HttpCookie(strName);
			}
			httpCookie[key] = strValue;
			HttpContext.Current.Response.AppendCookie(httpCookie);
		}

		public static void WriteCookie(string strName, string strValue, int expires)
		{
			HttpCookie httpCookie = HttpContext.Current.Request.Cookies[strName];
			if (httpCookie == null)
			{
				httpCookie = new HttpCookie(strName);
			}
			httpCookie.Value = strValue;
			httpCookie.Expires = DateTime.Now.AddMinutes(expires);
			HttpContext.Current.Response.AppendCookie(httpCookie);
		}

		public static string GetCookie(string strName)
		{
			if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
			{
				return HttpContext.Current.Request.Cookies[strName].Value.ToString();
			}
			return "";
		}

		public static string GetCookie(string strName, string key)
		{
			if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null && HttpContext.Current.Request.Cookies[strName][key] != null)
			{
				return HttpContext.Current.Request.Cookies[strName][key].ToString();
			}
			return "";
		}
	}
}
