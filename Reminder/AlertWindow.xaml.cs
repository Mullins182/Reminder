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
using System.Windows.Shapes;
using Windows.UI.Composition;

namespace Reminder
{
    public partial class AlertWindow : Window
    {

        private readonly DoubleAnimation TextboxAnimation = new();
        private bool animationFinished = false;
        private int animationTimerMsec = 3000;

        public AlertWindow(string notification)
        {
            InitializeComponent();
            Initialize(notification);
        }

        private async void Initialize(string notification)
        {

            CloseMessage.Visibility = Visibility.Collapsed;

            TextboxAnimation.Duration = TimeSpan.FromMilliseconds(animationTimerMsec);
            TextboxAnimation.From = 0;
            TextboxAnimation.To = 500;

            MessageBox.Text = notification;
            MessageBox.Width = 0;
            MessageBox.IsReadOnly = true;
            MessageBox.TextAlignment = TextAlignment.Center;
            MessageBox.VerticalContentAlignment = VerticalAlignment.Center;
            MessageBox.AcceptsReturn = true;
            this.Topmost = true;

            await StartTextboxAnimation();

            CloseMessage.Visibility = Visibility.Visible;
        }

        private async Task<bool> StartTextboxAnimation()
        {
            MessageBox.BeginAnimation(WidthProperty, TextboxAnimation);
            await Task.Delay(animationTimerMsec);
            return true;
        }

        private void CloseMessage_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
