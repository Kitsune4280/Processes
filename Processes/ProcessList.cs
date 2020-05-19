using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Processes
{
    public class ProcessList
    {
        public List<ProcessInfo> Processes { get; set; }

        private object lockObject = new object();

        public ProcessList()
        {
            Processes = new List<ProcessInfo>();
        }

        public void RefreshData()
        {
            BindingOperations.EnableCollectionSynchronization(Processes, lockObject);
            try
            {
                lock (lockObject)
                {
                    Process[] processes = Process.GetProcesses();
                    Processes = new List<ProcessInfo>();
                    int counter = 0;
                    foreach (Process p in processes)
                    {
                        Processes.Add(new ProcessInfo
                        {
                            Num = ++counter,
                            Id = p.Id,
                            Name = p.ProcessName,
                            PhysicalMemoryUsage = p.WorkingSet64,
                            Status = p.Responding ? "Running" : "Not responding",
                            Threads = p.Threads.Count,
                            Handles = p.HandleCount
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
