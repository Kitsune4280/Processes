using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Processes
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static List<ProcessInfo> CurrProcesses = new List<ProcessInfo>();
        static List<ScheduledApplication> ScheduledApps = new List<ScheduledApplication>();
        static ProcessList data = new ProcessList();
        static Schedule schedule = new Schedule();
        
        public MainWindow()
        {
            InitializeComponent();
            data.RefreshData();
            ProcessList.ItemsSource = data.Processes;
            scheduleBox.ItemsSource = schedule.Apps;
            ScheduleBtn.IsEnabled = false;
            DispatcherTimer refreshDataTimer = new DispatcherTimer();
            refreshDataTimer.Tick += new EventHandler(refreshDataTimer_Tick);
            refreshDataTimer.Interval = new TimeSpan(0, 0, 1);
            
            DispatcherTimer schedulerTimer = new DispatcherTimer();
            schedulerTimer.Tick += new EventHandler(schedulerTimer_Tick);
            schedulerTimer.Interval = new TimeSpan(0, 0, 1);

            refreshDataTimer.Start();
            schedulerTimer.Start();
            //data.RefreshData();

        }

        #region List
        private void refreshDataTimer_Tick(object sender, EventArgs e)
        {
            Task refreshProcListTask = new Task(() => data.RefreshData());
            refreshProcListTask.Start();
            CurrProcesses = data.Processes;
            ProcessList.ItemsSource = CurrProcesses;
        }
        #endregion

        #region CMD
        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] separator = { ", " };
                string[] commandLine = cmdLine.Text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (commandLine.Length > 2)
                {
                    MessageBox.Show("Wrong number of parameters", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (commandLine.Length == 1)
                {
                    Process.Start(commandLine[0]);
                }
                else
                {
                    Process.Start(commandLine[0], commandLine[1]);
                }
                cmdLine.Text = "***.exe, args";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cmdLine_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(startBtn != null)
            {
                if (string.IsNullOrEmpty(cmdLine.Text))
                    startBtn.IsEnabled = false;
                else
                    startBtn.IsEnabled = true;
            }
        }

        #endregion

        #region Scheduler

        private void schedulerTimer_Tick(object sender, EventArgs e)
        {
            Task scheduleTask = new Task(() => CheckSchedule());
            scheduleTask.Start();
            scheduleBox.ItemsSource = schedule.Apps;
        }

        private void CheckSchedule()
        {
            try
            {
                foreach (ScheduledApplication app in schedule.Apps)
                {
                    if (app.ScheduledTime > DateTime.Now)
                        break;
                    else
                    {
                        schedule.ExecuteApp(app);
                        //schedule.DeleteApp(app);
                        
                    }
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void openFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Executables (*.exe)|*.exe";
            if(dialog.ShowDialog() == true)
            {
                AppPathBox.Text = dialog.FileName;
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            AppPathBox.Text = "";
            HoursBox.Text = "";
            MinutesBox.Text = "";
        }

        private void AddToSchedule()
        {
            try
            {
                ScheduledApplication app = new ScheduledApplication();
                app.Name = AppPathBox.Text;
                app.ScheduledTime = DateTime.Now;
                TimeSpan ts = new TimeSpan(int.Parse(HoursBox.Text) - DateTime.Now.Hour,
                    int.Parse(MinutesBox.Text) - DateTime.Now.Minute, 0);
                app.ScheduledTime += ts;
                schedule.AddApplication(app);
                scheduleBox.ItemsSource = schedule.Apps;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ScheduleBtn_Click(object sender, RoutedEventArgs e)
        {
            AddToSchedule();
            ScheduledApps = schedule.Apps;
            AppPathBox.Text = "";
            HoursBox.Text = "";
            MinutesBox.Text = "";
        }

        private bool CanSave()
        {
            int hh, mm;
            bool successHH = int.TryParse(HoursBox.Text, out hh);
            bool successMM = int.TryParse(MinutesBox.Text, out mm);
            if (string.IsNullOrEmpty(HoursBox.Text) || string.IsNullOrEmpty(MinutesBox.Text)
                || string.IsNullOrEmpty(AppPathBox.Text) //|| !File.Exists(AppPathBox.Text)
                || !successHH || !successMM || hh < 0 || hh > 23 || mm < 0 || mm > 59
                || hh < DateTime.Now.Hour || (hh == DateTime.Now.Hour && mm < DateTime.Now.Minute))
                return false;
            else
                return true;
        }

        private void TimeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CanSave())
                ScheduleBtn.IsEnabled = true;
            else
                ScheduleBtn.IsEnabled = false;
        }

        private void AppPathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(ScheduleBtn != null)
            {
                if (CanSave())
                    ScheduleBtn.IsEnabled = true;
                else
                    ScheduleBtn.IsEnabled = false;
            }
            

        }
        #endregion
    }
}
