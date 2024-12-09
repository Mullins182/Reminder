using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.UI.Notifications;

namespace Reminder
{
    public partial class MainWindow : Window
    {

        private readonly DispatcherTimer Timer = new();
        private DoubleAnimation ReminderTextboxAnimation = new();
        private DoubleAnimation ReminderTextboxAnimationMouseOver = new();
        private static readonly string stdReminderText = "Enter Your Notification Message !";
        private int setTimerValue = 1;
        private bool timerRunning = false;
        private string notificationText = "";
        private string btn_startReminderContentStd = "Start\nTimer";
        private string btn_startReminderContentRunning = "Timer\nRunning";
        private readonly string[] tb_setTimeText = ["Notify me after\n> ", " <\nMinutes"];


        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            this.GotFocus += MainWindow_GotFocus;
            ReminderText.MouseLeave += ReminderText_MouseLeave;
            ReminderText.MouseEnter += ReminderText_MouseEnter;

            ReminderTextboxAnimation.From = 0.35;
            ReminderTextboxAnimation.To = 0.55;
            ReminderTextboxAnimation.Duration = TimeSpan.FromMilliseconds(500);
            ReminderTextboxAnimationMouseOver.From = 0.55;
            ReminderTextboxAnimationMouseOver.To = 0.35;
            ReminderTextboxAnimationMouseOver.Duration = TimeSpan.FromMilliseconds(500);

            Tbl_ReminderTimer.Text = btn_startReminderContentStd;
            Tb_SetTime.Text = tb_setTimeText[0] + setTimerValue + tb_setTimeText[1];

            ReminderText.CaretBrush = new SolidColorBrush(Colors.Transparent);
            ReminderText.IsReadOnly = true;
            ReminderText.Text = stdReminderText;
            ReminderText.TextAlignment = TextAlignment.Center;
            ReminderText.VerticalContentAlignment = VerticalAlignment.Center;
            ReminderText.AcceptsReturn = true;
            ReminderText.Focus();

            Timer.Interval = TimeSpan.FromMinutes(1);
            Timer.Tick += Notification_Tick;
        }

        private void UpdateShowSelectedTimeTextbox()
        {
            Tb_SetTime.Text = tb_setTimeText[0] + setTimerValue + tb_setTimeText[1];
        }

        private async void TimerStarted()
        {
            timerRunning = true;

            notificationText = ReminderText.Text;

            Tbl_ReminderTimer.Text = btn_startReminderContentRunning;

            for (int i = 10; i > 0; i--)
            {
                Btn_StartReminder.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
                Btn_StartReminder.Effect = (DropShadowEffect)Resources["ButtonShadowsOnMouseOver"];
                await Task.Delay(50);
                Btn_StartReminder.BorderBrush = new SolidColorBrush(Colors.Red);
                Btn_StartReminder.Effect = (DropShadowEffect)Resources["ButtonShadowRed"];
                await Task.Delay(50);
            }
        }

        private void MainWindow_GotFocus(object sender, RoutedEventArgs e)
        {
            ReminderText.Focus();
        }

        private async void Notification_Tick(object? sender, EventArgs e)
        {
            AlertWindow Notification = new(notificationText);

            Notification.Show();

            await Task.Delay(2000);

            Tbl_ReminderTimer.Text = btn_startReminderContentStd;
            Btn_StartReminder.BorderBrush = new SolidColorBrush(Colors.DarkGoldenrod);
            Btn_StartReminder.Effect = (DropShadowEffect)Resources["ButtonShadows"];

            timerRunning = false;

            Timer.Stop();
        }

        private bool CheckReminderText()
        {
            return ReminderText.Text == stdReminderText ? false : true;
        }

        // Button/Slider Eventhandler
        private void Btn_Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Btn_StartReminder_Click(object sender, RoutedEventArgs e)
        {
            if (CheckReminderText() && !timerRunning)
            {
                Timer.Interval = TimeSpan.FromMinutes(setTimerValue);
                Timer.Start();
                TimerStarted();
            }
        }

        private void Btn_ClearBox_Click(object sender, RoutedEventArgs e)
        {
            ReminderText.Clear();
            ReminderText.Text = stdReminderText;
        }

        private void Sld_SetTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setTimerValue = (int)Sld_SetTime.Value;
            UpdateShowSelectedTimeTextbox();
        }

        private void Btn_IncreaseTime_Click(object sender, RoutedEventArgs e)
        {
            setTimerValue = setTimerValue < 5 ? setTimerValue = 5 : setTimerValue += 5;
            Sld_SetTime.Value = setTimerValue < 60 ? setTimerValue : 60;
            UpdateShowSelectedTimeTextbox();
        }

        private void Btn_DecreaseTime_Click(object sender, RoutedEventArgs e)
        {
            setTimerValue = setTimerValue > 5 ? setTimerValue -= 5 : setTimerValue <= 5 ? 1 : 30;
            Sld_SetTime.Value = setTimerValue < 60 ? setTimerValue : 60;
            UpdateShowSelectedTimeTextbox();
        }

        // Mouse Enter/Leave Event Handler
        private void ReminderText_MouseEnter(object sender, MouseEventArgs e)
        {
            ReminderText.IsReadOnly = false;
            ReminderText.CaretBrush = new SolidColorBrush(Colors.Gold);
            var effekt = (DropShadowEffect)ReminderText.Effect;
            effekt.BeginAnimation(DropShadowEffect.OpacityProperty, ReminderTextboxAnimationMouseOver);
            ReminderText.CaretIndex = ReminderText.Text.Length;
            ReminderText.Text = ReminderText.Text == stdReminderText ? "" : ReminderText.Text;
        }
        private void ReminderText_MouseLeave(object sender, MouseEventArgs e)
        {
            ReminderText.Text = ReminderText.Text == "" ? stdReminderText : ReminderText.Text;
            ReminderText.CaretBrush = new SolidColorBrush(Colors.Transparent);
            var effekt = (DropShadowEffect)ReminderText.Effect;
            effekt.BeginAnimation(DropShadowEffect.OpacityProperty, ReminderTextboxAnimation);
            ReminderText.IsReadOnly = true;
        }

        private void Btn_ClearBox_MouseEnter(object sender, MouseEventArgs e)
        {
            Btn_ClearBox.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
            Btn_ClearBox.Foreground = new SolidColorBrush(Colors.GreenYellow);
            Btn_ClearBox.Effect = (DropShadowEffect)Resources["ButtonShadowsOnMouseOver"]; // Shadow Effekt aus ResourceDictionary "Style.xaml" laden
        }

        private void Btn_ClearBox_MouseLeave(object sender, MouseEventArgs e)
        {
            Btn_ClearBox.BorderBrush = new SolidColorBrush(Colors.DarkGoldenrod);
            Btn_ClearBox.Foreground = new SolidColorBrush(Colors.Goldenrod);
            Btn_ClearBox.Effect = (DropShadowEffect)Resources["ButtonShadows"]; // Shadow Effekt aus ResourceDictionary "Style.xaml" laden
        }

        private void Btn_IncreaseTime_MouseEnter(object sender, MouseEventArgs e)
        {
            Btn_IncreaseTime.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
            Btn_IncreaseTime.Foreground = new SolidColorBrush(Colors.GreenYellow);
            Btn_IncreaseTime.Effect = (DropShadowEffect)Resources["ButtonShadowsOnMouseOver"]; // Shadow Effekt aus ResourceDictionary "Style.xaml" laden
        }

        private void Btn_IncreaseTime_MouseLeave(object sender, MouseEventArgs e)
        {
            Btn_IncreaseTime.BorderBrush = new SolidColorBrush(Colors.DarkGoldenrod);
            Btn_IncreaseTime.Foreground = new SolidColorBrush(Colors.Goldenrod);
            Btn_IncreaseTime.Effect = (DropShadowEffect)Resources["ButtonShadows"]; // Shadow Effekt aus ResourceDictionary "Style.xaml" laden
        }

        private void Btn_DecreaseTime_MouseEnter(object sender, MouseEventArgs e)
        {
            Btn_DecreaseTime.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
            Btn_DecreaseTime.Foreground = new SolidColorBrush(Colors.GreenYellow);
            Btn_DecreaseTime.Effect = (DropShadowEffect)Resources["ButtonShadowsOnMouseOver"]; // Shadow Effekt aus ResourceDictionary "Style.xaml" laden
        }

        private void Btn_DecreaseTime_MouseLeave(object sender, MouseEventArgs e)
        {
            Btn_DecreaseTime.BorderBrush = new SolidColorBrush(Colors.DarkGoldenrod);
            Btn_DecreaseTime.Foreground = new SolidColorBrush(Colors.Goldenrod);
            Btn_DecreaseTime.Effect = (DropShadowEffect)Resources["ButtonShadows"]; // Shadow Effekt aus ResourceDictionary "Style.xaml" laden
        }

        private void Btn_StartReminder_MouseEnter(object sender, MouseEventArgs e)
        {
            Btn_StartReminder.BorderBrush = timerRunning ? Btn_StartReminder.BorderBrush : new SolidColorBrush(Colors.GreenYellow);
            Btn_StartReminder.Foreground = timerRunning ? Btn_StartReminder.Foreground : new SolidColorBrush(Colors.GreenYellow);
            // Shadow Effekt aus ResourceDictionary "Style.xaml" laden
            Btn_StartReminder.Effect = timerRunning ? Btn_StartReminder.Effect : (DropShadowEffect)Resources["ButtonShadowsOnMouseOver"];
        }

        private void Btn_StartReminder_MouseLeave(object sender, MouseEventArgs e)
        {
            Btn_StartReminder.BorderBrush = timerRunning ? Btn_StartReminder.BorderBrush : new SolidColorBrush(Colors.DarkGoldenrod);
            Btn_StartReminder.Foreground = timerRunning ? Btn_StartReminder.Foreground : new SolidColorBrush(Colors.Goldenrod);
            // Shadow Effekt aus ResourceDictionary "Style.xaml" laden
            Btn_StartReminder.Effect = timerRunning ? Btn_StartReminder.Effect : (DropShadowEffect)Resources["ButtonShadows"];
        }

        private void Btn_Quit_MouseEnter(object sender, MouseEventArgs e)
        {
            Btn_Quit.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
            Btn_Quit.Foreground = new SolidColorBrush(Colors.GreenYellow);
            Btn_Quit.Effect = (DropShadowEffect)Resources["ButtonShadowsOnMouseOver"]; // Shadow Effekt aus ResourceDictionary "Style.xaml" laden
        }

        private void Btn_Quit_MouseLeave(object sender, MouseEventArgs e)
        {
            Btn_Quit.BorderBrush = new SolidColorBrush(Colors.DarkGoldenrod);
            Btn_Quit.Foreground = new SolidColorBrush(Colors.Goldenrod);
            Btn_Quit.Effect = (DropShadowEffect)Resources["ButtonShadows"]; // Shadow Effekt aus ResourceDictionary "Style.xaml" laden
        }
    }
}