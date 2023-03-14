using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationCLogic.Extensions;

namespace AutomationCLogic.Repository
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private IDbConnection CreateConnection()
            => new SqlConnection(_configuration.GetConnectionString("ERPDataBase"));

        public DataTable GetDataTableBySqlQuery(string sql)
        {
            using (var connection = this.CreateConnection())
            {
                var dataReader = connection.ExecuteReader(sql);

                return dataReader.ConvertToDataSet().Tables[0];
            }
        }
    }
}
