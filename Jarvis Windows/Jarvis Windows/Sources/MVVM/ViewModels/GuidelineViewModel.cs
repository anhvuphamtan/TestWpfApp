using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Jarvis_Windows.Sources.MVVM.ViewModels;

public class GuidelineViewModel : ViewModelBase
{
    private INavigationService _navigationService;
    public RelayCommand NavigateToIntroductionCommand { get; set; }
    public RelayCommand StartJarvisServiceCommand { get; set; }

    public INavigationService NavigationService
    {
        get => _navigationService;
        set
        {
            _navigationService = value;
            OnPropertyChanged();
        }
    }

    public UIElementDetector UIElementDetector { get; }

    public GuidelineViewModel(INavigationService navigationService, UIElementDetector uIElementDetector)
    {
        NavigationService = navigationService;
        UIElementDetector = uIElementDetector;
        NavigateToIntroductionCommand = new RelayCommand(o => { NavigationService.NavigateTo<IntroductionViewModel>(); }, o => true);
        StartJarvisServiceCommand = new RelayCommand(o => { 
            Application.Current.MainWindow.Hide();
            UIElementDetector.SubscribeToElementFocusChanged(); }, o => true);
    }
}
