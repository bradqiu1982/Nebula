using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;

namespace Nebula.Models
{
    public class POComponentVM
    {
        public POComponentVM()
        {
            PN = "";
            PO = "";
            Rev = "";
            QTY = "";
            QTYRecieve = "";
            PromiseDate = "";
            PNDesc = "";
            PODesc = "";
            POStaus = "";
            MakeBuy = "";
            Planner = "";
        }

        public POComponentVM(string pn,string po,string rev,string qty,string qtyr,string promdate,string pndes,string podes,string postat,string buy,string pl)
        {
            PN = pn;
            PO = po;
            Rev = rev;
            QTY = qty;
            QTYRecieve = qtyr;
            PromiseDate = promdate;
            PNDesc = pndes;
            PODesc = podes;
            POStaus = postat;
            MakeBuy = buy;
            Planner = pl;
        }

        public static void LoadComponentInfo(Controller ctrl)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            var srcfile = syscfgdict["POCOMPONETINFO"];
            var descfile = ERPVM.DownloadERPFile(srcfile, ctrl);
            if (descfile != null && NebulaDataCollector.FileExist(ctrl, descfile))
            {
                var data = NebulaDataCollector.RetrieveDataFromExcel(ctrl, descfile, null);
                var polist = new List<POComponentVM>();
                foreach (var line in data)
                {
                    if (!string.IsNullOrEmpty(line[15]))
                    {
                        var PN = line[15];
                        var PO = line[3];
                        var Rev = line[16];
                        var QTY = line[32];
                        var QTYRecieve = line[36];
                        var PromiseDate = line[30];
                        var PNDesc = line[17];
                        var PODesc = line[6];
                        var POStaus = line[7];
                        var MakeBuy = line[46];
                        var Planner = line[52];
                        polist.Add(new POComponentVM(PN, PO, Rev, QTY, QTYRecieve, PromiseDate, PNDesc, PODesc, POStaus, MakeBuy, Planner));
                    }
                }//end foreach

                if (polist.Count > 0)
                {
                    var sql = "delete from POComponentVM";
                    DBUtility.ExeLocalSqlNoRes(sql);

                    var datatable = new System.Data.DataTable();
                    PropertyInfo[] properties = typeof(POComponentVM).GetProperties();
                    var i = 0;
                    for (i = 0; i < properties.Length;)
                    {
                        datatable.Columns.Add(properties[i].Name, properties[i].PropertyType);
                        i = i + 1;
                    }//end for
                    foreach (var item in polist)
                    {
                        properties = typeof(POComponentVM).GetProperties();
                        var temprow = new object[properties.Length];
                        for (i = 0; i < properties.Length;)
                        {
                            temprow[i] = properties[i].GetValue(item);
                            i = i + 1;
                        }
                        datatable.Rows.Add(temprow);
                    }//end foreach

                    DBUtility.WriteDBWithTable(datatable, "POComponentVM");
                }//end if

            }//end if
        }

        public static List<POComponentVM> RetrieveComponentInfo(List<string> pns, Dictionary<string, double> TotalDict)
        {
            var ret = new List<POComponentVM>();

            var pncond = "('";
            foreach (var p in pns)
            {
                pncond = pncond + p + "','";
            }
            pncond = pncond.Substring(0, pncond.Length - 2);
            pncond = pncond + ")";

            var sql = "select PN, PO, Rev, QTY, QTYRecieve, PromiseDate, PNDesc, PODesc, POStaus, MakeBuy, Planner from POComponentVM where PN in <pncond> order by PN";
            sql = sql.Replace("<pncond>", pncond);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new POComponentVM(Convert.ToString(line[0]), Convert.ToString(line[1]), Convert.ToString(line[2])
                    , Convert.ToString(line[3]), Convert.ToString(line[4]), Convert.ToString(line[5])
                    , Convert.ToString(line[6]), Convert.ToString(line[7]), Convert.ToString(line[8])
                    , Convert.ToString(line[9]), Convert.ToString(line[10]));
                ret.Add(temp);

                var qty = 0.0;
                try { qty = Convert.ToDouble(temp.QTY); } catch (Exception e) { }

                if (TotalDict.ContainsKey(temp.PN))
                {
                    TotalDict[temp.PN] = TotalDict[temp.PN] + qty;
                }
                else
                {
                    TotalDict.Add(temp.PN, qty);
                }
            }//end foreach

            return ret;
        }

        public string PN { set; get; }
        public string PO { set; get; }
        public string Rev { set; get; }
        public string QTY { set; get; }
        public string QTYRecieve { set; get; }
        public string PromiseDate { set; get; }
        public string PNDesc { set; get; }
        public string PODesc { set; get; }
        public string POStaus { set; get; }
        public string MakeBuy { set; get; }
        public string Planner { set; get; }
    }


    public class IQCComponentVM
    {

        public IQCComponentVM()
        {
            PN = "";
            Rev = "";
            QTY = "";
            Desc = "";
            LotNum = "";
            IQCWorkFlow = "";
            TransactionDate = "";
            Planner = "";
        }

        public IQCComponentVM(string pn, string rev, string qty, string des, string lot, string iqcwf, string transdate, string pl)
        {
            PN = pn;
            Rev = rev;
            QTY = qty;
            Desc = des;
            LotNum = lot;
            IQCWorkFlow = iqcwf;
            TransactionDate = transdate;
            Planner = pl;
        }

        public static void LoadComponentInfo(Controller ctrl)
        {
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);
            var srcfile = syscfgdict["IQCCOMPONETINFO"];
            var descfile = ERPVM.DownloadERPFile(srcfile, ctrl);
            if (descfile != null && NebulaDataCollector.FileExist(ctrl, descfile))
            {
                var data = NebulaDataCollector.RetrieveDataFromExcel(ctrl, descfile, null);
                if (data.Count > 0)
                {
                    var iqclist = new List<IQCComponentVM>();
                    foreach (var line in data)
                    {
                        if (string.Compare(line[0], "WXI", true) == 0)
                        {
                            var PN = line[19];
                            var Rev = line[20];
                            var QTY = line[23];
                            var Desc = line[24];
                            var LotNum = line[6];
                            var IQCWorkFlow = line[13];
                            var TransactionDate = line[31];
                            var Planner = line[40];
                            if (string.IsNullOrEmpty(Planner))
                            {
                                Planner = line[39];
                            }
                            iqclist.Add(new IQCComponentVM(PN, Rev, QTY, Desc, LotNum, IQCWorkFlow, TransactionDate, Planner));
                        }//check wuxi
                    }//foreach

                    if (iqclist.Count > 0)
                    {
                        var sql = "delete from IQCComponentVM";
                        DBUtility.ExeLocalSqlNoRes(sql);

                        var datatable = new System.Data.DataTable();
                        PropertyInfo[] properties = typeof(IQCComponentVM).GetProperties();
                        var i = 0;
                        for (i = 0; i < properties.Length;)
                        {
                            datatable.Columns.Add(properties[i].Name, properties[i].PropertyType);
                            i = i + 1;
                        }//end for
                        foreach (var item in iqclist)
                        {
                            properties = typeof(IQCComponentVM).GetProperties();
                            var temprow = new object[properties.Length];
                            for (i = 0; i < properties.Length;)
                            {
                                temprow[i] = properties[i].GetValue(item);
                                i = i + 1;
                            }
                            datatable.Rows.Add(temprow);
                        }//end foreach

                        DBUtility.WriteDBWithTable(datatable, "IQCComponentVM");
                    }//end if (iqclist.Count > 0)
                }//end if
            }
        }

        public static List<IQCComponentVM> RetrieveComponentInfo(List<string> pns,Dictionary<string,double> TotalDict)
        {
            var ret = new List<IQCComponentVM>();

            var pncond = "('";
            foreach (var p in pns)
            {
                pncond = pncond + p + "','";
            }
            pncond = pncond.Substring(0, pncond.Length - 2);
            pncond = pncond + ")";

            var sql = "select PN,Rev,QTY,[Desc],LotNum,IQCWorkFlow,TransactionDate,Planner from IQCComponentVM where PN in <pncond> order by PN";
            sql = sql.Replace("<pncond>", pncond);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new IQCComponentVM(Convert.ToString(line[0]), Convert.ToString(line[1]), Convert.ToString(line[2])
                    , Convert.ToString(line[3]), Convert.ToString(line[4]), Convert.ToString(line[5])
                    , Convert.ToString(line[6]), Convert.ToString(line[7]));
                ret.Add(temp);

                var qty = 0.0;
                try { qty = Convert.ToDouble(temp.QTY); } catch (Exception e) { }
                
                if (TotalDict.ContainsKey(temp.PN)){
                    TotalDict[temp.PN] = TotalDict[temp.PN] + qty;
                }
                else {
                    TotalDict.Add(temp.PN,qty);
                }
            }//end foreach
            
            return ret;
        }

        
        public string PN { set; get; }
        public string Rev { set; get; }
        public string QTY { set; get; }
        public string Desc { set; get; }
        public string LotNum { set; get; }
        public string IQCWorkFlow { set; get; }
        public string TransactionDate { set; get; }
        public string Planner { set; get; }
    }

    public class OnhandComponentVM
    {
        public OnhandComponentVM()
        {
            PN = "";
            Rev = "";
            QTY = "";
            Desc = "";
            RecieveDate = "";
            MakeBuy = "";
            LotNum = "";
            Planner = "";
            Place = "";
        }

        public OnhandComponentVM(string pn,string rev,string qty,string desc,string rvdate,string mb,string lot,string pl,string plc)
        {
            PN = pn;
            Rev = rev;
            QTY = qty;
            Desc = desc;
            RecieveDate = rvdate;
            MakeBuy = mb;
            LotNum = lot;
            Planner = pl;
            Place = plc;
        }

        public static void LoadComponentInfo(Controller ctrl)
        {
            var onhandlist = new string[] { "ONHANDCOMPONETINFO1", "ONHANDCOMPONETINFO2" };
            var syscfgdict = CfgUtility.GetSysConfig(ctrl);

            var vmlist = new List<OnhandComponentVM>();

            var componentplace = "MAINSTORE";
            foreach (var oh in onhandlist)
            {
                if (oh.Contains("MFG")) { componentplace = "MFG"; }

                var srcfile = syscfgdict[oh];
                var descfile = ERPVM.DownloadERPFile(srcfile, ctrl);
                if (descfile != null && NebulaDataCollector.FileExist(ctrl, descfile))
                {
                    var data = NebulaDataCollector.RetrieveDataFromExcel(ctrl, descfile, null);
                    if (data.Count > 0)
                    {
                        foreach (var line in data)
                        {
                            if (!string.IsNullOrEmpty(line[0]))
                            {
                                var PN = line[0];
                                var Rev = line[2];
                                var QTY = line[14];
                                var Desc = line[1];
                                var RecieveDate = line[12];
                                var MakeBuy = line[22];
                                var LotNum = line[8];
                                var Planner = line[16];
                                var Place = componentplace;
                                vmlist.Add(new OnhandComponentVM(PN,Rev,QTY,Desc,RecieveDate,MakeBuy,LotNum,Planner,Place));
                            }//end if
                        }//end foreach
                    }//end if
                }//end if
            }//end foreach

            if (vmlist.Count > 0)
            {
                var sql = "delete from OnhandComponentVM";
                DBUtility.ExeLocalSqlNoRes(sql);

                var datatable = new System.Data.DataTable();
                PropertyInfo[] properties = typeof(OnhandComponentVM).GetProperties();
                var i = 0;
                for (i = 0; i < properties.Length;)
                {
                    datatable.Columns.Add(properties[i].Name, properties[i].PropertyType);
                    i = i + 1;
                }//end for
                foreach (var item in vmlist)
                {
                    properties = typeof(OnhandComponentVM).GetProperties();
                    var temprow = new object[properties.Length];
                    for (i = 0; i < properties.Length;)
                    {
                        temprow[i] = properties[i].GetValue(item);
                        i = i + 1;
                    }
                    datatable.Rows.Add(temprow);
                }//end foreach

                DBUtility.WriteDBWithTable(datatable, "OnhandComponentVM");
            }

        }

        public static List<OnhandComponentVM> RetrieveComponentInfo(List<string> pns, Dictionary<string, double> TotalDict)
        {
            var ret = new List<OnhandComponentVM>();

            var pncond = "('";
            foreach (var p in pns)
            {
                pncond = pncond + p + "','";
            }
            pncond = pncond.Substring(0, pncond.Length - 2);
            pncond = pncond + ")";

            var sql = "select PN,Rev,QTY,[Desc],RecieveDate,MakeBuy,LotNum,Planner,Place from OnhandComponentVM where PN in <pncond> and MakeBuy <> 'Make' order by PN";
            sql = sql.Replace("<pncond>", pncond);

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new OnhandComponentVM(Convert.ToString(line[0]), Convert.ToString(line[1]), Convert.ToString(line[2])
                    , Convert.ToString(line[3]), Convert.ToString(line[4]), Convert.ToString(line[5])
                    , Convert.ToString(line[6]), Convert.ToString(line[7]), Convert.ToString(line[8]));
                ret.Add(temp);

                var qty = 0.0;
                try { qty = Convert.ToDouble(temp.QTY); } catch (Exception e) { }

                if (TotalDict.ContainsKey(temp.PN))
                {
                    TotalDict[temp.PN] = TotalDict[temp.PN] + qty;
                }
                else
                {
                    TotalDict.Add(temp.PN, qty);
                }
            }//end foreach

            return ret;
        }

        public string PN { set; get; }
        public string Rev { set; get; }
        public string QTY { set; get; }
        public string Desc { set; get; }
        public string RecieveDate { set; get; }
        public string MakeBuy { set; get; }
        public string LotNum { set; get; }
        public string Planner { set; get; }
        public string Place { set; get; }

    }

}