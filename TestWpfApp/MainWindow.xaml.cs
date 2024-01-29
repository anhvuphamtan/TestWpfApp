using System.Windows;
using System.Windows.Controls;

namespace TestWpfApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Set the DataContext to an instance of your MainViewModel
            DataContext = new MainViewModel();
        }
    }
}
