using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
        private static readonly string stdReminderText = "Enter Your Notification Message !";
        private int setTimerValue = 1;


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

            Btn_StartReminder.Content = "Start\nTimer";
            Btn_ClearBox.Content = "Clear\nMessage\nBox";
            Tb_SetTime.Text = $"Notify me after\n[ {setTimerValue} ]\nMinutes";

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
            Tb_SetTime.Text = $"Notify me after\n[ {setTimerValue} ]\nMinutes";
        }

        private void MainWindow_GotFocus(object sender, RoutedEventArgs e)
        {
            ReminderText.Focus();
        }

        private void ReminderText_MouseEnter(object sender, MouseEventArgs e)
        {
            ReminderText.IsReadOnly = false;
            ReminderText.CaretBrush = new SolidColorBrush(Colors.Gold);
            ReminderText.CaretIndex = ReminderText.Text.Length;
            ReminderText.Text = ReminderText.Text == stdReminderText ? "" : ReminderText.Text;
        }
        private void ReminderText_MouseLeave(object sender, MouseEventArgs e)
        {
            ReminderText.Text = ReminderText.Text == "" ? stdReminderText : ReminderText.Text;
            ReminderText.CaretBrush = new SolidColorBrush(Colors.Transparent);
            ReminderText.IsReadOnly = true;
        }

        private void Notification_Tick(object? sender, EventArgs e)
        {
            AlertWindow Notification = new(ReminderText.Text);

            Notification.Show();

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
            if (CheckReminderText())
            {
                Timer.Interval = TimeSpan.FromMinutes(setTimerValue);
                Timer.Start();
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
            setTimerValue = setTimerValue > 4 ? setTimerValue -= 5 : setTimerValue = 0;
            Sld_SetTime.Value = setTimerValue < 60 ? setTimerValue : 60;
            UpdateShowSelectedTimeTextbox();
        }
    }
}