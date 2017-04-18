using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Diagnostics;  // for stopwatch
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices; //for dllimport

using System.Reflection;
namespace GammaConformance
{

    public class GammaConformanceReport
    {
        
        public static string GetConformanceReportDirectory()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string jobWorkFolder = Path.Combine(desktopPath, @"JobWork");
            string configFolder = Path.Combine(jobWorkFolder, "GammaConformanceReport");
            return configFolder + "\\";
        }
        private string ReportDirectory = GetConformanceReportDirectory(); //@"C:\JobArchive\GammaConformanceReport";

       
        protected List<double> _module_cps = new List<double>();
        protected double _module_netcps;
        protected double _module_apicps;
        protected List<double> _housing_cps = new List<double>();
        protected double _housing_netcps;
        protected double _housing_apicps;
        protected bool _use_module_only = true;

        protected bool is_verification_done = true;
        protected bool is_conformance_done = true;
        protected bool is_calibration_done = false;
        const string _lead_blanks = "    ";
        public void setup(bool ve, bool con, bool cal)
        {
            is_verification_done = ve;
            is_conformance_done = con;
            is_calibration_done = cal;
        }
        
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }
        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        
    }

}
