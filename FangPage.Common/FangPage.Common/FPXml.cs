using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace FangPage.Common
{
	public class FPXml
	{
		private static object lockHelper = new object();

		public static void CreateXml<T>(string filename)
		{
			lock (lockHelper)
			{
				XmlDocument xmlDocument = new XmlDocument();
				XmlElement newChild = xmlDocument.CreateElement(typeof(T).Name + "s");
				xmlDocument.AppendChild(newChild);
				FPFile.WriteFile(filename, xmlDocument.InnerXml);
			}
		}

		public static T LoadModel<T>(string filename) where T : new()
		{
			T val = new T();
			Type typeFromHandle = typeof(T);
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(filename);
			}
			catch
			{
				return default(T);
			}
			PropertyInfo[] properties = typeFromHandle.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (!(propertyInfo?.CanWrite ?? false))
				{
					continue;
				}
				XmlNode xmlNode = xmlDocument.SelectSingleNode(typeFromHandle.Name + "/" + propertyInfo.Name);
				if (xmlNode != null)
				{
					Type propertyType = propertyInfo.PropertyType;
					if (propertyType == typeof(string))
					{
						propertyInfo.SetValue(val, xmlNode.InnerText, null);
					}
					else if (propertyType == typeof(int))
					{
						propertyInfo.SetValue(val, FPUtils.StrToInt(xmlNode.InnerText), null);
					}
					else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
					{
						propertyInfo.SetValue(val, FPUtils.StrToDateTime(xmlNode.InnerText), null);
					}
					else if (propertyType == typeof(decimal))
					{
						propertyInfo.SetValue(val, FPUtils.StrToDecimal(xmlNode.InnerText), null);
					}
					else if (propertyType == typeof(float))
					{
						propertyInfo.SetValue(val, FPUtils.StrToFloat(xmlNode.InnerText), null);
					}
					else if (propertyType == typeof(double))
					{
						propertyInfo.SetValue(val, FPUtils.StrToDouble(xmlNode.InnerText), null);
					}
					else if (propertyType == typeof(bool))
					{
						propertyInfo.SetValue(val, FPUtils.StrToBool(xmlNode.InnerText, false), null);
					}
					else if (propertyType == typeof(short))
					{
						propertyInfo.SetValue(val, short.Parse(FPUtils.StrToInt(xmlNode.InnerText).ToString()), null);
					}
				}
			}
			return val;
		}

		public static List<T> LoadList<T>(string filename) where T : new()
		{
			List<T> list = new List<T>();
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(filename);
			}
			catch
			{
				return list;
			}
			if (xmlDocument.ChildNodes.Count == 0)
			{
				return list;
			}
			if (xmlDocument.ChildNodes[xmlDocument.ChildNodes.Count - 1].Name.ToLower() != typeof(T).Name.ToLower() + "s")
			{
				return list;
			}
			XmlNode xmlNode = xmlDocument.ChildNodes[xmlDocument.ChildNodes.Count - 1];
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				if (childNode.Name.ToLower() == typeof(T).Name.ToLower())
				{
					list.Add(NodeToModel<T>(childNode));
				}
			}
			return list;
		}

		public static void SaveXml<T>(T model, string filename)
		{
			lock (lockHelper)
			{
				Type typeFromHandle = typeof(T);
				XmlDocument xmlDocument = new XmlDocument();
				XmlElement xmlElement = xmlDocument.CreateElement(typeFromHandle.Name);
				PropertyInfo[] properties = typeFromHandle.GetProperties(BindingFlags.Instance | BindingFlags.Public);
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (propertyInfo?.CanWrite ?? false)
					{
						XmlElement xmlElement2 = xmlDocument.CreateElement(propertyInfo.Name);
						string innerText = string.Empty;
						if (propertyInfo.GetValue(model, null) != null)
						{
							innerText = propertyInfo.GetValue(model, null).ToString();
						}
						xmlElement2.InnerText = innerText;
						xmlElement.AppendChild(xmlElement2);
					}
				}
				xmlDocument.AppendChild(xmlElement);
				xmlDocument.Save(filename);
			}
		}

		public static void SaveXml<T>(List<T> list, string filename)
		{
			lock (lockHelper)
			{
				XmlDocument xmlDocument = new XmlDocument();
				XmlElement xmlElement = xmlDocument.CreateElement(typeof(T).Name + "s");
				foreach (T item in list)
				{
					ModelToNode(xmlDocument, xmlElement, item);
				}
				xmlDocument.AppendChild(xmlElement);
				xmlDocument.Save(filename);
			}
		}

		public static T ToModel<T>(string xml) where T : new()
		{
			T val = new T();
			Type typeFromHandle = typeof(T);
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.LoadXml(xml);
			}
			catch (Exception)
			{
				return default(T);
			}
			PropertyInfo[] properties = typeFromHandle.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (!(propertyInfo?.CanWrite ?? false))
				{
					continue;
				}
				XmlNode xmlNode = xmlDocument.SelectSingleNode(typeFromHandle.Name + "/" + propertyInfo.Name);
				if (xmlNode != null)
				{
					string text = propertyInfo.Name.ToLower();
					Type propertyType = propertyInfo.PropertyType;
					if (propertyType == typeof(string))
					{
						propertyInfo.SetValue(val, xmlNode.InnerText, null);
					}
					else if (propertyType == typeof(int))
					{
						propertyInfo.SetValue(val, FPUtils.StrToInt(xmlNode.InnerText), null);
					}
					else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
					{
						propertyInfo.SetValue(val, FPUtils.StrToDateTime(xmlNode.InnerText), null);
					}
					else if (propertyType == typeof(decimal))
					{
						propertyInfo.SetValue(val, FPUtils.StrToDecimal(xmlNode.InnerText), null);
					}
					else if (propertyType == typeof(float))
					{
						propertyInfo.SetValue(val, FPUtils.StrToFloat(xmlNode.InnerText), null);
					}
					else if (propertyType == typeof(double))
					{
						propertyInfo.SetValue(val, FPUtils.StrToDouble(xmlNode.InnerText), null);
					}
					else if (propertyType == typeof(bool))
					{
						propertyInfo.SetValue(val, FPUtils.StrToBool(xmlNode.InnerText, false), null);
					}
					else if (propertyType == typeof(short))
					{
						propertyInfo.SetValue(val, short.Parse(FPUtils.StrToInt(xmlNode.InnerText).ToString()), null);
					}
				}
			}
			return val;
		}

		public static string ToXml<T>(T model)
		{
			Type typeFromHandle = typeof(T);
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement(typeFromHandle.Name);
			PropertyInfo[] properties = typeFromHandle.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (propertyInfo?.CanWrite ?? false)
				{
					XmlElement xmlElement2 = xmlDocument.CreateElement(propertyInfo.Name);
					string innerText = string.Empty;
					if (propertyInfo.GetValue(model, null) != null)
					{
						innerText = propertyInfo.GetValue(model, null).ToString();
					}
					xmlElement2.InnerText = innerText;
					xmlElement.AppendChild(xmlElement2);
				}
			}
			xmlDocument.AppendChild(xmlElement);
			return xmlDocument.InnerXml;
		}

		public static string ToXml<T>(List<T> list)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement(typeof(T).Name + "s");
			foreach (T item in list)
			{
				ModelToNode(xmlDocument, xmlElement, item);
			}
			xmlDocument.AppendChild(xmlElement);
			return xmlDocument.InnerXml;
		}

		public static List<T> ToList<T>(string xml) where T : new()
		{
			List<T> list = new List<T>();
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.LoadXml(xml);
			}
			catch
			{
				return list;
			}
			if (xmlDocument.ChildNodes.Count == 0)
			{
				return list;
			}
			if (xmlDocument.ChildNodes[xmlDocument.ChildNodes.Count - 1].Name.ToLower() != typeof(T).Name.ToLower() + "s")
			{
				return list;
			}
			XmlNode xmlNode = xmlDocument.ChildNodes[xmlDocument.ChildNodes.Count - 1];
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				if (childNode.Name.ToLower() == typeof(T).Name.ToLower())
				{
					list.Add(NodeToModel<T>(childNode));
				}
			}
			return list;
		}

		private static void ModelToNode<T>(XmlDocument doc, XmlElement root, T model)
		{
			Type typeFromHandle = typeof(T);
			XmlElement xmlElement = doc.CreateElement(typeFromHandle.Name);
			PropertyInfo[] properties = typeFromHandle.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (propertyInfo?.CanWrite ?? false)
				{
					string name = propertyInfo.Name;
					string value = string.Empty;
					if (propertyInfo.GetValue(model, null) != null)
					{
						value = propertyInfo.GetValue(model, null).ToString();
					}
					xmlElement.SetAttribute(name, value);
				}
			}
			root.AppendChild(xmlElement);
		}

		private static T NodeToModel<T>(XmlNode node) where T : new()
		{
			T val = new T();
			if (node.NodeType == XmlNodeType.Element)
			{
				XmlElement xmlElement = (XmlElement)node;
				PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
				foreach (XmlAttribute attribute in xmlElement.Attributes)
				{
					string b = attribute.Name.ToLower();
					string text = attribute.Value.ToString();
					PropertyInfo[] array = properties;
					foreach (PropertyInfo propertyInfo in array)
					{
						if (propertyInfo?.CanWrite ?? false)
						{
							string a = propertyInfo.Name.ToLower();
							if (a == b && !string.IsNullOrEmpty(text))
							{
								Type propertyType = propertyInfo.PropertyType;
								if (propertyType == typeof(string))
								{
									propertyInfo.SetValue(val, text, null);
								}
								else if (propertyType == typeof(int))
								{
									propertyInfo.SetValue(val, FPUtils.StrToInt(text), null);
								}
								else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
								{
									propertyInfo.SetValue(val, FPUtils.StrToDateTime(text), null);
								}
								else if (propertyType == typeof(decimal))
								{
									propertyInfo.SetValue(val, FPUtils.StrToDecimal(text), null);
								}
								else if (propertyType == typeof(float))
								{
									propertyInfo.SetValue(val, FPUtils.StrToFloat(text), null);
								}
								else if (propertyType == typeof(double))
								{
									propertyInfo.SetValue(val, FPUtils.StrToDouble(text), null);
								}
								else if (propertyType == typeof(bool))
								{
									propertyInfo.SetValue(val, FPUtils.StrToBool(text, false), null);
								}
								else if (propertyType == typeof(short))
								{
									propertyInfo.SetValue(val, short.Parse(FPUtils.StrToInt(text).ToString()), null);
								}
							}
						}
					}
				}
			}
			return val;
		}
	}
}
