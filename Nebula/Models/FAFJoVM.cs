using System;
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
            var sql = "select PN,PNDes,JO,SN,WorkFlowStep,ArriveTime from FAFJoVM order by ArriveTime desc";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                ret.Add(new FAFJoVM(Convert.ToString(line[0]), Convert.ToString(line[1]), Convert.ToString(line[2])
                    , Convert.ToString(line[3]), Convert.ToString(line[4]), Convert.ToDateTime(line[5]).ToString("yyyy-MM-dd HH:mm:ss")));
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

        public static void StoreFAFJO(string pn, string des, string jo, string sn, string wf, string at)
        {
            var sql = "insert into FAFJoVM(PN,PNDes,JO,SN,WorkFlowStep,ArriveTime) values(@PN,@PNDes,@JO,@SN,@WorkFlowStep,@ArriveTime)";
            var param = new Dictionary<string, string>();
            param.Add("@PN", pn);
            param.Add("@PNDes", des);
            param.Add("@JO", jo);
            param.Add("@SN", sn);
            param.Add("@WorkFlowStep", wf);
            param.Add("@ArriveTime", at);
            DBUtility.ExeLocalSqlNoRes(sql, param);
        }

        public string PN { set; get; }
        public string PNDes { set; get; }
        public string JO { set; get; }
        public string SN { set; get; }
        public string WorkFlowStep { set; get; }
        public string ArriveTime { set; get; }

    }
}