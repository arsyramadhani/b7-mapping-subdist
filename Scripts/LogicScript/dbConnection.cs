using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace PAOnline.Scripts.LogicScript
{
    public class dbConnection
    {
        DataTable dt = new DataTable();
        readonly ConnectionStringSettings dbDFIS = ConfigurationManager.ConnectionStrings["dbDFIS"];
        public class AttributeParameter
        {
            public string Option { get; set; }
            public string Username { get; set; }
        }

        
    }

}