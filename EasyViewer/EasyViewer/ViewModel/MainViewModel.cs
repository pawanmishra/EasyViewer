using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using EasyViewer.Dto;
using GalaSoft.MvvmLight;
using EasyViewer.Model;
using GalaSoft.MvvmLight.Command;

namespace EasyViewer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public static int Counter = 0;
        private readonly IDataService _dataService;

        public RelayCommand ExecuteQuery { get; private set; }
        public RelayCommand<DataGridAutoGenerateCommandArgs> AutoGenerateColumn { get; private set; }
        public RelayCommand<DataGridDoubleClickCommandArgs> DoubleClickCommand { get; private set; }
        public RelayCommand<int> RemoveTableFromGridCommand { get; private set; } 
        public ObservableCollection<QueryData> DataItems { get; set; }

        private Dictionary<string, List<ForeignKeyMetaData>> _metaData;

        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _metaData = new Dictionary<string, List<ForeignKeyMetaData>>();
            ExecuteQuery = new RelayCommand(ExecuteDbQuery);
            AutoGenerateColumn = new RelayCommand<DataGridAutoGenerateCommandArgs>(AutoGenerateColumnHandler);
            DoubleClickCommand = new RelayCommand<DataGridDoubleClickCommandArgs>(DataGridDoubleClickHandler);
            RemoveTableFromGridCommand = new RelayCommand<int>(RemoveTableFromGrid);
            DataItems = new ObservableCollection<QueryData>();
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        public void ExecuteDbQuery()
        {
            var data = _dataService.FetchQueryData("TimeTracker", "TimeTrackerHistory",
                "select top(2) * from TimeTrackerHistory");
            data.Counter = Counter++;
            DataItems.Add(data);
        }

        public void AutoGenerateColumnHandler(DataGridAutoGenerateCommandArgs e)
        {
            ForeignKeyMetaData data = null;

            if (_metaData.ContainsKey("TimeTracker"))
            {
                var foreignKeyMetadata = _metaData["TimeTracker"];
                data = foreignKeyMetadata.FirstOrDefault(x => x.CurrentColumn == 
                    e.ColumnEventArgs.PropertyName && x.CurrentTable.Equals(e.Grid.Tag.ToString()));
            }
            else
            {
                var foreignKeyMetadata = _dataService.GetForeignKeyMetaData("TimeTracker");
                _metaData.Add("TimeTracker", foreignKeyMetadata);
                data = foreignKeyMetadata.FirstOrDefault(x => x.CurrentColumn == e.ColumnEventArgs.PropertyName
                    && x.CurrentTable.Equals(e.Grid.Tag.ToString()));
            }

            if (data != null)
            {
                Style customStyle = (Style)e.Grid.FindResource("customStyle");
                e.ColumnEventArgs.Column.CellStyle = customStyle;
            }
        }

        public void DataGridDoubleClickHandler(DataGridDoubleClickCommandArgs args)
        {
            string queryFmt = "select * from {0} where {1} = {2}";
            var frameworkMetaData = _metaData["TimeTracker"];
            var tableName = args.Grid.Tag.ToString();
            var columnName = args.Grid.CurrentCell.Column.Header.ToString();
            var columnIndex = args.Grid.CurrentCell.Column.DisplayIndex;
            var dataRowView = args.Grid.CurrentItem as DataRowView;
            if (dataRowView != null)
            {
                var columnValue = dataRowView.Row.ItemArray[columnIndex];

                var data =
                    frameworkMetaData.FirstOrDefault(x => x.CurrentColumn.Equals(columnName) && x.CurrentTable == tableName);

                if (data != null)
                {
                    var returnedData = _dataService.FetchQueryData("TimeTracker", data.ReferencedTable, 
                        string.Format(queryFmt, data.ReferencedTable, data.ReferencedColumn, columnValue));
                    returnedData.Counter = Counter++;
                    DataItems.Add(returnedData);
                }
            }
        }

        public void RemoveTableFromGrid(int counter)
        {
            var item = DataItems.First(x => x.Counter == counter);
            DataItems.Remove(item);
        }
    }
}