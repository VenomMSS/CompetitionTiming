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
    /// Interaction logic for editSetupWindow.xaml
    /// </summary>
    public partial class editSetupDialog : Window
    {
        public editSetupDialog()
        {
            InitializeComponent();
        }

        public editSetupDialog(string name, double distance, byte speed, DateTime starttime)
        {
            InitializeComponent();
            editEventName.Text = name;
            editDistance.Text = distance.ToString();
            editSpeed.Text = speed.ToString();
            editDate.Text = starttime.Date.ToShortDateString();
            editTime.Text = starttime.TimeOfDay.ToString();

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; 
            // this.Close();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            // this.Close();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            editEventName.SelectAll();
            editDistance.SelectAll();
            editSpeed.SelectAll();
            editDate.SelectAll();
            editTime.SelectAll();
            editEventName.Focus();
        }

        public string Answer
        {
            get { return editEventName.Text +","+ editDistance.Text +","+ editSpeed.Text
                  +","+ editDate.Text +","+ editTime.Text; }
        }
    }
}
