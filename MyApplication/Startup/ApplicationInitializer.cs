using AppConfigure;
using Dialogs;
using Logger;
using Logger.Tasks;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static Logger.Model.Enums;

namespace MyApplication.Startup
{
    /// <summary>
    /// Třída obsahující metody pro inicializaci a registraci aplikace.
    /// </summary>
    public class ApplicationInitializer
    {
        /// <summary>
        /// Registruje okno aplikace a notifikuje o inicializaci.
        /// </summary>
        /// <typeparam name="W">Typ okna</typeparam>
        /// <typeparam name="C">Typ konfigurace</typeparam>
        /// <typeparam name="R">Typ prostředků</typeparam>
        /// <param name="iMyApp">Instance rozhraní aplikace</param>
        /// <param name="appViewModel">Instance modelu aplikace</param>
        /// <param name="startup">Argumenty spuštění aplikace</param>
        /// <param name="application">Instance aplikace</param>
        /// <returns>True, pokud inicializace proběhla úspěšně, jinak False</returns>
        public static async Task<bool> InitializeAppConfigurationAndResources<W,M,R,C>(IMyApp iMyApp, MyApplication.IViewModel appViewModel, StartupEventArgs startup, Application application) where W : Window, new() where M : ILogManager, new() where R : IAppResources, new() where C : IAppConfig, new()
        {
            Events.iMyApp = iMyApp;
            iMyApp.Resources = new R();
            iMyApp.AppConfig = new C();

            application.Exit += Events.AppAction;

            if (iMyApp == null)
            {
                MessageBox.Show("IMyApp is null");
                return false;
            }
            if (iMyApp?.Resources == null)
            {
                MessageBox.Show("Resources is null");
                return false;
            }
            if (appViewModel == null)
            {
                MessageBox.Show("IViewModel is null");
                return false;
            }
            iMyApp.Resources.AppViewModel = appViewModel;
            if (appViewModel == null)
            {
                MessageBox.Show("AppViewModel source is null");
                return false;
            }
            if (iMyApp?.Resources?.AppViewModel == null)
            {
                MessageBox.Show("AppViewModel is null");
                return false;
            }
            if (iMyApp?.Resources?.AppType == null)
            {
                MessageBox.Show("AppType is null");
                return false;
            }
            if (iMyApp?.Resources?.CancellationTokenSource == null)
            {
                MessageBox.Show("CancellationTokenSource is null");
                return false;
            }
            if (iMyApp?.Resources?.AppInstance == null)
            {
                MessageBox.Show("AppInstance is null");
                return false;
            }
            if (iMyApp?.Resources?.AppViewModel?.LogManager == null)
            {
                MessageBox.Show("LogManager is null");
                return false;
            }
            if (iMyApp?.Resources?.AppViewModel?.LogManager?.LogEntries == null)
            {
                MessageBox.Show("LogEntries is null");
                return false;
            }
            if (iMyApp?.AppConfig == null)
            {
                MessageBox.Show("AppConfig is null");
                return false;
            }
            if (iMyApp?.EnableVadidatingDicz == null)
            {
                MessageBox.Show("EnableVadidatingDicz is null");
                return false;
            }
            if (iMyApp?.AppConfig?.ProductionPriority == null)
            {
                MessageBox.Show("ProductionPriority is null");
                return false;
            }

            // Inicializace utility pro informace o systému
            AppConfigure.SystemInfoUtility systemInfoUtility = new SystemInfoUtility(iMyApp?.Resources?.AppType?.Assembly?.GetName()?.Name);
          
            // Kontrola, zda je aplikace již spuštěna
            if (systemInfoUtility.ZkontrolujBěžícíProgram() == AppRunning.Běžící)
            {
                await HandleError.HandleConfigurationError(iMyApp, "Aplikace je již spuštěna", systemInfoUtility, application);
                application.Shutdown();
                return false;
            }

            // Získání verze .NET Framework, na které aplikace běží
            GetDotNetVersion(iMyApp);

            // Načtení hlavního nastavení aplikace a zkontroluj, zda načtení proběhlo úspěšně
            if (!await Xml.LoadMainSettings(iMyApp, application))
            {
                return false;
            }

            // Inicializace konfigurace BCS čtečky
            Xml.InitializeBCSReaderConfiguration(iMyApp);

            // Získání a zpracování argumentů spuštění aplikace
            iMyApp.AppConfig.DaikinAppConfig.GetCooperationArguments(startup.Args);
          
            // Inicializace hlavní databáze a pokračuj pouze v případě úspěšné inicializace
            if (await Xml.InitializeMainDatabase(iMyApp, systemInfoUtility, application))
            {
              
                // Nastavení složky pro obrázky liquidů
                await Xml.SetImageFolder(iMyApp);

                // Kontrola argumentů aplikace a ukončení aplikace v případě neplatných argumentů
                if (!await Xml.CheckArgumentsAsync(iMyApp, systemInfoUtility, application))
                {
                    return false;
                }
               
                // Nastavení kontextu hlavního okna a spuštění časovače
                await SetMainWindowContextAndStartTimer<W>(iMyApp);
              
                await await Logger.Tasks.LoggerPublic.LoggerTitleInternal(iMyApp, "", MethodBase.GetCurrentMethod(), GetState.Get(ProcessState.Done));

                // Spustí další asynchronní úkol (PostTask).
                await iMyApp?.PostTask();

                return true;
            }

            return false;
        }


        /// <summary>
        /// Získá a uloží verzi .NET Framework, na které aplikace běží.
        /// </summary>
        /// <param name="iMyApp">Instance rozhraní aplikace</param>
        static void GetDotNetVersion(IMyApp iMyApp)
        {          
            iMyApp.Resources.DotNetVersion = netUtilities.VerzeDotNetFrameworkuProAssembly.ZiskejVerziDotNetFrameworku(AppDomain.CurrentDomain?.FriendlyName);
        }


        /// <summary>
        /// Nastaví kontext hlavního okna a spustí dispečerský časovač pro pravidelné úkoly.
        /// Inicializuje obsah hlavního okna s informacemi o monitorování výroby liquidů pro zvolenou linku a pozici.
        /// </summary>
        /// <typeparam name="W">Typ okna, které bude použito jako hlavní okno aplikace</typeparam>
        /// <param name="iMyApp">Instance rozhraní IIMyApp</param>
        /// <param name="appViewModel">Instance rozhraní IIViewModel obsahující data o aplikaci</param>
        /// <returns>Asynchronní úkol</returns>
        static async Task SetMainWindowContextAndStartTimer<W>(IMyApp iMyApp) where W : Window, new()
        {

            // Vytvoří novou instanci hlavního okna aplikace (typ W) a nastaví jeho datový kontext.
            iMyApp.Resources.AppWindow = new W
            {
                DataContext = iMyApp?.Resources?.AppViewModel
            };

            // Vytvoří nový dispečerský časovač pro vykonávání pravidelných úkolů.
            iMyApp.Resources.AppDispatcherTimer = new DispatcherTimer();

            // Nastaví hlavní okno aplikace a konfiguraci pomocí metody NastavHlavniOkno v rámci rozhraní IDispatcher.
            await iMyApp?.Resources?.AppWindow?.Dispatcher?.NastavHlavniOkno(iMyApp?.Resources?.AppWindow, iMyApp?.AppConfig?.DaikinAppConfig);
        }
    }
}
