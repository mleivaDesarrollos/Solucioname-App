using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    internal static class Conexion
    {
        // Disponemos de la cadena de conexión para el resto del aplicativo
        internal static String Cadena = ConfigurationManager.ConnectionStrings["Cadena"].ConnectionString;
    }
}
