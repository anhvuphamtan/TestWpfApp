//using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;
using Jarvis_Windows.Sources.DataAccess;
namespace Jarvis_Windows.Sources.MVVM.Views.MainView;
public partial class MainView : Window
{
    private bool _isMainWindowOpened;
    private NotifyIcon _notifyIcon;
    private ContextMenuStrip _contextMenuStrip;
    private SendEventGA4 _sendEventGA4;

    public SendEventGA4 SendEventGA4
    {
        get { return _sendEventGA4; }
        set { _sendEventGA4 = value; }
    }

    public MainView()
    {
        InitializeComponent();
        InitTrayIcon();

    }

    private void InitTrayIcon()
    {
        _notifyIcon = new NotifyIcon();
        _contextMenuStrip = new ContextMenuStrip();

        string relativePath = Path.Combine("Assets", "Icons", "jarvis_logo_large.ico");
        string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
        _notifyIcon.Icon = new System.Drawing.Icon(fullPath);


        _notifyIcon.MouseClick += NotifyIcon_MouseClick;
        _contextMenuStrip.Renderer = new MyRenderer();

        _contextMenuStrip.Items.Add("Quit Jarvis", null, QuitMenuItem_Click);

        _notifyIcon.ContextMenuStrip = _contextMenuStrip;
        _notifyIcon.Visible = true;
    }

    private async void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            this.Show();
            this.Activate();
            if (_isMainWindowOpened == false)
                await SendEventGA4.SendEvent("open_main_window");

            _isMainWindowOpened = true;
        }
    }

    private async void QuitMenuItem_Click(object sender, EventArgs e)
    {
        try 
        {
            await SendEventGA4.SendEvent("quit_app");
            Process.GetCurrentProcess().Kill(); //DaiTT fix
        }

        catch (Exception ex) 
        {
            System.Windows.MessageBox.Show(ex.Message);
        }
    }
    private void OnExit(object sender, ExitEventArgs e)
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }

    private void btnCloseMainWindows_Click(object sender, RoutedEventArgs e)
    {
        _isMainWindowOpened = false;
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


    private class MyRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected) base.OnRenderMenuItemBackground(e);
            else
            {
                Rectangle rc = new Rectangle(System.Drawing.Point.Empty, e.Item.Size);
                e.Graphics.FillRectangle(System.Drawing.Brushes.Transparent, rc);
                e.Graphics.DrawRectangle(Pens.Transparent, 1, 0, rc.Width - 2, rc.Height - 1);
            }
        }
    }


}
