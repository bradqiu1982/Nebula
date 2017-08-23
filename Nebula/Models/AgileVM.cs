using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.IO;

namespace Nebula.Models
{
    public class AgileVM
    {
        public static string PMLastUpdateTime(string pmname,string FirstTraceTime)
        {
            var ret = string.Empty;
            try
            {
                var sql = "select LatestTime from PMUpdateTime where PMName = '<PMName>'";
                sql = sql.Replace("<PMName>", pmname);
                var dbret = DBUtility.ExeLocalSqlWithRes(sql);
                if (dbret.Count > 0)
                {
                    ret = Convert.ToDateTime(dbret[0][0]).ToString("yyyy-MM-dd hh:mm:ss");
                }
            }
            catch (Exception ex)
            {
                ret = FirstTraceTime;
            }

            if (string.IsNullOrEmpty(ret))
                ret = FirstTraceTime;

            return ret;
        }

        public static void RetrieveNewBR(string AGILEURL,string LOCALSITEPORT,string SAVELOCATION,string PMNames,string FirstTraceTime)
        {
            var args = "NEWBRQUERY " + AGILEURL + " " + LOCALSITEPORT + " " + SAVELOCATION;

            var pmlist = PMNames.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pm in pmlist)
            {
                var tm = PMLastUpdateTime(pm.Trim(), FirstTraceTime);
                var pmtm = (pm.Trim() + ";;;" + tm.Trim()).Replace(" ", "###");
                args = args + " " + pmtm;
            }

            using (Process myprocess = new Process())
            {
                myprocess.StartInfo.FileName = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, @"Scripts\agiledownloadwraper\BRAgileDownload.exe").Replace("\\", "/");
                myprocess.StartInfo.Arguments = args;
                myprocess.StartInfo.CreateNoWindow = true;
                myprocess.Start();
            }
        }

        public static void UpdateExistBR(string AGILEURL, string LOCALSITEPORT, string SAVELOCATION)
        {
            var args = "UPDATEBRLIST " + AGILEURL + " " + LOCALSITEPORT + " " + SAVELOCATION;

            using (Process myprocess = new Process())
            {
                myprocess.StartInfo.FileName = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, @"Scripts\agiledownloadwraper\BRAgileDownload.exe").Replace("\\", "/");
                myprocess.StartInfo.Arguments = args;
                myprocess.StartInfo.CreateNoWindow = true;
                myprocess.Start();
            }
        }

    }
}