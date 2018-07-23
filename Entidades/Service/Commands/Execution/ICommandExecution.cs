using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Service.Commands.Execution
{
    public interface ICommandExecution
    {
        void Call(string pMsj);
    }
}
