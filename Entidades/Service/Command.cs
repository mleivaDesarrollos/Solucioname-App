using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades.Service.Commands;

namespace Entidades.Service
{
    public abstract class Command : IDisposable
    {
        public string Name { get; set; }

        public Command(string paramName)
        {
            Name = paramName;
        }

        public static readonly List<Command> List = new List<Command>()
        {
            new Message()
        };

        /// <summary>
        /// Chequeamos si el comando pasado por consola es valido, 
        /// se devuelve el comando en cuestión si cumple con los parametros y con el nombre.        
        /// </summary>
        /// <param name="sCommand"></param>
        /// <returns>null si no encuentro comando relacionado, devuelve un comando</returns>
        public static Command Get(string sCommand)
        {
            // Dividimos el comando por espacios
            string command = sCommand.Split(null)[0];
            string[] parameters = sCommand.Split(null).Skip(1).ToArray();
            // Buscamos el comando cargado dentro del listado de comandos
            Command cmdFind = List.Find((x) => x.Name.ToLower() == command.ToLower());
            // Si se encuentra el comando averiguamos si tiene la cantidad de parametros adecuados
            if (cmdFind != null)
            {
                // Duplicamos el comando para no modificar el lsitado original
                cmdFind = cmdFind.MemberwiseClone() as Command;
                // Chequeamos los parametros pasados al comando
                if (cmdFind.CheckParameters(parameters))
                {
                    return cmdFind;
                }                
            }            
            return null;
        }

        /// <summary>
        /// Metodo dispuesto para comprobar los parametros pasados por comando
        /// Cada herencia deberá chequear sus parametros correspondientes
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal abstract bool CheckParameters(string[] parameters);

        public void Dispose()
        {
            
        }
    }
}
