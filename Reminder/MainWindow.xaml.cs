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
            ReminderText.TextChanged += ReminderText_TextChanged;

            Btn_StartReminder.Content = "Start\nTimer";

            ReminderText.CaretBrush = new SolidColorBrush(Colors.Transparent);
            ReminderText.IsReadOnly = true;
            ReminderText.Text = stdReminderText;
            ReminderText.TextAlignment = TextAlignment.Center;
            ReminderText.VerticalContentAlignment = VerticalAlignment.Center;
            ReminderText.AcceptsReturn = true;
            ReminderText.Focus();

            Timer.Interval = TimeSpan.FromSeconds(4);
            Timer.Tick += Notification_Tick;
        }

        private void MainWindow_GotFocus(object sender, RoutedEventArgs e)
        {
            ReminderText.Focus();
        }

        private void ReminderText_TextChanged(object sender, TextChangedEventArgs e)
        {
            
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

        // Button Click-Eventhandler
        private void Btn_Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Btn_StartReminder_Click(object sender, RoutedEventArgs e)
        {
            if (CheckReminderText())
            {
                Timer.Start();
            }
        }
    }
}