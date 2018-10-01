using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Servicio_Principal
{
    public static class Config
    {
        /// <summary>
        /// When needs test on weekend, set in config 1, also 0
        /// </summary>
        public static readonly bool TEST = Convert.ToBoolean(ConfigurationManager.AppSettings.GetValues("TEST")[0]);

        public static readonly int BACKOFFICE_TIMEOUT = Convert.ToInt32(ConfigurationManager.AppSettings.GetValues("BACKOFFICE_TIMEOUT")[0]);

        public static readonly double ASUNTO_SERVICE_CHECK_CICLE_TIME = Convert.ToDouble(ConfigurationManager.AppSettings.GetValues("ASUNTO_SERVICE_CHECK_CICLE_TIME")[0]);

        public static readonly bool TEST_MAILING_SERVICE = Convert.ToBoolean(ConfigurationManager.AppSettings.GetValues("TEST_MAILING_SERVICE")[0]);
    }
}
