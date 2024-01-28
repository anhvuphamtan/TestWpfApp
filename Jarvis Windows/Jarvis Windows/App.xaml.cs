using Jarvis_Windows.Sources.MVVM.ViewModels;
using Jarvis_Windows.Sources.MVVM.Views.JarvisActionView;
using Jarvis_Windows.Sources.MVVM.Views.MainView;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Jarvis_Windows.Sources.DataAccess;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace Jarvis_Windows;

public partial class App : Application
{
    private ServiceProvider _serviceProvider;
    private const string _uniqueEventName = "Jarvis Windows";
    private EventWaitHandle _eventWaitHandle;

    public App()
    {
        SingleInstanceWatcher();

        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<Func<Type, ViewModelBase>>(serviceProvider => viewModelType => (ViewModelBase)serviceProvider.GetRequiredService(viewModelType));
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<PopupDictionaryService>();     
        services.AddSingleton<SendEventGA4>();

        services.AddSingleton<UIElementDetector>(provider => new UIElementDetector
        {
            PopupDictionaryService = _serviceProvider.GetRequiredService<PopupDictionaryService>(),
            SendEventGA4 = provider.GetRequiredService<SendEventGA4>()
        });


        services.AddSingleton<MainViewModel>();
        // Logging.Log("Before MainView MainviewModel");
        services.AddSingleton<MainView>(provider => new MainView
        {
            DataContext = provider.GetRequiredService<MainViewModel>(),
            SendEventGA4 = provider.GetRequiredService<SendEventGA4>()
        });

        // Logging.Log("After MainView MainviewModel\n");

        services.AddSingleton<JarvisActionViewModel>(provider => new JarvisActionViewModel
        {
            PopupDictionaryService = _serviceProvider.GetRequiredService<PopupDictionaryService>()
        });

        _serviceProvider = services.BuildServiceProvider();
    }


    protected void OnStartup(object sender, StartupEventArgs e)
    {
        // Logging.Log("Before mainview OnStartup");
        try
        {
            MainView mainView = _serviceProvider.GetRequiredService<MainView>();
            // Logging.Log("After 1 mainview OnStartup");
            mainView.Show();

            // Logging.Log("After 2 mainview OnStartup");
            _serviceProvider.GetRequiredService<PopupDictionaryService>().MainWindow = mainView;
            // Logging.Log("After 3 mainview OnStartup");

        }

        catch (Exception ex)
        {
            
        }
    }

    private void SingleInstanceWatcher()
    {
        try
        {
            this._eventWaitHandle = EventWaitHandle.OpenExisting(_uniqueEventName);
            this._eventWaitHandle.Set();
            this.Shutdown();
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            this._eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, _uniqueEventName);
        }

        new Task(() =>
        {
            while (this._eventWaitHandle.WaitOne())
            {
                Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (!Current.MainWindow.Equals(null))
                    {
                        var mw = Current.MainWindow;

                        if (mw.WindowState == WindowState.Minimized || mw.Visibility != Visibility.Visible)
                        {
                            mw.Show();
                            mw.WindowState = WindowState.Normal;
                        }

                        mw.Activate();
                        mw.Topmost = true;
                        mw.Topmost = false;
                        mw.Focus();
                    }
                }));
            }
        })
        .Start();
    }
}
