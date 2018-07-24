using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades.Service;

namespace Servicio_Principal.CommandExecution
{
    public class Test : Entidades.Service.CommandType.Test, IExecution
    {
        public void Call(Servicio currentServiceInstance)
        {
            
        }

        public IExecution Convert(Command commSource)
        {
            // Generamos una nueva entidad para devolverlo en el proceso
            Test executionTest = new Test();
            // Convertimos el comando pasado por parametro al tipo correspondiente
            Entidades.Service.CommandType.Test testCommand = commSource as Entidades.Service.CommandType.Test;
            // Si el comando no es encontrado, se dispara una exception informando el resultado
            if (testCommand == null) throw new Exception(Error.COMMAND_WITH_NOACTION_RELATED);
            // Devolvemos el comando procesado
            return executionTest;
        }
    }
}
