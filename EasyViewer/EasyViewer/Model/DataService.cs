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
        public QueryData FetchQueryData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TimeTracker"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(@"select top(2) * from TimeTrackerHistory", conn);
                DataSet dataset = new DataSet("TimeTracker");
                adapter.Fill(dataset);
                return new QueryData(dataset.Tables[0], "TimeTrackerHistory");
            }

            return null;
        }

        public List<ForeignKeyMetaData> GetForeignKeyMetaData()
        {
            List<ForeignKeyMetaData> lst = new List<ForeignKeyMetaData>();
            Server srv = new Server(".");
            Database db = srv.Databases["TimeTracker"];

            foreach (Table tb in db.Tables)
            {
                foreach (ForeignKey f in tb.ForeignKeys)
                {
                    foreach (ForeignKeyColumn data in f.Columns)
                    {
                        lst.Add(new ForeignKeyMetaData(data.Name, "TimeTrackerHistory", data.ReferencedColumn, f.ReferencedTable));
                    }
                }
            }

            return lst;
        }
    }
}