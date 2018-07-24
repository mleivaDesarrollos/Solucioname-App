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

        internal abstract void loadAndCheckparameters(string[] parameters);
    }
}
