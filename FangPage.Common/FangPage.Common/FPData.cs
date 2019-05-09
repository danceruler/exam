using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace FangPage.Common
{
	[Serializable]
	public class FPData : ISerializable
	{
		private Dictionary<string, string> m_data = new Dictionary<string, string>();

		public string this[string key]
		{
			get
			{
				if (!m_data.ContainsKey(key))
				{
					return "";
				}
				return m_data[key];
			}
			set
			{
				m_data[key] = value;
			}
		}

		public string this[int index]
		{
			get
			{
				if (!m_data.ContainsKey(index.ToString()))
				{
					return "";
				}
				return m_data[index.ToString()];
			}
			set
			{
				m_data[index.ToString()] = value;
			}
		}

		public int Count => m_data.Count;

		public Dictionary<string, string> Data => m_data;

		public string Json => FPJson.ToJson(m_data);

		public string[] Keys => new List<string>(m_data.Keys).ToArray();

		public string[] Values => new List<string>(m_data.Values).ToArray();

		public FPData()
		{
		}

		public FPData(string json)
		{
			m_data = FPJson.ToModel<Dictionary<string, string>>(json);
		}

		public FPData(object obj)
		{
			try
			{
				Type type = obj.GetType();
				if (type.GetProperties().Length != 0)
				{
					PropertyInfo[] properties = type.GetProperties();
					foreach (PropertyInfo propertyInfo in properties)
					{
						m_data[propertyInfo.Name] = propertyInfo.GetValue(obj, null).ToString();
					}
				}
			}
			catch
			{
			}
		}

		protected FPData(SerializationInfo info, StreamingContext context)
		{
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				m_data[enumerator.Name] = enumerator.Value.ToString();
			}
		}

		public bool ContainsKey(string key)
		{
			return m_data.ContainsKey(key);
		}

		public bool ContainsValue(string value)
		{
			return m_data.ContainsValue(value);
		}

		public void Add(string key, string value)
		{
			m_data.Add(key, value);
		}

		public bool Remove(string key)
		{
			return m_data.Remove(key);
		}

		public void Clear()
		{
			m_data.Clear();
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			foreach (KeyValuePair<string, string> datum in m_data)
			{
				info.AddValue(datum.Key, datum.Value);
			}
		}

		public override string ToString()
		{
			return FPJson.ToJson(m_data);
		}
	}
}
