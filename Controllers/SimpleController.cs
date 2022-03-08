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
    public class SimpleController : Controller
    {
        readonly string conString = ConfigurationManager.ConnectionStrings["RESERVE_DISCOUNT"].ConnectionString;
        readonly SubdistDAL DAL = new SubdistDAL();

        readonly string SP_MAPPING_SUBDIST = "[DBO].[SP_MAPPING_SUBDIST]";
        readonly string SubdistConn = "RESERVE_DISCOUNT";
        // GET: Simple
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SimpleQuery(SimpleQueryModel Models, string spname) //untuk query ringan seperti select dan single insert ,update,delete,
        {
            var parameters = new DynamicParameters(Models.SimpleModel);
            return Json(DAL.StoredProcedure(parameters, SP_MAPPING_SUBDIST, SubdistConn));

        }
    }
}