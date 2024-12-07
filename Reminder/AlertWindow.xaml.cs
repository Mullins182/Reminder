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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Reminder
{
    public partial class AlertWindow : Window
    {
        public AlertWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            MessageBox.Text = MainWindow.notificationString;
            MessageBox.IsReadOnly = true;
            MessageBox.TextAlignment = TextAlignment.Center;
            MessageBox.VerticalContentAlignment = VerticalAlignment.Center;
            MessageBox.AcceptsReturn = true;
            this.Topmost = true;

        }

        private void CloseMessage_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
