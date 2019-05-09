using FangPage.Common;
using FangPage.MVC;
using FP_entity;
using FP_wab.Help;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FP_wab.Controllers
{
    public class LoginController : Controller
    {
        private FP_ExamEntities db = new FP_ExamEntities();
        //private string HomeUrl = "http://localhost:52822/Exam/Index";
        //private string LoginUrl = "http://localhost:52822/Login/Index";
        private string HomeUrl = ConfigurationManager.AppSettings["DomainPath"] + "/Exam/Index";
        private string LoginUrl = ConfigurationManager.AppSettings["DomainPath"]+"/Login/Index";
        //private string HomeUrl = "http://47.99.118.16:8011/Exam/Index";
        //private string LoginUrl = "http://47.99.118.16:8011/Login/Index";
        // GET: Login
        public ActionResult Index(string backurl = "")
        {
            ViewBag.backUrl = backurl;
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult CheckLogin(UserLoginModel userinfomodel)
        {
            var password = FPUtils.MD5(userinfomodel.password);
            var userInfo = db.FP_WMS_UserInfo.SingleOrDefault(t => t.username == userinfomodel.username & t.password == password);
            if(userInfo == null)
            {
                return Json(new { Status = 0, Content = "用户名或者密码错误" });
            }
            else
            {
                if (userInfo.id > 0)
                {
                    if (userInfo.roleid == 4)
                    {
                        return Json(new { Status = 0, Content = "对不起，该帐户已被禁止登录" });
                    }
                    if (userInfo.roleid == 3)
                    {
                        return Json(new { Status = 0, Content = "对不起，您的账号尚未被激活或者尚未被审核" });
                    }
                    if (userInfo.state == 0)
                    {
                        return Json(new { Status = 0, Content = "抱歉, 您的帐号已被禁止使用。" });
                    }
                    Session.Add("FP_WAPLOGIN", userInfo);
                    //SysBll.InsertLog(userInfo.id, "用户登录", "登录成功，登录名：" + userInfo.username, true);
                    if(userinfomodel.callbackurl == ""|| userinfomodel.callbackurl == null)
                    {
                        userinfomodel.callbackurl = HomeUrl;
                    }
                    return Json(new { Status = 1, Content = "登录成功",backurl = userinfomodel.callbackurl });
                }
                else
                {
                    //SysBll.InsertLog(userInfo.id, "用户登录", "登录失败，登录名：" + userinfomodel.username + ",密码：" + password, false);
                    return Json(new { Status = 0, Content = "用户id异常" });
                }
                
            }
            
        }

        public ActionResult CreateUser(UserRegModel userReg)
        {
            try
            {
                var user = db.FP_WMS_UserInfo.SingleOrDefault(t => t.username == userReg.username);
                if (user != null)
                {
                    return Json(new { Status = 0, Content = "用户已存在" });
                }
                FP_WMS_UserInfo new_user = new FP_WMS_UserInfo();
                new_user.roleid = 3;
                new_user.roles = "";
                new_user.departid = 0;
                new_user.departname = "";
                new_user.departlist = "";
                new_user.departs = "";
                new_user.display = 0;
                new_user.gradeid = 0;
                new_user.types = "";
                new_user.username = userReg.username;
                new_user.password = FPUtils.MD5(userReg.password);
                new_user.password2 = "";
                new_user.email = "";
                new_user.isemail = 0;
                new_user.mobile = userReg.phonenumber;
                new_user.ismobile = 0;
                new_user.realname = userReg.truename;
                new_user.cardtype = "";
                new_user.idcard = "";
                new_user.isreal = 0;
                new_user.usercode = "";
                new_user.nickname = "";
                new_user.avatar = "";
                new_user.sex = "";
                new_user.exp = 0;
                new_user.credits = 0;
                new_user.regip = CreateExamHelp.GetLocalIP();
                new_user.joindatetime = DateTime.Now;
                new_user.sumlogin = 0;
                new_user.lastip = "";
                new_user.lastvisit = new_user.joindatetime;
                new_user.secques = "";
                new_user.authstr = "";
                new_user.authtime = new_user.joindatetime;
                new_user.authflag = 1;
                new_user.vipdate = "";
                new_user.state = 1;
                new_user.issso = 0;
                new_user.extend = "{}";
                db.FP_WMS_UserInfo.Add(new_user);
                db.SaveChanges();
                return Json(new { Status = 1, Content = "注册成功，请等待管理员审核" });

            }
            catch(Exception e)
            {
                return Json(new { Status = 0, Content = "注册失败出现异常" });
            }
            

        }

        public ActionResult Logout()
        {
            var user = Session["FP_WAPLOGIN"] as FP_WMS_UserInfo;
            if (user == null) return Json(new { Status = 0, Content = "用户已经注销" });
            Session.Remove("FP_WAPLOGIN");
            return Json(new { Status = 1, Content = "注销成功",url = LoginUrl });
        }
    }

    public class UserLoginModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string callbackurl { get; set; }
    }

    public class UserRegModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string truename { get; set; }
        public string phonenumber { get; set; }
        public string email { get; set; }
    }
}