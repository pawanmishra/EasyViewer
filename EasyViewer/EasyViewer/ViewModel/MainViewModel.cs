using System;
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
        private readonly IDataService _dataService;

        public RelayCommand ExecuteQuery { get; private set; }
        public RelayCommand<DataGridAutoGenerateCommandArgs> AutoGenerateColumn { get; private set; }
        public RelayCommand<DataGridDoubleClickCommandArgs> DoubleClickCommand { get; private set; } 
        public ObservableCollection<QueryData> DataItems { get; set; }
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

        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            ExecuteQuery = new RelayCommand(ExecuteDbQuery);
            AutoGenerateColumn = new RelayCommand<DataGridAutoGenerateCommandArgs>(AutoGenerateColumnHandler);
            DoubleClickCommand = new RelayCommand<DataGridDoubleClickCommandArgs>(DataGridDoubleClickHandler);
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

        public void FetchDataTablesQuery(string value)
        {
            var data = _dataService.FetchQueryData(value, "SELECT Table_schema +'.'+TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'");
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
            var data = _dataService.FetchQueryData("master", "select name from sys.databases order by name");
            var dataTable = data.QueryDataTable;
            var datatableRow = dataTable.Rows;
            foreach (DataRow dr in datatableRow)
            {

                var a = dr.ItemArray[0];
                DataBases.Add(a.ToString());
            }
        }

        public void ExecuteDbQuery()
        {
            var data = _dataService.FetchQueryData(ChosenDB, QueryString);
            DataItems.Add(data);
        }

        public void AutoGenerateColumnHandler(DataGridAutoGenerateCommandArgs e)
        {
            var foreignKeyMetadata = _dataService.GetForeignKeyMetaData("");
            ForeignKeyMetaData data = foreignKeyMetadata.FirstOrDefault(x => x.CurrentColumn == e.ColumnEventArgs.PropertyName);

            if (data != null)
            {
                Style customStyle = (Style)e.Grid.FindResource("customStyle");
                e.ColumnEventArgs.Column.CellStyle = customStyle;
            }
        }

        public void DataGridDoubleClickHandler(DataGridDoubleClickCommandArgs args)
        {
            
        }
    }
}