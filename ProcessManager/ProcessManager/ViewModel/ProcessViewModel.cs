using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private ICommand endProcessCommand;
        private bool isCreatedFromGui;

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

        public bool IsCreatedFromGui {
            get { return isCreatedFromGui; }
            set
            {
                isCreatedFromGui = value;
                OnPropertyChanged();
            }
        }

        public ICommand EndProcessCommand
        {
            get { return endProcessCommand ?? (endProcessCommand = new RelayCommand(EndProcess)); }
        }

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

            ListenExit = new Thread(WaitForExit);
            ListenExit.Start();
        }

        public void Dispose()
        {
            process?.Dispose();
            ListenExit?.Abort();
        }

        private void WaitForExit()
        {
            try
            {
                this.process.WaitForExit();
                StopWatching();
                OnExited();
            }
            catch (Exception)
            {

            }
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

            Task.Run(() => CpuWatchingTask());
            Task.Run(() => ThreadWatchingTask());
            Task.Run(() => MemoryWatchingTask());
        }
        public void StopWatching()
        {
            this.refresh = false;
        }

        private async void CpuWatchingTask()
        {
            try
            {
                using (var cpuPerformance = ProcessPerformanceCounterFactory.GetPerfCounterForProcess(process, ProcessPerformanceCounterFactory.PROCESSOR_USAGE, this.instanceName))
                {
                    while (this.refresh)
                    {
                        this.CPU = cpuPerformance.NextValue();
                        await Task.Delay(REFRESH_CYLCE);
                    }
                }
            }
            catch (Exception)
            {
                // closing application
            }
        }

        private async void ThreadWatchingTask()
        {
            try
            {
                using (var threadCountPerformance = ProcessPerformanceCounterFactory.GetPerfCounterForProcess(process, ProcessPerformanceCounterFactory.THREAD_COUNT, this.instanceName))
                {
                    while (this.refresh)
                    {
                        this.ThreadCount = threadCountPerformance.NextValue();
                        await Task.Delay(REFRESH_CYLCE);
                    }
                }
            }
            catch (Exception)
            {
                // closing application
            }
        }

        private async void MemoryWatchingTask()
        {
            try
            {
                using (var memoryPerformance = ProcessPerformanceCounterFactory.GetPerfCounterForProcess(process, ProcessPerformanceCounterFactory.MEMORY, this.instanceName))
                {
                    while (this.refresh)
                    {
                        this.Memory = memoryPerformance.NextValue() / KILO / KILO;
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
