using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Entidades.Service.Interface;

namespace Servicio_Principal
{

    /// <summary>
    /// Class who controls activity of operators and cleans invalid entrys
    /// </summary>
    public partial class Servicio : IServicio
    {
        /// <summary>
        /// Set up basic parameters for the timer
        /// </summary>
        private void configureOperatorCheckTimer()
        {
            try {
                // Generates a new instance of object timer
                operatorCheckTimer = new System.Timers.Timer();
                // Set up timer parameters
                operatorCheckTimer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings.GetValues("OPERATOR_ACTIVE_CHECK_TIME_INTERVAL"[0]));
                operatorCheckTimer.Elapsed += OperatorCheckTimer_Elapsed;
            }
            catch (Exception ex) {
                // Logs error on console
                Console.WriteLine(GetFullShortDateTime + " - OperatorCheckTimerTask : [Error] " + ex.Message);
            }
        }

        /// <summary>
        /// Starts with the timer
        /// </summary>
        private void startOperatorCheckTimer()
        {
            if (operatorCheckTimer != null) {
                if (!operatorCheckTimer.Enabled) {
                    operatorCheckTimer.Enabled = true;
                    Console.WriteLine(GetFullShortDateTime + " - OperatorCheckTimerTask : [Start] task started normally.");
                }
                // If the timer already active, don't do anything
            }
            else {
                // Show on the console the error on start
                Console.WriteLine(GetFullShortDateTime + " - OperatorCheckTimerTask : [Error] the timer cannot be started, lacks of configuration.");
            }
        }

        private void stopOperatorChecktimer()
        {
            if(operatorCheckTimer != null) {
                if (operatorCheckTimer.Enabled) {
                    operatorCheckTimer.Enabled = true;
                    Console.WriteLine(GetFullShortDateTime + " - OperatorCheckTimerTask : [Stop] the timer has been stopped correctly.");
                }
            }
            else {
                // Show on the console the error on start
                Console.WriteLine(GetFullShortDateTime + " - OperatorCheckTimerTask : [Error] the timer cannot be stopped, lacks of configuration.");
            }
        }

        private void OperatorCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            
        }
        
        /// <summary>
        /// Validates if the operator is connected
        /// </summary>
        /// <param name="pOper"></param>
        private bool isOperatorLoggedIn(Entidades.Operador pOper)
        {
            // TODO : new implementation required. 
            // 1 - Check if in some moment the operator has been logged
            // 2 - Check if the operator connection is live
            // 3 - Return result
            return true;
        }

        private void addOperator(Entidades.Operador paramOperator)
        {

        }

        /// <summary>
        /// Remove operator from the service
        /// </summary>
        /// <param name="paramOperator">Operator to remove from the service</param>
        private void removeOperator(Entidades.Operador paramOperator)
        {

        }

        /// <summary>
        /// Remove operator from the service
        /// </summary>
        /// <param name="callbackFromClient">Callback of the operator to disconnect</param>
        private void removeOperator(Entidades.Service.Interface.IServicioCallback callbackFromClient)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="iscOperatorRelated"></param>
        /// <returns></returns>
        internal Entidades.Operador getOperator(IServicioCallback iscOperatorRelated)
        {
            return lstConnectedClients.First((client) => client.Callback == iscOperatorRelated).Operator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramOperator"></param>
        /// <returns></returns>
        internal IServicioCallback getCallback(Entidades.Operador paramOperator)
        {
            try {
                return lstConnectedClients.First((client) => client.Operator.UserName == paramOperator.UserName).Callback;
                // TODO : Need to check if the operator is already connected with short timeout
            }
            catch (Exception) {
                throw new Exception(string.Format(Error.CALLBACK_RELATED_WITH_OPERATOR_NOTFOUND, paramOperator.UserName));
            }
        }

        /// <summary>
        /// Checks if the callback of the client related is active
        /// </summary>
        /// <returns>true if is active, false</returns>
        private async Task<bool> isCallbackActive(IServicioCallback paramCallbackToCheck)
        {
            try {
                return await Task.Run(() => { return paramCallbackToCheck.IsActive(); });
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
