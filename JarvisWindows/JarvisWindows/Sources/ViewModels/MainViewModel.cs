using JarvisWindows.Sources.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace JarvisWindows.Sources.ViewModels
{
    public class MainViewModel
    {
        public ICommand LaunchJarvisCommand { get; set; }

        public MainViewModel() 
        {
            LaunchJarvisCommand = new RelayCommand(StartNativeService, CanStartNativeService);
        }

        private bool CanStartNativeService(object obj)
        {
            //Add handling for "false" case
            return true;
        }

        private void StartNativeService(object obj)
        {
            //TODO:
            //Call function to start the native servive for listening, hooking event, etc

            //TODO:
            //Hide the main windows
            var mainWindows = obj as Window;
            if (mainWindows != null) { mainWindows.Hide(); }
        }
    }
}
