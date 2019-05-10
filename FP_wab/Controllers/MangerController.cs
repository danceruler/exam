using FP_entity;
using FP_wab.Filter;
using FP_wab.Help;
using FP_wab.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace FP_wab.Controllers
{
    //[logincheck]
    public class MangerController : Controller
    {
        private FP_ExamEntities db = new FP_ExamEntities();

        #region 视图
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 接口

        /// <summary>
        /// 创建examinfo
        /// </summary>
        /// <param name="request"></param>
        public void CreatePaper(PaperResModel request)
        {

        }

        /// <summary>
        /// 创建题库
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult CreateQuestionRes(QuestionResModel request)
        {
            int createResult = QuestionHelp.AddQuestionRes(request);
            if (createResult == 1) return Json(new { Status = 1, Content = "创建题库成功" });
            else
            {
                return Json(new { Status = 0, Content = "创建题库失败" });
            }
        }

        public ActionResult UploadQuestionXls(HttpPostedFileBase file,int sortid)
        {
            var uid = 1;
            var fileName = file.FileName;
            if(!fileName.Contains(".xls")) return Json(new { Status = 0, Content = "文件格式有误" });
            var filePath = Server.MapPath(string.Format("~/{0}","UploadXls"));
            var path = Path.Combine(filePath, DateTime.Now.ToString("yyyyMMddHHmmss") + fileName);
            path = path.Replace(".xlsx", ".xls");
            file.SaveAs(path);
            Thread.Sleep(100);
            var questions = QuestionHelp.GetQuestionsFromXls(path, uid);
            int addResult = QuestionHelp.AddQuestion(sortid, questions);
            return Json(new { Status = addResult, Content = addResult == 1 ? "添加成功" : "添加失败" });
        }

        #endregion
    }

    
}