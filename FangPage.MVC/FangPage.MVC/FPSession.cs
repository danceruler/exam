using FangPage.Common;
using System.Collections;
using System.Web;

namespace FangPage.MVC
{
	public class FPSession
	{
		public static void Insert(string key, object obj)
		{
			HttpContext.Current.Session.Add(key, obj);
		}

		public static void Insert(string key, string u_key, object obj)
		{
			object obj2 = Get(key);
			Hashtable hashtable = (obj2 == null) ? new Hashtable() : (obj2 as Hashtable);
			if (hashtable[u_key] == null)
			{
				hashtable.Add(u_key, obj);
			}
			else
			{
				hashtable[u_key] = obj;
			}
			Insert(key, hashtable);
		}

		public static void Insert(string key, int u_key, object obj)
		{
			Insert(key, u_key.ToString(), obj);
		}

		public static object Get(string key)
		{
			if (HttpContext.Current.Session[key] != null)
			{
				return HttpContext.Current.Session[key];
			}
			return null;
		}

		public static object Get(string key, string u_key)
		{
			object obj = Get(key);
			object result = null;
			if (obj != null)
			{
				Hashtable hashtable = obj as Hashtable;
				if (hashtable[u_key] != null)
				{
					result = hashtable[u_key];
				}
			}
			return result;
		}

		public static object Get(string key, int u_key)
		{
			return Get(key, u_key.ToString());
		}

		public static void Remove(string key)
		{
			if (!string.IsNullOrEmpty(key) && HttpContext.Current.Session[key] != null)
			{
				HttpContext.Current.Session.Remove(key);
				HttpContext.Current.Session[key] = null;
			}
		}

		public static void Remove(string key, string u_keys)
		{
			if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(u_keys))
			{
				return;
			}
			object obj = Get(key);
			if (obj == null)
			{
				return;
			}
			Hashtable hashtable = obj as Hashtable;
			string[] array = FPArray.SplitString(u_keys);
			foreach (string key2 in array)
			{
				if (hashtable[key2] != null)
				{
					hashtable.Remove(key2);
				}
			}
			Insert(key, hashtable);
		}

		public static void Remove(string key, int u_key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return;
			}
			object obj = Get(key);
			if (obj != null)
			{
				Hashtable hashtable = obj as Hashtable;
				if (hashtable[u_key] != null)
				{
					hashtable.Remove(u_key);
				}
				Insert(key, hashtable);
			}
		}
	}
}
