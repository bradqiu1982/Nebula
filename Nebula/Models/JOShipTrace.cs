using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nebula.Models
{
    public class JOShipTrace
    {
        public JOShipTrace()
        {
            JO = "";
            SONum = "";
            CustPO = "";
            ShipCust = "";
            OrderQTY = "";
            ShippedQTY = "";
            ShipDate = "";
            DateCode = "";
            TraceNum = "";
            RealCust = "";
        }

        public JOShipTrace(List<string> line,string jo)
        {
            JO = jo;
            SONum = line[0];
            CustPO = line[3];
            ShipCust = line[6];
            OrderQTY = line[30];
            ShippedQTY = line[31];
            ShipDate = line[35];
            DateCode = line[41];
            TraceNum = line[48];
            RealCust = line[54];
        }

        public string JO { set; get; }
        public string SONum { set; get; }
        public string CustPO { set; get; }
        public string ShipCust { set; get; }
        public string OrderQTY { set; get; }
        public string ShippedQTY { set; get; }
        public string ShipDate { set; get; }
        public string DateCode { set; get; }
        public string TraceNum { set; get; }
        public string RealCust { set; get; }

        private void RemoveDup(string jo, string datecode, string shippedqty)
        {
            var sql = "delete from JOShipTrace where JO = '<JO>' and DateCode = '<DateCode>' and ShippedQTY = '<ShippedQTY>'";
            sql = sql.Replace("<JO>", jo).Replace("<DateCode>", datecode).Replace("<ShippedQTY>", shippedqty);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public void StoreTraceInfo()
        {
            RemoveDup(JO, DateCode, ShippedQTY);
            var sql = "insert into JOShipTrace(JO,SONum,CustPO,ShipCust,OrderQTY,ShippedQTY,ShipDate,DateCode,TraceNum,RealCust) values('<JO>','<SONum>','<CustPO>','<ShipCust>','<OrderQTY>','<ShippedQTY>','<ShipDate>','<DateCode>','<TraceNum>','<RealCust>')";
            sql = sql.Replace("<JO>", JO).Replace("<SONum>", SONum).Replace("<CustPO>", CustPO).Replace("<ShipCust>", ShipCust)
                .Replace("<OrderQTY>", OrderQTY).Replace("<ShippedQTY>", ShippedQTY).Replace("<ShipDate>", ShipDate)
                .Replace("<DateCode>", DateCode).Replace("<TraceNum>", TraceNum).Replace("<RealCust>", RealCust);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static List<JOShipTrace> RetireTraceInfo(string jonum)
        {
            var ret = new List<JOShipTrace>();
            var sql = "select JO,SONum,CustPO,ShipCust,OrderQTY,ShippedQTY,ShipDate,DateCode,TraceNum,RealCust from JOShipTrace where JO = '<JO>'";
            sql = sql.Replace("<JO>", jonum);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new JOShipTrace();
                temp.JO = Convert.ToString(line[0]);
                temp.SONum = Convert.ToString(line[1]);
                temp.CustPO = Convert.ToString(line[2]);
                temp.ShipCust = Convert.ToString(line[3]);
                temp.OrderQTY = Convert.ToString(line[4]);
                temp.ShippedQTY = Convert.ToString(line[5]);
                temp.ShipDate = Convert.ToString(line[6]);
                temp.DateCode = Convert.ToString(line[7]);
                temp.TraceNum = Convert.ToString(line[8]);
                temp.RealCust = Convert.ToString(line[9]);
                ret.Add(temp);
            }
            return ret;
        }

        public static Dictionary<string, string> LoadJODateCode()
        {
            var ret = new Dictionary<string, string>();
            var jolist = JOBaseInfo.RetrieveJoInfoInxMonth(6);
            if (jolist.Count > 0)
            {
                var cond = "'";
                foreach (var jo in jolist)
                {
                    cond = cond + jo + "','";
                }
                cond = cond.Substring(0, cond.Length - 2);

                //var sql = "SELECT [ContainerName],[CurrMfgOrder] FROM [PDMS].[dbo].[ActlContainer](nolock) where CurrMfgOrder in (<CurrMfgOrder>) and(ContainerName like 'F%' or  ContainerName like 'P%' or  ContainerName like 'R%') and LEN(ContainerName) > 7";
                var sql = "SELECT [ContainerName],[CurrMfgOrder] FROM [PDMS].[dbo].[ActlContainer](nolock) where CurrMfgOrder in (<CurrMfgOrder>) and LEN(ContainerName) > 7";
                sql = sql.Replace("<CurrMfgOrder>", cond);
                var dbret = DBUtility.ExeMESReportSqlWithRes(sql);
                foreach (var line in dbret)
                {
                    var lotnum = Convert.ToString(line[0]).Trim().ToUpper();
                    var jo = Convert.ToString(line[1]).Trim().ToUpper();
                    if (!ret.ContainsKey(lotnum))
                    {
                        ret.Add(lotnum, jo);
                    }
                }
            }
            return ret;
        }

    }
}