using FangPage.Common;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace FangPage.MVC
{
	public class FPController : Page
	{
		protected StringBuilder ViewBuilder = new StringBuilder();

		protected int loop__id;

		protected string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

		protected string serverip = FPRequest.GetServerIP();

		protected string iis = FPRequest.GetServerIIS();

		protected string ip = FPRequest.GetIP();

		protected string domain = FPRequest.GetDomain().ToLower();

		protected int port = FPRequest.GetPort();

		protected string random = FPRandom.CreateCode(4);

		protected string webpath = WebConfig.WebPath;

		protected string sitepath = "";

		protected SiteConfig siteinfo = new SiteConfig();

		protected string rawurl = FPRequest.GetRawUrl();

		protected string rawpath = "";

		protected string cururl = "";

		protected string curpath = "";

		protected string curname = "";

		protected string pagepath = "";

		protected string pageurl = "";

		protected string pagename = FPRequest.GetPageName();

		protected string sitetitle = "";

		protected string pagetitle = "";

		protected string query = "";

		protected string meta = "";

		protected string link = "";

		protected string script = "";

		protected string pagenav = "";

		protected string adminpath = "";

		protected string apppath = "";

		protected string plupath = "";

		protected string backurl = FPRequest.GetString("backurl");

		protected bool ispost;

		protected bool isget;

		protected bool isfile;

		protected int err;

		protected string msg = "";

		protected string action = "";

		protected int op;

		protected string ua = "";

		protected string browser = "";

		protected int isie;

		protected string[] args;

		protected int iswrite;

		public FPController()
		{
			port = FPArray.SplitInt(domain, ":", 2)[1];
			if (rawurl.IndexOf("/") >= 0)
			{
				if (rawurl.IndexOf("?") >= 0)
				{
					rawpath = rawurl.Substring(0, rawurl.IndexOf("?"));
					rawpath = rawpath.Substring(0, rawpath.LastIndexOf("/")) + "/";
				}
				else
				{
					rawpath = rawurl.Substring(0, rawurl.LastIndexOf("/")) + "/";
				}
			}
			else
			{
				rawpath = webpath;
			}
			cururl = rawurl.Substring(webpath.Length);
			pageurl = pagename;
			if (cururl.Contains("?"))
			{
				curname = cururl.Substring(0, cururl.IndexOf("?"));
				query = cururl.Substring(cururl.IndexOf("?") + 1);
				pageurl = pagename + "?" + query;
			}
			else
			{
				curname = cururl;
			}
			if (curname.IndexOf("/") > 0)
			{
				curpath = curname.Substring(0, curname.LastIndexOf("/")) + "/";
			}
			if (curname.IndexOf("/") >= 0)
			{
				sitepath = curname.Substring(0, curname.IndexOf("/"));
			}
			else
			{
				sitepath = WebConfig.SitePath;
			}
			if (sitepath == "sites")
			{
				sitepath = curpath.Substring(curpath.IndexOf("/") + 1).TrimEnd('/');
			}
			if (!Directory.Exists(FPFile.GetMapPath(webpath + sitepath)))
			{
				sitepath = WebConfig.SitePath;
			}
			pagepath = webpath + sitepath + "/";
			siteinfo = SiteConfigs.GetSiteInfo(sitepath);
			adminpath = webpath + "admin/";
			plupath = webpath + "plugins/";
			apppath = webpath + "app/";
			sitetitle = siteinfo.sitetitle;
			pagetitle = siteinfo.sitetitle;
			CreateSeoInfo(siteinfo.keywords, siteinfo.description, siteinfo.otherhead);
			ispost = FPRequest.IsPost();
			isget = FPRequest.IsGet();
			isfile = FPRequest.IsPostFile();
			action = FPRequest.GetString("action");
			op = FPRequest.GetInt("op");
			try
			{
				ua = HttpContext.Current.Request.UserAgent.ToLower();
			}
			catch
			{
			}
			browser = getBrowserName(ua, out isie);
			args = FPArray.SplitString(Path.GetFileNameWithoutExtension(pagename), "-");
		}

		protected override void OnPreInit(EventArgs e)
		{
			Init();
			Controller();
			PreView();
			View();
			Complete();
			HttpContext.Current.Response.End();
		}

		protected virtual void Init()
		{
		}

		protected virtual void Controller()
		{
		}

		protected virtual void PreView()
		{
		}

		protected virtual void View()
		{
		}

		protected virtual void Complete()
		{
		}

		private string getBrowserName(string ua, out int iever)
		{
			iever = 0;
			if (string.IsNullOrEmpty(ua))
			{
				return "";
			}
			if (ua.IndexOf("msie 6") >= 0)
			{
				iever = 6;
				return "ie6";
			}
			if (ua.IndexOf("msie 7") >= 0)
			{
				iever = 7;
				return "ie7";
			}
			if (ua.IndexOf("msie 8") >= 0)
			{
				iever = 8;
				return "ie8";
			}
			if (ua.IndexOf("msie 9") >= 0)
			{
				iever = 9;
				return "ie9";
			}
			if (ua.IndexOf("msie 10") >= 0)
			{
				iever = 10;
				return "ie10";
			}
			if (ua.IndexOf("trident/7.0; rv:11.0") >= 0)
			{
				iever = 11;
				return "ie11";
			}
			if (ua.IndexOf("edge/") >= 0)
			{
				iever = 12;
				return "ie12";
			}
			if (ua.IndexOf("chrome") >= 0)
			{
				return "chrome";
			}
			if (ua.IndexOf("firefox") >= 0)
			{
				return "firefox";
			}
			if (ua.IndexOf("opera") >= 0)
			{
				return "opera";
			}
			if (ua.IndexOf("webkit") >= 0)
			{
				return "webkit";
			}
			return ua;
		}

		protected string GetThumbnail(string imgpath, int maxsize)
		{
			return FPThumb.GetThumbnail(imgpath, maxsize);
		}

		protected void AddErr(string errinfo)
		{
			if (msg.Length == 0)
			{
				msg += errinfo;
			}
			else
			{
				msg = msg + "<br />" + errinfo;
			}
			err++;
		}

		protected void AddMsg(string strinfo)
		{
			if (msg.Length == 0)
			{
				msg += strinfo;
			}
			else
			{
				msg = msg + "<br />" + strinfo;
			}
		}

		public void SetMetaRefresh()
		{
			SetMetaRefresh(2, pagename);
		}

		public void SetMetaRefresh(int sec)
		{
			SetMetaRefresh(sec, "index.aspx");
		}

		public void SetMetaRefresh(int sec, string url)
		{
			meta = meta + "\r\n<meta http-equiv=\"refresh\" content=\"" + sec.ToString() + "; url=" + url + "\" />";
		}

		public void AddMeta(string metastr)
		{
			meta = meta + "\r\n<meta " + metastr + " />";
		}

		private void CreateSeoInfo(string Seokeywords, string Seodescription, string Otherhead)
		{
			if (Seokeywords != "")
			{
				meta = meta + "<meta name=\"keywords\" content=\"" + FPUtils.RemoveHtml(Seokeywords).Replace("\"", " ") + "\" />\r\n";
			}
			if (Seodescription != "")
			{
				meta = meta + "<meta name=\"description\" content=\"" + FPUtils.RemoveHtml(Seodescription).Replace("\"", " ") + "\" />\r\n";
			}
			meta += Otherhead;
		}

		public void UpdateSeoInfo(string Seokeywords, string Seodescription)
		{
			string[] array = FPArray.SplitString(meta, "\r\n");
			meta = "";
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.ToLower().IndexOf("name=\"keywords\"") > 0 && Seokeywords != null && Seokeywords.Trim() != "")
				{
					meta = meta + "<meta name=\"keywords\" content=\"" + FPUtils.RemoveHtml(Seokeywords).Replace("\"", " ") + "\" />\r\n";
				}
				else if (text.ToLower().IndexOf("name=\"description\"") > 0 && Seodescription != null && Seodescription.Trim() != "")
				{
					meta = meta + "<meta name=\"description\" content=\"" + FPUtils.RemoveHtml(Seodescription).Replace("\"", " ") + "\" />\r\n";
				}
				else
				{
					meta = meta + text + "\r\n";
				}
			}
		}

		private void AddSeoInfo(string Seokeywords, string Seodescription)
		{
			string[] array = FPArray.SplitString(meta, "\r\n");
			meta = "";
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.ToLower().IndexOf("name=\"keywords\"") > 0 && Seokeywords != null && Seokeywords.Trim() != "")
				{
					meta = meta + "<meta name=\"keywords\" content=\"" + FPUtils.RemoveHtml(Seokeywords + "," + siteinfo.keywords).Replace("\"", " ") + "\" />\r\n";
				}
				else if (text.ToLower().IndexOf("name=\"description\"") > 0 && Seodescription != null && Seodescription.Trim() != "")
				{
					meta = meta + "<meta name=\"description\" content=\"" + FPUtils.RemoveHtml(siteinfo.description + "," + Seodescription).Replace("\"", " ") + "\" />\r\n";
				}
				else
				{
					meta = meta + text + "\r\n";
				}
			}
		}

		public void AddLinkCss(string url)
		{
			link = link + "\r\n<link href=\"" + url + "\" rel=\"stylesheet\" type=\"text/css\" />";
		}

		public void AddScript(string script_str)
		{
			script = script + "\r\n<script type=\"text/javascript\">" + script_str + "</script>";
		}

		public void AddLinkScript(string script_src)
		{
			script = script + "\r\n<script type=\"text/javascript\" src=\"" + script_src + "\"></script>";
		}

		protected string seturl(string param)
		{
			string text = "";
			param = param.Trim();
			if (query != "")
			{
				string text2 = "";
				string[] array = FPArray.SplitString(query, "&");
				foreach (string text3 in array)
				{
					bool flag = true;
					string[] array2 = FPArray.SplitString(param, "&");
					for (int j = 0; j < array2.Length; j++)
					{
						string[] array3 = FPArray.SplitString(array2[j], "=", 2);
						if (text3.StartsWith(array3[0] + "="))
						{
							flag = false;
						}
					}
					if (flag)
					{
						text2 = FPArray.Push(text2, text3, "&");
					}
				}
				string text4 = "";
				array = FPArray.SplitString(param, "&");
				foreach (string text5 in array)
				{
					string[] array4 = FPArray.SplitString(text5, "=", 2);
					if (array4[1] != "" && array4[1] != "0")
					{
						text4 = FPArray.Push(text4, text5, "&");
					}
				}
				text = pagename;
				if (text2 != "" || text4 != "")
				{
					text += "?";
				}
				if (text2 != "")
				{
					text = ((!(text4 != "")) ? (text + text2) : (text + text2 + "&" + text4));
				}
				else if (text4 != "")
				{
					text += text4;
				}
			}
			else
			{
				string text6 = "";
				string[] array = param.Trim().Split('&');
				foreach (string text7 in array)
				{
					string[] array5 = FPArray.SplitString(text7, "=", 2);
					if (array5[1] != "" && array5[1] != "0")
					{
						text6 = FPArray.Push(text6, text7, "&");
					}
				}
				if (text6 != "")
				{
					text = pagename + "?" + text6;
				}
			}
			return text;
		}

		public static bool isnew(DateTime postdatetime, int days)
		{
			try
			{
				if (days >= 0)
				{
					DateTime.Now.ToString("yyyy-MM-dd");
					Convert.ToDateTime(postdatetime).ToString("yyyy-MM-dd");
					return DateTime.Now.Subtract(postdatetime).TotalDays < (double)days;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

		public static string echo(string obj)
		{
			if (string.IsNullOrEmpty(obj))
			{
				return "";
			}
			return obj;
		}

		public static string echo(string[] array)
		{
			return echo(FPArray.Join(array));
		}

		public static string echo(string obj, int len)
		{
			if (!string.IsNullOrEmpty(obj))
			{
				return FPUtils.CutString(obj, len);
			}
			return obj;
		}

		public static string echo(int obj)
		{
			return obj.ToString();
		}

		public static string echo(int[] array)
		{
			return echo(FPArray.Join(array));
		}

		public static string echo(DateTime obj)
		{
			return FPUtils.FormatDateTime(obj);
		}

		public static string echo(DateTime obj, string format)
		{
			return FPUtils.FormatDateTime(obj, format);
		}

		public static string echo(string obj, string format)
		{
			return FPUtils.FormatDateTime(obj, format);
		}

		public static string echo(DateTime? obj)
		{
			return FPUtils.FormatDateTime(obj);
		}

		public static string echo(DateTime? obj, string fmstr)
		{
			return FPUtils.FormatDateTime(obj, fmstr);
		}

		public static string echo(object obj)
		{
			if (obj != null)
			{
				return obj.ToString().Trim();
			}
			return "";
		}

		public static string echo(string obj, string oldStr, string newStr)
		{
			if (string.IsNullOrEmpty(obj))
			{
				return "";
			}
			if (string.IsNullOrEmpty(oldStr))
			{
				return obj;
			}
			string[] array = FPArray.SplitString(oldStr, "|");
			string[] array2 = FPArray.SplitString(newStr, "|", array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != "")
				{
					obj = obj.Replace(array[i], array2[i]);
				}
			}
			return obj;
		}

		public static string echo(string obj, string oldStr, int newStr)
		{
			return echo(obj, oldStr, newStr.ToString());
		}
	}
}
