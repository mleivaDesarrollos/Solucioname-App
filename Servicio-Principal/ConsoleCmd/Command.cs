using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio_Principal
{
    public class Command
    {
        public string Name { get; set; }

        public int Parameters { get; set; }

        public ConsoleCmd.Actions.Action Action { get; set; }

        public static List<Command> List = new List<Command>()
        {
            new Command() { Name = "test" , Parameters = 0, Action = new ConsoleCmd.Actions.Test()},
            new Command() { Name = "dummy", Parameters = 3, Action = new ConsoleCmd.Actions.Test()},
            new Command() { Name = "message" , Parameters = 2, Action = new ConsoleCmd.Actions.Test()},
        };

        /// <summary>
        /// Chequeamos si el comando pasado por consola es valido
        /// </summary>
        /// <param name="sCommand"></param>
        /// <returns></returns>
        public static bool Check(string sCommand)
        {
            // Dividimos el comando por espacios
            string[] commandDivided = sCommand.Split(null);
            // Buscamos el comando cargado dentro del listado de comandos
            Command cmdFind = List.Find((x) => x.Name.ToLower() == commandDivided[0].ToLower());
            // Si se encuentra el comando averiguamos si tiene la cantidad de parametros adecuados
            if(cmdFind != null)
            {
                // Consultamos si la cantidad de parametros corresponde
                if(cmdFind.Parameters == commandDivided.Length - 1)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Ejecuta el comando pasado por parametro
        /// </summary>
        /// <returns></returns>
        public static void Run(Servicio srvCurrInstance, string sCommand)
        {
            // Divimos el comando obtenido
            string command = sCommand.Split(null)[0];
            string[] parameters = sCommand.Split(null).Skip(1).ToArray();
            Command cmdToExec = List.Find((cmd) => cmd.Name == command);
            // Para evitar errores comprobamos si encontramos el comando
            if(cmdToExec != null)
            {
                // Ejecutamos la accion vinculada
                cmdToExec.Action.Call(srvCurrInstance, parameters);
            }
        }
    }
}
