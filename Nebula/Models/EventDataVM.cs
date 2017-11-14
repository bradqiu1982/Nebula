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
            title = string.Empty;
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
            title = _title;
            className = _className;
            desc = _desc;
            start = _start;
            end = _end;
        }

        private static string GetUniqKey()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static void AddUpdateSchedule(string jonum, string workflow, DateTime date)
        {
            var exist = RetrieveSchedule(jonum, workflow);
            if (exist.Count == 0)
            {
                var sql = "insert into JOScheduleEventDataVM(jonum,id,workflow,className,description,starttime,endtime) "
                    + " values('<jonum>','<id>','<workflow>','<className>','<description>','<starttime>','<endtime>')";
                sql = sql.Replace("<jonum>", jonum).Replace("<id>", GetUniqKey())
                    .Replace("<workflow>", workflow).Replace("<className>", "")
                    .Replace("<description>", "").Replace("<starttime>", date.ToString("yyyy-MM-dd HH:mm:ss")).Replace("<endtime>", date.ToString("yyyy-MM-dd HH:mm:ss"));
                DBUtility.ExeLocalSqlNoRes(sql);
            }
            else
            {
                if (date > Convert.ToDateTime(exist[0].end))
                {
                    var sql = "update JOScheduleEventDataVM set endtime = '<endtime>' where id = '<id>'";
                    sql = sql.Replace("id", exist[0].id).Replace("<endtime>", date.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
        }

        public bool AddSchedule()
        {
            var exist = RetrieveSchedule(jonum, title);
            if (exist.Count == 0)
            {
                var sql = "insert into JOScheduleEventDataVM(jonum,id,workflow,className,description,starttime,endtime) "
                    + " values('<jonum>','<id>','<workflow>','<className>','<description>','<starttime>','<endtime>')";
                sql = sql.Replace("<jonum>", jonum).Replace("<id>", id)
                    .Replace("<workflow>", title).Replace("<className>", className)
                    .Replace("<description>", desc).Replace("<starttime>", start).Replace("<endtime>", end);
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
                if (string.Compare(exist[0].title, title, true) == 0)
                {
                    ret = true;
                }
                else
                {
                    var exist1 = RetrieveSchedule(exist[0].jonum, title);
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
                var sql = "update JOScheduleEventDataVM set workflow = '<workflow>',className = '<className>',description = '<description>',starttime = '<starttime>',endtime = '<endtime>' where id = '<id>'";
                sql = sql.Replace("<workflow>", title).Replace("<className>", className)
                    .Replace("<description>", desc).Replace("<starttime>", start)
                    .Replace("<endtime>", end).Replace("<id>", id);
                DBUtility.ExeLocalSqlNoRes(sql);
            }

            return ret;
        }

        public static List<JOScheduleEventDataVM> RetrieveSchedule(string id)
        {
            var ret = new List<JOScheduleEventDataVM>();
            var sql = "select jonum,id,workflow,className,description,starttime,endtime from JOScheduleEventDataVM where id = '<id>'";
            sql = sql.Replace("<id>", id);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new JOScheduleEventDataVM();
                temp.jonum = Convert.ToString(line[0]);
                temp.id = Convert.ToString(line[1]);
                temp.title = Convert.ToString(line[2]);
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
            var sql = "select jonum,id,workflow,className,description,starttime,endtime from JOScheduleEventDataVM where jonum = '<jonum>' and workflow = '<workflow>'";
            sql = sql.Replace("<jonum>", jonum).Replace("<workflow>", workflow);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new JOScheduleEventDataVM();
                temp.jonum = Convert.ToString(line[0]);
                temp.id = Convert.ToString(line[1]);
                temp.title = Convert.ToString(line[2]);
                temp.className = Convert.ToString(line[3]);
                temp.desc = Convert.ToString(line[4]);
                temp.start = Convert.ToString(line[5]);
                temp.end = Convert.ToString(line[6]);
                ret.Add(temp);
            }
            return ret;
        }

        public static List<JOScheduleEventDataVM> RetrieveScheduleByJoNum(string jonum)
        {
            var ret = new List<JOScheduleEventDataVM>();
            var sql = "select jonum,id,workflow,className,description,starttime,endtime from JOScheduleEventDataVM where jonum = '<jonum>'";
            sql = sql.Replace("<jonum>", jonum);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            foreach (var line in dbret)
            {
                var temp = new JOScheduleEventDataVM();
                temp.jonum = Convert.ToString(line[0]);
                temp.id = Convert.ToString(line[1]);
                temp.title = Convert.ToString(line[2]);
                temp.className = Convert.ToString(line[3]);
                temp.desc = Convert.ToString(line[4]);
                temp.start = Convert.ToString(line[5]);
                temp.end = Convert.ToString(line[6]);
                ret.Add(temp);
            }
            return ret;
        }

        public void RemoveSchedule(string eid)
        {
            var sql = "delete from JOScheduleEventDataVM where id = '<id>'";
            sql = sql.Replace("<id>", eid);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public bool MoveSchedule()
        {
            var ret = false;
            var exist = RetrieveSchedule(id);
            if (exist.Count > 0)
            {
                ret = true;
            }
            else
            {
                ret = false;
            }

            if (ret)
            {
                var sql = "update JOScheduleEventDataVM set starttime = '<starttime>',endtime = '<endtime>' where id = '<id>'";
                sql = sql.Replace("<starttime>", start).Replace("<endtime>", end).Replace("<id>", id);
                DBUtility.ExeLocalSqlNoRes(sql);
            }

            return ret;
        }

        public static string RetriveLatestScheduleDate(string jonum)
        {
            var sql = "select top 1 endtime from JOScheduleEventDataVM where jonum = '<jonum>' order by endtime desc";
            sql = sql.Replace("<jonum>", jonum);
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                return Convert.ToString(dbret[0][0]);
            }
            else
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public string jonum { set; get; }
        public string id { set; get; }
        public string title { set; get; }
        public string className { set; get; }
        public string desc { set; get; }
        public string start { set; get; }
        public string end { set; get; }
    }
}