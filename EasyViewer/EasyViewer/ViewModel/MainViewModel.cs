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
        public List<String> DataBases { get; set; }

        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            ExecuteQuery = new RelayCommand(ExecuteDbQuery);
            AutoGenerateColumn = new RelayCommand<DataGridAutoGenerateCommandArgs>(AutoGenerateColumnHandler);
            DoubleClickCommand = new RelayCommand<DataGridDoubleClickCommandArgs>(DataGridDoubleClickHandler);
            DataItems = new ObservableCollection<QueryData>();
            DataBases = new List<string>();

           FetchDatabasesQuery();
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        public void FetchDatabasesQuery()
        {
            var data = _dataService.FetchQueryData("master", "select name from sys.databases");
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
            var data = _dataService.FetchQueryData("", "");
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