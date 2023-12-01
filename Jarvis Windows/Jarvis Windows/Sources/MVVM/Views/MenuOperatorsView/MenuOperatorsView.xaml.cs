using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jarvis_Windows.Sources.MVVM.Views.MenuOperatorsView
{
    /// <summary>
    /// Interaction logic for MenuOperatorsView.xaml
    /// </summary>
    public partial class MenuOperatorsView : UserControl
    {
        public MenuOperatorsView()
        {
            InitializeComponent();
        }

        private void languageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem != null)
                {
                    string selectedItemContent = (comboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    Debug.WriteLine($"{selectedItemContent}");
                }
            }
        }
    }
}
