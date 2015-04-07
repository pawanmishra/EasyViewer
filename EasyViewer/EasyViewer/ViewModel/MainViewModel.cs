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
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        private void ExecuteDbQuery()
        {
            var data = _dataService.FetchQueryData("TSQL2012", "Orders",
                "select top(2) * from sales.orders");
            AddQueryData(data);
        }

        /// <summary>
        /// Autogenerate column handler used for highlighting foreign key columns
        /// </summary>
        private void AutoGenerateColumnHandler(DataGridAutoGenerateCommandArgs e)
        {
            ForeignKeyMetaData data = null;
            List<ForeignKeyMetaData> metaData;
            if (_metaData.ContainsKey("TSQL2012"))
            {
                metaData = _metaData["TSQL2012"];
            }
            else
            {
                metaData = _dataService.GetForeignKeyMetaData("TSQL2012");
                _metaData.Add("TSQL2012", metaData);
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
            var frameworkMetaData = _metaData["TSQL2012"];
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