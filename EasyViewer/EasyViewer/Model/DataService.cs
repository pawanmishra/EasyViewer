using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using EasyViewer.Dto;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace EasyViewer.Model
{
    public class DataService : IDataService
    {
        private readonly IConnectionInfoService _connectionInfoService;

        public DataService(IConnectionInfoService connectionInfoService)
        {
            _connectionInfoService = connectionInfoService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public QueryData FetchQueryData(string dataBase, string tableName, string query)
        {
            string connectionString = _connectionInfoService.GetConnectionString(dataBase);

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var adapter = new SqlDataAdapter(query, conn);
                DataSet dataset = new DataSet(tableName);
                adapter.Fill(dataset);
                return new QueryData(dataset.Tables[0], tableName);
            }
        }

        public List<ForeignKeyMetaData> GetForeignKeyMetaData(string dataBase)
        {
            var lst = new List<ForeignKeyMetaData>();
            string connectionString = _connectionInfoService.GetConnectionString(dataBase);
            using (var connection = new SqlConnection(connectionString))
            {
                var serverConnection = new ServerConnection(connection);
                var srv = new Server(serverConnection);
                Database db = srv.Databases[dataBase];

                foreach (Table tb in db.Tables)
                {
                    foreach (ForeignKey f in tb.ForeignKeys)
                    {
                        foreach (ForeignKeyColumn data in f.Columns)
                        {
                            lst.Add(new ForeignKeyMetaData(data.Name, tb.Name, tb.Schema,
                                f.ReferencedTable, data.ReferencedColumn, f.ReferencedTableSchema));
                        }
                    }
                }
            }

            return lst;
        }
    }
}