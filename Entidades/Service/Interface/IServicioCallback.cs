using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Entidades;


namespace Entidades.Service.Interface
{
    [ServiceContract]
    public interface IServicioCallback
    {
        [OperationContract(IsOneWay = true)]
        void Mensaje(string message);

        [OperationContract(IsOneWay = true)]
        void EnviarAsunto(Asunto a);

        [OperationContract(IsOneWay = true)]
        void ForceDisconnect();

        [OperationContract(IsOneWay = true)]
        void ServiceChangeStatusRequest(AvailabiltyStatus paramNewStatus);

        [OperationContract]
        bool IsActive();
    }
}
