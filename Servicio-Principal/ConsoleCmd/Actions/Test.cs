using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio_Principal.ConsoleCmd.Actions
{
    public class Test : Action
    {
        public void Call(Servicio currentServiceInstance, string[] parameters)
        {
            currentServiceInstance.TestCommand();
        }
    }
}
