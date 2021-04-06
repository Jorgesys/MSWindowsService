using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;
using System.Runtime.InteropServices;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace WinServiceAndProcess
{
    public partial class WinServicAndProcess : ServiceBase
    {
        private int eventId = 1;
        private EventLog eventLog;
        private string outputPathLog = ConfigurationManager.AppSettings["OutputPathLog"];

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);
        /*public WinServicAndProcess()
        {
            InitializeComponent();

            eventLog1 = new EventLog();
            if (!EventLog.SourceExists("MySource"))
            {
                EventLog.CreateEventSource(
                    "MySource", "MyNewLog");
            }
            eventLog1.Source = "MySource";
            eventLog1.Log = "MyNewLog";
        }*/
        public WinServicAndProcess(string[] args)
        {
            InitializeComponent();

            string eventSourceName = "WinServicAndProcess";
            string logName = "AndroidLogFile";

            if (args.Length > 0)
            {
                eventSourceName = args[0];
            }

            if (args.Length > 1)
            {
                logName = args[1];
            }

            eventLog = new EventLog();

            if (!EventLog.SourceExists(eventSourceName))
            {
                EventLog.CreateEventSource(eventSourceName, logName);
            }

            eventLog.Source = eventSourceName;
            eventLog.Log = logName;
        }
        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog.WriteEntry("In OnStart(). " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            WriteToFile("In OnStart() started at " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));

            // Set up a timer that triggers every minute.
            Timer timer = new Timer();
            int intervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalSeconds"]);
            timer.Interval = intervalMinutes;//60000; // 60 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

        }

        protected override void OnPause()
        {            
            eventLog.WriteEntry("In OnPause(). " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            WriteToFile("In OnPause() at " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
        }

        protected override void OnShutdown()
        {
            eventLog.WriteEntry("In OnShutdown().");
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("In OnStop()." + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));            
            WriteToFile("In OnStop() at " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));

            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnContinue()
        {
            eventLog.WriteEntry("In OnContinue()." + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            eventLog.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
            //Set process
            //CallAPI();
        }


        private void WriteToFile(string text)
        {         
            using (StreamWriter writer = new StreamWriter(outputPathLog, true))
            {
                writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }

    }
}
