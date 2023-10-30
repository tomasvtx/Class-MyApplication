using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MyApplication
{
    /// <summary>
    /// Rozhraní definující zdroje a prostředky pro aplikaci.
    /// </summary>
    public interface IAppResources
    {
        /// <summary>
        /// Získá nebo nastaví rozhraní modelu aplikace (ViewModel).
        /// </summary>
        IViewModel AppViewModel { get; set; }

        /// <summary>
        /// Získá typ aplikace.
        /// </summary>
        Type AppType { get; }

        /// <summary>
        /// Získá nebo nastaví verzi .NET Framework, na které aplikace běží.
        /// </summary>
        string DotNetVersion { get; set; }

        /// <summary>
        /// Získá nebo nastaví hlavní okno aplikace.
        /// </summary>
        Window AppWindow { get; set; }

        /// <summary>
        /// Získá instanci CancellationTokenSource pro správu asynchronních úkolů.
        /// </summary>
        CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// Získá nebo nastaví dispečerský časovač pro hlavní úkoly aplikace.
        /// </summary>
        DispatcherTimer AppDispatcherTimer { get; set; }

        /// <summary>
        /// Získá nebo nastaví instanci aplikace.
        /// </summary>
        Application AppInstance { get; }
    }

}
