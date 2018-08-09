using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades.Service;

namespace Servicio_Principal.CommandExecution
{
    public class Message : Entidades.Service.CommandType.Message, IExecution
    {
        public void Call(Servicio currentServiceInstance)
        {
            if (Destinatario.UserName.ToLower() == "all")
            {
                // Enviamos el mensaje a todos los operadores conectados al servicio
                currentServiceInstance.MessageToAllOperators(Contenido);
            }
            else if (Destinatario.UserName.ToLower() == "backoffice")
            {
                // Checks if a backoffice is logged in
                if (currentServiceInstance.connectedBackoffice != null)
                {                             
                    // if To is a backoffice, send message to the current backoffice
                    currentServiceInstance.connectedBackoffice.Callback.Mensaje(Contenido);
                    // Notify on console the message sent
                    Console.WriteLine(currentServiceInstance.GetFullShortDateTime + ": message sent to backoffice + " + currentServiceInstance.connectedBackoffice.Operator.UserName + ".");
                }
                else
                {
                    // Notifies on console the message sent
                    Console.WriteLine(currentServiceInstance.GetFullShortDateTime + ": no logged backoffice on the service.");
                }
            }
        }

        public IExecution Convert(Command commSource)
        {
            // Generamos una nueva instancia del objeto mensaje con acción para que pueda ser devuelto en el proceso
            Message executionMessage = new Message();
            // Validamos si el commando pasado por parametro corresponde a una instancia correcta de comando
            Entidades.Service.CommandType.Message messageOrigen = commSource as Entidades.Service.CommandType.Message;
            // Si es nulo se rechaza el proceso
            if (messageOrigen == null) throw new Exception(Error.COMMAND_WITH_NOACTION_RELATED);
            // Trasladamos los parametros hacia el comando de ejecución
            executionMessage.Contenido = messageOrigen.Contenido;
            executionMessage.Destinatario = messageOrigen.Destinatario;
            // Devolvemos el valor procesado
            return executionMessage;
        }
    }
}
