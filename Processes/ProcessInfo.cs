using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processes
{
    public class ProcessInfo
    { 
        public int Num { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public long PhysicalMemoryUsage { get; set; }
        public string Status { get; set; }
        public int Threads { get; set; }
        public int Handles { get; set; }

        //public override string ToString()
        //{
        //    return base.ToString();
        //}
    }
}
