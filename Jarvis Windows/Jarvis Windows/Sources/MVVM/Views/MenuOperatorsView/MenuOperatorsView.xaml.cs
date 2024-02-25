using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Automation;
using Jarvis_Windows.Sources.Utils.WindowsAPI;

namespace Jarvis_Windows.Sources.MVVM.Views.MenuOperatorsView;

public partial class MenuOperatorsView : UserControl
{
    private bool _isWindowClosed = false;
    public MenuOperatorsView()
    {
        InitializeComponent();
        EventAggregator.JarvisActionPositionChanged += OnJarvisActionPositionChanged;
    }

    private void OnJarvisActionPositionChanged(object sender, EventArgs e)
    {
        string objID = (string)sender;
        if (objID == "GuidelineText") _isWindowClosed = false;
        else _isWindowClosed = true;
    }

    private void Jarvis_Custom_Action_TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        Window mainWindow = Window.GetWindow(this);

        if (mainWindow == null) return;
        if (_isWindowClosed) mainWindow.Hide();

        mainWindow.Activate();
        mainWindow.Focus();
    }

    private void Jarvis_Custom_Action_TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        HwndSource source = (HwndSource)PresentationSource.FromVisual(textBox);
        if (source != null)
        {
            IntPtr handle = source.Handle;
            var currentAutomation = AutomationElement.FromHandle(handle);
            NativeUser32API.SetForegroundWindow(handle);
            Jarvis_Custom_Action_TextBox.Focus();
        }
    }
}