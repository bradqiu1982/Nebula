using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Nebula.Models
{
    public class UserDepart
    {
        public string UserName { set; get; }
        public string Depart { set; get; }
        public string Auth { set; get; }
    }

    public class UserHistoryRec
    {
        public string UserName { set; get; }
        public string ECONum { set; get; }
        public string CardType { set; get; }
        public string CardKey { set; get; }
    }

    public class NebulaUserViewModels
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public static bool CheckUserExist(string username)
        {
            var dbret = RetrieveUser(username);
            if (dbret != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RegistUser()
        {
            var sql = "insert into UserTable(UserName,PassWD,UpdateDate) values('<UserName>','<PassWD>','<UpdateDate>')";
            sql = sql.Replace("<UserName>", Email.ToUpper()).Replace("<PassWD>", Password).Replace("<UpdateDate>", DateTime.Now.ToString());
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static NebulaUserViewModels RetrieveUser(string username)
        {
            var sql = "select PassWD from UserTable where UserName = '<UserName>'";
            sql = sql.Replace("<UserName>", username.ToUpper());
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                var ret = new NebulaUserViewModels();
                ret.Email = username.ToUpper();
                ret.Password = Convert.ToString(dbret[0][0]);
                return ret;
            }
            return null;
        }
        
        public static void RestPwd(string username, string pwd)
        {
            var sql = "update UserTable set PassWD = '<PassWD>' where UserName = '<UserName>'";
            sql = sql.Replace("<UserName>", username.ToUpper()).Replace("<PassWD>", pwd);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        //public static List<string> RetrieveAllUser()
        //{
        //    var sql = "select APVal1 from UserMatrix";
        //    var dbret = DBUtility.ExeLocalSqlWithRes(sql);
        //    var ret = new List<string>();

        //    foreach (var line in dbret)
        //    {
        //        ret.Add(Convert.ToString(line[0]));
        //    }

        //    ret.Sort();
        //    return ret;
        //}

        //public static List<string> RetrieveAllDepartment()
        //{
        //    var sql = "select distinct APVal2 from UserMatrix order by APVal2";
        //    var dbret = DBUtility.ExeLocalSqlWithRes(sql);
        //    var ret = new List<string>();

        //    foreach (var line in dbret)
        //    {
        //        ret.Add(Convert.ToString(line[0]));
        //    }

        //    ret.Sort();
        //    return ret;
        //}

        //public static void StoreUserMatrix(string username, string depart,string auth)
        //{
        //    var sql = "delete from UserMatrix where APVal1='<APVal1>'";
        //    sql = sql.Replace("<APVal1>", username.ToUpper());
        //    DBUtility.ExeLocalSqlNoRes(sql);

        //    sql = "insert into UserMatrix(APVal1,APVal2,APVal3) values('<APVal1>','<APVal2>','<APVal3>')";
        //    sql = sql.Replace("<APVal1>", username).Replace("<APVal2>", depart).Replace("<APVal3>", auth);
        //    DBUtility.ExeLocalSqlNoRes(sql);
        //}

        //public static List<UserDepart> RetrieveAllUserDepart()
        //{
        //    var sql = "select APVal1,APVal2,APVal3 from UserMatrix";
        //    var dbret = DBUtility.ExeLocalSqlWithRes(sql);
        //    var ret = new List<UserDepart>();

        //    foreach (var line in dbret)
        //    {
        //        var tempinfo = new UserDepart();
        //        tempinfo.UserName = Convert.ToString(line[0]);
        //        tempinfo.Depart = Convert.ToString(line[1]);
        //        tempinfo.Auth = Convert.ToString(line[2]);
        //        ret.Add(tempinfo);
        //    }
        //    return ret;
        //}

        //public static bool IsAdmin(string username)
        //{
        //    var sql = "select APVal1,APVal2,APVal3 from UserMatrix where APVal1 = '<APVal1>'";
        //    sql = sql.Replace("<APVal1>", username.ToUpper());
        //    var dbret = DBUtility.ExeLocalSqlWithRes(sql);
        //    if (dbret.Count > 0)
        //    {
        //        if (string.Compare(Convert.ToString(dbret[0][2]).ToUpper(), "ADMIN") == 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public static bool IsDemo(string username)
        //{
        //    var sql = "select APVal1,APVal2,APVal3 from UserMatrix where APVal1 = '<APVal1>'";
        //    sql = sql.Replace("<APVal1>", username.ToUpper());
        //    var dbret = DBUtility.ExeLocalSqlWithRes(sql);
        //    if (dbret.Count > 0)
        //    {
        //        if (string.Compare(Convert.ToString(dbret[0][2]).ToUpper(), "DEMO") == 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public static bool UserExist(string username)
        //{
        //    var sql = "select APVal1,APVal2,APVal3 from UserMatrix where APVal1 = '<APVal1>'";
        //    sql = sql.Replace("<APVal1>", username.ToUpper());
        //    var dbret = DBUtility.ExeLocalSqlWithRes(sql);
        //    if (dbret.Count > 0)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        

    }

}