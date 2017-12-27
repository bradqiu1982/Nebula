﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Threading;
using System.Web.UI.WebControls;

namespace Nebula.Models
{
    public class EmailUtility
    {
        private static void logthdinfo(string info)
        {
            try
            {
                var filename = "d:\\log\\emailexception-" + DateTime.Now.ToString("yyyy-MM-dd");
                if (System.IO.File.Exists(filename))
                {
                    var content = System.IO.File.ReadAllText(filename);
                    content = content + "\r\n" + DateTime.Now.ToString() + " : " + info;
                    System.IO.File.WriteAllText(filename, content);
                }
                else
                {
                    System.IO.File.WriteAllText(filename, DateTime.Now.ToString() + " : " + info);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static bool SendEmail(Controller ctrl, string title, List<string> tolist, string content, bool isHtml = false, string attachpath = null)
        {
            try
            {
                var syscfgdict = CfgUtility.GetSysConfig(ctrl);

                var message = new MailMessage();
                if (!string.IsNullOrEmpty(attachpath))
                {
                    var attach = new Attachment(attachpath);
                    message.Attachments.Add(attach);
                }

                message.IsBodyHtml = isHtml;
                message.From = new MailAddress(syscfgdict["APPEMAILADRESS"]);
                foreach (var item in tolist)
                {
                    if (!item.Contains("@"))
                        continue;
                    try
                    {
                        if (item.Contains(";"))
                        {
                            var ts = item.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var t in ts)
                            {
                                message.To.Add(t);
                            }
                        }
                        else
                        {
                            message.To.Add(item);
                        }
                    }
                    catch (Exception e) { logthdinfo("Address exception: " + e.Message); }
                }

                message.Subject = title;
                message.Body = content;

                SmtpClient client = new SmtpClient();
                client.Host = syscfgdict["EMAILSERVER"];
                client.EnableSsl = true;
                client.Timeout = 60000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(syscfgdict["APPEMAILADRESS"], syscfgdict["APPEMAILPWD"]);

                ServicePointManager.ServerCertificateValidationCallback
                    = delegate (object s, X509Certificate certificate, X509Chain chain
                    , SslPolicyErrors sslPolicyErrors) { return true; };

                new Thread(() => {
                    try
                    {
                        client.Send(message);
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        logthdinfo("SmtpFailedRecipientsException exception: " + ex.Message);
                        try
                        {
                            message.To.Clear();
                            foreach (var item in tolist)
                            {
                                if (ex.Message.Contains(item))
                                {
                                    try
                                    {
                                        message.To.Add(item);
                                    }
                                    catch (Exception e) { logthdinfo("Address exception2: " + e.Message); }
                                }
                            }
                            client.Send(message);
                        }
                        catch (Exception ex1)
                        {
                            logthdinfo("nest exception1: " + ex1.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        logthdinfo("send exception: " + ex.Message);
                    }
                }).Start();
            }
            catch (Exception ex)
            {
                logthdinfo("main exception: " + ex.Message);
                return false;
            }
            return true;
        }


        public static string CreateTableStr(List<List<string>> table)
        {
            var content = string.Empty;
            var idx = 0;
            if (table != null)
            {
                content += "<div><br>";
                content += "<table border='1' cellpadding='0' cellspacing='0' width='100%'>";
                content += "<colgroup><col style='width:20%;'><col style='width:60%;'><col style='width:20%;'></colgroup>";
                content += "<thead style='background-color: #006DC0; color: #fff;'>";
                foreach (var th in table[0])
                {
                    content += "<th>" + th + "</th>";
                }
                content += "</thead>";
                foreach (var tr in table)
                {
                    if (idx > 0)
                    {
                        var tidx = 0;
                        content += "<tr>";
                        foreach (var td in tr)
                        {
                            if (tidx == 0)
                            {
                                content += "<td><strong>" + td + "</strong></td>";
                            }
                            else
                            {
                                content += "<td>" + td + "</td>";
                            }
                            tidx++;
                        }
                        content += "</tr>";
                    }
                    idx++;
                }
                content += "</table>";
                content += "</div>";

                return content;
            }

            return string.Empty;
        }

        public static string CreateTableHtml2(string greetig, string description, string comment, List<string> tables)
        {

            var content = "<!DOCTYPE html>";
            content += "<html>";
            content += "<head>";
            content += "<meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />";
            content += "<title></title>";
            content += "</head>";
            content += "<body>";
            content += "<div><p>" + greetig + ",</p></div>";
            content += "<div><p>" + description + "</p></div>";
            if (!string.IsNullOrEmpty(comment))
            {
                content += "<div><p>" + comment + "</p>";
            }

            foreach (var tb in tables)
            {
                content += tb;
            }

            content += "<br><br>";
            content += "<div><p style='font-size: 12px; font-style: italic;'>This is a system generated message, please do not reply to this email.</p></div>";
            content += "</body>";
            content += "</html>";

            return content;
        }


    }
}