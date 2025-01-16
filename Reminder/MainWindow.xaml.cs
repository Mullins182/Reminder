using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Resources;
using System.Windows.Threading;

namespace Reminder
{
    public partial class MainWindow : Window
    {

        private readonly DispatcherTimer Timer = new();
        private readonly SoundPlayer _button_click = new();
        private readonly SoundPlayer _start_action = new();
        private Stream? _stream1;
        private Stream? _stream2;
        private readonly DoubleAnimation ReminderTextboxAnimation = new();
        private readonly DoubleAnimation ReminderTextboxAnimationMouseOver = new();
        private static readonly string stdReminderText = "Enter Your Notification Message !";
        private int setTimerValue = 1;
        private bool timerRunning = false;
        private string notificationText = "";
        private readonly string prgVersion = "v1.1";
        private readonly string button_snd_src = "pack://application:,,,/sounds/button_click.wav";
        private readonly string startTimer_snd_src = "pack://application:,,,/sounds/start_action.wav";
        private readonly string btn_startReminderContentStd = "Start\nTimer";
        private readonly string btn_startReminderContentRunning = "Timer\nRunning";
        private readonly string[] tb_setTimeText = ["Notify me after\n> ", " <\nMinutes"];


        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            this.GotFocus += MainWindow_GotFocus;
            PrgVersion.Content = prgVersion;
            AlertWindow.BtnClick += AlertWindow_BtnClick;
            ReminderText.MouseLeave += ReminderText_MouseLeave;
            ReminderText.MouseEnter += ReminderText_MouseEnter;

            // Lade die Ressource aus dem Assembly mit der WPF-Pack-URI-Syntax
            var resourceInfo_button_click = Application.GetResourceStream(new Uri(button_snd_src, UriKind.Absolute));
            var resourceInfo_start_action = Application.GetResourceStream(new Uri(startTimer_snd_src, UriKind.Absolute));
            _stream1 = resourceInfo_button_click.Stream;
            _stream2 = resourceInfo_start_action.Stream;
            _button_click.Stream = _stream1;
            _start_action.Stream = _stream2;

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

        private void UpdateTimeInSetTimeTb()
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

            Timer.Stop();
            timerRunning = false;
        }

        private bool CheckReminderText()
        {
            return ReminderText.Text == stdReminderText ? false : true;
        }

        // Button/Slider Eventhandler
        private void AlertWindow_BtnClick(object? sender, EventArgs e)
        {
            _button_click.Play();
        }

        private async void Btn_Quit_Click(object sender, RoutedEventArgs e)
        {
            _button_click.Play();

            await Task.Delay(650);

            // Streams schließen
            _stream1?.Dispose();
            _stream2?.Dispose();
            _button_click?.Dispose();
            _start_action?.Dispose();

            Application.Current.Shutdown();
        }

        private void Btn_StartReminder_Click(object sender, RoutedEventArgs e)
        {
            if (CheckReminderText() && !timerRunning)
            {
                _start_action.Play();

                Timer.Interval = TimeSpan.FromMinutes(setTimerValue);
                Timer.Start();
                TimerStarted();
            }
        }

        private void Btn_ClearBox_Click(object sender, RoutedEventArgs e)
        {
            _button_click.Play();

            ReminderText.Clear();
            ReminderText.Text = stdReminderText;
        }

        private void Sld_SetTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            setTimerValue = (int)Sld_SetTime.Value;
            UpdateTimeInSetTimeTb();
        }

        private void Btn_IncreaseTime_Click(object sender, RoutedEventArgs e)
        {
            _button_click.Play();

            setTimerValue = setTimerValue < 5 ? setTimerValue = 5 : setTimerValue += 5;
            Sld_SetTime.Value = setTimerValue < 60 ? setTimerValue : 60;
            UpdateTimeInSetTimeTb();
        }

        private void Btn_DecreaseTime_Click(object sender, RoutedEventArgs e)
        {
            _button_click.Play();

            setTimerValue = setTimerValue > 5 ? setTimerValue -= 5 : setTimerValue <= 5 ? 1 : 30;
            Sld_SetTime.Value = setTimerValue < 60 ? setTimerValue : 60;
            UpdateTimeInSetTimeTb();
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