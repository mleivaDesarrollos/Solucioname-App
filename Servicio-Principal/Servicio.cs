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
using System.ServiceModel.Channels;
using Entidades.Service.Interface;

namespace Servicio_Principal
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class Servicio : IServicio
    {

        /// <summary>
        /// Listado de operadores conectados con sus correspondientes nombres de usuario
        /// </summary>
        // Dictionary<Operador, IServicioCallback> lstOperadoresConectados = new Dictionary<Operador, IServicioCallback>();
        // ObservableCollection<Client> lstConnectedClients = new ObservableCollection<Client>();

        /// <summary>
        /// List of operator must connected today
        /// </summary>
        ObservableCollection<Client> lstOperatorMustConnected = new ObservableCollection<Client>();

        /// <summary>
        /// Lista que se utilizará para procesos de limpieza de operadores que ya perdieron la conexión al servidor
        /// </summary>
        List<Client> lstOperatorToRemove = new List<Client>();

        // Almacenamos el callback del administrador de consola en una variable separada
        internal IServicioCallback ConsoleAdminCallback = null;

        /// <summary>
        /// Dispose a backoffice user for connections. By operation choice, only one backoffice can be connected
        /// </summary>
        internal Client connectedBackoffice = null;

        /// <summary>
        /// Listado de asuntos pendientes de entrega a operadores
        /// </summary>
        internal ObservableCollection<Entidades.Asunto> lstAsuntosToDeliver = new ObservableCollection<Entidades.Asunto>();

        /// <summary>
        /// Temporizador asignado para la distribución de asuntos pendientes
        /// </summary>
        System.Timers.Timer deliverAsuntosPendingTimer;

        System.Timers.Timer operatorCheckTimer;

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
        public IServicioCallback CurrentCallback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IServicioCallback>();
            }
        }

        // Método que obtiene el número de IP del cliente conectado
        public string GetCallbackHostname
        {
            get
            {
                // Extraemos el contexto
                var context = OperationContext.Current;
                // Obtenemos la propiedad que tiene almacenada esta información
                RemoteEndpointMessageProperty property = (RemoteEndpointMessageProperty)context.IncomingMessageProperties[RemoteEndpointMessageProperty.Name];
                // Devolvemos el valor de la IP
                return property.Address;
            }
        }

        public string GetFullShortDateTime
        {
            get
            {
                return DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
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
            // Configure operator service check
            configureOperatorCheckTimer();
            // Load operator to must be connected today
            loadOperatorsMustWorkToday();    
        }
        #endregion

        #region contract_implementation
        
        /// <summary>
        /// Process a connection request to the service
        /// </summary>
        /// <param name="oper"></param>
        /// <returns></returns>
        public bool Conectar(Operador oper)
        {
            // guarantees exclusive petition
            lock (syncObject) {
                // Checks if the operator is correct
                if (oper == null) return false;
                if (isConsoleAdmin(oper)) {
                    // If is a console admin the connection has suceess and saves console admin callback
                    ConsoleAdminCallback = CurrentCallback;
                    return true;
                }
                // Checks if the operator credentials are valid
                SQL.Operador connOperator = new SQL.Operador();
                // if the connection is valid
                if (connOperator.ValidarIngreso(oper)) {
                    // search on list of working operators for username
                    Client operatorClient = lstOperatorMustConnected.First((operConn) => operConn.Operator.UserName == oper.UserName);
                    // if the username is on the list 
                    if(operatorClient != null) {
                        // save current callback with this value
                        operatorClient.Callback = CurrentCallback;
                    }
                    // if the username is not on list, this means the operator is not programmed to log in today. 
                    else {                        
                        // Creates new client
                        operatorClient = new Client() { Callback = CurrentCallback, Operator = oper };                        
                        // Add new operator to the list.                       
                        addClient(operatorClient);
                    }
                    // Set status for new client connected to readytoreceive.
                    operatorClient.Operator.Status = AvailabiltyStatus.ReadyToReceive;
                    // validate successfull the login
                    return true;
                }
                // if the operators credentials are invalid return false
                return false;
            }
        }

        public void AsuntoReceiptCompleted(Asunto asuntoToConfirm)
        {
            // Como la tarea esta a cargo de la clase AsuntosPendingDeliverTask
            // La ejecución y preparación del procedimiento queda a cargo de esa clase
            ConfirmAsuntoReceipt(asuntoToConfirm);
        }

        /// <summary>
        /// Process a request for disconnect from the service
        /// </summary>
        /// <param name="oper"></param>
        public void Disconnect(Operador oper)
        {
            // Validates input
            if (oper == null || !isOperatorLogged(oper)) return;
            // Process request of desconnectio
            disconnectOperator(oper);
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
                Log.Info("Command", string.Format("{0} executed succefully.", commandBuild.Name));
            }
            catch (Exception ex)
            {
                // Al procesarse una exception se informa por consola el resultado                
                Log.Error("Command", string.Format("details : " + ex.Message));
            }
        }

        /// <summary>
        /// Operacion que gestiona un acceso en backoffice
        /// </summary>
        /// <param name="oper"></param>
        /// <returns>Nulo si el operador de backoffice no fue encontrado</returns>
        public Operador ConnectBackoffice(Operador oper)
        {
            try
            {
                // Generate a new SQL Operator object
                SQL.Operador sqlOperator = new SQL.Operador();
                // if a backoffice are already connected, kicks current backoffice
                if (connectedBackoffice != null) sentDisconnectToCurrentBackoffice(connectedBackoffice, oper);
                // Set a new backoffice connected on the service
                connectedBackoffice = new Client()
                {
                    Callback = CurrentCallback,
                    Operator = oper
                };
                // Return the value processed on the login method
                return sqlOperator.ValidateBackofficeOperator(oper);
            }
            catch (Exception ex)
            {
                // If the process fails, notifies on console the error.
                Log.Error("MainService", "details : " + ex.Message);
                // Send a client a null operator
                return null;
            }
        }

        /// <summary>
        /// Retrieve a operator list from the service
        /// </summary>
        /// <returns>List of operator</returns>
        public List<Entidades.Operador> getOperatorList()
        {
            try
            {
                Console.WriteLine(GetFullShortDateTime + ": sending list of connected operators.");
                // Return full list of connected operators
                return lstOperatorMustConnected.Select((operConn) => operConn.Operator).ToList();
            }
            catch (Exception ex)
            {
                // Log error on console screen
                Log.Error("MainService", "details : " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Returns a list with total operator must working today
        /// </summary>
        /// <returns></returns>
        public List<Operador> getListOfOperatorMustWorkToday()
        {
            return lstOperatorMustConnected.Select((operators) => operators.Operator).ToList();
        }

        /// <summary>
        /// Set a new status from current Callback. On successfull change, service sent a signal to the client
        /// </summary>
        /// <param name="paramNewStatus"></param>
        public void SetStatus(Operador operatorToChange, AvailabiltyStatus paramNewStatus)
        {
            lock (syncObject) {
                try {
                    // gets client from the service
                    Client clientToChange = getClientByOperator(operatorToChange);                    
                    // Set the status on operator finded
                    clientToChange.Operator.Status = paramNewStatus;
                    // Sent a confirmation signal to the client
                    CurrentCallback.ServiceChangeStatusRequest(paramNewStatus);
                    // if the callback related to operator has change, update client
                    CheckAndUpdateCallback(clientToChange, CurrentCallback);
                    // For debug purposses, sent a console message to inform
                    Log.Info(_operatorClassName, string.Format("{0} has changed status to {1}.", operatorToChange, paramNewStatus));
                } catch (Exception ex) {
                    Log.Error("Service", "error setting status: " + ex.Message);
                    // If the callback is no related with any operator, forces disconnection
                    CurrentCallback.Mensaje("There is an error getting operator, please contact the administrator. The service connection has been ended.");
                    // Force the client's desconnection from de the service
                    CurrentCallback.ForceDisconnect();
                }
            }
        }

        /// <summary>
        /// Method for fast check if the service is active
        /// </summary>
        public bool IsServiceActive()
        {
            // This is like ping method, only awaits a true return
            return true;
        }
        #endregion

        #region helper_methods
        private async Task sentDisconnectToCurrentBackoffice(Client prevClient, Entidades.Operador paramNewBackoffice)
        {
            Log.Info("MainService", string.Format("backoffice {0} has been forcedly disconnected by {1}", connectedBackoffice.Operator, paramNewBackoffice.UserName));
            await Task.Run(() =>
            {
                // Sent to previous backoffice a message and force disconnection
                prevClient.Callback.Mensaje(string.Format("the operator {0} has been connected to the service. Your connection has been forcedly terminated.", paramNewBackoffice.UserName));
                prevClient.Callback.ForceDisconnect();
            }).TimeoutAfter(5000);
            
        }
        #endregion

        #region command_helpers
        public void TestCommand()
        {
            // Recorremos todos los clientes conectados y le mandamos un mensaje
            foreach (var callback in lstOperatorMustConnected.Select( (client) => client.Callback).ToList())
            {
                callback.Mensaje("Comando prueba desde consola." );
            }
        }

        /// <summary>
        /// Envia un mensaje a todos los operadores logueados
        /// </summary>
        public void MessageToAllOperators(string sMessage)
        {
            // Recorremos todos los clientes conectados
            foreach (var client in lstOperatorMustConnected)
            {
                try
                {
                    // Controlamos con un bloque Try Catch el envío de mensajes, por si el callback ya no responde y debe ser removido del listado
                    client.Callback.Mensaje(sMessage);                    
                }
                catch (Exception)
                {
                    Log.Error("MainService", string.Format("{0} not responding to interaction.", client.Operator));
                }                
            }            
        }
        
        /// <summary>
        /// Devuelve un listado con nombres de usuarios conectados al servicio actualmente
        /// </summary>
        /// <returns></returns>
        internal void retreiveListOfConnectedOperators()
        {
            // Devolvemos el listado ya procesado
            ConsoleAdminCallback.Mensaje(getListConnectedOperator());
        }

        private string getListConnectedOperator()
        {
            // Preparamos el listado para procesar
            string lstOperatorConnected = "";
            // Agregamos un mensaje inicial
            lstOperatorConnected += @"List of operators connected to service:\n";
            // Recorremos el listado de operadores conectados
            foreach (var username in lstOperatorMustConnected.Select((client) => client.Operator.UserName))
            {
                lstOperatorConnected += username + " ";
            }
            return lstOperatorConnected;
        }

        /// <summary>
        /// Procesa una desconexión forzada del servicio de un cliente especifico
        /// </summary>
        /// <param name="operToDisconnect"></param>
        public void ForceToDisconnectFromService(Entidades.Operador operToDisconnect)
        {
            // Forzamos la desconexión del cliente mandando una operación de Callback
            getCallback(operToDisconnect).ForceDisconnect();
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
