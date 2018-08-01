using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleHostTesting.ServicioSolucioname;
using System.ServiceModel;

namespace ConsoleHostTesting
{
    public class InteraccionServicio : IServicioCallback
    {
        ServicioClient _proxy;

        private ServicioClient Proxy
        {
            get
            {
                if (_proxy == null)
                {
                    // Generamos un contexto para el proxy
                    InstanceContext contexto = new InstanceContext(this);
                    // Instanciamos un nuevo contexto de servicio
                    _proxy = new ServicioClient(contexto);
                }
                return _proxy;
            }
            set
            {
                _proxy = value;
            }   
        }

        /// <summary>
        /// Implementación de la interface de callback del servicio. 
        /// Procesa un mensaje enviado desde el servicio de consola
        /// </summary>
        /// <param name="m"></param>
        public void Mensaje(Mensaje m)
        {
            Console.WriteLine(m.Contenido);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Conectarse()
        {
            // Gestionamos el Proxy y reintentamos la conexión
            ManejarProxy();
            // Generamos una solicitud de conexión y mostramos por pantalla el mensaje de conexión exitosa
            if (Proxy.Conectar(Program.consoleAdm))
            {
                Console.WriteLine("Conexión Exitosa. Actualmente estas interactuando con el servicio.");
                return true;
            }
            else
            {
                Console.WriteLine("Se ha producido una falla al intentar conectarse. ");
                return false;
            }            
        }

        public void EnviarAsunto(Entidades.Asunto a)
        {
            Console.WriteLine("Un asunto ha sido enviado para su gestion : " + a.Numero + ". Descripción breve: " + a.DescripcionBreve);
        }
       

        /// <summary>
        /// Método que procesa el comando de envio de mensajes hacia el servicio
        /// Como la metodología de envío SOAP no reconoce correctamente clases que contengan interfaces y herencia
        /// El comando se envía nativamente como el usuario lo escribe consola
        /// </summary>
        /// <param name="sCommandToSend"></param>
        public void EnviarComando(string sCommandToSend)
        {
            // Controlamos que el proxy se encuentre en un estado optimo para lanzar solicitudes
            ManejarProxy();
            // Enviamos el comando al servidor
            Proxy.EjecutarComando(Program.consoleAdm, sCommandToSend);
            // Enviamos una confirmación de ejecución exitosa
            Console.WriteLine("Command Sent.");
        }

        private void ManejarProxy()
        {
            switch (Proxy.State)
            {
                case CommunicationState.Created:
                    Proxy.Open();
                    break;
                case CommunicationState.Opening:
                    break;
                case CommunicationState.Opened:
                    break;
                case CommunicationState.Closing:
                case CommunicationState.Closed:
                case CommunicationState.Faulted:
                    Proxy = null;
                    Proxy.Open();
                    break;
                default:
                    break;
            }
        }
        
    }
}
