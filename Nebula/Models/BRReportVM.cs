using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nebula.Models
{
    public class BRReportVM
    {

        public BRReportVM()
        {
            BRNum = "";
            BRStatus = "";
            PNDesc = "";
            ProductPhase = "";
            BRDesc = "";
            Originator = "";
            ReqStartDate = "";
            ExpectedEndDate = "";
            StartQty = "";
            TotalCost = "";
            ScrapQty = "";
            SaleRevenue = "";
            PN = "";
            BuildLocation = "";
        }

        public string BRNum { set; get; }
        public string BRStatus { set; get; }
        public string PNDesc { set; get; }
        public string ProductPhase { set; get; }
        public string BRDesc { set; get; }
        public string Originator { set; get; }
        public string ReqStartDate { set; get; }
        public string ExpectedEndDate { set; get; }
        public string StartQty { set; get; }
        public string TotalCost { set; get; }
        public string ScrapQty { set; get; }
        public string SaleRevenue { set; get; }
        public string PN { set; get; }
        public string BuildLocation { set; get; }


        public static List<BRReportVM> RetrieveActiveBRRptVM(string starttime, string endtime)
        {
            var ret = new List<BRReportVM>();
            var brdict = new Dictionary<string, bool>();

            var sql = "SELECT BRNumber,status,nextstatus FROM AgileHistory where localtime > '<starttime>' and localtime < '<endtime>' and action <> 'Get file attachment' order by BRNumber,localtime desc";
            sql = sql.Replace("<starttime>", starttime).Replace("<endtime>", endtime);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var brnum = Convert.ToString(line[0]);
                if (!brdict.ContainsKey(brnum))
                {
                    brdict.Add(brnum, true);

                    var tempvm = new BRReportVM();
                    tempvm.BRNum = Convert.ToString(line[0]);
                    var status = Convert.ToString(line[1]);
                    tempvm.BRStatus = status;

                    var nstatus = Convert.ToString(line[2]);
                    if (!string.IsNullOrEmpty(nstatus))
                        tempvm.BRStatus = tempvm.BRStatus + "-->" + nstatus;
                    ret.Add(tempvm);
                }
            }//end foreach

            RetrievePNPNDesc(ret);
            RetrievePageThree(ret);
            RetrieveBRDesc(ret);

            return ret;
        }

        private static void RetrievePNPNDesc(List<BRReportVM> brlist)
        {
            foreach (var br in brlist)
            {
                var sql = "SELECT PN,itemdesc from AgileAffectItem where BRNumber = '<BRNumber>'";
                sql = sql.Replace("<BRNumber>", br.BRNum);
                var dbret = DBUtility.ExeLocalSqlWithRes(sql);
                foreach (var line in dbret)
                {
                    br.PN = Convert.ToString(line[0]);
                    br.PNDesc = Convert.ToString(line[1]);
                    break;
                }
            }
        }


        private static void RetrievePageThree(List<BRReportVM> brlist)
        {
            foreach (var br in brlist)
            {
                var sql = "SELECT startqty,totalcost,scrapqty,salerevenue,reqdjostartdate,buildlocation,productphase,pcshipdate from AgilePageThree where BRNumber = '<BRNumber>'";
                sql = sql.Replace("<BRNumber>", br.BRNum);
                var dbret = DBUtility.ExeLocalSqlWithRes(sql);
                foreach (var line in dbret)
                {
                    br.StartQty = Convert.ToString(line[0]);
                    br.TotalCost = Convert.ToString(line[1]);
                    br.ScrapQty = Convert.ToString(line[2]);
                    br.SaleRevenue = Convert.ToString(line[3]);
                    br.ReqStartDate = Convert.ToString(line[4]);
                    br.BuildLocation = Convert.ToString(line[5]);
                    br.ProductPhase = Convert.ToString(line[6]);
                    br.ExpectedEndDate = Convert.ToString(line[7]);
                    break;
                }
            }
        }

        private static void RetrieveBRDesc(List<BRReportVM> brlist)
        {
            foreach (var br in brlist)
            {
                var sql = "SELECT Description,Originator FROM  BRAgileBaseInfo  where BRNumber = '<BRNumber>'";
                sql = sql.Replace("<BRNumber>", br.BRNum);
                var dbret = DBUtility.ExeLocalSqlWithRes(sql);
                foreach (var line in dbret)
                {
                    br.BRDesc = Convert.ToString(line[0]);
                    br.Originator = Convert.ToString(line[1]);
                    break;
                }
            }
        }


        public static List<List<string>> BRReportToArray(BRReportVM br)
        {
                var temptablist = new List<List<string>>();

                var templist = new List<string>();
                templist.Add(br.BRNum);
                templist.Add(br.PNDesc);
                templist.Add("NPI Lifecycle:" + br.ProductPhase);
                temptablist.Add(templist);

                templist = new List<string>();
                templist.Add("Agile Status");
                templist.Add(br.BRStatus);
                templist.Add("");
                temptablist.Add(templist);

                templist = new List<string>();
                templist.Add("Description & Justification");
                templist.Add(br.BRDesc);
                templist.Add("");
                temptablist.Add(templist);

                templist = new List<string>();
                templist.Add("Assembly Product Families affected");
                templist.Add(br.PNDesc);
                templist.Add("");
                temptablist.Add(templist);

                templist = new List<string>();
                templist.Add("Originator");
                templist.Add(br.Originator);
                templist.Add("");
                temptablist.Add(templist);

                templist = new List<string>();
                templist.Add("Build Site");
                templist.Add(br.BuildLocation);
                templist.Add("");
                temptablist.Add(templist);

                templist = new List<string>();
                templist.Add("Requested Start Date");
                templist.Add(br.ReqStartDate);
                templist.Add("");
                temptablist.Add(templist);

                templist = new List<string>();
                templist.Add("Expected End Date");
                templist.Add(br.ExpectedEndDate);
                templist.Add("");
                temptablist.Add(templist);

                templist = new List<string>();
                templist.Add("Start QTY");
                templist.Add(br.StartQty);
                templist.Add("");
                temptablist.Add(templist);

                templist = new List<string>();
                templist.Add("Total Cost");
                templist.Add(br.TotalCost);
                templist.Add("");
                temptablist.Add(templist);

                templist = new List<string>();
                templist.Add("Scrap QTY");
                templist.Add(br.ScrapQty);
                templist.Add("");
                temptablist.Add(templist);

                templist = new List<string>();
                templist.Add("Sale Revenue");
                templist.Add(br.SaleRevenue);
                templist.Add("");
                temptablist.Add(templist);

            return temptablist;
        }

        public static List<string> RetrieveActiveBRRpt(string starttime, string endtime)
        {
            var htabelist = new List<string>();
            var brlist = RetrieveActiveBRRptVM(starttime, endtime);
            foreach (var br in brlist)
            {
                var temptablist = BRReportToArray(br);
                htabelist.Add(EmailUtility.CreateTableStr(temptablist));
            }
            return htabelist;
        }

        public static List<string> RetrievePMList()
        {
            var ret = new List<string>();
            var sql = "SELECT distinct Originator FROM  BRAgileBaseInfo order by Originator ASC";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                ret.Add(Convert.ToString(line[0]));
            }
            return ret;
        }

    }
}