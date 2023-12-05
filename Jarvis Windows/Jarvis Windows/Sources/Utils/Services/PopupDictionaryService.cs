using Jarvis_Windows.Sources.MVVM.Views.MainView;
using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Media;

namespace Jarvis_Windows.Sources.Utils.Services;

public class PopupDictionaryService : ObserveralObject
{
    private bool _isShowJarvisAction;
    private bool _isShowMenuOperations; 
    private Point _jarvisActionPosition;
    private Point _menuOperationsPosition;

    public bool IsShowJarvisAction
    {
        get { return _isShowJarvisAction; }
        set
        {
            _isShowJarvisAction = value;
            OnPropertyChanged();
        }
    }

    public bool IsShowMenuOperations
    {
        get { return _isShowMenuOperations; }
        set
        {
            _isShowMenuOperations = value;
            OnPropertyChanged();
        }
    }

    public Point JarvisActionPosition
    {
        get { return _jarvisActionPosition; }
        set
        {
            _jarvisActionPosition = value;
            OnPropertyChanged();
        }
    }

    public Point MenuOperationsPosition
    {
        get { return _menuOperationsPosition; }
        set
        {
            _menuOperationsPosition = value;
            OnPropertyChanged();
        }
    }

    public MainView MainWindow { get; set; }

    public PopupDictionaryService()
    {
        IsShowJarvisAction = false;
        IsShowMenuOperations = false;
        JarvisActionPosition = new Point(0, 0);
        MenuOperationsPosition = new Point(0, 0);
    }

    public void ShowJarvisAction(bool isShow)
    {
        IsShowJarvisAction = isShow;
    }

    private Point ConvertFromSystemCoorToVisualCoord(Point systemPoint)
    {
        Point visualPos = new Point();

        //Access to UI thread
        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            if (source != null)
            {
                visualPos = source.CompositionTarget.TransformFromDevice.Transform(new Point(systemPoint.X, systemPoint.Y));
                visualPos.X = visualPos.X - 22;
                visualPos.Y = visualPos.Y - 22 / 2;
                JarvisActionPosition = visualPos;
                visualPos.X = visualPos.X - 200;
                visualPos.Y = visualPos.Y - 180;
                MenuOperationsPosition = visualPos;
            }    
        }));

        return visualPos;
    }

    public void UpdateJarvisActionPosition(Point systemPoint)
    {
        //TODO: Convert system point to visual point
        Point visualPoint = ConvertFromSystemCoorToVisualCoord(systemPoint);
        JarvisActionPosition = visualPoint;
    }

    public void ShowMenuOperations(bool isShow)
    {
        IsShowMenuOperations = isShow;
    }

    public void UpdateMenuOperationsPosition(Point systemPoint)
    {
        //TODO: Convert system point to visual point
        Point visualPoint = ConvertFromSystemCoorToVisualCoord(systemPoint);
        MenuOperationsPosition = visualPoint;
    }
}
