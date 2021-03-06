﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nebula.Models
{
    public class FAFJoVM
    {
        public FAFJoVM()
        {
            PN = "";
            PNDes = "";
            JO = "";
            SN = "";
            WorkFlowStep = "";
            ArriveTime = "1982-05-06 10:00:00";
        }

        public FAFJoVM(string pn,string des,string jo,string sn,string wf,string at)
        {
            PN = pn;
            PNDes = des;
            JO = jo;
            SN = sn;
            WorkFlowStep = wf;
            ArriveTime = at;
        }

        public static List<FAFJoVM> RetrieveAllFAFJO()
        {
            var ret = new List<FAFJoVM>();
            var sql = "select PN,PNDes,JO,SN,WorkFlowStep,ArriveTime,PE,Checked,ConfirmPeople,ConfirmTime from FAFJoVM";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            
            foreach (var line in dbret)
            {
                var tempvm = new FAFJoVM(Convert.ToString(line[0]), Convert.ToString(line[1]), Convert.ToString(line[2])
                    , Convert.ToString(line[3]), Convert.ToString(line[4]), Convert.ToDateTime(line[5]).ToString("yyyy-MM-dd HH:mm:ss"));
                tempvm.PE = Convert.ToString(line[6]);
                tempvm.Checked = Convert.ToString(line[7]);
                tempvm.ConfirmPeople = Convert.ToString(line[8]);
                tempvm.ConfirmTime = Convert.ToString(line[9]);
                ret.Add(tempvm);
            }
            return ret;
        }

        public static List<FAFJoVM> RetrieveAllFAFJOIn3Month()
        {
            var ret = new List<FAFJoVM>();
            var sql = "select PN,PNDes,JO,SN,WorkFlowStep,ArriveTime,PE,Checked,ConfirmPeople,ConfirmTime from FAFJoVM where ArriveTime > @ArriveTime  order by Checked, ArriveTime desc";
            var dict = new Dictionary<string, string>();
            dict.Add("@ArriveTime", DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd HH:mm:ss"));
            var dbret = DBUtility.ExeLocalSqlWithRes(sql, dict);

            foreach (var line in dbret)
            {
                var tempvm = new FAFJoVM(Convert.ToString(line[0]), Convert.ToString(line[1]), Convert.ToString(line[2])
                    , Convert.ToString(line[3]), Convert.ToString(line[4]), Convert.ToDateTime(line[5]).ToString("yyyy-MM-dd HH:mm:ss"));
                tempvm.PE = Convert.ToString(line[6]);
                tempvm.Checked = Convert.ToString(line[7]);
                tempvm.ConfirmPeople = Convert.ToString(line[8]);
                tempvm.ConfirmTime = Convert.ToString(line[9]);
                ret.Add(tempvm);
            }
            return ret;
        }

        public static List<FAFJoVM> RetrieveAllFAFJOByNum(string jonum)
        {
            var ret = new List<FAFJoVM>();
            var sql = "select PN,PNDes,JO,SN,WorkFlowStep,ArriveTime,PE,Checked,ConfirmPeople,ConfirmTime from FAFJoVM where JO = @JO";
            var dict = new Dictionary<string, string>();
            dict.Add("@JO", jonum);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql, dict);

            foreach (var line in dbret)
            {
                var tempvm = new FAFJoVM(Convert.ToString(line[0]), Convert.ToString(line[1]), Convert.ToString(line[2])
                    , Convert.ToString(line[3]), Convert.ToString(line[4]), Convert.ToDateTime(line[5]).ToString("yyyy-MM-dd HH:mm:ss"));
                tempvm.PE = Convert.ToString(line[6]);
                tempvm.Checked = Convert.ToString(line[7]);
                tempvm.ConfirmPeople = Convert.ToString(line[8]);
                tempvm.ConfirmTime = Convert.ToString(line[9]);
                ret.Add(tempvm);
            }
            return ret;
        }

        public static Dictionary<string, bool> RetrieveAllSolvedFAFJODict()
        {
            var dict = new Dictionary<string, bool>();
            var allfa = RetrieveAllFAFJO();
            foreach (var item in allfa)
            {
                if (!dict.ContainsKey(item.JO))
                {
                    dict.Add(item.JO, true);
                }
            }
            return dict;
        }

        public static void StoreFAFJO(string pn, string des, string jo, string sn, string wf, string at,string pe)
        {
            var Checked = "FALSE";
            //if (string.IsNullOrEmpty(pe))
            //{ Checked = "TRUE"; }

            var sql = "insert into FAFJoVM(PN,PNDes,JO,SN,WorkFlowStep,ArriveTime,PE,Checked) values(@PN,@PNDes,@JO,@SN,@WorkFlowStep,@ArriveTime,@PE,@Checked)";
            var param = new Dictionary<string, string>();
            param.Add("@PN", pn);
            param.Add("@PNDes", des);
            param.Add("@JO", jo);
            param.Add("@SN", sn);
            param.Add("@WorkFlowStep", wf);
            param.Add("@ArriveTime", at);
            param.Add("@PE", pe);
            param.Add("@Checked", Checked);
            DBUtility.ExeLocalSqlNoRes(sql, param);
        }

        public static void UpdateFAFCheckStatus(string jo,string cname)
        {
            var sql = "update FAFJoVM set Checked = 'TRUE',ConfirmPeople=@ConfirmPeople,ConfirmTime=@ConfirmTime where JO=@JO";
            var param = new Dictionary<string, string>();
            param.Add("@JO", jo);
            param.Add("@ConfirmPeople", cname);
            param.Add("@ConfirmTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            DBUtility.ExeLocalSqlNoRes(sql, param);
        }

        public string PN { set; get; }
        public string PNDes { set; get; }
        public string JO { set; get; }
        public string SN { set; get; }
        public string WorkFlowStep { set; get; }
        public string ArriveTime { set; get; }
        public string PE { set; get; }
        public string Checked { set; get; }
        public string ConfirmPeople { set; get; }
        public string ConfirmTime { set; get; }
    }
}