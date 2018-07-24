using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Service.CommandType
{
    public class Test : Command
    {
        public Test() : base("test")
        {

        }

        internal override void loadAndCheckparameters(string[] parameters)
        {
            
        }
    }
}
