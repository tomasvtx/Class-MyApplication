using Logger;
using System.ComponentModel;
using System.Windows.Controls;

namespace MyApplication
{
    /// <summary>
    /// Rozhraní definující model (ViewModel) aplikace, který implementuje notifikace o změnách vlastností.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Získá nebo nastaví název aplikace.
        /// </summary>
        string ApplicationTitle { get; set; }

        ILogManager LogManager { get; set; }

        /// <summary>
        /// ListView, který bude použit pro zobrazování logu v aplikaci.
        /// </summary>
        ListView LogerViewer { get; set; }
    }

}
