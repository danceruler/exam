//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace FP_entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class FP_WMS_AttachInfo
    {
        public int id { get; set; }
        public string attachid { get; set; }
        public Nullable<int> parentid { get; set; }
        public string platform { get; set; }
        public string app { get; set; }
        public Nullable<int> postid { get; set; }
        public Nullable<int> uid { get; set; }
        public string name { get; set; }
        public string filename { get; set; }
        public string filetype { get; set; }
        public Nullable<int> filesize { get; set; }
        public string description { get; set; }
        public Nullable<System.DateTime> uploadtime { get; set; }
        public Nullable<int> downloads { get; set; }
        public Nullable<int> reads { get; set; }
        public Nullable<int> issync { get; set; }
    }
}