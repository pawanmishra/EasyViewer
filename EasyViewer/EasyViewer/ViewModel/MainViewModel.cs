using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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

        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            ExecuteQuery = new RelayCommand(ExecuteDbQuery);
            AutoGenerateColumn = new RelayCommand<DataGridAutoGenerateCommandArgs>(AutoGenerateColumnHandler);
            DoubleClickCommand = new RelayCommand<DataGridDoubleClickCommandArgs>(DataGridDoubleClickHandler);
            DataItems = new ObservableCollection<QueryData>();
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        public void ExecuteDbQuery()
        {
            var data = _dataService.FetchQueryData();
            DataItems.Add(data);
        }

        public void AutoGenerateColumnHandler(DataGridAutoGenerateCommandArgs e)
        {
            var foreignKeyMetadata = _dataService.GetForeignKeyMetaData();
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