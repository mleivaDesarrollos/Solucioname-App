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
                // Cargamos los asuntos encolados en memoria
                lstAsuntosToDeliver = new ObservableCollection<Entidades.Asunto>(SQL.Asunto.getQueue());
                // Configuramos la colección para que opere en base a las modificaciones
                lstAsuntosToDeliver.CollectionChanged += LstAsuntosToDeliver_CollectionChanged;
                // Inicializamos el valor del timer
                tmrDeliverPendingAsuntos = new System.Timers.Timer();
                // Obtenemos el intervalo de repetición del timer
                tmrDeliverPendingAsuntos.Interval = Convert.ToDouble(ConfigurationManager.AppSettings.GetValues("DELIVER_PENDING_ASUNTOS_TIME_INTERVAL")[0]);
                // Establecemos la funcion relacionada
                tmrDeliverPendingAsuntos.Elapsed += DeliverPendingAsuntos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void LstAsuntosToDeliver_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Si se agrega un elemento a la colección de distribución y está desactivado se procesa la activación del deliver pending timer
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Si la acción corresponde a un agregado, se procede a hacer una inserción sobre la base de respaldo
                Entidades.Asunto asuntoNuevo = e.NewItems[0] as Entidades.Asunto;
                // Add to SQL Qeue
                SQL.Asunto.AddToQueue(asuntoNuevo);
                // Si el timer se encuentra deshabilitado se habilita
                if (!tmrDeliverPendingAsuntos.Enabled)
                {
                    // Iniciamos el servicio de distribución de asuntos
                    StartSendAsuntosPending();
                }                
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // Convertimos el asunto a tipo entidad
                Entidades.Asunto asuntoEliminado = e.OldItems[0] as Entidades.Asunto;
                // Save date of asunto assignment
                asuntoEliminado.AssignmentDate = DateTime.Now;
                // Removemos el asunto de la base de respaldo
                SQL.Asunto.RemoveFromQueueAndSaveHistoricData(asuntoEliminado);
                // Sent update request to logged backoffice
                SentBalanceRefreshOnBackoffice(asuntoEliminado);
                // Si el listado queda en 0 luego de remover el asunto del listado de pendientes, se detiene la task
                if (lstAsuntosToDeliver.Count == 0)
                {
                    // Detenemos el timer
                    StopSendAsuntosPending();
                    Log.Info(_asuntosPendingClassName, "the service has been stopped correctly because all asuntos has been delivered.");
                }
            }
        }

        /// <summary>
        /// Si todos los valores del timer para envío son correctos, se inicia el proceso de envío de asuntos
        /// </summary>
        private void StartSendAsuntosPending(bool reportLog = true)
        {
            // Si el timer cumple con las siguientes condiciones se inicia procedimiento
            if (tmrDeliverPendingAsuntos != null && lstAsuntosToDeliver.Count > 0 )
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
            if (tmrDeliverPendingAsuntos != null)
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
                    List<Entidades.Asunto> lstAsuntosOfOperator = lstAsuntosToDeliver.Where(asunto => asunto.Oper.UserName == connectedOperator.UserName).ToList();
                    // Sent asuntos to operator
                    SentAsuntoToOperator(lstAsuntosOfOperator);
                }                
            }            
            StartSendAsuntosPending(false);               
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

        /// <summary>
        /// Del listado de asuntos pendientes a entregar el día de hoy, comprueba que operadores se encuentran disponibles para recibir los asuntos
        /// </summary>
        /// <returns></returns>
        private List<Entidades.Asunto> getAsuntosToDeliverCheckingConnectedOperators()
        {
            // Generamos el listado de asuntos a devolver
            List<Entidades.Asunto> lstAsuntosFilteredByConnectedOperator = new List<Entidades.Asunto>();
            // Recorremos el diccionario con operadores conectados
            foreach (var asuntosPending in lstAsuntosToDeliver)
            {
                // Si el operador
                if (lstOperatorMustConnected.ToList().Exists( (operIterate) => operIterate.Operator.Status != Entidades.AvailabiltyStatus.Disconnected && operIterate.Operator.UserName == asuntosPending.Oper.UserName))
                {
                    lstAsuntosFilteredByConnectedOperator.Add(asuntosPending);
                }
            }            
            // Devolvemos el listado procesado
            return lstAsuntosFilteredByConnectedOperator;
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
                    lstAsuntosToDeliver.Remove(lstAsuntosToDeliver.First((asunto) => asunto.Numero == asuntoToConfirm.Numero && asunto.Oper.UserName == asuntoToConfirm.Oper.UserName));
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
                    // Unconnects event temporaly to process confirmation
                    lstAsuntosToDeliver.CollectionChanged -= LstAsuntosToDeliver_CollectionChanged;
                    // Update on Service Database confirmation asuntos
                    SQL.Asunto.RemoveFromQueueAndSaveHistoricData(lstOfAsuntoToConfirm);
                    // Remove from asunto pending to deliver list
                    foreach (var asuntoToConfirm in lstOfAsuntoToConfirm) {
                        lstAsuntosToDeliver.Remove(
                            lstAsuntosToDeliver.First(
                                asuntoDeliver => asuntoDeliver.Numero == asuntoToConfirm.Numero &&
                                    asuntoDeliver.Oper.UserName == asuntoToConfirm.Oper.UserName
                                ));
                    }
                    // Check if post remove asuntos the delivering list is empty
                    if (lstAsuntosToDeliver.Count == 0) {
                        StopSendAsuntosPending();
                        Log.Info(_asuntosPendingClassName, "the service has been stopped correctly because all asuntos has been delivered.");
                    }
                    // Connect event again to event handler collection changed
                    lstAsuntosToDeliver.CollectionChanged += LstAsuntosToDeliver_CollectionChanged;
                }
            } catch (Exception ex) {
                Log.Error("ConfirmingAsuntoReceipt", string.Format("Error processing batch confirmation asuntos for {0} : {1}", lstOfAsuntoToConfirm[INDEX_START].Oper, ex.Message));
            }
        }
        
    }
}
