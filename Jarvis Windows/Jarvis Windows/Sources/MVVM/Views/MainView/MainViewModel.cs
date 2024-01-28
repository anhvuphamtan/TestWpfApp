using Jarvis_Windows.Sources.DataAccess.Network;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Constants;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using Jarvis_Windows.Sources.Utils.EventAggregator;
using System;
using System.Diagnostics;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Documents;
using Jarvis_Windows.Sources.MVVM.Models;
using System.Collections.Generic;
using System.IO;
using Jarvis_Windows.Sources.MVVM.Views.MainView;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace Jarvis_Windows.Sources.MVVM.ViewModels;

public class MainViewModel : ViewModelBase
{
    private INavigationService? _navigationService;
    private PopupDictionaryService _popupDictionaryService;
    private UIElementDetector _uIElementDetector;
    private bool _isSpinningJarvisIcon; // Spinning Jarvis icon
    private string _mainWindowInputText;
    private IKeyboardMouseEvents globalHook;

    private SendEventGA4 _sendEventGA4;
    public List<Language> Languages { get; set; }
    public RelayCommand ShowMenuOperationsCommand { get; set; }
    public RelayCommand HideMenuOperationsCommand { get; set; }
    public RelayCommand ReviseCommand { get; set; }
    public RelayCommand ShortenCommand { get; set; }
    public RelayCommand TranslateCommand { get; set; }
    public RelayCommand OpenSettingsCommand { get; set; }
    public RelayCommand QuitAppCommand { get; set; }

    public INavigationService NavigationService
    {
        get => _navigationService;
        set
        {
            _navigationService = value;
            OnPropertyChanged();
        }
    }

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

    // Spinning Jarvis icon
    public bool IsSpinningJarvisIcon
    {
        get { return _isSpinningJarvisIcon; }
        set
        {
            _isSpinningJarvisIcon = value;
            OnPropertyChanged();
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
        HideMenuOperationsCommand = new RelayCommand(o => { PopupDictionaryService.ShowMenuOperations(false); }, o => true);
        ReviseCommand = new RelayCommand(ExecuteReviseCommand, o => true);
        ShortenCommand = new RelayCommand(ExecuteShortenCommand, o => true);
        TranslateCommand = new RelayCommand(ExecuteTranslateCommand, o => true);
        OpenSettingsCommand = new RelayCommand(ExecuteOpenSettingsCommand, o => true);
        QuitAppCommand = new RelayCommand(ExecuteQuitAppCommand, o => true);

        string relativePath = Path.Combine("Appsettings", "Configs", "languages_supported.json");
        string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
        string jsonContent = "";
        jsonContent = File.ReadAllText(fullPath);

        Languages = JsonConvert.DeserializeObject<List<Language>>(jsonContent);

        //Register Acceccibility service
        UIElementDetector.SubscribeToElementFocusChanged();

        EventAggregator.LanguageSelectionChanged += OnLanguageSelectionChanged;

        // Checking App update here
        try
        {
            ExecuteCheckUpdate();
        }

        catch { }
        finally { ExecuteSendEventOpenMainWindow(); }

        globalHook = Hook.GlobalEvents();
        globalHook.MouseDown += GlobalHook_MouseDown;
    }

    private void OnLanguageSelectionChanged(object sender, EventArgs e)
    {
        ExecuteTranslateCommand(sender);
    }

    private void ExecuteQuitAppCommand(object obj)
    {
        Process.GetCurrentProcess().Kill();
        Task.Run(async () =>
        {
            // Some processing before the await (if needed)
            await Task.Delay(0); // This allows the method to yield to the caller

            await SendEventGA4.SendEvent("quit_app");
        });
    }

    private void ExecuteOpenSettingsCommand(object obj)
    {
        throw new NotImplementedException();
    }

    private void GlobalHook_MouseDown(object sender, MouseEventArgs e)
    {
        //if (e.Button == MouseButtons.Left && _popupDictionaryService.IsShowMenuOperations)
        //{
        //    PresentationSource source = PresentationSource.FromVisual(System.Windows.Application.Current.MainWindow);
        //    Point mousePos = source.CompositionTarget.TransformFromDevice.Transform(new Point(e.X, e.Y));
        //    // Point mousePos = new Point(e.X, e.Y);
        //    Point JarvisMenuPosition = UIElementDetector.PopupDictionaryService.MenuOperationsPosition;

        //    double X1 = JarvisMenuPosition.X;
        //    double Y1 = JarvisMenuPosition.Y;
        //    double X2 = X1 + 400;
        //    double Y2 = Y1 + 165;
        //    if ((mousePos.X < X1 || mousePos.X > X2 || mousePos.Y < Y1 || mousePos.Y > Y2) && (X1 != 0 && Y1 != 0))
        //        HideMenuOperationsCommand.Execute(null);
        //}
    }

    private async void ExecuteCheckUpdate()
    {
        // Checking App update here
        await SendEventGA4.CheckVersion();
    }

    private async void ExecuteSendEventOpenMainWindow()
    {
        // Starting app
        await SendEventGA4.SendEvent("open_main_window");
    }

    public async void ExecuteShowMenuOperationsCommand(object obj)
    {
        bool _menuShowStatus = PopupDictionaryService.IsShowMenuOperations;
        PopupDictionaryService.ShowMenuOperations(!_menuShowStatus);

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

    public async void ExecuteTranslateCommand(object obj)
    {
        try
        {
            bool _fromWindow = false;
            // Trigger here
            HideMenuOperationsCommand.Execute(null);
            IsSpinningJarvisIcon = true; // Start spinning animation

            var textFromElement = "";
            try { textFromElement = UIElementDetector.GetTextFromFocusingEditElement(); }
            catch
            {
                textFromElement = this.MainWindowInputText;
                _fromWindow = true;
            }

            if (textFromElement == "") return;

            var textFromAPI = await JarvisApi.Instance.TranslateHandler(textFromElement, PopupDictionaryService.TargetLangguage);

            if (textFromAPI == null)
            {
                Debug.WriteLine($"🆘🆘🆘 {ErrorConstant.translateError}");
                return;
            }

            if (_fromWindow != true) { UIElementDetector.SetValueForFocusingEditElement(textFromAPI ?? ErrorConstant.translateError); }
            else { MainWindowInputText = textFromAPI; }
        }
        catch { }
        finally
        {
            IsSpinningJarvisIcon = false; // Stop spinning animation
            await SendEventGA4.SendEvent("do_ai_action", new Tuple<string, string>("translate", PopupDictionaryService.TargetLangguage));
        }
    }

    private async void ExecuteReviseCommand(object obj)
    {
        try
        {
            bool _fromWindow = false;
            HideMenuOperationsCommand.Execute(null);
            IsSpinningJarvisIcon = true; // Start spinning animation

            var textFromElement = "";
            try { textFromElement = UIElementDetector.GetTextFromFocusingEditElement(); }
            catch
            {
                textFromElement = this.MainWindowInputText;
                _fromWindow = true;
            }

            var textFromAPI = await JarvisApi.Instance.ReviseHandler(textFromElement);

            if (textFromAPI == null)
            {
                Debug.WriteLine($"🆘🆘🆘 {ErrorConstant.reviseError}");
                return;
            }

            if (_fromWindow != true) { UIElementDetector.SetValueForFocusingEditElement(textFromAPI ?? ErrorConstant.reviseError); }
            else { MainWindowInputText = textFromAPI; }


        }
        catch { }
        finally
        {
            await SendEventGA4.SendEvent("do_ai_action", new Tuple<string, string>("revise", ""));
            IsSpinningJarvisIcon = false; // Stop spinning animation
        }
    }

    private async void ExecuteShortenCommand(object obj)
    {
        try
        {
            bool _fromWindow = false;
            HideMenuOperationsCommand.Execute(null);
            IsSpinningJarvisIcon = true; // Start spinning animation

            var textFromElement = "";
            try { textFromElement = UIElementDetector.GetTextFromFocusingEditElement(); }
            catch
            {
                textFromElement = this.MainWindowInputText;
                _fromWindow = true;
            }

            var textFromAPI = await JarvisApi.Instance.ShortenHandler(textFromElement);

            if (textFromAPI == null)
            {
                Debug.WriteLine($"🆘🆘🆘 {ErrorConstant.shortennError}");
                return;
            }

            if (_fromWindow != true) { UIElementDetector.SetValueForFocusingEditElement(textFromAPI ?? ErrorConstant.shortennError); }
            else { MainWindowInputText = textFromAPI; }
        }
        catch { }
        finally
        {
            await SendEventGA4.SendEvent("do_ai_action", new Tuple<string, string>("shorten", ""));
            IsSpinningJarvisIcon = false; // Stop spinning animation
        }
    }
}
