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
        /// Operador común intenta realizar la llamada a un comando de consola, el servicio debe rechazar la petición. Necesita un parametro.
        /// </summary>
        public static string CONSOLE_COMMAND_CALLED_BY_STANDARD_USER = "Warning: Operator {0} is trying to call a console command. Permission Denied.";

        /// <summary>
        /// No se encuentra callback relacionado al operador. Requiere un parametro (username de operador)
        /// </summary>
        public static string CALLBACK_RELATED_WITH_OPERATOR_NOTFOUND = "Error: Callback related to {0} not found.";

        /// <summary>
        /// If the Backoffice sender is not logged throw an exception
        /// </summary>
        public static string BACKOFFICE_SENDER_IS_NOT_CORRECTLY_LOGGED = "backoffice asunto sender is not logged on the service. Rejecting asunto sent request";
    }
}
