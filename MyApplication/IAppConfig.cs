using AppConfigure;
using AppConfigure.Model.Args;
using AppConfigure.Model.Xml;
using AppConfigure.Model.Xml.BaseModel;
using System.Collections.Generic;
using System.Windows.Threading;
using static AppConfigure.Model.BaseModelProgr.BaseModelProgr;

namespace MyApplication
{
    /// <summary>
    /// Rozhraní definující konfiguraci aplikace a její součásti.
    /// </summary>
    public interface IAppConfig
    {
        /// <summary>
        /// Získá argumenty aplikace.
        /// </summary>
        Argumenty Argument { get; set; }

        /// <summary>
        /// Získá  konfiguraci specifickou pro aplikaci Daikin.
        /// </summary>
        AppConfiguration AppConfiguration { get; set; }

        /// <summary>
        /// Získá prioritu dispečera pro produkční úkoly aplikace.
        /// </summary>
        DispatcherPriority ProductionPriority { get; }

        /// <summary>
        /// Získá nebo nastaví konfiguraci složky pro obrázky aplikace.
        /// </summary>
        ImgFolder ImageFolder { get; set; }

        /// <summary>
        /// Získá nebo nastaví konfiguraci sériových portů aplikace.
        /// </summary>
        Dictionary<string, SerialPortConfProg> SerialPortConfig { get; set; }

        /// <summary>
        /// Získá nebo nastaví konfiguraci Oracle databází aplikace.
        /// </summary>
        Dictionary<string, DatabaseConfProg> OracleDatabaseConfig { get; set; }
    }

}
