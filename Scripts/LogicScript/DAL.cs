using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using Newtonsoft.Json;
//using PAOnline.Models.DAL;

namespace MappingSubdist.Scripts.LogicScript
{
    //public class DAL
    //{
    //    readonly ConnectionStringSettings dbDFIS = ConfigurationManager.ConnectionStrings["ReserverDiscount"];

    //    public string StoredProcedure(DynamicParameters parameters, String Spname)
    //    {
    //        string result;

    //        using (IDbConnection db = new SqlConnection(dbDFIS.ConnectionString))
    //        {
    //            var StoredProcedure = db.Query<dynamic>(Spname, parameters,
    //                            commandType: CommandType.StoredProcedure).ToList();

    //            var json = JsonConvert.SerializeObject(StoredProcedure, Formatting.Indented);
    //            result = json;
    //        }

    //        return result;
    //    }
    //}
}