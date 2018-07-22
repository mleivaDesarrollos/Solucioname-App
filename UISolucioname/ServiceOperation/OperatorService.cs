using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UISolucioname.SrvSolucioname;
using System.ServiceModel;
using Entidades;

namespace UISolucioname.ServiceOperation
{
    public class OperatorService : IServicioCallback
    {
        ServicioClient _proxy;

        frmMainFrame _frmOperatorWindow;

        /// <summary>
        /// Para poder inicializar el servicio de operador debe pasarse por parametro la ventana principal
        /// De esta forma tenemos acceso a las operaciones de la ventana que nos permitan comunicar 
        /// informacion proveniente del servicio
        /// </summary>
        /// <param name="frmOperator"></param>
        public OperatorService(frmMainFrame frmOperator)
        {
            // Almacenamos el parametro conseguido en una variable local
            _frmOperatorWindow = frmOperator;
        }

        private ServicioClient proxy
        {
            get
            {
                if(_proxy == null)
                {
                    // Generamos una contexto que se comunicara con el servicio
                    InstanceContext contexto = new InstanceContext(this);
                    // Iniciamos la nueva instancia del servicio utilizando el contexto generado
                    _proxy = new ServicioClient(contexto);
                }
                return _proxy;
            }
        }

        public void EnviarAsunto(Entidades.Asunto a)
        {
            Util.MsgBox.Error("Asunto recibido: " + a.Numero);
        }

        public void Mensaje(Mensaje m)
        {
            Util.MsgBox.Error("El servicio ha enviado el siguiente mensaje: " + m.Contenido);
        }

        /// <summary>
        /// Realiza la apertura de conexión del proxy haciendo las comprobaciones correspondientes
        /// </summary>
        private void AbrirConexion()
        {
            if(proxy.State != CommunicationState.Opened)
            {
                // Realizamos apertura
                proxy.Open();
            }
        }

        /// <summary>
        /// Gestiona las conexiones de servicio correspondientes
        /// </summary>
        public void ConectarConServicio()
        {
            // Abrimos la conexión
            AbrirConexion();
            // Ejecutamos la solicitud de conexion pasando los parametros requeridos por el servicio SOAP
            if (proxy.Conectar(App.Current.Properties["user"] as Entidades.Operador))
            {
                Util.MsgBox.Error("Conexion con servicio establecida.");
            }
            else
            {
                Util.MsgBox.Error("Conexion Rechazada.");
            }
        }
    }
}
