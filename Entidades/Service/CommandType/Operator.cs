using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Service.CommandType
{
    public class Operator : Command
    {
        public Operator() : base("operator")
        {

        }

        internal override void loadAndCheckparameters(string[] parameters)
        {
            // El minimo de parametros es 1 y lleva una lista
            if (parameters.Length < 1 && parameters[0] != "list") throw new Exception("ussage : operator (list)");
        }
    }
}
