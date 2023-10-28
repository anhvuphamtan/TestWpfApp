using Jarvis_Windows.Sources.Views.MainView;
using Jarvis_Windows.Sources.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Jarvis_Windows.Sources.Views.MainView
{
    /// <summary>
    /// Interaction logic for MainWindows.xaml
    /// </summary>
    public partial class MainWindows : Window
    {
        public MainWindows()
        {
            InitializeComponent();
            string apiUrl = DataConfiguration.ApiUrl;
            MainViewModel mainViewModel = new MainViewModel();
            this.DataContext = mainViewModel;
        }
    }
}
