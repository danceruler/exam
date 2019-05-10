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
        public ActionResult CreatePaper(PaperResModel request)
        {
            FP_Exam_ExamInfo examinfo = new FP_Exam_ExamInfo();
            examinfo.departid = null;
            examinfo.uid = request.uid;
            examinfo.channelid = request.channalid;
            examinfo.sortid = request.sortid;
            examinfo.typelist = "";
            examinfo.name = request.name;
            examinfo.type = 1;
            examinfo.total = request.total;
            examinfo.passmark = request.passmark;
            examinfo.examtime = request.examtime;
            examinfo.islimit = request.islimit?1:0;
            examinfo.starttime = request.startime;
            examinfo.endtime = request.endtime;
            examinfo.repeats = request.repeats;
            examinfo.showanswer = 1;
            examinfo.allowdelete = 0;
            examinfo.display = 0;
            examinfo.postdatetime = DateTime.Now;
            examinfo.examtype = 0;
            examinfo.examroles = "";
            //部门（暂时不设置）
            examinfo.examdeparts = "";
            examinfo.examuser = "";
            examinfo.credits = 0;
            examinfo.exams = 0;
            examinfo.score = 0;
            examinfo.views = 0;
            examinfo.questions = 0;
            examinfo.status = 1;
            examinfo.papers = 1;
            examinfo.title = examinfo.name;
            examinfo.address = "";
            examinfo.opentime = "";
            examinfo.description = request.description;
            examinfo.content = "";
            examinfo.iscopy = 0;
            examinfo.isvideo = 0;
            examinfo.iswitch = 1;
            examinfo.issign = 0;
            examinfo.client = "{\"pc\":\"1\",\"mobile\":\"1\"}";
            examinfo.papertype = 0;
            int result = CreateExamHelp.CreateExamInfo(examinfo);
            return Json(new { Status = result, Content = result == 1 ? "创建成功" : "创建失败" });
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

        /// <summary>
        /// 上传excel并从其中导入题目
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sortid"></param>
        /// <returns></returns>
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