using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades.Service;

namespace Servicio_Principal.CommandExecution
{
    public class Gestion : Entidades.Service.CommandType.Gestion, IExecution
    {
        public void Call(Servicio currentServiceInstance)
        {
            // Obtenemos el Callback desde el cliente
            IServicioCallback callbackCliente = currentServiceInstance.getOperatorCallback(Asunto.Oper);
            if (callbackCliente == null) throw new Exception(string.Format(Error.CALLBACK_RELATED_WITH_OPERATOR_NOTFOUND, Asunto.Oper.UserName));
            // Se envía el asunto utilizando los métodos de callback del cliente para enviar la solicutd
            callbackCliente.EnviarAsunto(Asunto);

        }

        /// <summary>
        /// Convierte el Comando a un comando ejecutable
        /// </summary>
        /// <param name="commSource"></param>
        /// <returns></returns>
        public IExecution Convert(Command commSource)
        {
            // Generamos una nueva entidad de comando ejecutable
            Gestion gestionExecutable = new Gestion();
            // Convertimos el comando al tipo especializado de comando
            Entidades.Service.CommandType.Gestion commGestion = commSource as Entidades.Service.CommandType.Gestion;
            if (commGestion == null) throw new Exception(Error.COMMAND_WITH_NOACTION_RELATED);
            // Se almacenan los datos del comando sobre el ejecutable
            gestionExecutable.Asunto = commGestion.Asunto;
            // Se devuelve el proceso obtenido
            return gestionExecutable;
        }
    }
}
