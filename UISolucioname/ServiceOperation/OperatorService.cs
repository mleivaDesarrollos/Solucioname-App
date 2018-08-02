﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UISolucioname.SrvSolucioname;
using System.ServiceModel;
using Entidades;
using System.Windows.Threading;

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
            set
            {
                _proxy = value;
            }
        }

        /// <summary>
        /// Procedimiento disponible para que el servicio opere sobre el operador cliente
        /// </summary>
        /// <param name="a"></param>
        public void EnviarAsunto(Entidades.Asunto a)
        {
            try
            {
                // Generamos un objeto de logica para procesar el agregado de los asuntos
                Logica.Asunto logAsunto = new Logica.Asunto();
                // Gestionamos una variable operador disponible para el método
                Operador operLogged = App.Current.Properties["user"] as Operador;
                // Si el operador al que era destino de este asunto no es el logueado, se lanza una excepción
                if (a.Oper.UserName != operLogged.UserName)
                    throw new Exception("Se ha recibido un asunto de un operador erroneo. Informar al administrador. Asunto: " + a.Numero + ". Operador: " + a.Oper.UserName);
                // TODO : A modo de prueba inicial, el primer estado lo generamos en la capa de presentación. Esto debería ser generado en el servicio, para mantener fidelidad con el horario de entrada del asunto en bandeja
                if (a.Estados == null)
                    a.Estados = new List<Estado>()
                    {
                        new Estado()
                        {
                            Ord = 1,
                            Detalle = "Nuevo asunto asignado",
                            FechaHora = DateTime.Now,
                            Tipo = Logica.TipoEstado.TraerEstadoAsuntoInicialNormal()
                        }
                    };
                // Consultamos si el asunto en cuestion existe en la base de datos del operador
                if (!logAsunto.ExisteAsunto(a))
                {
                    // Si no existe, se agrega a la base de datos
                    logAsunto.Agregar(a);
                }
                // Enviamos la confirmación al servidor de que el asunto fue procesado de manera correcta y agregado a la base de datos
                // Como SOAP no puede procesar listas, para no colgar el servicio nullificamos el listado de estados
                a.Estados = null;
                proxy.AsuntoReceiptCompleted(a);
                // Actualizamos la capa de presentación y los casos diarios
                _frmOperatorWindow.NuevoAsuntoDesdeServicio(a);
            }
            catch (Exception ex)
            {
                // Se informa a la capa de presentación del error ocurrido
                Util.MsgBox.Error("Se intentó recibir un asunto desde servicio pero ha ocurrido un error: " + ex.Message); 
            }
        }

        public void Mensaje(Mensaje m)
        {
            // Utilizamos la interfaz que nos brinda la capa de presentación para recibir mensajes
            _frmOperatorWindow.NewMessageFromService(m.Contenido);
        }

        /// <summary>
        /// Realiza la apertura de conexión del proxy haciendo las comprobaciones correspondientes
        /// </summary>
        async private Task HandleProxy()
        {
            switch (proxy.State)
            {
                case CommunicationState.Created:
                    await Task.Run(() =>{ try { proxy.Open(); } catch (Exception){ }});
                    break;
                case CommunicationState.Opening:
                case CommunicationState.Opened:
                    break;
                case CommunicationState.Closing:
                case CommunicationState.Closed:
                    proxy = null;
                    break;
                case CommunicationState.Faulted:
                    proxy = null;            
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gestiona las conexiones de servicio correspondientes
        /// </summary>
        async public void ConnectService()
        {
            try
            {
                // Abrimos la conexión
                await HandleProxy();
                // Comprobamos si la conexión se encuentra abierta
                if (proxy.State == CommunicationState.Opened)
                {
                    // Ejecutamos la solicitud de conexion pasando los parametros requeridos por el servicio SOAP
                    if (proxy.Conectar(App.Current.Properties["user"] as Entidades.Operador))
                    {
                        _frmOperatorWindow.setConnectionStatus(CommunicationState.Opened);
                    }
                    else
                    {
                        Util.MsgBox.Error("Conexion Rechazada. Claves de usuario no reconocidas.");
                        _frmOperatorWindow.setConnectionStatus(CommunicationState.Faulted);
                    }
                }
                else
                {
                    // Configuramos el proxy como nulo antes de proceder
                    _proxy = null;
                    // Comunicamos la falla en la capa de presentación
                    _frmOperatorWindow.setConnectionStatus(CommunicationState.Faulted);
                }
            }
            catch (Exception ex)
            {
                Util.MsgBox.Error(ex.Message);
            }
        }

        /// <summary>
        /// Desconecta el proxy
        /// </summary>
        public async void DisconnectService()
        {
            await Task.Run(() =>
            {
                try
                {
                    if (proxy.State == CommunicationState.Opened)
                    {
                        proxy.Disconnect(App.Current.Properties["user"] as Operador);
                        proxy.Close();
                        proxy = null;
                    }                    
                }
                catch (Exception){ }
            });
            _frmOperatorWindow.setConnectionStatus(CommunicationState.Closed); 
        }
        
        public void ForceDisconnect()
        {
            DisconnectService();
        }
    }
}
