﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Datos.SrvSolucioname {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="SrvSolucioname.IServicio", CallbackContract=typeof(Datos.SrvSolucioname.IServicioCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface IServicio {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/Conectar", ReplyAction="http://tempuri.org/IServicio/ConectarResponse")]
        bool Conectar(Entidades.Operador oper);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/Conectar", ReplyAction="http://tempuri.org/IServicio/ConectarResponse")]
        System.Threading.Tasks.Task<bool> ConectarAsync(Entidades.Operador oper);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/ConnectBackoffice", ReplyAction="http://tempuri.org/IServicio/ConnectBackofficeResponse")]
        Entidades.Operador ConnectBackoffice(Entidades.Operador oper);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/ConnectBackoffice", ReplyAction="http://tempuri.org/IServicio/ConnectBackofficeResponse")]
        System.Threading.Tasks.Task<Entidades.Operador> ConnectBackofficeAsync(Entidades.Operador oper);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, IsTerminating=true, Action="http://tempuri.org/IServicio/Disconnect")]
        void Disconnect(Entidades.Operador oper);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, IsTerminating=true, Action="http://tempuri.org/IServicio/Disconnect")]
        System.Threading.Tasks.Task DisconnectAsync(Entidades.Operador oper);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/EjecutarComando")]
        void EjecutarComando(Entidades.Operador oper, string strCommand);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/EjecutarComando")]
        System.Threading.Tasks.Task EjecutarComandoAsync(Entidades.Operador oper, string strCommand);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/AsuntoReceiptCompleted")]
        void AsuntoReceiptCompleted(Entidades.Asunto asuntoToConfirm);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/AsuntoReceiptCompleted")]
        System.Threading.Tasks.Task AsuntoReceiptCompletedAsync(Entidades.Asunto asuntoToConfirm);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/BatchAsuntoReceiptCompleted")]
        void BatchAsuntoReceiptCompleted(Entidades.Asunto[] lstAsuntoToConfirm);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/BatchAsuntoReceiptCompleted")]
        System.Threading.Tasks.Task BatchAsuntoReceiptCompletedAsync(Entidades.Asunto[] lstAsuntoToConfirm);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/SentAsuntoToOperator")]
        void SentAsuntoToOperator(Entidades.Operador prmOperatorBackoffice, Entidades.Asunto prmAsunto);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/SentAsuntoToOperator")]
        System.Threading.Tasks.Task SentAsuntoToOperatorAsync(Entidades.Operador prmOperatorBackoffice, Entidades.Asunto prmAsunto);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/SentBatchOfAsuntosToOperator")]
        void SentBatchOfAsuntosToOperator(Entidades.Operador prmOperatorBackoffice, Entidades.Asunto[] lstA);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/SentBatchOfAsuntosToOperator")]
        System.Threading.Tasks.Task SentBatchOfAsuntosToOperatorAsync(Entidades.Operador prmOperatorBackoffice, Entidades.Asunto[] lstA);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/getOperatorList", ReplyAction="http://tempuri.org/IServicio/getOperatorListResponse")]
        Entidades.Operador[] getOperatorList();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/getOperatorList", ReplyAction="http://tempuri.org/IServicio/getOperatorListResponse")]
        System.Threading.Tasks.Task<Entidades.Operador[]> getOperatorListAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/getListOfOperatorMustWorkToday", ReplyAction="http://tempuri.org/IServicio/getListOfOperatorMustWorkTodayResponse")]
        Entidades.Operador[] getListOfOperatorMustWorkToday();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/getListOfOperatorMustWorkToday", ReplyAction="http://tempuri.org/IServicio/getListOfOperatorMustWorkTodayResponse")]
        System.Threading.Tasks.Task<Entidades.Operador[]> getListOfOperatorMustWorkTodayAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/getAssignedAsuntosOfCurrentDay", ReplyAction="http://tempuri.org/IServicio/getAssignedAsuntosOfCurrentDayResponse")]
        Entidades.Asunto[] getAssignedAsuntosOfCurrentDay();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/getAssignedAsuntosOfCurrentDay", ReplyAction="http://tempuri.org/IServicio/getAssignedAsuntosOfCurrentDayResponse")]
        System.Threading.Tasks.Task<Entidades.Asunto[]> getAssignedAsuntosOfCurrentDayAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/getUnassignedAsuntos", ReplyAction="http://tempuri.org/IServicio/getUnassignedAsuntosResponse")]
        Entidades.Asunto[] getUnassignedAsuntos();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/getUnassignedAsuntos", ReplyAction="http://tempuri.org/IServicio/getUnassignedAsuntosResponse")]
        System.Threading.Tasks.Task<Entidades.Asunto[]> getUnassignedAsuntosAsync();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/SetStatus")]
        void SetStatus(Entidades.Operador operatorToChange, Entidades.AvailabiltyStatus paramNewStatus);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/SetStatus")]
        System.Threading.Tasks.Task SetStatusAsync(Entidades.Operador operatorToChange, Entidades.AvailabiltyStatus paramNewStatus);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/IsServiceActive", ReplyAction="http://tempuri.org/IServicio/IsServiceActiveResponse")]
        bool IsServiceActive(bool isBackoffice, Entidades.Operador prmOperator);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/IsServiceActive", ReplyAction="http://tempuri.org/IServicio/IsServiceActiveResponse")]
        System.Threading.Tasks.Task<bool> IsServiceActiveAsync(bool isBackoffice, Entidades.Operador prmOperator);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/Mensaje")]
        void Mensaje(string message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/EnviarAsunto")]
        void EnviarAsunto(Entidades.Asunto a);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/SentAsuntosBatch")]
        void SentAsuntosBatch(Entidades.Asunto[] lstA);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/AsuntoProcessCompleted")]
        void AsuntoProcessCompleted(Entidades.Asunto a);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/BatchAsuntoProcessCompleted")]
        void BatchAsuntoProcessCompleted(Entidades.Asunto[] lstA);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/ForceDisconnect")]
        void ForceDisconnect();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/ServiceChangeStatusRequest")]
        void ServiceChangeStatusRequest(Entidades.AvailabiltyStatus paramNewStatus);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/RefreshOperatorStatus")]
        void RefreshOperatorStatus();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/NotifyNewAsuntoFromSolucioname")]
        void NotifyNewAsuntoFromSolucioname();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IServicio/UpdateOnAsuntosWithoutAssignation")]
        void UpdateOnAsuntosWithoutAssignation();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServicio/IsActive", ReplyAction="http://tempuri.org/IServicio/IsActiveResponse")]
        bool IsActive();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServicioChannel : Datos.SrvSolucioname.IServicio, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServicioClient : System.ServiceModel.DuplexClientBase<Datos.SrvSolucioname.IServicio>, Datos.SrvSolucioname.IServicio {
        
        public ServicioClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public ServicioClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public ServicioClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public ServicioClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public bool Conectar(Entidades.Operador oper) {
            return base.Channel.Conectar(oper);
        }
        
        public System.Threading.Tasks.Task<bool> ConectarAsync(Entidades.Operador oper) {
            return base.Channel.ConectarAsync(oper);
        }
        
        public Entidades.Operador ConnectBackoffice(Entidades.Operador oper) {
            return base.Channel.ConnectBackoffice(oper);
        }
        
        public System.Threading.Tasks.Task<Entidades.Operador> ConnectBackofficeAsync(Entidades.Operador oper) {
            return base.Channel.ConnectBackofficeAsync(oper);
        }
        
        public void Disconnect(Entidades.Operador oper) {
            base.Channel.Disconnect(oper);
        }
        
        public System.Threading.Tasks.Task DisconnectAsync(Entidades.Operador oper) {
            return base.Channel.DisconnectAsync(oper);
        }
        
        public void EjecutarComando(Entidades.Operador oper, string strCommand) {
            base.Channel.EjecutarComando(oper, strCommand);
        }
        
        public System.Threading.Tasks.Task EjecutarComandoAsync(Entidades.Operador oper, string strCommand) {
            return base.Channel.EjecutarComandoAsync(oper, strCommand);
        }
        
        public void AsuntoReceiptCompleted(Entidades.Asunto asuntoToConfirm) {
            base.Channel.AsuntoReceiptCompleted(asuntoToConfirm);
        }
        
        public System.Threading.Tasks.Task AsuntoReceiptCompletedAsync(Entidades.Asunto asuntoToConfirm) {
            return base.Channel.AsuntoReceiptCompletedAsync(asuntoToConfirm);
        }
        
        public void BatchAsuntoReceiptCompleted(Entidades.Asunto[] lstAsuntoToConfirm) {
            base.Channel.BatchAsuntoReceiptCompleted(lstAsuntoToConfirm);
        }
        
        public System.Threading.Tasks.Task BatchAsuntoReceiptCompletedAsync(Entidades.Asunto[] lstAsuntoToConfirm) {
            return base.Channel.BatchAsuntoReceiptCompletedAsync(lstAsuntoToConfirm);
        }
        
        public void SentAsuntoToOperator(Entidades.Operador prmOperatorBackoffice, Entidades.Asunto prmAsunto) {
            base.Channel.SentAsuntoToOperator(prmOperatorBackoffice, prmAsunto);
        }
        
        public System.Threading.Tasks.Task SentAsuntoToOperatorAsync(Entidades.Operador prmOperatorBackoffice, Entidades.Asunto prmAsunto) {
            return base.Channel.SentAsuntoToOperatorAsync(prmOperatorBackoffice, prmAsunto);
        }
        
        public void SentBatchOfAsuntosToOperator(Entidades.Operador prmOperatorBackoffice, Entidades.Asunto[] lstA) {
            base.Channel.SentBatchOfAsuntosToOperator(prmOperatorBackoffice, lstA);
        }
        
        public System.Threading.Tasks.Task SentBatchOfAsuntosToOperatorAsync(Entidades.Operador prmOperatorBackoffice, Entidades.Asunto[] lstA) {
            return base.Channel.SentBatchOfAsuntosToOperatorAsync(prmOperatorBackoffice, lstA);
        }
        
        public Entidades.Operador[] getOperatorList() {
            return base.Channel.getOperatorList();
        }
        
        public System.Threading.Tasks.Task<Entidades.Operador[]> getOperatorListAsync() {
            return base.Channel.getOperatorListAsync();
        }
        
        public Entidades.Operador[] getListOfOperatorMustWorkToday() {
            return base.Channel.getListOfOperatorMustWorkToday();
        }
        
        public System.Threading.Tasks.Task<Entidades.Operador[]> getListOfOperatorMustWorkTodayAsync() {
            return base.Channel.getListOfOperatorMustWorkTodayAsync();
        }
        
        public Entidades.Asunto[] getAssignedAsuntosOfCurrentDay() {
            return base.Channel.getAssignedAsuntosOfCurrentDay();
        }
        
        public System.Threading.Tasks.Task<Entidades.Asunto[]> getAssignedAsuntosOfCurrentDayAsync() {
            return base.Channel.getAssignedAsuntosOfCurrentDayAsync();
        }
        
        public Entidades.Asunto[] getUnassignedAsuntos() {
            return base.Channel.getUnassignedAsuntos();
        }
        
        public System.Threading.Tasks.Task<Entidades.Asunto[]> getUnassignedAsuntosAsync() {
            return base.Channel.getUnassignedAsuntosAsync();
        }
        
        public void SetStatus(Entidades.Operador operatorToChange, Entidades.AvailabiltyStatus paramNewStatus) {
            base.Channel.SetStatus(operatorToChange, paramNewStatus);
        }
        
        public System.Threading.Tasks.Task SetStatusAsync(Entidades.Operador operatorToChange, Entidades.AvailabiltyStatus paramNewStatus) {
            return base.Channel.SetStatusAsync(operatorToChange, paramNewStatus);
        }
        
        public bool IsServiceActive(bool isBackoffice, Entidades.Operador prmOperator) {
            return base.Channel.IsServiceActive(isBackoffice, prmOperator);
        }
        
        public System.Threading.Tasks.Task<bool> IsServiceActiveAsync(bool isBackoffice, Entidades.Operador prmOperator) {
            return base.Channel.IsServiceActiveAsync(isBackoffice, prmOperator);
        }
    }
}
