using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using Newtonsoft.Json;
using MappingSubdist.DAL;
using MappingSubdist.Models;

namespace MappingSubdist.Controllers
{
    public class HomeController : Controller
    {
        readonly string conString = ConfigurationManager.ConnectionStrings["RESERVE_DISCOUNT"].ConnectionString; 
        readonly SubdistDAL DAL = new SubdistDAL();

        readonly string SP_MAPPING_SUBDIST = "[DBO].[SP_MAPPING_SUBDIST]"; 
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
        #endregion


        [Route("subdist")]
        public ActionResult Subdist(SubdistModel subdistModel)
        {
            var dictionary = new Dictionary<string, object>
            {
                 { "OPTION", subdistModel.Option },
                 { "CABANG", subdistModel.KodeCabang },
                 { "REGIONAL", subdistModel.Region },
                 { "KODESUBDIST", subdistModel.KodeSubdist },
                 { "USERNAME", subdistModel.Username },
                 { "EMAIL", subdistModel.Email },
                 { "PASSWORD", subdistModel.Password }
            };
            var parameters = new DynamicParameters(dictionary);
            return Json(DAL.StoredProcedure(parameters, SP_MAPPING_SUBDIST, SubdistConn)); 
        }
         
    }
}