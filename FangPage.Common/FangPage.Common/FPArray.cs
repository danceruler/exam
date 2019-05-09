using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace FangPage.Common
{
	public class FPArray
	{
		public static string[] SplitString(string strContent)
		{
			return SplitString(strContent, ",");
		}

		public static string[] SplitString(string strContent, string separator)
		{
			if (string.IsNullOrEmpty(strContent))
			{
				strContent = "";
			}
			if (strContent.IndexOf(separator) < 0)
			{
				return new string[1]
				{
					strContent
				};
			}
			return Regex.Split(strContent, Regex.Escape(separator), RegexOptions.IgnoreCase);
		}

		public static string[] SplitString(string strContent, int length)
		{
			return SplitString(strContent, ",", length);
		}

		public static string[] SplitString(string strContent, string separator, int length)
		{
			if (string.IsNullOrEmpty(strContent))
			{
				strContent = "";
			}
			string[] array = new string[length];
			string[] array2 = SplitString(strContent, separator);
			for (int i = 0; i < length; i++)
			{
				if (i < array2.Length)
				{
					array[i] = array2[i];
				}
				else
				{
					array[i] = string.Empty;
				}
			}
			return array;
		}

		public static string[] SplitString(string strContent, string[] separator)
		{
			if (string.IsNullOrEmpty(strContent))
			{
				strContent = "";
			}
			if (separator.Length == 0)
			{
				return new string[1]
				{
					strContent
				};
			}
			return strContent.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		}

		public static string[] SplitString(string strContent, string[] separator, int length)
		{
			string[] array = new string[length];
			string[] array2 = SplitString(strContent, separator);
			for (int i = 0; i < length; i++)
			{
				if (i < array2.Length)
				{
					array[i] = array2[i];
				}
				else
				{
					array[i] = string.Empty;
				}
			}
			return array;
		}

		public static int[] SplitInt(string strContent)
		{
			return SplitInt(strContent, ",");
		}

		public static int[] SplitInt(string strContent, int length)
		{
			return SplitInt(strContent, ",", length);
		}

		public static int[] SplitInt(string strContent, string separator)
		{
			string[] array = SplitString(strContent, separator);
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = FPUtils.StrToInt(array[i]);
			}
			return array2;
		}

		public static int[] SplitInt(string strContent, string separator, int length)
		{
			int[] array = new int[length];
			string[] array2 = SplitString(strContent, separator);
			for (int i = 0; i < length; i++)
			{
				if (i < array2.Length)
				{
					array[i] = FPUtils.StrToInt(array2[i]);
				}
				else
				{
					array[i] = 0;
				}
			}
			return array;
		}

		public static int[] SplitInt(string strContent, string[] separator)
		{
			string[] array = SplitString(strContent, separator);
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = FPUtils.StrToInt(array[i]);
			}
			return array2;
		}

		public static int[] SplitInt(string strContent, string[] separator, int length)
		{
			int[] array = new int[length];
			string[] array2 = SplitString(strContent, separator);
			for (int i = 0; i < length; i++)
			{
				if (i < array2.Length)
				{
					array[i] = FPUtils.StrToInt(array2[i]);
				}
				else
				{
					array[i] = 0;
				}
			}
			return array;
		}

		public static string Join(string[] strContent)
		{
			return Join(strContent, "");
		}

		public static string Join(string[] strContent, string separator)
		{
			if (string.IsNullOrEmpty(separator))
			{
				separator = ",";
			}
			if (strContent.Length == 0)
			{
				return "";
			}
			return string.Join(separator, strContent);
		}

		public static string Join(int[] strContent)
		{
			return Join(strContent, "");
		}

		public static string Join(int[] strContent, string separator)
		{
			if (string.IsNullOrEmpty(separator))
			{
				separator = ",";
			}
			if (strContent.Length == 0)
			{
				return "";
			}
			string text = "";
			foreach (int item in strContent)
			{
				text = Append(text, item, separator);
			}
			return text;
		}

		public static string Append(string strContent, string item)
		{
			return Append(strContent, item, ",");
		}

		public static string Append(string strContent, int item)
		{
			return Append(strContent, item, ",");
		}

		public static string Append(string strContent, int item, string separator)
		{
			return Append(strContent, item.ToString(), separator);
		}

		public static string Append(string strContent, string item, string separator)
		{
			if (string.IsNullOrEmpty(strContent))
			{
				strContent = "";
			}
			if (separator.Length == 1)
			{
				char c = Convert.ToChar(separator);
				strContent = strContent.TrimStart(c).TrimEnd(c);
				item = item.TrimStart(c).TrimEnd(c);
			}
			if (!string.IsNullOrEmpty(item))
			{
				if (!string.IsNullOrEmpty(strContent))
				{
					strContent += separator;
				}
				strContent += item;
			}
			return strContent;
		}

		public static string Push(string strContent, string item)
		{
			return Push(strContent, item, ",");
		}

		public static string Push(string strContent, int item)
		{
			return Push(strContent, item, ",");
		}

		public static string Push(string strContent, int item, string separator)
		{
			return Push(strContent, item.ToString(), separator);
		}

		public static string Push(string strContent, string item, string separator)
		{
			if (string.IsNullOrEmpty(strContent))
			{
				strContent = "";
			}
			if (separator.Length == 1)
			{
				char c = Convert.ToChar(separator);
				strContent = strContent.TrimStart(c).TrimEnd(c);
				item = item.TrimStart(c).TrimEnd(c);
			}
			if (!string.IsNullOrEmpty(item))
			{
				string[] array = SplitString(item, separator);
				foreach (string item2 in array)
				{
					if (InArray(item2, strContent, separator.ToString()) == -1)
					{
						strContent = Append(strContent, item2, separator);
					}
				}
			}
			return strContent;
		}

		public static int InArray(string item, string[] stringArray)
		{
			for (int i = 0; i < stringArray.Length; i++)
			{
				if (stringArray[i] == item)
				{
					return i;
				}
			}
			return -1;
		}

		public static int InArray(int item, string[] stringArray)
		{
			for (int i = 0; i < stringArray.Length; i++)
			{
				if (FPUtils.StrToInt(stringArray[i]) == item)
				{
					return i;
				}
			}
			return -1;
		}

		public static int InArray(int item, string stringArray)
		{
			return InArray(item, stringArray, ",");
		}

		public static int InArray(string item, string stringArray)
		{
			return InArray(item, stringArray, ",");
		}

		public static int InArray(int item, string stringArray, string separator)
		{
			return InArray(item.ToString(), stringArray, separator);
		}

		public static int InArray(string item, string stringArray, string separator)
		{
			if (stringArray != null && stringArray != "")
			{
				string[] array = SplitString(stringArray, separator);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == item)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static int InArray(string item, string stringArray, string[] separator)
		{
			if (stringArray != null && stringArray != "")
			{
				string[] array = SplitString(stringArray, separator);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == item)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static bool InIPArray(string ip, string[] iparray)
		{
			string[] array = SplitString(ip, ".");
			for (int i = 0; i < iparray.Length; i++)
			{
				string[] array2 = SplitString(iparray[i], ".");
				int num = 0;
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j] == "*")
					{
						return true;
					}
					if (array.Length > j && array2[j] == array[j])
					{
						num++;
						continue;
					}
					break;
				}
				if (num == 4)
				{
					return true;
				}
			}
			return false;
		}

		public static bool InMobileArray(string mobile, string[] mobilearray)
		{
			for (int i = 0; i < mobilearray.Length; i++)
			{
				if (mobilearray[i].IndexOf('*') > 0)
				{
					string value = mobile.Substring(0, mobilearray[i].IndexOf('*'));
					if (mobile.StartsWith(value))
					{
						return true;
					}
				}
				else if (mobilearray[i] == mobile)
				{
					return true;
				}
			}
			return false;
		}

		public static bool Contain(string[] stringArray, string item)
		{
			return InArray(item, stringArray) >= 0;
		}

		public static bool Contain(string[] stringArray, int item)
		{
			return InArray(item, stringArray) >= 0;
		}

		public static bool Contain(string stringArray, string item)
		{
			return InArray(item, stringArray) >= 0;
		}

		public static bool Contain(string stringArray, int item)
		{
			return InArray(item, stringArray) >= 0;
		}

		public static bool Contain(string stringArray, string item, string separator)
		{
			return InArray(item, stringArray, separator) >= 0;
		}

		public static bool Contain(string stringArray, int item, string separator)
		{
			return InArray(item, stringArray, separator) >= 0;
		}

		public static bool Contain(string stringArray, string item, string[] separator)
		{
			return InArray(item, stringArray, separator) >= 0;
		}

		public static string Replace(string strContent, string item, string value)
		{
			string text = "";
			string[] array = SplitString(strContent);
			foreach (string a in array)
			{
				text = ((!(a == item)) ? Push(text, item) : Push(text, value));
			}
			return text;
		}

		public static string Replace(string strContent, string value, int index)
		{
			return Replace(strContent, value, index, ",");
		}

		public static string Replace(string strContent, int value, int index)
		{
			return Replace(strContent, value, index, ",");
		}

		public static string Replace(string strContent, int value, int index, string separator)
		{
			return Replace(strContent, value.ToString(), index, separator);
		}

		public static string Replace(string strContent, string value, int index, string separator)
		{
			string text = "";
			int num = 0;
			string[] array = SplitString(strContent, separator);
			foreach (string str in array)
			{
				if (num > 0)
				{
					text += separator;
				}
				text = ((num != index) ? (text + str) : (text + value));
				num++;
			}
			return text;
		}

		public static string Update(string strContent, int value, int index)
		{
			return Update(strContent, value, index, ",");
		}

		public static string Update(string strContent, int value, int index, string separator)
		{
			string text = "";
			int num = 0;
			int[] array = SplitInt(strContent, separator);
			for (int i = 0; i < array.Length; i++)
			{
				int num2 = array[i];
				if (num > 0)
				{
					text += separator;
				}
				text = ((num != index) ? (text + num2.ToString()) : (text + (num2 + value).ToString()));
				num++;
			}
			return text;
		}

		public static string GetString(string strContent, int index)
		{
			return GetString(strContent, index, ",");
		}

		public static string GetString(string strContent, int index, string separator)
		{
			string[] array = SplitString(strContent, separator);
			if (index > -1 && index < array.Length)
			{
				return array[index];
			}
			return "";
		}

		public static string GetString(string strContent, int index, string[] separator)
		{
			string[] array = SplitString(strContent, separator);
			if (index > -1 && index < array.Length)
			{
				return array[index];
			}
			return "";
		}

		public static string GetString(string strContent, string strValue, string item)
		{
			int num = InArray(item, strContent);
			if (num >= 0)
			{
				return GetString(strValue, num);
			}
			return "";
		}

		public static string GetString(string strContent, string strValue, string item, string separator)
		{
			int num = InArray(item, strContent, separator);
			if (num >= 0)
			{
				return GetString(strValue, num, separator);
			}
			return "";
		}

		public static string GetString(string strContent, string strValue, string item, string[] separator)
		{
			int num = InArray(item, strContent, separator);
			if (num >= 0)
			{
				return GetString(strValue, num, separator);
			}
			return "";
		}

		public static int GetInt(string strContent, int index)
		{
			return GetInt(strContent, index, ",");
		}

		public static int GetInt(string strContent, int index, string separator)
		{
			int[] array = SplitInt(strContent, separator);
			if (index > -1 && index < array.Length)
			{
				return array[index];
			}
			return 0;
		}

		public static int GetInt(string strContent, int index, string[] separator)
		{
			int[] array = SplitInt(strContent, separator);
			if (index > -1 && index < array.Length)
			{
				return array[index];
			}
			return 0;
		}

		public static int GetInt(string strContent, string strValue, string item)
		{
			int num = InArray(item, strContent);
			if (num >= 0)
			{
				return GetInt(strValue, num);
			}
			return 0;
		}

		public static int GetInt(string strContent, string strValue, string item, string separator)
		{
			int num = InArray(item, strContent, separator);
			if (num >= 0)
			{
				return GetInt(strValue, num, separator);
			}
			return 0;
		}

		public static int GetInt(string strContent, string strValue, string item, string[] separator)
		{
			int num = InArray(item, strContent, separator);
			if (num >= 0)
			{
				return GetInt(strValue, num, separator);
			}
			return 0;
		}

		public static string FmatInt(string strContent)
		{
			string text = "";
			string[] array = SplitString(strContent);
			foreach (string text2 in array)
			{
				if (FPUtils.IsNumeric(text2))
				{
					text = Push(text, text2);
				}
			}
			return text;
		}

		public static string Remove(string strContent, int index)
		{
			return Remove(strContent, index, ",");
		}

		public static string Remove(string strContent, string remove)
		{
			return Remove(strContent, remove, ",");
		}

		public static string Remove(string strContent, string remove, string separator)
		{
			string text = "";
			string[] array = SplitString(strContent, separator);
			foreach (string item in array)
			{
				if (InArray(item, remove, separator) == -1)
				{
					text = Append(text, item, separator);
				}
			}
			return text;
		}

		public static string Remove(string strContent, int index, string separator)
		{
			string text = "";
			int num = 0;
			string[] array = SplitString(strContent, separator);
			foreach (string item in array)
			{
				if (num != index)
				{
					text = Append(text, item, separator);
				}
				num++;
			}
			return text;
		}

		public static string RemoveSame(string TempArray)
		{
			string[] array = RemoveSame(SplitString(TempArray));
			if (array.Length != 0)
			{
				return string.Join(",", array);
			}
			return "";
		}

		public static string RemoveSame(string TempArray, string separator)
		{
			string[] array = RemoveSame(SplitString(TempArray, separator));
			if (array.Length != 0)
			{
				return string.Join(separator, array);
			}
			return "";
		}

		public static string[] RemoveSame(string[] TempArray)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < TempArray.Length; i++)
			{
				if (!arrayList.Contains(TempArray[i]))
				{
					arrayList.Add(TempArray[i]);
				}
			}
			return (string[])arrayList.ToArray(typeof(string));
		}
	}
}
