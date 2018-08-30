using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nebula.Models
{
    public class CurrentQuarterData
    {
        public CurrentQuarterData()
        {

        }

        public static CurrentQuarterData GetCurrentQuart()
        {
            var ret = new CurrentQuarterData();

            var now = DateTime.Now;
            var currentyear = now.ToString("yyyy");

            var qs1 = DateTime.Parse(currentyear + "-05-01 00:00:00");
            var qe1 = DateTime.Parse(currentyear + "-07-31 00:00:00");
           
            var qs2 = DateTime.Parse(currentyear + "-07-31 00:00:00");
            var qe2 = DateTime.Parse(currentyear + "-10-30 00:00:00");

            var qs4 = DateTime.Parse(currentyear + "-01-29 00:00:00");
            var qe4 = DateTime.Parse(currentyear + "-05-01 00:00:00");

            if (now >= qs1 && now < qe1)
            {
                ret.QuarterStart = qs1;
                ret.Weeks = (now - ret.QuarterStart).Days / 7 + 1;
                ret.QuarterStr = (Convert.ToInt32(currentyear)+1).ToString() + "_Q1";
            }
            else if (now >= qs2 && now < qe2)
            {
                ret.QuarterStart = qs2;
                ret.Weeks = (now - ret.QuarterStart).Days / 7 + 1;
                ret.QuarterStr = (Convert.ToInt32(currentyear) + 1).ToString() + "_Q2";
            }
            else if (now >= qs4 && now < qe4)
            {
                ret.QuarterStart = qs4;
                ret.Weeks = (now - ret.QuarterStart).Days / 7 + 1;
                ret.QuarterStr = currentyear + "_Q4";
            }
            else
            {
                if (now.Month == 1)
                {
                    ret.QuarterStart = DateTime.Parse((Convert.ToInt32(currentyear) - 1).ToString() + "-10-30 00:00:00");
                    ret.Weeks = (now - ret.QuarterStart).Days / 7 + 1;
                    ret.QuarterStr = currentyear + "_Q3";
                }
                else
                {
                    ret.QuarterStart = DateTime.Parse(currentyear + "-10-30 00:00:00");
                    ret.Weeks = (now - ret.QuarterStart).Days / 7 + 1;
                    ret.QuarterStr = (Convert.ToInt32(currentyear) + 1).ToString() + "_Q3";
                }
            }
            return ret;
        }

        public int Weeks { set; get; }
        public string QuarterStr { set; get; }
        public DateTime QuarterStart { set; get; }
    }


    public class OQMWarning
    {
        private static Dictionary<string, int> ProductOQMPlan(Controller ctrl)
        {
            var ret = new Dictionary<string, int>();

            var syscfg = CfgUtility.GetSysConfig(ctrl);
            var descfile = ERPVM.DownloadERPFile(syscfg["OQMWARNING"], ctrl);
            if (descfile != null && NebulaDataCollector.FileExist(ctrl, descfile))
            {
                var data = NebulaDataCollector.RetrieveDataFromExcel(ctrl, descfile, null);
                foreach (var line in data)
                {
                    if (!string.IsNullOrEmpty(line[0]) && !string.IsNullOrEmpty(line[1]))
                    {
                        try
                        {
                            if (!ret.ContainsKey(line[0].Trim().ToUpper()))
                            {
                                ret.Add(line[0].Trim().ToUpper(), Convert.ToInt32(line[1]));
                            }
                        }
                        catch (Exception ex) { }
                    }//end if
                }//end foreach
            }

            return ret;
        }


        private static Dictionary<string, int> ActualOQM()
        {
            var ret = new Dictionary<string, int>();
            var currentquart = CurrentQuarterData.GetCurrentQuart();
            var sql = "select JONumber,StartQuantity from JOBaseInfo where DateReleased > '<starttime>' and BRKey = 'OQM'";
            sql = sql.Replace("<starttime>", currentquart.QuarterStart.ToString("yyyy-MM-dd HH:mm:ss"));
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                try
                {
                    var oqm = Convert.ToString(line[0]).Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToUpper();
                    var qty = Convert.ToInt32(line[1]);
                    if (ret.ContainsKey(oqm))
                    {
                        ret[oqm] = ret[oqm] + qty;
                    }
                    else
                    {
                        ret.Add(oqm, qty);
                    }
                }
                catch (Exception ex) { }
            }//end foreach

            return ret;
        }

        public static void SendOQMWarning(Controller ctrl)
        {
            var syscfg = CfgUtility.GetSysConfig(ctrl);
            var currentquart = CurrentQuarterData.GetCurrentQuart();

            var planningdict = ProductOQMPlan(ctrl);
            var newplanningdict = new Dictionary<string, int>();
            foreach (var kv in planningdict)
            {
                newplanningdict.Add(kv.Key, Convert.ToInt32(((double)currentquart.Weeks / 13.0) * (double)kv.Value));
            }

            var actualdict = ActualOQM();

            var warninglist = new List<List<string>>();
            foreach (var kv in newplanningdict)
            {
                if(actualdict.ContainsKey(kv.Key))
                {
                    if (actualdict[kv.Key] < kv.Value)
                    {
                        var templist = new List<string>();
                        templist.Add(kv.Key);
                        templist.Add(kv.Value.ToString());
                        templist.Add(actualdict[kv.Key].ToString());
                        templist.Add("FQ: "+ currentquart.QuarterStr + " WK:" + currentquart.Weeks.ToString());
                        warninglist.Add(templist);
                    }
                }
                else
                {
                    var templist = new List<string>();
                    templist.Add(kv.Key);
                    templist.Add(kv.Value.ToString());
                    templist.Add("0");
                    templist.Add("FQ: " + currentquart.QuarterStr + " WK:" + currentquart.Weeks.ToString());
                    warninglist.Add(templist);
                }
            }

            if (warninglist.Count > 0)
            {
                var tolist = syscfg["OQMWARNLIST"].Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                var templist = new List<string>();
                templist.Add("Product Family");
                templist.Add("Planning Count");
                templist.Add("Actual Count");
                templist.Add("Date");
                warninglist.Insert(0, templist);

                var tablehtml = EmailUtility.CreateTableStr(warninglist);
                var tablelist = new List<string>();
                tablelist.Add(tablehtml);
                var content = EmailUtility.CreateTableHtml2("Hi PQEs", "Below is the OQM JO Module Checking Warning:", "", tablelist);

                EmailUtility.SendEmail(ctrl, "OQM JO Module Checking Warning", tolist, content, true);
                new System.Threading.ManualResetEvent(false).WaitOne(500);
            }
        }


    }


}