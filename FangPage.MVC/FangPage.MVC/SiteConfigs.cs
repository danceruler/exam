using FangPage.Common;
using System.Collections.Generic;
using System.IO;

namespace FangPage.MVC
{
	public class SiteConfigs
	{
		private static object lockHelper = new object();

		public static List<SiteConfig> GetSysSiteList()
		{
			List<SiteConfig> list = new List<SiteConfig>();
			string mapPath = FPFile.GetMapPath(WebConfig.WebPath);
			if (Directory.Exists(mapPath))
			{
				DirectoryInfo[] directories = new DirectoryInfo(mapPath).GetDirectories();
				foreach (DirectoryInfo directoryInfo in directories)
				{
					if (File.Exists(directoryInfo.FullName + "\\site.config"))
					{
						SiteConfig siteConfig = FPSerializer.Load<SiteConfig>(directoryInfo.FullName + "\\site.config");
						siteConfig.sitepath = directoryInfo.Name;
						list.Add(siteConfig);
					}
				}
			}
			return list;
		}

		public static List<SiteConfig> GetMapSiteList()
		{
			List<SiteConfig> list = new List<SiteConfig>();
			string mapPath = FPFile.GetMapPath(WebConfig.WebPath + "sites");
			if (Directory.Exists(mapPath))
			{
				DirectoryInfo[] directories = new DirectoryInfo(mapPath).GetDirectories();
				foreach (DirectoryInfo directoryInfo in directories)
				{
					if (File.Exists(directoryInfo.FullName + "\\site.config") && directoryInfo.Name.ToLower() != "app" && directoryInfo.Name.ToLower() != "plugins")
					{
						SiteConfig siteConfig = FPSerializer.Load<SiteConfig>(directoryInfo.FullName + "\\site.config");
						siteConfig.sitepath = directoryInfo.Name;
						string mapPath2 = FPFile.GetMapPath(WebConfig.WebPath + directoryInfo.Name);
						if (Directory.Exists(mapPath2))
						{
							siteConfig.size = FPFile.FormatBytesStr(FPFile.GetDirSize(mapPath2));
						}
						else
						{
							siteConfig.size = FPFile.FormatBytesStr(FPFile.GetDirSize(directoryInfo.FullName));
						}
						list.Add(siteConfig);
					}
				}
			}
			return list;
		}

		public static SiteConfig GetMapSiteConfig(string sitepath)
		{
			SiteConfig result = new SiteConfig();
			string mapPath = FPFile.GetMapPath(WebConfig.WebPath + "sites/" + sitepath + "/site.config");
			if (File.Exists(mapPath))
			{
				result = FPSerializer.Load<SiteConfig>(mapPath);
			}
			return result;
		}

		public static SiteConfig GetSiteConfig(string guid)
		{
			SiteConfig result = new SiteConfig();
			List<SiteConfig> mapSiteList = GetMapSiteList();
			for (int i = 0; i < mapSiteList.Count; i++)
			{
				if (mapSiteList[i].guid.ToLower() == guid.ToLower())
				{
					result = mapSiteList[i];
					break;
				}
			}
			return result;
		}

		public static List<SiteConfig> GetSiteList()
		{
			object obj = FPCache.Get("FP_SITELIST");
			List<SiteConfig> list = new List<SiteConfig>();
			if (obj != null)
			{
				return obj as List<SiteConfig>;
			}
			return new List<SiteConfig>();
		}

		public static SiteConfig GetSiteInfo(string sitepath)
		{
			List<SiteConfig> list = GetSiteList().FindAll((SiteConfig item) => item.sitepath.ToLower() == sitepath.ToLower());
			if (list.Count > 0)
			{
				return list[0];
			}
			SiteConfig siteConfig = LoadSiteConfig(sitepath);
			if (siteConfig.guid != "")
			{
				FPCache.Remove("FP_SITELIST");
				list.Add(siteConfig);
				FPCache.Insert("FP_SITELIST", list);
			}
			return siteConfig;
		}

		public static SiteConfig LoadSiteConfig(string sitepath)
		{
			SiteConfig siteConfig = new SiteConfig();
			if (sitepath.ToLower() == "app")
			{
				siteConfig.name = "系统应用";
				siteConfig.sitepath = "app";
				siteConfig.version = "1.0.0";
				siteConfig.urltype = 0;
			}
			else if (sitepath.ToLower() == "plugins")
			{
				siteConfig.name = "系统插件";
				siteConfig.sitepath = "plugins";
				siteConfig.version = "1.0.0";
				siteConfig.urltype = 0;
			}
			else if (File.Exists(FPFile.GetMapPath(WebConfig.WebPath + "sites/" + sitepath + "/site.config")))
			{
				siteConfig = FPSerializer.Load<SiteConfig>(FPFile.GetMapPath(WebConfig.WebPath + "sites/" + sitepath + "/site.config"));
			}
			else if (File.Exists(FPFile.GetMapPath(WebConfig.WebPath + sitepath + "/site.config")))
			{
				siteConfig = FPSerializer.Load<SiteConfig>(FPFile.GetMapPath(WebConfig.WebPath + sitepath + "/site.config"));
			}
			else if (File.Exists(FPFile.GetMapPath(WebConfig.WebPath + "/site.config")))
			{
				siteConfig = FPSerializer.Load<SiteConfig>(FPFile.GetMapPath(WebConfig.WebPath + "/site.config"));
			}
			return siteConfig;
		}

		public static void SaveSiteConfig(SiteConfig siteconfig)
		{
			string mapPath = FPFile.GetMapPath(WebConfig.WebPath + "sites/" + siteconfig.sitepath + "/site.config");
			SaveSiteConfig(siteconfig, mapPath);
		}

		public static void SaveSiteConfig(SiteConfig siteconfig, string configfilepath)
		{
			List<SiteConfig> siteList = GetSiteList();
			FPCache.Remove("FP_SITELIST");
			if (siteList.Count == 0)
			{
				siteList.Add(siteconfig);
			}
			else
			{
				bool flag = false;
				for (int i = 0; i < siteList.Count; i++)
				{
					if (siteList[i].sitepath.ToLower() == siteconfig.sitepath.ToLower())
					{
						siteList[i] = siteconfig;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					siteList.Add(siteconfig);
				}
			}
			FPCache.Insert("FP_SITELIST", siteList);
			FPSerializer.Save(siteconfig, configfilepath);
		}
	}
}
