using System;
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

        public string PN { set; get; }
        public string WorkflowStepName { set; get; }
        public int Sequence { set; get; }
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

        public string JONumber { set; get; }
        public string ModuleSN { set; get; }
        public string WorkflowStepName { set; get; }
        public DateTime LastMoveDate { set; get; }
    }

    public class CamstarVM
    {
        public static void UpdatePNWorkflow()
        {
            var pnlist = AgileAffectItem.BRPNIn3Month();
            var pnnoworkflow = new List<string>();
            foreach (var p in pnlist)
            {
                if (!PNWorkflow.PNWorkflowExist(p))
                {
                    pnnoworkflow.Add(p);
                }
            }//end foreach

            if (pnnoworkflow.Count > 0)
            {
                var pncond = "'";
                foreach (var p in pnnoworkflow)
                {
                    pncond = pncond + p + "','";
                }
                pncond = pncond.Substring(0, pncond.Length - 2);

                var sql = "select pb.ProductName,s.WorkflowStepName,s.Sequence from InsiteDB.insite.WorkflowStep s (nolock)"
                 + " left join InsiteDB.insite.Workflow w (nolock)on s.WorkflowID = w.WorkflowID"
                 +" left join InsiteDB.insite.Product p(nolock) on w.WorkflowBaseId = p.WorkflowBaseId"
                 +" left join InsiteDB.insite.ProductBase pb(nolock) on pb.ProductBaseId = p.ProductBaseId"
                 + " where pb.ProductName in (<pncond>)"
                 + " and p.WorkflowBaseId is not null and p.WorkflowBaseId <> '0000000000000000'";
                sql = sql.Replace("<pncond>", pncond);
                var dbret = DBUtility.ExeMESSqlWithRes(sql);
                if (dbret.Count == 0)
                {

                    sql = "select pb.ProductName,s.WorkflowStepName,s.Sequence from InsiteDB.insite.WorkflowStep s (nolock) "
                     + " left join InsiteDB.insite.Product p (nolock)on s.WorkflowID = p.WorkflowID"
                     + " left join InsiteDB.insite.ProductBase pb(nolock) on pb.ProductBaseId = p.ProductBaseId"
                     + " where pb.ProductName in (<pncond>)"
                     + " and p.WorkflowId is not null and p.WorkflowId <> '0000000000000000' ";
                    sql = sql.Replace("<pncond>", pncond);
                    dbret = DBUtility.ExeMESSqlWithRes(sql);
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
            }//end if
        }


        public static void UpdateJoStatus()
        {
            var jolist = JOBaseInfo.RetrieveJOin3Month();
            foreach (var jo in jolist)
            {
                var sql = " select c.ContainerName,w.WorkflowStepName,cs.LastMoveDate from [InsiteDB].[insite].[Container] c (nolock) "
                            + " left join[InsiteDB].[insite].[MfgOrder] m(nolock) on m.MfgOrderId = c.MfgOrderId"
                            + " left join InsiteDB.insite.CurrentStatus cs (nolock) on cs.CurrentStatusId = c.CurrentStatusId"
                            + " left join InsiteDB.insite.WorkflowStep w (nolock) on w.WorkflowStepId = cs.WorkflowStepId"
                            + " where m.MfgOrderName  = '<jocond>'";
                sql = sql.Replace("<jocond>", jo);
                var jostatuslist = new List<JOSNStatus>();
                var dbret = DBUtility.ExeMESSqlWithRes(sql);
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
                    }
                    catch (Exception ex) { }
                    
                }//end foreach
                foreach (var item in jostatuslist)
                {
                    item.StoreStatus();
                }
            }//end foreach
        }


    }
}