using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.SrvSolucioname;
using Entidades;
using System.ServiceModel;
using System.Timers;

namespace Datos.ServiceOperation
{
    /// <summary>
    /// Implementación de cliente de servicio
    /// </summary>
    public sealed class Client : IServicioCallback
    {
        #region singleton_interface_implementation
        public static Client Instance{ get { return lazy.Value; }}
        
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

        private ServicioClient proxy
        {
            get
            {
                if (_proxy == null)
                {
                    // Generamos un contexto nuevo que permitirá generar una nueva conexión
                    InstanceContext context = new InstanceContext(this);
                    // Generamos un nuevo servicio de cliente a partir de este contexto
                    _proxy = new ServicioClient(context);
                }
                return _proxy;
            }
            set
            {
                _proxy = null;
            }
        }
                
        /// <summary>
        /// Administración de estados del proxy
        /// </summary>
        private async Task HandleProxy()
        {
            switch (proxy.State)
            {
                case CommunicationState.Created:                    
                    await (Task.Run(() => { OpenProxy(); })).TimeoutAfter(TimeSpan.FromMilliseconds(defaultGeneralTimeout));
                    break;
                case CommunicationState.Opening:
                    break;
                case CommunicationState.Opened:
                    break;
                case CommunicationState.Closing:
                    break;
                case CommunicationState.Closed:
                    proxy = null;
                    await (Task.Run(() => { OpenProxy(); })).TimeoutAfter(TimeSpan.FromMilliseconds(defaultGeneralTimeout));
                    break;
                case CommunicationState.Faulted:
                    proxy = null;
                    await (Task.Run(() => { OpenProxy(); })).TimeoutAfter(TimeSpan.FromMilliseconds(defaultGeneralTimeout));
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

        private async Task<bool> isProxyReady()
        {
            // Checks and set current status of proxy method
            await HandleProxy();
            // if the proxy is opened, the proxy is ready to use
            return proxy.State == CommunicationState.Opened ? true : false;
        }
        #endregion

        #region property_client
        private Entidades.Service.Interface.IServicioCallback _callbackInteraction;

        public Entidades.Service.Interface.IServicioCallback callbackInteraction
        {
            get
            {
                if (_callbackInteraction == null) throw new Exception("Error ClientBO : The callback has not been setted.");
                return _callbackInteraction;
            }
            set
            {
                _callbackInteraction = value;
            }
        }

        Timer _tmrCheckActiveService;

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
            if (await isProxyReady())
            {
                // Almacenamos la respuesta del servicio en una variable que deberia venir completa
                Entidades.Operador backofficeOper = proxy.ConnectBackoffice(pOperator);
                if (backofficeOper != null)
                {
                    // Sets callback for the client
                    callbackInteraction = paramCallback;                    
                    // Devolvemos el operador con todos sus datos cargados
                    return backofficeOper;
                }
                else
                {
                    throw new Exception("El usuario o contraseña no son validos, o no se disponen de permisos de backoffice.");
                }

            }
            throw new Exception("Error conectando al servicio");
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
                if (await isProxyReady()) {
                    // When connection is established we need check if the operator credentials are valid
                    if (proxy.Conectar(paramOperator)) {
                        // when connection has been acepted, the callback is saved to memory
                        callbackInteraction = paramCallback;
                        // Starts service checking activity
                        startCheckStatusOfConnectionWithService();
                        return true;
                    }
                    throw new Exception("Las credenciales locales no se condicen con las credenciales de servicio. Consulte al administrador.");
                }
                else {
                    throw new Exception("Ha ocurrido un error en la conexión con el servicio");
                }
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
            try {
                if (await isProxyReady()) {
                    // Run disconnection on the service
                    proxy.Disconnect(paramOperatorToDisconnect);
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            finally {
                // Whatever is the result of method, the service activity checking may be stopped
                stopCheckStatusOfConnectionWithService();
            }
        }

        /// <summary>
        /// Sent a request to service for changing current status on the service
        /// </summary>
        /// <param name="pOperator"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public async Task ChangeCurrentStatus(Entidades.Operador pOperator, Entidades.AvailabiltyStatus newStatus)
        {
            try {
                // Checks if the proxy is ready
                if(await isProxyReady()) {
                    // Sent to the service petition to change te current status
                    proxy.SetStatus(pOperator, newStatus);
                }
                else {
                    throw new Exception("Error en la conexión con el servicio. La comunicación no está abierta.");
                }
                
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Communicates with the service and get a list of connected operators with full data
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entidades.Operador>> GetFullOperatorList()
        {
            try
            {
                if (await isProxyReady())
                {
                    return proxy.getOperatorList().ToList();
                }
                else
                {
                    throw new Exception("Error on proxy status.");
                }
            }
            catch (Exception ex)
            {
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
                _tmrCheckActiveService.Interval = 10000;                                
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
                await runAsyncTimeoutMethod(proxy.IsServiceActiveAsync, defaultGeneralTimeout);                 
            }
            catch (Exception ex) {
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
            TimeSpan tsTimeoutService = TimeSpan.FromMilliseconds(timeoutInMilliseconds);
            await tskToExecute().TimeoutAfter(tsTimeoutService);            
        }

        /// <summary>
        /// Generic Methods : Allows to the class for calling async methods with timeout
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="tskToExecute"></param>
        /// <returns></returns>
        private async Task runAsyncTimeoutMethod(Func<Task> tskToExecute, double timeoutInMilliseconds)
        {
            TimeSpan tsTimeoutService = TimeSpan.FromMilliseconds(timeoutInMilliseconds);
            await tskToExecute().TimeoutAfter(tsTimeoutService);
        }
        #endregion

        #region callback_implementation

        void IServicioCallback.Mensaje(string message)
        {
            callbackInteraction.Mensaje(message);
        }

        void IServicioCallback.EnviarAsunto(Entidades.Asunto a)
        {
            callbackInteraction.EnviarAsunto(a);
        }

        void IServicioCallback.ForceDisconnect()
        {
            callbackInteraction.ForceDisconnect();
        }

        void IServicioCallback.ServiceChangeStatusRequest(AvailabiltyStatus paramNewStatus)
        {
            callbackInteraction.ServiceChangeStatusRequest(paramNewStatus);
        }
        #endregion

    }
}
