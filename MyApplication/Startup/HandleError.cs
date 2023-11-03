using AppConfigure;
using AppConfigure.Utils;
using Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MyApplication.Startup
{
    internal class HandleError
    {
        /// <summary>
        /// Asynchronně zpracuje chybu v argumentech aplikace a zobrazí chybový dialog s podrobným vysvětlením.
        /// </summary>
        /// <param name="iMyApp"></param>
        /// <param name="systemInfoUtility"></param>
        /// <param name="AppInstance"></param>
        /// <returns></returns>
        internal static async Task HandleArgumentError(IMyApp iMyApp, SystemInfoUtility systemInfoUtility, Application AppInstance)
        {
            // Získá informace o operačním systému.
            string osInfo = systemInfoUtility?.GetOSInfo();

            // Získá název aplikace a její verzi.
            string appName = iMyApp?.Resources?.AppType?.Assembly?.GetName()?.Name;
            string appVersion = iMyApp?.Resources?.AppType?.Assembly?.GetName()?.Version.ToString();

            // Definice očekávaných a aktuálních argumentů aplikace.
            string expectedArgs = "FULLSCREEN";
            string currentArgs = iMyApp?.AppConfig?.Argument?.ArgumentList;

            // Získá konkrétní hodnoty argumentů.
            bool? fullscreen = iMyApp?.AppConfig?.AppConfiguration?.WindowConf?.Fullscreen;
            bool? clearBuffer = iMyApp?.AppConfig?.Argument?.VyprázdnitBufferSériovéhoPortu;
            int? bcsDelay = iMyApp?.AppConfig?.Argument?.ProdlevaSériovéKomunikace;

            // Sestaví chybové hlášení s uvedenými informacemi.
            string errorMessage = $"{osInfo}\n{appName} {appVersion}\nThe arguments are not set up correctly.\nFor example, set the arguments: {expectedArgs}\nActual args: {currentArgs}\nFullscreen: {fullscreen}\nCLEARBUFFER: {clearBuffer}\nBCSDELAY: {bcsDelay}";

            // Zobrazí chybový dialog s chybovým hlášením, tímto hlášením a dalšími relevantními informacemi.
            await await Dispatcher.CurrentDispatcher.ShowErrorDialogAsync(ErrorDialog.Description.ConfigurationError, Logger.Msg.ArgsNotValid, errorMessage, AppInstance, "App arguments", ErrorDialog.TypeMessage.Critical);
        }

        /// <summary>
        /// Asynchronně zpracuje chybu v konfiguraci aplikace a zobrazí chybový dialog s relevantními informacemi.
        /// </summary>
        /// <param name="iMyApp"></param>
        /// <param name="Description"></param>
        /// <param name="systemInfoUtility"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        internal static Task HandleConfigurationError(IMyApp iMyApp, string Description, SystemInfoUtility systemInfoUtility, Application application)
        {
            // Získá informace o operačním systému.
            string osInfo = systemInfoUtility?.GetOSInfo();

            // Získá název aplikace a její verzi.
            string appName = iMyApp?.Resources?.AppType?.Assembly?.GetName()?.Name;
            string appVersion = iMyApp?.Resources?.AppType?.Assembly?.GetName()?.Version.ToString();

            // Sestaví chybové hlášení s uvedenými informacemi.
            string errorMessage = $"{osInfo}\n{appName} {appVersion}\n{Description}";
         
            // Zobrazí chybový dialog s chybovým hlášením, tímto hlášením a dalšími relevantními informacemi.
            CustomMessageBox customMessageBox = new CustomMessageBox("Aplikace je již spuštěna",errorMessage, MessageBoxButton.OK);
            customMessageBox.ShowDialog();
            application.Shutdown();

            return Task.FromResult(0);
        }

        /// <summary>
        /// Asynchronně zpracuje chybu při čtení XML souboru a zobrazí chybové hlášení s informacemi o chybě.
        /// Ukončí běh aplikace v případě chyby.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        internal static async Task HandleXmlReadError(string error, Application application)
        {
            // Zobrazí chybové hlášení pro chybu z čtení XML souboru s dodatečnými informacemi.
            await await Dispatcher.CurrentDispatcher?.ShowXmlReadErrorAsync(error, application);

            // Ukončí běh aplikace.
            application?.Shutdown();
        }
    }
}
