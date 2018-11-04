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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RAllyClasses;
using Microsoft.Win32;
using System.IO;



namespace CompetitionTiming
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Rally myRally;
        List<Stage> Stages = new List<Stage>();
        List<Competitor> Competitors = new List<Competitor>();
        OpenFileDialog openFileDialog;
        int maxtimeallowed = 15;
        byte latePenalty = 50;
        byte missedpenalty = 50;
        int interval = 20;
        int replicate = 1;
        string errors ="";
        string messages = "";

        public MainWindow()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// Called whenever the program is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult key = MessageBox.Show(
                "Are you sure you want to quit",
                "Confirm Close",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);
            if (key == MessageBoxResult.Yes)
            {
                key = MessageBox.Show(
                "Do you want to save before closing",
                "Save Data",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);
                if (key == MessageBoxResult.Yes)
                { 
                    // call save method
                }
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
            
        }

        private void editSetup_Click(object sender, RoutedEventArgs e)
        {
          if (myRally ==null)
            {
                myRally = new Rally("event Name",2015,5,1,10,0,0,0,24,0);
            }
            editSetupDialog win = new editSetupDialog(myRally.getEventName(), myRally.getTotalDistance(),
                                                                myRally.getEventSpeed(), myRally.getEventStart());
            if (win.ShowDialog() == true)
            {
                string[] fields = win.Answer.Split(',');
                EventTitle.Content = fields[0];
                myRally.setEventName(fields[0]);
                myRally.setTotalDistance(Double.Parse(fields[1]));
                myRally.setEventSpeed(byte.Parse(fields[2]));
                string[] dates = fields[3].Split('/');
                string[] times = fields[4].Split(':');
                int yr = Int32.Parse(dates[2]);
                int mnt = Int32.Parse(dates[1]);
                int dy = Int32.Parse(dates[0]);
                int hr = Int32.Parse(times[0]);
                int mn = Int32.Parse(times[1]);
                int sc = Int32.Parse(times[2]);
                myRally.setDateTime(yr, mnt, dy, hr, mn, sc);
                
                // udate display to show new details
                EventTitle.Content = myRally.getEventName();
                distanceLabel.Content = myRally.getTotalDistance().ToString();
                speedLabel.Content = myRally.getEventSpeed().ToString();
                startDateTimeLabel.Content = myRally.getEventStart().ToString();
            }
        }

        private void editStages_Click(object sender, RoutedEventArgs e)
        {
            Stage StageToEdit;
            if (StagesComboBox.SelectedItem != null)
            {
                int index = StagesComboBox.SelectedIndex;
                StageToEdit = (Stage) StagesComboBox.SelectedItem;
                editStageDialog dialog = new editStageDialog(StageToEdit.getStageName(), StageToEdit.getStageDistance(),
                                               StageToEdit.getStageBreak());
                 if (dialog.ShowDialog() == true)
                 {
                     string[] fields = dialog.Answer.Split(',');
                // need a way to edit the item in the collection not the copy StageToEdit.
               // change to EventTitle is just for testing to show method returns
                     Stages[index].setStageName(fields[0]);
                     Stages[index].setStageDistance(Double.Parse(fields[1]),myRally.getEventSpeed());
                     Stages[index].setStageBreakMinutes(byte.Parse(fields[2]));
                     Stages[index].setTimeDelay(myRally.getEventSpeed());
                     StagesComboBox.ItemsSource = null;
                     StagesComboBox.ItemsSource = Stages;
                     StagesComboBox.SelectedIndex = index;
                     // need some way to set total distance in myRally if it is changed here
                 }
            }
           
        }

        private void editCompetitor_Click(object sender, RoutedEventArgs e)
        {
            Competitor CompToEdit;
            if (CompetitorList.SelectedItem != null)
            {
                int index = CompetitorList.SelectedIndex;
                CompToEdit = Competitors[index];
                editCompetitorDialog dialog = new editCompetitorDialog(CompToEdit.getCompetitorsName(), 
                                                                       CompToEdit.getCompetitorsMotorcycle());
                if (dialog.ShowDialog() == true)
                {
                    string[] fields = dialog.Answer.Split(',');
                    Competitors[index].setCompetitorsName(fields[0]);
                    Competitors[index].setCompetitorsMotorcycle(fields[1]);
                    CompetitorList.ItemsSource = null;
                    CompetitorList.ItemsSource = Competitors;
                }
            }

        }

        private void editTime_Click(object sender, RoutedEventArgs e)
        {

        }

        private void showCompetitors_Click(object sender, RoutedEventArgs e)
        {
           // change to output to secondDoc
            Paragraph para = new Paragraph();
            Bold boldtext = new Bold();
            Run thistext = new Run();

            secondDoc.Blocks.Clear();

            para.Margin = new Thickness(30, 0, 0, 0);
            para.FontSize = 20;
                        
            boldtext.Inlines.Add(new Run(myRally.getEventName() + "    " +  '\n'));
            para.Inlines.Add(boldtext);
                        
            para.Inlines.Add(new Run("   " + '\n'));

            para.Inlines.Add(new Run("                Competitors"));

            para.Inlines.Add(new Run("  " + '\n'));

            secondDoc.Blocks.Add(para);
         
            
            Table pointTable;
            TableRow currentrow;

            foreach (Competitor c in myRally.Competitors)
            {
                para = new Paragraph();
                para.Margin = new Thickness(30, 0, 0, 0);
                para.FontSize = 16;
                para.FontWeight = FontWeights.Normal;
                // para.Inlines.Add(".............................." + '\n');
                para.Inlines.Add(new Run("" + '\n'));
                para.Inlines.Add(new Run("No. " + c.getEntrantNumber() + ":  " + c.getCompetitorsName() + " riding " + c.getCompetitorsMotorcycle()
                    + "  Total Points scored = " + c.getTotal() + "Starting " + c.getStartTime().ToLongTimeString()));
                // para.Inlines.Add("Started at : " + c.getStartTime().ToLongTimeString() + "  " + "\n\r");
                // para.Inlines.Add("points scored\n\r");
                
                secondDoc.Blocks.Add(para);

                if (c.CompetitorTimings.Count >0)
                {   
                    pointTable = new Table();
                    pointTable.Margin = new Thickness(60, 0, 0, 0);
                    secondDoc.Blocks.Add(pointTable);
                    pointTable.Columns.Add(new TableColumn());
                    pointTable.Columns[0].Width = new GridLength(100);
                    pointTable.Columns.Add(new TableColumn());
                    pointTable.Columns[0].Width = new GridLength(300);
                    pointTable.Columns.Add(new TableColumn());
                    pointTable.Columns[0].Width = new GridLength(300);
                    pointTable.Columns.Add(new TableColumn());
                    pointTable.Columns[0].Width = new GridLength(100);

                   // add header row
                    pointTable.RowGroups.Add(new TableRowGroup());
                    pointTable.RowGroups[0].Rows.Add(new TableRow());
                    currentrow = pointTable.RowGroups[0].Rows[0];
                    currentrow.FontWeight = FontWeights.Bold;
                    currentrow.FontSize = 18;
                    currentrow.Cells.Add(new TableCell(new Paragraph(new Run("Stage #"))));
                    currentrow.Cells.Add(new TableCell(new Paragraph(new Run("CP Location"))));
                    currentrow.Cells.Add(new TableCell(new Paragraph(new Run("Time at CP"))));
                    currentrow.Cells.Add(new TableCell(new Paragraph(new Run("Penalty Points"))));
                    int rownumber = 1;
                    foreach (TimingEvent timed in c.CompetitorTimings)
                    {
                        // output.AppendText(t.getRank().ToString() + " " + t.ToString() +'\n');
                        pointTable.RowGroups[0].Rows.Add(new TableRow());
                        currentrow = pointTable.RowGroups[0].Rows[rownumber];
                        currentrow.Cells.Add(new TableCell(new Paragraph(new Run(timed.getRank().ToString()))));
                        currentrow.Cells.Add(new TableCell(new Paragraph(new Run("  "))));
                        currentrow.Cells.Add(new TableCell(new Paragraph(new Run(timed.TimedAt().ToShortTimeString()))));
                        currentrow.Cells.Add(new TableCell(new Paragraph(new Run(timed.getPoints().ToString()))));
                        rownumber++;
                    }
                    // add blank row
                    pointTable.RowGroups[0].Rows.Add(new TableRow());
                    currentrow = pointTable.RowGroups[0].Rows[rownumber];
                    currentrow.Cells.Add(new TableCell(new Paragraph(new Run("  "))));
                    currentrow.Cells[0].ColumnSpan = 4;
                    currentrow.Cells[0].TextAlignment = TextAlignment.Center;
                } // end if
       

               
            } //  End foreach
            // turn on printing
            printButton.IsEnabled = true;
            menuPrint.IsEnabled = true;
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {

        }

       /// <summary>
        /// This method is called to read the setup file
        /// The setup file holds the name of the event, the distance and the speed.
        /// It also holds details on each stage.
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
         private void setupFile_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.FileOk += openSetupFileDialogFileOk;
            openFileDialog.Title = "Open Setup File";
            openFileDialog.Filter = "Setup Files(*.sup)|*.sup|All Files(*.*)|*.*";
           
            openFileDialog.ShowDialog();
         }

           
        

        private void compFile_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.FileOk += openEntrantsFileDialogFileOk;
            openFileDialog.Title = "Open Entrant File";
            openFileDialog.Filter = "Entrant Files(*.csv)|*.csv|All Files(*.*)|*.*";

            openFileDialog.ShowDialog();
        }

        private void timeFile_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.FileOk +=openTimeFileDialog_FileOk;
            openFileDialog.Title = "Open Timing File for " + Stages[StagesComboBox.SelectedIndex].getStageName();
            openFileDialog.Filter = "Timing Files(*.txt)|*.txt|All Files(*.*)|*.*";

            openFileDialog.ShowDialog();
            // see if timings have been added for all stages 
            Boolean still_awaiting_timings = false;
            foreach (Stage testStage in Stages) 
            {
                if (testStage.LocationTimings == null)
                {
                    still_awaiting_timings = true;
                }
            }
            if (still_awaiting_timings == false)
            {
                calcButton.IsEnabled = true;
            }




        }

        private void calcButton_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan SectionTime, Timetaken, timeError;

            DateTime startTime = new DateTime();
            DateTime finishTime = new DateTime(); ;
            int startTimingEvent, finishTimingEvent;
            byte points;

            secondDoc.Blocks.Clear();
            Paragraph para = new Paragraph();
            para.Margin = new Thickness(30, 0, 0, 0);
            para.FontSize = 20;

            para.Inlines.Add(new Run("Calculations in progress \n\r\n"));
            secondDoc.Blocks.Add(para);
            Paragraph StagePara = new Paragraph();
            StagePara.Margin = new Thickness(30, 0, 0, 0);
            StagePara.FontSize = 16;

            Paragraph compPara = new Paragraph();
            compPara.Margin = new Thickness(30, 0, 0, 0);
            compPara.FontSize = 14;
            
            foreach (Stage section in Stages) 
            {
                // compPara.Inlines.Clear();
                
                SectionTime = new TimeSpan(0, section.getStageTimeMinutes(), section.getStageTimeSeconds());
                // output.AppendText(section.getStageName() + " " + section.getStageRank()+"  " + SectionTime.ToString() + '\n');

                compPara.Inlines.Add(new Run("Stage " + section.getStageRank() + ": " + section.getStageName() + "| Set time =  " + SectionTime.ToString() + '\n'));
                // secondDoc.Blocks.Add(compPara);
                messages += "Stage "+ section.getStageRank()+": "+ section.getStageName()+ "| Set time =  " + SectionTime.ToString() + '\n';

                for (int record = 0; record < myRally.Competitors.Count; record++)
                {
                   // compPara.Inlines.Clear();
                    // output.AppendText("Working on "+ myRally.Competitors[record].getCompetitorsName() + '\n');
                    compPara.Inlines.Add(new Run(myRally.Competitors[record].getCompetitorsName() + '\n'));
                    messages +=  myRally.Competitors[record].getCompetitorsName() + '\n';
                    // find the time at the beginning of the section
                    startTimingEvent = -1;
                    finishTimingEvent = -1;

                    if (section.getStageRank() ==1)
                    {
                        // first stage; therefore starttime is competitors start time
                        startTime = myRally.Competitors[record].getStartTime();
                        startTimingEvent = 0;
                    }
                    else
                    {
                        // starttime is the clcoked time at previous stage .i.e. stage rank = section.getrank -1
                        int lookingforbeginStage = section.getStageRank() - 1;
                        int t = 0;
                        while (t < myRally.Competitors[record].CompetitorTimings.Count && startTimingEvent <0)
                        {
                            if (myRally.Competitors[record].CompetitorTimings[t].getRank() == lookingforbeginStage)
                            {
                                startTimingEvent = t;
                                startTime = myRally.Competitors[record].CompetitorTimings[t].TimedAt();
                            }
                            t++;

                        } // end while
                        
                    } // end else . end of the search for starttime

                    // Search for finishtime
                    int lookingforfinishStage = section.getStageRank();
                    int ft = 0;
                    while (ft < myRally.Competitors[record].CompetitorTimings.Count && finishTimingEvent < 0)
                    {
                        if (myRally.Competitors[record].CompetitorTimings[ft].getRank() == lookingforfinishStage)
                        {
                            finishTimingEvent = ft;
                            finishTime = myRally.Competitors[record].CompetitorTimings[ft].TimedAt();
                        }
                        ft++;

                    } // end while . end search for finish time

                    // Calculaue the points
                    // Can calculaue only if both startTime and finishTime is found
                    if (startTimingEvent != -1 && finishTimingEvent != -1)
                    {
                        // timings found so can calculate points
                        Timetaken = new TimeSpan();
                        Timetaken = finishTime.Subtract(startTime).Duration();
                        messages += " TimeTaken = " + Timetaken.ToString() + " ( finish " + finishTime.ToLongTimeString() + " - start " + startTime.ToLongTimeString() + " )\n";
                        compPara.Inlines.Add(new Run(" TimeTaken = " + Timetaken.ToString() + " ( finish " + finishTime.ToLongTimeString() + " - start " + startTime.ToLongTimeString() + " )\n"));
                        // output.AppendText(" TimeTaken = "+ Timetaken.ToString()+" ( finish "+ finishTime.ToString() + " - start " +startTime.ToString() + " )\n\r");
                        if (Timetaken > SectionTime)
                            timeError = Timetaken.Subtract(SectionTime);
                        else
                            timeError = SectionTime.Subtract(Timetaken);
                        points = (byte)timeError.TotalMinutes;
                        if (points > maxtimeallowed)
                        {
                            points = latePenalty;
                        }
                        messages += "Time error = " + timeError.ToString()+ " ( " + Timetaken.ToString() +" - " +SectionTime.ToString()+ " ).  Points scored = " +  points.ToString()+"\n\r\n";
                        compPara.Inlines.Add(new Run("Time error = " + timeError.ToString() + " ( " + Timetaken.ToString() + " - " + SectionTime.ToString() + " ).  Points scored = " + points.ToString() + "\n\r\n"));
                        // output.AppendText("Taken " + Timetaken.ToString() + " time error = " + timeError.ToString() +   "Points scored = " +  points.ToString());

                    }
                    else
                    {
                        // either start or finish time event not found so maximun points
                        points = missedpenalty;
                        messages += "No Time recorded for section start or ending " + "Points scored = " + points.ToString() + "\n\r\n";
                        compPara.Inlines.Add(new Run("No Time recorded for section start or ending " + "Points scored = " + points.ToString() + "\n\r\n"));
                        // outp.ut.AppendText("No Time recorded for section start or ending " + "Points scored = " + points.ToString());
                    }

                    // Update points scored in timing event and in competitor record
                    if (finishTimingEvent != -1)
                    {
                        myRally.Competitors[record].CompetitorTimings[finishTimingEvent].setPoints(points);
                    }
                    
                    myRally.Competitors[record].addPoint(section.getStageRank(), points);
                    myRally.Competitors[record].addUpTotal();
                    // messages += "  total points scored =" + myRally.Competitors[record].getTotal() + '\n';
                    // output.AppendText("  total points scored =" + myRally.Competitors[record].getTotal() + '\n');

                    

                } // end for record competitor
            } // end foreach stage

            secondDoc.Blocks.Add(compPara);
            compFile.IsEnabled = false;
            menuCompFile.IsEnabled = false;
            timeFile.IsEnabled = false;
            menuTimeFile.IsEnabled = false;
            calcButton.IsEnabled = false;
            scoreButton.IsEnabled = true;
            resultButton.IsEnabled = true;
            printButton.IsEnabled = true;

            MessageBoxResult keys = MessageBox.Show(
                "Results and scores have been calculated.\nClick on the Score or result button to see results.",
                "Calculation completed",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK);

        } // end calcButton_click

        private void scoreButton_Click(object sender, RoutedEventArgs e)
        {
            Paragraph para = new Paragraph();
            Bold boldtext = new Bold();
            Run thistext = new Run();

            secondDoc.Blocks.Clear();

            para.Margin = new Thickness(30, 0, 0, 0);
            para.FontSize = 20;

            
            boldtext.Inlines.Add(new Run(myRally.getEventName() + "    " + myRally.getEventStart().ToLongDateString() + '\n'));
            para.Inlines.Add(boldtext);

            para.Inlines.Add( new Run("Event Distance:  " + myRally.getTotalDistance() + " Miles.      Regulated Speed: " + myRally.getEventSpeed() + " MPH\n"));
            para.Inlines.Add( new Run("   " + '\n'));
            
            
            para.Inlines.Add(new Run("                 Competitors Scores"));
           
            para.Inlines.Add(new Run("  " + '\n'));

            secondDoc.Blocks.Add(para);

            para = new Paragraph();
            para.Margin = new Thickness(30, 0, 0, 0);
           
            byte rank;
            TimingEvent TE;
            Table pointTable;
            TableRow currentrow;

            foreach (Competitor c in myRally.Competitors)
            {
                para = new Paragraph();
                para.Margin = new Thickness(30, 0, 0, 0);
                para.FontSize = 18;
                para.FontWeight = FontWeights.Bold;
                // para.Inlines.Add(".............................." + '\n');
                para.Inlines.Add(new Run("" + '\n'));
                para.Inlines.Add(new Run("No. " + c.getEntrantNumber() + ":  " + c.getCompetitorsName() + " riding " + c.getCompetitorsMotorcycle()
                    + "  Total Points scored = " + c.getTotal()));
                // para.Inlines.Add("Started at : " + c.getStartTime().ToLongTimeString() + "  " + "\n\r");
                // para.Inlines.Add("points scored\n\r");
                secondDoc.Blocks.Add(para);

                pointTable = new Table();
                pointTable.Margin = new Thickness(60, 0, 0, 0);
                secondDoc.Blocks.Add(pointTable);
                pointTable.Columns.Add(new TableColumn());
                pointTable.Columns[0].Width = new GridLength(100);
                pointTable.Columns.Add(new TableColumn());
                pointTable.Columns[0].Width = new GridLength(300);
                pointTable.Columns.Add(new TableColumn());
                pointTable.Columns[0].Width = new GridLength(300);
                pointTable.Columns.Add(new TableColumn());
                pointTable.Columns[0].Width = new GridLength(100);

                // add header row
                pointTable.RowGroups.Add(new TableRowGroup());
                pointTable.RowGroups[0].Rows.Add(new TableRow());
                currentrow = pointTable.RowGroups[0].Rows[0];
                currentrow.FontWeight = FontWeights.Bold;
                currentrow.FontSize = 18;
                currentrow.Cells.Add(new TableCell(new Paragraph(new Run("Stage #"))));
                currentrow.Cells.Add(new TableCell(new Paragraph(new Run("CP Location"))));
                currentrow.Cells.Add(new TableCell(new Paragraph(new Run("Time at CP"))));
                currentrow.Cells.Add(new TableCell(new Paragraph(new Run("Penalty Points"))));


                for (int n = 0; n < Stages.Count; n++)
                {
                    pointTable.RowGroups[0].Rows.Add(new TableRow());
                    currentrow = pointTable.RowGroups[0].Rows[n+1];
                    currentrow.Cells.Add(new TableCell(new Paragraph(new Run(Stages[n].getStageRank().ToString()))));
                    currentrow.Cells.Add(new TableCell(new Paragraph(new Run(Stages[n].getStageName()))));
                    
                    // para.Inlines.Add("Stage " + Stages[n].getStageRank()+"  "+Stages[n].getStageName() +"\t");
                    rank = Stages[n].getStageRank();
                    TE = FindTEbyRank(rank, c.CompetitorTimings);
                    if (TE != null)
                    {
                        currentrow.Cells.Add(new TableCell(new Paragraph(new Run(TE.TimedAt().ToLongTimeString()))));
                        currentrow.Cells.Add(new TableCell(new Paragraph(new Run(TE.getPoints().ToString()))));
                       //  para.Inlines.Add("Timed at " + TE.TimedAt().ToLongTimeString() + " Scored= " + TE.getPoints() + " Points\n\r");
                    }
                    else
                    {
                        currentrow.Cells.Add(new TableCell(new Paragraph(new Run("NOT TIMED"))));
                        currentrow.Cells.Add(new TableCell(new Paragraph(new Run(c.getPoints(rank).ToString()))));
                        // para.Inlines.Add("No time recorded! Scored = " + c.getPoints(rank) + " Points\n\r");
                    }
                    
                }

                pointTable.RowGroups[0].Rows.Add(new TableRow());
                // alisase last row ( count-1)
                int r = pointTable.RowGroups[0].Rows.Count;
                currentrow = pointTable.RowGroups[0].Rows[r - 1];
                currentrow.Cells.Add(new TableCell(new Paragraph(new Run("--------------------------"))));
                 currentrow.Cells[0].ColumnSpan = 4;
                currentrow.Cells[0].TextAlignment = TextAlignment.Center;
                
               
            }

            printButton.IsEnabled = true;
            menuPrint.IsEnabled = true;
        }

        private void resultButton_Click(object sender, RoutedEventArgs e)
        {
            // THIS PRINTS THE RESULTS IN FINISHING ORDER
            // use LINQ to sort competitors by points scored
            // Only sort competitors who scored less than the maximum point (max point =  NoOfStages * 50
            // do the Linq query
            int rownumner = 0;
            int maxpoints = Stages.Count * 50;
            int finishingPosition = 0;
            int rider = 1;
            short currentpointscore = -1;
            Boolean joint = false;
            string jtext = "";
            var subset = from Competitor theRider in myRally.Competitors
                         where theRider.getTotal() < maxpoints
                         orderby theRider.getTotal()
                         select theRider;


            Paragraph para = new Paragraph();
            Bold boldtext = new Bold();
            Run thistext = new Run();

            secondDoc.Blocks.Clear();
            
            para.Margin = new Thickness(24, 0, 0, 0);
            para.FontSize = 28;
            
            thistext.Text = myRally.getEventName() +"    "+ myRally.getEventStart().ToShortDateString()+'\n';
            boldtext.Inlines.Add( myRally.getEventName() +"    "+ myRally.getEventStart().ToShortDateString()+'\n');
            para.Inlines.Add(boldtext);

            para.Inlines.Add("Event Distance:  " + myRally.getTotalDistance()+ " Miles.      Regulated Speed: " + myRally.getEventSpeed()+" MPH\n");
            
            secondDoc.Blocks.Add(para);

            
            
            Table resulttable= new Table();
            resulttable.Margin = new Thickness(60, 0, 0, 0);
            secondDoc.Blocks.Add(resulttable);
            // add 4 columns
            resulttable.Columns.Add(new TableColumn());
            resulttable.Columns[0].Width = new GridLength(100);
            resulttable.Columns.Add(new TableColumn());
            resulttable.Columns[1].Width = new GridLength(100);
            resulttable.Columns.Add(new TableColumn());
            resulttable.Columns[2].Width = new GridLength(300);
            resulttable.Columns.Add(new TableColumn());
            resulttable.Columns[1].Width = new GridLength(100);

            // add title row
            TableRow currentrow;
            resulttable.RowGroups.Add(new TableRowGroup());
            resulttable.RowGroups[0].Rows.Add(new TableRow());
            currentrow = resulttable.RowGroups[0].Rows[rownumner];
            currentrow.Background = Brushes.WhiteSmoke;
            currentrow.FontSize = 30;
            currentrow.FontWeight = FontWeights.Bold;
            currentrow.Cells.Add(new TableCell(new Paragraph(new Run("RESULTS"))));
            currentrow.Cells[0].ColumnSpan = 4;
            currentrow.Cells[0].TextAlignment = TextAlignment.Center;
            
            // add header row
            rownumner++;
            resulttable.RowGroups[0].Rows.Add(new TableRow());
            currentrow = resulttable.RowGroups[0].Rows[rownumner];
            currentrow.FontSize = 18;
            currentrow.FontWeight = FontWeights.Bold;
            currentrow.Background = Brushes.WhiteSmoke;
            currentrow.Cells.Add(new TableCell(new Paragraph(new Run("POSITION"))));
            currentrow.Cells.Add(new TableCell(new Paragraph(new Run("CompNo."))));
            currentrow.Cells.Add(new TableCell(new Paragraph(new Run("NAME"))));
            currentrow.Cells.Add(new TableCell(new Paragraph(new Run("SCORE"))));

             // add blank row
            rownumner++;
            resulttable.RowGroups[0].Rows.Add(new TableRow());
            currentrow = resulttable.RowGroups[0].Rows[rownumner];
            currentrow.FontSize = 18;
            currentrow.FontWeight = FontWeights.Bold;
            currentrow.Background = Brushes.WhiteSmoke;
            currentrow.Cells.Add(new TableCell(new Paragraph(new Run("   "))));
            currentrow.Cells[0].ColumnSpan = 4;
            currentrow.Cells[0].TextAlignment = TextAlignment.Center;
 
            
            
            foreach (Competitor c in subset)
            {

                rownumner++;
                resulttable.RowGroups[0].Rows.Add(new TableRow());
                currentrow = resulttable.RowGroups[0].Rows[rownumner];
                currentrow.FontSize = 14;

                // check if joint position
                if (c.getTotal() > currentpointscore)
                {
                    joint = false;
                    jtext = "";
                    finishingPosition = rider;
                    currentpointscore = c.getTotal();
                }
                else
                {
                    joint = true;
                    jtext = " #";
                }

                if (finishingPosition <4)
                {
                    currentrow.FontWeight = FontWeights.Bold;
                    currentrow.Foreground = Brushes.RoyalBlue;
                    currentrow.Background = Brushes.White;
                }
                switch (finishingPosition)
                {
                    case 1: currentrow.Cells.Add(new TableCell(new Paragraph(new Run("WINNER"+ jtext)))); 
                        break;
                    case 2: currentrow.Cells.Add(new TableCell(new Paragraph(new Run("SECOND" + jtext))));
                        break;
                    case 3: currentrow.Cells.Add(new TableCell(new Paragraph(new Run("THIRD " + jtext))));
                        break;
                    default: currentrow.Cells.Add(new TableCell(new Paragraph(new Run("  " + finishingPosition + "th  " + jtext))));
                        break;
                }

                currentrow.Cells.Add(new TableCell(new Paragraph(new Run(c.getEntrantNumber().ToString()))));
                currentrow.Cells.Add(new TableCell(new Paragraph(new Run(c.getCompetitorsName()))));
                currentrow.Cells.Add(new TableCell(new Paragraph(new Run(c.getTotal().ToString()))));
                
                rider++;
            }
            
            
            Paragraph finalpara = new Paragraph();
            para.Margin = new Thickness(24, 24, 0, 0);
            para.FontSize = 15;
            finalpara.Inlines.Add("# . Joint position. Two or more entrants finished in this position. \n");
            finalpara.Inlines.Add("All other competitors scored the maximum of " + maxpoints + " penalty points.\n");
            finalpara.Inlines.Add(" " + '\n');
            finalpara.Inlines.Add("One single penalty point is awarded for each minute the competitor arrives early or late at a Check Point");
            finalpara.Inlines.Add(" up to a maximum of " + maxtimeallowed + " minutes\n");
            finalpara.Inlines.Add(" Competitors arriving outside of this "+ maxtimeallowed+" minute window receive a late/early penalty of " + latePenalty+" penalty points\n");
            finalpara.Inlines.Add(" Competitors missing the Checkpoint receive a missed penalty of "+ missedpenalty+" penalty points\n");
            secondDoc.Blocks.Add(finalpara);

            printButton.IsEnabled = true;
            menuPrint.IsEnabled = true;
        }

        private void printButton_Click(object sender, RoutedEventArgs e)
        {
            // printCommand();
            docViewer.Print();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void help_Click(object sender, RoutedEventArgs e)
        {
            ApplicationHelp helpScreen = new ApplicationHelp();
            helpScreen.ShowDialog();
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void openSetupFileDialogFileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string fullPathname = openFileDialog.FileName;
            FileInfo src = new FileInfo(fullPathname);

            string line;
            int yr =2014;
            int  mnth = 5;
            int dy = 1;
            int hr =10;
            int mn = 0;
            int sec = 0;
            byte speed = 24;
            double distance = 0;


             // output.Text = "";
            TextReader reader = src.OpenText();
            

            // read first line containing the name of event
            line = reader.ReadLine();
            messages +=line + "\r\n";
            if (line.Contains("name,"))
            {
                string[] fields = line.Split(',');
                EventTitle.Content = fields[1];
            }

            // read second line containing the  speed
            line = reader.ReadLine();
            messages += line + " | ";
            if (line.Contains("speed,"))
            {
                string[] fields = line.Split(',');
                speedLabel.Content = fields[1] +" mph";
                speed = System.Byte.Parse(fields[1]);
            }

            // read third line containing the  date
            line = reader.ReadLine();
            messages += line + " | ";
            if (line.Contains("date,"))
            {
                string[] fields = line.Split(',');
               // dateLabel.Content = fields[1] + fields[2] + fields[3];
                yr = System.Int32.Parse(fields[1]);
                mnth = System.Int32.Parse(fields[2]);
                dy = System.Int32.Parse(fields[3]);
            }

            // read fourth line containing the  starttime
            line = reader.ReadLine();
            messages += line + " | ";
            if (line.Contains("starttime,"))
            {
                string[] fields = line.Split(',');
               // startDateTimeLabel.Content = fields[1] + fields[2] + fields[3];
                hr = System.Int32.Parse(fields[1]);
                mn = System.Int32.Parse(fields[2]);
                sec = System.Int32.Parse(fields[3]);
            }
            // read fifth line containing the  distance
            line = reader.ReadLine();
            messages += line + "\r\n";
            if (line.Contains("distance,"))
            {
                string[] fields = line.Split(',');
                distanceLabel.Content = fields[1] +"miles";
                distance = System.Double.Parse(fields[1]);
            }

            
            myRally = new Rally((string)EventTitle.Content, yr, mnth, dy, hr, mn, sec, distance, speed, 50);
            competitorLabel.Content = myRally.getNoOfCompetitors();
            startDateTimeLabel.Content = myRally.getEventStart().ToString();

            // read the timaallowed and the penaltys
            // read sixth line containing the  timeallowed
            line = reader.ReadLine();
            messages += line + " | ";
            if (line.Contains("timeallowed,"))
            {
                string[] fields = line.Split(',');
                maxtimeallowed = Int32.Parse(fields[1]);
                TimeAllowedLbl.Content = fields[1];
            }

            // read seventh line containing the  latepenalty,
            line = reader.ReadLine();
            messages += line + " | ";
            if (line.Contains("latepenalty,"))
            {
                string[] fields = line.Split(',');
                latePenalty = byte.Parse(fields[1]);
                LatePenaltyLbl.Content = fields[1];
            }

            // read eigth line containing the  missedpenalty,
            line = reader.ReadLine();
            messages += line + "\r\n";
            if (line.Contains("missedpenalty"))
            {
                string[] fields = line.Split(',');
                missedpenalty= byte.Parse(fields[1]);
                missedPenaltyLbl.Content = fields[1];
            }

            // read ninth line containing the  startinterval,
            line = reader.ReadLine();
            messages += line + "\r\n";
            if (line.Contains("startinginterval"))
            {
                string[] fields = line.Split(',');
                interval = Int32.Parse(fields[1]);
                intervalLbl.Content = fields[1];
            }

            // read tenth line containing the  startingtogether,
            line = reader.ReadLine();
            messages += line + "\r\n";
            if (line.Contains("startingtogether"))
            {
                string[] fields = line.Split(',');
                replicate = Int32.Parse(fields[1]);
                replicateLbl.Content = fields[1];
            }


            
            // Now begin to read the stages information in the next few lines
            // each line represents a new stage

             
            Stages.Clear();
            StagesComboBox.Items.Clear();

            messages += "Stages :\r\n";
            line = reader.ReadLine();
           
            while (line != null)
            {
                if (line.Contains("stage,"))
                {
                    string[] fields = line.Split(',');
                    byte rank = System.Byte.Parse( fields[1]);
                    double dist = System.Double.Parse(fields[3]);
                    byte delay = System.Byte.Parse(fields[4]);
                    
                    Stage mystage = new Stage(fields[2],rank,dist,delay, myRally.getEventSpeed());
                    TimeSpan Timedelay = new TimeSpan(0,mystage.getStageTimeMinutes(),mystage.getStageTimeSeconds());
                    Stages.Add(mystage);
                    messages += mystage.ToString() + " TimeSpan =  " + Timedelay.ToString() + "\r\n";
                }
                line = reader.ReadLine();
               //  messages += line + "\r\n";

            }
            reader.Close();
            StagesComboBox.ItemsSource = Stages;
            StagesComboBox.SelectedIndex = 0;
            myRally.ConnectStages(Stages);
            
            messages += "Setup completed \r\n";
            errors += myRally.ToString() + "Setup completed\r\n";

            editSetup.IsEnabled = true;
            menuEditSetup.IsEnabled = true;
            editStages.IsEnabled = true;
            menuEditStage.IsEnabled = true;
            compFile.IsEnabled = true;
            menuCompFile.IsEnabled = true;

            MessageBoxResult key = MessageBox.Show(
                "Details for this event are set and the stages added.\nUse the 'EditSetup' button to change details or 'edit Stages' button to edit a selected Stage.",
                myRally.getEventName(),
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK);
        }



        private void openEntrantsFileDialogFileOk(object sender,  System.ComponentModel.CancelEventArgs e)
        {
             string fullPathname = openFileDialog.FileName;
            FileInfo src = new FileInfo(fullPathname);
            Competitor myComp;
            string line;
            
            
            CompetitorList.Items.Clear();
            Competitors.Clear();
           
            messages += "Entrants from file " + src.FullName + "\r\n";
            TextReader reader = src.OpenText();
            
            // read first line containing the name of event
            line = reader.ReadLine();
            
            while (line != null) 
            {
                string[] fields = line.Split(',');
                byte competitionNumber = System.Byte.Parse(fields[0]);
                myComp = new Competitor(System.Byte.Parse(fields[0]), fields[1],fields[2], myRally.getEventStart(), interval,replicate, (byte)myRally.Stages.Count);
               
                Competitors.Add(myComp);
                messages += myComp.ToString() + "\r\n";
                line = reader.ReadLine();
            }

	        reader.Close();
            myRally.ConnectCompetitors(Competitors);
            CompetitorList.ItemsSource = Competitors;
            competitorLabel.Content = myRally.getNoOfCompetitors();
            MessageBoxResult key = MessageBox.Show(
                myRally.getNoOfCompetitors().ToString() + " entrants added to the Competitor list",
                myRally.getNoOfCompetitors().ToString() + " entrants added",
                MessageBoxButton.OK,
                MessageBoxImage.None,
                MessageBoxResult.OK);

            showCompetitors.IsEnabled = true;
            menuShowCompetitor.IsEnabled = true;
            editCompetitor.IsEnabled = true;
            menuEditComp.IsEnabled = true;
            timeFile.IsEnabled = true;
            menuTimeFile.IsEnabled = true;
            setupFile.IsEnabled = false;
            menuSetupFile.IsEnabled = false;


            messages += myRally.getNoOfCompetitors().ToString() + " entrants added to the Competitor list\n\r\n";
            errors += myRally.getNoOfCompetitors().ToString() + " entrants added to the Competitor list\n\r\n";
            
         }

        private void openTimeFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string fullPathname = openFileDialog.FileName;
            FileInfo src = new FileInfo(fullPathname);
            TimingEvent timing;
            string fileformat;
            DateTime when;

            byte competitornum;
            int yr, mnth, dy, hr, mn, sc;
            string line;
           
            int index = StagesComboBox.SelectedIndex;
            byte where = Stages[index].getStageRank();
            // Stage selectedStage = (Stage)StagesComboBox.SelectedItem;
            // byte where = selectedStage.getStageRank();
            TextReader reader = src.OpenText();

            if (Stages[index].LocationTimings == null)
            {
                Stages[index].LocationTimings = new List<TimingEvent>();
            }

            messages += "Timings for Stage " + where + "\r\n";
            when = new DateTime();
            // read first line containing the name of event
            line = reader.ReadLine();
            // output.AppendText(line + "\r\n");
            while (line != null)
            {
                // string[] fields = line.Split(new Char[] { ',','#','*','-','+',' '});
                // only comma is acceptable splitter for fileds. '/' , ' ' and ':' are used to splite date
                string[] fields = line.Split(',');
                // need to identify here if new or old file
                if (fields[0].Contains("vAn"))  fileformat = fields[0];
                       else fileformat = "vAn01";
                if (fileformat=="vAn10" | fileformat =="vAn11")
                {
                    // new type record
                    competitornum = byte.Parse(fields[2]);
                    try
                    {
                       when = Convert.ToDateTime(fields[4]);
                        
                    }
                    catch (Exception )
                    {
                        messages += "Time  " + competitornum + " "+fields[4]+" not in order. Not added to Com record \r\n";
                        errors += "Time  " + competitornum + " " + fields[4] + " not in order. Not added to Com record \r\n" + where + "\r\n";
                    }
                   
                } else
                {
                    // original record type
                    competitornum = byte.Parse(fields[1]);
                    yr = Int32.Parse(fields[3]);
                    mnth = Int32.Parse(fields[4]);
                    dy = Int32.Parse(fields[5]);
                    hr = Int32.Parse(fields[6]);
                    mn = Int32.Parse(fields[7]);
                    sc = Int32.Parse(fields[8]);
                    when = new DateTime(yr, mnth, dy, hr, mn, sc);
                }
                
                // only try to store information if competition number is valid
                // rest of code is common regardless of file type
                if (competitornum > 0 && competitornum <= myRally.Competitors.Count )
                { 
                    // create timing event
                    timing = new TimingEvent(where, competitornum, when);
                               
                    // only add the record if it is not already entered.
                    if (AlreadyEntered(competitornum, Stages[index].LocationTimings) <0 )
                    {
                        //add timing record to the timing file for this stage
                        Stages[index].LocationTimings.Add(timing);
                       // add timing event to relevent competition record
                       // assumes that competition records are in order and 
                        // record 0 contains competitor 1, etc
                       if (Competitors[competitornum - 1].getEntrantNumber() == competitornum)
                       {
                          Competitors[competitornum - 1].CompetitorTimings.Add(timing);
                          messages += "Competitor " + competitornum + " timed at " + timing.TimedAt().ToLongTimeString() +"\r\n";
                       }
                       else
                       {
                         messages+= "Competitor record " + competitornum + " not in order. Not added to Com record \r\n";
                         errors += "Competitor record " + competitornum + " not in order. Not added to Com record for stage "+ where +"\r\n";

                          MessageBoxResult key = MessageBox.Show(
                          "Competitor numbers are not in order",
                          "Record " + competitornum+" Not added to Competitor record",
                           MessageBoxButton.OK,
                           MessageBoxImage.Error,
                           MessageBoxResult.OK);
                       }
                    } else
                        // this record is already entered. 
                      {
                        int previous = AlreadyEntered(competitornum, Stages[index].LocationTimings);
                        messages += "DUPLICATE:  Competitor #" + competitornum + " Timed at Checkpoint " + where + " at " + timing.TimedAt().ToLongTimeString() + "\r\n";
                        messages += "PREVIOUSLY: Competitor #" + Stages[index].LocationTimings[previous].getCompetitionNumber() + " ";
                        messages += "timed at Checkpoint " + Stages[index].LocationTimings[previous].getRank() + " at " + Stages[index].LocationTimings[previous].TimedAt().ToLongTimeString()+"\r\n";
                        errors += "DUPLICATE:  Competitor #"  + competitornum + " Timed at Checkpoint " + where +" at "+ timing.TimedAt().ToLongTimeString() + "\r\n";
                        errors += "PREVIOUSLY: Competitor #" + Stages[index].LocationTimings[previous].getCompetitionNumber() + " ";
                        errors += "timed at Checkpoint " + Stages[index].LocationTimings[previous].getRank() + " at " + Stages[index].LocationTimings[previous].TimedAt().ToLongTimeString()+"\r\n";
                       
                        MessageBoxResult key = MessageBox.Show(
                        "Duplicate timing for Competitor No " + competitornum + 
                        "\nThis timing at          " + timing.TimedAt().ToLongTimeString() + 
                        "\nPrevious timing at  " + Stages[index].LocationTimings[previous].TimedAt().ToLongTimeString(),
                        "Duplicate record",
                         MessageBoxButton.OK,
                         MessageBoxImage.Error,
                         MessageBoxResult.OK);
                    }
                     
                }
                else
                {
                    // user has entered a number outside the range of entrants.
                    messages += "Competitor number " + competitornum + " Outside range (,0 or >100 \r\n";
                    errors += "Competitor number " + competitornum + " for stage " + where +" : Outside range (,0 or >100 \r\n";
                    MessageBoxResult key = MessageBox.Show(
                    "Competitor "+ competitornum+" out of range ",
                    "Not recorded ",
                     MessageBoxButton.OK,
                     MessageBoxImage.Error,
                     MessageBoxResult.OK);
                }
                

                line = reader.ReadLine();
                // output.AppendText(line + "\r\n");

                 
            } // while


            reader.Close();

            TimingsListBox.ItemsSource = Stages[index].LocationTimings;
            messages += Stages[index].LocationTimings.Count().ToString() + " Timing Events now recorded for " + Stages[index].getStageName() + " Stage\n\r\n";
            //output.AppendText(myRally.ToString());
            MessageBoxResult key2 = MessageBox.Show(
                    Stages[index].LocationTimings.Count().ToString() + " Timing Events now recorded for " + Stages[index].getStageName() + " Stage",
                    "Timing Events added to " + Stages[index].getStageName()+ " Stage",
                    MessageBoxButton.OK,
                    MessageBoxImage.None,
                    MessageBoxResult.OK);

            // increment index to save time next input
            if (index < StagesComboBox.Items.Count)
            {
                StagesComboBox.SelectedIndex = index + 1;
            }
            else
            {
                StagesComboBox.SelectedIndex=0;
            }
            // unselect competitor button
            compFile.IsEnabled = false;

        } // end opentime file

        private Boolean readFileversion_10or11()
        {
            return false;
        }

        private int AlreadyEntered(int C_number, List<TimingEvent> listing)
        {
            
            int index =0;
            int record = -1;
            while (record <0 && index < listing.Count )
            {
                if (listing[index].getCompetitionNumber() == C_number)
                {
                    
                    record = index;
                }
                index++;
            }
            return record; ;
        }

        private void StagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void StagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = StagesComboBox.SelectedIndex;
            
            //  if (Stages[index].LocationTimings != null)
           //  {
            //    TimingsListBox.ItemsSource = Stages[index].LocationTimings;
           // } 
        }

        private void printCommand()
        {
            PrintDialog pdlg = new PrintDialog();
            if (pdlg.ShowDialog()== true)
            {
                Thickness margin = new Thickness(96, 96, 96, 96);
                pdlg.PrintDocument((((IDocumentPaginatorSource)secondDoc).DocumentPaginator), "printint as paginator");
            }
        }

        private TimingEvent FindTEbyRank(byte rank, List<TimingEvent> TEList)
        {
            int found = -1;
            int n =0;
            while (n < TEList.Count && found == -1)
            {
                if (TEList[n].getRank() == rank)
                {
                    found = n;
                }
                n++;
            }
            if (found == -1)
            {
                return null;
            }
            else
            {
                return TEList[found];
            }
        }

       
        private void messages_Click(object sender, RoutedEventArgs e)
        {
            // messsage window
            MessageWindow messageDialog = new MessageWindow(messages);
            messageDialog.Show();
        }

        private void errors_Click(object sender, RoutedEventArgs e)
        {
            // errors window
            ErrorsWindow errorsDialog = new ErrorsWindow(errors);
            errorsDialog.Show();
        }

    }
}
