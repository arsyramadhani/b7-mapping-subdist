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


            if (dt.Rows.Count > 0)
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://DC=ONEKALBE,DC=DOM");
                DirectorySearcher mySearcher = new DirectorySearcher(entry);
                mySearcher.Filter = "(&(objectClass=user)(objectCategory=person)(SamAccountName =" + "yogesh.patel" + "))";

                try
                {
                    SearchResult sr = mySearcher.FindOne();
                    System.Web.HttpContext.Current.Session["ISLOGIN"] = true;
                }
                catch (Exception)
                {
                    System.Web.HttpContext.Current.Session["ISLOGIN"] = false;
                    throw;
                }
            }
            else
            {

            }

 
            
            var Res = new Dictionary<string, string>(){};

            Res.Add("username", System.Web.HttpContext.Current.Session["USERNAME"].ToString());
            Res.Add("islogin", System.Web.HttpContext.Current.Session["ISLOGIN"].ToString().ToLower());

            return Json(Res);
        }
        
        [Route("authorize/login")]
        [WebMethod(EnableSession = true)]
        public ActionResult LoginProcess(SubdistModel model)
        {
            string loginUser = "yogesh.patel";
             

            SearchResultCollection results;
            DirectorySearcher ds = null;
            DirectoryEntry de = new DirectoryEntry("LDAP://DC=ONEKALBE,DC=DOM");

            ds = new DirectorySearcher(de);
            ds.Filter = "(&(objectCategory=User)(objectClass=person)(samaccountname=" + loginUser + "))";

            SearchResult sr = ds.FindOne();



            return Json(sr);
        }

        [Route("auth/logout")]
        public ActionResult Logout()
        {
            return Json("OK");
        }

    };
}