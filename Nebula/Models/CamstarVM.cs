﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nebula.Models
{
    public class PNWorkflow
    {
        public PNWorkflow()
        {
            PN = "";
            WorkflowStepName = "";
            Sequence = 0;
        }

        public static bool PNWorkflowExist(string pn)
        {
            var sql = "select top 1 PN from PNWorkflow where PN='<PN>'";
            sql = sql.Replace("<PN>", pn);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void StoreWorkflow()
        {
            var sql = "insert into PNWorkflow(PN,WorkflowStepName,Sequence) values('<PN>','<WorkflowStepName>',<Sequence>)";
            sql = sql.Replace("<PN>", PN).Replace("<WorkflowStepName>", WorkflowStepName).Replace("<Sequence>", Sequence.ToString());
            DBUtility.ExeLocalSqlNoRes(sql);
        }


        public static List<PNWorkflow> RetrievePNWorkflow(string pn)
        {
            var workflowdict = new Dictionary<string, bool>();

            var sql = "select PN,WorkflowStepName,Sequence from PNWorkflow where PN = '<PN>' order by Sequence ASC";
            sql = sql.Replace("<PN>", pn);
            var ret = new List<PNWorkflow>();
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempflow = new PNWorkflow();
                tempflow.PN = Convert.ToString(line[0]);
                tempflow.WorkflowStepName = Convert.ToString(line[1]);
                tempflow.Sequence = Convert.ToInt32(line[2]);
                var tempkey = tempflow.WorkflowStepName.ToUpper().Replace("_","").Replace(" ","");
                if (!workflowdict.ContainsKey(tempkey))
                {
                    workflowdict.Add(tempkey, true);
                    ret.Add(tempflow);
                }
            }

            return ret;
        }

        public static Dictionary<string,bool> RetrievePNFromJoInfo()
        {
            var ret = new Dictionary<string, bool>();
            var sql = "select distinct PN from PNWorkflow";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                ret.Add(Convert.ToString(line[0]),true);
            }
            return ret;
        }

        public string PN { set; get; }
        public string WorkflowStepName { set; get; }
        public int Sequence { set; get; }
    }

    public class JOSNonStation
    {
        public JOSNonStation(string stat, int num)
        {
            Station = stat;
            Amount = num;
        }

        public static List<JOSNonStation> RetrieveJOSNStation(List<JOSNStatus> josnlist)
        {
            var dict = new Dictionary<string, int>();
            foreach (var item in josnlist)
            {
                if (dict.ContainsKey(item.WorkflowStepName))
                {
                    dict[item.WorkflowStepName] = dict[item.WorkflowStepName] + 1;
                }
                else
                {
                    dict[item.WorkflowStepName] = 1;
                }
            }

            var ret = new List<JOSNonStation>();
            foreach (var kv in dict)
            {
                var tempval = new JOSNonStation(kv.Key,kv.Value);
                ret.Add(tempval);
            }
            return ret;
        }

        public string Station { set; get; }
        public int Amount { set; get; }
    }

    public class JOSNStatus
    {
        public JOSNStatus()
        {
            JONumber = "";
            ModuleSN = "";
            WorkflowStepName = "";
            LastMoveDate = DateTime.Parse("1982-05-06 10:00:00");
        }

        public void StoreStatus()
        {
            var sql = "delete from JOSNStatus where JONumber = '<JONumber>' and ModuleSN = '<ModuleSN>'";
            sql = sql.Replace("<JONumber>", JONumber).Replace("<ModuleSN>", ModuleSN);
            DBUtility.ExeLocalSqlNoRes(sql);

            sql = "insert into JOSNStatus(JONumber,ModuleSN,WorkflowStepName,LastMoveDate) values('<JONumber>','<ModuleSN>','<WorkflowStepName>','<LastMoveDate>')";
            sql = sql.Replace("<JONumber>", JONumber).Replace("<ModuleSN>", ModuleSN)
                .Replace("<WorkflowStepName>", WorkflowStepName).Replace("<LastMoveDate>", LastMoveDate.ToString());
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static List<JOSNStatus> RetrieveJOSNStatus(string jonum)
        {
            var ret = new List<JOSNStatus>();
            var sql = "select JONumber,ModuleSN,WorkflowStepName,LastMoveDate from JOSNStatus where JONumber = '<JONumber>'";
            sql = sql.Replace("<JONumber>", jonum);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempstat = new JOSNStatus();
                tempstat.JONumber = Convert.ToString(line[0]);
                tempstat.ModuleSN = Convert.ToString(line[1]);
                tempstat.WorkflowStepName = Convert.ToString(line[2]);
                tempstat.LastMoveDate = Convert.ToDateTime(line[3]);
                ret.Add(tempstat);
            }
            return ret;
        }

        public string JONumber { set; get; }
        public string ModuleSN { set; get; }
        public string WorkflowStepName { set; get; }
        public DateTime LastMoveDate { set; get; }
    }

    public class CamstarVM
    {
        public static void UpdatePNWorkflow()
        {
            var existpn = PNWorkflow.RetrievePNFromJoInfo();
            var pnlist = JOBaseInfo.RetrievePNFromJoInfoWithStatus(BRJOSYSTEMSTATUS.OPEN);

            var pnnoworkflow = new List<string>();
            foreach (var p in pnlist)
            {
                if(!existpn.ContainsKey(p))
                {
                    pnnoworkflow.Add(p);
                }
            }//end foreach

            foreach (var p in pnnoworkflow)
            {
                var pncond = "'"+p+"'";

                var sql = "select pb.ProductName,s.WorkflowStepName,s.Sequence from InsiteDB.insite.WorkflowStep s (nolock)"
                 + " left join InsiteDB.insite.Workflow w (nolock)on s.WorkflowID = w.WorkflowID"
                 +" left join InsiteDB.insite.Product p(nolock) on w.WorkflowBaseId = p.WorkflowBaseId"
                 +" left join InsiteDB.insite.ProductBase pb(nolock) on pb.ProductBaseId = p.ProductBaseId"
                 + " where pb.ProductName in (<pncond>)"
                 + " and p.WorkflowBaseId is not null and p.WorkflowBaseId <> '0000000000000000'";
                sql = sql.Replace("<pncond>", pncond);
                var dbret = DBUtility.ExeMESSqlWithRes(sql); //DBUtility.ExeSumSqlWithRes(sql);
                if (dbret.Count == 0)
                {

                    sql = "select pb.ProductName,s.WorkflowStepName,s.Sequence from InsiteDB.insite.WorkflowStep s (nolock) "
                     + " left join InsiteDB.insite.Product p (nolock)on s.WorkflowID = p.WorkflowID"
                     + " left join InsiteDB.insite.ProductBase pb(nolock) on pb.ProductBaseId = p.ProductBaseId"
                     + " where pb.ProductName in (<pncond>)"
                     + " and p.WorkflowId is not null and p.WorkflowId <> '0000000000000000' ";
                    sql = sql.Replace("<pncond>", pncond);
                    dbret = DBUtility.ExeMESSqlWithRes(sql); //DBUtility.ExeSumSqlWithRes(sql);
                }

                var pnworkflowlist = new List<PNWorkflow>();
                foreach (var line in dbret)
                {
                    try
                    {
                        var temp= new PNWorkflow();
                        temp.PN = Convert.ToString(line[0]);
                        temp.WorkflowStepName = Convert.ToString(line[1]);
                        temp.Sequence = Convert.ToInt32(line[2]);
                        pnworkflowlist.Add(temp);
                    }
                    catch (Exception ex) { }
                }

                foreach (var item in pnworkflowlist)
                {
                    item.StoreWorkflow();
                }
            }//end for
        }


        public static void UpdateJoMESStatus()
        {
            var jolist = JOBaseInfo.RetrieveJOWithStatus(BRJOSYSTEMSTATUS.OPEN);
            foreach (var jo in jolist)
            {
                var sql = " select c.ContainerName,w.WorkflowStepName,cs.LastMoveDate from [InsiteDB].[insite].[Container] c (nolock) "
                            + " left join[InsiteDB].[insite].[MfgOrder] m(nolock) on m.MfgOrderId = c.MfgOrderId"
                            + " left join InsiteDB.insite.CurrentStatus cs (nolock) on cs.CurrentStatusId = c.CurrentStatusId"
                            + " left join InsiteDB.insite.WorkflowStep w (nolock) on w.WorkflowStepId = cs.WorkflowStepId"
                            + " where m.MfgOrderName  = '<jocond>'";

                //sql = "select ContainerName,WorkflowStepName,LastActivityDate from SummaryDB.SUM_Popular_Container where MfgOrderName = '<jocond>' and Len(ContainerName) = 7";

                sql = sql.Replace("<jocond>", jo);

                

                var jostatuslist = new List<JOSNStatus>();
                var dbret = DBUtility.ExeMESSqlWithRes(sql); //DBUtility.ExeSumSqlWithRes(sql);
                foreach (var line in dbret)
                {
                    try
                    {
                        var temp = new JOSNStatus();
                        temp.JONumber = jo;
                        temp.ModuleSN = Convert.ToString(line[0]);
                        temp.WorkflowStepName = Convert.ToString(line[1]);
                        temp.LastMoveDate = Convert.ToDateTime(line[2]);
                        jostatuslist.Add(temp);

                        JOScheduleEventDataVM.AddUpdateSchedule(temp.JONumber, temp.WorkflowStepName, temp.LastMoveDate);
                    }
                    catch (Exception ex) { }
                    
                }//end foreach
                foreach (var item in jostatuslist)
                {
                    item.StoreStatus();
                }
            }//end foreach
        }

        public static string GetSubContainerName(string pcontainer)
        {
            var sql = @"select top 1 co.ContainerName from [InsiteDB].[insite].[Container] co
                    left join [InsiteDB].[insite].[Container] c on co.ParentContainerId = c.ContainerId
                    where c.ContainerName = '<pcontainer>'";
            sql = sql.Replace("<pcontainer>", pcontainer);
            var dbret = DBUtility.ExeMESSqlWithRes(sql);
            foreach (var line in dbret)
            {
                return Convert.ToString(line[0]);
            }
            return string.Empty;
        }

        public static List<JOSNStatus> UpdateJoMESStatus(string jo)
        {
                var sql = " select c.ContainerName,w.WorkflowStepName,cs.LastMoveDate from [InsiteDB].[insite].[Container] c (nolock) "
                            + " left join[InsiteDB].[insite].[MfgOrder] m(nolock) on m.MfgOrderId = c.MfgOrderId"
                            + " left join InsiteDB.insite.CurrentStatus cs (nolock) on cs.CurrentStatusId = c.CurrentStatusId"
                            + " left join InsiteDB.insite.WorkflowStep w (nolock) on w.WorkflowStepId = cs.WorkflowStepId"
                            + " where m.MfgOrderName  = '<jocond>'";

                //sql = "select ContainerName,WorkflowStepName,LastActivityDate from SummaryDB.SUM_Popular_Container where MfgOrderName = '<jocond>' and Len(ContainerName) = 7";

                sql = sql.Replace("<jocond>", jo);

                var jostatuslist = new List<JOSNStatus>();
                var dbret = DBUtility.ExeMESSqlWithRes(sql); //DBUtility.ExeSumSqlWithRes(sql);
                foreach (var line in dbret)
                {
                    try
                    {
                        var temp = new JOSNStatus();
                        temp.JONumber = jo;
                        temp.ModuleSN = Convert.ToString(line[0]);
                        if (!string.IsNullOrEmpty(temp.ModuleSN))
                        {
                            var subsn = GetSubContainerName(temp.ModuleSN);
                            if (!string.IsNullOrEmpty(subsn))
                            {
                                temp.ModuleSN += "/" + subsn;
                            }//end if
                        }//end if

                        temp.WorkflowStepName = Convert.ToString(line[1]);
                        temp.LastMoveDate = Convert.ToDateTime(line[2]);
                        jostatuslist.Add(temp);
                    }
                    catch (Exception ex) { }

                }//end foreach

            return jostatuslist;
        }

    }
}