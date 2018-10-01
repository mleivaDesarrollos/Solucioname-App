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
using System.ComponentModel;
using System.Configuration;

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
            public string Value { get; set; }

            public string Name { get; set; }

            public int IntValue { get; set; }
            
            public HourReport(string hour, string visibleName, int intvalue)
            {
                Value = hour;
                Name = visibleName;
                IntValue = intvalue;
            }
        }   

        internal enum TimeFilteringReport
        {
            Totals,
            Quarters,
            QuartersAndTotals
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
        Logica.OperBackofficeList lstDetailedOperators;

        BackgroundWorker bgwShowMessage = null;

        List<Asunto> lstAsuntosWithoutAssign = new List<Asunto>();

        System.Timers.Timer tmrCheckTimeForNextEvent;

        System.Timers.Timer tmrCheckCurrentTimeHourChange;

        static readonly double TIME_MARGIN_FOR_NEXT_HOUR_CALCULATION = 5000;

        ReportDataSource rptDataSourceBalanceDay;

        Logica.Balance balanceOfOperators;

        OperatorReport currentOperatorFilter;

        HourReport currentHourFilter;

        TimeFilteringReport currentTimeFiltering;

        ConfigBackoffice Config;

        static readonly OperatorReport oprAllOperators = new OperatorReport("all", "<Todos>");

        static readonly HourReport basicHourFilter = new HourReport("all", "<Todos>", 0);

        static readonly HourReport currentTimeHourFilter = new HourReport("current", "<Ahora>", -1);

        List<HourReport> lstHourFilter = new List<HourReport>()
        {
            basicHourFilter,
            currentTimeHourFilter,
            new HourReport("seven", "07:00", 7),
            new HourReport("eight", "08:00", 8),
            new HourReport("nine", "09:00", 9),
            new HourReport("ten", "10:00", 10),
            new HourReport("eleven", "11:00", 11),
            new HourReport("twelve", "12:00", 12),
            new HourReport("thirsteen", "13:00", 13),
            new HourReport("fourteen", "14:00", 14),
            new HourReport("fifteen", "15:00", 15),
            new HourReport("sixteen", "16:00", 16),
            new HourReport("seventeen", "17:00", 17),
            new HourReport("eighteen", "18:00", 18),
            new HourReport("nineteen", "19:00", 19),
            new HourReport("twenty", "20:00", 20),
            new HourReport("twentyone", "21:00", 21)
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
                frmNewAsunto.Owner = this;
                frmNewAsunto.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                if (frmNewAsunto.ShowDialog() == true) {
                    balanceOfOperators.Increment(frmNewAsunto.confirmedNewAsunto);
                    frmNewAsunto.Close();
                    RefreshReportBalanceCurrentDay();
                }
            }            
        }

        private void txtAssignStackAsunto_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(lstAsuntosWithoutAssign.Count > 0) {
                // Generate a new instance of Batch add
                frmAddBatchAsunto frmBatchAddAsunto = new frmAddBatchAsunto(lstDetailedOperators.GetOperatorListReadyToReceive(), lstAsuntosWithoutAssign);
                // Configures owner of window
                frmBatchAddAsunto.Owner = this;
                // Configure window related on parent
                frmBatchAddAsunto.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                // Show dialog
                frmBatchAddAsunto.ShowDialog();
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
        private void rdbTotals_Checked(object sender, RoutedEventArgs e)
        {
            if(rdbTotals.IsChecked == true) {
                currentTimeFiltering = TimeFilteringReport.Totals;
            }
            else if (rdbQuarters.IsChecked == true) {
                currentTimeFiltering = TimeFilteringReport.Quarters;
            }
            else {
                currentTimeFiltering = TimeFilteringReport.QuartersAndTotals;
            }
            LoadReportInformation();
        }

        /// <summary>
        /// Reaching next hour, update filter information and setup next start time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmrCheckCurrentTimeHourChange_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                // Establish an update for report filtering
                SetReportFiltering(currentTimeHourFilter);
                // Desactivate timer
                tmrCheckCurrentTimeHourChange.Enabled = false;
            }));
        }

        private void BgwShowMessage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblMessage.Visibility = Visibility.Hidden;
            lblMessage.Text = "";
            bgwShowMessage = null;
        }

        private void BgwShowMessage_DoWork(object sender, DoWorkEventArgs e, int iCantSegs)
        {
            // Pausamos la ejecución teniendo en cuenta la cantidad de segundos solicitados
            System.Threading.Thread.Sleep(iCantSegs * 1000);
        }

        #endregion

        #region public_interface
        public void LoadConnectionInformation()
        {
            getConfigurationInformation();
            configureAfterSuccessConnectionValues();
            configureTimeToNextEvent();
            configureErrorService();
            LoadServiceVariable(true);
            getAsuntosWithoutAssignation();
        }


        #endregion

        #region helper_methods


        /// <summary>
        ///  Gets from configuration file information related to application work
        /// </summary>
        private void getConfigurationInformation()
        {
            try {                
                Config = new ConfigBackoffice();                                
            }
            catch (Exception ex) {
                Assert.Throw(ex);                
            }            
        }

        /// <summary>
        /// Start timer for hour change event
        /// </summary>
        private void StartTimerToCheckHourChange()
        {
            if (tmrCheckCurrentTimeHourChange == null) tmrCheckCurrentTimeHourChange = new System.Timers.Timer();
            if (!tmrCheckCurrentTimeHourChange.Enabled) {
                // Check current Timepsan of the day
                TimeSpan nowTime = DateTime.Now.TimeOfDay;
                // Check next hour difference using Math Ceiling
                var nextHour = TimeSpan.FromHours(Math.Ceiling(nowTime.TotalHours));
                // Calc the difference and save on a variable
                var differenceToNextHour = (nextHour - nowTime).TotalMilliseconds;
                // Set calulated value on interval property
                tmrCheckCurrentTimeHourChange.Interval = differenceToNextHour + TIME_MARGIN_FOR_NEXT_HOUR_CALCULATION;
                // Configure launch event
                tmrCheckCurrentTimeHourChange.Elapsed += TmrCheckCurrentTimeHourChange_Elapsed;
                // Start operation of timer
                tmrCheckCurrentTimeHourChange.Enabled = true;              
            }
        }

        /// <summary>
        /// Checks if the timer is active and stop current activity
        /// </summary>
        private void StopTimerToCheckHourChange()
        {
            if(tmrCheckCurrentTimeHourChange != null) {
                if (tmrCheckCurrentTimeHourChange.Enabled) {
                    // Stop timer activity
                    tmrCheckCurrentTimeHourChange.Enabled = false;
                }
            }
        }

        private async void getAsuntosWithoutAssignation()
        {
            // On notification receipt, loads asuntos from service. Generate a new logic object
            Logica.Asunto logAsunto = new Logica.Asunto();
            // Gets from the logic object all asuntos
            lstAsuntosWithoutAssign = await logAsunto.GetUnassignedAsuntos();
            // Update total of asuntos pendientes
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                txtTotalAsuntoWithoutAssign.Text = lstAsuntosWithoutAssign.Count.ToString();
            }));
        }

        private void ActivateTimeFiltering()
        {
            rdbQuartersAndTotals.IsChecked = true;
            rdbQuarters.Checked += rdbTotals_Checked;
            rdbQuartersAndTotals.Checked += rdbTotals_Checked;
            rdbTotals.Checked += rdbTotals_Checked;
        }

        private void FillHourComboByOperator()
        {
            // Deactivate selection change event
            cboHourReportFiltering.SelectionChanged -= CboHourReportFiltering_SelectionChanged;
            if(currentOperatorFilter == oprAllOperators) {
                cboHourReportFiltering.ItemsSource = lstHourFilter;
            }
            else {
                // Gets current operator filtered
                OperatorReport opReportFiltered = cboOperatorReportFiltering.SelectedItem as OperatorReport;
                // Gets Operator Backoffice entity
                OperBackoffice operboFilter = lstDetailedOperators.First((opbo) => opbo.UserName == opReportFiltered.UserName);
                // Get Hour of start and end
                int opboStartHour = operboFilter.StartTime.Hour;
                int opboEndHour = operboFilter.EndTime.Hour;
                // Loads combo of hours with operation time of seleceted operator
                cboHourReportFiltering.ItemsSource = lstHourFilter.Where((hrFlt) => (hrFlt.IntValue >= opboStartHour && hrFlt.IntValue < opboEndHour) || hrFlt.IntValue == basicHourFilter.IntValue).ToList();
                
            }
            // Change index of filter
            cboHourReportFiltering.SelectedItem = basicHourFilter;
            currentHourFilter = basicHourFilter;
            // Activate selectión changed event
            cboHourReportFiltering.SelectionChanged += CboHourReportFiltering_SelectionChanged;
        }

        private string GetTimeFilterName(TimeFilteringReport filterOption)
        {
            switch (filterOption) {
                case TimeFilteringReport.Totals:
                    return "HourOnly";
                case TimeFilteringReport.Quarters:
                    return "QuarterOnly";
                case TimeFilteringReport.QuartersAndTotals:
                default:
                    return "QuarterAndHour";
            }
        }

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
            // Stop timer checking
            StopTimerToCheckHourChange();
            if (currentOperatorFilter != prmOperatorToFilter) {
                // Save current operator in local Variable
                currentOperatorFilter = prmOperatorToFilter;
                // Set hour combo with user related times
                FillHourComboByOperator();
                // Load and refresh report
                LoadReportInformation();
            }
        }
        
        private void SetReportFiltering(HourReport prmHourToFilter)
        {
            // Stop current time checking
            StopTimerToCheckHourChange();
            if (currentHourFilter != prmHourToFilter) {
                // Save current hour filter on local variable
                currentHourFilter = prmHourToFilter;
            }
            if (prmHourToFilter == currentTimeHourFilter) {
                // Apply special filter for check current time
                currentHourFilter = lstHourFilter.First((hrFlt) => hrFlt.IntValue == DateTime.Now.Hour);
                StartTimerToCheckHourChange();
            }
            // Load and refresh report
            LoadReportInformation();
        }


        /// <summary>
        /// Muestra en la barra de estado un mensaje 
        /// </summary>
        /// <param name="messageToShow">Mensaje a mostrar por pantalla</param>
        /// <param name="iCantSegs">Cantidad de segundos que se quiere mostrar el mensaje en pantalla</param>
        public void ShowMessageOnStatusBar(string messageToShow, int iCantSegs)
        {
            if (bgwShowMessage == null) {
                // Generamos un Background Worker para mantener el mensaje activo por unos momentos y luego ocultar
                bgwShowMessage = new BackgroundWorker();
                bgwShowMessage.DoWork += (sender, e) => BgwShowMessage_DoWork(sender, e, iCantSegs);
                bgwShowMessage.RunWorkerCompleted += BgwShowMessage_RunWorkerCompleted;
                // Cargamos el mensaje y lo hacemos visible
                lblMessage.Text = messageToShow;
                lblMessage.Visibility = Visibility.Visible;
                // Ejecutamos el worker asincronico            
                bgwShowMessage.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Instanciate a new balance day object and loads with base information
        /// </summary>
        private async void StartReportBalanceDayConfiguration()
        {
            if (balanceOfOperators == null) {
                currentOperatorFilter = oprAllOperators;
                currentHourFilter = basicHourFilter;
                currentTimeFiltering = TimeFilteringReport.QuartersAndTotals;
                // Generate a new Report Data Source
                if (rptDataSourceBalanceDay == null) rptDataSourceBalanceDay = new ReportDataSource();
                rptDataSourceBalanceDay.Name = "dsBalanceToday";                
                rptBalanceTotals.LocalReport.DataSources.Add(rptDataSourceBalanceDay);
                rptBalanceTotals.LocalReport.ReportEmbeddedResource = "UIBackoffice.Reports.RptBalanceTodayWithQuarters.rdlc";
                rptBalanceTotals.ShowBackButton = false;
                rptBalanceTotals.ShowDocumentMapButton = false;
                rptBalanceTotals.ShowPageNavigationControls = false;
                rptBalanceTotals.ShowRefreshButton = false;
                rptBalanceTotals.ShowStopButton = false;
                balanceOfOperators = new Logica.Balance();
                await balanceOfOperators.Generate(lstDetailedOperators.GetOperatorList());
                rptDataSourceBalanceDay.Value = balanceOfOperators.List;
                ActivateTimeFiltering();
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
            ReportParameter operatorFilter = new ReportParameter("ShowOneUserOnly", currentOperatorFilter.UserName);
            ReportParameter hourFilter = new ReportParameter("ShowHour", currentHourFilter.Value);
            ReportParameter timeFilter = new ReportParameter("FilterBy", GetTimeFilterName(currentTimeFiltering));
            rptBalanceTotals.LocalReport.SetParameters(new ReportParameter[] { operatorFilter, hourFilter, timeFilter });
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
            lstDetailedOperators = new Logica.OperBackofficeList(Config);
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
        public void SentAsuntosBatch(List<Asunto> lstA)
        {

        }

        public void BatchAsuntoProcessCompleted(List<Asunto> lstA)
        {

        }

        public async void NotifyNewAsuntoFromSolucioname()
        {
            // Loads asuntos without assignation
            getAsuntosWithoutAssignation();
            // When the list is received, notifies
            await Dispatcher.BeginInvoke((Action)(() =>
            {
                ShowMessageOnStatusBar("Se recibieron nuevos asuntos", 10);
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
