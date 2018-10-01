using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using Entidades.Service.Interface;

namespace Servicio_Principal
{
    [ServiceContract(CallbackContract = typeof(IServicioCallback), SessionMode = SessionMode.Required)]
    public interface IServicio
    {
        [OperationContract(IsInitiating = true)]
        bool Conectar(Operador oper);

        [OperationContract(IsInitiating = true)]
        Operador ConnectBackoffice(Operador oper);

        [OperationContract(IsTerminating = true, IsOneWay = true)]
        void Disconnect(Operador oper);

        [OperationContract(IsOneWay = true)]
        void EjecutarComando(Operador oper, string strCommand);

        [OperationContract(IsOneWay = true)]
        void AsuntoReceiptCompleted(Entidades.Asunto asuntoToConfirm);

        [OperationContract(IsOneWay = true)]
        void BatchAsuntoReceiptCompleted(List<Entidades.Asunto> lstAsuntoToConfirm);

        [OperationContract(IsOneWay = true)]
        void SentAsuntoToOperator(Operador prmOperatorBackoffice, Asunto prmAsunto);

        [OperationContract(IsOneWay = true)]
        void SentBatchOfAsuntosToOperator(Operador prmOperatorBackoffice, List<Entidades.Asunto> lstA);

        [OperationContract]
        List<Entidades.Operador> getOperatorList();

        [OperationContract]
        List<Operador> getListOfOperatorMustWorkToday();

        [OperationContract]
        List<Asunto> getAssignedAsuntosOfCurrentDay();

        [OperationContract]
        List<Asunto> getUnassignedAsuntos();

        [OperationContract(IsOneWay = true)]
        void SetStatus(Operador operatorToChange, AvailabiltyStatus paramNewStatus);

        [OperationContract]
        bool IsServiceActive(bool isBackoffice, Operador prmOperator);
    }
}
