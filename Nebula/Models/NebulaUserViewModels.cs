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

    }

}