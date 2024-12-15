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
        public static event EventHandler? BtnClick;
        private readonly MediaPlayer openBox = new();
        private readonly MediaPlayer notify = new();
        private readonly DoubleAnimation TextboxAnimation = new();
        private readonly DoubleAnimation Btn_CloseMessageAnimation = new();
        private readonly int animationTimerMsec = 650; // Notify Window Animation Duration
        private readonly int messageBoxBorderAnim = 30; // Notify Window and Close Btn Border Blinking interval
        private readonly int timerDelayFrom = 5; // DispatcherTimer Minute Interval Random Value
        private readonly int timerDelayTo = 15;

        public AlertWindow(string notificationMsg)
        {
            InitializeComponent();
            Initialize(notificationMsg);
        }

        private async void Initialize(string msg)
        {
            this.Topmost = true;
            openBox.Open(new Uri("sounds/raiseUp.mp3", UriKind.Relative));
            notify.Open(new Uri("sounds/notify.mp3", UriKind.Relative));

            AttentionBordersTimer.Interval = TimeSpan.FromMinutes(new Random().Next(timerDelayFrom, timerDelayTo + 1));
            AttentionBordersTimer.Tick += AttentionBordersTimer_Tick;

            CloseMessage.Opacity = 0.00;
            CloseMessage.BorderThickness = new Thickness(1.75, 0, 1.75, 1.75); 

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

            await StartTextboxAnimation();
            MessageBox.Text = msg;
            CloseMessage.BeginAnimation(OpacityProperty, Btn_CloseMessageAnimation);
#pragma warning disable CS4014 // Da auf diesen Aufruf nicht gewartet wird, wird die Ausführung der aktuellen Methode vor Abschluss des Aufrufs fortgesetzt.
            MessageBoxBorderAnim();
#pragma warning restore CS4014 // Da auf diesen Aufruf nicht gewartet wird, wird die Ausführung der aktuellen Methode vor Abschluss des Aufrufs fortgesetzt.
            await PlayNotifySound();
            await PlayNotifySound();
            AttentionBordersTimer.Start();
        }

        private async void AttentionBordersTimer_Tick(object? sender, EventArgs e)
        {
            await MessageBoxBorderAnim();
            AttentionBordersTimer.Interval = TimeSpan.FromMinutes(new Random().Next(timerDelayFrom, timerDelayTo + 1));
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
            MessageBox.BorderThickness = new Thickness(1.75, 1.75, 1.75, 1.75);

            for (int i = 25; i > 0; i--)
            {
                MessageBox.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
                CloseMessage.BorderBrush = new SolidColorBrush(Colors.YellowGreen);
                await Task.Delay(messageBoxBorderAnim);
                MessageBox.BorderBrush = new SolidColorBrush(Colors.Red);
                CloseMessage.BorderBrush = new SolidColorBrush(Colors.Red);
                await Task.Delay(messageBoxBorderAnim);
            }

            MessageBox.BorderThickness = new Thickness(0.65, 0.65, 0.65, 0.65);
            CloseMessage.BorderBrush = new SolidColorBrush(Colors.Black);
            return true;
        }

        private void CloseMessage_Click(object sender, RoutedEventArgs e)
        {
            BtnClick?.Invoke(this, e);
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
