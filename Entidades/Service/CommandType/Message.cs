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

        internal override void loadAndCheckparameters(string[] parameters)
        {
            
        }
    }
}
