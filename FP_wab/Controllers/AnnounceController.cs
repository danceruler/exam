using FP_wab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FP_wab.Controllers
{
    public class AnnounceController : Controller
    {
        // GET: Announce
        public ActionResult Index(string content,string buttonContent,string backUrl,AnnounceType type)
        {
            ViewBag.Content = content;
            ViewBag.buttonContent = buttonContent;
            ViewBag.backUrl = backUrl;
            switch (type)
            {
                case AnnounceType.SUCCESS:
                    ViewBag.title = "提交成功";
                    break;
                case AnnounceType.FAIL:
                    ViewBag.title = "提示";
                    break;
                default:
                    break;
            }
            return View();
        }
    }
}