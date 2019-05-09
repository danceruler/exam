using FangPage.Common;
using FP_entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace FP_wab.Help
{
    public static class CreateExamHelp
    {
        private static FP_ExamEntities db = new FP_ExamEntities();

        /// <summary>
        /// 创建examresult和examresultopic
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="examInfo"></param>
        /// <returns></returns>
        public static int CreateExam(int userid, FP_Exam_ExamInfo examInfo)
        {
            Dictionary<int, List<string>> question_strs = CreateQuestion(examInfo.id);
            if (question_strs == null) return -2;
            if (question_strs.Count() == 0) return -1;
            //创建新的examresult对象
            FP_Exam_ExamResult new_exam = new FP_Exam_ExamResult();
            new_exam.uid = userid;
            new_exam.channelid = examInfo.channelid;
            new_exam.sortid = examInfo.sortid;
            new_exam.departid = null;
            new_exam.examid = examInfo.id;
            new_exam.examname = examInfo.name;
            new_exam.examtime = examInfo.examtime;
            new_exam.examtype = examInfo.examtype;
            new_exam.showanswer = 1;
            new_exam.allowdelete = 0;
            new_exam.total = examInfo.total;
            new_exam.passmark = examInfo.passmark;
            new_exam.score1 = 0;
            new_exam.score2 = 0;
            new_exam.score = 0;
            new_exam.utime = 0;
            new_exam.islimit = 0;
            new_exam.starttime = DateTime.Now;
            new_exam.endtime = new_exam.starttime.Value.AddMinutes(examInfo.examtime.Value);
            new_exam.examdatetime = new_exam.starttime;
            new_exam.credits = 0;
            int question_sum = 0;
            foreach(int key in question_strs.Keys)
            {
                question_sum += question_strs[key].Count();
            }
            new_exam.questions = question_sum;
            new_exam.wrongs = 0;
            new_exam.unanswer = 0;
            new_exam.exp = 0;
            new_exam.getcredits = 0;
            new_exam.exnote = "";
            new_exam.attachid = FPRandom.CreateCode(30);
            new_exam.status = 0;
            new_exam.paper = 1;
            new_exam.ip = GetLocalIP();
            new_exam.mac = "00-00-00-00-00-00";
            new_exam.isvideo = 0;
            new_exam.client = "mobile";
            new_exam.papertype = 0;
            new_exam.title = examInfo.name;
            new_exam.address = "";
            db.FP_Exam_ExamResult.Add(new_exam);
            db.SaveChanges();
            //创建新的examresultopic对象
            List<FP_Exam_ExamTopic> examtopics = db.FP_Exam_ExamTopic.Where(t => t.examid == new_exam.examid).ToList();
            for (int i = 0; i < examtopics.Count(); i++)
            {
                FP_Exam_ExamResultTopic new_examresultopic = new FP_Exam_ExamResultTopic();
                new_examresultopic.resultid = new_exam.id;
                new_examresultopic.type = examtopics[i].type;
                new_examresultopic.title = examtopics[i].title;
                new_examresultopic.display = examtopics[i].display;
                new_examresultopic.perscore = examtopics[i].perscore;
                new_examresultopic.score = 0;
                new_examresultopic.questions = examtopics[i].questions;
                new_examresultopic.wrongs = 0;
                List<string> questionlist = question_strs[examtopics[i].id];
                string questionlist_str = "";
                for (int j = 0; j < questionlist.Count(); j++)
                {
                    questionlist_str += questionlist[j];
                    if (j < (questionlist.Count() - 1))
                    {
                        questionlist_str += ",";
                    }
                }
                new_examresultopic.questionlist = questionlist_str;
                new_examresultopic.answerlist = "";
                new_examresultopic.scorelist = "";
                new_examresultopic.correctlist = "";
                new_examresultopic.paper = null;
                db.FP_Exam_ExamResultTopic.Add(new_examresultopic);
            }
            db.SaveChanges();
            return new_exam.id;
        }

        /// <summary>
        /// 创建该场考试的问题列表
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        public static Dictionary<int, List<string>> CreateQuestion(int examid)
        {
            var examTopics = db.FP_Exam_ExamTopic.Where(t => t.examid == examid).ToList();
            if (examTopics.Count() == 0) return null;
            Dictionary<int, List<string>> question_dic = new Dictionary<int, List<string>>();
            List<string> question_strs = new List<string>();
            foreach (FP_Exam_ExamTopic examtopic in examTopics)
            {
                question_strs.Clear();
                if (examtopic.randomsort != "")
                {
                    string[] sort_strs = examtopic.randomsort.Split(',');
                    string[] count_strs = examtopic.randomcount.Split(',');
                    for (int i = 0; i < sort_strs.Count(); i++)
                    {
                        int sortid = int.Parse(sort_strs[i].Split('_')[0]);
                        int count = int.Parse(count_strs[i]);
                        FP_Exam_SortQuestion sortQuestion = db.FP_Exam_SortQuestion.SingleOrDefault(t => t.sortid == sortid & t.type == examtopic.type);
                        string[] questionlist = sortQuestion.questionlist.Split(',');
                        List<string> randomQuestions = getRandomQuestion(questionlist, count);
                        question_strs.AddRange(randomQuestions);
                    }
                    question_strs = ListRandom(question_strs);
                }
                else
                {
                    List<FP_Exam_SortQuestion> sortQuestions = db.FP_Exam_SortQuestion.Where(t => t.type == examtopic.type & t.counts > 0).ToList();
                    List<string[]> questionlists = new List<string[]>();
                    int len = 0;
                    for (int i = 0; i < sortQuestions.Count(); i++)
                    {
                        string[] questionlist = sortQuestions[i].questionlist.Split(',');
                        questionlists.Add(questionlist);
                        len += questionlist.Length;
                    }
                    string[] waitquestions = new string[len];
                    int cnd = 0;
                    for (int i = 0; i < questionlists.Count(); i++)
                    {
                        questionlists[i].CopyTo(waitquestions, cnd);
                        cnd += questionlists[i].Length;
                    }
                    List<string> randomQuestions = getRandomQuestion(waitquestions, examtopic.questions.Value);
                    question_strs.AddRange(randomQuestions);
                }
                List<string> strs = new List<string>();
                for (int i = 0; i < question_strs.Count(); i++)
                {
                    strs.Add(question_strs[i]);
                }
                question_dic.Add(examtopic.id, strs);
            }

            return question_dic;
        }

        /// <summary>
        /// 获取随机问题排列
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<string> getRandomQuestion(string[] questions, int count)
        {
            List<string> result = new List<string>();
            string[] random_qs = ListRandom(questions);
            for (int i = 0; i < random_qs.Length; i++)
            {
                if (i < count)
                {
                    result.Add(random_qs[i]);
                }
                else break;
            }
            return result;
        }
        

        /// <summary>
        /// 随机排列数组元素
        /// </summary>
        /// <param name="myList"></param>
        /// <returns></returns>
        private static string[] ListRandom(string[] myList)
        {

            Random ran = new Random();
            int index = 0;
            string temp = "";
            for (int i = 0; i < myList.Length; i++)
            {

                index = ran.Next(0, myList.Length - 1);
                if (index != i)
                {
                    temp = myList[i];
                    myList[i] = myList[index];
                    myList[index] = temp;
                }
            }
            return myList;
        }

        /// <summary>
        /// 随机排列数组元素
        /// </summary>
        /// <param name="myList"></param>
        /// <returns></returns>
        private static List<string> ListRandom(List<string> myList)
        {

            Random ran = new Random();
            int index = 0;
            string temp = "";
            for (int i = 0; i < myList.Count(); i++)
            {

                index = ran.Next(0, myList.Count() - 1);
                if (index != i)
                {
                    temp = myList[i];
                    myList[i] = myList[index];
                    myList[index] = temp;
                }
            }
            return myList;
        }
        
        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <returns>本机IP地址</returns>
        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

    
}