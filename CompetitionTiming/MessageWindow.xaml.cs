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

namespace CompetitionTiming
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        string messages;

        public MessageWindow()
        {
            InitializeComponent();
            messages = "";
            messageTextBox.Text = "";
        }

        public MessageWindow(string value)
        {
            InitializeComponent();
            messages = value;
            messageTextBox.Text = value;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            messageTextBox.Focus();
            
           
        }

        public string Answer
        {
            get
            {
                return "";
            }
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            messageTextBox.Text = "";
            
        }

        private void return_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
