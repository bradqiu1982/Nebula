using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nebula.Models
{
    public class MachineUserMap
    {
        public static Dictionary<string, string> RetrieveUserMap(string machine = "", string username = "")
        {
            var ret = new Dictionary<string, string>();

            var sql = "select machine,username from machineusermap where 1 = 1";
            if (!string.IsNullOrEmpty(machine))
            {
                sql = sql + " and machine = '<machine>'";
                sql = sql.Replace("<machine>", machine);
            }
            if (!string.IsNullOrEmpty(username))
            {
                sql = sql + " and username = '<username>'";
                sql = sql.Replace("<username>", username);
            }

            var dbret = DBUtility.ExeTraceSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var tempvm = new MachineUserMap();
                tempvm.machine = Convert.ToString(line[0]);
                tempvm.username = Convert.ToString(line[1]);
                if (!ret.ContainsKey(tempvm.machine))
                {
                    ret.Add(tempvm.machine, tempvm.username);
                }
            }
            return ret;
        }

        public string machine { set; get; }
        public string username { set; get; }
    }
}