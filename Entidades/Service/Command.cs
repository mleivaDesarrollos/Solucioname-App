using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Entidades.Service
{
    [DataContract]
    public abstract class Command
    {
        /// <summary>
        /// Nombre del comando
        /// </summary>
        [DataMember]
        public String Name
        {
            get; set;
        }

        /// <summary>
        /// Al instanciar un comando se debe definir el nombre del mismo
        /// </summary>
        /// <param name="commName"></param>
        public Command(string commName)
        {
            Name = commName;
        }

        /// <summary>
        /// Lista que contiene los tipos de comandos almacenados
        /// </summary>
        public static List<Command> List = new List<Command>()
        {
            new CommandType.Message(),
            new CommandType.Test()
        };

        /// <summary>
        /// Se recibe una cadena de caracteres como parametros y se procesa para conseguir un comando
        /// </summary>
        /// <param name="strCommand"></param>
        public static Command Get(string strCommand)
        {
            // Comprobamos si el comando recibió valores
            if (strCommand.Length == 0) throw new Exception("Empty Command.");
            // Dividimos la cadena de caracteres comunicada en comando y parametros
            string command = strCommand.Split(null)[0];
            string[] parameters = strCommand.Split(null).Skip(1).ToArray();
            // Buscamos si dentro de los listados de comandos almacenados se encuentra algún comando relacionado con el comando pasado por parametro
            Command commandResult = List.Find((cmdToFind) => cmdToFind.Name == command);
            // Si no encontramos el comando disparamos una excepción
            if (commandResult == null) throw new Exception("Command not found.");
            // Cargamos los parametros correspondientes al comando según lo pasado en la cadena de caracteres
            commandResult.loadAndCheckparameters(parameters);
            // Devolvemos el valor procesado
            return commandResult;
        }


        internal abstract void loadAndCheckparameters(string[] parameters);
    }
}
