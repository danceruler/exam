using FangPage.Common;
using System.Collections.Generic;
using System.IO;

namespace FangPage.MVC
{
	public class ViewConfigs
	{
		public static List<ViewConfig> GetViewList()
		{
			object obj = FPCache.Get("FP_VIEWLIST");
			List<ViewConfig> list = new List<ViewConfig>();
			if (obj != null)
			{
				list = (obj as List<ViewConfig>);
			}
			else
			{
				string mapPath = FPFile.GetMapPath(WebConfig.WebPath + "config/view.config");
				list = FPXml.LoadList<ViewConfig>(mapPath);
				FPCache.Insert("FP_VIEWLIST", list, mapPath);
			}
			return list;
		}

		public static ViewConfig GetViewInfo(string path)
		{
			List<ViewConfig> list = GetViewList().FindAll((ViewConfig item) => item.path.ToLower() == path.ToLower());
			if (list.Count > 0)
			{
				return list[0];
			}
			return new ViewConfig();
		}

		public static void SaveViewConfig(ViewConfig viewconfig)
		{
			bool flag = false;
			List<ViewConfig> viewList = GetViewList();
			for (int i = 0; i < viewList.Count; i++)
			{
				if (viewList[i].path.ToLower() == viewconfig.path.ToLower())
				{
					viewList[i].include = viewconfig.include;
					flag = true;
				}
			}
			if (!flag)
			{
				viewList.Add(viewconfig);
			}
			string mapPath = FPFile.GetMapPath(WebConfig.WebPath + "config/view.config");
			FPXml.SaveXml(viewList, mapPath);
			FPCache.Remove("FP_VIEWLIST");
		}

		public static void SaveViewConfig(List<ViewConfig> viewlist)
		{
			string mapPath = FPFile.GetMapPath(WebConfig.WebPath + "config/view.config");
			FPXml.SaveXml(viewlist, mapPath);
			FPCache.Remove("FP_VIEWLIST");
		}

		public static void ReSetViewConfig()
		{
			List<ViewConfig> viewList = GetViewList();
			for (int num = viewList.Count - 1; num >= 0; num--)
			{
				if (!File.Exists(FPFile.GetMapPath(WebConfig.WebPath + viewList[num].path)))
				{
					viewList.RemoveAt(num);
				}
			}
			SaveViewConfig(viewList);
		}
	}
}
