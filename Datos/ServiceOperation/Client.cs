using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.SrvSolucioname;
using Entidades;
using System.ServiceModel;
using System.Timers;
using Errors;

namespace Datos.ServiceOperation
{
    /// <summary>
    /// Implementación de cliente de servicio
    /// </summary>
    public sealed class Client : IServicioCallback
    {
        #region singleton_interface_implementation
        public static Client Instance { get { return lazy.Value; } }

        private static readonly Lazy<Client> lazy = new Lazy<Client>(() => new Client());
        #endregion

        #region class_constructor
        private Client() {
            // Configure the timer service of check service status
            configureCheckStatusOfConnectionWithService();
        }
        #endregion

        #region proxy_administration
        /// <summary>
        /// Propiedad que maneja el estado de conexión con el servicio
        /// </summary>
        private ServicioClient _proxy = null;

        private ServicioClient proxy {
            get {
                if (_proxy == null) {
                    // Generamos un contexto nuevo que permitirá generar una nueva conexión
                    InstanceContext context = new InstanceContext(this);
                    // Generamos un nuevo servicio de cliente a partir de este contexto
                    _proxy = new ServicioClient(context);
                }
                return _proxy;
            }
            set {
                _proxy = null;
            }
        }

        /// <summary>
        /// Administración de estados del proxy
        /// </summary>
        private async Task HandleProxy()
        {
            switch (proxy.State) {
                case CommunicationState.Created:
                    await (Task.Run(() => { OpenProxy(); })).TimeoutAfter(defaultGeneralTimeout);
                    break;
                case CommunicationState.Opening:
                    break;
                case CommunicationState.Opened:
                    break;
                case CommunicationState.Closing:
                    break;
                case CommunicationState.Closed:
                    proxy = null;
                    await (Task.Run(() => { OpenProxy(); })).TimeoutAfter(defaultGeneralTimeout);
                    break;
                case CommunicationState.Faulted:
                    proxy = null;
                    await (Task.Run(() => { OpenProxy(); })).TimeoutAfter(defaultGeneralTimeout);
                    break;
                default:
                    break;
            }
        }

        private void OpenProxy()
        {
            try {
                // Run the open service connection with async type with timeout
                proxy.Open();
            }
            catch (Exception) {

            }
        }

        private async Task prepareProxy()
        {
            proxy = null;
            // Checks and set current status of proxy method
            await HandleProxy();
            // if the proxy is opened, the proxy is ready to use
            if (!(proxy.State == CommunicationState.Opened)) {
                throw new Exception("el servicio no está disponible.");
            }

        }
        #endregion

        #region property_client
        private Entidades.Service.Interface.IServicioCallback _callbackInteraction;

        public Entidades.Service.Interface.IServicioCallback callbackInteraction {
            get {
                if (_callbackInteraction == null) throw new Exception("Error ClientBO : The callback has not been setted.");
                return _callbackInteraction;
            }
            set {
                _callbackInteraction = value;
            }
        }

        Timer _tmrCheckActiveService;

        Entidades.Operador operatorLogged;

        bool isBackoffice = false;

        /// <summary>
        /// Timeout on async methods to service.
        /// </summary>
        static readonly double defaultGeneralTimeout = 5000;
        #endregion

        #region public_interface
        /// <summary>
        /// Conexión hacia el servicio
        /// </summary>
        /// <returns></returns>
        public async Task<Entidades.Operador> ConnectBackoffice(Entidades.Operador pOperator, Entidades.Service.Interface.IServicioCallback paramCallback)
        {
            // Aguardamos que el proxy este disponible
            await prepareProxy();
            // Almacenamos la respuesta del servicio en una variable que deberia venir completa
            Entidades.Operador backofficeOper = proxy.ConnectBackoffice(pOperator);
            if (backofficeOper != null) {
                // Sets callback for the client
                callbackInteraction = paramCallback;
                // Starts task to check for service activity status
                startCheckStatusOfConnectionWithService();
                // Set up local variables
                operatorLogged = pOperator;
                isBackoffice = true;
                // Devolvemos el operador con todos sus datos cargados                
                return backofficeOper;
            }
            else {
                throw new Exception("El usuario o contraseña no son validos, o no se disponen de permisos de backoffice.");
            }
        }

        /// <summary>
        /// Sent a request petition to the service for connection
        /// </summary>
        /// <param name="paramOperator"></param>
        /// <param name="paramCallback"></param>
        /// <returns></returns>
        public async Task<bool> ConnectOperator(Entidades.Operador paramOperator, Entidades.Service.Interface.IServicioCallback paramCallback)
        {
            try {
                await prepareProxy();
                // When connection is established we need check if the operator credentials are valid
                if (proxy.Conectar(paramOperator)) {
                    // when connection has been acepted, the callback is saved to memory
                    callbackInteraction = paramCallback;
                    // Setup variable for operator logged
                    operatorLogged = paramOperator;
                    // Starts service checking activity
                    startCheckStatusOfConnectionWithService();
                    return true;
                }
                throw new Exception("Las credenciales locales no se condicen con las credenciales de servicio. Consulte al administrador.");
            }
            catch (TimeoutException) {
                throw new Exception("El servicio no responde. Se ha agotado el tiempo de espera para la conexión. Contacte al administrador.");
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Sents a disconnect petition to service
        /// </summary>
        /// <param name="paramOperatorToDisconnect">Operator to disconnect</param>
        public async Task DisconnectOperator(Entidades.Operador paramOperatorToDisconnect)
        {
            // Stop checking service
            stopCheckStatusOfConnectionWithService();
            try {
                await prepareProxy();
                // Run disconnection on the service
                await Task.Run(() => { proxy.Disconnect(paramOperatorToDisconnect); }).TimeoutAfter(5000);

            }
            catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Sent a request to service for changing current status on the service
        /// </summary>
        /// <param name="pOperator"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public async Task ChangeCurrentStatus(Entidades.Operador prmOper, AvailabiltyStatus newStatus)
        {
            try {
                // Checks if the proxy is ready
                await prepareProxy();
                // Sent to the service petition to change te current status
                proxy.SetStatus(prmOper, newStatus);
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Sent to the service an asunto to deliver
        /// </summary>
        /// <param name="prmBackofficeSender">Backoffice who sends asunto</param>
        /// <param name="prmAsuntoToSent">Asunto to send</param>
        public async Task SentAsuntoToOperator(Entidades.Operador prmBackofficeSender, Entidades.Asunto prmAsuntoToSent)
        {
            try {
                // Manage status of the proxy
                await HandleProxy();
                proxy.SentAsuntoToOperator(prmBackofficeSender, prmAsuntoToSent);
            }
            catch (Exception ex) {
                Except.Throw(ex);
            }
        }

        /// <summary>
        /// Sent to proxy service a request for distribute a batch of asuntos
        /// </summary>
        /// <param name="prmBackofficeSender"></param>
        /// <param name="prmListOfAsunto"></param>
        public async Task SentBatchAsuntoToOperators(Entidades.Operador prmBackofficeSender, List<Entidades.Asunto> prmListOfAsunto)
        {
            try {
                await HandleProxy();
                proxy.SentBatchOfAsuntosToOperator(prmBackofficeSender, prmListOfAsunto.ToArray());
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Communicates with the service and get a list of connected operators with full data
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entidades.Operador>> GetFullOperatorList()
        {
            try {
                // Prepare proxy for operation
                await prepareProxy();
                // Return the list to the interface
                return proxy.getOperatorList().ToList();
            }
            catch (Exception ex) {
                throw ex;
            }
        }


        /// <summary>
        /// Get a complete list from service of current working operator of the day
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entidades.Operador>> GetOperatorWorkingToday()
        {
            try {
                // Prepares proxy for operation
                await prepareProxy();
                // Return list of operator must be connected today
                return proxy.getListOfOperatorMustWorkToday().ToList();
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        
        /// <summary>
        /// Gets from service a list of asuntos assigned today
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entidades.Asunto>> GetListOfAsuntosAssignedToday()
        {
            try {
                // Prepares proxy for request to service
                await prepareProxy();
                // gets list from the service and return to the caller
                return proxy.getAssignedAsuntosOfCurrentDay().ToList() ;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<List<Entidades.Asunto>> GetListOfUnassignedAsuntos()
        {
            try {
                // Prepares proxy for operation
                await prepareProxy();
                // Calls service for get list of asuntos
                return proxy.getUnassignedAsuntos().ToList();
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        #endregion

        #region private_helper_methods
        /// <summary>
        /// Configure timer on subthread for periodically check of service status
        /// </summary>
        private void configureCheckStatusOfConnectionWithService()
        {
            // Checks if the service is null for new instance
            if (_tmrCheckActiveService == null) {
                // Generates new timer instance and configure it
                _tmrCheckActiveService = new Timer();
                _tmrCheckActiveService.Elapsed += _tmrCheckActiveService_Elapsed;
                _tmrCheckActiveService.Interval = 60000;                                
            }            
        }

        /// <summary>
        /// Starts task of checking status of the service
        /// </summary>
        private void startCheckStatusOfConnectionWithService()
        {
            // If the service isn't configured, cannot be started
            if (_tmrCheckActiveService != null) {
                if(!_tmrCheckActiveService.Enabled) {
                    _tmrCheckActiveService.Enabled = true;
                }
                // if the service is already active, the method don't do anything
            }
            else {
                // Exception is throwed if the timer is isnt configured
                throw new Exception("El chequeo de estado de servicio no puede iniciarse por que no está configurado.");
            }
        }
        /// <summary>
        /// Stops task checking of service
        /// </summary>
        private void stopCheckStatusOfConnectionWithService()
        {
            // If the service isn't configured, cannot be stopped
            if (_tmrCheckActiveService != null) {
                if (_tmrCheckActiveService.Enabled) {
                    _tmrCheckActiveService.Enabled = false;
                }
                // If the service is already stopped, the method don't do anything
            }
        }

        private async void _tmrCheckActiveService_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Gets Client's Interface for a easy calls multiple methods from explicit interface
            IServicioCallback UIInteraction = this;
            try {
                // Configure timeout for wait response
                await runAsyncTimeoutMethod(() => { return proxy.IsServiceActiveAsync(isBackoffice, operatorLogged); }, defaultGeneralTimeout);                 
            }
            catch (Exception) {
                // Sent status message to the UI
                UIInteraction.Mensaje("Se ha perdido conexión con el servicio. Se procede a detener la conexión.");
                // Change the UI graphic status
                UIInteraction.ServiceChangeStatusRequest(AvailabiltyStatus.Disconnected);
                // Sets proxy to null
                proxy = null;
                // Stop timer activity
                stopCheckStatusOfConnectionWithService();
            }
        }

        /// <summary>
        /// Generic Methods : Allows to the class for calling async methods with timeout
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="tskToExecute"></param>
        /// <returns></returns>
        private async Task runAsyncTimeoutMethod<TResult>(Func<Task<TResult>> tskToExecute, double timeoutInMilliseconds)
        {
            await tskToExecute().TimeoutAfter(timeoutInMilliseconds);
        }

        /// <summary>
        /// Generic Methods : Allows to the class for calling async methods with timeout
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="tskToExecute"></param>
        /// <returns></returns>
        private async Task runAsyncTimeoutMethod(Func<Task> tskToExecute, double timeoutInMilliseconds)
        {            
            await tskToExecute().TimeoutAfter(timeoutInMilliseconds);
        }
        #endregion

        #region callback_implementation

        void IServicioCallback.Mensaje(string message)
        {
            callbackInteraction.Mensaje(message);
        }

        async void IServicioCallback.EnviarAsunto(Entidades.Asunto a)
        {
            try {
                // Duplicates entity for confirmation
                Entidades.Asunto entNewAsunto = new Entidades.Asunto(a);
                callbackInteraction.EnviarAsunto(entNewAsunto);
                await prepareProxy();
                proxy.AsuntoReceiptCompleted(a);
            }
            catch (Exception ex) {
                Except.Throw(ex);
            }            
        }

        async void IServicioCallback.SentAsuntosBatch(Entidades.Asunto[] lstA)
        {
            // Generate a new duplicate list for interface processing
            List<Entidades.Asunto> lstDuplicated = new List<Entidades.Asunto>();
            // Duplicates all elements on received list
            lstA.ToList().ForEach(asunto => lstDuplicated.Add(new Entidades.Asunto(asunto)));
            callbackInteraction.SentAsuntosBatch(lstDuplicated);
            // If the process complete correctly, return response
            await prepareProxy();
            // Sent to Service confirmation with reception of asuntos
            proxy.BatchAsuntoReceiptCompleted(lstA); 
        }

        void IServicioCallback.BatchAsuntoProcessCompleted(Entidades.Asunto[] lstA)
        {
            callbackInteraction.BatchAsuntoProcessCompleted(lstA.ToList());
        }

        void IServicioCallback.AsuntoProcessCompleted(Entidades.Asunto a)
        {
            callbackInteraction.AsuntoProcessCompleted(a);
        }

        void IServicioCallback.UpdateOnAsuntosWithoutAssignation()
        {
            callbackInteraction.UpdateOnAsuntosWithoutAssignation();
        }

        void IServicioCallback.ForceDisconnect()
        {
            callbackInteraction.ForceDisconnect();
        }

        void IServicioCallback.ServiceChangeStatusRequest(AvailabiltyStatus paramNewStatus)
        {
            callbackInteraction.ServiceChangeStatusRequest(paramNewStatus);
        }

        void IServicioCallback.RefreshOperatorStatus()
        {
            callbackInteraction.RefreshOperatorStatus();
        }

        
        void IServicioCallback.NotifyNewAsuntoFromSolucioname()
        {
            callbackInteraction.NotifyNewAsuntoFromSolucioname();
        }

        bool IServicioCallback.IsActive()
        {
            return true;
        }


        #endregion

    }
}
