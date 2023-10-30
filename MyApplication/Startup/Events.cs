using Logger.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Logger.Model.Enums;
using System.Windows;

namespace MyApplication.Startup
{
    internal class Events
    {
        public static IMyApp iMyApp;

        public static void AppAction(object sender, ExitEventArgs e)
        {
            Logger.Tasks.LoggerPublic.LoggerTitleInternal(iMyApp, "Aplikace ukončena uživatelem", MethodBase.GetCurrentMethod(), GetState.Get(ProcessState.ReadyToExit));

            e.ApplicationExitCode = 0;
        }
    }
}
