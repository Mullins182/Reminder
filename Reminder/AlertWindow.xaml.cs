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
        private readonly MediaPlayer openBox = new();
        private readonly MediaPlayer notify = new();
        private readonly DoubleAnimation TextboxAnimation = new();
        private readonly DoubleAnimation Btn_CloseMessageAnimation = new();
        private readonly int animationTimerMsec = 650;

        public AlertWindow(string notificationMsg)
        {
            InitializeComponent();
            Initialize(notificationMsg);
        }

        private async void Initialize(string msg)
        {
            openBox.Open(new Uri("sounds/raiseUp.mp3", UriKind.Relative));
            notify.Open(new Uri("sounds/notify.mp3", UriKind.Relative));

            CloseMessage.Opacity = 0.00;

            TextboxAnimation.Duration = TimeSpan.FromMilliseconds(animationTimerMsec);
            TextboxAnimation.From = 0;
            TextboxAnimation.To = BoxColumn.Width.Value;

            Btn_CloseMessageAnimation.Duration = TimeSpan.FromMilliseconds(animationTimerMsec);
            Btn_CloseMessageAnimation.From = 0.00;
            Btn_CloseMessageAnimation.To = 0.85;

            MessageBox.Width = 0;
            MessageBox.IsReadOnly = true;
            MessageBox.TextAlignment = TextAlignment.Center;
            MessageBox.VerticalContentAlignment = VerticalAlignment.Center;
            MessageBox.AcceptsReturn = true;
            this.Topmost = true;

            await StartTextboxAnimation();
            MessageBox.Text = msg;
            CloseMessage.BeginAnimation(OpacityProperty, Btn_CloseMessageAnimation);
            await PlayNotifySound();
            await PlayNotifySound();
        }

        private async Task<bool> PlayNotifySound()
        {
            notify.Position = TimeSpan.Zero;
            notify.Play();
            await Task.Delay(4000);
            return true;
        }

        private async Task<bool> StartTextboxAnimation()
        {
            MessageBox.BeginAnimation(WidthProperty, TextboxAnimation);
            openBox.Position = TimeSpan.Zero;
            openBox.Play();
            await Task.Delay(animationTimerMsec);
            return true;
        }

        private void CloseMessage_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseMessage_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseMessage.Foreground = new SolidColorBrush(Colors.GreenYellow);
        }

        private void CloseMessage_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseMessage.Foreground = new SolidColorBrush(Colors.Red);
        }
    }
}
