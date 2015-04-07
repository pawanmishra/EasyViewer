using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private readonly IViewerService _viewerService;

        public RelayCommand ExecuteQuery { get; private set; }
        public RelayCommand<DataGridAutoGenerateCommandArgs> AutoGenerateColumn { get; private set; }
        public RelayCommand<DataGridDoubleClickCommandArgs> DoubleClickCommand { get; private set; }
        public RelayCommand<ContextMenuItemCommandArgs> ContextMenuCommand { get; private set; }
        public RelayCommand<int> RemoveTableFromGridCommand { get; private set; } 
        public ObservableCollection<QueryData> DataItems { get; set; }

        private readonly Dictionary<string, List<ForeignKeyMetaData>> _metaData;


        public ObservableCollection<String> DataTables { get; set; } 
        public IList<String> DataBases { get; set; }

        private string _chosenDB ;
        public String ChosenDB
        {
            get { return _chosenDB; }
            set
            {
                if (_chosenDB != value)
                {
                    _chosenDB = value;
                    FetchDataTablesQuery(value);
                }
            }
        }

        private string _queryString;
        public String QueryString
        {
            get { return _queryString; }
            set
            {
                if (_queryString != value)
                {
                    _queryString = value;
                }
            }
        }

        public MainViewModel(IDataService dataService, IViewerService viewerService)
        {
            _dataService = dataService;
            _viewerService = viewerService;
            _metaData = new Dictionary<string, List<ForeignKeyMetaData>>();
            ExecuteQuery = new RelayCommand(ExecuteDbQuery);
            AutoGenerateColumn = new RelayCommand<DataGridAutoGenerateCommandArgs>(AutoGenerateColumnHandler);
            DoubleClickCommand = new RelayCommand<DataGridDoubleClickCommandArgs>(DataGridDoubleClickHandler);
            ContextMenuCommand = new RelayCommand<ContextMenuItemCommandArgs>(ContextMenuHandler);
            RemoveTableFromGridCommand = new RelayCommand<int>(RemoveTableFromGrid);
            DataItems = new ObservableCollection<QueryData>();
            DataTables = new ObservableCollection<string>();
            DataBases = new List<string>();

           FetchDatabasesQuery();
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        public void FetchDataTablesQuery(string DB)
        {
            var data = _dataService.FetchQueryData(DB, "INFORMATION_SCHEMA.TABLES", "SELECT Table_schema +'.'+TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'");
            var dataTable = data.QueryDataTable;
            var datatableRow = dataTable.Rows;
            foreach (DataRow dr in datatableRow)
            {

                var a = dr.ItemArray[0];
                DataTables.Add(a.ToString());
            }
        }

        public void FetchDatabasesQuery()
        {
            var data = _dataService.FetchQueryData("master","sys.databases", "select name from sys.databases order by name");
            var dataTable = data.QueryDataTable;
            var datatableRow = dataTable.Rows;
            foreach (DataRow dr in datatableRow)
            {

                var a = dr.ItemArray[0];
                DataBases.Add(a.ToString());
            }
        }

        private void ExecuteDbQuery()
        {
            var data = _dataService.FetchQueryData(ChosenDB, "SalesOrderDetail", "select top(2) * from sales.SalesOrderDetail");
            AddQueryData(data);
        }

        /// <summary>
        /// Autogenerate column handler used for highlighting foreign key columns
        /// </summary>
        private void AutoGenerateColumnHandler(DataGridAutoGenerateCommandArgs e)
        {
            ForeignKeyMetaData data = null;
            List<ForeignKeyMetaData> metaData;
            if (_metaData.ContainsKey(ChosenDB))
            {
                metaData = _metaData[ChosenDB];
            }
            else
            {
                metaData = _dataService.GetForeignKeyMetaData(ChosenDB);
                _metaData.Add(ChosenDB, metaData);
            }

            data = metaData.FirstOrDefault(x => x.CurrentColumn ==
                    e.ColumnEventArgs.PropertyName && x.CurrentTable.Equals(e.Grid.Tag.ToString()));

            if (data != null)
            {
                Style customStyle = (Style)e.Grid.FindResource("customStyle");
                e.ColumnEventArgs.Column.CellStyle = customStyle;
            }
        }

        /// <summary>
        /// Datagrid row double click handler
        /// </summary>
        private void DataGridDoubleClickHandler(DataGridDoubleClickCommandArgs args)
        {
            var frameworkMetaData = _metaData[ChosenDB];
            var returnedData = _viewerService.ProcessGridDoubleClick(args, frameworkMetaData);
            if (returnedData != null)
            {   
                AddQueryData(returnedData);
            }
        }

        /// <summary>
        /// Handle the DataGrid row right click delete menu handler
        /// </summary>
        /// <param name="args"></param>
        private void ContextMenuHandler(ContextMenuItemCommandArgs args)
        {
            var contextMenu = (ContextMenu)args.MenuItem.Parent;
            var item = (DataGrid)contextMenu.PlacementTarget;
            var tableName = item.Tag.ToString();
            var selectedIndex = item.SelectedIndex;
            QueryData queryData = DataItems.FirstOrDefault(x => x.TableName.Equals(tableName));

            if (queryData != null)
            {
                queryData.QueryDataTable.Rows.RemoveAt(selectedIndex);
            }
        }

        /// <summary>
        /// Handler for removing data grid of a given table
        /// </summary>
        /// <param name="counter"></param>
        private void RemoveTableFromGrid(int counter)
        {
            var item = DataItems.First(x => x.Counter == counter);
            DataItems.Remove(item);
        }

        private void AddQueryData(QueryData data)
        {
            var items = DataItems.ToList();
            var index = items.FindIndex(x => x.TableName.Equals(data.TableName));
            if (index < 0)
            {
                data.Counter = Counter++;
                DataItems.Add(data);
            }
            else
            {
                var querydata = DataItems[index];
                foreach (DataRow row in data.QueryDataTable.Rows)
                {
                    querydata.QueryDataTable.ImportRow(row);
                }
            }
        }
    }
}