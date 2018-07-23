using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades.Service.Commands;

namespace Servicio_Principal.ConsoleCmd
{
    public class MessageExecution : Message, ICommandExecution
    {
        public void Call(Servicio currentServiceInstance)
        {
            
        }
    }
}
