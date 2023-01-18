using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Dapper;
using Newtonsoft.Json;
using MappingSubdist.DAL;
using MappingSubdist.Models;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace MappingSubdist.Controllers
{
    public class HomeController : Controller
    {
        readonly string conString = ConfigurationManager.ConnectionStrings["RESERVE_DISCOUNT"].ConnectionString;
        readonly SubdistDAL DAL = new SubdistDAL();

        readonly string SP_MAPPING_SUBDIST = "[DBO].[SP_MAPPING_SUBDIST]";
        readonly string SP_INSERT_SUBDIST = "[DBO].[SP_INSERT_SUBDIST]";
        readonly string SP_INJECT_QP = "[DBO].[SP_INJECT_QP]";
        readonly string SubdistConn = "RESERVE_DISCOUNT";

        #region View

        public ActionResult Index()
        {
            if (Session["USERNAME"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();

            }

        }

        [Route("CreateUser")]
        public ActionResult CreateUser()
        {
            if (Session["USERNAME"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View("CreateUser");

            }

        }

        [Route("mappingemail")]
        public ActionResult MappingEmail()
        {
            if (Session["USERNAME"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View("MappingEmail");

            }

        }

        [Route("mappinguser")]
        public ActionResult MappingUser()
        {
            if (Session["USERNAME"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View("MappingUser");

            }

        }

        [Route("login")]
        public ActionResult Login()
        {
            return View("Login");


        }

        [Route("InsertSubdist")]
        public ActionResult InsertSubdist()
        {
            return View();
        }

        [Route("MappingSubdist")]
        public ActionResult MappingSubdist()
        {
            return View();
        }

        [Route("InjectQP")]
        public ActionResult InjectQP()
        {
            return View();
        }
        #endregion

        #region RedirectView
        [Route("home/CreateUser")]
        public ActionResult RedirectCreateUser()
        {

            return RedirectToAction("CreateUser");
        }

        [Route("home/MappingEmail")]
        public ActionResult RedirectMappingEmail()
        {

            return RedirectToAction("MappingEmail");
        }
        [Route("home/MappingUser")]
        public ActionResult RedirectMappingUser()
        {

            return RedirectToAction("MappingUser");
        }
        [Route("home/Login")]
        public ActionResult RedirectLogin()
        { 
            return RedirectToAction("Login");
        }

        [Route("home/InsertSubdist")]
        public ActionResult RedirectCreateSubdist()
        {
            return RedirectToAction("InsertSubdist");
        }

        [Route("home/MappingSubdist")]
        public ActionResult RedirectMappingSubdist()
        {
            return RedirectToAction("MappingSubdist");
        }

        [Route("home/InjectQP")]
        public ActionResult RedirectInjectQP()
        {
            return RedirectToAction("InjectQP");
        }
        #endregion


        [Route("subdist")]
        public ActionResult Subdist(SubdistModel subdistModel)
        {
            var dictionary = new Dictionary<string, object>
            {
                 { "OPTION", subdistModel.Option },
                 { "CABANG", subdistModel.KodeCabang },
                 { "REGION", subdistModel.Region },
                 { "KODESUBDIST", subdistModel.KodeSubdist },
                 { "NAMASUBDIST", subdistModel.NamaSubdist },
                 { "GROUPSPB", subdistModel.GroupSPB },
                 { "USERNAME", subdistModel.Username },
                 { "EMAIL", subdistModel.Email },
                 { "PASSWORD", subdistModel.Password }
            };
            var parameters = new DynamicParameters(dictionary);
            return Json(DAL.StoredProcedure(parameters, SP_MAPPING_SUBDIST, SubdistConn)); 
        }
         
        [Route("insert_subdist")]
        public ActionResult InsertSubdist(InsertSubdistModel model)
        {
            var dict = new Dictionary<string, object>
            {
                { "OPTION", model.Option },
                { "KODESUBDIST", model.KodeSubdist },
                { "NAMASUBDIST", model.NamaSubdist },
                { "REGION", model.Region },
                { "CABANG", model.Cabang },
                { "GROUP", model.Group },
                { "PIC", model.PIC },
                { "NPWP", model.NPWP },
                { "ALAMAT", model.Alamat },
                { "GROUPDESC", model.GroupDesc },
                { "GROUPSPB", model.GroupSPB }
            };

            var param = new DynamicParameters(dict);
            return Json(DAL.StoredProcedure(param, SP_INSERT_SUBDIST, SubdistConn));
        }

        [Route("QPOperation")]
        public ActionResult QPOperation(QPModel model)
        {
            var dict = new Dictionary<string, object>
            {
                { "OPTION", model.Option },
                { "MEMO", model.Memo },
                { "REMARKS", model.Remarks }
            };

            var param = new DynamicParameters(dict);
            return Json(DAL.StoredProcedure(param, SP_INJECT_QP, SubdistConn));
        }

        public void injectQP(string path, string name)
        {
            string excelConnString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0\"", path);

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                // Delete old entries
                SqlCommand truncate = new SqlCommand("TRUNCATE TABLE temp_InjectQP", con);
                truncate.ExecuteNonQuery();
                con.Close();
            }

            using (OleDbConnection excelConnection = new OleDbConnection(excelConnString))
            {
                using (OleDbCommand cmd = new OleDbCommand("Select [SITE_USE_ID],[NAMALANG],[SALES HNA],[QP] from [RECAP$] WHERE [SITE_USE_ID] IS NOT NULL", excelConnection))
                {
                    excelConnection.Open();
                    using (OleDbDataReader dReader = cmd.ExecuteReader())
                    {
                        using (SqlBulkCopy sqlBulk = new SqlBulkCopy(conString))
                        {
                            //Give your Destination table name 
                            sqlBulk.DestinationTableName = "temp_InjectQP";
                            sqlBulk.WriteToServer(dReader);
                        }
                    }
                }
            }
        }

        [Route("fileUpload")]
        public string fileUpload()
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            string status;

            var file = Request.Files[0];
            var fileName = rgx.Replace(Path.GetFileName(file.FileName), "");
            var fileNameWithoutExt = rgx.Replace(Path.GetFileNameWithoutExtension(file.FileName), "");

            string folder = ("~/Content/fileQP");
            var excelPath = Path.Combine(Server.MapPath(folder), fileName);
            file.SaveAs(excelPath);

            injectQP(excelPath, fileName);

            return fileName;
        }

        
    }
}