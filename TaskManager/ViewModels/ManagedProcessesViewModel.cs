using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Controls;

namespace TaskManager.ViewModels
{
    public class ChangePriorityConverter : IMultiValueConverter
    {


        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ManagedProcessesViewModel : INotifyPropertyChanged
    {
        static string FileNameProc;

        private Dictionary<int, string> DictForRecoveringProcesses { get; set; }
        public ObservableCollection<Process> ManagedProcesses { get; set; }

        public ObservableCollection<Process> DeletedProcesses { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string _InfoLabel;
        public string InfoLabel
        {
            get { return _InfoLabel; }
            set
            {
                _InfoLabel = value;
                OnPropertyChanged(nameof(InfoLabel));
            }
        }

        public string _PriorityClassSelectedItemCB;
        public string PriorityClassSelectedItemCB
        {
            get { return _PriorityClassSelectedItemCB; }
            set
            {
                _PriorityClassSelectedItemCB = value;
                OnPropertyChanged(nameof(PriorityClassSelectedItemCB));
            }
        }

        public bool IsProcessIdCBChecked { get; set; }

        public bool IsProcessPriorityClassCBChecked { get; set; }

        public bool IsPriorityClassCBChecked { get; set; }

        public bool IsProcessBasePriorityCBChecked { get; set; }

        public bool IsProcessWorkingSet64CBChecked { get; set; }
        
        private bool _IsDeleteProcessesButtonVisible;

        public bool IsDeleteProcessesButtonVisible
        {
            get { return _IsDeleteProcessesButtonVisible; }
            set
            {
                _IsDeleteProcessesButtonVisible = value;
                OnPropertyChanged("IsDeleteProcessesButtonVisible");
            }
        }

        private bool _IsRecoverProcessesButtonVisible;

        public bool IsRecoverProcessesButtonVisible
        {
            get { return _IsRecoverProcessesButtonVisible; }
            set
            {
                _IsRecoverProcessesButtonVisible = value;
                OnPropertyChanged("IsRecoverProcessesButtonVisible");
            }
        } 

        private bool _IsLabelInfoVisible;

        public bool IsLabelInfoVisible
        {
            get { return _IsLabelInfoVisible; }
            set
            {
                _IsLabelInfoVisible = value;
                OnPropertyChanged("IsLabelInfoVisible");
            }
        }

        public ManagedProcessesViewModel()
        {
            this.ManagedProcesses = new ObservableCollection<Process>();
            this.DeletedProcesses = new ObservableCollection<Process>();
            this.DictForRecoveringProcesses = new Dictionary<int, string>();
            CreateDeleteProcessCommand();
            CreateRecoverProcessCommand();
            CreateChangePriorityCommand();
            Messenger.Default.Register<AddingMessage>(this, AddingProcessToManagedCol);
            Messenger.Default.Register<DeletingMessage>(this, DeletingProcessFromManagedCol);
            Messenger.Default.Register<RecoveringMessage>(this, RecoveringProcessFromDeletedCol);
            Messenger.Default.Register<ChangingPriorityMessage>(this, ChangingProcessPriorityFromManagedCol);
        }

        private void AddingProcessToManagedCol(AddingMessage addingMessage)
        {
            foreach(var proc in this.ManagedProcesses)
            {
                if (proc == addingMessage.process)
                {
                    InfoLabel = "Cannot add item which was added before.";
                    IsLabelInfoVisible = true;
                    OnPropertyChanged(nameof(InfoLabel));
                    return;
                }
            }
            ManagedProcesses.Add(addingMessage.process);
            InfoLabel = "Added process.";
            IsLabelInfoVisible = true;
            IsDeleteProcessesButtonVisible = true;
        }

        private void DeletingProcessFromManagedCol(DeletingMessage deletingMessage)
        {
            try
            {
                var deletedProcessName = deletingMessage.process.ProcessName;
                var IDofDeletedProcess = deletingMessage.process.Id;
                var priorClassOfDeletedProc = deletingMessage.process.PriorityClass;
                var basePriorityOfDeletedProc = deletingMessage.process.BasePriority;
                var workingSet64OfDeletedProc = deletingMessage.process.WorkingSet64;
                this.ManagedProcesses.Remove(deletingMessage.process);
                this.DeletedProcesses.Add(deletingMessage.process);
                IsRecoverProcessesButtonVisible = true;
                var fileNameOfDeletedProcess = deletingMessage.process.MainModule.FileName;
                deletingMessage.process.Kill();
                deletingMessage.process.WaitForExit();
                this.DictForRecoveringProcesses.Add(deletingMessage.process.Id, fileNameOfDeletedProcess);

                var logInfo = string.Format("Deleted process: {0}",
                                    deletedProcessName);
                if (IsProcessIdCBChecked)
                    logInfo += string.Format(" with ID: {0}", IDofDeletedProcess.ToString());
                if (IsProcessPriorityClassCBChecked)
                    logInfo += string.Format(" with priority class: {0}", priorClassOfDeletedProc);
                if (IsProcessBasePriorityCBChecked)
                    logInfo += string.Format(" with base priority: {0}", priorClassOfDeletedProc.ToString());
                if (IsProcessWorkingSet64CBChecked)
                    logInfo += string.Format(" with working set 64: {0}", workingSet64OfDeletedProc.ToString());
                File.AppendAllText(@"..\..\DeleteLogs.txt", logInfo + Environment.NewLine);
                Messenger.Default.Send<LoadingOnRequestMessage>(new LoadingOnRequestMessage());
                InfoLabel = "Deleted process.";
                IsLabelInfoVisible = true;
            }
            catch (Win32Exception)
            {

            }
            catch (InvalidOperationException)
            {

            }
        }

        private void RecoveringProcessFromDeletedCol(RecoveringMessage recoveringMessage)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo();
                foreach (var item in this.DictForRecoveringProcesses)
                {
                    if (item.Key == recoveringMessage.process.Id)
                    {
                        processStartInfo.FileName = item.Value;
                        break;
                    }
                }
                this.DeletedProcesses.Remove(recoveringMessage.process);
                Process.Start(processStartInfo);      
                OnPropertyChanged("DeletedProcesses");
                InfoLabel = "Restarted process.";
                IsLabelInfoVisible = true;
            }
            catch(InvalidOperationException)
            {
                InfoLabel = "It is needed to add name of file to start process.";
                IsLabelInfoVisible = true;
            }
            catch (Win32Exception)
            {
                InfoLabel = "Cannot restart this process.";
                IsLabelInfoVisible = true;
            }

           
        }

        private void ChangingProcessPriorityFromManagedCol(ChangingPriorityMessage changingPriorMessage)
        {
            if (changingPriorMessage.priorityClass.Equals("High"))
            {
                changingPriorMessage.process.PriorityClass = ProcessPriorityClass.High;
            }
            else if (changingPriorMessage.priorityClass.Equals("Above normal"))
            {
                changingPriorMessage.process.PriorityClass = ProcessPriorityClass.AboveNormal;
            }
            else if (changingPriorMessage.priorityClass.Equals("Normal"))
            {
                changingPriorMessage.process.PriorityClass = ProcessPriorityClass.Normal;
            }
            else if (changingPriorMessage.priorityClass.Equals("Below normal"))
            {
                changingPriorMessage.process.PriorityClass = ProcessPriorityClass.BelowNormal;
            }
            else if (changingPriorMessage.priorityClass.Equals("Low"))
            {
                changingPriorMessage.process.PriorityClass = ProcessPriorityClass.Idle;
            }
            InfoLabel = "Changed priority";
            IsLabelInfoVisible = true;
        }

        public ICommand DeleteProcessCommand { get; set; }

        private bool CanExecuteDeleteProcessCommand(object parameter)
        {
            return true;
        }

        private void CreateDeleteProcessCommand()
        {
            DeleteProcessCommand = new RelayCommand<object>(DeleteProcessExecute, CanExecuteDeleteProcessCommand);
        }

        public void DeleteProcessExecute(object parameter)
        {

            if (parameter != null)
            {
                    Messenger.Default.Send(new DeletingMessage((Process)parameter));
            }else
            {
                InfoLabel = "It is needed to mark process to delete.";
                IsLabelInfoVisible = true;
            }
        }

        public ICommand RecoverProcessCommand { get; set; }

        private bool CanExecuteRecoverProcessCommand(object parameter)
        {
            return true;
        }

        private void CreateRecoverProcessCommand()
        {
            RecoverProcessCommand = new RelayCommand<object>(RecoverProcessExecute, CanExecuteRecoverProcessCommand);
        }

        public void RecoverProcessExecute(object parameter)
        {

            if (parameter != null)
            {
                Messenger.Default.Send(new RecoveringMessage((Process)parameter));
            }
            else
            {
                InfoLabel = "It is needed to mark process to recover.";
                IsLabelInfoVisible = true;
            }
        }

        public ICommand ChangePriorityCommand { get; set; }

        private bool CanExecuteChangePriorityCommand(object parameter)
        {
            return true;
        }

        private void CreateChangePriorityCommand()
        {
            ChangePriorityCommand = new RelayCommand<object>(ChangePriorityExecute, CanExecuteChangePriorityCommand);
        }

        public void ChangePriorityExecute(object parameter)
        {

            if (parameter != null)
            {
                var values = (object[])parameter;
                if (values[0] == null)
                {
                    InfoLabel = "It is needed to mark process to change priority.";
                    IsLabelInfoVisible = true;
                    return;
                }
                else if (values[1] == null)
                {
                    InfoLabel = "It is needed to select priority class to change priority.";
                    IsLabelInfoVisible = true;
                    return;
                }
                var proc = (Process)values[0];
                var selectedPriorItem = (ComboBoxItem)values[1];
                var selectedPrior = selectedPriorItem.Content.ToString();
                Messenger.Default.Send(new ChangingPriorityMessage(proc, selectedPrior));
            }
            else
            {
                InfoLabel = "It is needed to mark process to change priority.";
                IsLabelInfoVisible = true;
            }
        }
    }
}
