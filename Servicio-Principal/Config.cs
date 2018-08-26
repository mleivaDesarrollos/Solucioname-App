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
        public static readonly bool Test = Convert.ToBoolean(ConfigurationManager.AppSettings.GetValues("TEST")[0]);
    }
}
