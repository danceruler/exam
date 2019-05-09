using FluentScheduler;
using FP_entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace FP_wab.Help
{
    public class ExamingHelp
    {
        private static FP_ExamEntities db = new FP_ExamEntities();
        public static string domainPath = ConfigurationManager.AppSettings["DomainPath"];

        private static ExamingHelp _ExamingHelp = null;

        static ExamingHelp()
        {

            _ExamingHelp = new ExamingHelp();
        }

        public static ExamingHelp CreateInstance()
        {
            return _ExamingHelp;
        }

        /// <summary>
        /// 试卷提交方法
        /// </summary>
        /// <param name="resultid"></param>
        /// <returns></returns>
        public SubmitPaperModel SubmitPapers(int resultid)
        {
            try
            {
                var examresult = db.FP_Exam_ExamResult.SingleOrDefault(t => t.id == resultid);
                var examresultopics = db.FP_Exam_ExamResultTopic.Where(t => t.resultid == resultid).ToList();
                int finalscore = 0;
                int finalwrong_sum = 0;
                foreach (var examresultopic in examresultopics)
                {
                    int score = 0;
                    int wrongsum = 0;
                    if(examresultopic.questionlist == "")
                    {
                        continue;
                    }
                    string[] questions = examresultopic.questionlist.Split(',');
                    string[] answers = examresultopic.answerlist.Split('§');
                    for (int i = 0; i < questions.Length; i++)
                    {
                        int q_id = int.Parse(questions[i]);
                        var question = db.FP_Exam_ExamQuestion.SingleOrDefault(t => t.id == q_id);
                        if (answers.Length < (i + 1))
                        {
                            examresultopic.correctlist += "0";
                            examresultopic.scorelist += "0";
                            question.wrongs += 1;
                            wrongsum++;
                        }
                        else
                        {
                            if (answers[i] == question.answer)
                            {
                                examresultopic.correctlist += "1";
                                examresultopic.scorelist += examresultopic.perscore;
                                score += (int)examresultopic.perscore.Value;
                            }
                            else
                            {
                                examresultopic.correctlist += "0";
                                examresultopic.scorelist += "0";
                                question.wrongs += 1;
                                wrongsum++;
                            }
                        }
                        if (i < (questions.Length - 1))
                        {
                            examresultopic.correctlist += "|";
                            examresultopic.scorelist += "|";
                        }
                        //题目考试次数加一
                        question.exams += 1;
                        db.SaveChanges();
                    }
                    examresultopic.score = score;
                    examresultopic.wrongs = wrongsum;
                    finalscore += score;
                    finalwrong_sum += wrongsum;
                }
                examresult.score = finalscore;
                examresult.score1 = finalscore;
                examresult.wrongs = finalwrong_sum;
                examresult.status = 1;
                db.SaveChanges();
                string backurl = domainPath + "/Announce/Index?content=提交成功，成绩为:" + finalscore + "分&buttonContent=返回首页&backUrl=" + domainPath + "/Exam/Index&type=1";
                return new SubmitPaperModel() { Status = 1, score = finalscore,backUrl = backurl};
            }
            catch (Exception e)
            {
                return new SubmitPaperModel() { Status = 0, score = 0 };
            }
        }

        /// <summary>
        /// 后台自动提交考试
        /// </summary>
        public void AutoUpdatePaperTime()
        {
            var nowtime = DateTime.Now;
            var examingResults = db.FP_Exam_ExamResult.Where(t => t.status == 0 & t.client == "mobile" & t.endtime < nowtime).ToList();
            foreach(FP_Exam_ExamResult examresult in examingResults)
            {
                SubmitPapers(examresult.id);
            }
        }
    }

    public class AutoUpdateExamTime : IJob
    {
        public void Execute()
        {
            ExamingHelp examingHelp = ExamingHelp.CreateInstance();
            examingHelp.AutoUpdatePaperTime();
        }
    }

    public class SubmitPaperModel
    {
        public int Status { get; set; }
        public int score { get; set; }
        public string backUrl { get; set; }
    }
}