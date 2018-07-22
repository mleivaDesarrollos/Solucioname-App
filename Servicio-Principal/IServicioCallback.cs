using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Entidades;

namespace Servicio_Principal
{
    [ServiceContract]
    public interface IServicioCallback
    {
        [OperationContract(IsOneWay = true)]
        void Mensaje(Mensaje m);

        [OperationContract(IsOneWay = true)]
        void EnviarAsunto(Asunto a);
    }
}
