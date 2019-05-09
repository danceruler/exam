using FangPage.Common;
using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FangPage.MVC
{
	public class FPViews
	{
		private static Regex[] r;

		private static Regex[] t;

		private static RegexOptions options;

		static FPViews()
		{
			r = new Regex[34];
			t = new Regex[4];
			options = (RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
			r[0] = new Regex("(<%--(.*?)--%>)", options);
			r[1] = new Regex("<%controller\\((?:\"?)([\\s\\S]+?)(?:\"?)\\)%>", options);
			r[2] = new Regex("<%using\\((?:\"?)([\\s\\S]+?)(?:\"?)\\)%>", options);
			r[3] = new Regex("<%#([\\s\\S]+?)%>", options);
			r[4] = new Regex("<%include\\((?:\"?)([\\s\\S]+?)(?:\"?)\\)%>", options);
			r[5] = new Regex("<%loop ((\\(([^\\[\\]\\{\\}\\s]+)\\) )?)([^\\[\\]\\{\\}\\s]+) ([^\\s]+)%>", options);
			r[6] = new Regex("<%\\/loop%>", options);
			r[7] = new Regex("<%while\\(([^\\[\\]\\{\\}\\s]+)\\)%>", options);
			r[8] = new Regex("<%\\/while\\(([^\\[\\]\\{\\}\\s]+)\\)%>", options);
			r[9] = new Regex("<%for\\(([^\\s]+?)(?:\\s*),(?:\\s*)([^\\s]+?)((,([a-zA-Z]+))?)\\)%>", options);
			r[10] = new Regex("<%\\/for%>", options);
			r[11] = new Regex("<%continue%>");
			r[12] = new Regex("<%break%>");
			r[13] = new Regex("<%if (?:\\s*)(([^\\s]+)((?:\\s*)(\\|\\||\\&\\&)(?:\\s*)([^\\s]+))?)(?:\\s*)%>", options);
			r[14] = new Regex("<%else(( (?:\\s*)if (?:\\s*)(([^\\s]+)((?:\\s*)(\\|\\||\\&\\&)(?:\\s*)([^\\s]+))?))?)(?:\\s*)%>", options);
			r[15] = new Regex("<%\\/if%>", options);
			r[16] = new Regex("(\\{request\\((?:\"?)([^\\[\\]\\{\\}\\s]+)\\)\\}(?:\"?))", options);
			r[17] = new Regex("<%set ((\\(?([\\w\\.<>]+)(?:\\)| ))?)(?:\\s*)\\{?([^\\s\\{\\}]+)\\}?(?:\\s*)=(?:\\s*)(.*?)(?:\\s*)%>", options);
			r[18] = new Regex("(\\${url\\((?:\\s*)(.*?)(?:\\s*)\\)})", options);
			r[19] = new Regex("(\\{int\\(([^\\s]+?)\\)\\})", options);
			r[20] = new Regex("(\\${urlencode\\(([^\\s]+?)\\)})", options);
			r[21] = new Regex("(\\${thumb\\(([^\\s]+?),([^\\s]+?)\\)})", options);
			r[22] = new Regex("(\\${txt\\(([^\\s]+?)\\)})", options);
			r[23] = new Regex("(\\${substr\\(([^\\s]+?),(.\\d*?)\\)})", options);
			r[24] = new Regex("(\\${fmdate\\(([^\\s]+?),(.*?)\\)})", options);
			r[25] = new Regex("(\\${([a-zA-Z]+)\\(([^\\[\\]/=\\s]+)\\)})", options);
			r[26] = new Regex("(\\$\\{([^\\.\\{\\}\\s]+)\\.([^\\[\\]\\{\\}\\s]+)\\})", options);
			r[27] = new Regex("(\\$\\{([^\\[\\]\\{\\}\\s]+)\\[([^\\[\\]\\{\\}\\s]+)\\]\\})", options);
			r[28] = new Regex("(\\${([^\\[\\]/\\{\\}='\\s]+)})", options);
			r[29] = new Regex("(\\${console\\(([^\\s]+?)\\)})", options);
			r[30] = new Regex("(\\${([^\\s\\{\\}\\$]+?),(.*?)})", options);
			r[31] = new Regex("<%\\/else(( (?:\\s*)if (?:\\s*)(([^\\s]+)((?:\\s*)(\\|\\||\\&\\&)(?:\\s*)([^\\s]+))?))?)(?:\\s*)%>", options);
			r[32] = new Regex("(\\${replace\\(([^\\s]+?),(.*?),(.*?)\\)})", options);
			r[33] = new Regex("(\\${light\\(([^\\s]+?),(.*?)\\)})", options);
			t[0] = new Regex("({([a-zA-Z]+)\\(([^\\[\\]/='\\s]+)\\)})", options);
			t[1] = new Regex("(\\{([^\\[\\]\\{\\}\\s]+)\\[([^\\[\\]\\{\\}\\s]+)\\]\\})", options);
			t[2] = new Regex("(\\{([^\\.\\{\\}\\s]+)\\.([^\\[\\]\\{\\}\\s]+)\\})", options);
			t[3] = new Regex("({([^\\[\\]/\\{\\}='\\s]+)})", options);
		}

		public static string CreateView(SiteConfig siteconfig, string viewpath, string aspxpath, int nest, string linkpath, out string includefile, out string includeimport)
		{
			includefile = "";
			includeimport = "";
			if (nest < 1)
			{
				nest = 1;
			}
			else if (nest > 5)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = siteconfig.urltype;
			if (viewpath.StartsWith("/"))
			{
				num = 2;
			}
			string mapPath = FPFile.GetMapPath(WebConfig.WebPath + viewpath.TrimStart('/'));
			Path.GetFileName(viewpath);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(viewpath);
			if (!File.Exists(mapPath))
			{
				return "";
			}
			HtmlDocument htmlDocument = new HtmlDocument();
			HtmlNode.ElementsFlags.Remove("option");
			htmlDocument.Load(mapPath, Encoding.UTF8);
			HtmlNodeCollection htmlNodeCollection;
			if (num > 0)
			{
				htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//img[@src]");
				if (htmlNodeCollection != null)
				{
					foreach (HtmlNode item in (IEnumerable<HtmlNode>)htmlNodeCollection)
					{
						if (!item.Attributes["src"].Value.StartsWith("data:image"))
						{
							item.Attributes["src"].Value = FormatPath(viewpath, aspxpath, item.Attributes["src"].Value, num);
						}
					}
				}
				htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//input[@src]");
				if (htmlNodeCollection != null)
				{
					foreach (HtmlNode item2 in (IEnumerable<HtmlNode>)htmlNodeCollection)
					{
						item2.Attributes["src"].Value = FormatPath(viewpath, aspxpath, item2.Attributes["src"].Value, siteconfig.urltype);
					}
				}
				htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//link[@href]");
				if (htmlNodeCollection != null)
				{
					foreach (HtmlNode item3 in (IEnumerable<HtmlNode>)htmlNodeCollection)
					{
						item3.Attributes["href"].Value = FormatPath(viewpath, aspxpath, item3.Attributes["href"].Value, siteconfig.urltype);
					}
				}
				htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//style[@type='text/css']");
				if (htmlNodeCollection != null)
				{
					foreach (HtmlNode item4 in (IEnumerable<HtmlNode>)htmlNodeCollection)
					{
						item4.InnerHtml = FormatPageCSS(item4.InnerHtml, viewpath, aspxpath, siteconfig.urltype);
					}
				}
				htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//*[@style]");
				if (htmlNodeCollection != null)
				{
					foreach (HtmlNode item5 in (IEnumerable<HtmlNode>)htmlNodeCollection)
					{
						item5.Attributes["style"].Value = FormatPageCSS(item5.Attributes["style"].Value, viewpath, aspxpath, siteconfig.urltype);
					}
				}
				htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//script[@src]");
				if (htmlNodeCollection != null)
				{
					foreach (HtmlNode item6 in (IEnumerable<HtmlNode>)htmlNodeCollection)
					{
						item6.Attributes["src"].Value = FormatPath(viewpath, aspxpath, item6.Attributes["src"].Value, siteconfig.urltype);
					}
				}
				htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//*[@background]");
				if (htmlNodeCollection != null)
				{
					foreach (HtmlNode item7 in (IEnumerable<HtmlNode>)htmlNodeCollection)
					{
						item7.Attributes["background"].Value = FormatPath(viewpath, aspxpath, item7.Attributes["background"].Value, siteconfig.urltype);
					}
				}
				htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//param[@name='movie']");
				if (htmlNodeCollection != null)
				{
					foreach (HtmlNode item8 in (IEnumerable<HtmlNode>)htmlNodeCollection)
					{
						if (item8.Attributes["value"] != null)
						{
							item8.Attributes["value"].Value = FormatPath(viewpath, aspxpath, item8.Attributes["value"].Value, siteconfig.urltype);
						}
					}
				}
				htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//embed[@src]");
				if (htmlNodeCollection != null)
				{
					foreach (HtmlNode item9 in (IEnumerable<HtmlNode>)htmlNodeCollection)
					{
						item9.Attributes["src"].Value = FormatPath(viewpath, aspxpath, item9.Attributes["src"].Value, siteconfig.urltype);
					}
				}
			}
			if (linkpath != "")
			{
				htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
				if (htmlNodeCollection != null)
				{
					foreach (HtmlNode item10 in (IEnumerable<HtmlNode>)htmlNodeCollection)
					{
						item10.Attributes["href"].Value = FormatHref(item10.Attributes["href"].Value, linkpath);
					}
				}
			}
			htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//*[@loop]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item11 in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					LoopNodes(item11);
				}
			}
			htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//*[@for]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item12 in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					ForNodes(item12);
				}
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//input[@is-checked]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item13 in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					string text = FPRandom.CreateCode(20);
					string text2 = item13.Attributes["is-checked"].Value;
					if (text2.StartsWith("${"))
					{
						text2 = FPUtils.TrimStart(text2, "${");
						text2 = FPUtils.TrimEnd(text2, "}");
					}
					text2 = "${" + text2 + ",\"checked\",\"\"}";
					dictionary.Add(text, text2);
					item13.Attributes["is-checked"].Value = text;
				}
			}
			htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//option[@is-selected]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item14 in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					string text3 = FPRandom.CreateCode(20);
					string value = "${" + item14.Attributes["is-selected"].Value.Replace("${", "").Replace("}", "") + ",\"selected\",\"\"}";
					dictionary.Add(text3, value);
					item14.Attributes["is-selected"].Value = text3;
				}
			}
			htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//input[@is-disabled]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item15 in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					string text4 = FPRandom.CreateCode(20);
					string text5 = item15.Attributes["is-disabled"].Value;
					if (text5.StartsWith("${"))
					{
						text5 = FPUtils.TrimStart(text5, "${");
						text5 = FPUtils.TrimEnd(text5, "}");
					}
					text5 = "${" + text5 + ",\"disabled\",\"\"}";
					dictionary.Add(text4, text5);
					item15.Attributes["is-disabled"].Value = text4;
				}
			}
			htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//*[@is-show]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item16 in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					string text6 = "${" + item16.Attributes["is-show"].Value.Replace("'", "\"") + ",\"\",\"display:none\"}";
					item16.Attributes["is-show"].Remove();
					if (item16.Attributes["style"] != null)
					{
						string value2 = item16.Attributes["style"].Value;
						text6 = text6 + ";" + value2;
					}
					item16.SetAttributeValue("style", text6);
				}
			}
			htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//*[@is-class]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item17 in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					string value3 = item17.Attributes["is-class"].Value;
					item17.Attributes["is-class"].Remove();
					string text7 = "";
					if (item17.Attributes["class"] != null)
					{
						text7 = item17.Attributes["class"].Value;
					}
					string[] array = FPArray.SplitString(value3);
					if (array.Length == 1)
					{
						text7 = "${" + array[0] + ",'" + text7 + "',''}";
					}
					else if (array.Length == 2)
					{
						text7 = text7 + "${" + array[0] + ",' '+" + array[1] + ",''}";
					}
					else if (array.Length == 3)
					{
						text7 = text7 + "${" + array[0] + ",' '+" + array[1] + ",' '+" + array[2] + "}";
					}
					item17.SetAttributeValue("class", text7);
				}
			}
			htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//*[@is-style]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item18 in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					string value4 = item18.Attributes["is-style"].Value;
					item18.Attributes["is-style"].Remove();
					string text8 = "";
					if (item18.Attributes["style"] != null)
					{
						text8 = item18.Attributes["style"].Value;
					}
					string[] array2 = FPArray.SplitString(value4);
					if (array2.Length == 1)
					{
						text8 = "${" + array2[0] + ",'" + text8 + "',''}";
					}
					else if (array2.Length == 2)
					{
						text8 = text8 + "${" + array2[0] + ",';'+" + array2[1] + ",''}";
					}
					else if (array2.Length == 3)
					{
						text8 = text8 + "${" + array2[0] + ",';'+" + array2[1] + ",';'+" + array2[2] + "}";
					}
					item18.SetAttributeValue("style", text8.Trim());
				}
			}
			htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//*[@if-show]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item19 in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					IfShowNodes(item19);
				}
			}
			htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//*[@else-show]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item20 in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					ElseShowNodes(item20);
				}
			}
			htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//include[@src]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item21 in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					IncldeNodes(item21);
				}
			}
			string text9 = htmlDocument.DocumentNode.InnerHtml;
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (Match item22 in r[0].Matches(text9))
			{
				text9 = text9.Replace(item22.Groups[0].ToString(), "");
			}
			text9 = IncludeTag(text9);
			stringBuilder2.Append(text9);
			if (dictionary.Count > 0)
			{
				foreach (KeyValuePair<string, string> item23 in dictionary)
				{
					stringBuilder2.Replace("is-checked=\"" + item23.Key + "\"", item23.Value);
					stringBuilder2.Replace("is-selected=\"" + item23.Key + "\"", item23.Value);
					stringBuilder2.Replace("is-disabled=\"" + item23.Key + "\"", item23.Value);
					stringBuilder2.Replace("is-checked='" + item23.Key + "'", item23.Value);
					stringBuilder2.Replace("is-selected='" + item23.Key + "'", item23.Value);
					stringBuilder2.Replace("is-disabled='" + item23.Key + "'", item23.Value);
				}
			}
			for (int i = 0; i < 3; i++)
			{
				stringBuilder2.Replace("<% ", "<%");
				stringBuilder2.Replace(" %>", "%>");
			}
			for (int j = 0; j < 3; j++)
			{
				stringBuilder2.Replace("<%Controller ", "<%Controller");
				stringBuilder2.Replace("<%controller ", "<%controller");
				stringBuilder2.Replace("<%Using ", "<%Using");
				stringBuilder2.Replace("<%using ", "<%using");
				stringBuilder2.Replace("<%Include ", "<%Include");
				stringBuilder2.Replace("<%include ", "<%include");
				stringBuilder2.Replace("<%loop  ", "<%loop ");
				stringBuilder2.Replace("<%Loop  ", "<%Loop ");
				stringBuilder2.Replace("<%set  ", "<%set ");
				stringBuilder2.Replace("<%Set  ", "<%Set ");
				stringBuilder2.Replace("<%for ", "<%for");
				stringBuilder2.Replace("<%For ", "<%For");
			}
			stringBuilder2.Replace("<%loop(", "<%loop (");
			stringBuilder2.Replace("<%Loop(", "<%Loop (");
			stringBuilder2.Replace("<%set(", "<%set (");
			stringBuilder2.Replace("<%Set(", "<%Set (");
			stringBuilder2.Replace("~/", "${webpath}");
			stringBuilder2.Replace("{webpath}/", "{webpath}");
			stringBuilder2.Replace("{rawpath}/", "{rawpath}");
			stringBuilder2.Replace("{curpath}/", "{curpath}");
			stringBuilder2.Replace("{plupath}/", "{plupath}");
			stringBuilder2.Replace("{adminpath}/", "{adminpath}");
			stringBuilder2.Replace("{apppath}/", "{apppath}");
			string text10 = "FangPage.MVC.FPController";
			string text11 = "<%@ Import namespace=\"System.Collections.Generic\" %>\r\n<%@ Import namespace=\"FangPage.Common\" %>\r\n<%@ Import namespace=\"FangPage.MVC\" %>";
			string str = FPArray.SplitString(siteconfig.dll, 2)[0];
			if (nest == 1)
			{
				{
					IEnumerator enumerator2 = r[1].Matches(stringBuilder2.ToString()).GetEnumerator();
					try
					{
						if (enumerator2.MoveNext())
						{
							Match match2 = (Match)enumerator2.Current;
							text10 = match2.Groups[1].ToString();
							if (text10 == "*")
							{
								text10 = "FangPage.WMS.Web.WebController";
							}
							else if (text10 == "#")
							{
								text10 = "FangPage.WMS.Web.LoginController";
							}
							else if (text10 == "$.*")
							{
								text10 = str + "." + fileNameWithoutExtension;
							}
							else if (text10.StartsWith("$."))
							{
								text10 = text10.Replace("$.", str + ".");
							}
							else if (text10.EndsWith(".*"))
							{
								text10 = text10.Substring(0, text10.LastIndexOf(".")) + "." + fileNameWithoutExtension;
								if (text10.StartsWith("*."))
								{
									text10 = text10.Replace("*.", "FangPage.WMS.Web.Controller.");
								}
							}
							else if (text10.StartsWith("*."))
							{
								text10 = text10.Replace("*.", "FangPage.WMS.Web.Controller.");
							}
							stringBuilder2.Replace(match2.Groups[0].ToString(), string.Empty);
						}
					}
					finally
					{
						IDisposable disposable = enumerator2 as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
				if ("\"".Equals(text10))
				{
					text10 = "FangPage.MVC.FPController";
				}
				if (text10 != "FangPage.MVC.FPController")
				{
					string text12 = AddExtImport(text10.Substring(0, text10.LastIndexOf('.')) + ".#");
					if (text12 != "")
					{
						text11 = text11 + "\r\n" + text12;
					}
				}
				if (siteconfig.import != "")
				{
					string[] array3 = siteconfig.import.Split(new string[4]
					{
						"\r\n",
						";",
						",",
						"|"
					}, StringSplitOptions.RemoveEmptyEntries);
					foreach (string text13 in array3)
					{
						if (!(text13 != ""))
						{
							continue;
						}
						if (text11 != "")
						{
							text11 += "\r\n";
						}
						string text14 = text13;
						if (Path.GetExtension(text14).ToLower() == ".dll")
						{
							text14 = text14.Substring(0, text14.LastIndexOf(".")) + ".*";
						}
						if (text14.EndsWith(".*") || text14.EndsWith(".#"))
						{
							string text15 = AddExtImport(text14);
							if (text15 != "")
							{
								text11 = text11 + "\r\n" + text15;
							}
						}
						else
						{
							text11 = text11 + "<%@ Import namespace=\"" + text14 + "\" %>";
						}
					}
				}
			}
			foreach (Match item24 in r[2].Matches(stringBuilder2.ToString()))
			{
				string text16 = item24.Groups[1].ToString();
				if (includeimport != "")
				{
					includeimport += "\r\n";
				}
				if (text16.EndsWith(".*") || text16.EndsWith(".#"))
				{
					includeimport += AddExtImport(text16);
				}
				else
				{
					includeimport = includeimport + "<%@ Import namespace=\"" + text16 + "\" %>";
				}
				stringBuilder2.Replace(item24.Groups[0].ToString(), string.Empty);
			}
			foreach (Match item25 in r[3].Matches(stringBuilder2.ToString()))
			{
				stringBuilder2.Replace(item25.Groups[0].ToString(), item25.Groups[0].ToString().Replace("\r\n", "\r\t\r"));
			}
			foreach (Match item26 in r[29].Matches(stringBuilder2.ToString()))
			{
				stringBuilder2.Replace(item26.Groups[0].ToString(), "<script type=\"text/javascript\">\r\n\tconsole.log('${" + item26.Groups[2].ToString() + "}');\r\n</script>");
			}
			stringBuilder2.Replace("\r\n", "\r\r\r");
			stringBuilder2.Replace("<%", "\r\r\n<%");
			stringBuilder2.Replace("%>", "%>\r\r\n");
			string[] array4 = FPArray.SplitString(stringBuilder2.ToString(), "\r\r\n");
			int upperBound = array4.GetUpperBound(0);
			for (int l = 0; l <= upperBound; l++)
			{
				string includefile2 = "";
				string includeimport2 = "";
				stringBuilder.Append(ConvertTags(nest, siteconfig, viewpath, aspxpath, array4[l], out includefile2, out includeimport2));
				includefile = FPArray.Push(includefile, includefile2, ";");
				if (includeimport2 != "")
				{
					includeimport = includeimport + "\r\n" + includeimport2;
				}
			}
			if (nest == 1)
			{
				if (includeimport != "")
				{
					text11 = text11 + "\r\n" + includeimport;
				}
				text11 = DelSameImport(text11);
				string text17 = "";
				FPFile.WriteFile(content: (!(stringBuilder.ToString() != "")) ? $"<%@ Page language=\"c#\" AutoEventWireup=\"false\" EnableViewState=\"false\" Inherits=\"{text10}\" %>\r\n</script>\r\n" : $"<%@ Page language=\"c#\" AutoEventWireup=\"false\" EnableViewState=\"false\" Inherits=\"{text10}\" %>\r\n{text11}\r\n<script runat=\"server\">\r\nprotected override void View()\r\n{{\r\n\tbase.View();\r\n{stringBuilder.ToString()}\tif(iswrite==0)\r\n\t{{\r\n\tResponse.Write(ViewBuilder.ToString());\r\n\t}}\r\n\telse if(iswrite==1)\r\n\t{{\r\n\tHashtable hash = new Hashtable();\r\n\thash[\"errcode\"] = 0;\r\n\thash[\"errmsg\"] =\"\";\r\n\thash[\"html\"]=ViewBuilder.ToString();\r\n\tFPResponse.WriteJson(hash);\r\n\t}}\r\n}}\r\n</script>\r\n", filename: FPFile.GetMapPath(WebConfig.WebPath + aspxpath.TrimStart('/')));
				ViewConfigs.SaveViewConfig(new ViewConfig
				{
					path = viewpath,
					include = includefile
				});
			}
			return stringBuilder.ToString();
		}

		private static string ConvertTags(int nest, SiteConfig siteconfig, string viewpath, string aspxpath, string inputStr, out string includefile, out string includeimport)
		{
			includefile = "";
			includeimport = "";
			string result = "";
			string text = inputStr.Replace("\\", "\\\\");
			text = text.Replace("\"", "\\\"");
			text = text.Replace("<SCRIPT", "<script");
			text = text.Replace("</SCRIPT>", "</script>");
			text = text.Replace("</script>", "</\");\r\n\tViewBuilder.Append(\"script>");
			bool flag = false;
			foreach (Match item3 in r[4].Matches(text))
			{
				flag = true;
				string text2 = item3.Groups[1].ToString().Replace("\\", "").Replace("\"", "")
					.Replace("\\", "/");
				string text3 = Path.GetDirectoryName(viewpath).Replace("\\", "/");
				string text4 = Path.GetDirectoryName(aspxpath).Replace("\\", "/");
				string text5 = "";
				while (text2.StartsWith("../"))
				{
					if (text3 != "")
					{
						text3 = Path.GetDirectoryName(text3).Replace("\\", "/");
					}
					if (text4 != "")
					{
						text4 = Path.GetDirectoryName(text4).Replace("\\", "/");
					}
					text2 = text2.Substring(3);
					text5 += "../";
				}
				if (!text2.StartsWith("/"))
				{
					text4 = FPFile.Combine(text4, text2);
					text2 = FPFile.Combine(text3, text2);
				}
				else
				{
					text4 = text2;
				}
				includefile = FPArray.Push(includefile, text2, ";");
				string includefile2 = "";
				string includeimport2 = "";
				string text6 = CreateView(siteconfig, text2, text4, nest + 1, text5, out includefile2, out includeimport2);
				text = ((!(text6 != "")) ? text.Replace(item3.Groups[0].ToString(), string.Empty) : text.Replace(item3.Groups[0].ToString(), text6));
				includefile = FPArray.Push(includefile, includefile2, ";");
				if (includeimport2 != "")
				{
					includeimport = includeimport + "\r\n" + includeimport2;
				}
			}
			foreach (Match item4 in r[5].Matches(text))
			{
				flag = true;
				string text7 = ReplaceTemplateFuntion(item4.Groups[5].ToString());
				string text8 = "loop__id";
				string text9 = "";
				if (item4.Groups[4].ToString().StartsWith("_"))
				{
					text9 = "int ";
					text8 = "loop__id" + item4.Groups[4].ToString();
				}
				text = ((!(item4.Groups[3].ToString() == "")) ? text.Replace(item4.Groups[0].ToString(), string.Format("\r\n\t{4}{3}=0;\r\n\tforeach({0} {1} in {2})\r\n\t{{\r\n\t{3}++;\r\n", item4.Groups[3].ToString(), item4.Groups[4].ToString(), text7, text8, text9)) : ((!text7.EndsWith(".Data")) ? text.Replace(item4.Groups[0].ToString(), string.Format("\r\n\t{3}{2}=0;\r\n\tforeach(DataRow {0} in {1}.Rows)\r\n\t{{\r\n\t{2}++;\r\n", item4.Groups[4].ToString(), text7, text8, text9)) : text.Replace(item4.Groups[0].ToString(), string.Format("\r\n\t{3}{2}=0;\r\n\tforeach(KeyValuePair<string,string> {0} in {1})\r\n\t{{\r\n\t{2}++;\r\n", item4.Groups[4].ToString(), text7, text8, text9))));
			}
			foreach (Match item5 in r[6].Matches(text))
			{
				flag = true;
				text = text.Replace(item5.Groups[0].ToString(), "\t}//end loop\r\n");
			}
			foreach (Match item6 in r[7].Matches(text))
			{
				flag = true;
				text = text.Replace(item6.Groups[0].ToString(), $"\r\n\tloop__id=0;\r\n\twhile({ReplaceTemplateFuntion(item6.Groups[1].ToString())}.Read())\r\n\t{{\r\n\tloop__id++;\r\n");
			}
			foreach (Match item7 in r[8].Matches(text))
			{
				flag = true;
				text = text.Replace(item7.Groups[0].ToString(), "\t}//end while\r\n\t" + ReplaceTemplateFuntion(item7.Groups[1].ToString()) + ".Close();\r\n");
			}
			foreach (Match item8 in r[9].Matches(text))
			{
				flag = true;
				string text10 = item8.Groups[5].ToString();
				if (text10 == "")
				{
					text10 = "i";
				}
				text = text.Replace(item8.Groups[0].ToString(), "\tfor (int " + text10 + " = " + item8.Groups[1] + "; " + text10 + " <= " + item8.Groups[2] + "; " + text10 + "++){\r\n");
			}
			foreach (Match item9 in r[10].Matches(text))
			{
				flag = true;
				text = text.Replace(item9.Groups[0].ToString(), "\t}//end for\r\n");
			}
			foreach (Match item10 in r[11].Matches(text))
			{
				flag = true;
				text = text.Replace(item10.Groups[0].ToString(), "\tcontinue;\r\n");
			}
			foreach (Match item11 in r[12].Matches(text))
			{
				flag = true;
				text = text.Replace(item11.Groups[0].ToString(), "\tbreak;\r\n");
			}
			foreach (Match item12 in r[13].Matches(text))
			{
				flag = true;
				string str = ReplaceTemplateFuntion(item12.Groups[1].ToString().Replace("\\\"", "\""));
				text = text.Replace(item12.Groups[0].ToString(), "\r\n\tif (" + str + ")\r\n\t{\r\n");
			}
			foreach (Match item13 in r[14].Matches(text))
			{
				flag = true;
				if (item13.Groups[1].ToString() == string.Empty)
				{
					text = text.Replace(item13.Groups[0].ToString(), "\t}\r\n\telse\r\n\t{\r\n");
				}
				else
				{
					string str2 = ReplaceTemplateFuntion(item13.Groups[3].ToString().Replace("\\\"", "\""));
					text = text.Replace(item13.Groups[0].ToString(), "\t}\r\n\telse if (" + str2 + ")\r\n\t{\r\n");
				}
			}
			foreach (Match item14 in r[31].Matches(text))
			{
				flag = true;
				if (item14.Groups[1].ToString() == string.Empty)
				{
					text = text.Replace(item14.Groups[0].ToString(), "\telse\r\n\t{\r\n");
				}
				else
				{
					string str3 = ReplaceTemplateFuntion(item14.Groups[3].ToString().Replace("\\\"", "\""));
					text = text.Replace(item14.Groups[0].ToString(), "\telse if (" + str3 + ")\r\n\t{\r\n");
				}
			}
			foreach (Match item15 in r[15].Matches(text))
			{
				flag = true;
				text = text.Replace(item15.Groups[0].ToString(), "\t}//end if\r\n");
			}
			foreach (Match item16 in r[16].Matches(text))
			{
				text = text.Replace(item16.Groups[0].ToString(), "FPRequest.GetString(\"" + item16.Groups[2].ToString() + "\")");
			}
			foreach (Match item17 in r[17].Matches(text.ToString()))
			{
				flag = true;
				string arg = "";
				if (item17.Groups[3].ToString() != string.Empty)
				{
					arg = item17.Groups[3].ToString();
				}
				text = text.Replace(item17.Groups[0].ToString(), string.Format("\t{0} {1} = {2};\r\n", arg, item17.Groups[4].ToString(), ReplaceTemplateFuntion(item17.Groups[5].ToString()).Replace("\\\"", "\"")));
			}
			foreach (Match item18 in r[18].Matches(text.ToString()))
			{
				text = text.Replace(item18.Groups[0].ToString(), string.Format("\" + seturl({0})+ \"", ReplaceTemplateFuntion(item18.Groups[2].ToString().Trim()).Replace("'", "\"")));
			}
			foreach (Match item19 in r[19].Matches(text))
			{
				text = text.Replace(item19.Groups[0].ToString(), "FPUtils.StrToInt(" + ReplaceTemplateFuntion(item19.Groups[2].ToString()) + ", 0)");
			}
			foreach (Match item20 in r[20].Matches(text))
			{
				text = text.Replace(item20.Groups[0].ToString(), "\"+FPUtils.UrlEncode(" + item20.Groups[2].ToString() + ")+\"");
			}
			foreach (Match item21 in r[21].Matches(text))
			{
				text = ((!flag) ? text.Replace(item21.Groups[0].ToString(), $"\" + FPThumb.GetThumbnail({ReplaceTemplateFuntion(item21.Groups[2].ToString())},{ReplaceTemplateFuntion(item21.Groups[3].ToString())})+ \"") : text.Replace(item21.Groups[0].ToString(), $"FPThumb.GetThumbnail({ReplaceTemplateFuntion(item21.Groups[2].ToString())},{ReplaceTemplateFuntion(item21.Groups[3].ToString())})"));
			}
			foreach (Match item22 in r[22].Matches(text))
			{
				text = ((!flag) ? text.Replace(item22.Groups[0].ToString(), $"\" + FPUtils.RemoveHtml({ReplaceTemplateFuntion(item22.Groups[2].ToString())}) + \"") : text.Replace(item22.Groups[0].ToString(), $"FPUtils.RemoveHtml({ReplaceTemplateFuntion(item22.Groups[2].ToString())})"));
			}
			foreach (Match item23 in r[23].Matches(text))
			{
				text = ((!flag) ? text.Replace(item23.Groups[0].ToString(), $"\" + echo({ReplaceTemplateFuntion(item23.Groups[2].ToString())},{item23.Groups[3].ToString()})+ \"") : text.Replace(item23.Groups[0].ToString(), $"echo({ReplaceTemplateFuntion(item23.Groups[2].ToString())},{item23.Groups[3].ToString()})"));
			}
			foreach (Match item24 in r[24].Matches(text))
			{
				text = ((!flag) ? text.Replace(item24.Groups[0].ToString(), string.Format("\" + echo({0},\"{1}\") + \"", ReplaceTemplateFuntion(item24.Groups[2].ToString()), item24.Groups[3].ToString().Replace("\\\"", string.Empty))) : text.Replace(item24.Groups[0].ToString(), string.Format("echo({0},\"{1}\")", ReplaceTemplateFuntion(item24.Groups[2].ToString()), item24.Groups[3].ToString().Replace("\\\"", string.Empty))));
			}
			foreach (Match item25 in r[32].Matches(text))
			{
				string text11 = item25.Groups[3].ToString().Replace("\\\"", "\"");
				if ((text11.StartsWith("\"") && text11.EndsWith("\"")) || (text11.StartsWith("'") && text11.EndsWith("'")))
				{
					string[] array = FPArray.SplitString(text11.TrimStart('"').TrimEnd('"').TrimStart('\'')
						.TrimEnd('\''), "|");
					text11 = "";
					string[] array2 = array;
					foreach (string text12 in array2)
					{
						text11 = ((!text12.StartsWith("{") || !text12.EndsWith("}")) ? FPArray.Append(text11, "\"" + text12 + "\"", "+\"|\"+") : FPArray.Append(text11, text12.TrimStart('{').TrimEnd('}'), "+\"|\"+"));
					}
				}
				else
				{
					string[] array3 = FPArray.SplitString(text11, "|");
					text11 = "";
					string[] array2 = array3;
					foreach (string item in array2)
					{
						text11 = FPArray.Append(text11, item, "+\"|\"+");
					}
				}
				string text13 = item25.Groups[4].ToString().Replace("\\\"", "\"");
				if ((text13.StartsWith("\"") && text13.EndsWith("\"")) || (text13.StartsWith("'") && text13.EndsWith("'")))
				{
					string[] array4 = FPArray.SplitString(text13.TrimStart('"').TrimEnd('"').TrimStart('\'')
						.TrimEnd('\''), "|");
					text13 = "";
					string[] array2 = array4;
					foreach (string text14 in array2)
					{
						text13 = ((!text14.StartsWith("{") || !text14.EndsWith("}")) ? FPArray.Append(text13, "\"" + text14 + "\"", "+\"|\"+") : FPArray.Append(text13, text14.TrimStart('{').TrimEnd('}'), "+\"|\"+"));
					}
				}
				else
				{
					string[] array5 = FPArray.SplitString(text13, "|");
					text13 = "";
					string[] array2 = array5;
					foreach (string item2 in array2)
					{
						text13 = FPArray.Append(text13, item2, "+\"|\"+");
					}
				}
				text = text.Replace(item25.Groups[0].ToString(), $"\" + echo({ReplaceTemplateFuntion(item25.Groups[2].ToString())},{text11},{text13}) + \"");
			}
			foreach (Match item26 in r[33].Matches(text))
			{
				string text15 = item26.Groups[3].ToString().Replace("\\\"", "\"");
				string text16 = "";
				if ((text15.StartsWith("\"") && text15.EndsWith("\"")) || (text15.StartsWith("'") && text15.EndsWith("'")))
				{
					string[] array6 = FPArray.SplitString(text15.TrimStart('"').TrimEnd('"').TrimStart('\'')
						.TrimEnd('\''), "|");
					text15 = "";
					string[] array2 = array6;
					foreach (string text17 in array2)
					{
						if (text17.StartsWith("{") && text17.EndsWith("}"))
						{
							text15 = FPArray.Append(text15, text17.TrimStart('{').TrimEnd('}'), "+\"|\"+");
							text16 = FPArray.Append(text16, "\"<span style=\\\"background-color:#ffd800;\\\">\"+" + text17.TrimStart('{').TrimEnd('}') + "+\"</span>\"", "+\"|\"+");
						}
						else
						{
							text15 = FPArray.Append(text15, "\"" + text17 + "\"", "+\"|\"+");
							text16 = FPArray.Append(text16, "\"<span style=\\\"background-color:#ffd800;\\\">" + text17 + "</span>\"", "+\"|\"+");
						}
					}
				}
				else
				{
					string[] array7 = FPArray.SplitString(text15, "|");
					text15 = "";
					string[] array2 = array7;
					foreach (string text18 in array2)
					{
						text15 = FPArray.Append(text15, text18, "+\"|\"+");
						text16 = FPArray.Append(text16, "\"<span style=\\\"background-color:#ffd800;\\\">\"+" + text18 + "+\"</span>\"", "+\"|\"+");
					}
				}
				text = text.Replace(item26.Groups[0].ToString(), $"\" + echo({ReplaceTemplateFuntion(item26.Groups[2].ToString())},{text15},{text16}) + \"");
			}
			foreach (Match item27 in r[25].Matches(text))
			{
				text = ((!flag) ? text.Replace(item27.Groups[0].ToString(), string.Format("\" + echo({0}({1})) + \"", ReplaceTemplateFuntion(item27.Groups[2].ToString()), item27.Groups[3].ToString().Replace("\\\"", "\"").Replace("'", "\""))) : text.Replace(item27.Groups[0].ToString(), string.Format("echo({0}({1}))", ReplaceTemplateFuntion(item27.Groups[2].ToString()), item27.Groups[3].ToString().Replace("\\\"", "\"").Replace("'", "\""))));
			}
			foreach (Match item28 in r[30].Matches(text))
			{
				string text19 = ReplaceTemplateFuntion(item28.Groups[2].ToString().Replace("\\\"", "\"").Replace("'", "\""));
				string[] array8 = FPArray.SplitString(item28.Groups[3].ToString().Replace("\\\"", "\"").Replace("'", "\""), 2);
				if (!text19.StartsWith("("))
				{
					if (array8[0] == "")
					{
						array8[0] = "\"\"";
					}
					if (array8[1] == "")
					{
						array8[1] = "\"\"";
					}
					text = text.Replace(item28.Groups[0].ToString(), "\"+(" + text19 + "?echo(" + array8[0] + "):echo(" + array8[1] + "))+\"");
				}
			}
			foreach (Match item29 in r[26].Matches(text))
			{
				string text20 = item29.Groups[2].ToString();
				if (text20.IndexOf("(") >= 0 && text20.IndexOf(")") >= 0)
				{
					text20 = item29.Groups[2].ToString();
				}
				text = ((!flag) ? ((!(item29.Groups[3].ToString().ToLower() == "_id")) ? text.Replace(item29.Groups[0].ToString(), $"\" + echo({ReplaceTemplateFuntion(text20)}.{item29.Groups[3].ToString()}) + \"") : ((!text20.StartsWith("_")) ? text.Replace(item29.Groups[0].ToString(), "\" + loop__id.ToString() + \"") : text.Replace(item29.Groups[0].ToString(), $"\" + loop__id{text20}.ToString() + \""))) : ((!(item29.Groups[3].ToString().ToLower() == "_id")) ? text.Replace(item29.Groups[0].ToString(), $"{ReplaceTemplateFuntion(text20)}.{item29.Groups[3].ToString()}") : ((!text20.StartsWith("_")) ? text.Replace(item29.Groups[0].ToString(), "loop__id") : text.Replace(item29.Groups[0].ToString(), "loop__id" + text20))));
			}
			foreach (Match item30 in r[27].Matches(text))
			{
				string text21 = item30.Groups[3].ToString();
				text = ((!flag) ? ((!FPUtils.IsNumeric(text21)) ? ((!(text21.ToLower() == "_id")) ? ((!(text21.ToLower() == "i") && !(text21.ToLower() == "j") && (!text21.StartsWith("(") || !text21.EndsWith(")"))) ? text.Replace(item30.Groups[0].ToString(), $"\" + echo({item30.Groups[2].ToString()}[\"{item30.Groups[3].ToString()}\"]) + \"") : text.Replace(item30.Groups[0].ToString(), $"\" + echo({item30.Groups[2].ToString()}[{text21.TrimStart('(').TrimEnd(')')}]) + \"")) : text.Replace(item30.Groups[0].ToString(), "\" + loop__id.ToString() + \"")) : text.Replace(item30.Groups[0].ToString(), $"\" + echo({item30.Groups[2].ToString()}[{text21}]) + \"")) : ((!FPUtils.IsNumeric(text21)) ? ((!(text21.ToLower() == "_id")) ? ((!(text21.ToLower() == "i") && !(text21.ToLower() == "j") && (!text21.StartsWith("(") || !text21.EndsWith(")"))) ? text.Replace(item30.Groups[0].ToString(), "echo(" + item30.Groups[2].ToString() + "[\"" + text21 + "\"])") : text.Replace(item30.Groups[0].ToString(), "echo(" + item30.Groups[2].ToString() + "[" + text21.TrimStart('(').TrimEnd(')') + "])")) : text.Replace(item30.Groups[0].ToString(), "loop__id")) : text.Replace(item30.Groups[0].ToString(), "echo(" + item30.Groups[2].ToString() + "[" + text21 + "])")));
			}
			foreach (Match item31 in r[28].Matches(text))
			{
				string text22 = ReplaceTemplateFuntion(item31.Groups[2].ToString());
				text = ((!flag) ? text.Replace(item31.Groups[0].ToString(), $"\" + echo({text22}) + \"") : text.Replace(item31.Groups[0].ToString(), text22));
			}
			foreach (Match item32 in r[3].Matches(text))
			{
				flag = true;
				text = text.Replace(item32.Groups[0].ToString(), item32.Groups[1].ToString().Replace("\r\t\r", "\r\n\t").Replace("\\\"", "\""));
			}
			if (flag)
			{
				result = text;
			}
			else if (text.Trim() != "")
			{
				StringBuilder stringBuilder = new StringBuilder();
				string[] array2 = FPArray.SplitString(text, "\r\r\r");
				foreach (string text23 in array2)
				{
					if (!(text23.Trim() == ""))
					{
						stringBuilder.Append("\tViewBuilder.Append(\"" + text23 + "\\r\\n\");\r\n");
					}
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		public static void CreateSite(SiteConfig siteconfig)
		{
			CreateSite(siteconfig, siteconfig.sitepath);
		}

		public static void CreateSite(SiteConfig siteinfo, string sitepath)
		{
			if (!Directory.Exists(FPFile.GetMapPath(WebConfig.WebPath + "sites/" + siteinfo.sitepath)))
			{
				return;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(FPFile.GetMapPath(WebConfig.WebPath + "sites/" + sitepath));
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (fileInfo.Extension.ToLower() == ".aspx" && !fileInfo.Name.StartsWith("_"))
				{
					string viewpath = "sites/" + sitepath + "/" + fileInfo.Name;
					string aspxpath = sitepath + "/" + fileInfo.Name;
					string includefile = "";
					string includeimport = "";
					CreateView(siteinfo, viewpath, aspxpath, 1, "", out includefile, out includeimport);
				}
				else if (fileInfo.Extension.ToLower() != ".aspx" && (siteinfo.urltype == 0 || siteinfo.urltype == 1))
				{
					string mapPath = FPFile.GetMapPath(WebConfig.WebPath + sitepath);
					if (!Directory.Exists(mapPath))
					{
						Directory.CreateDirectory(mapPath);
					}
					if (File.Exists(mapPath + "\\" + fileInfo.Name) && File.GetAttributes(mapPath + "\\" + fileInfo.Name).ToString().IndexOf("ReadOnly") != -1)
					{
						File.SetAttributes(mapPath + "\\" + fileInfo.Name, FileAttributes.Normal);
					}
					fileInfo.CopyTo(mapPath + "\\" + fileInfo.Name, true);
				}
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				CreateSite(siteinfo, sitepath + "/" + directoryInfo2.Name);
			}
		}

		private static string IncludeTag(string content)
		{
			content.Replace("<!--#include  ", "<!--#include ");
			content.Replace("<!--#include   ", "<!--#include ");
			content.Replace("<!--#include    ", "<!--#include ");
			content.Replace("<!--#include     ", "<!--#include ");
			Regex[] array = new Regex[2]
			{
				new Regex("(<!--#include ([^>]*?)-->)", options),
				new Regex("(<title>([^>]*?)<\\/title>)", options)
			};
			foreach (Match item in array[0].Matches(content))
			{
				string str = IncludeFileTag(item.Groups[2].ToString());
				content = content.Replace(item.Groups[0].ToString(), "<%include(" + str + ")%>");
			}
			foreach (Match item2 in array[1].Matches(content))
			{
				if (content.IndexOf("<meta name=\"keywords\"") < 0 && content.IndexOf("<meta name=\"description\"") < 0)
				{
					content = content.Replace(item2.Groups[0].ToString(), item2.Groups[0].ToString() + "\r\n\t${meta}");
				}
			}
			return content;
		}

		private static string IncludeFileTag(string attributes)
		{
			Match match = new Regex("(file=[\"|']([\\s\\S]*?)[\"|'|])", options).Match(attributes);
			string result = attributes;
			if (match != null)
			{
				result = match.Groups[2].ToString();
			}
			return result;
		}

		private static string ReplaceTemplateFuntion(string FuntionName)
		{
			string text = FuntionName.Replace("\\", "");
			if (!text.StartsWith("{") && text.IndexOf("[") > 0 && text.IndexOf("]") > 0)
			{
				text = "{" + text;
				text = text.Replace("]", "]}");
			}
			foreach (Match item in r[19].Matches(text))
			{
				text = text.Replace(item.Groups[0].ToString(), "FPUtils.StrToInt(" + item.Groups[2].ToString() + ")");
			}
			foreach (Match item2 in r[16].Matches(text))
			{
				text = text.Replace(item2.Groups[0].ToString(), $"FPRequest.GetString(\"{item2.Groups[2].ToString()}\")");
			}
			foreach (Match item3 in t[0].Matches(text))
			{
				text = text.Replace(item3.Groups[0].ToString(), $"{ReplaceTemplateFuntion(item3.Groups[2].ToString())}({item3.Groups[3].ToString()})");
			}
			foreach (Match item4 in t[1].Matches(text))
			{
				if (FPUtils.IsNumeric(item4.Groups[3].ToString()))
				{
					text = text.Replace(item4.Groups[0].ToString(), $"{item4.Groups[2].ToString()}[{item4.Groups[3].ToString()}].ToString()");
				}
				else
				{
					string text2 = item4.Groups[3].ToString();
					text = ((!(text2 == "_id")) ? ((!(text2.ToLower() == "i") && !(text2.ToLower() == "j") && (!text2.StartsWith("(") || !text2.EndsWith(")"))) ? text.Replace(item4.Groups[0].ToString(), $"{item4.Groups[2].ToString()}[\"{text2}\"].ToString()") : text.Replace(item4.Groups[0].ToString(), $"{item4.Groups[2].ToString()}[{text2.TrimStart('(').TrimEnd(')')}]")) : text.Replace(item4.Groups[0].ToString(), "loop__id"));
				}
			}
			foreach (Match item5 in t[2].Matches(text))
			{
				text = ((!(item5.Groups[3].ToString().ToLower() == "_id")) ? text.Replace(item5.Groups[0].ToString(), $"{item5.Groups[2].ToString()}.{item5.Groups[3].ToString()}") : text.Replace(item5.Groups[0].ToString(), "loop__id"));
			}
			foreach (Match item6 in t[3].Matches(text))
			{
				string newValue = ReplaceTemplateFuntion(item6.Groups[2].ToString());
				text = text.Replace(item6.Groups[0].ToString(), newValue);
			}
			if (text.IndexOf("!#") > 0)
			{
				string[] array = FPArray.SplitString(text, new string[2]
				{
					"&&",
					"||"
				});
				foreach (string text3 in array)
				{
					if (text3.IndexOf("!#") > 0)
					{
						string[] array2 = FPArray.SplitString(text3, "!#", 2);
						text = text.Replace(text3, "!FPArray.Contain(" + array2[0] + "," + array2[1] + ")");
					}
				}
			}
			if (text.IndexOf("#") > 0)
			{
				string[] array = FPArray.SplitString(text, new string[2]
				{
					"&&",
					"||"
				});
				foreach (string text4 in array)
				{
					if (text4.IndexOf("#") > 0)
					{
						string[] array3 = FPArray.SplitString(text4, "#", 2);
						text = text.Replace(text4, "FPArray.Contain(" + array3[0] + "," + array3[1] + ")");
					}
				}
			}
			return text;
		}

		private static string FormatPageCSS(string content, string viewpath, string aspxpath, int urltype)
		{
			foreach (Match item in new Regex("(url\\(([^>]*?)\\))", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline).Matches(content))
			{
				content = content.Replace(item.Groups[0].ToString(), "url(" + FormatPath(viewpath, aspxpath, item.Groups[2].ToString(), urltype) + ")");
			}
			return content;
		}

		private static string FormatPath(string viewpath, string aspxpath, string linkpath, int urltype)
		{
			linkpath = linkpath.TrimStart('"').TrimEnd('"');
			linkpath = linkpath.TrimStart('\'').TrimEnd('\'');
			if (linkpath.StartsWith("/") || linkpath.StartsWith("\\") || linkpath.StartsWith("~/") || linkpath.StartsWith("~\\") || linkpath.StartsWith("${") || linkpath.StartsWith("http://") || linkpath.StartsWith("https://"))
			{
				return linkpath;
			}
			string text = "";
			if (urltype == 1)
			{
				aspxpath = Path.GetDirectoryName(aspxpath).Replace("\\", "/").TrimStart('/');
				while (linkpath.StartsWith("../"))
				{
					if (aspxpath != "")
					{
						aspxpath = Path.GetDirectoryName(aspxpath).Replace("\\", "/");
					}
					linkpath = linkpath.Substring(3);
				}
				if (aspxpath != "")
				{
					aspxpath = ((aspxpath.IndexOf("/") < 0) ? "" : aspxpath.Substring(aspxpath.IndexOf("/") + 1));
				}
				return "${webpath}/${sitepath}/" + FPFile.Combine(aspxpath, linkpath);
			}
			viewpath = Path.GetDirectoryName(viewpath).Replace("\\", "/").TrimStart('/');
			while (linkpath.StartsWith("../"))
			{
				if (viewpath != "")
				{
					viewpath = Path.GetDirectoryName(viewpath).Replace("\\", "/");
				}
				linkpath = linkpath.Substring(3);
			}
			return "${webpath}/" + FPFile.Combine(viewpath, linkpath);
		}

		private static string FormatHref(string href, string linkpath)
		{
			if (string.IsNullOrEmpty(href) || href.StartsWith("/") || href.StartsWith("\\") || href.StartsWith("#") || href.StartsWith("${") || href.StartsWith("http://") || href.StartsWith("https://"))
			{
				return href;
			}
			return linkpath + href;
		}

		private static string DelSameImport(string strIm)
		{
			string[] array = FPArray.RemoveSame(FPArray.SplitString(strIm, "\r\n"));
			string text = "";
			string[] array2 = array;
			foreach (string str in array2)
			{
				if (text != "")
				{
					text += "\r\n";
				}
				text += str;
			}
			return text;
		}

		private static string AddExtImport(string strIm)
		{
			string text = FindImportFile(strIm);
			string text2 = "";
			if (text != "")
			{
				Type[] types = FPUtils.GetAssembly(FPFile.GetMapPath(WebConfig.WebPath + "bin/" + text + ".dll")).GetTypes();
				foreach (Type type in types)
				{
					if (type.Namespace == null)
					{
						continue;
					}
					if (strIm.EndsWith(".#"))
					{
						if (type.Namespace.EndsWith(".Model") || type.Namespace == text)
						{
							if (text2 != "")
							{
								text2 += "\r\n";
							}
							text2 = text2 + "<%@ Import namespace=\"" + type.Namespace + "\" %>";
						}
						if (type.Namespace.EndsWith(".Bll") || type.Namespace == text)
						{
							if (text2 != "")
							{
								text2 += "\r\n";
							}
							text2 = text2 + "<%@ Import namespace=\"" + type.Namespace + "\" %>";
						}
					}
					else
					{
						if (text2 != "")
						{
							text2 += "\r\n";
						}
						text2 = text2 + "<%@ Import namespace=\"" + type.Namespace + "\" %>";
					}
				}
				text2 = DelSameImport(text2);
			}
			return text2;
		}

		private static string FindImportFile(string strIm)
		{
			string text = strIm;
			while (true)
			{
				if (File.Exists(FPFile.GetMapPath(WebConfig.WebPath + "bin/" + text + ".dll")))
				{
					return text;
				}
				if (text.LastIndexOf(".") <= 0)
				{
					break;
				}
				text = text.Substring(0, text.LastIndexOf("."));
			}
			return "";
		}

		private static void IncldeNodes(HtmlNode htmlnode)
		{
			string value = htmlnode.Attributes["src"].Value;
			HtmlNode htmlNode = HtmlNode.CreateNode("<%include(" + value + ")%>\r\n");
			htmlnode.ParentNode.ReplaceChild(htmlNode.ParentNode, htmlnode);
		}

		private static void LoopNodes(HtmlNode htmlnode)
		{
			string text = htmlnode.Attributes["loop"].Value.Replace("'", "\"");
			htmlnode.Attributes["loop"].Remove();
			HtmlNode htmlNode = HtmlNode.CreateNode("<%loop " + text + "%>\r\n" + htmlnode.OuterHtml + "<%/loop%>\r\n");
			HtmlNodeCollection htmlNodeCollection = htmlNode.SelectNodes("//*[@loop]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					LoopNodes(item);
				}
			}
			htmlnode.ParentNode.ReplaceChild(htmlNode.ParentNode, htmlnode);
		}

		private static void ForNodes(HtmlNode htmlnode)
		{
			string text = htmlnode.Attributes["for"].Value.Replace("'", "\"");
			if (text.IndexOf(',') > 0)
			{
				htmlnode.Attributes["for"].Remove();
				HtmlNode htmlNode = HtmlNode.CreateNode("<%for(" + text.Trim() + ")%>\r\n" + htmlnode.OuterHtml + "<%/for%>\r\n");
				HtmlNodeCollection htmlNodeCollection = htmlNode.SelectNodes("//*[@for]");
				if (htmlNodeCollection != null)
				{
					foreach (HtmlNode item in (IEnumerable<HtmlNode>)htmlNodeCollection)
					{
						ForNodes(item);
					}
				}
				htmlnode.ParentNode.ReplaceChild(htmlNode.ParentNode, htmlnode);
			}
		}

		private static void IfShowNodes(HtmlNode htmlnode)
		{
			string text = htmlnode.Attributes["if-show"].Value.Replace("'", "\"");
			htmlnode.Attributes["if-show"].Remove();
			HtmlNode htmlNode = HtmlNode.CreateNode("<%if " + text + "%>\r\n" + htmlnode.OuterHtml + "<%/if%>\r\n");
			HtmlNodeCollection htmlNodeCollection = htmlNode.SelectNodes("//*[@if-show]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					IfShowNodes(item);
				}
			}
			htmlnode.ParentNode.ReplaceChild(htmlNode.ParentNode, htmlnode);
		}

		private static void ElseShowNodes(HtmlNode htmlnode)
		{
			string text = htmlnode.Attributes["else-show"].Value.Replace("'", "\"");
			htmlnode.Attributes["else-show"].Remove();
			string text2 = "";
			text2 = ((!(text != "")) ? ("<%/else%>\r\n" + htmlnode.OuterHtml + "<%/if%>\r\n") : ("<%/else if " + text + "%>\r\n" + htmlnode.OuterHtml + "<%/if%>\r\n"));
			HtmlNode htmlNode = HtmlNode.CreateNode(text2);
			HtmlNodeCollection htmlNodeCollection = htmlNode.SelectNodes("//*[@else-show]");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode item in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					ElseShowNodes(item);
				}
			}
			htmlnode.ParentNode.ReplaceChild(htmlNode.ParentNode, htmlnode);
		}
	}
}
