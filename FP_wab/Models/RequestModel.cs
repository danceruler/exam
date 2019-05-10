using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FP_wab.Models
{
    public class PaperResModel
    {
        public int departid { get; set; }
        public int uid { get; set; }
        public int channalid { get; set; }
        public int sortid { get; set; }
        public string name { get; set; }
        public int total { get; set; }
        public int passmark { get; set; }
        public bool islimit { get; set; }
        public int examtime { get; set; }
        public DateTime startime { get; set; }
        public DateTime endtime { get; set; }
        public int repeats { get; set; }
        public string title { get; set; }
    }

    public class QuestionResModel
    {
        public int channalid { get; set; }
        public int display { get; set; }
        public int parentid { get; set; }
        public string name { get; set; }
        public string markup { get; set; }
        public int pagesize { get; set; }
        public string description { get; set; }
        public int showtype { get; set; }
    }
}