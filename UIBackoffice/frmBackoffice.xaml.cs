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
        private class OperatorReport
        {
            public string UserName { get; set; }

            public string DisplayName { get; set; }

            public OperatorReport(string userName, string displayName)
            {
                UserName = userName;
                DisplayName = displayName;
            }
        }    
        
        private class HourReport
        {
            public int Value { get; set; }

            public string Name { get; set; }

            public string ReportSource { get; set; }

            public HourReport(int hour, string visibleName, string reportOrigin)
            {
                Value = hour;
                Name = visibleName;
                ReportSource = reportOrigin;
            }
        }   
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

        BalanceDay balanceOfOperators;

        OperatorReport currentOperatorFilter;

        HourReport currentHourFilter;

        static OperatorReport oprAllOperators = new OperatorReport("all", "<Todos>");

        static HourReport basicHourFilter = new HourReport(0, "<Todos>", "UIBackoffice.Reports.RptBalanceTodayWithQuarters.rdlc");

        List<HourReport> lstHourFilter = new List<HourReport>()
        {
            basicHourFilter,
            new HourReport(7, "07:00", "UIBackoffice.Reports.RptBalanceTodaySevenHour.rdlc"),
            new HourReport(8, "08:00", "UIBackoffice.Reports.RptBalanceTodayEightHour.rdlc"),
            new HourReport(9, "09:00", "UIBackoffice.Reports.RptBalanceTodayNineHour.rdlc"),
            new HourReport(10, "10:00", "UIBackoffice.Reports.RptBalanceTodayTenHour.rdlc"),
            new HourReport(11, "11:00", "UIBackoffice.Reports.RptBalanceTodayElevenHour.rdlc"),
            new HourReport(12, "12:00", "UIBackoffice.Reports.RptBalanceTodayTwelveHour.rdlc"),
            new HourReport(13, "13:00", "UIBackoffice.Reports.RptBalanceTodayThirsteenHour.rdlc"),
            new HourReport(14, "14:00", "UIBackoffice.Reports.RptBalanceTodayFourteenHour.rdlc"),
            new HourReport(15, "15:00", "UIBackoffice.Reports.RptBalanceTodayFifteenHour.rdlc"),
            new HourReport(16, "16:00", "UIBackoffice.Reports.RptBalanceTodaySixteenHour.rdlc"),
            new HourReport(17, "17:00", "UIBackoffice.Reports.RptBalanceTodaySeventeenHour.rdlc"),
            new HourReport(18, "18:00", "UIBackoffice.Reports.RptBalanceTodayEighteenHour.rdlc"),
            new HourReport(19, "19:00", "UIBackoffice.Reports.RptBalanceTodayNineteenHour.rdlc"),
            new HourReport(20, "20:00", "UIBackoffice.Reports.RptBalanceTodayTwentyHour.rdlc"),
            new HourReport(21, "21:00", "UIBackoffice.Reports.RptBalanceTodayTwentyOneHour.rdlc")
        };
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

        /// <summary>
        /// Controls report filtering by operators
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CboOperatorReportFiltering_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OperatorReport selectedOperator = cboOperatorReportFiltering.SelectedItem as OperatorReport;
            if (selectedOperator != null) SetReportFiltering(selectedOperator);
        }

        private void CboHourReportFiltering_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HourReport selectedHour = cboHourReportFiltering.SelectedItem as HourReport;
            if (selectedHour != null) SetReportFiltering(selectedHour);
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
            LoadServiceVariable(true);
            
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

        private void SetReportFiltering(OperatorReport prmOperatorToFilter)
        {
            if(currentOperatorFilter != prmOperatorToFilter) {
                // Save current operator in local Variable
                currentOperatorFilter = prmOperatorToFilter;
                // Load and refresh report
                LoadReportInformation();
            }
        }

        private void SetReportFiltering(HourReport prmHourToFilter)
        {
            if(currentHourFilter != prmHourToFilter) {
                // Set origin of report
                rptBalanceTotals.LocalReport.ReportEmbeddedResource = prmHourToFilter.ReportSource;
                // Save current hour filter on local variable
                currentHourFilter = prmHourToFilter;
                // Load and refresh report
                LoadReportInformation();
            }
        }

        /// <summary>
        /// Instanciate a new balance day object and loads with base information
        /// </summary>
        private async void StartReportBalanceDayConfiguration()
        {
            if (balanceOfOperators == null) {
                currentOperatorFilter = oprAllOperators;
                // Generate a new Report Data Source
                if (rptDataSourceBalanceDay == null) rptDataSourceBalanceDay = new ReportDataSource();
                rptDataSourceBalanceDay.Name = "dsBalanceToday";
                rptBalanceTotals.LocalReport.DataSources.Add(rptDataSourceBalanceDay);
                rptBalanceTotals.LocalReport.ReportEmbeddedResource = basicHourFilter.ReportSource;
                rptBalanceTotals.ShowBackButton = false;
                rptBalanceTotals.ShowDocumentMapButton = false;
                rptBalanceTotals.ShowPageNavigationControls = false;
                rptBalanceTotals.ShowRefreshButton = false;
                rptBalanceTotals.ShowStopButton = false;
                balanceOfOperators = new BalanceDay();
                await balanceOfOperators.Generate(lstDetailedOperators.GetOperatorList());
                // Load initial information
                LoadReportInformation();
            }
        }
        

        /// <summary>
        /// Filter report list by operator
        /// </summary>
        /// <param name="prmReportToFilter"></param>
        private void LoadReportInformation()
        {
            rptDataSourceBalanceDay.Value = balanceOfOperators.List.FindAll((operSelect) => operSelect.UserName == currentOperatorFilter.UserName).ToList();
            if (currentOperatorFilter == oprAllOperators) rptDataSourceBalanceDay.Value = balanceOfOperators.List;            
            RefreshReportBalanceCurrentDay();
        }

        private void generateContentForHourReportFiltering()
        {
            // Unsubscribe event for selection change
            cboHourReportFiltering.SelectionChanged -= CboHourReportFiltering_SelectionChanged;
            // Load itemsource for combo hour filtering
            cboHourReportFiltering.ItemsSource = lstHourFilter;
            // Set current index for combo in start
            cboHourReportFiltering.SelectedIndex = 0;
            // Subscribe event for control selection changed
            cboHourReportFiltering.SelectionChanged += CboHourReportFiltering_SelectionChanged;
        }
        private void generateContentAndLoadCboOperatorReportFiltering()
        {
            // Unsusbcribe to the event
            cboOperatorReportFiltering.SelectionChanged -= CboOperatorReportFiltering_SelectionChanged;
            // Create a list for save all user variables
            List<OperatorReport> lstUserFullNames = new List<OperatorReport>();
            // Add a generic value
            lstUserFullNames.Add(oprAllOperators);
            // Iterates over all operator of backoffice
            foreach (var opBackoffice in lstDetailedOperators) {
                // Add to the list of full users
               lstUserFullNames.Add(new OperatorReport(opBackoffice.UserName, opBackoffice.FullName));
            }
            // Load itemsource of Combo
            cboOperatorReportFiltering.ItemsSource = lstUserFullNames;
            // Set index of combo in first position
            cboOperatorReportFiltering.SelectedIndex = 0;
            // Subscribe to selection changed event
            cboOperatorReportFiltering.SelectionChanged += CboOperatorReportFiltering_SelectionChanged;    
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

        private void RefreshReportBalanceCurrentDay()
        { 
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
        private async void LoadServiceVariable(bool isStartPetition = false)
        {
            try
            {
                // Generate a new Operator Logic object
                Logica.Operador logOper = new Logica.Operador();
                // Create a new list and save all operators on them
                List<Entidades.Operador> lstOperators = await logOper.GetOperatorWorkingToday();
                // Convert to observable collection
                lstDetailedOperators.ClearListAndConvertFromOperator(lstOperators);
                // Update next event time left
                updateNextEventTimeLeft();
                // Start timer to check periodically
                startTimeToNextEvent();
                // Puts the observable collection on itemsource
                dgConnectedUser.ItemsSource = lstDetailedOperators;
                // Update combobox for operator filtering and hour filtering
                generateContentAndLoadCboOperatorReportFiltering();
                generateContentForHourReportFiltering();
                // If is a start petition refresh balance of the day
                if (isStartPetition) StartReportBalanceDayConfiguration();
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

        /// <summary>
        /// On asunto process completed sent a refresh to balance list
        /// </summary>
        /// <param name="a"></param>
        public async void AsuntoProcessCompleted(Asunto a)
        {   
            try {
                balanceOfOperators.Increment(a);
            }
            catch (Exception ex) {
                Except.Throw(ex);                
            }
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                LoadReportInformation();                
            }));
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
                LoadServiceVariable();
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
