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
    /// Interaction logic for editStageDialog.xaml
    /// </summary>
    public partial class editStageDialog : Window
    {
        public editStageDialog()
        {
            InitializeComponent();
        }

        public editStageDialog(string name, double dist, byte breaks)
        {
            InitializeComponent();
            editStageName.Text = name;
            editDistance.Text = dist.ToString();
            editBreakMinutes.Text = breaks.ToString();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            editStageName.SelectAll();
            editDistance.SelectAll();
            editBreakMinutes.SelectAll();
            editStageName.Focus();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Answer
        {
            get
            {
                return editStageName.Text + "," + editDistance.Text + "," + editBreakMinutes.Text;
            }
        }
    }
}
