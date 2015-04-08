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
        private readonly IMasterDataService _masterDataService;
        private readonly IConnectionInfoService _connectionInfoService;

        public RelayCommand<String> ExecuteQuery { get; private set; }
        public RelayCommand<DataGridAutoGenerateCommandArgs> AutoGenerateColumn { get; private set; }
        public RelayCommand<DataGridDoubleClickCommandArgs> DoubleClickCommand { get; private set; }
        public RelayCommand<ContextMenuItemCommandArgs> ContextMenuCommand { get; private set; }
        public RelayCommand<int> RemoveTableFromGridCommand { get; private set; }
        public RelayCommand<SqlInstanceConnectionInfo> ConnectSqlInstanceCommand { get; set; } 

        public ObservableCollection<QueryData> DataItems { get; set; }
        public ObservableCollection<String> DataBases { get; set; }
        public ObservableCollection<String> Tables { get; set; } 
        public Dictionary<String, IEnumerable<String>> DataTablesDictionary { get; set; } 

        private readonly Dictionary<string, List<ForeignKeyMetaData>> _metaData;

        public MainViewModel(IDataService dataService, IViewerService viewerService,
            IMasterDataService masterDataService, IConnectionInfoService connectionInfoService)
        {
            _dataService = dataService;
            _viewerService = viewerService;
            _masterDataService = masterDataService;
            _connectionInfoService = connectionInfoService;

            _metaData = new Dictionary<string, List<ForeignKeyMetaData>>();
            ExecuteQuery = new RelayCommand<string>(ExecuteDbQuery);
            AutoGenerateColumn = new RelayCommand<DataGridAutoGenerateCommandArgs>(AutoGenerateColumnHandler);
            DoubleClickCommand = new RelayCommand<DataGridDoubleClickCommandArgs>(DataGridDoubleClickHandler);
            ContextMenuCommand = new RelayCommand<ContextMenuItemCommandArgs>(ContextMenuHandler);
            RemoveTableFromGridCommand = new RelayCommand<int>(RemoveTableFromGrid);
            ConnectSqlInstanceCommand = new RelayCommand<SqlInstanceConnectionInfo>(ConnectToSqlInstance);

            DataItems = new ObservableCollection<QueryData>();
            DataTablesDictionary = new Dictionary<string, IEnumerable<string>>();
            DataBases = new ObservableCollection<string>();
            Tables = new ObservableCollection<string>();
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        private string _chosenDb;
        public String ChosenDb
        {
            get { return _chosenDb; }
            set
            {
                if (_chosenDb != value)
                {
                    _chosenDb = value;
                    FetchDataTablesQuery(value);
                }
            }
        }

        private string _chosenTable;
        public String ChosenTable
        {
            get { return _chosenTable; }
            set
            {
                if (value != null && _chosenTable != value)
                {
                    _chosenTable = value;
                }
            }
        }

        public string RemoteInstancePassword { get; set; }

        private void ConnectToSqlInstance(SqlInstanceConnectionInfo connectionInfo)
        {
            connectionInfo.Password = RemoteInstancePassword;
            _connectionInfoService.InitializeConnectionString(connectionInfo);
            ResetApp();
            FetchDatabasesQuery();
        }

        private void ResetApp()
        {
            DataItems.Clear();
            Tables.Clear();
            DataBases.Clear();
        }

        /// <summary>
        /// Fetach all the tables for a given database
        /// </summary>
        /// <param name="database">database name for which list of tables have to be retrieved</param>
        public void FetchDataTablesQuery(string database)
        {
            Tables.Clear();
            if (!DataTablesDictionary.ContainsKey(database))
            {
                DataTablesDictionary.Add(database, _masterDataService.GetAllTablesForGivenDatabase(database));
            }

            foreach (var item in DataTablesDictionary[database])
            {
                Tables.Add(item);
            }
        }

        /// <summary>
        /// Featch all the database for a given SQL Server Instance
        /// </summary>
        public void FetchDatabasesQuery()
        {
            foreach (var item in _masterDataService.GetAllDatabases())
            {
                DataBases.Add(item);
            }
        }

        private void ExecuteDbQuery(string query)
        {
            var data = _dataService.FetchQueryData(ChosenDb, ChosenTable, query);
            AddQueryData(data);
        }

        /// <summary>
        /// Autogenerate column handler used for highlighting foreign key columns
        /// </summary>
        private void AutoGenerateColumnHandler(DataGridAutoGenerateCommandArgs e)
        {
            ForeignKeyMetaData data = null;
            List<ForeignKeyMetaData> metaData;
            if (_metaData.ContainsKey(ChosenDb))
            {
                metaData = _metaData[ChosenDb];
            }
            else
            {
                metaData = _dataService.GetForeignKeyMetaData(ChosenDb);
                _metaData.Add(ChosenDb, metaData);
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
            var frameworkMetaData = _metaData[ChosenDb];
            var returnedData = _viewerService.ProcessGridDoubleClick(args, frameworkMetaData, ChosenDb);
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