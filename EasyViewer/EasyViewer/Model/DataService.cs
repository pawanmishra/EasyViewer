using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using EasyViewer.Dto;
using Microsoft.SqlServer.Management.Smo;

namespace EasyViewer.Model
{
    public class DataService : IDataService
    {
        private readonly string _connectionStringFormat = ConfigurationManager.AppSettings["ConnectionStringFormat"];

        public QueryData FetchQueryData(string dataBase, string tableName, string query)
        {
            string connectionString = string.Format(_connectionStringFormat, dataBase);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataSet dataset = new DataSet(tableName);
                adapter.Fill(dataset);
                return new QueryData(dataset.Tables[0], tableName);
            }

            return null;
        }

        public List<ForeignKeyMetaData> GetForeignKeyMetaData(string dataBase)
        {
            List<ForeignKeyMetaData> lst = new List<ForeignKeyMetaData>();
            Server srv = new Server(".");
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

            return lst;
        }
    }
}