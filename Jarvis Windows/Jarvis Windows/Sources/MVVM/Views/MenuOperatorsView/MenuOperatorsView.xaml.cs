using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.MVVM.ViewModels;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.Utils.EventAggregator;
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
using System.Windows.Threading;

namespace Jarvis_Windows.Sources.MVVM.Views.MenuOperatorsView
{
    /// <summary>
    /// Interaction logic for MenuOperatorsView.xaml
    /// </summary>
    public partial class MenuOperatorsView : UserControl
    {
        private int _languageSelectedIndex = 14;
        private bool isInit = false;
        public MenuOperatorsView()
        {
            InitializeComponent();

            languageComboBox.Loaded += (sender, e) =>
            {
                if (languageComboBox.Items.Count > 0)
                {
                    languageComboBox.SelectedIndex = _languageSelectedIndex;
                }
            };
        }

        private void languageComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                if (comboBox.SelectedItem != null)
                {
                    PopupDictionaryService.TargetLangguage = ((Language)comboBox.SelectedItem).Value;
                    _languageSelectedIndex = comboBox.SelectedIndex;

                    comboBox.IsDropDownOpen = false;
                    if (isInit)
                        EventAggregator.PublishLanguageSelectionChanged(this, EventArgs.Empty);
                    
                    isInit = true;
                }
            }
        }


    }
}
