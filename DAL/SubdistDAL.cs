using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace MappingSubdist.DAL
{
    public class SubdistDAL
    { 
        public string StoredProcedure(DynamicParameters parameters, String Spname, String Conn)
        {
            string result;
            ConnectionStringSettings dbConnString = ConfigurationManager.ConnectionStrings[Conn];
             
            IDbConnection db = new SqlConnection(dbConnString.ConnectionString);
            try
            {
                var StoredProcedure = db.Query<dynamic>(Spname, parameters,
                                commandType: CommandType.StoredProcedure).ToList();

                return JsonConvert.SerializeObject(StoredProcedure, Formatting.Indented);

                //result = json; 
                //return result;
            }
            catch (Exception ex)
            { 
                //throw ex;
                return JsonConvert.SerializeObject(ex);
            } 
        }
  
    }
}