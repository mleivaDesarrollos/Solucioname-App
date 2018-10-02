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
            // Agregamos el asunto al listado de entrega de asuntos del servicio
            currentServiceInstance.DeliverAsuntoList.Add(Asunto);
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
