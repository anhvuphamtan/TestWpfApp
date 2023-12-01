using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jarvis_Windows.Sources.MVVM.Views.IntroductionView
{
    /// <summary>
    /// Interaction logic for IntroductionView.xaml
    /// </summary>
    public partial class IntroductionView : UserControl
    {
        public IntroductionView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Xử lý lỗi nếu có
            }
            e.Handled = true;
        }
    }
}
