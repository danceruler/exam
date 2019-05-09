using System;
using System.Security.Cryptography;
using System.Text;

namespace FangPage.Common
{
	public class FPRandom
	{
		private static string[] verifycodeRange = new string[32]
		{
			"0",
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7",
			"8",
			"9",
			"a",
			"b",
			"c",
			"d",
			"e",
			"f",
			"g",
			"h",
			"j",
			"k",
			"m",
			"n",
			"p",
			"q",
			"r",
			"s",
			"t",
			"u",
			"v",
			"w",
			"x",
			"y"
		};

		public static string CreateCode(int len)
		{
			string text = string.Empty;
			long num = GetRandomSeed();
			Random random = new Random((int)(num & uint.MaxValue) | (int)(num >> 32));
			for (int i = 0; i < len; i++)
			{
				int num2 = random.Next();
				text += ((char)((num2 % 2 != 0) ? ((ushort)(65 + (ushort)(num2 % 26))) : ((ushort)(48 + (ushort)(num2 % 10))))).ToString();
			}
			return text;
		}

		public static string CreateCode(string prefix, int len)
		{
			string empty = string.Empty;
			if (string.IsNullOrEmpty(prefix))
			{
				prefix = "";
			}
			return prefix + CreateCode(len);
		}

		public static string CreateCodeNum(int len)
		{
			string text = string.Empty;
			long num = GetRandomSeed();
			Random random = new Random((int)(num & uint.MaxValue) | (int)(num >> 32));
			for (int i = 0; i < len; i++)
			{
				int num2 = random.Next();
				text += ((char)(ushort)(48 + (ushort)(num2 % 10))).ToString();
			}
			return text;
		}

		public static string CreateCodeNum(string prefix, int len)
		{
			string empty = string.Empty;
			if (string.IsNullOrEmpty(prefix))
			{
				prefix = "";
			}
			return prefix + CreateCodeNum(len);
		}

		public static string CreateAuth(int len)
		{
			StringBuilder stringBuilder = new StringBuilder();
			long num = GetRandomSeed();
			Random random = new Random((int)(num & uint.MaxValue) | (int)(num >> 32));
			for (int i = 0; i < len; i++)
			{
				int num2 = random.Next();
				if (num2 % 2 == 0)
				{
					stringBuilder.Append((char)(48 + (ushort)(num2 % 10)));
				}
				else
				{
					stringBuilder.Append((char)(65 + (ushort)(num2 % 26)));
				}
			}
			return stringBuilder.ToString();
		}

		public static string CreateGuid()
		{
			return Guid.NewGuid().ToString();
		}

		private static int GetRandomSeed()
		{
			byte[] array = new byte[4];
			RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
			rNGCryptoServiceProvider.GetBytes(array);
			int num = BitConverter.ToInt32(array, 0);
			if (num < 0)
			{
				num *= -1;
			}
			return num;
		}
	}
}
