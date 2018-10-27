using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace ProcessManager.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        public const int REFRESH_CYCLE = 5;
        private ICommand windowLoaded;

        private DispatcherTimer dispatcherTimer;
        private TabViewModel tab1ViewModel;
        private TabViewModel tab2ViewModel;
        private ICommand watchProcessCommand;
        private ICommand windowClosed;
        private ICommand cmdCommand;

        public TabViewModel Tab1ViewModel
        {
            get { return tab1ViewModel; }
            set
            {
                tab1ViewModel = value;
                OnPropertyChanged();
            }
        }

        public TabViewModel Tab2ViewModel
        {
            get { return tab2ViewModel; }
            set
            {
                tab2ViewModel = value;
                OnPropertyChanged();
            }
        }


        public ICommand WindowLoaded
        {
            get { return windowLoaded ?? (windowLoaded = new RelayCommand(SetUpProcess)); }
        }

        public ICommand WindowClosed
        {
            get { return windowClosed ?? (windowClosed = new RelayCommand(ClearContext)); }
        }

        public ICommand WatchProcessCommand
        {
            get { return watchProcessCommand ?? (watchProcessCommand = new RelayCommand(AddToWatch)); }
        }

        public ICommand CmdCommand
        {
            get { return cmdCommand ?? (cmdCommand = new RelayCommand(RunCmd)); }
        }

        private void RunCmd(object obj)
        {
            var command = obj as string;
            try
            {
                var process = Process.Start(command);
                ShowProcess(process, true);
            }
            catch (Exception)
            {
                MessageBox.Show($"Command {command} is incorrect command", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddToWatch(object obj)
        {
            tab2ViewModel.AddProcess((ProcessViewModel)obj);
        }


        public MainWindowViewModel()
        {
            Tab1ViewModel = new TabViewModel();
            Tab2ViewModel = new TabViewModel();
        }

        private void SetUpProcess()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += RefreshProcess;
            dispatcherTimer.Interval = new TimeSpan(0, 0, REFRESH_CYCLE);
            dispatcherTimer.Start();
            RefreshProcess(null, null);
        }

        private void RefreshProcess(object sender, EventArgs e)
        {
            Process[] proceses = Process.GetProcesses();
            foreach (var process in proceses)
            {
                ProcessViewModel processVM = tab1ViewModel.Find(process.Id);
                if (processVM == null)
                {
                    ShowProcess(process);
                }
            }
        }

        private void ShowProcess(Process process, bool isCreatedFromGui = false)
        {
            var processItem = new ProcessViewModel(process, isCreatedFromGui);
            processItem.Exited += Process_Exited;
            this.tab1ViewModel.AddProcess(processItem);
        }

        private void Process_Exited(ProcessViewModel processVM)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => Process_Exited(processVM)));
            }
            else
            {
                this.tab1ViewModel.RemoveProcess(processVM);
                this.tab2ViewModel.RemoveProcess(processVM);
                processVM.Dispose();
            }
        }

        private void ClearContext()
        {
            tab1ViewModel.ClearContext();
            tab2ViewModel.ClearContext();
        }
    }
}
