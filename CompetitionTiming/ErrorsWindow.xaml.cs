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
    /// Interaction logic for ErrorsWindow.xaml
    /// </summary>
    public partial class ErrorsWindow : Window
    {
        public ErrorsWindow()
        {
            InitializeComponent();
            errorsTextBox.Text = "";
        }

        public ErrorsWindow(string value)
        {
            InitializeComponent();
            errorsTextBox.Text = value;

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            errorsTextBox.Focus();
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
            errorsTextBox.Text = "";
        }

        private void return_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
