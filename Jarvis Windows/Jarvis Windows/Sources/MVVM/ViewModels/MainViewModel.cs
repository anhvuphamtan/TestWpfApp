using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.ViewModels;

public class MainViewModel : ViewModelBase
{
    private INavigationService? _navigationService;

    public INavigationService NavigationService
    {
        get => _navigationService;
        set
        {
            _navigationService = value;
            OnPropertyChanged();
        }
    }
    public MainViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }
}
