using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio_Principal.SQL
{
    public static class Asunto
    {
        /// <summary>
        /// Agrega un asunto a la base de datos de respaldo
        /// </summary>
        /// <param name="asunto"></param>
        public static void AddToQueue(Entidades.Asunto asunto)
        {

        }

        /// <summary>
        /// Remueve un asunto de la base de datos de respaldado
        /// </summary>
        /// <param name="asunto"></param>
        public static void RemoveFromQueue(Entidades.Asunto asunto)
        {

        }

        /// <summary>
        /// Obtiene un listado de asuntos almacenados pendientes de envío a clientes en la base de datos de respaldo
        /// </summary>
        /// <returns></returns>
        public static List<Entidades.Asunto> getQueue()
        {
            return null;
        }
    }
}
