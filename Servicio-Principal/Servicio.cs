using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Entidades;
using Entidades.Service;
using System.Configuration;
using System.Timers;
using System.Collections.ObjectModel;

namespace Servicio_Principal
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class Servicio : IServicio
    {
        /// <summary>
        /// Listado de operadores conectados con sus correspondientes nombres de usuario
        /// </summary>
        Dictionary<Operador, IServicioCallback> lstOperadoresConectados = new Dictionary<Operador, IServicioCallback>();

        /// <summary>
        /// Lista que se utilizará para procesos de limpieza de operadores que ya perdieron la conexión al servidor
        /// </summary>
        List<IServicioCallback> lstOperatorToRemove = new List<IServicioCallback>();        

        /// <summary>
        /// Listado de asuntos pendientes de entrega a operadores
        /// </summary>
        internal ObservableCollection<Entidades.Asunto> lstAsuntosToDeliver = new ObservableCollection<Entidades.Asunto>();

        /// <summary>
        /// Temporizador asignado para la distribución de asuntos pendientes
        /// </summary>
        System.Timers.Timer deliverAsuntosPendingTimer;

        /// <summary>
        /// Objeto de sincronización, utilizado fundamentalmente para no producir deadlocks en peticiones de usuario
        /// </summary>
        object syncObject = new object();

        Entidades.Operador consoleAdmin = new Operador()
        {
            UserName = "ConsoleAdmin",
            Password = "Fm130414"
        };

        /// <summary>
        /// Obtenemos el callback correspondiente al cliente que esta interactuando con el servicio en este momento
        /// </summary>
        public IServicioCallback CallbackActual
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IServicioCallback>();
            }
        }
        #region constructor_of_service
        /// <summary>
        /// Constructor de servicio, se utilizá para realizar la inicialización de variables importantes
        /// </summary>
        public Servicio()
        {
            // Configuramos el servicio de envío de asuntos y lo activamos
            ConfigSendAsuntosPending();
            StartSendAsuntosPending();            
        }
        #endregion

        #region contract_implementation
        /// <summary>
        /// Procesa una solicitud de conexión al servicio
        /// </summary>
        /// <param name="oper"></param>
        /// <returns></returns>
        public bool Conectar(Operador oper)
        {
            lock (syncObject)
            {
                if (oper.UserName != null)
                {
                    if (isConsoleAdmin(oper))
                    {
                        Console.WriteLine("Se ha logueado un administrador de consola.");                                        
                        return true;
                    }
                    SQL.Operador connOperador = new SQL.Operador();
                    // Validamos operador en bases de servicio
                    if (connOperador.ValidarIngreso(oper))
                    {
                        if(isOperatorLoggedIn(oper))
                        {
                            removeOperator(oper);
                        }
                        // Luego de las comprobaciones de agregado ejecutamos el agregado al servicio
                        addOperator(oper);
                        // Operador con credenciales correctas. Validamos si ya esta cargado
                        Console.WriteLine("Usuario : " + oper.UserName + " ha sido ingresado correctamente.");
                        return true;
                    }
                    return false;
                }
                else
                {
                    // Si no cuenta con nombre de usuario se rechaza la conexión
                    Console.WriteLine("El servidor ha rechazado la conexión debido a que no se ha comunicado el nombre de usuario. ");                   
                    return false;
                }
            }                   
        }



        public void AsuntoReceiptCompleted(Asunto asuntoToConfirm)
        {
            // Como la tarea esta a cargo de la clase AsuntosPendingDeliverTask
            // La ejecución y preparación del procedimiento queda a cargo de esa clase
            ConfirmAsuntoReceipt(asuntoToConfirm);
        }

        /// <summary>
        /// Por cliente de consola, se envian comandos a los clientes conectados
        /// </summary>
        /// <param name="oper"></param>
        /// <param name="sCmd"></param>
        public void EjecutarComando(Operador oper, string strCommand)
        {
            try
            {
                // Se rechaza la ejecución del comando si la solicitud no priviene de un adminstrador de consola
                if (!isConsoleAdmin(oper)) throw new Exception(string.Format(Error.CONSOLE_COMMAND_CALLED_BY_STANDARD_USER, oper.UserName));
                // Construimos el comando a partir de la cadena de caractéres
                Command commandBuild = Command.Get(strCommand);
                // Ejecutamos la accion si es que la misma es encontrada
                CommandExecution.Execution.getRelatedAction(commandBuild).Call(this);
                // Avisamos por consola que el comando ha sido ejecutado correctamente
                Console.WriteLine(commandBuild.Name + " has been executed succefully.");
            }
            catch (Exception ex)
            {
                // Al procesarse una exception se informa por consola el resultado                
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region operator_administration
        /// <summary>
        /// Validamos si el operador ya se encuentra logueado dentro del sistema
        /// </summary>
        /// <param name="pOper"></param>
        private bool isOperatorLoggedIn(Entidades.Operador pOper)
        {
            // Utilizando LINQ pasamos a lista los valores de las keys y averiguamos si existe el nombre de usuario
            // Para evitar errores de tipeo pasamos a lowercase el nombre de usuario de ambos
            return lstOperadoresConectados.Keys.ToList().Exists( oper => oper.UserName.ToLower() == pOper.UserName.ToLower());
        }

        /// <summary>
        /// Remueve un operador del listado de callbacks actuales
        /// </summary>
        /// <param name="pOper"></param>
        private void removeOperator(Entidades.Operador pOper)
        {
            try
            {
                // Se intenta remover el operador del listado
                lstOperadoresConectados.Remove(
                    lstOperadoresConectados.Keys.First( oper => oper.UserName == pOper.UserName)
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al remover el operador: " + ex.Message);
            }
        }

        /// <summary>
        /// Agrega un operador al listado de operadores conectados al servicio
        /// </summary>
        /// <param name="pOper"></param>
        private void addOperator(Entidades.Operador pOper)
        {
            try
            {
                // Agregamos la combinacion de operador y callback al listado del servicio
                lstOperadoresConectados.Add(pOper, CallbackActual);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ha ocurrido un error al agregar el operador al listado de conectados: " + ex.Message);                
            }
        }


        /// <summary>
        /// Remueve un operador del listado de conectados
        /// </summary>
        /// <param name="callbackToRemove"></param>
        private void removeConnectedOperator(IServicioCallback callbackRelated)
        {
            // Obtenemos el operador conectado
            Entidades.Operador connectedOperator = getConnectedOperator(callbackRelated);
            // Removemos el operador de los operadores conectados
            lstOperadoresConectados.Remove(connectedOperator);
            // Informamos que el cliente fue desconectado
            Console.WriteLine("the operator {0} has been removed from the connected operators.", connectedOperator.UserName);
        }

        /// <summary>
        /// Procesa el listado de operadores conectados que tuvieron algún problema de conexión en el proceso
        /// </summary>
        private void cleanOperatorsWithFails()
        {
            // Recorremos el listado de operadores a eliminar
            foreach (var callbackFromOperatorDelete in lstOperatorToRemove)
            {
                // Removemos el operador del listado de conectados
                removeConnectedOperator(callbackFromOperatorDelete);
            }
            // Limpiamos el listado de conectados
            lstOperatorToRemove.Clear();
        }

        /// <summary>
        /// Devuelve un callback relacionado a un operador desde el listado de conectados
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        internal Entidades.Operador getConnectedOperator(IServicioCallback iscOperatorRelated)
        {
            return lstOperadoresConectados.First((oper) => oper.Value == iscOperatorRelated).Key;
        }

        /// <summary>
        /// Obtiene el callback del listado de clientes conectados
        /// </summary>
        /// <param name="operParameter"></param>
        /// <returns>Devuelve el callback relacionado con el operador indicado</returns>
        internal IServicioCallback getOperatorCallback(Entidades.Operador operParameter)
        {
            try
            {
                return lstOperadoresConectados.First((opConnected) => opConnected.Key.UserName == operParameter.UserName).Value;
            }
            catch (Exception)
            {
                throw new Exception(string.Format(Error.CALLBACK_RELATED_WITH_OPERATOR_NOTFOUND, operParameter.UserName));
            }

        }

        #endregion

        #region command_helpers
        public void TestCommand()
        {
            // Recorremos todos los clientes conectados y le mandamos un mensaje
            foreach (var callback in lstOperadoresConectados.Values)
            {
                callback.Mensaje(new Mensaje() { Contenido = "Comando prueba desde consola." });
            }
        }

        /// <summary>
        /// Envia un mensaje a todos los operadores logueados
        /// </summary>
        public void MessageToAllOperators(string sMessage)
        {
            // Recorremos todos los clientes conectados
            foreach (var callback in lstOperadoresConectados.Values)
            {
                try
                {
                    // Controlamos con un bloque Try Catch el envío de mensajes, por si el callback ya no responde y debe ser removido del listado
                    callback.Mensaje(new Mensaje() { Contenido = sMessage });
                }
                catch (Exception)
                {
                    // Si hay una excepción, es posible que el cliente ya no este activo, por lo que se progrmaa para que el mismo sea eliminado posteriormente
                    lstOperatorToRemove.Add(callback);
                }                
            }
            cleanOperatorsWithFails();
        }


        /// <summary>
        /// Valida si es el administrador de consola
        /// </summary>
        /// <param name="oper"></param>
        private bool isConsoleAdmin(Entidades.Operador oper)
        {
            if(consoleAdmin.UserName == oper.UserName && consoleAdmin.Password == oper.Password)
            {
                return true;
            }
            return false;
        }
        #endregion

    }
}
