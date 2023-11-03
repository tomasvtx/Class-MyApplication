using Logger.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static Logger.Model.Enums;

namespace MyApplication
{
    public class Exit
    {
        /// <summary>
        /// Asynchronně ukončuje a uvolňuje zdroje aplikace.
        /// </summary>
        /// <param name="iMyApp">Instance rozhraní IMyApp s konfigurací a dispečerem.</param>
        public static async Task ExitResourcesAsync(MyApplication.IMyApp iMyApp)
        {
            await Task.Run(() => ExitResources(iMyApp));
        }

        /// <summary>
        /// Metoda pro bezpečné ukončení a uvolnění zdrojů aplikace. Blokuje volající vlákno po dobu určenou v konstantě WaitTimeoutInSeconds.
        /// </summary>
        /// <param name="iMyApp">Instance rozhraní IMyApp, obsahující konfiguraci a dispečer.</param>
        private static async void ExitResources(MyApplication.IMyApp iMyApp)
        {
            ExitResourcesInternal(iMyApp).Wait(WaitTimeoutInSeconds);
            await Kill();
        }

        /// <summary>
        /// Konstanta definující dobu, po kterou bude zablokováno volající vlákno ve funkci ExitResourcesBlocking.
        /// </summary>
        private const int WaitTimeoutInSeconds = 4000;

        /// <summary>
        /// Konstanta definující dobu čekání pro uzavření a uvolnění sériového portu ve funkci CloseAndDisposeAsync.
        /// </summary>
        private const int SerialPortWaitTimeoutInSeconds = 2000;


        /// <summary>
        /// Asynchronní metoda, která provádí úplné ukončení a uvolnění všech zdrojů aplikace včetně zrušení úkolů a uzavření sériových portů.
        /// </summary>
        /// <param name="application">Instance aplikace</param>
        /// <param name="iMyApp">Instance rozhraní IMyApp, obsahující konfiguraci a dispečer</param>
        /// <returns>Asynchronní úkol reprezentující operaci ukončení a uvolnění zdrojů</returns>
        private static async Task ExitResourcesInternal(MyApplication.IMyApp iMyApp)
        {
            CancelTasks(iMyApp?.Resources?.CancellationTokenSource);

            foreach (var item in iMyApp?.AppConfig?.OracleDatabaseConfig)
            {
                try { 
                item.Value?.DbConnection?.Close();
                }catch { }
                try
                {
                    item.Value?.DbConnection?.Dispose();
                }
                catch { }
            }

            foreach (var item in iMyApp?.AppConfig?.SerialPortConfig)
            {
                CloseAndDisposeAsync(item.Value?.SerialPort);
            }

            await ExitAppAsync(iMyApp);
        }


        /// <summary>
        /// Synchronní metoda, která slouží k zrušení všech úkolů sledovaných pomocí CancellationTokenSource.
        /// </summary>
        /// <param name="application">Instance aplikace</param>
        /// <param name="cancellationTokenSource">CancellationTokenSource pro zrušení úkolů</param>
        private static void CancelTasks(CancellationTokenSource cancellationTokenSource)
        {
            try { cancellationTokenSource?.Cancel(); } catch { }
        }


        /// <summary>
        /// Synchronní metoda, která zajišťuje bezpečné uzavření a uvolnění otevřeného sériového portu.
        /// </summary>
        /// <param name="application">Instance aplikace</param>
        /// <param name="serialPort">Otevřený sériový port, který má být uzavřen a uvolněn</param>
        private static void CloseAndDisposeAsync(SerialPort serialPort)
        {
            Task.Run(() =>
            {
                try { serialPort?.Close(); } catch { }
                try { serialPort?.Dispose(); } catch { }
            }).Wait(SerialPortWaitTimeoutInSeconds);
        }


        /// <summary>
        /// Asynchronní metoda, která bezpečně ukončí běžící aplikaci použitím zadaného dispečera.
        /// </summary>
        /// <param name="application">Instance aplikace, která má být ukončena</param>
        /// <param name="dispatcher">Dispečer pro zpracování ukončení aplikace na správném vlákně</param>
        /// <returns>Asynchronní úkol reprezentující operaci ukončení aplikace</returns>
        private static async Task ExitAppAsync(IMyApp iMyApp)
        {
            await await Logger.Tasks.LoggerPublic.LoggerTitleInternal(iMyApp, "Aplikace ukončena operátorem", MethodBase.GetCurrentMethod(), GetState.Get(ProcessState.ReadyToExit));

            await iMyApp?.Resources?.AppWindow?.Dispatcher?.InvokeAsync(() => iMyApp?.Resources?.AppInstance?.Shutdown());
        }


        /// <summary>
        /// Asynchronní metoda, která bezpodmínečně ukončí běžící proces aplikace po uplynutí 5 sekund.
        /// </summary>
        /// <param name="application">Instance aplikace, která má být ukončena</param>
        /// <returns>Asynchronní úkol reprezentující operaci ukončení</returns>
        private static async Task Kill()
        {
            var sw = Stopwatch.StartNew();
            while (sw?.Elapsed <= TimeSpan.FromSeconds(5))
            {
                await Task.Delay(1);
            }
            Process.GetCurrentProcess()?.Kill();
        }
    }
}
