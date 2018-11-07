﻿using GalaSoft.MvvmLight.CommandWpf;
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

        private string _LastName = null;

        //public ICommand ContextMenuOpeningCommand
        //{
        //    get
        //    {
        //        if (_contextMenuOpeningCommand == null)
        //        {
        //            _contextMenuOpeningCommand = new RelayCommand<object>(param => this.ContextMenuOpening(),
        //                null);
        //        }

        //        return _contextMenuOpeningCommand;
        //    }
        //}

        //public void ContextMenuOpening()
        //{
        //    MessageBox.Show("test", "test");
        //}

        //private ICommand _contextMenuOpeningCommand;



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

            MessageBox.Show("xd");
            if (parameter != null)
            {
                var process = (Process)parameter;
                MessageBox.Show(process.ProcessName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = null;

        public ProcessesViewModel()
        {
            CreateShowModulesCommand();
            this.Processes = new ObservableCollection<Process>();
        }

        public void SetAllProcesses()
        {
            this.Processes = new ObservableCollection<Process>(Process.GetProcesses().ToList());
            //foreach (var process in Processes)
            //{
            //    ProcessModuleCollection myProcessModuleCollection = process.Modules;
            //    foreach (var module in process.Modules)
            //    {
            //        ProcessModule x = (ProcessModule) module;
            //    }
            //}

        }

        //public string LastName
        //{
        //    get
        //    {
        //        return _LastName;
        //    }
        //    set
        //    {
        //        _LastName = null;
        //        OnPropertyChanged("LastName");
        //    }
        //}

        virtual protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }
}