using System.Configuration;

namespace FangPage.MVC
{
	public class WebConfig
	{
		private static string m_webpath;

		private static string m_sitepath;

		public static string WebPath => m_webpath;

		public static string SitePath => m_sitepath;

		static WebConfig()
		{
			m_webpath = "";
			m_sitepath = "";
			ReSet();
		}

		public static void ReSet()
		{
			m_webpath = ConfigurationManager.AppSettings["webpath"];
			if (string.IsNullOrEmpty(m_webpath))
			{
				m_webpath = "/";
			}
			if (!m_webpath.StartsWith("/"))
			{
				m_webpath = "/" + m_webpath;
			}
			if (!m_webpath.EndsWith("/"))
			{
				m_webpath += "/";
			}
			m_sitepath = ConfigurationManager.AppSettings["sitepath"];
			if (string.IsNullOrEmpty(m_sitepath))
			{
				m_sitepath = "";
			}
		}
	}
}
