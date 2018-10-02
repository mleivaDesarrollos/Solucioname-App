using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Servicio_Principal
{
    /// <summary>
    /// AsuntosPendingDeliverTask : Tareas relacionadas a la asignación de casos sin entregar
    /// </summary>
    public partial class Servicio : IServicio
    {
        private string _asuntosPendingClassName = "AsuntosPendingDeliverTask";

        /// <summary>
        /// Configuración del timer de envío de asuntos pendientes
        /// </summary>
        private void ConfigSendAsuntosPending()
        {
            try {
                // Load all asunto
                DeliverAsuntoList = new AsuntoDeliverList(SQL.Asunto.getQueue());
                // Inicializamos el valor del timer
                tmrDeliverPendingAsuntos = new System.Timers.Timer();
                // Obtenemos el intervalo de repetición del timer
                tmrDeliverPendingAsuntos.Interval = Convert.ToDouble(ConfigurationManager.AppSettings.GetValues("DELIVER_PENDING_ASUNTOS_TIME_INTERVAL")[0]);
                // Establecemos la funcion relacionada
                tmrDeliverPendingAsuntos.Elapsed += DeliverPendingAsuntos;
                // Checks if the list is empty
                if (!DeliverAsuntoList.IsEmpty) StartSendAsuntosPending(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Si todos los valores del timer para envío son correctos, se inicia el proceso de envío de asuntos
        /// </summary>
        private void StartSendAsuntosPending(bool reportLog = false)
        {
            // Si el timer cumple con las siguientes condiciones se inicia procedimiento
            if (tmrDeliverPendingAsuntos != null && ! DeliverAsuntoList.IsEmpty )
            {
                // Habilitamos el servicio
                tmrDeliverPendingAsuntos.Enabled = true;
                if (reportLog) Log.Info(_asuntosPendingClassName, "service started normally.");
            }
        }

        /// <summary>
        /// Se deshabilita el timer de envío de asuntos si se cumplen con las condiciones informadas por debajo
        /// </summary>
        private void StopSendAsuntosPending()
        {
            if (tmrDeliverPendingAsuntos != null && DeliverAsuntoList.IsEmpty)
            {
                // Deshabilitamos el servicio temporizador
                tmrDeliverPendingAsuntos.Enabled = false;
            }
        }

        /// <summary>
        /// Distribuye asuntos si cumple con el tiempo y las condiciones definidas
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>        
        private async void DeliverPendingAsuntos(object o, ElapsedEventArgs e)
        {
            StopSendAsuntosPending();
            // Retrieve the list of connected operators
            List<Entidades.Operador> lstConnectedOperators = getListConnectedOperators();
            // Si hay operadores conectados, se procesa
            if(lstConnectedOperators.Count > 0) {
                // Iterates over al connected operators
                foreach (var connectedOperator in lstConnectedOperators) {
                    // Get the list of asuntos from current operator
                    List<Entidades.Asunto> lstAsuntosOfOperator = DeliverAsuntoList.Get.Where(asunto => asunto.Oper.UserName == connectedOperator.UserName).ToList();
                    // Sent asuntos to operator
                    SentAsuntoToOperator(lstAsuntosOfOperator);
                }                
            }            
            StartSendAsuntosPending();               
        }

        /// <summary>
        /// Sent asunto in batch to a cliente
        /// 
        /// </summary>
        /// <param name="lstAsuntoToSent"></param>
        private async void SentAsuntoToOperator(List<Entidades.Asunto> lstAsuntoToSent)
        {
            try {
                await Task.Run(() =>
                {
                    try {
                        if (lstAsuntoToSent.Count == 1) {
                            // Get asunto to sent
                            Entidades.Asunto asuntoToSent = lstAsuntoToSent[0];
                            // Sent callback to operator
                            getCallback(asuntoToSent.Oper).EnviarAsunto(asuntoToSent);
                        } else if (lstAsuntoToSent.Count > 1) {
                            // Get operator from list
                            Entidades.Operador operToSent = lstAsuntoToSent[0].Oper;
                            // Sent a batch with the list of asuntos
                            getCallback(operToSent).SentAsuntosBatch(lstAsuntoToSent);
                        }
                    } catch (Exception ex) {
                        Log.Error("AsuntosPendingDelivery-SentAsunto", ex.Message);                       
                    }
                }).TimeoutAfter(2000);

            } 
            catch (TimeoutException) { }
            catch (Exception ex) {
                Log.Error("AsuntosPendingDelivery-SentAsunto", ex.Message);
            }
        }
        
        
        private bool isAsuntoOwnerConnected(Entidades.Asunto asuntoToQuery)
        {
            return lstOperatorMustConnected
                .ToList()
                .Exists(
                (operIterate) => operIterate.Operator.Status != Entidades.AvailabiltyStatus.Disconnected &&
                operIterate.Operator.UserName == asuntoToQuery.Oper.UserName);
        }

        /// <summary>
        /// Completamos la recepción de asuntos de parte del servicio. Se implementa en clase dedicada a la administración de entrega de asuntos pendientes
        /// </summary>
        /// <param name="asuntoToConfirm"></param>
        private void ConfirmAsuntoReceipt(Entidades.Asunto asuntoToConfirm)
        {
            try
            {
                lock(syncObject)
                {
                    RemovePending(asuntoToConfirm);
                }

            }
            catch (Exception)
            {
                Log.Error(_asuntosPendingClassName, string.Format("error delivering asunto number {0} to {1}.", asuntoToConfirm.Numero, asuntoToConfirm.Oper));
            }
        }

        private void ConfirmAsuntoReceipt(List<Entidades.Asunto> lstOfAsuntoToConfirm)
        {
            try {
                lock (syncObject) {
                    // Remove from asunto pending to deliver list
                    RemovePending(lstOfAsuntoToConfirm);
                }
            } catch (Exception ex) {
                Log.Error("ConfirmingAsuntoReceipt", string.Format("Error processing batch confirmation asuntos for {0} : {1}", lstOfAsuntoToConfirm[INDEX_START].Oper, ex.Message));
            }
        }

        /// <summary>
        /// Add pending asunto to qeue
        /// </summary>
        /// <param name="asuntoToEnqueue"></param>
        private void AddPending(Entidades.Asunto asuntoToEnqueue)
        {
            // Add the asunto to distribution list
            DeliverAsuntoList.Add(asuntoToEnqueue);
            // Add to SQL Qeue
            SQL.Asunto.AddToQueue(asuntoToEnqueue);
            // Check if deliver pending asunto is in the list of asuntos without assignation.
            if (!asuntoToEnqueue.isCreatedByBackoffice) {
                lstAsuntoFromServiceUnassigned.RemoveAll(asunto => asunto.Numero == asuntoToEnqueue.Numero);
            }
            // When adds concludes, start deilvering task
            StartSendAsuntosPending();
        }

        /// <summary>
        /// Add a list of asunto to queue
        /// </summary>
        /// <param name="listAsuntoToEnqueue"></param>
        private void AddPending(List<Entidades.Asunto> listAsuntoToEnqueue)
        {
            // Add asunto to distribution list
            DeliverAsuntoList.Add(listAsuntoToEnqueue);
            // Process add on database first
            SQL.Asunto.AddToQueue(listAsuntoToEnqueue);
            // Iterates over all asuntos gived for remove unassigned asunto
            listAsuntoToEnqueue.ForEach(asuntoQueued =>
            {
                if (!asuntoQueued.isCreatedByBackoffice) {
                    lstAsuntoFromServiceUnassigned.RemoveAll(asuntoWithoutAssign => asuntoQueued.Numero == asuntoWithoutAssign.Numero);
                }
            });        
            // When adds concludes, start deilvering task
            StartSendAsuntosPending();
        }

        /// <summary>
        /// Remove asunto from Queue
        /// </summary>
        /// <param name="asuntoToUnqueue"></param>
        private void RemovePending(Entidades.Asunto asuntoToUnqueue)
        {
            // Save date of asunto assignment
            asuntoToUnqueue.AssignmentDate = DateTime.Now;
            // Removemos el asunto de la base de respaldo
            SQL.Asunto.RemoveFromQueueAndSaveHistoricData(asuntoToUnqueue);
            // Sent update request to logged backoffice
            SentBalanceRefreshOnBackoffice(asuntoToUnqueue);
            // Save pending information to list
            DeliverAsuntoList.Remove(asuntoToUnqueue);
            // Stop sending pending asuntos
            StopSendAsuntosPending();
        }

        /// <summary>
        /// Remove list of asunto from queue
        /// </summary>
        /// <param name="listAsuntoToUnqueue"></param>
        private void RemovePending(List<Entidades.Asunto> listAsuntoToUnqueue)
        {
            // Set up the assignation date to the exact moment who run the method
            DateTime assignationTime = DateTime.Now;
            listAsuntoToUnqueue.ForEach(asunto => asunto.AssignmentDate = assignationTime);
            // Update on Service Database confirmation asuntos
            SQL.Asunto.RemoveFromQueueAndSaveHistoricData(listAsuntoToUnqueue);
            // Save pending information to list
            DeliverAsuntoList.Remove(listAsuntoToUnqueue);
            // Stop sending pending asuntos
            StopSendAsuntosPending();
        }

        
        
    }
}
