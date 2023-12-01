using Jarvis_Windows.Sources.MVVM.ViewModels;
using Jarvis_Windows.Sources.MVVM.Views.MainView;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace Jarvis_Windows;

public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    public App()
    {
        IServiceCollection services = new ServiceCollection();
        
        services.AddSingleton<Func<Type, ViewModelBase>>(serviceProvider => viewModelType => (ViewModelBase)serviceProvider.GetRequiredService(viewModelType));
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<PopupDictionaryService>();
        services.AddSingleton<UIElementDetector>(provider => new UIElementDetector
        {
            PopupDictionaryService = _serviceProvider.GetRequiredService<PopupDictionaryService>()
        });
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainView>(proider => new MainView
        {
            DataContext = proider.GetRequiredService<MainViewModel>()
        });
        services.AddSingleton<IntroductionViewModel>();
        services.AddSingleton<GuidelineViewModel>();
        services.AddSingleton<JarvisActionViewModel>(provider => new JarvisActionViewModel
        {
            PopupDictionaryService = _serviceProvider.GetRequiredService<PopupDictionaryService>()
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    protected void OnStartup(object sender, StartupEventArgs e)
    {
        MainView mainView = _serviceProvider.GetRequiredService<MainView>();
        mainView.Show();

        _serviceProvider.GetRequiredService<INavigationService>().NavigateTo<IntroductionViewModel>();
        _serviceProvider.GetRequiredService<PopupDictionaryService>().MainWindow = mainView;
    }
}
