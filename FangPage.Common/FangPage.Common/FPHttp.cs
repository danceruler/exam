using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace FangPage.Common
{
	public sealed class FPHttp : IDisposable
	{
		private int _timeout = 20000;

		private int _readWriteTimeout = 60000;

		private bool _ignoreSSLCheck = true;

		public int Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				_timeout = value;
			}
		}

		public int ReadWriteTimeout
		{
			get
			{
				return _readWriteTimeout;
			}
			set
			{
				_readWriteTimeout = value;
			}
		}

		public bool IgnoreSSLCheck
		{
			get
			{
				return _ignoreSSLCheck;
			}
			set
			{
				_ignoreSSLCheck = value;
			}
		}

		public string Post(string url)
		{
			return Post(url, "");
		}

		public string Post(string url, FPData query)
		{
			try
			{
				string text = BuildQuery(query);
				HttpWebRequest webRequest = GetWebRequest(url, "POST", null);
				webRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
				if (!string.IsNullOrEmpty(text))
				{
					byte[] bytes = Encoding.UTF8.GetBytes(text);
					Stream requestStream = webRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Flush();
					requestStream.Close();
				}
				else
				{
					webRequest.ContentLength = 0L;
				}
				HttpWebResponse rsp = (HttpWebResponse)webRequest.GetResponse();
				Encoding responseEncoding = GetResponseEncoding(rsp);
				return GetResponseAsString(rsp, responseEncoding);
			}
			catch (Exception ex)
			{
				return "Error:" + ex.Message;
			}
		}

		public string Post(string url, string content)
		{
			try
			{
				HttpWebRequest webRequest = GetWebRequest(url, "POST", null);
				webRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
				if (!string.IsNullOrEmpty(content))
				{
					byte[] bytes = Encoding.UTF8.GetBytes(content);
					Stream requestStream = webRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Flush();
					requestStream.Close();
				}
				else
				{
					webRequest.ContentLength = 0L;
				}
				HttpWebResponse rsp = (HttpWebResponse)webRequest.GetResponse();
				Encoding responseEncoding = GetResponseEncoding(rsp);
				return GetResponseAsString(rsp, responseEncoding);
			}
			catch (Exception ex)
			{
				return "Error:" + ex.Message;
			}
		}

		public string PostJson(string url, object obj)
		{
			try
			{
				string text = FPJson.ToJson(obj);
				HttpWebRequest webRequest = GetWebRequest(url, "POST", null);
				webRequest.ContentType = "application/json;charset=utf-8";
				if (!string.IsNullOrEmpty(text))
				{
					byte[] bytes = Encoding.UTF8.GetBytes(text);
					Stream requestStream = webRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Flush();
					requestStream.Close();
				}
				else
				{
					webRequest.ContentLength = 0L;
				}
				HttpWebResponse rsp = (HttpWebResponse)webRequest.GetResponse();
				Encoding responseEncoding = GetResponseEncoding(rsp);
				return GetResponseAsString(rsp, responseEncoding);
			}
			catch (Exception ex)
			{
				return "Error:" + ex.Message;
			}
		}

		public string PostModel<T>(string url, T model) where T : new()
		{
			Type typeFromHandle = typeof(T);
			FPData fPData = new FPData();
			PropertyInfo[] properties = typeFromHandle.GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CanRead)
				{
					object value = propertyInfo.GetValue(model, null);
					if (value != null)
					{
						fPData.Add(propertyInfo.Name, value.ToString());
					}
				}
			}
			return Post(url, fPData);
		}

		public T GetModel<T>(string url, FPData query) where T : new()
		{
			string json = Post(url, query);
			return FPJson.ToModel<T>(json);
		}

		public List<T> GetList<T>(string url, FPData query) where T : new()
		{
			string json = Post(url, query);
			return FPJson.ToList<T>(json);
		}

		public string Get(string url)
		{
			try
			{
				HttpWebRequest webRequest = GetWebRequest(url, "GET", null);
				webRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
				HttpWebResponse rsp = (HttpWebResponse)webRequest.GetResponse();
				Encoding responseEncoding = GetResponseEncoding(rsp);
				return GetResponseAsString(rsp, responseEncoding);
			}
			catch (Exception ex)
			{
				return "Error:" + ex.Message;
			}
		}

		public string Get(string url, FPData query)
		{
			try
			{
				if (query != null && query.Count > 0)
				{
					url = BuildRequestUrl(url, query);
				}
				HttpWebRequest webRequest = GetWebRequest(url, "GET", null);
				webRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
				HttpWebResponse rsp = (HttpWebResponse)webRequest.GetResponse();
				Encoding responseEncoding = GetResponseEncoding(rsp);
				return GetResponseAsString(rsp, responseEncoding);
			}
			catch (Exception ex)
			{
				return "Error:" + ex.Message;
			}
		}

		public string Get(string url, string query)
		{
			try
			{
				if (!string.IsNullOrEmpty(query))
				{
					url = BuildRequestUrl(url, query);
				}
				HttpWebRequest webRequest = GetWebRequest(url, "GET", null);
				webRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
				HttpWebResponse rsp = (HttpWebResponse)webRequest.GetResponse();
				Encoding responseEncoding = GetResponseEncoding(rsp);
				return GetResponseAsString(rsp, responseEncoding);
			}
			catch (Exception ex)
			{
				return "Error:" + ex.Message;
			}
		}

		public HttpWebRequest GetWebRequest(string url, string method, FPData headerParams)
		{
			HttpWebRequest httpWebRequest = null;
			if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
			{
				if (_ignoreSSLCheck)
				{
					ServicePointManager.ServerCertificateValidationCallback = TrustAllValidationCallback;
				}
				httpWebRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
			}
			else
			{
				httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			}
			if (headerParams != null && headerParams.Count > 0)
			{
				foreach (string key in headerParams.Data.Keys)
				{
					httpWebRequest.Headers.Add(key, headerParams[key]);
				}
			}
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.Method = method;
			httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
			httpWebRequest.Timeout = _timeout;
			httpWebRequest.ReadWriteTimeout = _readWriteTimeout;
			return httpWebRequest;
		}

		public string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
		{
			Stream stream = null;
			StreamReader streamReader = null;
			try
			{
				stream = rsp.GetResponseStream();
				if ("gzip".Equals(rsp.ContentEncoding, StringComparison.OrdinalIgnoreCase))
				{
					stream = new GZipStream(stream, CompressionMode.Decompress);
				}
				streamReader = new StreamReader(stream, encoding);
				return streamReader.ReadToEnd();
			}
			finally
			{
				streamReader?.Close();
				stream?.Close();
				rsp?.Close();
			}
		}

		public static string BuildRequestUrl(string url, FPData parameters)
		{
			if (parameters != null && parameters.Count > 0)
			{
				return BuildRequestUrl(url, BuildQuery(parameters));
			}
			return url;
		}

		public static string BuildRequestUrl(string url, params string[] queries)
		{
			if (queries == null || queries.Length == 0)
			{
				return url;
			}
			StringBuilder stringBuilder = new StringBuilder(url);
			bool flag = url.Contains("?");
			bool flag2 = url.EndsWith("?") || url.EndsWith("&");
			foreach (string value in queries)
			{
				if (string.IsNullOrEmpty(value))
				{
					continue;
				}
				if (!flag2)
				{
					if (flag)
					{
						stringBuilder.Append("&");
					}
					else
					{
						stringBuilder.Append("?");
						flag = true;
					}
				}
				stringBuilder.Append(value);
				flag2 = false;
			}
			return stringBuilder.ToString();
		}

		public static string BuildQuery(FPData parameters)
		{
			if (parameters == null || parameters.Count == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (KeyValuePair<string, string> datum in parameters.Data)
			{
				string key = datum.Key;
				string value = datum.Value;
				if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
				{
					if (flag)
					{
						stringBuilder.Append("&");
					}
					stringBuilder.Append(key);
					stringBuilder.Append("=");
					stringBuilder.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
					flag = true;
				}
			}
			return stringBuilder.ToString();
		}

		private Encoding GetResponseEncoding(HttpWebResponse rsp)
		{
			string text = rsp.CharacterSet;
			if (string.IsNullOrEmpty(text))
			{
				text = "utf-8";
			}
			return Encoding.GetEncoding(text);
		}

		private static bool TrustAllValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
		{
			return true;
		}

		public void Dispose()
		{
		}
	}
}
