using Jarvis_Windows.Sources.MVVM.ViewModels;
using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.Utils.Services;

public interface INavigationService
{
    ViewModelBase CurrentView { get; }

    void NavigateTo<T>() where T : ViewModelBase;
}

public class NavigationService : ObserveralObject, INavigationService
{
    private ViewModelBase _currentView;
    private readonly Func<Type, ViewModelBase> _viewModelFactory;

    public ViewModelBase CurrentView
    {
        get => _currentView;
        private set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
    {
        ViewModelBase viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
        CurrentView = viewModel;
    }
}
