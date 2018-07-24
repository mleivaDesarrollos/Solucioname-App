using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Entidades;

namespace Servicio_Principal
{
    [ServiceContract(CallbackContract = typeof(IServicioCallback), SessionMode = SessionMode.Required)]    
    public interface IServicio
    {
        [OperationContract(IsInitiating = true)]
        bool Conectar(Operador oper);

        [OperationContract(IsOneWay = true)]
        void EjecutarComando(Operador oper, Entidades.Service.Command command);
    }
}
