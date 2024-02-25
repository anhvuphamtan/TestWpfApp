using System.Threading.Tasks;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.MVVM.Views.MenuOperatorsView;

namespace Jarvis_Windows.Sources.MVVM.ViewModels;

public class MainViewModel : ViewModelBase
{
    private PopupDictionaryService _popupDictionaryService;
    private INavigationService? _navigationService;
    private UIElementDetector _uIElementDetector;
    private SendEventGA4 _sendEventGA4;
    public RelayCommand ShowMenuOperationsCommand { get; set; }
    public MenuOperatorsViewModel MenuOperatorsViewModel { get; set; }

    public PopupDictionaryService PopupDictionaryService
    {
        get { return _popupDictionaryService; }
        set
        {
            _popupDictionaryService = value;
            OnPropertyChanged();
        }
    }
    public INavigationService NavigationService
    {
        get => _navigationService;
        set
        {
            _navigationService = value;
            OnPropertyChanged();
        }
    } 

    public UIElementDetector UIElementDetector
    {
        get { return _uIElementDetector; }
        set
        {
            _uIElementDetector = value;
            OnPropertyChanged();
        }
    }

    public SendEventGA4 SendEventGA4
    {
        get { return _sendEventGA4; }
        set
        {
            _sendEventGA4 = value;
            OnPropertyChanged();
        }
    }


    public MainViewModel(INavigationService navigationService, PopupDictionaryService popupDictionaryService, UIElementDetector uIElementDetector, SendEventGA4 sendEventGA4)
    {
        NavigationService = navigationService;
        PopupDictionaryService = popupDictionaryService;
        UIElementDetector = uIElementDetector;
        SendEventGA4 = sendEventGA4;

        ShowMenuOperationsCommand = new RelayCommand(ExecuteShowMenuOperationsCommand, o => true);

        // ViewModel
        MenuOperatorsViewModel = new MenuOperatorsViewModel(popupDictionaryService, uIElementDetector, sendEventGA4);
    }

    public async void ExecuteShowMenuOperationsCommand(object obj)
    {
        bool _menuShowStatus = PopupDictionaryService.IsShowMenuOperations;
        PopupDictionaryService.ShowMenuOperations(!_menuShowStatus);
        PopupDictionaryService.ShowJarvisAction(false);

        if (_menuShowStatus == false)
        {
            await Task.Run(async () =>
            {
                // Some processing before the await (if needed)
                await Task.Delay(0); // This allows the method to yield to the caller

                await SendEventGA4.SendEvent("open_input_actions");
            });
        }
    }
}
