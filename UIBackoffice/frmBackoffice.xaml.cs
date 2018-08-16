using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Entidades;
using Entidades.Service.Interface;
using System.Collections.ObjectModel;

namespace UIBackoffice
{
    /// <summary>
    /// Interaction logic for frmBackoffice.xaml
    /// </summary>
    public partial class frmBackoffice : Window, Entidades.Service.Interface.IServicioCallback
    {       
        #region constructor
        public frmBackoffice()
        {
            InitializeComponent();
            ConfigurarCustomWindow();
            SetUpDate();
            configureTimeToNextEvent();
        }
        #endregion

        #region local_properties
        /// <summary>
        /// List of operator working today
        /// </summary>
        ObservableCollection<OperBackoffice> lstOperatorWorkingToday = new ObservableCollection<OperBackoffice>();

        System.Timers.Timer tmrCheckTimeForNextEvent;
        #endregion

        #region event_subscription

        /// <summary>
        /// On closing window of any source, sents a confirmation dialog to front.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ConsultarCierreApp() == true) {
                e.Cancel = false;
            }
        }
        private void btnGetOperatorList_Click(object sender, RoutedEventArgs e)
        {
            PopulateOperatorList();
        }
        #endregion

        #region helper_methods
        /// <summary>
        /// Standarized message for closing application
        /// </summary>
        /// <returns>null, false or true</returns>
        private bool? ConsultarCierreApp()
        {
            return Util.MsgBox.Consulta("¿Estás seguro de que deseas salir de la aplicacion?");
        }
        /// <summary>
        /// Get and set operator information from the service
        /// </summary>
        private async void PopulateOperatorList()
        {
            try
            {
                // Generate a new Operator Logic object
                Logica.Operador logOper = new Logica.Operador();
                // Create a new list and save all operators on them
                List<Entidades.Operador> lstOperators = await logOper.GetOperatorWorkingToday();
                // Convert to observable collection
                convertOperatorToOperatorBackoffice(lstOperators);
                // Puts the observable collection on itemsource
                dgConnectedUser.ItemsSource = lstOperatorWorkingToday;
                // Update next event time left
                updateNextEventTimeLeft();
                // Start timer to check periodically
                startTimeToNextEvent();                
            }
            catch (Exception ex)
            {
                // Shows error details on a message box
                Util.MsgBox.Error("Ha ocurrido un error al llenar el listado de operadores: " + ex.Message);   
            }
        }

        /// <summary>
        /// Configure timer to be ready on requirement
        /// </summary>
        private void configureTimeToNextEvent()
        {
            tmrCheckTimeForNextEvent = new System.Timers.Timer();
            tmrCheckTimeForNextEvent.Interval = 10000;
            tmrCheckTimeForNextEvent.Elapsed += TmrCheckTimeForNextEvent_Elapsed;
        }

        /// <summary>
        /// Start time to check and update next event
        /// </summary>
        private void startTimeToNextEvent()
        {
            if(tmrCheckTimeForNextEvent != null) {
                if (!tmrCheckTimeForNextEvent.Enabled) {
                    tmrCheckTimeForNextEvent.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Stop timer to check and update next event
        /// </summary>
        private void stopTimeToNextEvent()
        {
            if(tmrCheckTimeForNextEvent != null) {
                if (tmrCheckTimeForNextEvent.Enabled) {
                    tmrCheckTimeForNextEvent.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Event to launch when time is elapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmrCheckTimeForNextEvent_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            updateNextEventTimeLeft();
        }

        /// <summary>
        /// Update time left for next event on operator
        /// </summary>
        private void updateNextEventTimeLeft()
        {
            if (lstOperatorWorkingToday != null) {
                foreach (var operBackoffice in lstOperatorWorkingToday) {
                    TimeSpan difference = operBackoffice.NextEvent - DateTime.Now;
                    operBackoffice.TimeLeftToNextEvent = Convert.ToInt32(difference.TotalMinutes);
                }
            }
        }

        private void SetUpDate()
        {
            txtTodayDate.Text = DateTime.Now.ToShortDateString();
        }

        /// <summary>
        /// Convert list of operators to backoffice operators
        /// </summary>
        private void convertOperatorToOperatorBackoffice(List<Entidades.Operador> paramOperatorList)
        {
            // Clean all data on the list
            lstOperatorWorkingToday.Clear();
            foreach (var oper in paramOperatorList) {
                // Adds a operator to the list
                lstOperatorWorkingToday.Add(new OperBackoffice(oper));
            }
        }

        private async void disconnectWaitingTime()
        {
            await Task.Run(() => { System.Threading.Thread.Sleep(10000); });
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                App.Current.Shutdown();
            }));
        }
        #endregion

        #region service_callback_implementation
        public void EnviarAsunto(Asunto a)
        {
            
        }

        public void ForceDisconnect()
        {
            disconnectWaitingTime();
        }

        public void Mensaje(string message)
        {
            // Calls to show message on async thread
            Dispatcher.BeginInvoke((Action)(() =>
            {
                Util.MsgBox.Error("Mensaje proveniente del servicio: " + message);
            }));            
        }

        public void ServiceChangeStatusRequest(AvailabiltyStatus paramNewStatus)
        {
            
        }

        bool IServicioCallback.IsActive()
        {
            return true;
        }
        #endregion

    }
}
