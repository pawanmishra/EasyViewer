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
            var grid = sender as DataGrid;
            var commandArgs = new DataGridAutoGenerateCommandArgs(grid, eventArgs);
            var vm = ((MainViewModel)this.DataContext);
            if (vm.AutoGenerateColumn.CanExecute(commandArgs))
                vm.AutoGenerateColumn.Execute(commandArgs);
        }

        private void DbDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs eventArgs)
        {
            var grid = sender as DataGrid;
            var commandArgs = new DataGridDoubleClickCommandArgs(grid, eventArgs);
            var vm = ((MainViewModel)this.DataContext);
            if (vm.DoubleClickCommand.CanExecute(commandArgs))
                vm.DoubleClickCommand.Execute(commandArgs);
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var grid = sender as MenuItem;
            var commandArgs = new ContextMenuItemCommandArgs(grid, e);
            var vm = ((MainViewModel)this.DataContext);
            if (vm.ContextMenuCommand.CanExecute(commandArgs))
                vm.ContextMenuCommand.Execute(commandArgs);
        }
    }
}