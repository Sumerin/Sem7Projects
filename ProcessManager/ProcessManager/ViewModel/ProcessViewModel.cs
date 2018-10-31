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
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;

namespace ProcessManager.ViewModel
{
    public class ProcessViewModel : NotifyPropertyChanged, IDisposable
    {
        #region Settings
        private const int REFRESH_CYLCE = 1000;
        private const int KILO = 1024;
        private bool refresh;
        #endregion

        #region Fields
        private Process process;
        private int pid;
        private string name;
        private float cpu;
        private string processName;
        private float threadCount;
        private float memory;
        private string instanceName;
        private Thread ListenExit;
        private bool isCreatedFromGui;
        private ICommand endProcessCommand;
        private ICommand addCommand;
        private ICommand removeCommand;
        private ProcessPriorityClass priority;
        private float virtualMemory;
        private float pageFile;
        private float privateKBytes;
        private float readKBytes;
        private float writeKBytes;

        #endregion

        public event Action<ProcessViewModel> Exited;

        #region Properties
        public int PID
        {
            get
            {
                return pid;
            }
            private set
            {
                pid = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
            private set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        public string ProcessName
        {
            get
            {
                return processName;
            }
            private set
            {
                processName = value;
                OnPropertyChanged();
            }
        }
        public float CPU
        {
            get
            {
                return cpu;
            }
            private set
            {
                cpu = value;
                OnPropertyChanged();
            }
        }
        public float ThreadCount
        {
            get
            {
                return threadCount;
            }
            private set
            {
                threadCount = value;
                OnPropertyChanged();
            }
        }
        public float Memory
        {
            get
            {
                return memory;
            }
            private set
            {
                memory = value;
                OnPropertyChanged();
            }
        }
        public float VirtualMemory
        {
            get
            {
                return virtualMemory;
            }
            private set
            {
                virtualMemory = value;
                OnPropertyChanged();
            }
        }
        public float PageFile
        {
            get
            {
                return pageFile;
            }
            private set
            {
                pageFile = value;
                OnPropertyChanged();
            }
        }
        public float PrivateKBytes
        {
            get
            {
                return privateKBytes;
            }
            private set
            {
                privateKBytes = value;
                OnPropertyChanged();
            }
        }
        public float ReadKBytes
        {
            get
            {
                return readKBytes;
            }
            private set
            {
                readKBytes = value;
                OnPropertyChanged();
            }
        }
        public float WriteKBytes
        {
            get
            {
                return writeKBytes;
            }
            private set
            {
                writeKBytes = value;
                OnPropertyChanged();
            }
        }

        public bool IsCreatedFromGui
        {
            get { return isCreatedFromGui; }
            set
            {
                isCreatedFromGui = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand
        {
            get { return addCommand ?? (addCommand = new RelayCommand(AddExitingCommand)); }
        }

        public ICommand RemoveCommand
        {
            get { return removeCommand ?? (removeCommand = new RelayCommand(RemoveExitingCommand)); }
        }

        public ICommand EndProcessCommand
        {
            get { return endProcessCommand ?? (endProcessCommand = new RelayCommand(EndProcess)); }
        }

        public ObservableCollection<string> CommandOnExiting { get; set; }

        public ProcessPriorityClass Priority
        {
            get { return this.priority; }
            set
            {
                if (UnBlockedPriority)
                {
                    this.priority = value;
                    this.process.PriorityClass = value;
                    OnPropertyChanged();
                }
            }
        }


        public bool UnBlockedPriority { get; private set; } = true;

        #endregion

        public ProcessViewModel()
        {
        }

        public ProcessViewModel(Process process, bool isCreatedFromGui)
        {
            this.IsCreatedFromGui = isCreatedFromGui;
            this.process = process;
            this.PID = process.Id;
            this.Name = process.ProcessName;
            this.ProcessName = process.MainWindowTitle;
            this.CommandOnExiting = new ObservableCollection<string>();
            try
            {

                this.priority = process.PriorityClass;
            }
            catch (Exception)
            {
                this.UnBlockedPriority = false;
            }

            ListenExit = new Thread(WaitForExit)
            {
                IsBackground = true
            };
            ListenExit.Start();
        }

        public void Dispose()
        {
            process?.Dispose();
        }

        private void WaitForExit()
        {
            try
            {
                this.process.WaitForExit();
                StopWatching();
                OnExiting();
                OnExited();
            }
            catch (Exception)
            {

            }
        }

        private void OnExiting()
        {
            foreach (var settedCommand in CommandOnExiting)
            {
                var executeCommand = settedCommand.Contains('%') ? ParseCommand(settedCommand) : settedCommand;

                try
                {
                    Process.Start("CMD.exe", $"/C {executeCommand}");
                }
                catch (Exception)
                {
                    MessageBox.Show($"Command {executeCommand} is incorrect command", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string ParseCommand(string settedCommand)
        {
            StringBuilder executableCmdBuilder = new StringBuilder();
            foreach (var commandPart in settedCommand.Split('%'))
            {
                if (!string.IsNullOrEmpty(commandPart))
                {
                    executableCmdBuilder.Append(commandPart);
                    executableCmdBuilder.Append(this.Name);
                }
            }
            return executableCmdBuilder.ToString();
        }

        private void AddExitingCommand(object command)
        {
            this.CommandOnExiting.Add(command as string);
        }

        private void RemoveExitingCommand(object command)
        {
            this.CommandOnExiting.Remove(command as string);
        }

        private void OnExited()
        {
            Exited?.Invoke(this);
        }

        private void EndProcess()
        {
            this.process.CloseMainWindow();
            this.process.Close();
        }

        public void StartWatching()
        {
            try
            {
                if (string.IsNullOrEmpty(this.instanceName))
                {
                    this.instanceName = ProcessPerformanceCounterFactory.GetInstanceNameForProcess(process);
                }
                this.refresh = true;
            }
            catch (Exception)
            {
                // closing application
            }

            Task.Run(
                () => { WatchingTask(ProcessPerformanceCounterFactory.PROCESSOR_USAGE, x => this.CPU = x); }
            );

            Task.Run(
                () => { WatchingTask(ProcessPerformanceCounterFactory.THREAD_COUNT, x => this.ThreadCount = x); }
            );

            Task.Run(
                () => { WatchingTask(ProcessPerformanceCounterFactory.MEMORY, x => this.Memory = x / KILO); }
            );
            Task.Run(
                () => { WatchingTask(ProcessPerformanceCounterFactory.VIRTUAL_MEMORY, x => this.VirtualMemory = x / KILO); }
            );
            Task.Run(
                () => { WatchingTask(ProcessPerformanceCounterFactory.PAGE_FILE, x => this.PageFile = x / KILO); }
            );
            Task.Run(
                () => { WatchingTask(ProcessPerformanceCounterFactory.PRIVATE_MEMORY, x => this.PrivateKBytes = x / KILO); }
            );
            Task.Run(
                () => { WatchingTask(ProcessPerformanceCounterFactory.READ_MEMORY, x => this.ReadKBytes = x / KILO); }
            );
            Task.Run(
                () => { WatchingTask(ProcessPerformanceCounterFactory.WRITE_MEMORY, x => this.WriteKBytes = x / KILO); }
            );
        }

        public void StopWatching()
        {
            this.refresh = false;
        }

        private async void WatchingTask(string usage, Action<float> Update)
        {
            try
            {
                using (var performance = ProcessPerformanceCounterFactory.GetPerfCounterForProcess(process, usage, this.instanceName))
                {
                    while (this.refresh)
                    {
                        Update?.Invoke(performance.NextValue());
                        await Task.Delay(REFRESH_CYLCE);
                    }
                }
            }
            catch (Exception)
            {
                // closing application
            }
        }
    }
}
