using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Jarvis_Windows.Sources.MVVM.Views.MainView;
public partial class MainView : Window
{
    private TaskbarIcon _taskbarIcon;
    public MainView()
    {
        InitializeComponent();
        InitTrayIcon();
    }

    private void GuidelineText_TextChanged(object sender, TextChangedEventArgs e)
    {
        
    }



    private void InitTrayIcon()
    {
        _taskbarIcon = new TaskbarIcon();
        _taskbarIcon.Icon = new System.Drawing.Icon("Assets/Icons/jarvis_icon.ico");

        _taskbarIcon.TrayLeftMouseDown += (sender, e) =>
        {
            this.Show();
        };

        _taskbarIcon.TrayRightMouseDown += TaskbarIcon_TrayRightMouseDown;

        trayMenuPopup.LostFocus += (sender, e) =>
        {
            trayMenuPopup.IsOpen = false;
        };

        trayMenuPopup.LostMouseCapture += (sender, e) =>
        {
            trayMenuPopup.IsOpen = false;
        };
    }

    private void TrayMenuPopup_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        return;
        if (trayMenuPopup.IsMouseOver)
        {
            trayMenuPopup.IsOpen = false;
            trayMenuPopup.PreviewMouseDown -= TrayMenuPopup_PreviewMouseDown;
        }
    }

    private void TaskbarIcon_TrayRightMouseDown(object sender, RoutedEventArgs e)
    {
        //Point mousePos = NativeUser32API.GetCursorPosition();
        //FIXME:
        trayMenuPopup.HorizontalOffset = 1100;
        trayMenuPopup.VerticalOffset = 700;
        trayMenuPopup.IsOpen = true;
        trayMenuPopup.PreviewMouseDown += TrayMenuPopup_PreviewMouseDown;
    }

    private void btnCloseMainWindows_Click(object sender, RoutedEventArgs e)
    {
        this.Hide();
    }

    private void btnMoreAtJarvis_Click(object sender, RoutedEventArgs e)
    {
        System.Diagnostics.Process.Start(new ProcessStartInfo
        {
            FileName = "https://jarvis.cx/",
            UseShellExecute = true
        });
    }
}
