using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio_Principal
{
    public static class Error
    {
        public static string COMMAND_WITH_NOACTION_RELATED = "Imcompatible command with action";

        /// <summary>
        /// Cuando un operador común intenta realizar la llamada a un comando de consola, el servicio debe rechazar la petición. Necesita un parametro.
        /// </summary>
        public static string CONSOLE_COMMAND_CALLED_BY_STANDARD_USER = "Warning: Operator {0} is trying to call a console command. Permission Denied.";
    }
}
