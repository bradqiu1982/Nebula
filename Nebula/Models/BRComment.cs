using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nebula.Models
{
    public class BRCOMMENTTP
    {
        public static string COMMENT = "COMMENT";
        public static string CONCLUSION = "CONCLUSION";
    }

    public class BRComment
    {
        public BRComment()
        {
            BRNum = "";
            Comment = "";
            CommentKey = "";
            Reporter = "";
            CommentType = "";
            CommentDate = DateTime.Parse("1982-05-06 10:00:00");
        }

        public string BRNum { set; get; }
        public string CommentKey { set; get; }

        private string sComment = "";
        public string Comment
        {
            set { sComment = value; }
            get { return sComment; }
        }

        public string dbComment
        {
            get
            {
                if (string.IsNullOrEmpty(sComment))
                {
                    return "";
                }
                else
                {
                    try
                    {
                        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sComment));
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }

            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    sComment = "";
                }
                else
                {
                    try
                    {
                        sComment = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
                    }
                    catch (Exception)
                    {
                        sComment = "";
                    }

                }

            }
        }

        public string Reporter { set; get; }

        public DateTime CommentDate { set; get; }

        public string CommentType { set; get; }

    }

    public class SeverHtmlDecode
    {
        public static string Decode(Controller ctrl, string src)
        {
            return ctrl.Server.HtmlDecode(src).Replace("border=\"0\"", "border=\"2\"");
        }
    }
}