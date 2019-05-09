using System.Configuration;

namespace FangPage.Common
{
	public class FPConfig
	{
		static FPConfig()
		{
		}

		public static string GetAppSettingsKeyValue(string keyName)
		{
			string text = ConfigurationManager.AppSettings.Get(keyName);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return "";
		}

		private static bool AppSettingsKeyExists(string strKey, Configuration config)
		{
			string[] allKeys = config.AppSettings.Settings.AllKeys;
			foreach (string a in allKeys)
			{
				if (a == strKey)
				{
					return true;
				}
			}
			return false;
		}

		public static void AppSettingsSave(string strKey, string newValue)
		{
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			if (AppSettingsKeyExists(strKey, configuration))
			{
				configuration.AppSettings.Settings[strKey].Value = newValue;
				configuration.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection("appSettings");
			}
			else
			{
				configuration.AppSettings.Settings.Add(strKey, newValue);
				configuration.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection("appSettings");
			}
		}

		public static string GetConnectionStringsElementValue(string ConnectionStringsName)
		{
			ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionStringsName];
			return connectionStringSettings.ConnectionString;
		}

		public static void ConnectionStringsSave(string ConnectionStringsName, string elementValue)
		{
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			configuration.ConnectionStrings.ConnectionStrings[ConnectionStringsName].ConnectionString = elementValue;
			configuration.Save(ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection(ConnectionStringsName);
		}
	}
}
