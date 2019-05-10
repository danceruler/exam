using FangPage.Common;
using FP_entity;
using FP_wab.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace FP_wab.Help
{
    public class QuestionHelp
    {
        private static FP_ExamEntities db = new FP_ExamEntities();
        public static string specialSym = "§";
        /// <summary>
        /// 添加题库
        /// </summary>
        /// <param name="questionres"></param>
        public static int AddQuestionRes(QuestionResModel request)
        {
            try
            {
                var channalInfo = db.FP_WMS_ChannelInfo.SingleOrDefault(t => t.id == request.channalid);
                FP_WMS_SortInfo sortinfo = new FP_WMS_SortInfo();
                sortinfo.channelid = request.channalid;
                sortinfo.appid = int.Parse(channalInfo.sortapps);
                sortinfo.display = request.display;
                sortinfo.parentid = request.parentid;
                List<int> parents = new List<int>();
                int cnd = request.parentid;
                while (cnd != 0)
                {
                    parents.Add(cnd);
                    cnd = db.FP_WMS_SortInfo.SingleOrDefault(t => t.id == cnd).parentid.Value;
                }
                parents.Add(cnd);
                string parentlist = "";
                for (int i = 0; i < parents.Count(); i++)
                {
                    parentlist += parents[parents.Count() - i - 1];
                    if (i < (parents.Count() - 1)) parentlist += ",";
                }
                sortinfo.parentlist = parentlist;
                sortinfo.name = request.name;
                sortinfo.markup = request.markup;
                sortinfo.pagesize = request.pagesize;
                sortinfo.description = request.description;
                sortinfo.icon = "";
                sortinfo.attach_icon = "";
                sortinfo.img = "";
                sortinfo.attach_img = FPRandom.CreateCode(20);
                sortinfo.subcounts = 0;
                sortinfo.types = "";
                sortinfo.showtype = request.showtype;
                sortinfo.otherurl = "";
                sortinfo.posts = 0;
                db.FP_WMS_SortInfo.Add(sortinfo);
                db.SaveChanges();
                sortinfo.parentlist += "," + sortinfo.id;
                db.SaveChanges();
                foreach (QuestionType qt in Enum.GetValues(typeof(QuestionType)))
                {
                    FP_Exam_SortQuestion sortQuestion = new FP_Exam_SortQuestion();
                    sortQuestion.channelid = sortinfo.channelid;
                    sortQuestion.sortid = sortinfo.id;
                    sortQuestion.type = qt.ToString();
                    sortQuestion.typeid = 0;
                    sortQuestion.counts = 0;
                    sortQuestion.questionlist = "";
                    db.FP_Exam_SortQuestion.Add(sortQuestion);
                    db.SaveChanges();
                }
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
            
        }

        /// <summary>
        /// 添加题目
        /// </summary>
        /// <param name="sortid">题库id</param>
        /// <param name="questions">题目列表</param>
        public static int AddQuestion(int sortid, List<FP_Exam_ExamQuestion> questions)
        {
            try
            {
                var sortinfo = db.FP_WMS_SortInfo.SingleOrDefault(t => t.id == sortid);
                var nowtime = DateTime.Now;
                foreach (FP_Exam_ExamQuestion question in questions)
                {
                    question.sortid = sortid;
                    question.channelid = sortinfo.channelid;
                    question.postdatetime = nowtime;
                    db.FP_Exam_ExamQuestion.Add(question);
                    db.SaveChanges();
                    var sortquestion = db.FP_Exam_SortQuestion.SingleOrDefault(t => t.sortid == sortid & t.type == question.type);
                    sortquestion.counts += 1;
                    if (sortquestion.counts == 1) sortquestion.questionlist += question.id;
                    else sortquestion.questionlist += "," + question.id;
                    db.SaveChanges();
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
            

        }
        
        /// <summary>
        /// 删除题目，并在题目存在的题库中删除题目
        /// </summary>
        /// <param name="questionid"></param>
        public static int DeleteQuestion(int questionid)
        {
            try
            {
                var question = db.FP_Exam_ExamQuestion.SingleOrDefault(t => t.id == questionid);
                var sortq = db.FP_Exam_SortQuestion.SingleOrDefault(t => t.sortid == question.sortid & t.channelid == question.channelid & t.type == question.type);
                string[] qlist = sortq.questionlist.Split(',');
                sortq.questionlist = "";
                for (int i = 0; i < qlist.Length; i++)
                {
                    if (qlist[i] != question.id + "")
                    {
                        sortq.questionlist += qlist[i] + ",";
                    }
                }
                if (sortq.questionlist != "") sortq.questionlist = sortq.questionlist.Substring(0, sortq.questionlist.Length - 1);
                sortq.counts -= 1;
                db.FP_Exam_ExamQuestion.Remove(question);
                db.SaveChanges();
                return 1;
            }catch(Exception ex)
            {
                return 0;
            }
            
        }

        /// <summary>
        /// 将xls转为题目列表
        /// </summary>
        public static List<FP_Exam_ExamQuestion> GetQuestionsFromXls(string filepath,int uid)
        {
            DataTable dt = ExcelHelp.InputFromExcel(filepath,"sheet1");
            List<FP_Exam_ExamQuestion> result = new List<FP_Exam_ExamQuestion>();
            foreach (DataRow row in dt.Rows)
            {
                FP_Exam_ExamQuestion question = new FP_Exam_ExamQuestion();
                question.kid = Guid.NewGuid().ToString();
                question.parentid = "";
                question.uid = uid;
                question.channelid = 0;
                question.sortid = 0;
                question.typelist = "";
                question.type = row["type"].ToString();
                question.display = 1;
                question.title = row["title"].ToString();
                string content = "";
                int optionSum = 0;
                foreach(string option in ExcelHelp.optionList)
                {
                    if(row[option].ToString() != "")
                    {
                        content += row[option].ToString() + specialSym;
                        optionSum++;
                    }
                }
                if (optionSum == 0) continue;
                content = content.Substring(0, content.Length - 1);
                question.content = content;
                question.answer = row["answer"].ToString();
                question.upperflg = 0;
                question.orderflg = 0;
                question.answerkey = row["answerkey"].ToString();
                question.ascount = optionSum;
                question.explain = row["explain"].ToString();
                int difficulty = 0;
                try
                {
                    difficulty = int.Parse(row["difficulty"].ToString());
                }
                catch (Exception)
                {
                    difficulty = -1;
                }
                question.difficulty = difficulty;
                question.postdatetime = DateTime.Now;
                question.attachid = FPRandom.CreateCode(20);
                question.exams = 0;
                question.wrongs = 0;
                question.status = 1;
                result.Add(question);
            }
            return result;
        }
        
    }
    

    
}