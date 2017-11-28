using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nebula.Models
{
    public class PNErrorDistribute
    {
        public PNErrorDistribute()
        {
            PN = "";
            ErrAttr = "";
            Amount = 0;
            ProjectKey = "";
        }

        public string PN { set; get;}
        public string ErrAttr { set; get; }
        public int Amount { set; get; }
        public string ProjectKey { set; get; }
    }

    public class PNErrorPareto
    {
        public string Failure { set; get; }
        public int Amount { set; get; }
        public double ABPercent { set; get; }
        public double PPercent { set; get; }
    }

    public class ProjectTestData
    {
        public ProjectTestData()
        {
            ModuleSerialNum = "";
            WhichTest = "";
            ErrAbbr = "";
            ProjectKey = "";
        }

        public string ModuleSerialNum { set; get; }
        public string WhichTest { set; get; }
        public string ErrAbbr { set; get; }
        public string ProjectKey { set; get; }
    }

    public class TestYield
    {
        public string WhichTest { set; get; }
        public int InputCount { set; get; }
        public int OutputCount { set; get; }
        public double Yield
        {
            get
            {
                try
                {
                    return OutputCount / (double)(InputCount);
                }
                catch (Exception)
                { return 0.0; }
            }
        }

        public int CorrectOutputCount { set; get; }
        public double CorrectYield
        {
            get
            {
                try
                {
                    return CorrectOutputCount / (double)(InputCount);
                }
                catch (Exception)
                { return 0.0; }
            }
        }

        private Dictionary<string, bool> allsndict = new Dictionary<string, bool>();
        public Dictionary<string, bool> AllSNDict { get { return allsndict; } }


        private Dictionary<string, bool> errsndict = new Dictionary<string, bool>();
        public Dictionary<string, bool> ErrSNDict { get { return errsndict; } }

        private Dictionary<string, bool> corsndict = new Dictionary<string, bool>();
        public Dictionary<string, bool> CorSNDict { get { return errsndict; } }
    }

    public class JOBaseInfo
    {
        public JOBaseInfo()
        {
            BRKey  = "";
            BRNumber  = "";
            JONumber  = "";
            JOType  = "";
            JOStatus  = "";
            DateReleased  = DateTime.Parse("1982-05-06 10:00:00");
            PN  = "";
            PNDesc  = "";
            Category  = "";
            StartQuantity  = 0;
            MRPNetQuantity  = 0;
            QuantityCompleted  = 0;
            WIP  = 0;
            IncurredSum  = 0.0;
            IncurredMaterialSum  = 0.0;
            Planner  = "";
            CreatedBy  = "";
            ExistQty = 0;
            Originator = "";
            PNYield = "";
            ProjectKey = "";
            JORealStatus = "";
    }

        public static JOBaseInfo CreateItem(List<string> line)
        {
            var ret = new JOBaseInfo();
            ret.JONumber = line[11].ToUpper().Trim();
            ret.JOType = line[13];
            ret.JOStatus = line[14];
            ret.DateReleased = DateTime.Parse(line[17]);
            ret.PN = line[0];
            ret.PNDesc = line[1];
            ret.Category = line[3];
            ret.StartQuantity = ERPVM.Convert2Int(line[20]);
            ret.MRPNetQuantity = ERPVM.Convert2Int(line[21]);
            ret.QuantityCompleted = ERPVM.Convert2Int(line[22]);
            ret.WIP = ret.MRPNetQuantity - ret.QuantityCompleted;
            ret.IncurredSum = ERPVM.Convert2Double(line[23]);
            ret.IncurredMaterialSum = ERPVM.Convert2Double(line[24]);
            ret.Planner = line[37];
            ret.CreatedBy = line[38];
            return ret;
        }

        public void StoreInfo()
        {
            var pjkey = "";
            PNYield = RetrivePNYield(PN,out pjkey);
            ProjectKey = pjkey;

            var sql = "delete from JOBaseInfo where JONumber = '<JONumber>'";
            sql = sql.Replace("<JONumber>", JONumber);
            DBUtility.ExeLocalSqlNoRes(sql);

            sql = "insert into JOBaseInfo(BRKey,BRNumber,JONumber,JOType,JOStatus,DateReleased,PN,PNDesc,Category,StartQuantity,MRPNetQuantity,QuantityCompleted,WIP,IncurredSum,IncurredMaterialSum,Planner,CreatedBy,ExistQty,PNTestYield,JORealStatus,ProjectKey) "
                + " values('<BRKey>','<BRNumber>','<JONumber>','<JOType>','<JOStatus>','<DateReleased>','<PN>','<PNDesc>','<Category>',<StartQuantity>,<MRPNetQuantity>,<QuantityCompleted>,<WIP>,<IncurredSum>,<IncurredMaterialSum>,'<Planner>','<CreatedBy>',<ExistQty>,'<PNTestYield>','<JORealStatus>','<ProjectKey>')";
            sql = sql.Replace("<BRKey>", BRKey).Replace("<BRNumber>", BRNumber).Replace("<JONumber>", JONumber).Replace("<JOType>", JOType).Replace("<JOStatus>", JOStatus)
                .Replace("<DateReleased>", DateReleased.ToString()).Replace("<PN>", PN).Replace("<PNDesc>", PNDesc).Replace("<Category>", Category).Replace("<StartQuantity>", StartQuantity.ToString())
                .Replace("<MRPNetQuantity>", MRPNetQuantity.ToString()).Replace("<QuantityCompleted>", QuantityCompleted.ToString()).Replace("<WIP>", WIP.ToString()).Replace("<IncurredSum>", IncurredSum.ToString("0.00"))
                .Replace("<IncurredMaterialSum>", IncurredMaterialSum.ToString("0.00")).Replace("<Planner>", Planner).Replace("<CreatedBy>", CreatedBy)
                .Replace("<ExistQty>", ExistQty.ToString()).Replace("<PNTestYield>", PNYield).Replace("<JORealStatus>",BRJOSYSTEMSTATUS.OPEN).Replace("<ProjectKey>",pjkey);
            DBUtility.ExeLocalSqlNoRes(sql);

            if (!string.IsNullOrEmpty(pjkey))
            {
                BRAgileBaseInfo.UpdateBRProjectkey(BRNumber, pjkey);
            }

        }

        public static void CloseJO(string jonum)
        {
            var sql = "update JOBaseInfo set JORealStatus = '<JORealStatus>' where JONumber = '<JONumber>'";
            sql = sql.Replace("<JORealStatus>", BRJOSYSTEMSTATUS.CLOSE).Replace("<JONumber>", jonum);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static void CloseBR(string jonum)
        {
            var sql = "select BRKey from JOBaseInfo where JONumber = '<JONumber>'";
            sql = sql.Replace("<JONumber>", jonum);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                var brkey = Convert.ToString(dbret[0][0]);
                sql = "select BRKey from JOBaseInfo where BRKey = '<BRKey>' and JORealStatus = '<JORealStatus>'";
                sql = sql.Replace("<BRKey>", brkey).Replace("<JORealStatus>", BRJOSYSTEMSTATUS.OPEN);
                dbret = DBUtility.ExeLocalSqlWithRes(sql);
                if (dbret.Count == 0)
                {
                    BRAgileBaseInfo.UpdateBRStatus(brkey, BRJOSYSTEMSTATUS.CLOSE);
                }//no jo is open under current BR
            }//get brkey
        }

        public static void OpenBR(string jonum)
        {
            var sql = "select BRKey from JOBaseInfo where JONumber = '<JONumber>'";
            sql = sql.Replace("<JONumber>", jonum);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                var brkey = Convert.ToString(dbret[0][0]);
                BRAgileBaseInfo.UpdateBRStatus(brkey, BRJOSYSTEMSTATUS.OPEN);
            }//get brkey
        }

        public static List<string> RetrieveJO()
        {
            var ret = new List<string>();
                var sql = "select distinct JONumber from JOBaseInfo";
                var dbret = DBUtility.ExeLocalSqlWithRes(sql);
                foreach (var line in dbret)
                {
                    ret.Add(Convert.ToString(line[0]));
                }
            return ret;
        }

        public static List<string> RetrieveJOWithStatus(string status)
        {
            var ret = new List<string>();

                var sql = "select distinct JONumber from JOBaseInfo where JORealStatus = '<JORealStatus>'";
                sql = sql.Replace("<JORealStatus>",status);
                var dbret = DBUtility.ExeLocalSqlWithRes(sql);
                foreach (var line in dbret)
                {
                    ret.Add(Convert.ToString(line[0]));
                }
            return ret;
        }

        public static List<JOBaseInfo> RetrieveActiveJoInfoWithStatus(string reviewer,string jostatus)
        {
            var ret = new List<JOBaseInfo>();

            var sql = string.Empty;
            if (reviewer == null)
            {
                sql = "select a.Originator,j.BRNumber,JONumber,JOType,JOStatus,DateReleased,PN,PNDesc,Category,StartQuantity,MRPNetQuantity,QuantityCompleted "
                      + ",WIP,IncurredSum,IncurredMaterialSum,Planner,CreatedBy,ExistQty,j.PNTestYield,JORealStatus,j.ProjectKey from JOBaseInfo j (nolock) "
                      + "left join BRAgileBaseInfo a(nolock) on a.BRKey = j.BRKey  where JORealStatus = '<JORealStatus>'  order by JONumber";
                sql = sql.Replace("<JORealStatus>", jostatus);
            }
            else
            {
                reviewer = reviewer.Replace("@FINISAR.COM", "").Replace(".", " ");
                sql = "select a.Originator,j.BRNumber,JONumber,JOType,JOStatus,DateReleased,PN,PNDesc,Category,StartQuantity,MRPNetQuantity,QuantityCompleted "
                      + ",WIP,IncurredSum,IncurredMaterialSum,Planner,CreatedBy,ExistQty,j.PNTestYield,JORealStatus,j.ProjectKey from JOBaseInfo j (nolock) "
                      + "left join BRAgileBaseInfo a(nolock) on a.BRKey = j.BRKey where a.Originator = '<Originator>' and JORealStatus = '<JORealStatus>' order by JONumber";
                sql = sql.Replace("<Originator>", reviewer).Replace("<JORealStatus>", jostatus);
            }
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new JOBaseInfo();
                temp.Originator = Convert.ToString(line[0]);
                temp.BRNumber = Convert.ToString(line[1]);
                temp.JONumber = Convert.ToString(line[2]);
                temp.JOType = Convert.ToString(line[3]);
                temp.JOStatus = Convert.ToString(line[4]);
                temp.DateReleased = Convert.ToDateTime(line[5]);
                temp.PN = Convert.ToString(line[6]);
                temp.PNDesc = Convert.ToString(line[7]);
                temp.Category = Convert.ToString(line[8]);
                temp.StartQuantity = ERPVM.Convert2Int(line[9]);
                temp.MRPNetQuantity = ERPVM.Convert2Int(line[10]);
                temp.QuantityCompleted = ERPVM.Convert2Int(line[11]);
                temp.WIP = ERPVM.Convert2Int(line[12]);
                temp.IncurredSum = ERPVM.Convert2Double(line[13]);
                temp.IncurredMaterialSum = ERPVM.Convert2Double(line[14]);
                temp.Planner = Convert.ToString(line[15]);
                temp.CreatedBy = Convert.ToString(line[16]);
                temp.ExistQty = ERPVM.Convert2Int(line[17]);
                temp.PNYield = Convert.ToString(line[18]);
                temp.JORealStatus = Convert.ToString(line[19]);
                temp.ProjectKey = Convert.ToString(line[20]);

                if (string.IsNullOrEmpty(temp.PNYield))
                {
                    var pjkey = "";
                    temp.PNYield = RetrivePNYield(temp.PN,out pjkey);
                    temp.ProjectKey = pjkey;

                    if (!string.IsNullOrEmpty(pjkey))
                    {
                        UpdatePNYield(temp.PN, temp.PNYield);
                        UpdatePNProjectKey(temp.BRNumber, pjkey);
                        BRAgileBaseInfo.UpdateBRProjectkey(temp.BRNumber, pjkey);
                    }
                }

                ret.Add(temp);
            }

            return ret;
        }



        public static List<JOBaseInfo> RetrieveJoInfo(string JoNum)
        {
            var ret = new List<JOBaseInfo>();

            var sql = "select a.Originator,j.BRNumber,JONumber,JOType,JOStatus,DateReleased,PN,PNDesc,Category,StartQuantity,MRPNetQuantity,QuantityCompleted "
                      + ",WIP,IncurredSum,IncurredMaterialSum,Planner,CreatedBy,ExistQty,j.PNTestYield,JORealStatus,j.ProjectKey  from  JOBaseInfo j (nolock) "
                      + "left join BRAgileBaseInfo a(nolock) on a.BRKey = j.BRKey where j.JONumber like '%<JONumber>%' ";
            sql = sql.Replace("<JONumber>", JoNum);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new JOBaseInfo();
                temp.Originator = Convert.ToString(line[0]);
                temp.BRNumber = Convert.ToString(line[1]);
                temp.JONumber = Convert.ToString(line[2]);
                temp.JOType = Convert.ToString(line[3]);
                temp.JOStatus = Convert.ToString(line[4]);
                temp.DateReleased = Convert.ToDateTime(line[5]);
                temp.PN = Convert.ToString(line[6]);
                temp.PNDesc = Convert.ToString(line[7]);
                temp.Category = Convert.ToString(line[8]);
                temp.StartQuantity = ERPVM.Convert2Int(line[9]);
                temp.MRPNetQuantity = ERPVM.Convert2Int(line[10]);
                temp.QuantityCompleted = ERPVM.Convert2Int(line[11]);
                temp.WIP = ERPVM.Convert2Int(line[12]);
                temp.IncurredSum = ERPVM.Convert2Double(line[13]);
                temp.IncurredMaterialSum = ERPVM.Convert2Double(line[14]);
                temp.Planner = Convert.ToString(line[15]);
                temp.CreatedBy = Convert.ToString(line[16]);
                temp.ExistQty = ERPVM.Convert2Int(line[17]);
                temp.PNYield = Convert.ToString(line[18]);
                temp.JORealStatus = Convert.ToString(line[19]);
                temp.ProjectKey = Convert.ToString(line[20]);
                ret.Add(temp);
            }

            return ret;
        }

        public static List<string> RetrieveJoInfoInxMonth(int month)
        {
            var ret = new List<string>();
            var sql = "select JONumber from  JOBaseInfo  where DateReleased > '<DateReleased>'";
            sql = sql.Replace("<DateReleased>", DateTime.Now.AddMonths(0-month).ToString());
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                ret.Add(Convert.ToString(line[0]));
            }
            return ret;
        }

        public static List<JOBaseInfo> RetrieveJoInfoByBRNum(string BRNum)
        {
            var ret = new List<JOBaseInfo>();

            var sql = "select a.Originator,j.BRNumber,JONumber,JOType,JOStatus,DateReleased,PN,PNDesc,Category,StartQuantity,MRPNetQuantity,QuantityCompleted "
                      + ",WIP,IncurredSum,IncurredMaterialSum,Planner,CreatedBy,ExistQty,j.PNTestYield,JORealStatus,j.ProjectKey from  JOBaseInfo j (nolock) "
                      + "left join BRAgileBaseInfo a(nolock) on a.BRKey = j.BRKey where j.BRNumber = '<BRNumber>' ";
            sql = sql.Replace("<BRNumber>", BRNum);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new JOBaseInfo();
                temp.Originator = Convert.ToString(line[0]);
                temp.BRNumber = Convert.ToString(line[1]);
                temp.JONumber = Convert.ToString(line[2]);
                temp.JOType = Convert.ToString(line[3]);
                temp.JOStatus = Convert.ToString(line[4]);
                temp.DateReleased = Convert.ToDateTime(line[5]);
                temp.PN = Convert.ToString(line[6]);
                temp.PNDesc = Convert.ToString(line[7]);
                temp.Category = Convert.ToString(line[8]);
                temp.StartQuantity = ERPVM.Convert2Int(line[9]);
                temp.MRPNetQuantity = ERPVM.Convert2Int(line[10]);
                temp.QuantityCompleted = ERPVM.Convert2Int(line[11]);
                temp.WIP = ERPVM.Convert2Int(line[12]);
                temp.IncurredSum = ERPVM.Convert2Double(line[13]);
                temp.IncurredMaterialSum = ERPVM.Convert2Double(line[14]);
                temp.Planner = Convert.ToString(line[15]);
                temp.CreatedBy = Convert.ToString(line[16]);
                temp.ExistQty = ERPVM.Convert2Int(line[17]);
                temp.PNYield = Convert.ToString(line[18]);
                temp.JORealStatus = Convert.ToString(line[19]);
                temp.ProjectKey = Convert.ToString(line[20]);

                ret.Add(temp);
            }

            return ret;
        }

        private static void StorePNErrorSum(string pn, string errattr, string amount,string pjkey)
        {
            var sql = "delete from PNErrorDistribute where PN = '<PN>' and ErrAttr = '<ErrAttr>'";
            sql = sql.Replace("<PN>",pn).Replace("<ErrAttr>",errattr);
            DBUtility.ExeLocalSqlNoRes(sql);

            sql = "insert into PNErrorDistribute(PN,ErrAttr,AMount,ProjectKey) values('<PN>','<ErrAttr>',<AMount>,'<ProjectKey>')";
            sql = sql.Replace("<PN>", pn).Replace("<ErrAttr>", errattr).Replace("<AMount>", amount).Replace("<ProjectKey>",pjkey);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static List<PNErrorDistribute> RetrievePNErrorSum(string pn)
        {
            var ret = new List<PNErrorDistribute>();
            var sql = "select PN,ErrAttr,AMount,ProjectKey from PNErrorDistribute where PN = '<PN>'";
            sql = sql.Replace("<PN>", pn);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new PNErrorDistribute();
                temp.PN = Convert.ToString(line[0]);
                temp.ErrAttr = Convert.ToString(line[1]);
                temp.Amount = Convert.ToInt32(line[2]);
                temp.ProjectKey = Convert.ToString(line[3]);
                ret.Add(temp);
            }
            return ret;
        }

        private static string RetrieveCummYield(List<ProjectTestData> plist,string PN)
        {
            var perrdict = new Dictionary<string, int>();

            var yielddict = new Dictionary<string, TestYield>();
            var sndict = new Dictionary<string, bool>();
            foreach (var p in plist)
            {
                if (!sndict.ContainsKey(p.WhichTest + ":" + p.ModuleSerialNum))
                {
                    sndict.Add(p.WhichTest + ":" + p.ModuleSerialNum, true);

                    if (string.Compare(p.ErrAbbr, "PASS", true) != 0)
                    {
                        if (perrdict.ContainsKey(p.ErrAbbr))
                        {
                            perrdict[p.ErrAbbr] = perrdict[p.ErrAbbr] + 1;
                        }
                        else
                        {
                            perrdict.Add(p.ErrAbbr, 1);
                        }
                    }

                    if (yielddict.ContainsKey(p.WhichTest))
                    {
                        yielddict[p.WhichTest].InputCount = yielddict[p.WhichTest].InputCount + 1;
                        if (string.Compare(p.ErrAbbr, "PASS", true) == 0)
                            yielddict[p.WhichTest].OutputCount = yielddict[p.WhichTest].OutputCount + 1;
                    }
                    else
                    {
                        var tempyield = new TestYield();
                        tempyield.InputCount = 1;
                        if (string.Compare(p.ErrAbbr, "PASS", true) == 0)
                            tempyield.OutputCount = 1;
                        else
                            tempyield.OutputCount = 0;
                        tempyield.WhichTest = p.WhichTest;

                        yielddict.Add(p.WhichTest, tempyield);
                    }
                }
            }

            foreach (var kv in perrdict)
            {
                StorePNErrorSum(PN, kv.Key, kv.Value.ToString(), plist[0].ProjectKey);
            }

            var retyield = 1.0;
            foreach (var y in yielddict)
            {
                retyield = retyield * y.Value.Yield;
            }
            return retyield.ToString("0.000");
        }


        public static string RetrivePNYield(string PN,out string pjkey)
        {
            var pjdatalist = new List<ProjectTestData>();
            var sql = "select ModuleSerialNum,WhichTest,ErrAbbr,ProjectKey from ProjectTestData where PN = '<PN>'  order by ModuleSerialNum,TestTimeStamp DESC";
            sql = sql.Replace("<PN>", PN);
            var dbret = DBUtility.ExeTraceSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempdata = new ProjectTestData();
                tempdata.ModuleSerialNum = Convert.ToString(line[0]);
                tempdata.WhichTest = Convert.ToString(line[1]);
                tempdata.ErrAbbr = Convert.ToString(line[2]);
                tempdata.ProjectKey = Convert.ToString(line[3]);
                pjdatalist.Add(tempdata);
            }

            if (pjdatalist.Count == 0)
            {
                pjkey = "";
                return "";
            }
            else
            {
                pjkey = pjdatalist[0].ProjectKey;
                return RetrieveCummYield(pjdatalist,PN);
            }
        }

        public static void UpdatePNYield(string PN, string yield)
        {
            var sql = "Update JOBaseInfo set PNTestYield = '<PNTestYield>' where PN = '<PN>'";
            sql = sql.Replace("<PNTestYield>", yield).Replace("<PN>", PN);
            DBUtility.ExeLocalSqlWithRes(sql);
        }

        public static void UpdatePNProjectKey(string BRNUM, string pjkey)
        {
            var sql = "Update JOBaseInfo set ProjectKey = '<ProjectKey>' where BRNumber like '%<BRNumber>%'";
            sql = sql.Replace("<ProjectKey>", pjkey).Replace("<BRNumber>", BRNUM+"-");
            DBUtility.ExeLocalSqlWithRes(sql);
        }

        public static List<string> RetrievePNFromJoInfoWithStatus(string status)
        {
            var ret = new List<string>();
            var sql = "select distinct PN from JOBaseInfo where JORealStatus = '<JORealStatus>'";
            sql = sql.Replace("<JORealStatus>", status);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                ret.Add(Convert.ToString(line[0]));
            }
            return ret;
        }

        public string BRKey { set; get; }
        public string BRNumber { set; get; }
        public string JONumber { set; get; }
        public string JOType { set; get; }
        public string JOStatus { set; get; }
        public DateTime DateReleased { set; get; }
        public string PN { set; get; }
        public string PNDesc { set; get; }
        public string Category { set; get; }
        public int StartQuantity { set; get; }
        public int MRPNetQuantity { set; get; }
        public int QuantityCompleted { set; get; }
        public int WIP { set; get; }
        public double IncurredSum { set; get; }
        public double IncurredMaterialSum { set; get; }
        public string Planner { set; get; }
        public string CreatedBy { set; get; }
        public int ExistQty { set; get; }
        public string Originator { set; get; }

        public string JORealStatus { set; get; }

        public string ProjectKey { set; get; }
        public string PNYield { set; get; }
        public string PNYieldStr { get {
                if (string.IsNullOrEmpty(PNYield))
                {
                    return PNYield;
                }
                else
                {
                    return (Convert.ToDouble(PNYield)*100.0).ToString("0.0") + " %";
                }
            } }
    }

    public class JOComponentInfo
    {

        public JOComponentInfo()
        {
            BRKey = "";
            BRNumber = "";
            JONumber = "";
            MPN = "";
            MPNDesc = "";
            ReqdSum = 0;
            ItemCost = 0.0;
            QtyIssuedSum = 0;
            QtyOpen = 0;
            CostSum = 0.0;
        }

        public static JOComponentInfo CreateItem(List<string> line)
        {
            var ret = new JOComponentInfo();
            ret.BRKey = "";
            ret.BRNumber = "";
            ret.JONumber = line[0];
            ret.MPN = line[12];
            ret.MPNDesc = line[13];
            ret.ReqdSum = ERPVM.Convert2Double(line[17]);
            ret.ItemCost = ERPVM.Convert2Double(line[21]);
            ret.QtyIssuedSum = ERPVM.Convert2Double(line[20]);
            ret.QtyOpen = ret.ReqdSum - ret.QtyIssuedSum;
            ret.CostSum = ret.QtyIssuedSum* ret.ItemCost;
            return ret;
        }

        public void StoreInfo()
        {
            var sql = "delete from JOComponentInfo where JONumber = '<JONumber>' and MPN = '<MPN>'";
            sql = sql.Replace("<JONumber>", JONumber).Replace("<MPN>",MPN);
            DBUtility.ExeLocalSqlNoRes(sql);

            sql = "insert into JOComponentInfo(BRKey,BRNumber,JONumber,MPN,MPNDesc,ReqdSum,ItemCost,QtyIssuedSum,QtyOpen,CostSum) "
                + " values('<BRKey>','<BRNumber>','<JONumber>','<MPN>','<MPNDesc>',<ReqdSum>,<ItemCost>,<QtyIssuedSum>,<QtyOpen>,<CostSum>)";
            sql = sql.Replace("<BRKey>", BRKey).Replace("<BRNumber>", BRNumber).Replace("<JONumber>", JONumber).Replace("<MPN>", MPN).Replace("<MPNDesc>", MPNDesc)
                .Replace("<ReqdSum>", ReqdSum.ToString("0.00")).Replace("<ItemCost>", ItemCost.ToString("0.00")).Replace("<QtyIssuedSum>", QtyIssuedSum.ToString("0.00"))
                .Replace("<QtyOpen>", QtyOpen.ToString("0.00")).Replace("<CostSum>", CostSum.ToString("0.00"));
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static List<JOComponentInfo> RetrieveInfo(string jonum)
        {
            var ret = new List<JOComponentInfo>();
            try
            {
                var sql = "select MPN,MPNDesc,ReqdSum,ItemCost,QtyIssuedSum,QtyOpen,CostSum from JOComponentInfo where JONumber = '<JONumber>'";
                sql = sql.Replace("<JONumber>", jonum);
                var dbret = DBUtility.ExeLocalSqlWithRes(sql);
                foreach (var line in dbret)
                {
                    var tempitem = new JOComponentInfo();
                    tempitem.JONumber = jonum;
                    tempitem.MPN = Convert.ToString(line[0]);
                    tempitem.MPNDesc = Convert.ToString(line[1]);
                    tempitem.ReqdSum = Convert.ToDouble(line[2]);
                    tempitem.ItemCost = Convert.ToDouble(line[3]);
                    tempitem.QtyIssuedSum = Convert.ToDouble(line[4]);
                    tempitem.QtyOpen = Convert.ToDouble(line[5]);
                    tempitem.CostSum = Convert.ToDouble(line[6]);
                    ret.Add(tempitem);
                }
            }
            catch (Exception ex) { return ret; }
            return ret;
        }

        public string BRKey { set; get; }
        public string BRNumber { set; get; }
        public string JONumber { set; get; }
        public string MPN {set;get;}
        public string MPNDesc { set; get; }
        public double ReqdSum { set; get; }
        public double ItemCost { set; get; }
        public double QtyIssuedSum { set; get; }
        public double QtyOpen { set; get; }
        public double CostSum { set; get; }
    }


    public class ERPVM
    {
        public static double Convert2Double(string numstr)
        {
            if (string.IsNullOrEmpty(numstr))
            {
                return 0.0;
            }
            else
            {
                try
                {
                    return Convert.ToDouble(numstr);
                }
                catch (Exception ex)
                {
                    return 0.0;
                }
            }
        }
        public static double Convert2Double(object num)
        {
            var numstr = Convert.ToString(num);
            if (string.IsNullOrEmpty(numstr))
            {
                return 0.0;
            }
            else
            {
                try
                {
                    return Convert.ToDouble(numstr);
                }
                catch (Exception ex)
                {
                    return 0.0;
                }
            }
        }
        public static int Convert2Int(string numstr)
        {
            if (string.IsNullOrEmpty(numstr))
            {
                return 0;
            }
            else
            {
                try
                {
                    return Convert.ToInt32(numstr);
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        public static int Convert2Int(object num)
        {
            var numstr = Convert.ToString(num);
            if (string.IsNullOrEmpty(numstr))
            {
                return 0;
            }
            else
            {
                try
                {
                    return Convert.ToInt32(numstr);
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        private static string DownloadERPFile(string srcfile, Controller ctrl)
        {
            try
            {
                if (File.Exists(srcfile))
                {
                    var filename = Path.GetFileName(srcfile);
                    var descfolder = ctrl.Server.MapPath("~/userfiles") + "\\docs\\ERP\\";
                    if (!Directory.Exists(descfolder))
                        Directory.CreateDirectory(descfolder);
                    var descfile = descfolder + Path.GetFileNameWithoutExtension(srcfile)+DateTime.Now.ToString("yyyy-MM-dd")+Path.GetExtension(srcfile);
                    File.Copy(srcfile, descfile, true);
                    return descfile;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static void LoadJOBaseInfo(Controller ctrl)
        {
            var joexistmainstore = LoadJOExistMainstore(ctrl);

            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            var srcfile = syscfgdict["ERPJOBASEINFO"];
            var descfile = DownloadERPFile(srcfile, ctrl);
            if (descfile != null && File.Exists(descfile))
            {
                var data = ExcelReader.RetrieveDataFromExcel(descfile, null);
                if (data.Count > 0)
                {

                    //retrieve jo from database with open status
                    var dbopeningjo = JOBaseInfo.RetrieveJOWithStatus(BRJOSYSTEMSTATUS.OPEN);

                    //retrieve wip jo from excel data
                    var wipjodict = new Dictionary<string, bool>();
                    foreach (var line in data)
                    {
                        var jobnum = line[11].ToUpper().Trim();
                        if (!string.IsNullOrEmpty(jobnum) && !wipjodict.ContainsKey(jobnum))
                        {
                            wipjodict.Add(jobnum, true);
                        }
                    }
                    //retrieve jo which not exist in wip jo dictionary, close this jo
                    var jo2beclose = new List<string>();
                    foreach (var openjo in dbopeningjo)
                    {
                        if (!wipjodict.ContainsKey(openjo))
                        {
                            jo2beclose.Add(openjo);
                        }
                    }
                    //close the jo which not in wip list
                    foreach (var jo2close in jo2beclose)
                    {
                        JOBaseInfo.CloseJO(jo2close);
                    }
                    //close the br
                    foreach (var jo2close in jo2beclose)
                    {
                        JOBaseInfo.CloseBR(jo2close);
                    }


                    //load new jo
                    var jobaseinfolist = new List<JOBaseInfo>();

                    var brdict = BRAgileBaseInfo.RetrieveAllBRDictIn3Month();
                    var brlist = brdict.Keys.ToList();
                    foreach (var line in data)
                    {
                        var jobnum = line[11];
                        foreach (var br in brlist)
                        {
                            if (jobnum.ToUpper().Contains(br.ToUpper()+"-"))
                            {
                                try
                                {
                                    var tempinfo = JOBaseInfo.CreateItem(line);
                                    tempinfo.BRKey = brdict[br];
                                    tempinfo.BRNumber = br;
                                    var jopnkey = tempinfo.JONumber + ":::" + tempinfo.PN;
                                    if (joexistmainstore.ContainsKey(jopnkey))
                                    {
                                        tempinfo.ExistQty = joexistmainstore[jopnkey];
                                    }
                                    jobaseinfolist.Add(tempinfo);
                                    break;
                                }
                                catch (Exception ex) { }
                            }
                        }//end foreach
                    }//end foreach

                    foreach (var item in jobaseinfolist)
                    {
                        item.StoreInfo();
                    }

                    foreach (var item in jobaseinfolist)
                    {
                        JOBaseInfo.OpenBR(item.JONumber);
                    }
                }//end if
            }//end if
        }

        public static void LoadJOComponentInfo(Controller ctrl)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            var srcfile = syscfgdict["ERPJOCOMPONENTINFO"];
            var descfile = DownloadERPFile(srcfile, ctrl);
            if (descfile != null && File.Exists(descfile))
            {
                var data = ExcelReader.RetrieveDataFromExcel(descfile, null);
                if (data.Count > 0)
                {
                    var jocomponentlist = new List<JOComponentInfo>();

                    var jolist = JOBaseInfo.RetrieveJOWithStatus(BRJOSYSTEMSTATUS.OPEN);
                    var jodict = new Dictionary<string, bool>();
                    foreach (var jo in jolist)
                    {
                        if (!jodict.ContainsKey(jo.ToUpper().Trim()))
                        {
                            jodict.Add(jo.ToUpper().Trim(), true);
                        }
                    }

                    //var brdict = BRAgileBaseInfo.RetrieveAllBRDictIn3Month();
                    //var brlist = brdict.Keys.ToList();
                    foreach (var line in data)
                    {
                        var jobnum = line[0].ToUpper().Trim();

                            if (jodict.ContainsKey(jobnum))
                            {
                                var tempinfo = JOComponentInfo.CreateItem(line);
                                //tempinfo.BRKey = brdict[br];
                                //tempinfo.BRNumber = br;
                                jocomponentlist.Add(tempinfo);
                            }
                    }//end foreach

                    foreach (var item in jocomponentlist)
                    {
                        item.StoreInfo();
                    }
                }//end if
            }
        }

        private static Dictionary<string,int> LoadJOExistMainstore(Controller ctrl)
        {
            var ret = new Dictionary<string, int>();
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            var srcfile = syscfgdict["ERPJOEXISTMAINSTORE"];
            var descfile = DownloadERPFile(srcfile, ctrl);
            if (descfile != null && File.Exists(descfile))
            {
                var data = ExcelReader.RetrieveDataFromExcel(descfile, null);
                if (data.Count > 0)
                {
                    var brdict = BRAgileBaseInfo.RetrieveAllBRDictIn3Month();
                    var brlist = brdict.Keys.ToList();

                    foreach (var line in data)
                    {
                        var jonum = string.Empty;
                        if (!string.IsNullOrEmpty(line[10]))
                        {
                            jonum = line[10];
                        }
                        else if (!string.IsNullOrEmpty(line[11]))
                        {
                            jonum = line[11];
                        }
                        if (!string.IsNullOrEmpty(jonum))
                        {
                            foreach (var br in brlist)
                            {
                                if (jonum.ToUpper().Contains(br.ToUpper()+"-"))
                                {
                                    try
                                    {
                                        var QTY = Convert.ToInt32(line[14]);
                                        var jopnkey = jonum + ":::" + line[0];
                                        if (ret.ContainsKey(jopnkey))
                                        {
                                            ret[jopnkey] = ret[jopnkey] + QTY;
                                        }
                                        else
                                        {
                                            ret.Add(jopnkey, QTY);
                                        }
                                    }
                                    catch (Exception ex) { }
                                }//end if
                            }//foreach
                        }//end if
                    }//end foreach
                }//end if
            }//end if

            return ret;
        }

        public static void LoadJOShipTraceInfo(Controller ctrl)
        {
            var datecodejodict = JOShipTrace.LoadJODateCode();
            var syscfg = CfgUtility.GetSysConfig(ctrl);
            if (syscfg.ContainsKey("SHIPPINGORDERFOLDER"))
            {
                var shippingfolder = syscfg["SHIPPINGORDERFOLDER"];
                var srcfiles = new List<string>();
                srcfiles.AddRange(Directory.EnumerateFiles(shippingfolder));

                var shipinfolist = new List<JOShipTrace>();

                foreach (var fl in srcfiles)
                {
                    var sfn = Path.GetFileName(fl).ToUpper();
                    if (sfn.Contains("SHIPPING") && sfn.Contains("WXI") && sfn.Contains("ORDERS"))
                    {
                        try
                        {
                            var dfl = DownloadERPFile(fl, ctrl);
                            if (!string.IsNullOrEmpty(dfl))
                            {
                                var data = ExcelReader.RetrieveDataFromExcel(dfl, null);
                                foreach (var line in data)
                                {
                                    try
                                    {
                                        var datecode = line[41].ToUpper();
                                        if (datecodejodict.ContainsKey(datecode))
                                        {
                                            var jo = datecodejodict[datecode];
                                            var temp = new JOShipTrace(line,jo);
                                            shipinfolist.Add(temp);
                                        }
                                    } catch (Exception ex1) { }
                                }
                            }
                        }
                        catch (Exception ex) { }
                    }//end if
                }//end foreach

                foreach (var info in shipinfolist)
                {
                    info.StoreTraceInfo();
                }
            }//end if
        }


    }
}