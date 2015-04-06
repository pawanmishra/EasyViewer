using GalaSoft.MvvmLight;
using EasyViewer.Model;
using GalaSoft.MvvmLight.Command;

namespace EasyViewer.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        public RelayCommand ExecuteQuery { get; private set; }

        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            ExecuteQuery = new RelayCommand(ExecuteDbQuery);
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        public void ExecuteDbQuery()
        {
            
        }
    }
}