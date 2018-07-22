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
        }

        /// <summary>
        /// Implementación de la interface de callback del servicio. 
        /// Procesa un mensaje enviado desde el servicio de consola
        /// </summary>
        /// <param name="m"></param>
        public void Mensaje(Mensaje m)
        {
            Console.WriteLine("El servicio ha emitido un mensaje : " + m.Contenido);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Conectarse()
        {
            // Si no esta abierto el Proxy, se genera una apertura de conexión
            if (Proxy.State != CommunicationState.Opened)
            {
                Proxy.Open();
            }
            // Generamos une nueva entidad operador
            Operador oper = new Operador()
            {
                UserName = "Mleiva",
                Nombre = "Maximiliano",
                Password = "Europa07",
                Apellido = "Leiva"
            };
            // Generamos una solicitud de conexión y mostramos por pantalla el mensaje de conexión exitosa
            if (Proxy.Conectar(oper))
            {
                Console.WriteLine("Conexión Exitosa. Actualmente estas interactuando con el servicio.");
            }
            else
            {
                Console.WriteLine("Se ha producido una falla al intentar conectarse. ");
            }
            Proxy.Close();
        }

        public void EnviarAsunto(Asunto a)
        {
            Console.WriteLine("Un asunto ha sido enviado para su gestion : " + a.Numero + ". Descripción breve: " + a.DescripcionBreve);
        }
    }
}
