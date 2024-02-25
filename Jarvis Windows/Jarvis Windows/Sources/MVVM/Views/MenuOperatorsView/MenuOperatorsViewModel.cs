using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jarvis_Windows.Sources.DataAccess;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.MVVM.Models;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.Utils.Constants;
using Jarvis_Windows.Sources.DataAccess.Network;
using Jarvis_Windows.Sources.Utils.Accessibility;


namespace Jarvis_Windows.Sources.MVVM.Views.MenuOperatorsView;

public class MenuOperatorsViewModel : ViewModelBase
{
    private PopupDictionaryService _popupDictionaryService;
    private UIElementDetector _uIElementDetector;
    private SendEventGA4 _sendEventGA4;

    private ObservableCollection<MenuButton> _fixedButtons;
    private ObservableCollection<MenuButton> _dynamicButtons;


    private bool _isTextEmpty;
    private string _filterText;
    private double _scrollBarHeight;
    private bool _isMainWindowFocus;
    private string _remainingAPIUsage;
    private bool _isSpinningJarvisIcon;
    private int _languageSelectedIndex;
    private string _mainWindowInputText;


    public RelayCommand HideMenuOperationsCommand { get; set; }
    public List<Language> MenuLanguages { get; set; }
    public RelayCommand ExpandCommand { get; set; }
    public RelayCommand AICommand { get; set; }
    public RelayCommand LanguageComboBoxCommand { get; set; }

    public PopupDictionaryService PopupDictionaryService
    {
        get { return _popupDictionaryService; }
        set
        {
            _popupDictionaryService = value;
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

    public ObservableCollection<MenuButton> FixedButtons
    {
        get { return _fixedButtons; }
        set
        {
            _fixedButtons = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<MenuButton> DynamicButtons
    {
        get { return _dynamicButtons; }
        set
        {
            _dynamicButtons = value;
            OnPropertyChanged();
        }
    }

    public bool IsTextEmpty
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FilterText)) _isTextEmpty = true;
            else _isTextEmpty = false;
            return _isTextEmpty;
        }
        set
        {
            _isTextEmpty = value;
            OnPropertyChanged();
        }
    }

    public string FilterText
    {
        get { return _filterText; }
        set
        {
            _filterText = value;
            UpdateButtonVisibility();
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsTextEmpty));
        }
    }

    public double ScrollBarHeight
    {
        get { return _scrollBarHeight; }
        set
        {
            _scrollBarHeight = value;
            OnPropertyChanged();
        }
    }

    public string RemainingAPIUsage
    {
        get { return _remainingAPIUsage; }
        set
        {
            _remainingAPIUsage = value;
            OnPropertyChanged();
        }
    }

    public bool IsSpinningJarvisIcon
    {
        get { return _isSpinningJarvisIcon; }
        set
        {
            _isSpinningJarvisIcon = value;
            OnPropertyChanged();
        }
    }

    public int LanguageSelectedIndex
    {
        get { return _languageSelectedIndex; }
        set
        {
            if (_languageSelectedIndex != value)
            {
                _languageSelectedIndex = value;
                OnPropertyChanged();
            }
        }
    }

    public string MainWindowInputText
    {
        get { return _mainWindowInputText; }
        set
        {
            _mainWindowInputText = value;
            OnPropertyChanged();
        }
    }

    public MenuOperatorsViewModel(PopupDictionaryService popupDictionaryService, UIElementDetector uIElementDetector, SendEventGA4 sendEventGA4)
    {
        PopupDictionaryService = popupDictionaryService;
        UIElementDetector = uIElementDetector;
        SendEventGA4 = sendEventGA4;
        
        RemainingAPIUsage = (AppStatus.IsPackaged)
                                ? $"{Windows.Storage.ApplicationData.Current.LocalSettings.Values["ApiUsageRemaining"]} 🔥"
                                : $"{DataConfiguration.ApiUsageRemaining} 🔥";

        HideMenuOperationsCommand = new RelayCommand(o => { PopupDictionaryService.ShowMenuOperations(false); }, o => true);

        AICommand = new RelayCommand(ExecuteAICommand, o => true);
        ExpandCommand = new RelayCommand(ExecuteExpandCommand, o => true);
        LanguageComboBoxCommand = new RelayCommand(OnLanguageSelectionChanged, o => true);

        string relativePath = Path.Combine("Appsettings", "Configs", "languages_supported.json");
        string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
        string jsonContent = "";
        jsonContent = File.ReadAllText(fullPath);

        MenuLanguages = JsonConvert.DeserializeObject<List<Language>>(jsonContent);
        LanguageSelectedIndex = 14;

        // Register Acceccibility service
        UIElementDetector.SubscribeToElementFocusChanged();
        
        
        try { ExecuteCheckUpdate(); }
        catch { }
        finally { ExecuteSendEventOpenMainWindow(); }

        InitializeButtons();

        EventAggregator.JarvisActionPositionChanged += OnJarvisActionPositionChanged; // Check whether text is in Main Window or in external applications
    }

    private void OnJarvisActionPositionChanged(object sender, EventArgs e)
    {
        // Check whether text is in Main Window or in external application
        string objID = (string) sender;
        _isMainWindowFocus = (objID == "GuidelineText");
    }
    public async void OnLanguageSelectionChanged(object sender)
    {
        AICommand.Execute("Translate it");
    }

    private async void ExecuteCheckUpdate()
    {
        await SendEventGA4.CheckVersion();
    }

    private async void ExecuteSendEventOpenMainWindow()
    {
        await SendEventGA4.SendEvent("open_main_window");
    }

    private async void ExecuteExpandCommand(object parameter)
    {
        DynamicButtons[1].Visibility = !DynamicButtons[1].Visibility;

        for (int i = 2; i < DynamicButtons.Count; i++)
            DynamicButtons[i].Visibility = !DynamicButtons[1].Visibility;

        UpdateButtonVisibility();
    }

    private async void ExecuteAICommand(object obj)
    {
        string _actionType = (string)obj;
        string _aiAction = "custom";
        try
        {
            HideMenuOperationsCommand.Execute(null);
            IsSpinningJarvisIcon = true; 
            PopupDictionaryService.ShowJarvisAction(true);

            var textFromElement = "";
            var textFromAPI = "";
            if (_isMainWindowFocus) textFromElement = this.MainWindowInputText; // faster ComboBox close than try catch
            else textFromElement = UIElementDetector.GetTextFromFocusingEditElement();       

            // if (textFromElement == "") return;

            if (_actionType == "Translate it")
            {
                string _targetLanguage = MenuLanguages[LanguageSelectedIndex].Value;
                textFromAPI = await JarvisApi.Instance.TranslateHandler(textFromElement, _targetLanguage);
                _aiAction = "translate";
            }

            else if (_actionType == "Revise it")
            {
                textFromAPI = await JarvisApi.Instance.ReviseHandler(textFromElement);
                _aiAction = "revise";
            }
            else if (_actionType == "Ask")
            {
                textFromAPI = await JarvisApi.Instance.AskHandler(textFromElement, FilterText);
                _aiAction = "ask";
            }

            else
                textFromAPI = await JarvisApi.Instance.AIHandler(textFromElement, _actionType);

            if (textFromAPI == null)
            {
                Debug.WriteLine($"🆘🆘🆘 {ErrorConstant.translateError}");
                return;
            }

            RemainingAPIUsage = (AppStatus.IsPackaged)
                                ? $"{Windows.Storage.ApplicationData.Current.LocalSettings.Values["ApiUsageRemaining"]} 🔥"
                                : $"{DataConfiguration.ApiUsageRemaining} 🔥";

            if (_isMainWindowFocus != true) { UIElementDetector.SetValueForFocusingEditElement(textFromAPI ?? ErrorConstant.translateError); }
            else { MainWindowInputText = textFromAPI; }
        }
        catch { }
        finally
        {
            IsSpinningJarvisIcon = false; 

            var eventParams = new Dictionary<string, object>
            {
                { "ai_action", _aiAction }
            };

            if (_aiAction == "translate")
                eventParams.Add("ai_action_translate_to", PopupDictionaryService.TargetLangguage);
            else if (_aiAction == "custom")
                eventParams.Add("ai_action_custom", _actionType);

            await SendEventGA4.SendEvent("do_ai_action", eventParams);
        }
    }

    private void UpdateButtonVisibility()
    {
        string _curFilterText = (string.IsNullOrEmpty(FilterText)) ? "" : FilterText.ToLower();
        double _currentHeight = 0;
        double _lineWidth = 0;

        foreach (var button in FixedButtons)
        {
            button.Visibility = (_curFilterText == "") || button.Content.ToLower().Contains(_curFilterText);
            button.Margin = new Thickness(0, 0, button.Visibility ? 10 : 0, button.Visibility ? 10 : 0);
            _lineWidth += (button.Visibility) ? (button.Width + 10) : 0;
        }

        if (_lineWidth > 0)
        {
            _lineWidth = 0;
            _currentHeight = 51;
        }

        foreach (var button in DynamicButtons)
        {
            int i = DynamicButtons.IndexOf(button);

            if (_curFilterText == "" && i >= 2) button.Visibility = !DynamicButtons[1].Visibility;
            else if (i != 1 && i < DynamicButtons.Count - 1)
                button.Visibility = button.Content.ToLower().Contains(_curFilterText);

            button.Margin = new Thickness(0, 0, button.Visibility ? 10 : 0, 10);
            _lineWidth += (button.Visibility) ? (button.Width + 10) : 0;

            if (_lineWidth > 376)
            {
                _lineWidth = button.Width + 10;
                _currentHeight += 51;
            }
        }

        if (_lineWidth > 0) { _currentHeight += 51; }

        _currentHeight = Math.Min(_currentHeight, 255);

        ScrollBarHeight = _currentHeight;

        OnPropertyChanged(nameof(FixedButtons));
        OnPropertyChanged(nameof(DynamicButtons));
    }

    private void InitializeButtons()
    {
        AIActionTemplate aIActionTemplate = new AIActionTemplate();
        DynamicButtons = aIActionTemplate.DynamicAIActionList;
        FixedButtons = aIActionTemplate.FixedAIActionList;

        foreach (var action in FixedButtons)
        {
            action.Command = new RelayCommand(ExecuteAICommand, o => true);
        }

        foreach (var action in DynamicButtons)
        {
            action.Command = (action.Content.Contains("More") || action.Content.Contains("Less"))
                ? new RelayCommand(ExecuteExpandCommand, o => true)
                : new RelayCommand(ExecuteAICommand, o => true);
        }

        UpdateButtonVisibility();
    }
}
