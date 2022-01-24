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

        [Route("auth/login")]
        [WebMethod(EnableSession = true)]
        public ActionResult Login(SubdistModel model)
        {
            System.Web.HttpContext.Current.Session["USERNAME"] = null;
            System.Web.HttpContext.Current.Session["ISLOGIN"] = false;

            string loginUser = WindowsIdentity.GetCurrent().Name.Replace(@"ONEKALBE\", "");
             
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

            if (dt.Rows.Count >= 0)
            {

                IntPtr tokenHandle = new IntPtr(0);
                try
                {
                    string UserName, MachineName, Pwd = null;
                    UserName = model.Username;
                    Pwd = model.Password;
                    MachineName = "ONEKALBE";
                    const int LOGON32_PROVIDER_DEFAULT = 0;
                    const int LOGON32_LOGON_INTERACTIVE = 2;
                    tokenHandle = IntPtr.Zero;

                    //Call the LogonUser function to obtain a handle to an access token.
                    bool returnValue = LogonUser(UserName, MachineName, Pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref tokenHandle);

                    System.Web.HttpContext.Current.Session["ISLOGIN"] = true;
                    System.Web.HttpContext.Current.Session["USERNAME"] = model.Username;  

                    if (returnValue == false)
                    {
                        int ret = Marshal.GetLastWin32Error();
                        if (ret == 1329)
                        {
                            System.Web.HttpContext.Current.Session["ISLOGIN"] = true;
                            System.Web.HttpContext.Current.Session["USERNAME"] = model.Username;
                            System.Web.HttpContext.Current.Session["xUser"] = model.Username;
                        }
                    }
                    else
                    {
                        System.Web.HttpContext.Current.Session["ISLOGIN"] = true;
                        System.Web.HttpContext.Current.Session["USERNAME"] = model.Username;
                    }

                }
                catch (Exception ex)
                {
                    System.Web.HttpContext.Current.Session["ISLOGIN"] = false;
                    throw ex;
                }

            }
            else
            {
                System.Web.HttpContext.Current.Session["ISLOGIN"] = false;
                System.Web.HttpContext.Current.Session["USERNAME"] = model.Username;
            }
            
            var Res = new Dictionary<string, string>(){};

            Res.Add("username", System.Web.HttpContext.Current.Session["USERNAME"].ToString());
            Res.Add("islogin", System.Web.HttpContext.Current.Session["ISLOGIN"].ToString().ToLower());

            return Json(Res);
        } 

        [Route("auth/logout")]
        public ActionResult Logout()
        {
            return Json("OK");
        }

    };
}