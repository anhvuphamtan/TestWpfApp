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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jarvis_Windows.Sources.MVVM.Views.JarvisActionView
{
    /// <summary>
    /// Interaction logic for JarvisActionView.xaml
    /// </summary>
    public partial class JarvisActionView : UserControl
    {
        public JarvisActionView()
        {
            InitializeComponent();
            CreateAnimation();
        }

        private DoubleAnimation CreateAnimation()
        {
            DoubleAnimation rotateAnimation = new DoubleAnimation();
            rotateAnimation.To = 360;
            rotateAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            rotateAnimation.RepeatBehavior = RepeatBehavior.Forever;
            rotateAnimation.AutoReverse = false;

            Storyboard.SetTargetName(rotateAnimation, "rotateTransform");
            Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath(RotateTransform.AngleProperty));

            return rotateAnimation;
        }
    }
}
