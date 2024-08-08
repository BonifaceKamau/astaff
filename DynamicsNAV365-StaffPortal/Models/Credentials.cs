using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using DynamicsNAV365_StaffPortal.TimeSheet;
using DynamicsNAV365_StaffPortal.Controllers;
using System.IO;

namespace DynamicsNAV365_StaffPortal.Models
{
    public class Credentials
    {
        public static HttpWebResponse GetOdataData(string page)
        {
            HttpWebResponse httpResponse = null;
            string Url = ConfigurationManager.AppSettings["W_PWD"];
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["ODATA_URI"] + page);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["W_USER"],
                        ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"]);

            httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            return httpResponse;
        }

        public static TimeSheetWS ObjNav
        {
            get
            {
                var ws = new TimeSheetWS();

                try
                {
                    var credentials = new NetworkCredential(ConfigurationManager.AppSettings["W_USER"],
                        ConfigurationManager.AppSettings["W_PWD"], ConfigurationManager.AppSettings["DOMAIN"]);

                    ws.Credentials = credentials;
                    ws.PreAuthenticate = true;

                }
                catch (Exception ex)
                {
                    ex.Data.Clear();
                }
                return ws;
            }
        }

        public static string ProfilePicture(string User)
        {
            string PicString = "";
            try
            {
                AccountController accountController = new AccountController();
                string StaffNo = AccountController.GetEmployeeNo();
                PicString = ObjNav.GetPassportPic(StaffNo);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return PicString;
        }

        public static string GetSignature(string User)
        {
            string PicString = "";
            try
            {
                AccountController accountController = new AccountController();
                string StaffNo = AccountController.GetEmployeeNo();
                PicString = ObjNav.GetSignature(StaffNo);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return PicString;
        }
        public static string GetDocumentAttachmet(int TblID, string DocNo, int Id)
        {
            string PicString = "";
            try
            {
                PicString = ObjNav.GetDocumentAttachment(TblID, DocNo, Id);
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            return PicString;
        }

        public static void DownloadAttachment(string path, Byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }
    }
}