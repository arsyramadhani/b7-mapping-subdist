using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Security.Principal;
using Dapper;
using MappingSubdist.DAL;
using System.Data; 
using System.Data.SqlClient;
using Newtonsoft.Json;
using MappingSubdist.Models;
using System.Web.Services;
using System.DirectoryServices;
using System.Diagnostics;

namespace MappingSubdist.Controllers
{
    public class AuthController : Controller
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = false)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        readonly string conString = ConfigurationManager.ConnectionStrings["RESERVE_DISCOUNT"].ConnectionString;
        readonly SubdistDAL DAL = new SubdistDAL();

        readonly string SP_MAPPING_SUBDIST = "[DBO].[SP_MAPPING_SUBDIST]";
        readonly string SubdistConn = "RESERVE_DISCOUNT";

        [Route("authorize/login")]
        [WebMethod(EnableSession = true)]
        public ActionResult Login(SubdistModel model)
        {
            System.Web.HttpContext.Current.Session["ISLOGIN"] = false;
            System.Web.HttpContext.Current.Session["USERNAME"] = null;
            System.Web.HttpContext.Current.Session["NIK"] = false;
            System.Web.HttpContext.Current.Session["JOBTTLNAME"] = false;
            System.Web.HttpContext.Current.Session["EMAIL"] = false;

            string loginUser = WindowsIdentity.GetCurrent().Name.Replace(@"ONEKALBE\", "");
            
            var Res = new Dictionary<string, string>() { };

            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(conString);

            try
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(SP_MAPPING_SUBDIST, conn))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@OPTION", System.Data.SqlDbType.NVarChar).Value = "LOGIN";
                    command.Parameters.Add("@USERNAME", System.Data.SqlDbType.NVarChar).Value = model.Username;
                    command.Parameters.Add("@PASSWORD", System.Data.SqlDbType.NVarChar).Value = model.Password;
                    SqlDataAdapter dataAdapt = new SqlDataAdapter();
                    dataAdapt.SelectCommand = command;
                    dataAdapt.Fill(dt);
                }
                conn.Close(); 
            }
            catch (Exception ex)
            {

                throw ex;
            }


            if (dt.Rows.Count > 0)
            {
                IntPtr tokenHandle = new IntPtr(0);


                string UserName, MachineName, Pwd = null;

                //The MachineName property gets the name of your computer.                
                UserName = model.Username;
                Pwd = model.Password;
                MachineName = "ONEKALBE";
                const int LOGON32_PROVIDER_DEFAULT = 0;
                const int LOGON32_LOGON_INTERACTIVE = 2;
                tokenHandle = IntPtr.Zero;

                //Call the LogonUser function to obtain a handle to an access token.
                bool returnValue = LogonUser(UserName, MachineName, Pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref tokenHandle);

                /*if (returnValue == false)
                {
                    int ret = Marshal.GetLastWin32Error();
                    if (ret == 1329)
                    {
                        System.Web.HttpContext.Current.Session["ISLOGIN"] = false;
                    }
                    else
                    {
                        System.Web.HttpContext.Current.Session["USERNAME"] = model.Username;
                        System.Web.HttpContext.Current.Session["ISLOGIN"] = false;
                     }
                }
                else
                {*/
                    System.Web.HttpContext.Current.Session["ISLOGIN"] = true;
                    System.Web.HttpContext.Current.Session["USERNAME"] = "Akip";
                    System.Web.HttpContext.Current.Session["NIK"] = "001";
                    System.Web.HttpContext.Current.Session["JOBTTLNAME"] = "Web Dev";
                    System.Web.HttpContext.Current.Session["EMAIL"] = "akiptsaqif@gmail.com";
                //    System.Web.HttpContext.Current.Session["USERNAME"] = dt.Rows[0]["USERNAME"].ToString();
                //    System.Web.HttpContext.Current.Session["NIK"] = dt.Rows[0]["NIK"].ToString();
                //    System.Web.HttpContext.Current.Session["JOBTTLNAME"] = dt.Rows[0]["JOBTTLNAME"].ToString();
                //    System.Web.HttpContext.Current.Session["EMAIL"] = dt.Rows[0]["EMAIL"].ToString();
              //  }
            }
            else
            {
                System.Web.HttpContext.Current.Session["ISLOGIN"] = true;
                System.Web.HttpContext.Current.Session["USERNAME"] = "Akip";
                System.Web.HttpContext.Current.Session["NIK"] = "001";
                System.Web.HttpContext.Current.Session["JOBTTLNAME"] = "Web Dev";
                System.Web.HttpContext.Current.Session["EMAIL"] = "akiptsaqif@gmail.com";
            }
             
            //System.Web.HttpContext.Current.Session["ISLOGIN"] = true;

            Res.Add("islogin", System.Web.HttpContext.Current.Session["ISLOGIN"].ToString().ToLower());

            if (System.Web.HttpContext.Current.Session["ISLOGIN"].ToString().ToLower() == "false")
            {
                return Json(Res); 
            }
            else
            {
                Res.Add("username", System.Web.HttpContext.Current.Session["USERNAME"].ToString());
                Res.Add("nik", System.Web.HttpContext.Current.Session["NIK"].ToString());
                Res.Add("jobttlname", System.Web.HttpContext.Current.Session["JOBTTLNAME"].ToString());
                Res.Add("email", System.Web.HttpContext.Current.Session["EMAIL"].ToString()); 
                return Json(Res);
            }

        }
         

        [Route("auth/logout")]
        public ActionResult Logout()
        {
            System.Web.HttpContext.Current.Session["USERNAME"] = null;
            System.Web.HttpContext.Current.Session["ISLOGIN"] = false;

            return RedirectToAction("Login", "Home");
        }

    };
}