using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Service.CommandType
{
    public class Operator : Command
    {
        public Operador Related;

        private string ussageMessage = "ussage : operator (list) {(disconnect) [username]}";

        public Operator() : base("operator")
        {

        }

        internal override void loadAndCheckparameters(string[] parameters)
        {
            // El minimo de parametros es 1
            if (parameters.Length < 1) throw new Exception(ussageMessage);
            // Dividimos el subcomando
            string strSubcommand = parameters[0];
            // Comprobamos el contenido del subcomando y determinamos acción según
            switch (strSubcommand)
            {
                case "list":
                    break;
                case "disconnect":
                    // Si es disconnect, se necesitan dos parametros
                    if (parameters.Length != 2) throw new Exception(ussageMessage);
                    // Tomamos el valor de segundo parametro, que corresponde al nombre del usuario
                    string strUserName = parameters[1];
                    // Generamos una nueva entidad én la que almacenaremos el nombre de usuario.
                    Related = new Operador() { UserName = strUserName };
                    break;
                default:
                    throw new Exception(ussageMessage);
            }
        }
    }
}
