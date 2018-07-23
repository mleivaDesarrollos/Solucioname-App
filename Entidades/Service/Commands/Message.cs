using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Service.Commands
{
    public class Message : Command
    {
        public Operador Destinatario
        {
            get; set;
        }

        public String Mensaje
        {
            get; set;
        }

        public Message() : base("message")
        {

        }

        internal override bool CheckParameters(string[] parameters)
        {
            if (parameters.Length >= 2)
            {
                // Dividimos los parametros
                Destinatario = new Operador() { UserName = parameters[0] };
                StringBuilder sb = new StringBuilder();
                foreach (var msjPart in parameters.Skip(1).ToArray())
                {
                    sb.Append(msjPart + " ");
                }
                Mensaje = sb.ToString();
                return true;
            }
            return false;
        }
    }
}
