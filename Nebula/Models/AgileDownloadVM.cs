using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.IO;

namespace Nebula.Models
{
    public class AgileDownloadVM
    {
        public static string PMLastUpdateTime(string pmname, string FirstTraceTime)
        {
            var ret = string.Empty;
            try
            {
                var sql = "select LatestTime from PMUpdateTime where UserName = '<UserName>'";
                sql = sql.Replace("<UserName>", pmname);
                var dbret = DBUtility.ExeLocalSqlWithRes(sql);
                if (dbret.Count > 0)
                {
                    ret = Convert.ToDateTime(dbret[0][0]).ToString("yyyy-MM-dd HH:mm:ss");
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

        public static void UpdatePMLastUpdateTime(string pmname, DateTime BROriginalTime)
        {
            try
            {
                var sql = "select LatestTime from PMUpdateTime where UserName = '<UserName>'";
                sql = sql.Replace("<UserName>", pmname);
                var dbret = DBUtility.ExeLocalSqlWithRes(sql);
                if (dbret.Count > 0)
                {
                    var existdate = Convert.ToDateTime(dbret[0][0]);
                    if (BROriginalTime > existdate)
                    {
                        sql = "update PMUpdateTime set LatestTime='<LatestTime>' where UserName = '<UserName>'";
                        sql = sql.Replace("<UserName>", pmname).Replace("<LatestTime>", BROriginalTime.ToString());
                        DBUtility.ExeLocalSqlNoRes(sql);
                    }
                }
                else
                {
                    sql = "insert into PMUpdateTime(UserName,LatestTime) values('<UserName>','<LatestTime>')";
                    sql = sql.Replace("<UserName>", pmname).Replace("<LatestTime>", BROriginalTime.ToString());
                    DBUtility.ExeLocalSqlNoRes(sql);
                }
            }
            catch (Exception ex)
            {}
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
            var brlist = BRAgileBaseInfo.RetrieveBRNumNeedToUpdate();
            foreach (var br in brlist)
            {
                args = args + " " + br;
            }

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