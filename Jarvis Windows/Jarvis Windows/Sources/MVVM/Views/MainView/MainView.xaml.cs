using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Forms;
using System.Drawing;

namespace Jarvis_Windows.Sources.MVVM.Views.MainView;
public partial class MainView : Window
{
    private NotifyIcon _notifyIcon;
    private ContextMenuStrip _contextMenuStrip;
    public MainView()
    {
        InitializeComponent();
        InitTrayIcon();
    }

    private void GuidelineText_TextChanged(object sender, TextChangedEventArgs e) { }

    private void InitTrayIcon()
    {
        _notifyIcon = new NotifyIcon();
        _contextMenuStrip = new ContextMenuStrip();
        _notifyIcon.Icon = new System.Drawing.Icon("Assets/Icons/jarvis_icon.ico");


        _notifyIcon.MouseClick += NotifyIcon_MouseClick;
        _contextMenuStrip.Renderer = new MyRenderer();

        // _contextMenuStrip.Items.Add("Open Jarvis main Window", null, QuitMenuItem_Click);
        _contextMenuStrip.Items.Add("Quit Jarvis", null, QuitMenuItem_Click);

        _notifyIcon.ContextMenuStrip = _contextMenuStrip;
        _notifyIcon.Visible = true;
    }

    private void NotifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            this.Show();
        }
    }

    private void QuitMenuItem_Click(object sender, EventArgs e)
    {
        System.Windows.Application.Current.Shutdown();
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
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
