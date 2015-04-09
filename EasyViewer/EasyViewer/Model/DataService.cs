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

                foreach (Table table in db.Tables)
                {
                    foreach (ForeignKey f in table.ForeignKeys)
                    {
                        foreach (ForeignKeyColumn data in f.Columns)
                        {
                            lst.Add(new ForeignKeyMetaData(data.Name, table.Name, table.Schema,
                                f.ReferencedTable, data.ReferencedColumn, f.ReferencedTableSchema, table.Columns[data.Name].DataType.Name));
                        }
                    }
                }
            }

            return lst;
        }

        public async Task<List<ForeignKeyMetaData>> GetKeyMetaData(string dataBase)
        {
            var lst = new List<ForeignKeyMetaData>();
            string query = @"SELECT  
    col1.name AS [column],
    tab1.name AS [table],
    sch1.name AS [schema_name],
    tab2.name AS [referenced_table],
    col2.name AS [referenced_column],
    sch2.name as [ref_schema_name],
    schmecols.DATA_TYPE as type
FROM sys.foreign_key_columns fkc
INNER JOIN sys.tables tab1
    ON tab1.object_id = fkc.parent_object_id
INNER JOIN sys.schemas sch1
    ON tab1.schema_id = sch1.schema_id
INNER JOIN sys.columns col1
    ON col1.column_id = parent_column_id AND col1.object_id = tab1.object_id
INNER JOIN sys.tables tab2
    ON tab2.object_id = fkc.referenced_object_id
INNER JOIN sys.schemas sch2
    ON tab2.schema_id = sch2.schema_id
INNER JOIN sys.columns col2
    ON col2.column_id = referenced_column_id AND col2.object_id = tab2.object_id
inner join INFORMATION_SCHEMA.COLUMNS schmecols
    on schmecols.COLUMN_NAME = col1.name and schmecols.TABLE_NAME = tab1.name";

            string connectionString = _connectionInfoService.GetConnectionString(dataBase);

            using (var conn = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lst.Add(new ForeignKeyMetaData(
                            reader.GetFieldValue<string>(0), 
                            reader.GetFieldValue<string>(1), 
                            reader.GetFieldValue<string>(2),
                            reader.GetFieldValue<string>(3), 
                            reader.GetFieldValue<string>(4), 
                            reader.GetFieldValue<string>(5),
                            reader.GetFieldValue<string>(6)));
                    }
                    return lst;
                }
            }
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