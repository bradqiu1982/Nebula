﻿using System;
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
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public int Validated { get; set; }

        public int Priority { get; set; }

        public DateTime UpdateDate { get; set; }

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
            sql = sql.Replace("<UserName>", Email.ToUpper()).Replace("<PassWD>", Password).Replace("<UpdateDate>", UpdateDate.ToString());
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static NebulaUserViewModels RetrieveUser(string username)
        {
            var sql = "select PassWD,Validated,Priority,UpdateDate from UserTable where UserName = '<UserName>'";
            sql = sql.Replace("<UserName>", username.ToUpper());
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                var ret = new NebulaUserViewModels();
                ret.Email = username.ToUpper();
                ret.Password = Convert.ToString(dbret[0][0]);
                ret.Validated = Convert.ToInt32(dbret[0][1]);
                ret.Priority = Convert.ToInt32(dbret[0][2]);
                ret.UpdateDate = DateTime.Parse(Convert.ToString(dbret[0][3]));
                return ret;
            }
            return null;
        }

        public static void ActiveUser(string username)
        {
            var sql = "update UserTable set Validated = 1 where UserName = '<UserName>'";
            sql = sql.Replace("<UserName>", username.ToUpper());
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static void UpdateUserTime(string username,DateTime date)
        {
            var sql = "update UserTable set UpdateDate = '<UpdateDate>' where UserName = '<UserName>'";
            sql = sql.Replace("<UpdateDate>", date.ToString()).Replace("<UserName>", username.ToUpper());
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static void RestPwd(string username, string pwd)
        {
            var sql = "update UserTable set PassWD = '<PassWD>' where UserName = '<UserName>'";
            sql = sql.Replace("<UserName>", username.ToUpper()).Replace("<PassWD>", pwd);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static List<string> RetrieveAllUser()
        {
            var sql = "select APVal1 from UserMatrix";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            var ret = new List<string>();

            foreach (var line in dbret)
            {
                ret.Add(Convert.ToString(line[0]));
            }

            ret.Sort();
            return ret;
        }

        public static List<string> RetrieveAllDepartment()
        {
            var sql = "select distinct APVal2 from UserMatrix order by APVal2";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            var ret = new List<string>();

            foreach (var line in dbret)
            {
                ret.Add(Convert.ToString(line[0]));
            }

            ret.Sort();
            return ret;
        }

        public static void StoreUserMatrix(string username, string depart,string auth)
        {
            var sql = "delete from UserMatrix where APVal1='<APVal1>'";
            sql = sql.Replace("<APVal1>", username.ToUpper());
            DBUtility.ExeLocalSqlNoRes(sql);

            sql = "insert into UserMatrix(APVal1,APVal2,APVal3) values('<APVal1>','<APVal2>','<APVal3>')";
            sql = sql.Replace("<APVal1>", username).Replace("<APVal2>", depart).Replace("<APVal3>", auth);
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static List<UserDepart> RetrieveAllUserDepart()
        {
            var sql = "select APVal1,APVal2,APVal3 from UserMatrix";
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            var ret = new List<UserDepart>();

            foreach (var line in dbret)
            {
                var tempinfo = new UserDepart();
                tempinfo.UserName = Convert.ToString(line[0]);
                tempinfo.Depart = Convert.ToString(line[1]);
                tempinfo.Auth = Convert.ToString(line[2]);
                ret.Add(tempinfo);
            }
            return ret;
        }

        public static bool IsAdmin(string username)
        {
            var sql = "select APVal1,APVal2,APVal3 from UserMatrix where APVal1 = '<APVal1>'";
            sql = sql.Replace("<APVal1>", username.ToUpper());
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                if (string.Compare(Convert.ToString(dbret[0][2]).ToUpper(), "ADMIN") == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsDemo(string username)
        {
            var sql = "select APVal1,APVal2,APVal3 from UserMatrix where APVal1 = '<APVal1>'";
            sql = sql.Replace("<APVal1>", username.ToUpper());
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                if (string.Compare(Convert.ToString(dbret[0][2]).ToUpper(), "DEMO") == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool UserExist(string username)
        {
            var sql = "select APVal1,APVal2,APVal3 from UserMatrix where APVal1 = '<APVal1>'";
            sql = sql.Replace("<APVal1>", username.ToUpper());
            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            if (dbret.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void UpdateUserHistory(string UserName, string ECONum, string CardType, string CardKey)
        {
            var sql = "insert into UserRank(UserName,Rank,APVal1,APVal2,UpdateDate) values('<UserName>','<ECONum>','<CardType>','<CardKey>','<UpdateDate>')";
            sql = sql.Replace("<UserName>", UserName.ToUpper()).Replace("<ECONum>", ECONum)
                .Replace("<CardType>", CardType).Replace("<CardKey>", CardKey).Replace("<UpdateDate>", DateTime.Now.ToString());
            DBUtility.ExeLocalSqlNoRes(sql);
        }

        public static List<UserHistoryRec> RetrieveUserHistory(string UserName)
        {
            var sql = "select top 10 UserName,Rank,APVal1,APVal2 from UserRank where UserName = '<UserName>' order by UpdateDate DESC";
            sql = sql.Replace("<UserName>", UserName.ToUpper());

            var dbret = DBUtility.ExeLocalSqlWithRes(sql);
            var ret = new List<UserHistoryRec>();

            foreach (var line in dbret)
            {
                var tempinfo = new UserHistoryRec();
                tempinfo.UserName = Convert.ToString(line[0]);
                tempinfo.ECONum = Convert.ToString(line[1]);
                tempinfo.CardType = Convert.ToString(line[2]);
                tempinfo.CardKey = Convert.ToString(line[3]);
                ret.Add(tempinfo);
            }
            return ret;
        }

    }

}