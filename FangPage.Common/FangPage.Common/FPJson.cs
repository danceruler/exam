using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace FangPage.Common
{
	public class FPJson
	{
		public static T LoadModel<T>(string filename) where T : new()
		{
			string json = FPFile.ReadFile(filename);
			return ToModel<T>(json);
		}

		public static List<T> LoadList<T>(string filename) where T : new()
		{
			string json = FPFile.ReadFile(filename);
			return ToList<T>(json);
		}

		public static void SaveJson<T>(T model, string filename)
		{
			string content = ToJson(model);
			FPFile.WriteFile(filename, content);
		}

		public static void SaveJson<T>(List<T> list, string filename)
		{
			string content = ToJson(list);
			FPFile.WriteFile(filename, content);
		}

		public static string ToJson(object obj)
		{
			IsoDateTimeConverter isoDateTimeConverter = new IsoDateTimeConverter();
			isoDateTimeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
			return JsonConvert.SerializeObject(obj, isoDateTimeConverter);
		}

		public static T ToModel<T>(string json) where T : new()
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(json);
			}
			catch
			{
				return default(T);
			}
		}

		public static List<T> ToList<T>(string json) where T : new()
		{
			try
			{
				return JsonConvert.DeserializeObject<List<T>>(json);
			}
			catch
			{
				return new List<T>();
			}
		}

		public static List<T> ToList<T>(string json, int count) where T : new()
		{
			List<T> list = ToList<T>(json);
			int count2 = list.Count;
			if (count > count2)
			{
				for (int i = 0; i < count - count2; i++)
				{
					list.Add(new T());
				}
			}
			return list;
		}
	}
}
