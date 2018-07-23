using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Service.Commands.Execution
{
    public static class CommandExecution
    {
        public static List<ICommandExecution> lstCmdExec = new List<ICommandExecution>()
        {
            new MessageExecution()
        };
            
    }
}
