using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Service.CommandType
{
    public class Message : Command
    {
        public Operador Destinatario
        {
            get; set;
        }

        public String Contenido
        {
            get; set;
        }

        public Message() : base("message")
        {

        }

        /// <summary>
        /// Chequeamos si los parametros enviados cumplen con los requisitos necesarios para que el comando pueda aceptarse como válido
        /// </summary>
        /// <param name="parameters"></param>
        internal override void loadAndCheckparameters(string[] parameters)
        {
            if (parameters.Length < 2) throw new Exception("ussage : message [target] [message]");
            // Subdividimos el parametro recibido en múltiples entradas
            string strDestinatario = parameters[0];
            // Generamos un StringBuilder para almacenar el mensaje
            StringBuilder sbContenido = new StringBuilder();
            // Recorremos todas palabras escritas luego del comando
            foreach (var strWord in parameters.Skip(1).ToArray())
            {
                sbContenido.Append(strWord + " ");                
            }
            Destinatario = new Operador() { UserName = strDestinatario };
            Contenido = sbContenido.ToString();
        }
    }
}
