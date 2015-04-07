using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyViewer.Dto;

namespace EasyViewer.Model
{
    public class ViewerService : IViewerService
    {
        private readonly IDataService _dataService;
        private const string QueryFormat = "select * from {0} where {1} = {2}";

        public ViewerService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public QueryData ProcessGridDoubleClick(DataGridDoubleClickCommandArgs args, List<ForeignKeyMetaData> metaData, string database)
        {
            var tableName = args.Grid.Tag.ToString();
            var columnName = args.Grid.CurrentCell.Column.Header.ToString();
            var columnIndex = args.Grid.CurrentCell.Column.DisplayIndex;
            var dataRowView = args.Grid.CurrentItem as DataRowView;
            if (dataRowView != null)
            {
                var columnValue = dataRowView.Row.ItemArray[columnIndex];
                var data = metaData.FirstOrDefault(x => x.CurrentColumn.Equals(columnName) && x.CurrentTable == tableName);

                if (data != null)
                {
                    var returnedData = _dataService.FetchQueryData(database, data.ReferencedTable,
                        string.Format(QueryFormat, data.ReferencedTable, data.ReferencedColumn, columnValue));
                    return returnedData;
                }
            }

            return null;
        }
    }
}
