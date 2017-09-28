using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nebula.Models
{
    public class JOScheduleEventDataVM
    {
        public JOScheduleEventDataVM()
        {
            id = string.Empty;
            workflow = string.Empty;
            className = string.Empty;
            desc = string.Empty;
            start = string.Empty;
            end = string.Empty;
            jonum = string.Empty;
        }

        public JOScheduleEventDataVM(string _jonum,string _id,string _title,string _className,string _desc,string _start,string _end)
        {
            jonum = _jonum;
            id = _id;
            workflow = _title;
            className = _className;
            desc = _desc;
            start = _start;
            end = _end;
        }

        public bool AddSchedule()
        {
            var exist = RetrieveSchedule(jonum, workflow);
            if (exist.Count == 0)
            {
                var sql = "insert into JOScheduleEventDataVM(jonum,id,workflow,className,desc,start,end) "
                    + " values('<jonum>','<id>','<workflow>','<className>','<desc>','<start>','<end>')";
                sql = sql.Replace("<jonum>", jonum).Replace("<id>", id)
                    .Replace("<workflow>", workflow).Replace("<className>", className)
                    .Replace("<desc>", desc).Replace("<start>", start).Replace("<end>", end);
                DBUtility.ExeLocalSqlNoRes(sql);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateSchedule()
        {
            var ret = false;
            var exist = RetrieveSchedule(id);
            if (exist.Count > 0)
            {
                if (string.Compare(exist[0].workflow, workflow, true) == 0)
                {
                    ret = true;
                }
                else
                {
                    var exist1 = RetrieveSchedule(exist[0].jonum, workflow);
                    if (exist1.Count == 0)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
            }
            else
            {
                ret = false;
            }

            if (ret)
            {
                var sql = "update JOScheduleEventDataVM set workflow = '<workflow>',className = '<className>',desc = '<desc>',start = '<start>',end = '<end>' where id = '<id>'";
                sql = sql.Replace("<workflow>", workflow).Replace("<className>", className)
                    .Replace("<desc>", desc).Replace("<start>", start)
                    .Replace("<end>", end).Replace("<id>", id);
                DBUtility.ExeLocalSqlNoRes(sql);
            }

            return ret;
        }

        public static List<JOScheduleEventDataVM> RetrieveSchedule(string id)
        {
            var ret = new List<JOScheduleEventDataVM>();
            var sql = "select jonum,id,workflow,className,desc,start,end from JOScheduleEventDataVM where id = '<id>'";
            sql = sql.Replace("<id>", id);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new JOScheduleEventDataVM();
                temp.jonum = Convert.ToString(line[0]);
                temp.id = Convert.ToString(line[1]);
                temp.workflow = Convert.ToString(line[2]);
                temp.className = Convert.ToString(line[3]);
                temp.desc = Convert.ToString(line[4]);
                temp.start = Convert.ToString(line[5]);
                temp.end = Convert.ToString(line[6]);
                ret.Add(temp);
            }
            return ret;
        }

        public static List<JOScheduleEventDataVM> RetrieveSchedule(string jonum,string workflow)
        {
            var ret = new List<JOScheduleEventDataVM>();
            var sql = "select jonum,id,workflow,className,desc,start,end from JOScheduleEventDataVM where jonum = '<jonum>' and workflow = '<workflow>'";
            sql = sql.Replace("<jonum>", jonum).Replace("<workflow>", workflow);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new JOScheduleEventDataVM();
                temp.jonum = Convert.ToString(line[0]);
                temp.id = Convert.ToString(line[1]);
                temp.workflow = Convert.ToString(line[2]);
                temp.className = Convert.ToString(line[3]);
                temp.desc = Convert.ToString(line[4]);
                temp.start = Convert.ToString(line[5]);
                temp.end = Convert.ToString(line[6]);
                ret.Add(temp);
            }
            return ret;
        }

        public string jonum { set; get; }
        public string id { set; get; }
        public string workflow { set; get; }
        public string className { set; get; }
        public string desc { set; get; }
        public string start { set; get; }
        public string end { set; get; }
    }
}