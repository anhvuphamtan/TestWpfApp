using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.ViewModels;

public class GuidelineViewModel : ViewModelBase
{
    private INavigationService _navigationService;
    public RelayCommand NavigateToIntroductionCommand { get; set; }

    public INavigationService NavigationService
    {
        get => _navigationService;
        set
        {
            _navigationService = value;
            OnPropertyChanged();
        }
    }

    public GuidelineViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        NavigateToIntroductionCommand = new RelayCommand(o => { NavigationService.NavigateTo<IntroductionViewModel>(); }, o => true);
    }
}
