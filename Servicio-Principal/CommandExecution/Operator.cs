using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades.Service;

namespace Servicio_Principal.CommandExecution
{
    public class Operator : Entidades.Service.CommandType.Operator, IExecution
    {
        public void Call(Servicio currentServiceInstance)
        {
            // Devolvemos al administrador de consola un listado de operadores conectados
            currentServiceInstance.retreiveListOfConnectedOperators();
        }

        public IExecution Convert(Command commSource)
        {
            // Generamos un comando para devolverlo
            Operator commOper = new Operator();
            // Convertimos el comando pasado por parametro al correspondiente comando
            Entidades.Service.CommandType.Operator commOperSource = commSource as Entidades.Service.CommandType.Operator;
            if (commOperSource == null) throw new Exception(Error.COMMAND_WITH_NOACTION_RELATED);
            // Devolvemos el valor procesado
            return commOper;
        }
    }
}
