using FangPage.Common;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace FangPage.MVC
{
	public class FPCache
	{
		public static void Insert(string key, object obj)
		{
			HttpContext.Current.Cache.Insert(key, obj);
		}

		public static void Insert(string key, object obj, string fileName)
		{
			CacheDependency dependencies = new CacheDependency(fileName);
			HttpContext.Current.Cache.Insert(key, obj, dependencies);
		}

		public static void Insert(string key, object obj, int expires)
		{
			HttpContext.Current.Cache.Insert(key, obj, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, expires, 0));
		}

		public static void InsertAt(string name, string key, object obj)
		{
			object obj2 = Get(name);
			Hashtable hashtable = (obj2 == null) ? new Hashtable() : (obj2 as Hashtable);
			if (hashtable[key] == null)
			{
				hashtable.Add(key, obj);
			}
			else
			{
				hashtable[key] = obj;
			}
			Insert(name, hashtable);
		}

		public static void InsertAt(string name, int key, object obj)
		{
			InsertAt(name, key.ToString(), obj);
		}

		public static void InsertAt(string name, string key, object obj, int expires)
		{
			object obj2 = Get(name);
			Hashtable hashtable = (obj2 == null) ? new Hashtable() : (obj2 as Hashtable);
			if (hashtable[key] == null)
			{
				hashtable.Add(key, obj);
			}
			else
			{
				hashtable[key] = obj;
			}
			Insert(name, hashtable, expires);
		}

		public static void InsertAt(string name, int key, object obj, int expires)
		{
			InsertAt(name, key.ToString(), obj, expires);
		}

		public static object Get(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			return HttpContext.Current.Cache.Get(key);
		}

		public static object Get(string name, string key)
		{
			object obj = Get(name);
			object result = null;
			if (obj != null)
			{
				Hashtable hashtable = obj as Hashtable;
				if (hashtable[key] != null)
				{
					result = hashtable[key];
				}
			}
			return result;
		}

		public static object Get(string name, int key)
		{
			return Get(name, key.ToString());
		}

		public static void Remove(string key)
		{
			if (!string.IsNullOrEmpty(key))
			{
				HttpContext.Current.Cache.Remove(key);
			}
		}

		public static void Remove(string name, int key)
		{
			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			object obj = Get(name);
			if (obj != null)
			{
				Hashtable hashtable = obj as Hashtable;
				if (hashtable[key] != null)
				{
					hashtable.Remove(key);
				}
				Insert(name, hashtable);
			}
		}

		public static void Remove(string name, string keys)
		{
			if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(keys))
			{
				return;
			}
			object obj = Get(name);
			if (obj == null)
			{
				return;
			}
			Hashtable hashtable = obj as Hashtable;
			string[] array = FPArray.SplitString(keys);
			foreach (string key in array)
			{
				if (hashtable[key] != null)
				{
					hashtable.Remove(key);
				}
			}
			Insert(name, hashtable);
		}

		public static void Remove(string name, int key, int expires)
		{
			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			object obj = Get(name);
			if (obj != null)
			{
				Hashtable hashtable = obj as Hashtable;
				if (hashtable[key] != null)
				{
					hashtable.Remove(key);
				}
				Insert(name, hashtable, expires);
			}
		}

		public static void Remove(string name, string keys, int expires)
		{
			if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(keys))
			{
				return;
			}
			object obj = Get(name);
			if (obj == null)
			{
				return;
			}
			Hashtable hashtable = obj as Hashtable;
			string[] array = FPArray.SplitString(keys);
			foreach (string key in array)
			{
				if (hashtable[key] != null)
				{
					hashtable.Remove(key);
				}
			}
			Insert(name, hashtable, expires);
		}

		public static void RemoveStart(string startkey)
		{
			if (string.IsNullOrEmpty(startkey))
			{
				return;
			}
			IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Key.ToString().StartsWith(startkey))
				{
					HttpContext.Current.Cache.Remove(enumerator.Key.ToString());
				}
			}
		}

		public static void RemoveList(string name, string keys)
		{
			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			string[] array = FPArray.SplitString(keys);
			foreach (string text in array)
			{
				if (text != "")
				{
					Remove(name + text);
				}
			}
		}

		public static void RemovePattern(string pattern)
		{
			IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
			while (enumerator.MoveNext())
			{
				if (regex.IsMatch(enumerator.Key.ToString()))
				{
					HttpContext.Current.Cache.Remove(enumerator.Key.ToString());
				}
			}
		}

		public static void Clear()
		{
			IDictionaryEnumerator enumerator = HttpContext.Current.Cache.GetEnumerator();
			ArrayList arrayList = new ArrayList();
			while (enumerator.MoveNext())
			{
				arrayList.Add(enumerator.Key);
			}
			foreach (string item in arrayList)
			{
				HttpContext.Current.Cache.Remove(item);
			}
		}
	}
}
