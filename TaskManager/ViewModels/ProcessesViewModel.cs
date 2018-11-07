using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TaskManager.ViewModels
{ 
    public class ProcessesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Process> Processes { get; set; }

        private bool _IsListBoxModulesVisible;
        private bool _IsListBoxThreadsVisible;
        private bool _IsManagingProcessesWindowVisible;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool IsManagingProcessesWindowVisible
        {
            get { return _IsManagingProcessesWindowVisible; }
            set
            {
                _IsManagingProcessesWindowVisible = value;
                OnPropertyChanged("IsManagingProcessesWindowVisible");
            }
        }

        public bool IsListBoxModulesVisible
        {
            get { return _IsListBoxModulesVisible; }
            set
            {
                _IsListBoxModulesVisible = value;
                OnPropertyChanged("IsListBoxModulesVisible");
            }
        }

        public bool IsListBoxThreadsVisible
        {
            get { return _IsListBoxThreadsVisible; }
            set
            {
                _IsListBoxThreadsVisible = value;
                OnPropertyChanged("IsListBoxThreadsVisible");
            }
        }

        public ProcessesViewModel()
        {
            CreateShowModulesCommand();
            CreateShowThreadsCommand();
            CreateAddProcessCommand();
            CreateSortProcessesCommand();
            CreateLoadProcessesOnRequestCommand();
            CreateOpenManagedWindowProcessesCommand();
            this.Processes = new ObservableCollection<Process>();
            Messenger.Default.Register<SortingMessage>(this, SortingProcessesByName);
            Messenger.Default.Register<LoadingOnRequestMessage>(this, LoadingProcessesOnRequest);
        }

        private void SortingProcessesByName(SortingMessage sortingMessage)
        {
            var helpfulLi = this.Processes.ToList();
            helpfulLi = new List<Process>(helpfulLi.OrderBy(proc => proc.ProcessName));
            Processes.Clear();
            helpfulLi.ForEach(this.Processes.Add);
        }

        private void LoadingProcessesOnRequest(LoadingOnRequestMessage loadingOnRequestMessage)
        {
            this.Processes.Clear();
            Process.GetProcesses().ToList().ForEach(this.Processes.Add);
        }


        public ICommand ShowModulesCommand { get; set; }

        private bool CanExecuteShowModulesCommand(object parameter)
        {
            return true;
        }

        private void CreateShowModulesCommand()
        {
            ShowModulesCommand = new RelayCommand<object>(ShowModulesExecute, CanExecuteShowModulesCommand);
        }

        public void ShowModulesExecute(object parameter)
        {

            if (parameter != null)
            {
                var process = (Process)parameter;
                IsListBoxModulesVisible = true;
                
            }
        }

        public ICommand ShowThreadsCommand { get; set; }

        private bool CanExecuteShowThreadsCommand(object parameter)
        {
            return true;
        }

        private void CreateShowThreadsCommand()
        {
            ShowThreadsCommand = new RelayCommand<object>(ShowThreadsExecute, CanExecuteShowThreadsCommand);
        }

        public void ShowThreadsExecute(object parameter)
        {

            if (parameter != null)
            {
                var process = (Process)parameter;
                IsListBoxThreadsVisible = true;

            }
        }

        public ICommand AddProcessCommand { get; set; }

        private bool CanExecuteAddProcessCommand(object parameter)
        {
            return true;
        }

        private void CreateAddProcessCommand()
        {
            AddProcessCommand = new RelayCommand<object>(AddProcessExecute, CanExecuteAddProcessCommand);
        }

        public void AddProcessExecute(object parameter)
        {   
            if (parameter != null)
            {
                Messenger.Default.Send<AddingMessage>(new AddingMessage((Process)parameter));
            }
        }

        public ICommand OpenManagedWindowProcessesCommand { get; set; }

        private bool CanExecuteOpenManagedWindowProcessesCommand(object parameter)
        {
            return true;
        }

        private void CreateOpenManagedWindowProcessesCommand()
        {
            OpenManagedWindowProcessesCommand = new RelayCommand<object>(OpenManagedWindowProcessesExecute, CanExecuteOpenManagedWindowProcessesCommand);
        }

        public void OpenManagedWindowProcessesExecute(object parameter)
        {
            var managedProcessesWindow = new ManagingProcessesWindow();
            managedProcessesWindow.Show();
            this.IsManagingProcessesWindowVisible = true; 
        }

        public ICommand SortProcessesCommand { get; set; }

        private bool CanExecuteSortProcessesCommand(object parameter)
        {
            return true;
        }

        private void CreateSortProcessesCommand()
        {
            SortProcessesCommand = new RelayCommand<object>(SortProcessesExecute, CanExecuteSortProcessesCommand);
        }

        public void SortProcessesExecute(object parameter)
        {
            Messenger.Default.Send<SortingMessage>(new SortingMessage());
        }

        public ICommand LoadProcessesOnRequestCommand { get; set; }

        private bool CanExecuteLoadProcessesOnRequestCommand(object parameter)
        {
            return true;
        }

        private void CreateLoadProcessesOnRequestCommand()
        {
            LoadProcessesOnRequestCommand = new RelayCommand<object>(LoadProcessesOnRequestExecute, CanExecuteLoadProcessesOnRequestCommand);
        }

        public void LoadProcessesOnRequestExecute(object parameter)
        {
            Messenger.Default.Send<LoadingOnRequestMessage>(new LoadingOnRequestMessage());
        }


    }
}
