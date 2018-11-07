using GalaSoft.MvvmLight.Command;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using TaskManager.ViewModels;

namespace TaskManager
{
    /// <summary>
    /// Logika interakcji dla klasy ManagingProcessesWindow.xaml
    /// </summary>
    public partial class ManagingProcessesWindow : Window
    {
        public ManagedProcessesViewModel ManagedProcessesVM;

        public ManagingProcessesWindow()
        {
            InitializeComponent();
            this.ManagedProcessesVM = new ManagedProcessesViewModel();
            this.DataContext = this.ManagedProcessesVM;
            var dTForUpdatingWindow = new DispatcherTimer();
            dTForUpdatingWindow.Tick += new EventHandler(DispatcherTimerForUpdatingWindow_Tick);
            dTForUpdatingWindow.Interval = new TimeSpan(0, 0, 10);
            dTForUpdatingWindow.Start();
        }

        private void DispatcherTimerForUpdatingWindow_Tick(object sender, EventArgs e)
        {
            ManagedProcessesVM.IsLabelInfoVisible = false;
            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
