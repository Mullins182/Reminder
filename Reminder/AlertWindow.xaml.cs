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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.UI.Composition;

namespace Reminder
{
    public partial class AlertWindow : Window
    {
        private readonly DispatcherTimer AttentionBordersTimer = new();
        private readonly MediaPlayer openBox = new();
        private readonly MediaPlayer notify = new();
        private readonly DoubleAnimation TextboxAnimation = new();
        private readonly DoubleAnimation Btn_CloseMessageAnimation = new();
        private readonly int animationTimerMsec = 650; // Notify Window Animation Duration
        private readonly int messageBoxBorderAnim = 30; // Notify Window and Close Btn Border Blinking interval
        private readonly int timerDelayFrom = 2;
        private readonly int timerDelayTo = 7;
        private int attentionBordersTimerInterval = new Random().Next(2, 7);

        public AlertWindow(string notificationMsg)
        {
            InitializeComponent();
            Initialize(notificationMsg);
        }

        private async void Initialize(string msg)
        {
            openBox.Open(new Uri("sounds/raiseUp.mp3", UriKind.Relative));
            notify.Open(new Uri("sounds/notify.mp3", UriKind.Relative));

            AttentionBordersTimer.Interval = TimeSpan.FromMinutes(attentionBordersTimerInterval);
            AttentionBordersTimer.Tick += AttentionBordersTimer_Tick;

            CloseMessage.Opacity = 0.00;
            CloseMessage.BorderThickness = new Thickness(1, 0, 1, 1); 

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
            MessageBoxBorderAnim();
            await PlayNotifySound();
            await PlayNotifySound();
            AttentionBordersTimer.Start();
        }

        private async void AttentionBordersTimer_Tick(object? sender, EventArgs e)
        {
            await MessageBoxBorderAnim();

            attentionBordersTimerInterval = new Random().Next(timerDelayFrom, timerDelayTo);
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

        private async Task<bool> MessageBoxBorderAnim()
        {
            for (int i = 20; i > 0; i--)
            {
                MessageBox.BorderBrush = new SolidColorBrush(Colors.YellowGreen);
                CloseMessage.BorderBrush = new SolidColorBrush(Colors.YellowGreen);
                await Task.Delay(messageBoxBorderAnim);
                MessageBox.BorderBrush = new SolidColorBrush(Colors.Red);
                CloseMessage.BorderBrush = new SolidColorBrush(Colors.Black);
                await Task.Delay(messageBoxBorderAnim);
            }
            return true;
        }

        private void CloseMessage_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseMessage_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseMessage.Foreground = new SolidColorBrush(Colors.GreenYellow);
            CloseMessage.Effect = (DropShadowEffect)Resources["ButtonShadowsOnMouseOver"]; // Shadow Effekt aus ResourceDictionary "Style.xaml" laden
        }

        private void CloseMessage_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseMessage.Foreground = new SolidColorBrush(Colors.Red);
            CloseMessage.Effect = (DropShadowEffect)Resources["ButtonShadows"]; // Shadow Effekt aus ResourceDictionary "Style.xaml" laden
        }
    }
}
