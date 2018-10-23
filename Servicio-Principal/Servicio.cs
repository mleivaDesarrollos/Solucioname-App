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
        //internal ObservableCollection<Entidades.Asunto> lstAsuntosToDeliver = new ObservableCollection<Entidades.Asunto>();
        internal AsuntoDeliverList DeliverAsuntoList;

        /// <summary>
        /// Asunto from service unassigned
        /// </summary>
        internal List<Entidades.Asunto> lstAsuntoFromServiceUnassigned = new List<Entidades.Asunto>();

        /// <summary>
        /// Temporizador asignado para la distribución de asuntos pendientes
        /// </summary>
        System.Timers.Timer tmrDeliverPendingAsuntos;

        System.Timers.Timer tmrOperatorCheck;

        System.Timers.Timer tmrAsuntoServiceCheck;

        private static readonly int INDEX_START = 0;

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
            // Configure asunto service checking task
            ConfigureAsuntoServiceCheckTimer();
            StartAsuntoCheckTimer(true);
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
                // if the connection is valid
                if (SQL.Operador.ValidarIngreso(oper)) {
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
                    // if backoffice is connected to the service, sent a signal to refresh list
                    if (connectedBackoffice != null) SentRefreshForConnectedBackoffice();
                    // validate successfull the login
                    return true;
                }
                // if the operators credentials are invalid return false
                return false;
            }
        }

        public void AsuntoReceiptCompleted(Asunto asuntoToConfirm)
        {
            // Update Callback from client
            CheckAndUpdateCallbackOperator(getClientByOperator(asuntoToConfirm.Oper), CurrentCallback);
            // Confirm asunto receipt
            ConfirmAsuntoReceipt(asuntoToConfirm);
        }
        
        /// <summary>
        /// Confirm Batch Asunto Reception from client 
        /// </summary>
        /// <param name="asuntoToConfirm"></param>
        public void BatchAsuntoReceiptCompleted(List<Entidades.Asunto> lstAsuntoToConfirm)
        {
            // Update callback from client
            CheckAndUpdateCallbackOperator(getClientByOperator(lstAsuntoToConfirm[0].Oper), CurrentCallback);
            // Confirm receipt from list
            ConfirmAsuntoReceipt(lstAsuntoToConfirm);
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
                // if a backoffice are already connected, kicks current backoffice
                if (connectedBackoffice != null) sentDisconnectToCurrentBackoffice(connectedBackoffice, oper);
                // Set a new backoffice connected on the service
                connectedBackoffice = new Client()
                {
                    Callback = CurrentCallback,
                    Operator = oper
                };
                // Return the value processed on the login method
                return SQL.Operador.ValidateBackofficeOperator(oper);
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
        /// Process a request from Backoffice and put delivery of them in a qeue
        /// </summary>
        /// <param name="prmAsunto"></param>
        public void SentAsuntoToOperator(Operador prmOperatorBackoffice, Asunto prmAsunto)
        {
            try {
                // Check if the operator who sents the asunto is a backoffice operator logged
                if (!CheckCallingBackofficeIsAlreadyLogged(prmOperatorBackoffice)) {
                    throw new Exception(Error.BACKOFFICE_SENDER_IS_NOT_CORRECTLY_LOGGED);
                }
                // Validates asunto if correct loaded
                if (SQL.Asunto.Validate(prmAsunto)) {
                    // Adds a asunto to deliver 
                    AddPending(prmAsunto);
                }
                else {
                    // Sent a reject sent message
                    CurrentCallback.Mensaje(string.Format("El asunto {0} para el operador {1} no puede ser agregada a la lista de distribución. Es posible que el operador no sea valido o probablemente el asunto ya esta cargado."));
                }
            }
            catch (Exception ex) {
                Log.Error("SentAsuntoToOperator", ex.Message);
                
            }            
        }

        /// <summary>
        /// Sent a batch of asuntos on wich service controls
        /// </summary>
        /// <param name="prmOperatorBackoffice"></param>
        /// <param name="lstA"></param>
        public void SentBatchOfAsuntosToOperator(Operador prmOperatorBackoffice, List<Entidades.Asunto> lstA)
        {
            // Validates if the petition sender is already logged on system
            if (!CheckCallingBackofficeIsAlreadyLogged(prmOperatorBackoffice))
                throw new Exception(Error.BACKOFFICE_SENDER_IS_NOT_CORRECTLY_LOGGED);
            // Validates if the list is correctly loaded
            if (SQL.Asunto.Validate(lstA)) {
                // Adds the list to pending of distribution
                AddPending(lstA);
                // Sent Callback response confirming add succesful
                CurrentCallback.UpdateOnAsuntosWithoutAssignation();
            } else {
                String conflictedAsuntos = SQL.Asunto.GetDuplicatedConflictedAsuntoNumbers(lstA);
                CurrentCallback.Mensaje(string.Format("No se ha podido enviar los asuntos en lote debido a que existen conflictos con los siguientes asuntos: {0}. Tienen el mismo número y operador", conflictedAsuntos));
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
        /// Get assigned asuntos of current day
        /// </summary>
        /// <returns></returns>
        public List<Asunto> getAssignedAsuntosOfCurrentDay()
        {
            try {
                
                List<Asunto> lstAsuntosAssignedOfDay = SQL.Asunto.GetAsuntosAssignedFromToday();
                foreach (var asuntoWithAssignationAndUndelivered in DeliverAsuntoList.Get.Where(asunto => asunto.SendingDate.Date == DateTime.Now.Date)) {
                    lstAsuntosAssignedOfDay.Add(asuntoWithAssignationAndUndelivered);
                }
                return lstAsuntosAssignedOfDay;
            }
            catch (Exception ex) {
                Log.Error(SQL.Asunto.CLASSNAME, ex.Message);
                return null;
            }
        }

        public List<Asunto> getUnassignedAsuntos()
        {
            lock (syncObject) {
                return lstAsuntoFromServiceUnassigned;
            }
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
        /// Gets Exception Saved on service based on current date and next two monts of exceptions
        /// </summary>
        /// <returns>List of all exception in specified range</returns>
        public List<ExceptionDay> GetRecentExceptionDayList()
        {
            return null;
        }

        /// <summary>
        /// Get exceptions based on a specific range of time
        /// </summary>
        /// <param name="RangeTime">Range of time of the needed exceptions</param>
        /// <returns></returns>
        public List<ExceptionDay> GetHistoricExceptionDayList(WorkTime RangeTime)
        {
            return null;
        }

        /// <summary>
        /// Add exception to service
        /// </summary>
        /// <param name="newException"></param>
        public void AddExceptionDay(ExceptionDay newException)
        {

        }

        /// <summary>
        /// Update Exception saved on service
        /// </summary>
        /// <param name="updatingException"></param>
        public void UpdateException(ExceptionDay updatingException)
        {

        }

        /// <summary>
        /// Remove a exception from service
        /// </summary>
        /// <param name="removingException"></param>
        public void RemoveException(ExceptionDay removingException)
        {

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
                    CheckAndUpdateCallbackOperator(clientToChange, CurrentCallback);
                    // Check if the backoffice is logged in and send refresh status
                    if (connectedBackoffice != null) SentRefreshForConnectedBackoffice();
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
        public bool IsServiceActive(bool isBackoffice, Operador prmOperator)
        {
            // if a operator is checking check and update
            if (!isBackoffice) CheckAndUpdateCallbackOperator(getClientByOperator(prmOperator), CurrentCallback);
            // else is a backoffice, update backoffice
            else CheckAndUpdateCallbackBackoffice(prmOperator, CurrentCallback);            
            // This is like ping method, only awaits a true return
            return true;
        }
        #endregion

        #region helper_methods
        
        /// <summary>
        /// Check operator paseed on parameter to validate is the same of the logged on service
        /// </summary>
        /// <param name="prmOperToCheck"></param>
        /// <returns></returns>
        private bool CheckCallingBackofficeIsAlreadyLogged(Operador prmOperToCheck)
        {
            // validate if the backoffice client is logged
            if (connectedBackoffice == null) return false;
            // check if the username of both are the same
            if (connectedBackoffice.Operator.UserName == prmOperToCheck.UserName) return true;
            // false if not the same
            return false;
        }

        private async void SentRefreshForConnectedBackoffice()
        {
            try {
                await Task.Run(() =>
                {
                    try {
                        connectedBackoffice.Callback.RefreshOperatorStatus();
                    } catch (Exception) {
                        Log.Error("MainService", "error sending refresh petition to backoffice");
                        connectedBackoffice = null;
                    }
                    
                }).TimeoutAfter(Config.BACKOFFICE_TIMEOUT);
            } catch (TimeoutException) {
                Log.Error("MainService", "timeout sending refresh for backoffice");
                connectedBackoffice = null;
            } catch (Exception ex) {
                Log.Error("MainService", "error sending refresh to backoffice: " + ex.Message);
            }

        }

        /// <summary>
        /// Sent confirmation to UI Backoffice of adding a new asunto
        /// </summary>
        /// <param name="prmAsuntoSented"></param>
        private async void SentBalanceRefreshOnBackoffice(Entidades.Asunto prmAsuntoSented)
        {
            // Validate if the backoffice is logged in
            if (connectedBackoffice == null) return;
            try {
                await Task.Run(() =>
                {
                    try {
                        // Using Callback of asunto sent information for backoffice client
                        connectedBackoffice.Callback.AsuntoProcessCompleted(prmAsuntoSented);
                    }
                    catch (Exception ex) {
                        Log.Error("MainService", "error sending confirmation to backoffice: " + ex.Message);
                    }
                }).TimeoutAfter(Config.BACKOFFICE_TIMEOUT);
            }
            catch (TimeoutException) {
                // The current logged backoffice is not responding, removing from the service
                connectedBackoffice = null;
                Log.Info("MainService", "the backoffice is not responding, closing connection...");
            }
            catch (Exception ex) {
                Log.Error("MainService", "there is an error processing callback sent" + ex.Message);
            }
        }
        /// <summary>
        /// Sent confirmation to UI Backoffice of adding a new asunto
        /// </summary>
        /// <param name="prmAsuntoSented"></param>
        private async void SentBalanceRefreshOnBackoffice(List<Entidades.Asunto> prmAsuntoSented)
        {
            // Validate if the backoffice is logged in
            if (connectedBackoffice == null) return;
            try {
                await Task.Run(() =>
                {
                    try {
                        // Using Callback of asunto sent information for backoffice client
                        connectedBackoffice.Callback.BatchAsuntoProcessCompleted(prmAsuntoSented);
                    } catch (Exception ex) {
                        Log.Error("MainService", "error sending confirmation to backoffice: " + ex.Message);
                    }
                }).TimeoutAfter(Config.BACKOFFICE_TIMEOUT);
            } catch (TimeoutException) {
                // The current logged backoffice is not responding, removing from the service
                connectedBackoffice = null;
                Log.Info("MainService", "the backoffice is not responding, closing connection...");
            } catch (Exception ex) {
                Log.Error("MainService", "there is an error processing callback sent" + ex.Message);
            }
        }

        /// <summary>
        /// Sent a signal to operator for refresh list of asuntos without assignation
        /// </summary>
        private async void SentRefreshAsuntoWithoutAssignationSignal()
        {
            // Validate if the backoffice is logged in
            if (connectedBackoffice == null) return;
            try {
                await Task.Run(() =>
                {
                    try {
                        // Using Callback of asunto sent information for backoffice client
                        connectedBackoffice.Callback.UpdateOnAsuntosWithoutAssignation();
                    } catch (Exception ex) {
                        Log.Error("MainService", "error sending confirmation to backoffice: " + ex.Message);
                    }
                }).TimeoutAfter(Config.BACKOFFICE_TIMEOUT);
            } catch (TimeoutException) {
                // The current logged backoffice is not responding, removing from the service
                connectedBackoffice = null;
                Log.Info("MainService", "the backoffice is not responding, closing connection...");
            } catch (Exception ex) {
                Log.Error("MainService", "there is an error processing callback sent" + ex.Message);
            }
        }

        /// <summary>
        /// Sents to backoffice a notification with new asunto on system
        /// </summary>
        private async void SentNewAsuntoFromSolucionameNotificationToBackoffice()
        {
            try {
                await Task.Run(() =>
                {
                    try {
                        // Sent to backoffice a callback notification
                        connectedBackoffice.Callback.NotifyNewAsuntoFromSolucioname();
                    }
                    catch (Exception ex) {
                        Log.Error("MainService", "error sending new asunto from solucioname notification to backoffice: " + ex.Message);                        
                    }
                });
            }
            catch (TimeoutException ex) {
                connectedBackoffice = null;
                Log.Info("MainService", "the backoffice is not responding, closing connection...");
            }
            catch (Exception ex) {
                Log.Error("MainService", "there is an error processing new asunto from solucioname notification: " + ex.Message);
            }
        }

        private async Task sentDisconnectToCurrentBackoffice(Client prevClient, Entidades.Operador paramNewBackoffice)
        {
            Log.Info("MainService", string.Format("backoffice {0} has been forcedly disconnected by {1}", connectedBackoffice.Operator, paramNewBackoffice.UserName));
            await Task.Run(() =>
            {
                // Sent to previous backoffice a message and force disconnection
                prevClient.Callback.Mensaje(string.Format("the operator {0} has been connected to the service. Your connection has been forcedly terminated.", paramNewBackoffice.UserName));
                prevClient.Callback.ForceDisconnect();
            }).TimeoutAfter(Config.BACKOFFICE_TIMEOUT);
            
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
        
        
        private List<Entidades.Operador> getListConnectedOperators()
        {
            return lstOperatorMustConnected.Where(
                operToday => operToday.Operator.Status != AvailabiltyStatus.Disconnected)
                .Select(client => client.Operator).
                ToList();
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
