using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using Entidades.Service.Interface;
using Entidades;
using System.Collections.Specialized;

namespace Servicio_Principal
{
    public partial class Servicio : IServicio
    {
        #region property_of_classes
        /// <summary>
        /// Saves on local variable interval for checking operator live
        /// </summary>
        double _dblOperatorCheckInterval = Convert.ToDouble(ConfigurationManager.AppSettings.GetValues("OPERATOR_ACTIVE_CHECK_TIME_INTERVAL")[0]);

        /// <summary>
        /// Local Variable getted from Configuration File. Timeout for await response from operator
        /// </summary>
        double _dblOperatorTimeoutAwaitResponse = Convert.ToDouble(ConfigurationManager.AppSettings.GetValues("OPERATOR_CHECK_TIMEOUT_AWAIT_RESPONSE")[0]);

        /// <summary>
        /// Portion of code name for loggin purpose
        /// </summary>
        private static readonly string _operatorClassName = "OperatorAdminTask";
        #endregion

        #region configure_operator_check_timer
        /// <summary>
        /// Configure operator check timer
        /// </summary>
        private void configureOperatorCheckTimer()
        {
            try {
                // Instance new timer object
                operatorCheckTimer = new Timer();
                // Parametrizes the timer
                operatorCheckTimer.Elapsed += OperatorCheckTimer_Elapsed;
                operatorCheckTimer.Interval = _dblOperatorCheckInterval;
                // Link a event to list for starting and stop timer
                lstOperatorMustConnected.CollectionChanged += LstConnectedClients_CollectionChanged;
            } catch (Exception ex) {
                Log.Error(_operatorClassName, ex.Message);
            }
        }

        /// <summary>
        /// Start operator check timer
        /// </summary>
        private void startOperatorCheckTimer(bool log = true)
        {
            // if the timer isn't configured, throw error
            if (operatorCheckTimer != null && lstOperatorMustConnected.Count != 0) {
                if (!operatorCheckTimer.Enabled) {
                    operatorCheckTimer.Enabled = true;
                    if (log) Log.Info(_operatorClassName, "started normally.");
                }
                    
                // If the timer isn't started, don't do anything
            } else {
                if (log) Log.Error(_operatorClassName, "timer isn't configured for start.");
            }
        }

        private void stopOperatorCheckTimer(bool log = true)
        {
            // if the timer isn't configured, throw error
            if (operatorCheckTimer != null) {
                if (operatorCheckTimer.Enabled) {
                    operatorCheckTimer.Enabled = false;
                    if(log) Log.Info(_operatorClassName, "stopped normally.");
                }
                // if the timer is already stopped, don't do anything
            } else {
                if (log) Log.Error(_operatorClassName, "timer isn't configured for start.");
            }
        }
        /// <summary>
        /// Periodically checks connected operators
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OperatorCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try {
                stopOperatorCheckTimer(false);
                // Iterates over al connected client list
                foreach (var client in lstOperatorMustConnected.Where((clientFind) => clientFind.Callback != null)) 
                    {
                    // if operator activity is false
                    if (!(await checkOperatorActivity(client.Operator))) {
                        // Remove client
                        lstOperatorToRemove.Add(client);
                        // Shows information on log service
                        Log.Info(_operatorClassName, string.Format("client {0} has been removed due inactivity.", client.Operator));
                    }                    
                }
                // All clients saved on lstOperToremove must be removed from the list (iterating foreach on lstConnnected restricts remove directly)
                removeInvalidClients();
                startOperatorCheckTimer(false);
            }
            catch (Exception ex) {
                Log.Error(_operatorClassName, string.Format("error during cleaning lost connected clients : {0}. ", ex.Message));
            }
        }

        /// <summary>
        /// Manages start and stopping service depending count of client connected and status of timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LstConnectedClients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // On add new client
            if(e.Action == NotifyCollectionChangedAction.Add) {
                // If timer isn't started
                if(!operatorCheckTimer.Enabled) {
                    // Start them
                    startOperatorCheckTimer();
                }
            }
            else if(e.Action == NotifyCollectionChangedAction.Remove) {
                // if no more client connected
                if(lstOperatorMustConnected.Count  == 0) {
                    // if timer is started
                    if (operatorCheckTimer.Enabled) {
                        // Stop the timer
                        stopOperatorCheckTimer();
                    }
                }
            } 

        }

        #endregion

        #region operator_administration
        /// <summary>
        /// check if the operator callback is already active
        /// </summary>
        /// <param name="paramOperator"></param>
        /// <returns></returns>
        private async Task<bool> checkOperatorActivity(Operador paramOperator)
        {
            try {
                // Gets callback related to operator
                IServicioCallback callbackRelated = getCallback(paramOperator);
                // Sent a signal to check if the operator callback is live
                bool isAlive = await sentAsyncSignalToCallback(callbackRelated);
                // Sent the result of checking
                return isAlive;
            } catch (Exception ex) {
                Log.Error(_operatorClassName, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets operator callback from the list of connected
        /// </summary>
        /// <param name="paramOperator">operator to find</param>
        /// <returns></returns>
        private IServicioCallback getCallback(Operador paramOperator)
        {
            // Check if the operator have data
            if (paramOperator != null) {
                // Gets a save on variable the callback
                IServicioCallback callbackOfOperator = lstOperatorMustConnected.First((client) => client.Operator.UserName == paramOperator.UserName).Callback;
                // Finds in the client connect list a operator related with a callback
                if (callbackOfOperator != null) {
                    return callbackOfOperator;
                } else {
                    throw new Exception(string.Format("callback related to operator {0} not found.", paramOperator));
                }
            }
            throw new Exception("operator parameter is empty");
        }

        /// <summary>
        /// Get operator related to callback
        /// </summary>
        /// <param name="paramCallback">Callback to find</param>
        /// <returns></returns>
        private Operador getOperator(IServicioCallback paramCallback)
        {
            try {
                // Instance a new operator object
                Operador newOper = lstOperatorMustConnected.First((clientToFind) => clientToFind.Callback == paramCallback).Operator;
                // Checks if the operator is null
                if (newOper != null) {
                    return newOper;
                } else {
                    return null;
                }
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Gets client instance by operator passed on parameter
        /// </summary>
        /// <param name="prmOperator"></param>
        /// <returns>client from operator list</returns>
        private Client getClientByOperator(Operador prmOperator)
        {
            try {
                return lstOperatorMustConnected.First((clientToFind) => clientToFind.Operator.UserName == prmOperator.UserName);
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Checks if operator is logged
        /// </summary>
        /// <param name="paramOperator">operator to check</param>
        /// <returns>true if logged, false if not</returns>
        private bool isOperatorLogged(Operador paramOperator)
        {
            return lstOperatorMustConnected.ToList().Exists((client) => client.Operator.UserName == paramOperator.UserName);
        }
        /// <summary>
        /// Add operator to list of connected clients
        /// </summary>
        /// <param name="paramOperator"></param>
        /// <param name="paramCallback"></param>
        private void addOperator(Operador paramOperator, IServicioCallback paramCallback)
        {
            try {
                // If operator to log is already logged in
                if (isOperatorLogged(paramOperator)) {
                    // Remove from client connected
                    disconnectOperator(paramOperator);
                }
                // Implement a new client object
                Client newClient = new Client() { Operator = paramOperator, Callback = paramCallback };
                // Add client to list of connected clients
                lstOperatorMustConnected.Add(newClient);
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Removes operator from client connected list
        /// </summary>
        /// <param name="paramOperator"></param>
        private void disconnectOperator(Operador paramOperator)
        {
            try {
                // Gets client to remove
                Client clientToRemove = lstOperatorMustConnected.First((client) => client.Operator.UserName == paramOperator.UserName);
                // Remove client from the list
                disconnectClient(clientToRemove);

            } catch (Exception ex) {
                Log.Error(_operatorClassName, string.Format("error removing operator {0} : {1}", paramOperator, ex.Message));
            }
        }

        /// <summary>
        /// Removes operator from client connected list
        /// </summary>
        /// <param name="paramCallback">Callback to remove</param>
        private void removeOperator(IServicioCallback paramCallback)
        {
            try {
                // Get connected client
                Client clientToRemove = lstOperatorMustConnected.First( (client) => client.Callback == paramCallback);
                if (clientToRemove != null) {
                    // Remove client from the list
                    disconnectClient(clientToRemove);
                }
            } catch (Exception ex) {
                Log.Error(_operatorClassName, string.Format("error removing operator from caLLback : {1}", paramCallback, ex.Message));
            }
        }

        /// <summary>
        /// Adds a new client to list of connected
        /// </summary>
        /// <param name="prmClientToAdd"></param>
        private void addClient(Client prmClientToAdd)
        {
            // because is a private method, don't need to check if the client is already added to the list
            // TODO : Adds a new entry to today connected operators persistence
            // Add operator to the list of active
            lstOperatorMustConnected.Add(prmClientToAdd);
        }

        /// <summary>
        /// Removes a client from the service
        /// </summary>
        /// <param name="paramClientToRemove">Client to remove</param>
        private void disconnectClient(Client paramClientToRemove)
        {
            // Locks for the singleton instance
            lock (syncObject) {
                try {
                    // Nullifies callback of client
                    paramClientToRemove.Callback = null;
                    // Sets status of operator to disconnected
                    paramClientToRemove.Operator.Status = AvailabiltyStatus.Disconnected;
                    // If a backoffice operator has been connected, sent a signal for refresh status
                    if (connectedBackoffice != null) SentRefreshForConnectedBackoffice();
                } catch (Exception ex) {
                    Log.Error(_operatorClassName, string.Format("error removing client {0} : {1}", paramClientToRemove.Operator, ex.Message));
                }
            }
        }

        /// <summary>
        /// using list Operator to remove, clean connected list
        /// </summary>
        private void removeInvalidClients()
        {
            foreach (var client in lstOperatorToRemove) {
                // Removes client from the connected list
                disconnectClient(client);                
            }
        }
        #endregion

        #region helper_methods

        /// <summary>
        /// Loads all operator corresponding to work on current week day
        /// </summary>
        private void loadOperatorsMustWorkToday()
        {
            try {
                // Try to fill the list of operator connecteds
                foreach (var operatorToday in SQL.Operador.getOperatorsOfTheDay()) {
                    lstOperatorMustConnected.Add(new Client() { Operator = operatorToday });
                }                
                // Logs the result of load
                Log.Info(_operatorClassName, "list of operators for work today has been loaded.");
            }
            catch (Exception ex) {
                Log.Error(_operatorClassName, "error loading operators who works today : " + ex.Message);
            }
        }

        /// <summary>
        /// Checks a client callback 
        /// </summary>
        /// <param name="prmClientToCheck"></param>
        private void CheckAndUpdateCallbackOperator(Client prmClientToCheck, IServicioCallback prmLastCallback)
        {
            // if the callback is not the same on saved in service, update them
            if (prmClientToCheck.Callback != prmLastCallback) prmClientToCheck.Callback = prmLastCallback;
        }

        /// <summary>
        /// Check backoffice logged in and validates if callback is the same
        /// </summary>
        /// <param name="prmOperatorBackoffice"></param>
        /// <param name="prmCallback"></param>
        private void CheckAndUpdateCallbackBackoffice(Operador prmOperatorBackoffice, IServicioCallback prmCallback)
        {
            // Checks if operator is the same and callback if different
            if (connectedBackoffice.Operator.UserName == prmOperatorBackoffice.UserName && connectedBackoffice.Callback != prmCallback) connectedBackoffice.Callback = prmCallback;
        }

        /// <summary>
        /// Checks if one callback is active
        /// </summary>
        /// <param name="paramCallbackToCheck">Callback to check</param>
        /// <returns></returns>
        private async Task<bool> sentAsyncSignalToCallback(IServicioCallback paramCallbackToCheck)
        {
            try {
                // Checks if the callback is null
                if (paramCallbackToCheck != null) {
                    // Checks on async method with timeout if the operator is alive
                    bool isActive = await (Task.Run<bool>(() =>
                    {
                        try {
                            return paramCallbackToCheck.IsActive();
                        }
                        catch (Exception ex) {
                            Log.Error(_operatorClassName, "details: " + ex.Message);
                            return false;
                        }
                        
                    })).TimeoutAfter(_dblOperatorTimeoutAwaitResponse);
                    return isActive;
                }
            }
            catch (TimeoutException) {
                return false;
            }
            catch (Exception ex) {
                Log.Error(_operatorClassName, "error awaiting response: " + ex.Message);
                return false;
            }
            throw new Exception("callback is null.");
        }
        #endregion
    }
}
