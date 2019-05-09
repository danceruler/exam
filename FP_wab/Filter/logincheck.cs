using FP_entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace FP_wab.Filter
{
    public class logincheck: ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        { 
            FP_ExamEntities db = new FP_ExamEntities();
            if(HttpContext.Current.Session["FP_WAPLOGIN"] == null)
            {
                HttpContext.Current.Response.Redirect("/Login/Index");
            }
            else
            {
                //检测用户信息
                FP_WMS_UserInfo nowuser = HttpContext.Current.Session["FP_WAPLOGIN"] as FP_WMS_UserInfo;
                FP_WMS_UserInfo userindb = db.FP_WMS_UserInfo.SingleOrDefault(t => t.id == nowuser.id);
                if(userindb == null)
                {
                    HttpContext.Current.Session.Remove("FP_WAPLOGIN");
                    HttpContext.Current.Response.Redirect("/Login/Index");
                }
                nowuser = userindb;
            }
        }
    }
}