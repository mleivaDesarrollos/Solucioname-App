using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio_Principal.CommandExecution
{
    public static class Execution
    {
        public static List<IExecution> List = new List<IExecution>()
        {
            new Message(),
            new Test(),
            new Gestion(),
            new Operator(),
        };

        /// <summary>
        /// Se procesa el comando pasado por parametro y se busca acciones relacionadas. Si existe el comando se devuelve el objeto con sus acciones cargadas.
        /// </summary>
        /// <param name="commandRelated"></param>
        /// <returns>La accion con todos los parametros relacionados. Dispara una exception si no lo encuentra</returns>
        public static IExecution getRelatedAction(Entidades.Service.Command commandRelated)
        {
            // Recorremos el listado de acciones almacenadas
            foreach (var execution in List)
            {
                // Averiguamos si la accion actual es subclase del comando pasado por parametro
                if (execution.GetType().IsSubclassOf(commandRelated.GetType()))
                {
                    return execution.Convert(commandRelated);
                }
            }
            throw new Exception("No action for command founded.");
        }
    }
}
