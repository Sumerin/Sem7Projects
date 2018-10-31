using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessManager
{
    public static class ProcessPerformanceCounterFactory
    {
        private static PerformanceCounterCategory PROCESS_PERFORMANCE_CATEGORY;
        private const string PROCESS_CATEGORY = "Process";
        public const string PROCESSOR_USAGE = "% Processor Time";
        public const string THREAD_COUNT = "Thread Count";
        public const string MEMORY = "Working Set";
        public const string VIRTUAL_MEMORY = "virtual Bytes";
        public const string PAGE_FILE = "Page file bytes";
        public const string PRIVATE_MEMORY = "Private Bytes";
        public const string PRIORITY = "Priority Base";
        public const string READ_MEMORY = "IO Read Bytes/sec";
        public const string WRITE_MEMORY = "IO Write Bytes/sec";

        public static PerformanceCounter GetPerfCounterForProcess(Process process,string processCounterName, string instanceName ="")
        {
            if (string.IsNullOrEmpty(instanceName))
            {
                instanceName = GetInstanceNameForProcess(process);
            }
            if (string.IsNullOrEmpty(instanceName))
            {
                return null;
            }
            return new PerformanceCounter(PROCESS_CATEGORY, processCounterName, instanceName);
        }

        public static string GetInstanceNameForProcess(Process process)
        {
            string processName = Path.GetFileNameWithoutExtension(process.ProcessName);

            if (PROCESS_PERFORMANCE_CATEGORY == null)
            {
                PROCESS_PERFORMANCE_CATEGORY = new PerformanceCounterCategory(PROCESS_CATEGORY);
            }

            string[] instances = PROCESS_PERFORMANCE_CATEGORY.GetInstanceNames()
                .Where(inst => inst.StartsWith(processName))
                .ToArray();

            foreach (string instance in instances)
            {
                using (PerformanceCounter cnt = new PerformanceCounter(PROCESS_CATEGORY,
                    "ID Process", instance, true))
                {
                    int val = (int)cnt.RawValue;
                    if (val == process.Id)
                    {
                        return instance;
                    }
                }
            }
            return null;
        }

    }
}
