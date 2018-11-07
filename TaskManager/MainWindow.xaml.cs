using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;
using TaskManager.ViewModels;

namespace TaskManager
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ProcessesViewModel ProcessesVM;

        public MainWindow()
        {
            InitializeComponent();
            this.ProcessesVM = new ProcessesViewModel();
            this.DataContext = this.ProcessesVM;
            Process.GetProcesses().ToList().ForEach(this.ProcessesVM.Processes.Add);
            var dTForUpdatingLiOfProcesses = new DispatcherTimer();
            dTForUpdatingLiOfProcesses.Tick += new EventHandler(DispatcherTimerForUpdatingLiOfProcesses_Tick);
            dTForUpdatingLiOfProcesses.Interval = new TimeSpan(0, 0, 30);
            dTForUpdatingLiOfProcesses.Start();
        }

        private void DispatcherTimerForUpdatingLiOfProcesses_Tick(object sender, EventArgs e)
        {
            this.ProcessesVM.Processes.Clear();
            Process.GetProcesses().ToList().ForEach(this.ProcessesVM.Processes.Add);
            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
