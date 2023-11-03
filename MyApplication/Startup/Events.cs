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
        /// <summary>
        /// Reference na instanci třídy IMyApp pro aplikaci.
        /// </summary>
        internal static IMyApp iMyApp;

        /// <summary>
        /// Obsluhuje akci ukončení aplikace.
        /// </summary>
        /// <param name="sender">Objekt, který vyvolal událost.</param>
        /// <param name="e">Argumenty události ukončení aplikace.</param>
        internal static void AppAction(object sender, ExitEventArgs e)
        {
            Logger.Tasks.LoggerPublic.LoggerTitleInternal(iMyApp, "Aplikace ukončena uživatelem", MethodBase.GetCurrentMethod(), GetState.Get(ProcessState.ReadyToExit));

            e.ApplicationExitCode = 0;
        }
    }
}
