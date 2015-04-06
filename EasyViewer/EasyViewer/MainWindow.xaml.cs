using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EasyViewer.Dto;
using EasyViewer.ViewModel;

namespace EasyViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void DbDataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs eventArgs)
        {
            DataGrid grid = sender as DataGrid;
            DataGridAutoGenerateCommandArgs commandArgs = new DataGridAutoGenerateCommandArgs(grid, eventArgs);
            var vm = ((MainViewModel)this.DataContext);
            if (vm.AutoGenerateColumn.CanExecute(commandArgs))
                vm.AutoGenerateColumn.Execute(commandArgs);
        }

        private void DbDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}