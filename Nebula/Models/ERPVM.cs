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
        }

        public string PN { set; get;}
        public string ErrAttr { set; get; }
        public int Amount { set; get; }
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
        }

        public string ModuleSerialNum { set; get; }
        public string WhichTest { set; get; }
        public string ErrAbbr { set; get; }
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
            JORealStatus = "";
    }

        public static JOBaseInfo CreateItem(List<string> line)
        {
            var ret = new JOBaseInfo();
            ret.JONumber = line[11];
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
            PNYield = RetrivePNYield(PN);

            var sql = "delete from JOBaseInfo where JONumber = '<JONumber>'";
            sql = sql.Replace("<JONumber>", JONumber);
            DBUtility.ExeLocalSqlNoRes(sql);

            sql = "insert into JOBaseInfo(BRKey,BRNumber,JONumber,JOType,JOStatus,DateReleased,PN,PNDesc,Category,StartQuantity,MRPNetQuantity,QuantityCompleted,WIP,IncurredSum,IncurredMaterialSum,Planner,CreatedBy,ExistQty,APVal1,JORealStatus) "
                + " values('<BRKey>','<BRNumber>','<JONumber>','<JOType>','<JOStatus>','<DateReleased>','<PN>','<PNDesc>','<Category>',<StartQuantity>,<MRPNetQuantity>,<QuantityCompleted>,<WIP>,<IncurredSum>,<IncurredMaterialSum>,'<Planner>','<CreatedBy>',<ExistQty>,'<APVal1>','<JORealStatus>')";
            sql = sql.Replace("<BRKey>", BRKey).Replace("<BRNumber>", BRNumber).Replace("<JONumber>", JONumber).Replace("<JOType>", JOType).Replace("<JOStatus>", JOStatus)
                .Replace("<DateReleased>", DateReleased.ToString()).Replace("<PN>", PN).Replace("<PNDesc>", PNDesc).Replace("<Category>", Category).Replace("<StartQuantity>", StartQuantity.ToString())
                .Replace("<MRPNetQuantity>", MRPNetQuantity.ToString()).Replace("<QuantityCompleted>", QuantityCompleted.ToString()).Replace("<WIP>", WIP.ToString()).Replace("<IncurredSum>", IncurredSum.ToString("0.00"))
                .Replace("<IncurredMaterialSum>", IncurredMaterialSum.ToString("0.00")).Replace("<Planner>", Planner).Replace("<CreatedBy>", CreatedBy)
                .Replace("<ExistQty>", ExistQty.ToString()).Replace("<APVal1>",PNYield).Replace("<JORealStatus>",BRJOSYSTEMSTATUS.OPEN);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static List<string> RetrieveJOin3Month()
        {
            var ret = new List<string>();
            var brdict = BRAgileBaseInfo.RetrieveAllBRDictIn3Month();
            var brkeylist = brdict.Values.ToList();
            if (brkeylist.Count > 0)
            {
                var brcond = "'";
                foreach (var k in brkeylist)
                {
                    brcond = brcond + k + "','";
                }
                brcond = brcond.Substring(0, brcond.Length - 2);

                var sql = "select distinct JONumber from JOBaseInfo where BRKey in (<BRCOND>)";
                sql = sql.Replace("<BRCOND>", brcond);
                var dbret = DBUtility.ExeLocalSqlWithRes(sql);
                foreach (var line in dbret)
                {
                    ret.Add(Convert.ToString(line[0]));
                }
            }
            return ret;
        }

        public static List<JOBaseInfo> RetrieveActiveJoInfo(string reviewer)
        {
            var ret = new List<JOBaseInfo>();

            var sql = string.Empty;
            if (reviewer == null)
            {
                sql = "select a.Originator,j.BRNumber,JONumber,JOType,JOStatus,DateReleased,PN,PNDesc,Category,StartQuantity,MRPNetQuantity,QuantityCompleted "
                      + ",WIP,IncurredSum,IncurredMaterialSum,Planner,CreatedBy,ExistQty,j.APVal1,JORealStatus from JOBaseInfo j (nolock) "
                      + "left join BRAgileBaseInfo a(nolock) on a.BRKey = j.BRKey order by JONumber";
            }
            else
            {
                reviewer = reviewer.Replace("@FINISAR.COM", "").Replace(".", " ");
                sql = "select a.Originator,j.BRNumber,JONumber,JOType,JOStatus,DateReleased,PN,PNDesc,Category,StartQuantity,MRPNetQuantity,QuantityCompleted "
                      + ",WIP,IncurredSum,IncurredMaterialSum,Planner,CreatedBy,ExistQty,j.APVal1,JORealStatus from JOBaseInfo j (nolock) "
                      + "left join BRAgileBaseInfo a(nolock) on a.BRKey = j.BRKey where a.Originator = '<Originator>' order by JONumber";
                sql = sql.Replace("<Originator>", reviewer);
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

                if (string.IsNullOrEmpty(temp.PNYield))
                {
                    temp.PNYield = RetrivePNYield(temp.PN);
                    UpdatePNYield(temp.PN, temp.PNYield);
                }

                ret.Add(temp);
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
                      + ",WIP,IncurredSum,IncurredMaterialSum,Planner,CreatedBy,ExistQty,j.APVal1,JORealStatus from JOBaseInfo j (nolock) "
                      + "left join BRAgileBaseInfo a(nolock) on a.BRKey = j.BRKey  where JORealStatus = '<JORealStatus>'  order by JONumber";
                sql = sql.Replace("<JORealStatus>", jostatus);
            }
            else
            {
                reviewer = reviewer.Replace("@FINISAR.COM", "").Replace(".", " ");
                sql = "select a.Originator,j.BRNumber,JONumber,JOType,JOStatus,DateReleased,PN,PNDesc,Category,StartQuantity,MRPNetQuantity,QuantityCompleted "
                      + ",WIP,IncurredSum,IncurredMaterialSum,Planner,CreatedBy,ExistQty,j.APVal1,JORealStatus from JOBaseInfo j (nolock) "
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

                if (string.IsNullOrEmpty(temp.PNYield))
                {
                    temp.PNYield = RetrivePNYield(temp.PN);
                    UpdatePNYield(temp.PN, temp.PNYield);
                }

                ret.Add(temp);
            }

            return ret;
        }



        public static List<JOBaseInfo> RetrieveJoInfo(string JoNum)
        {
            var ret = new List<JOBaseInfo>();

            var sql = "select a.Originator,j.BRNumber,JONumber,JOType,JOStatus,DateReleased,PN,PNDesc,Category,StartQuantity,MRPNetQuantity,QuantityCompleted "
                      + ",WIP,IncurredSum,IncurredMaterialSum,Planner,CreatedBy,ExistQty,j.APVal1,JORealStatus  from  JOBaseInfo j (nolock) "
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

                ret.Add(temp);
            }

            return ret;
        }

        public static List<JOBaseInfo> RetrieveJoInfoByBRNum(string BRNum)
        {
            var ret = new List<JOBaseInfo>();

            var sql = "select a.Originator,j.BRNumber,JONumber,JOType,JOStatus,DateReleased,PN,PNDesc,Category,StartQuantity,MRPNetQuantity,QuantityCompleted "
                      + ",WIP,IncurredSum,IncurredMaterialSum,Planner,CreatedBy,ExistQty,j.APVal1,JORealStatus from  JOBaseInfo j (nolock) "
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

                ret.Add(temp);
            }

            return ret;
        }

        private static void StorePNErrorSum(string pn, string errattr, string amount)
        {
            var sql = "delete from PNErrorDistribute where PN = '<PN>' and ErrAttr = '<ErrAttr>'";
            sql = sql.Replace("<PN>",pn).Replace("<ErrAttr>",errattr);
            DBUtility.ExeLocalSqlNoRes(sql);

            sql = "insert into PNErrorDistribute(PN,ErrAttr,AMount) values('<PN>','<ErrAttr>',<AMount>)";
            sql = sql.Replace("<PN>", pn).Replace("<ErrAttr>", errattr).Replace("<AMount>", amount);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static List<PNErrorDistribute> RetrievePNErrorSum(string pn)
        {
            var ret = new List<PNErrorDistribute>();
            var sql = "select PN,ErrAttr,AMount from PNErrorDistribute where PN = '<PN>'";
            sql = sql.Replace("<PN>", pn);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new PNErrorDistribute();
                temp.PN = Convert.ToString(line[0]);
                temp.ErrAttr = Convert.ToString(line[1]);
                temp.Amount = Convert.ToInt32(line[2]);
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
                StorePNErrorSum(PN, kv.Key, kv.Value.ToString());
            }

            var retyield = 1.0;
            foreach (var y in yielddict)
            {
                retyield = retyield * y.Value.Yield;
            }
            return retyield.ToString("0.000");
        }


        public static string RetrivePNYield(string PN)
        {
            var pjdatalist = new List<ProjectTestData>();
            var sql = "select ModuleSerialNum,WhichTest,ErrAbbr,TestTimeStamp from ProjectTestData where PN = '<PN>'  order by ModuleSerialNum,TestTimeStamp DESC";
            sql = sql.Replace("<PN>", PN);
            var dbret = DBUtility.ExeTraceSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempdata = new ProjectTestData();
                tempdata.ModuleSerialNum = Convert.ToString(line[0]);
                tempdata.WhichTest = Convert.ToString(line[1]);
                tempdata.ErrAbbr = Convert.ToString(line[2]);
                pjdatalist.Add(tempdata);
            }

            if (pjdatalist.Count == 0)
            {
                return "";
            }
            else
            {
                return RetrieveCummYield(pjdatalist,PN);
            }
        }

        public static void UpdatePNYield(string PN, string yield)
        {
            var sql = "Update JOBaseInfo set APVal1 = '<APVal1>' where PN = '<PN>'";
            sql = sql.Replace("<APVal1>", yield).Replace("<PN>", PN);
            DBUtility.ExeLocalSqlWithRes(sql);
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
            ret.MPN = line[1];
            ret.MPNDesc = line[4];
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
                                var tempinfo = JOBaseInfo.CreateItem(line);
                                tempinfo.BRKey = brdict[br];
                                tempinfo.BRNumber = br;
                                var jopnkey = tempinfo.JONumber + ":::" + tempinfo.PN;
                                if (joexistmainstore.ContainsKey(jopnkey))
                                {
                                    tempinfo.ExistQty = joexistmainstore[jopnkey];
                                }
                                jobaseinfolist.Add(tempinfo);
                            }
                        }//end foreach
                    }//end foreach

                    foreach (var item in jobaseinfolist)
                    {
                        item.StoreInfo();
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

                    var brdict = BRAgileBaseInfo.RetrieveAllBRDictIn3Month();
                    var brlist = brdict.Keys.ToList();
                    foreach (var line in data)
                    {
                        var jobnum = line[0];
                        foreach (var br in brlist)
                        {
                            if (jobnum.ToUpper().Contains(br.ToUpper() + "-"))
                            {
                                var tempinfo = JOComponentInfo.CreateItem(line);
                                tempinfo.BRKey = brdict[br];
                                tempinfo.BRNumber = br;
                                jocomponentlist.Add(tempinfo);
                            }
                        }//end foreach
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



    }
}