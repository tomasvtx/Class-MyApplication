using AppConfigure;
using AppConfigure.Model.Xml;
using AppConfigure.Model.Xml.BaseModel;
using AppConfigure.Utils;
using Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static AppConfigure.Model.BaseModelProgr.BaseModelProgr;

namespace MyApplication.Startup
{
    internal class XmlParser
    {
        /// <summary>
        /// Nastaví složku pro obrázky na základě konfigurace aplikace. 
        /// Pokud je povolena relativní cesta, použije aktuální adresář aplikace.
        /// </summary>
        /// <param name="iMyApp">Instance rozhraní IIMyApp</param>
        /// <returns>Asynchronní úkol</returns>
        internal static Task SetImageFolder(IMyApp iMyApp)
        {
            iMyApp.AppConfig.ImageFolder = iMyApp?.AppConfig?.AppConfiguration?.ImgFolder;

            // Pokud je povoleno použití umístění aplikace pro složku obrázků
            if (iMyApp?.AppConfig?.AppConfiguration?.ImgFolder?.UseAppLocation ?? false)
            {
                // Nastaví složku pro obrázky na základě aktuálního adresáře aplikace
                iMyApp.AppConfig.ImageFolder.FolderLocation = Path.Combine(Environment.CurrentDirectory, iMyApp?.AppConfig?.AppConfiguration?.ImgFolder?.FolderLocation);
            }
            else
            {
                // Nastaví složku pro obrázky na základě konfigurace
                iMyApp.AppConfig.ImageFolder.FolderLocation = iMyApp?.AppConfig?.AppConfiguration?.ImgFolder?.FolderLocation;
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// Načte hlavní nastavení aplikace z XML a zpracuje ho.
        /// </summary>
        /// <param name="iMyApp">Instance rozhraní aplikace</param>
        /// <param name="application">Instance aplikace</param>
        /// <returns>True, pokud načtení nastavení proběhlo úspěšně; jinak False</returns>
        internal static async Task<bool> LoadMainSettings(IMyApp iMyApp, Application application)
        {
            iMyApp.AppConfig.AppConfiguration = new AppConfiguration();

            if (!ArgsCooperation.TryReadXML(out AppConfiguration AppConfigureg, out string error))
            {
                // Pokud načtení XML selže, zavolá se metoda pro zpracování chyby a vrátí se False.
                await HandleError.HandleXmlReadError(error, application);
                return false;
            }
            iMyApp.AppConfig.AppConfiguration = AppConfigureg;
           
            // V případě úspěšného načtení XML se vrátí True.
            return true;
        }



        /// <summary>
        /// Inicializuje konfiguraci BCS čtečky.
        /// </summary>
        /// <param name="iMyApp">Instance rozhraní aplikace</param>
        internal static void InitializeBCSReaderConfiguration(IMyApp iMyApp)
        {
            // Inicializuje konfiguraci série sériových portů v aplikaci.
            iMyApp.AppConfig.SerialPortConfig = new Dictionary<string, SerialPortConfProg>();

            // Prochází všechny konfigurace sériových portů z hlavního nastavení aplikace.
            foreach (var SerialConfig in iMyApp?.AppConfig?.AppConfiguration?.SerialPort)
            {
                // Přidá konfiguraci sériového portu do slovníku s klíčem podle popisu.
                iMyApp?.AppConfig?.SerialPortConfig?.Add(SerialConfig.Description, new SerialPortConfProg { SerialPortConf = SerialConfig, SerialPort = null });
            }

            // Pokud nebyl definován žádný sériový port, přidá výchozí konfiguraci "MAIN".
            if (iMyApp?.AppConfig?.SerialPortConfig?.Count == 0)
            {
                iMyApp.AppConfig.SerialPortConfig.Add("MAIN", new SerialPortConfProg { SerialPort = null, SerialPortConf = new SerialPortConf { Description = "MAIN", BcsDelay = 900, ClearBuffer = true, PortName = "COM1" } });
            }

        }


        /// <summary>
        /// Inicializuje hlavní databázi aplikace na základě konfigurace.
        /// </summary>
        /// <param name="iMyApp">Instance rozhraní IIMyApp</param>
        /// <param name="systemInfoUtility">Instance utility pro systémové informace</param>
        /// <param name="application">Instance aplikace</param>
        /// <returns>True, pokud byla inicializace úspěšná; jinak False.</returns>
        internal static async Task<bool> InitializeMainDatabase(IMyApp iMyApp, SystemInfoUtility systemInfoUtility, Application application)
        {
            // Inicializuje slovník pro konfigurace hlavní databáze aplikace.
            iMyApp.AppConfig.OracleDatabaseConfig = new Dictionary<string, DatabaseConfProg>();

            // Prochází všechny konfigurace databází z hlavního nastavení aplikace a přidá je do slovníku.
            foreach (var DBConfOracle in iMyApp?.AppConfig?.AppConfiguration?.Database)
            {
                    iMyApp?.AppConfig?.OracleDatabaseConfig.Add(DBConfOracle.Description, new DatabaseConfProg
                    {
                        DatabaseConf = DBConfOracle,
                        DbConnection = OracleSQL.Tasks.OracleProvider.Get(DBConfOracle?.ConnectionString)
                    });
            }

            // Pokud konfigurace hlavní databáze nebyla nalezena, zpracuje se chyba konfigurace.
            if (iMyApp?.AppConfig?.OracleDatabaseConfig == null || iMyApp?.AppConfig?.OracleDatabaseConfig?.Count == 0)
            {
                await HandleError.HandleConfigurationError(iMyApp, "ConnectionString is not set", systemInfoUtility, application);
                return false; // Inicializace selhala.
            }

            return true; // Inicializace proběhla úspěšně.
        }
    }
}
