using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;

namespace FooConformance
{
    class LogTo
    {
        string processName;

        public LogTo()
        {
            processName = Process.GetCurrentProcess().ProcessName;
            EnableWinLog(processName, "Application");
        }

            
        public void GeneralLog( string appName, string message, string other )
        {
            DateTime now = DateTime.Now;

            try
            {
                var method = new StackTrace().GetFrame( 1 ).GetMethod();
                var cls = method.ReflectedType.Name;
                    LogToWinEvent( "Class " + cls + " -> " + message + "[Other - " + other + "]" );
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        /// <summary>
        /// Logs to win event.
        /// </summary>
        /// <param name="logMsg">The log MSG.</param>
        public void LogToWinEvent(string logMsg)
        {
            //if ( !EventLog.SourceExists( sSource ) )
            //{
            //    EventLog.CreateEventSource( sSource, sLogCategory );
            //}
            EventLog.WriteEntry(processName, logMsg, EventLogEntryType.Error);
        }

        /// <summary>
        /// Enables the win log.
        /// </summary>
        /// <param name="sSourceName">Name of the s source.</param>
        /// <param name="logCategory">The log category.</param>
        private void EnableWinLog(string sSourceName, string logCategory)
        {
            // Check whether registry key for source exists
            string keyName = @"SYSTEM\CurrentControlSet\Services\EventLog\" + logCategory + @"\" + sSourceName;
            RegistryKey rkEventSource = Registry.LocalMachine.OpenSubKey(keyName);

            // Check whether keys exists
            if (rkEventSource == null)
            {
                // Key doesnt exist. Create key which represents source
                Process Proc = new Process();
                ProcessStartInfo ProcStartInfo = new ProcessStartInfo("Reg.exe");
                ProcStartInfo.Arguments = @"add HKLM\" + keyName;
                ProcStartInfo.UseShellExecute = true;
                ProcStartInfo.Verb = "runas";
                Proc.StartInfo = ProcStartInfo;
                Proc.Start();
            }

        }
    }
}
