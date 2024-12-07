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
        public static string notificationString = "";

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            StartReminderTimer.Content = "Start\nTimer";

            ReminderText.TextAlignment = TextAlignment.Center;
            ReminderText.VerticalContentAlignment = VerticalAlignment.Center;
            ReminderText.AcceptsReturn = true;

            Timer.Interval = TimeSpan.FromSeconds(10);
            Timer.Tick += Notification_Tick;
        }

        private void Notification_Tick(object? sender, EventArgs e)
        {
            AlertWindow Notification = new();

            Notification.Show();

            Timer.Stop();
        }

        // Button Click-Eventhandler
        private void Btn_Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void StartReminderTimer_Click(object sender, RoutedEventArgs e)
        {
            Timer.Start();
            notificationString = ReminderText.Text;
        }
    }
}