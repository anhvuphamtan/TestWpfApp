using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.ViewModels;

public class IntroductionViewModel : ViewModelBase
{
    private INavigationService _navigationService;
    public RelayCommand NavigateToGuidelineCommand { get; set; }


    public INavigationService NavigationService
    {
        get => _navigationService;
        set
        {
            _navigationService = value;
            OnPropertyChanged();
        }
    }

    public IntroductionViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        NavigateToGuidelineCommand = new RelayCommand(o => { NavigationService.NavigateTo<GuidelineViewModel>(); }, o => true);
    }
}
