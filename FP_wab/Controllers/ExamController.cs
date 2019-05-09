using FP_entity;
using FP_wab.Filter;
using FP_wab.Help;
using FP_wab.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FP_wab.Controllers
{
    [logincheck]
    public class ExamController : Controller
    {
        private FP_ExamEntities db = new FP_ExamEntities();
        #region 视图
        /// <summary>
        /// 主界面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if(Session["FP_WAPLOGIN"] != null)
            {
                ViewBag.username = (Session["FP_WAPLOGIN"] as FP_WMS_UserInfo).username;
            }
            return View();
        }

        /// <summary>
        /// 考试列表
        /// </summary>
        /// <param name="channal">考试渠道(2模拟4正式)</param>
        /// <returns></returns>
        public ActionResult ExamList(int channal)
        {
            ViewBag.examTitle = channal == 4 ? "正式考试" : "模拟考试";
            var examlist = db.FP_Exam_ExamInfo.Where(t => t.channelid == channal).ToList();
            return View(examlist);
        }

        /// <summary>
        /// 考试历史记录界面
        /// </summary>
        /// <returns></returns>
        public ActionResult ExamHistory()
        {
            int uid = Session["FP_WAPLOGIN"]==null?-1:(Session["FP_WAPLOGIN"] as FP_WMS_UserInfo).id;
            var examhistorys = db.FP_Exam_ExamResult.Where(t => t.uid == uid).OrderByDescending(t => t.starttime).ToList();
            ViewBag.TotalSum = examhistorys.Count();
            ViewBag.PageSum = (ViewBag.TotalSum-1) / 10+1;
            ViewBag.NowPage = 1;
            return View(examhistorys);
        }

        /// <summary>
        /// 考试历史记录列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesum"></param>
        /// <returns></returns>
        public ActionResult ExamHistoryList(int page,int pagesum)
        {
            int uid = Session["FP_WAPLOGIN"] == null ? -1 : (Session["FP_WAPLOGIN"] as FP_WMS_UserInfo).id;
            var examhistorys = db.FP_Exam_ExamResult.Where(t => t.uid == uid).OrderByDescending(t => t.starttime).ToList();
            examhistorys = examhistorys.Skip((page - 1) * 10).Take(10).ToList();
            ViewBag.PageSum = pagesum;
            ViewBag.NowPage = page;
            return PartialView(examhistorys);
        }

        /// <summary>
        /// 考试信息页
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        public ActionResult StartExamView(int examid)
        {
            if (Session["FP_WAPLOGIN"] != null)
            {
                ViewBag.username = (Session["FP_WAPLOGIN"] as FP_WMS_UserInfo).username;
            }
            var exam = db.FP_Exam_ExamInfo.SingleOrDefault(t => t.id == examid);
            ViewBag.channalid = exam.channelid;
            return View(exam);

        }

        /// <summary>
        /// 考试页
        /// </summary>
        /// <param name="resultid"></param>
        /// <returns></returns>
        public ActionResult Examing(int resultid)
        {
            var examresult = db.FP_Exam_ExamResult.SingleOrDefault(t => t.id == resultid);
            var examresultopics = db.FP_Exam_ExamResultTopic.Where(t => t.resultid == resultid).ToList();
            //添加判断当前考试是否还在正常进行的条件
            //考试不存在
            if(examresult == null)
            {
                return RedirectToAction("Index", "Announce", new { content = "当前考试不存在", buttonContent = "返回首页", backUrl = ExamingHelp.domainPath + "Exam/Index", type = AnnounceType.FAIL });
            }
            //考试不属于当前用户
            var user = Session["FP_WAPLOGIN"] as FP_WMS_UserInfo;
            if (examresult.uid != user.id)
            {
                return RedirectToAction("Index", "Announce", new { content = "当前考试不属于您", buttonContent = "返回首页", backUrl = ExamingHelp.domainPath + "/Exam/Index", type = AnnounceType.FAIL });
            }
            //考试结束
            if(examresult.status == 1)
            {
                return RedirectToAction("Index", "Announce", new { content = "当前考试已经结束", buttonContent = "返回首页", backUrl = ExamingHelp.domainPath + "/Exam/Index", type = AnnounceType.FAIL });
            }
            //只能在手机端进行手机端创建的考试
            if (examresult.client != "mobile")
            {
                return RedirectToAction("Index", "Announce", new { content = "请在电脑端完成考试", buttonContent = "返回首页", backUrl = ExamingHelp.domainPath + "/Exam/Index", type = AnnounceType.FAIL });
            }
            Dictionary<FP_Exam_ExamResultTopic, List<QuestionModel>> questionDic = new Dictionary<FP_Exam_ExamResultTopic, List<QuestionModel>>();
            int answeredSum = 0;

            for(int i = 0;i< examresultopics.Count(); i++)
            {
                List<QuestionModel> questions = new List<QuestionModel>();
                if (examresultopics[i].questionlist.Contains(","))
                {
                    string[] question_strs = examresultopics[i].questionlist.Split(',');
                    string[] answer_strs = null;
                    if (examresultopics[i].answerlist != "") answer_strs = examresultopics[i].answerlist.Split('§');
                    int cnd = 1;
                    foreach (string question_str in question_strs)
                    {
                        int qid = int.Parse(question_str);
                        var question = db.FP_Exam_ExamQuestion.SingleOrDefault(t => t.id == qid);
                        QuestionModel new_q = new QuestionModel();
                        new_q.SortId = cnd++;
                        new_q.Title = question.title.Replace("<br />", "");
                        List<string> options = new List<string>();
                        string[] option_strs = question.content.Split('§');
                        foreach(string value in option_strs)
                        {
                            options.Add(value);
                        }
                        new_q.options = options;
                        if (answer_strs != null && answer_strs[cnd - 2] != "")
                        {
                            new_q.now_option = ((int)answer_strs[cnd - 2][0]) - 64;
                            answeredSum++;
                        }
                        else new_q.now_option = 0;
                        new_q.Id = qid;
                        questions.Add(new_q);
                    }
                }
                questionDic.Add(examresultopics[i], questions);
            }
            examresult.client = "mobile";
            db.SaveChanges();
            ViewBag.questionDic = questionDic;
            ViewBag.examname = examresult.examname;
            ViewBag.resultid = resultid;
            ViewBag.answeredSum = answeredSum;
            ViewBag.useTime = examresult.utime;
            ViewBag.channal = examresult.channelid;
            double stime = (examresult.endtime.Value - DateTime.Now).TotalSeconds;
            int mm = (int)stime / 60;
            int ss = (int)stime - (mm * 60);
            if (mm < 0) mm = 0;
            if (ss < 0) ss = 0;
            ViewBag.sTime = (mm >= 10 ? mm+"" : ("0" + mm)) + ":" + (ss >= 10 ? ss+"" : ("0" + ss));
            return View();
        }

        /// <summary>
        /// 答案解析页
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public ActionResult AnswerAnalysis(int resultid)
        {
            var examresult = db.FP_Exam_ExamResult.SingleOrDefault(t => t.id == resultid);
            if(examresult == null) return RedirectToAction("Index", "Announce", new { content = "当前考试不存在", buttonContent = "返回首页", backUrl = ExamingHelp.domainPath + "/Exam/Index", type = AnnounceType.FAIL });
            var examresultopics = db.FP_Exam_ExamResultTopic.Where(t => t.resultid == resultid).ToList();
            var user = Session["FP_WAPLOGIN"] as FP_WMS_UserInfo;
            Dictionary<FP_Exam_ExamResultTopic, List<AnalysisQuestionModel>> questionDic = new Dictionary<FP_Exam_ExamResultTopic, List<AnalysisQuestionModel>>();
            int answeredSum = 0;
            for (int i = 0; i < examresultopics.Count(); i++)
            {
                List<AnalysisQuestionModel> questions = new List<AnalysisQuestionModel>();
                if (examresultopics[i].questionlist.Contains(","))
                {
                    string[] question_strs = examresultopics[i].questionlist.Split(',');
                    string[] answer_strs = null;
                    if (examresultopics[i].answerlist != "") answer_strs = examresultopics[i].answerlist.Split('§');
                    int cnd = 1;
                    foreach (string question_str in question_strs)
                    {
                        int qid = int.Parse(question_str);
                        var question = db.FP_Exam_ExamQuestion.SingleOrDefault(t => t.id == qid);
                        AnalysisQuestionModel new_q = new AnalysisQuestionModel();
                        new_q.SortId = cnd++;
                        new_q.Title = question.title.Replace("<br />", "");
                        List<string> options = new List<string>();
                        string[] option_strs = question.content.Split('§');
                        foreach (string value in option_strs)
                        {
                            options.Add(value);
                        }
                        new_q.options = options;
                        if (answer_strs != null && answer_strs[cnd - 2] != "")
                        {
                            new_q.now_option = ((int)answer_strs[cnd - 2][0]) - 64;
                            answeredSum++;
                        }
                        else new_q.now_option = 0;
                        new_q.Id = qid;
                        new_q.Answer = question.answer;
                        questions.Add(new_q);
                    }
                }
                questionDic.Add(examresultopics[i], questions);
            }
            ViewBag.questionDic = questionDic;
            ViewBag.examname = examresult.examname;
            ViewBag.resultid = resultid;
            ViewBag.channal = examresult.channelid;
            ViewBag.answeredSum = answeredSum;
            return View();
        }
        #endregion

        #region 接口
        /// <summary>
        /// 判断当前用户的当前考试是否有正在进行的，没有就创建考试
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        public ActionResult checkExamExist(int examid)
        {
            var userid = (Session["FP_WAPLOGIN"] as FP_WMS_UserInfo).id;
            //如果考试次数达到限制，不允许考试
            var exam = db.FP_Exam_ExamInfo.SingleOrDefault(t => t.id == examid);
            var examedresults = db.FP_Exam_ExamResult.Where(t => t.examid == examid & t.uid == userid & t.status == 1).ToList();
            if(exam.repeats>0&&examedresults.Count() >= exam.repeats) return Json(new { Status = 1, url = "/Announce/Index?content=当前考试次数限制为" + exam.repeats+ "次，您已达到上限&buttonContent=返回考试列表&type="+ AnnounceType.FAIL+ "&backUrl="+ ExamingHelp.domainPath + "/Exam/ExamList?channal="+exam.channelid });
            var examresult = db.FP_Exam_ExamResult.SingleOrDefault(t => t.uid == userid & t.examid == examid & t.status == 0);
            if (examresult == null)
            {
                var examinfo = db.FP_Exam_ExamInfo.SingleOrDefault(t => t.id == examid);
                //创建考试
                int createResult = CreateExamHelp.CreateExam(userid, examinfo);
                if (createResult > 0)
                {
                    //跳转至考试界面
                    return Json(new { Status = 1, url = "/Exam/Examing?resultid=" + createResult });

                }
                else
                {
                    //跳转至考试界面
                    return Json(new { Status = 0, url = "/Error/Index" });
                }
            }
            else
            {
                //考试还在进行中，继续考试
                return Json(new { Status = 1, url = "/Exam/Examing?resultid=" + examresult.id });

            }
        }

        /// <summary>
        /// 选择答案
        /// </summary>
        /// <param name="resultopicid"></param>
        /// <param name="answers"></param>
        /// <returns></returns>
        public ActionResult ChangeAnswer(int resultopicid,List<string> answers)
        {
            try
            {
                var examresultopic = db.FP_Exam_ExamResultTopic.SingleOrDefault(t => t.id == resultopicid);
                string[] question_strs = examresultopic.questionlist.Split(',');
                examresultopic.answerlist = "";
                for (int i = 0; i < question_strs.Length; i++)
                {
                    if (answers[i] != "") examresultopic.answerlist += answers[i];
                    if (i < (question_strs.Length - 1)) examresultopic.answerlist += "§";
                }
                db.SaveChanges();
                return Json(new { Status = 1,Content="修改成功" });
            }
            catch (Exception)
            {
                return Json(new { Status = 0, Content = "修改失败" });
            }
            
        }

        /// <summary>
        /// 提交试卷
        /// </summary>
        /// <param name="resultid"></param>
        /// <returns></returns>
        public ActionResult SubmitPapers(int resultid)
        {
            ExamingHelp examingHelp = ExamingHelp.CreateInstance();
            return Json(examingHelp.SubmitPapers(resultid));
        }
        #endregion
        public ActionResult test()
        {
            return View();
        }
    }

    public class QuestionModel
    {
        public int Id { get; set; }
        public int SortId { get; set; }
        public string Title { get; set; }
        public List<string> options { get; set; }
        public int now_option { get; set; }
    }
    public class AnalysisQuestionModel
    {
        public int Id { get; set; }
        public int SortId { get; set; }
        public string Title { get; set; }
        public List<string> options { get; set; }
        public int now_option { get; set; }
        public string Answer { get; set; }
    }
}