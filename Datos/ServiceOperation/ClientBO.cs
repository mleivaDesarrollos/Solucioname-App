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
    public sealed class ClientBO
    {
        public static ClientBO Instance{ get { return lazy.Value; }}
        /// <summary>
        /// Propiedad que maneja el estado de conexión con el servicio
        /// </summary>
        private ServicioClient _proxy = null;

        private ServicioClient proxy
        {
            get
            {
                if(_proxy == null)
                {
                    // Generamos un contexto nuevo que permitirá generar una nueva conexión
                    InstanceContext context = new InstanceContext(new SrvClient());
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
        
        private static readonly Lazy<ClientBO> lazy = new Lazy<ClientBO>(() => new ClientBO());

        private ClientBO() { }
        
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

        private class SrvClient : IServicioCallback
        {
            public void EnviarAsunto(Entidades.Asunto a)
            {
                
            }

            public void ForceDisconnect()
            {
                
            }

            public void Mensaje(Mensaje m)
            {
                
            }
        }

        /// <summary>
        /// Conexión hacia el servicio
        /// </summary>
        /// <returns></returns>
        public async Task<Entidades.Operador> Connect(Entidades.Operador pOperator)
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
    }
}
