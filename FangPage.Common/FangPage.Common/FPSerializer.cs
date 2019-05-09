using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FangPage.Common
{
	public class FPSerializer
	{
		private static object lockHelper = new object();

		private FPSerializer()
		{
		}

		public static T Load<T>(string filename) where T : new()
		{
			T result = new T();
			if (File.Exists(filename))
			{
				Type typeFromHandle = typeof(T);
				lock (lockHelper)
				{
					using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						XmlSerializer xmlSerializer = new XmlSerializer(typeFromHandle);
						result = (T)xmlSerializer.Deserialize(fileStream);
						fileStream.Close();
					}
				}
			}
			return result;
		}

		public static bool Save<T>(string filename) where T : new()
		{
			T obj = new T();
			return Save(obj, filename);
		}

		public static bool Save<T>(T obj, string filename)
		{
			bool result = false;
			if (!Directory.Exists(Path.GetDirectoryName(filename)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(filename));
			}
			if (File.Exists(filename) && File.GetAttributes(filename).ToString().IndexOf("ReadOnly") != -1)
			{
				File.SetAttributes(filename, FileAttributes.Normal);
			}
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
				XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
				xmlSerializer.Serialize(fileStream, obj);
				result = true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				fileStream?.Close();
			}
			return result;
		}

		public static string Serialize<T>(T obj) where T : new()
		{
			if (obj == null)
			{
				obj = new T();
			}
			string result = "";
			XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
			MemoryStream memoryStream = new MemoryStream();
			XmlTextWriter xmlTextWriter = null;
			StreamReader streamReader = null;
			try
			{
				xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlSerializer.Serialize(xmlTextWriter, obj);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				streamReader = new StreamReader(memoryStream);
				result = streamReader.ReadToEnd();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				xmlTextWriter?.Close();
				streamReader?.Close();
				memoryStream.Close();
			}
			return result;
		}
	}
}
