using Jarvis_Windows.Sources.DataAccess.Network;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Constants;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Diagnostics;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Documents;
using Jarvis_Windows.Sources.MVVM.Models;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.IO;
using System.Windows.Media.Animation;

namespace Jarvis_Windows.Sources.MVVM.ViewModels;

public class MainViewModel : ViewModelBase
{
    private INavigationService? _navigationService;
    private PopupDictionaryService _popupDictionaryService;
    private UIElementDetector _uIElementDetector;
    private bool _isSpinningJarvisIcon; // Spinning Jarvis icon
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

    public MainViewModel(INavigationService navigationService, PopupDictionaryService popupDictionaryService, UIElementDetector uIElementDetector)
    {
        NavigationService = navigationService;
        PopupDictionaryService = popupDictionaryService;
        UIElementDetector = uIElementDetector;
        ShowMenuOperationsCommand = new RelayCommand(o => { PopupDictionaryService.ShowMenuOperations(true); }, o => true);
        HideMenuOperationsCommand = new RelayCommand(o => { PopupDictionaryService.ShowMenuOperations(false); }, o => true);
        ReviseCommand = new RelayCommand(ExecuteReviseCommand, o => true);
        ShortenCommand = new RelayCommand(ExecuteShortenCommand, o => true);
        TranslateCommand = new RelayCommand(ExecuteTranslateCommand, o => true);
        OpenSettingsCommand = new RelayCommand(ExecuteOpenSettingsCommand, o => true);
        QuitAppCommand = new RelayCommand(ExecuteQuitAppCommand, o => true);

        string jsonContent = File.ReadAllText("../../../Appsettings/Configs/languages_supported.json");
        Languages = JsonConvert.DeserializeObject<List<Language>>(jsonContent);

        //Register Acceccibility service
        UIElementDetector.SubscribeToElementFocusChanged();
    }

    private void ExecuteQuitAppCommand(object obj)
    {
        Application.Current.Shutdown();
    }

    private void ExecuteOpenSettingsCommand(object obj)
    {
        throw new NotImplementedException();
    }

    private async void ExecuteTranslateCommand(object obj)
    {
        try
        {
            IsSpinningJarvisIcon = true; // Start spinning animation
            var textFromElement = UIElementDetector.GetTextFromFocusingEditElement();
            var textFromAPI = await JarvisApi.TranslateHandler(textFromElement, PopupDictionaryService.TargetLangguage);
            UIElementDetector.SetValueForFocusingEditElement(textFromAPI ?? ErrorConstant.translateError);
        }
        catch { }
        finally
        {
            IsSpinningJarvisIcon = false; // Stop spinning animation
        }
    }

    private async void ExecuteReviseCommand(object obj)
    {
        try
        {
            IsSpinningJarvisIcon = true; // Start spinning animation
            var textFromElement = UIElementDetector.GetTextFromFocusingEditElement();
            var textFromAPI = await JarvisApi.ReviseHandler(textFromElement);
            UIElementDetector.SetValueForFocusingEditElement(textFromAPI ?? ErrorConstant.reviseError);
        }
        catch { }
        finally
        {
            IsSpinningJarvisIcon = false; // Stop spinning animation
        }
    }

    private async void ExecuteShortenCommand(object obj)
    {
        try
        {
            IsSpinningJarvisIcon = true; // Start spinning animation
            //_popupDictionaryService.ShowMenuOperations(false);
            var textFromElement = UIElementDetector.GetTextFromFocusingEditElement();
            var textFromAPI = await JarvisApi.ShortenHandler(textFromElement);
            UIElementDetector.SetValueForFocusingEditElement(textFromAPI ?? ErrorConstant.shortennError);
        }
        catch { }
        finally
        {
            IsSpinningJarvisIcon = false; // Stop spinning animation
        }
    }
}
