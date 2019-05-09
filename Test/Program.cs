using FP_wab.Help;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = ExcelHelp.InputFromExcel("D:\\周报\\test.xlsx","sheet1");
            foreach(DataRow row in t.Rows)
            {
                var a = row["list1"].ToString();
            }
        }
    }
}
