using FP_entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace FP_wab.Help
{
    public class QuestionHelp
    {
        private FP_ExamEntities db = new FP_ExamEntities();
        
        /// <summary>
        /// 添加题库
        /// </summary>
        /// <param name="questionres"></param>
        public int AddQuestionRes(FP_WMS_SortInfo questionres,int channalid)
        {
            try
            {
                db.FP_WMS_SortInfo.Add(questionres);
                db.SaveChanges();
                questionres.parentlist += "," + questionres.id;
                db.SaveChanges();
                foreach (QuestionType qt in Enum.GetValues(typeof(QuestionType)))
                {
                    FP_Exam_SortQuestion sortQuestion = new FP_Exam_SortQuestion();
                    sortQuestion.channelid = channalid;
                    sortQuestion.sortid = questionres.id;
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
        public int AddQuestion(int sortid, List<FP_Exam_ExamQuestion> questions)
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
            catch (Exception)
            {
                return 0;
            }
            

        }

        /// <summary>
        /// 将xls转为题目列表
        /// </summary>
        public List<FP_Exam_ExamQuestion> GetQuestionsFromXls(string filepath,int uid)
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
            }
            return result;
        }
        
    }
    public enum QuestionType
    {
        TYPE_RADIO,
        TYPE_MULTIPLE,
        TYPE_TRUE_FALSE,
        TYPE_BLANK,
        TYPE_ANSWER,
        TYPE_OPERATION
    }
}