using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using EasyViewer.Dto;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Threading.Tasks;

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

                Parallel.ForEach(db.Tables.Cast<Table>(),
                    () => new List<ForeignKeyMetaData>(),
                    (item, loopBody, localState) =>
                    {
                        foreach (ForeignKey f in item.ForeignKeys)
                        {
                            foreach (ForeignKeyColumn data in f.Columns)
                            {
                                localState.Add(new ForeignKeyMetaData(data.Name, item.Name, item.Schema,
                                    f.ReferencedTable, data.ReferencedColumn, f.ReferencedTableSchema));
                            }
                        }
                        return localState;
                    },
                    (localState) =>
                    {
                        lock (lst)
                        {
                            lst.AddRange(localState);
                        }
                    });
            }

            return lst;
        }

        public async Task<List<string>> ExecuteQuery(string dataBase, string query)
        {
            var items = new List<string>();
            string connectionString = _connectionInfoService.GetConnectionString(dataBase);

            using (var conn = new SqlConnection(connectionString))
            using(SqlCommand command = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        items.Add(reader.GetFieldValue<string>(0));
                    }
                    return items;
                }
            }
        }
    }
}