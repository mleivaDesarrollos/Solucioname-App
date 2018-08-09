using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.SrvSolucioname;
using Entidades;
using System.ServiceModel;

namespace Datos.ServiceOperation
{
    /// <summary>
    /// Implementación de cliente de servicio Backoffice
    /// </summary>
    public sealed class Client : IServicioCallback
    {
        #region singleton_interface_implementation
        public static Client Instance{ get { return lazy.Value; }}
        
        private static readonly Lazy<Client> lazy = new Lazy<Client>(() => new Client());

        private Client() { }
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
                    await Task.Run(() => { try { proxy.Open(); } catch (Exception){ } } );
                    break;
                case CommunicationState.Opening:
                    break;
                case CommunicationState.Opened:
                    break;
                case CommunicationState.Closing:
                    break;
                case CommunicationState.Closed:
                    proxy = null;
                    await Task.Run(() => { try { proxy.Open(); } catch (Exception) { } });
                    break;
                case CommunicationState.Faulted:
                    proxy = null;
                    await Task.Run(() => { try { proxy.Open(); } catch (Exception) { } });
                    break;
                default:
                    break;
            }
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
        #endregion

        #region helper_methods
        /// <summary>
        /// Conexión hacia el servicio
        /// </summary>
        /// <returns></returns>
        public async Task<Entidades.Operador> ConnectBackoffice(Entidades.Operador pOperator, Entidades.Service.Interface.IServicioCallback paramCallback)
        {
            // Manejamos el proxy para que este disponible para la conexión
            await HandleProxy();
            // Comprobamos el estado sea optimo para procesar una solicitud
            if (proxy.State == CommunicationState.Opened)
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
            try
            {
                // Awaits proxy proxy signal
                await HandleProxy();
                // Checks if the proxy is ready for operation
                if (proxy.State == CommunicationState.Opened)
                {
                    // When connection is established we need check if the operator credentials are valid
                    if (proxy.Conectar(paramOperator))
                    {
                        return true;
                    }
                    throw new Exception("Las credenciales locales no se condicen con las credenciales de servicio. Consulte al administrador.");
                }
                else
                {
                    throw new Exception("Ha ocurrido un error en la conexión con el servicio");
                }
            }
            catch (Exception ex)
            {
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
                // Set up proxy to proper condition for disconnection
                await HandleProxy();
                // Checks if the proxy is ready to execute
                if (proxy.State == CommunicationState.Opened) {
                    // Run disconnection on the service
                    proxy.Disconnect(paramOperatorToDisconnect);
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
                // Checks and set the proxy status
                await HandleProxy();
                // Checks if the proxy is ready to get information
                if (proxy.State == CommunicationState.Opened)
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
