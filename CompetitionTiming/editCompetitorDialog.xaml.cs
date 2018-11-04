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
    /// Interaction logic for editCompetitorDialog.xaml
    /// </summary>
    public partial class editCompetitorDialog : Window
    {
        public editCompetitorDialog()
        {
            InitializeComponent();
        }

        public editCompetitorDialog(string name, string machine)
        {
            InitializeComponent();
            editName.Text = name;
            editMotorcycle.Text = machine;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            editName.SelectAll();
            editMotorcycle.SelectAll();
            editName.Focus();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Answer
        {
            get
            {
                return editName.Text + "," + editMotorcycle.Text;
            }

        }
    }
}
