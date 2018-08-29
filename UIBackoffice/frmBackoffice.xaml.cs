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
using Errors;
using System.Diagnostics;
using Microsoft.Reporting.WinForms;

namespace UIBackoffice
{
    /// <summary>
    /// Interaction logic for frmBackoffice.xaml
    /// </summary>
    public partial class frmBackoffice : Window, Entidades.Service.Interface.IServicioCallback, IAssertion, IException
    {       
        #region constructor
        public frmBackoffice()
        {
            InitializeComponent();
            ConfigurarCustomWindow();
        }
        #endregion

        #region local_properties
        /// <summary>
        /// List of operator working today
        /// </summary>
        OperBackofficeList lstDetailedOperators = new OperBackofficeList();

        System.Timers.Timer tmrCheckTimeForNextEvent;

        ReportDataSource rptDataSourceBalanceDay;
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

        private void mnuAddAsunto_Click(object sender, RoutedEventArgs e)
        {
            if(dgConnectedUser.SelectedItem != null) {
                // gets selected operator
                Operador operatorToAddAsunto = (dgConnectedUser.SelectedItem as OperBackoffice).Operator;
                // generates a new instance of the add asunto dialog
                frmAddAsunto frmNewAsunto = new frmAddAsunto(operatorToAddAsunto);
                // Shows the dialog
                frmNewAsunto.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                frmNewAsunto.ShowDialog();
            }            
        }

        /// <summary>
        /// Dispose a relog for click on unlog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtUnlog_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(Util.MsgBox.Consulta("¿Está seguro de que desea salir y reingresar en sistema?") == true) {
                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void btnGetBalanceOper_Click(object sender, RoutedEventArgs e)
        {
            RefreshReportBalanceCurrentDay();
        }
        /// <summary>
        /// Dispose a Reconnect button for connection to the service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReconnect_Click(object sender, RoutedEventArgs e)
        {
            // Disable on request for connection
            btnReconnect.IsEnabled = false;
            // Sent reconnect signal
            SentReconnectSignal();
        }

        #endregion

        #region public_interface
        public void LoadConnectionInformation()
        {
            configureAfterSuccessConnectionValues();
            configureTimeToNextEvent();
            configureErrorService();
            PopulateOperatorList();
        }
        #endregion

        #region helper_methods
        /// <summary>
        /// Set up error service adding to the interface the current instance
        /// </summary>
        private void configureErrorService()
        {
            // Set up Exception Interface
            Except.LoadInterface(this);
            // Set up Assertion Interface
            Assert.LoadInterface(this);

        }

        private async void SentReconnectSignal()
        {
            // Generate new logic operator object
            Logica.Operador logOperator = new Logica.Operador();
            // Gets operator entity saved on application properties
            Operador operatorLogged = App.Current.Properties["user"] as Operador;
            // if connection is valid
            if(await logOperator.ConnectBackoffice(operatorLogged, this) != null) {
                // Loads connection information
                LoadConnectionInformation();
                // Change visibility of panel
                grdBackofficePanel.Visibility = Visibility.Visible;
                grdReconnectionPanel.Visibility = Visibility.Collapsed;
            } else {
                // if not reactivates the button
                btnReconnect.IsEnabled = true;
            }
        }

        private async void RefreshReportBalanceCurrentDay()
        {
            // Generate a new Report Data Source
            if (rptDataSourceBalanceDay == null) rptDataSourceBalanceDay = new ReportDataSource();
            BalanceCurrentDay balance = new BalanceCurrentDay();
            await balance.RefreshBalance();
            rptDataSourceBalanceDay.Name = "dsBalanceDay";
            rptDataSourceBalanceDay.Value = balance.List; 
            rptBalanceTotals.LocalReport.DataSources.Add(rptDataSourceBalanceDay);
            rptBalanceTotals.LocalReport.ReportEmbeddedResource = "UIBackoffice.rptBalanceOfAssignedAsuntosCurrentDay.rdlc";
            rptBalanceTotals.RefreshReport();
        }       

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
                lstDetailedOperators.ClearListAndConvertFromOperator(lstOperators);
                // Puts the observable collection on itemsource
                dgConnectedUser.ItemsSource = lstDetailedOperators;
                // Update next event time left
                updateNextEventTimeLeft();
                // Start timer to check periodically
                startTimeToNextEvent();
                dgConnectedUser.Items.Refresh();         
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
            // Execute method for update all operator on the list
            lstDetailedOperators.UpdateTimeLeftOnOperators();
        }
     
        
        private void configureAfterSuccessConnectionValues()
        {
            txtTodayDate.Text = DateTime.Now.ToShortDateString();
            // Gets operator from application
            Operador operLogged = App.Current.Properties["user"] as Operador;
            txtOper.Text = operLogged.Nombre + " " + operLogged.Apellido;
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
            if(paramNewStatus == AvailabiltyStatus.Disconnected) {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    // Deactivates backoffice panel
                    grdBackofficePanel.Visibility = Visibility.Collapsed;
                    // Shows reconection panel
                    grdReconnectionPanel.Visibility = Visibility.Visible;
                    // Activate reconnection button
                    btnReconnect.IsEnabled = true;
                }));
            }
        }

        public void RefreshOperatorStatus()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                PopulateOperatorList();
            }));
        }


        bool IServicioCallback.IsActive()
        {
            return true;
        }

        void IException.Notify(string Message)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                Util.MsgBox.Error(Message);
            }));
        }

        void IAssertion.NotifyAndClose(string prmMessage)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                Util.MsgBox.ErrorAndClose(prmMessage);
            }));            
        }
        
        void IAssertion.Notify(string prmMessage)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                Util.MsgBox.Error(prmMessage);
            }));
            
        }

        #endregion

    }
}
