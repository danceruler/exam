using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;

namespace FangPage.Common
{
	public class FPExcel
	{
		private static string OleDb = ConfigurationManager.AppSettings["OLEDB"];

		public static DataTable GetExcelTable(string xlspath)
		{
			return GetExcelTable(xlspath, true);
		}

		public static DataTable GetExcelTable(string xlspath, bool first)
		{
			if (string.IsNullOrEmpty(OleDb))
			{
				OleDb = "JET";
			}
			string text = "NO";
			if (!first)
			{
				text = "YES";
			}
			string text2 = "";
			text2 = ((!(OleDb.ToUpper() == "JET")) ? ("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + xlspath + "';Extended Properties='Excel 12.0;HDR=" + text + ";IMEX=1'") : ("Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + xlspath + "';Extended Properties='Excel 8.0;HDR=" + text + ";IMEX=1'"));
			DataTable dataTable = new DataTable();
			using (OleDbConnection oleDbConnection = new OleDbConnection(text2))
			{
				try
				{
					oleDbConnection.Open();
					DataTable oleDbSchemaTable = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[4]
					{
						null,
						null,
						null,
						"Table"
					});
					string[] array = new string[oleDbSchemaTable.Rows.Count];
					for (int i = 0; i < oleDbSchemaTable.Rows.Count; i++)
					{
						array[i] = oleDbSchemaTable.Rows[i]["TABLE_NAME"].ToString();
					}
					string selectCommandText = "SELECT * FROM [" + array[0] + "]";
					OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(selectCommandText, oleDbConnection);
					oleDbDataAdapter.Fill(dataTable);
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			if (dataTable.Rows.Count >= 1)
			{
				for (int j = 0; j < dataTable.Columns.Count; j++)
				{
					string text3 = dataTable.Rows[0].ItemArray[j].ToString().Trim();
					if (!string.IsNullOrEmpty(text3))
					{
						dataTable.Columns[j].ColumnName = text3;
					}
				}
				dataTable.Rows.RemoveAt(0);
			}
			return dataTable;
		}
	}
}
