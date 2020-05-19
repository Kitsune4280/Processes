using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processes
{
    public class Schedule
    {
        public List<ScheduledApplication> Apps { get; set; }
        private object LockObject = new object();

        public Schedule()
        {
            Apps = new List<ScheduledApplication>();
        }

        public void AddApplication(ScheduledApplication app)
        {
            try
            {
                lock (LockObject)
                {
                    Apps.Add(app);
                    Apps = Apps.OrderBy(a => a.ScheduledTime).ToList();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                //TODO: save to file
            }            
        }

        public void DeleteApp(ScheduledApplication app)
        {
            try
            {
                lock (LockObject)
                {
                    Apps.Remove(app);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ExecuteApp(ScheduledApplication app)
        {
            if(Apps.IndexOf(app) != -1)
            {
                try
                {
                    lock (LockObject)
                    {
                        Apps.Remove(app);
                        Process.Start(app.Name);
                    }
                }
                catch (Exception ex)
                {
                    Apps.Remove(app);
                    Console.WriteLine(ex.Message);
                }
            }            
        }
    }
}
