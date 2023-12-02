using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class MenuOperatorsView : UserControl, INotifyPropertyChanged
    {
        private String _selectedLanguage;

        public MenuOperatorsView()
        {
            InitializeComponent();

            languageComboBox.Loaded += (sender, e) =>
            {
                if (languageComboBox.Items.Count > 0)
                {
                    languageComboBox.SelectedIndex = 0;
                }
            };
        }

        public String SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void languageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem != null)
                {
                    string selectedItemContent = ((Language)comboBox.SelectedItem).Name;
                    Debug.WriteLine($"{selectedItemContent}");
                }
            }
        }
    }
}
