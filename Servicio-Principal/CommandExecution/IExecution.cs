using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio_Principal.CommandExecution
{
    public interface IExecution
    {
        void Call(Servicio currentServiceInstance);

        IExecution Convert(Entidades.Service.Command commSource)
    }
}
