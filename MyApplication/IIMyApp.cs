using Logger;
using System.Threading.Tasks;

namespace MyApplication
{
    /// <summary>
    /// Rozhraní definující metody a vlastnosti pro inicializaci a běh aplikace.
    /// </summary>
    public interface IMyApp
    {
        /// <summary>
        /// Spustí po inicializaci aplikace úkoly, které mají probíhat v pozadí.
        /// </summary>
        Task PostTask();

        /// <summary>
        /// Získá nebo nastaví konfiguraci aplikace
        /// </summary>
        IAppConfig AppConfig { get; set; }

        /// <summary>
        /// Získá nebo nastaví prostředky aplikace, jako jsou viewmodel, logger atd.
        /// </summary>
        IAppResources Resources { get; set; }

        bool EnableVadidatingDicz {  get; }
    }

}
