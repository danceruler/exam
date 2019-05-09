namespace FangPage.MVC
{
	public class SiteConfig
	{
		private string m_name = "";

		private string m_guid = string.Empty;

		private string m_markup = string.Empty;

		private string m_platform = string.Empty;

		private string m_sitepath = "";

		private string m_author = "方配";

		private string m_import = "";

		private string m_dll = "";

		private int m_urltype = 1;

		private string m_notes = "";

		private string m_version = "1.0.0";

		private string m_sitetitle = "";

		private string m_keywords = "";

		private string m_description = "";

		private string m_otherhead = "";

		private string m_copyright = "";

		private string m_homepage = "";

		private string m_adminurl = "";

		private string m_indexurl = "";

		private string m_icon = "";

		private int m_autocreate = 1;

		private int m_closed;

		private string m_closedreason = "站点正在升级，请稍后再访问！";

		private string m_ipdenyaccess = "";

		private string m_ipaccess = "";

		private string m_createdate = string.Empty;

		private string m_updatedate = string.Empty;

		private string m_size = string.Empty;

		private string m_roles = "";

		public string name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public string guid
		{
			get
			{
				return m_guid;
			}
			set
			{
				m_guid = value;
			}
		}

		public string markup
		{
			get
			{
				if (m_markup == "" && sitepath != "")
				{
					m_markup = "sites_" + sitepath;
				}
				return m_markup;
			}
			set
			{
				m_markup = value;
			}
		}

		public string platform
		{
			get
			{
				return m_platform;
			}
			set
			{
				m_platform = value;
			}
		}

		public string sitepath
		{
			get
			{
				return m_sitepath;
			}
			set
			{
				m_sitepath = value;
			}
		}

		public string author
		{
			get
			{
				return m_author;
			}
			set
			{
				m_author = value;
			}
		}

		public string import
		{
			get
			{
				return m_import;
			}
			set
			{
				m_import = value;
			}
		}

		public string dll
		{
			get
			{
				return m_dll;
			}
			set
			{
				m_dll = value;
			}
		}

		public int urltype
		{
			get
			{
				return m_urltype;
			}
			set
			{
				m_urltype = value;
			}
		}

		public string notes
		{
			get
			{
				return m_notes;
			}
			set
			{
				m_notes = value;
			}
		}

		public string version
		{
			get
			{
				return m_version;
			}
			set
			{
				m_version = value;
			}
		}

		public string sitetitle
		{
			get
			{
				if (m_sitetitle == "")
				{
					m_sitetitle = name;
				}
				return m_sitetitle;
			}
			set
			{
				m_sitetitle = value;
			}
		}

		public string keywords
		{
			get
			{
				return m_keywords;
			}
			set
			{
				m_keywords = value;
			}
		}

		public string description
		{
			get
			{
				return m_description;
			}
			set
			{
				m_description = value;
			}
		}

		public string otherhead
		{
			get
			{
				return m_otherhead;
			}
			set
			{
				m_otherhead = value;
			}
		}

		public string copyright
		{
			get
			{
				return m_copyright;
			}
			set
			{
				m_copyright = value;
			}
		}

		public string homepage
		{
			get
			{
				return m_homepage;
			}
			set
			{
				m_homepage = value;
			}
		}

		public string adminurl
		{
			get
			{
				return m_adminurl;
			}
			set
			{
				m_adminurl = value;
			}
		}

		public string indexurl
		{
			get
			{
				return m_indexurl;
			}
			set
			{
				m_indexurl = value;
			}
		}

		public string icon
		{
			get
			{
				return m_icon;
			}
			set
			{
				m_icon = value;
			}
		}

		public int autocreate
		{
			get
			{
				return m_autocreate;
			}
			set
			{
				m_autocreate = value;
			}
		}

		public int closed
		{
			get
			{
				return m_closed;
			}
			set
			{
				m_closed = value;
			}
		}

		public string closedreason
		{
			get
			{
				return m_closedreason;
			}
			set
			{
				m_closedreason = value;
			}
		}

		public string ipdenyaccess
		{
			get
			{
				return m_ipdenyaccess;
			}
			set
			{
				m_ipdenyaccess = value;
			}
		}

		public string ipaccess
		{
			get
			{
				return m_ipaccess;
			}
			set
			{
				m_ipaccess = value;
			}
		}

		public string createdate
		{
			get
			{
				return m_createdate;
			}
			set
			{
				m_createdate = value;
			}
		}

		public string updatedate
		{
			get
			{
				return m_updatedate;
			}
			set
			{
				m_updatedate = value;
			}
		}

		public string size
		{
			get
			{
				return m_size;
			}
			set
			{
				m_size = value;
			}
		}

		public string roles
		{
			get
			{
				return m_roles;
			}
			set
			{
				m_roles = value;
			}
		}
	}
}
