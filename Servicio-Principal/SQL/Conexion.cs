using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio_Principal.SQL
{
    internal class Conexion
    {
        /// <summary>
        /// Cadena de conexión utilizada para acceder a los datos alamacenados en persistencia de base de datos
        /// </summary>
        internal static string Cadena = ConfigurationManager.ConnectionStrings["CadenaServicioRelease"].ConnectionString;
    }
}
