using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Entidades;
using System.Collections.Generic;

namespace UISolucioname
{
    /// <summary>
    /// Interaction logic for frmMainFrame.xaml
    /// </summary>
    public partial class frmMainFrame : Window, Entidades.Service.Interface.IServicioCallback
    {
        #region property_declarations
        public PaginaAsunto pgAsunto = null;

        public PaginaActuacion pgActuacion = null;
                
        BackgroundWorker bgwShowMessage = null;

        public PaginaAsunto pagAsunto
        {
            get
            {
                if (pgAsunto == null)
                {
                    pgAsunto = new PaginaAsunto();
                }
                return pgAsunto;
            }
        }

        public PaginaActuacion pagActuacion
        {
            get
            {
                if (pgActuacion == null)
                {
                    pgActuacion = new PaginaActuacion();
                }
                return pgActuacion;
            }
        }

        public PaginaListadoGeneral pagListadogeneral = null;

        private PaginaReporte pgReporte;

        public PaginaReporte pagReporte
        {
            get
            {
                if (pgReporte == null)
                {
                    pgReporte = new PaginaReporte();
                }
                return pgReporte;
            }
        }

        private AvailabiltyStatus _currentStatus;

        public AvailabiltyStatus currentStatus {
            get {
                return _currentStatus;
            }
            set {
                _currentStatus = value;
                switch (value) {
                    case AvailabiltyStatus.Disconnected:
                        cboConnect.ItemsSource = lstDisconnectedStatus;
                        imgConnectStatus.Source = new BitmapImage(new Uri(@"/Images/idlestatus.png", UriKind.Relative));
                        break;
                    case AvailabiltyStatus.Connected:
                        cboConnect.ItemsSource = lstConnectedStatus;
                        imgConnectStatus.Source = new BitmapImage(new Uri(@"/Images/greenstatus.png", UriKind.Relative));
                        break;
                    case AvailabiltyStatus.ReadyToReceive:
                        cboConnect.ItemsSource = lstConnectedStatus;
                        imgConnectStatus.Source = new BitmapImage(new Uri(@"/Images/greenstatus.png", UriKind.Relative));
                        break;
                    case AvailabiltyStatus.Break:
                        imgConnectStatus.Source = new BitmapImage(new Uri(@"/Images/yellowstatus.png", UriKind.Relative));
                        break;
                    case AvailabiltyStatus.Bath:
                        imgConnectStatus.Source = new BitmapImage(new Uri(@"/Images/yellowstatus.png", UriKind.Relative));
                        break;
                    case AvailabiltyStatus.SpecialTask:
                        imgConnectStatus.Source = new BitmapImage(new Uri(@"/Images/yellowstatus.png", UriKind.Relative));
                        break;
                    case AvailabiltyStatus.Error:
                        cboConnect.ItemsSource = lstErrorStatus;
                        imgConnectStatus.Source = new BitmapImage(new Uri(@"/Images/redstatus.png", UriKind.Relative));
                        break;
                    default:
                        break;
                }
            }
        }


        class StatusUI
        {
            public AvailabiltyStatus Status { get; set; }

            public string Description { get; set; }

            public bool IsDefault { get; set; }
            
            public StatusUI(AvailabiltyStatus status, string description, bool isDefault = false) {
                Status = status;
                Description = description;
                IsDefault = isDefault;
            }
        }

        static readonly List<StatusUI> lstConnectedStatusOnly = new List<StatusUI>()
        {
            new StatusUI(AvailabiltyStatus.ReadyToReceive, "Conectado", true),
            new StatusUI(AvailabiltyStatus.Break, "Break"),
            new StatusUI(AvailabiltyStatus.Bath, "Baño"),
            new StatusUI(AvailabiltyStatus.SpecialTask, "Tareas especiales"),
        };

        static readonly List<StatusUI> lstDisconnectedStatus = new List<StatusUI>()
        {
            new StatusUI(AvailabiltyStatus.Disconnected, "Desconectado", true),
            new StatusUI(AvailabiltyStatus.Connected, "Conectado")
        };

        static readonly List<StatusUI> lstConnectedStatus = new List<StatusUI>(lstConnectedStatusOnly)
        {
            new StatusUI(AvailabiltyStatus.Disconnected, "Desconectado")
        };

        static readonly List<StatusUI> lstErrorStatus = new List<StatusUI>()
        {
            new StatusUI(AvailabiltyStatus.Disconnected, "Desconectado"),
            new StatusUI(AvailabiltyStatus.Error, "Error en servicio", true)
        };

        
        #endregion

        #region constructor
        public frmMainFrame()
        {
            InitializeComponent();
            ConfigurarCustomWindow();
            SubscribirEventosBotonesDiario();
            ConfiguraComandosVentana();
            loadConnectionStatus();
        }
        #endregion

        #region helper_methods

        public void CargarInformacionUsuario()
        {
            CargarAsuntosDiarios();
            ConfigurarTabControl();
            EstablecerNombreUser();
        }

        private void loadConnectionStatus() {
            // Suscribe to updates on Combo
            configureComboConnectionItemChanged();
            // Sets initial value of the status
            currentStatus = AvailabiltyStatus.Disconnected;
            // Subscribe Selection Changed event
            cboConnect.SelectionChanged += cboConnect_SelectionChanged;
        }

        /// <summary>
        /// Code snippet for ItemSource Changed
        /// </summary>
        private void configureComboConnectionItemChanged() {
            // Get Descriptor from dependencys of combo
            var dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(ComboBox));
            // if the Dependency of combo is find
            if (dependencyPropertyDescriptor != null) {
                // Adds a event to control change on itemssource
                dependencyPropertyDescriptor.AddValueChanged(cboConnect, CboConnect_ItemsSourceChanged);
            }
        }
        
        private void ConfiguraComandosVentana()
        {
            RoutedCommand rcmdNuevoAsunto = new RoutedCommand();
            rcmdNuevoAsunto.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(rcmdNuevoAsunto, BtnNuevoAsuntoDiario_Click));
        }

        private void SubscribirEventosBotonesDiario()
        {
            btnNuevoAsuntoDiario.Click += BtnNuevoAsuntoDiario_Click;
        }

        private void ConfigurarTabControl()
        {
            tbcMainControl.SelectionChanged += TbcMainControl_SelectionChanged;
            tbcMainControl.SelectedIndex = 0;
        }
        private void EstablecerNombreUser()
        {
            Entidades.Operador entOperLogged = App.Current.Properties["user"] as Entidades.Operador;
            txtUsuarioLogueado.Text = entOperLogged.Nombre + " " + entOperLogged.Apellido;
        }

        /// <summary>
        /// Carga el listado de asuntos diarios y actualiza el datagrid vinculado
        /// </summary>
        public void CargarAsuntosDiarios()
        {
            try
            {
                // Generamos un nuevo objeto de Logica Asunto
                Logica.Asunto logAsunto = new Logica.Asunto();
                // Cargamos el ItemSource del dg Asuntos Diarios con la información recolectada de la base personal
                dgAsuntosDia.ItemsSource = logAsunto.TraerAsuntosDelDia(App.Current.Properties["user"] as Entidades.Operador);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //MsgBox.Error("Ha ocurrido un error al intentar traer los asuntos diarios: " + ex.Message);
            }
        }
        private bool? ConsultarCierreApp()
        {
            return Util.MsgBox.Consulta("¿Estás seguro de que deseas salir de la aplicacion?");
        }

        /// <summary>
        /// Muestra en la barra de estado un mensaje 
        /// </summary>
        /// <param name="messageToShow">Mensaje a mostrar por pantalla</param>
        /// <param name="iCantSegs">Cantidad de segundos que se quiere mostrar el mensaje en pantalla</param>
        public void ShowMessageOnStatusBar(string messageToShow, int iCantSegs)
        {
            if (bgwShowMessage == null)
            {
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
        #endregion

        #region event_subscriptions
        private void BtnNuevoAsuntoDiario_Click(object sender, RoutedEventArgs e)
        {
            tbcMainControl.SelectedIndex = 1;
            pagAsunto.GestionarNuevoAsunto();  
        }

        private void CboConnect_ItemsSourceChanged(object sender, EventArgs e) {
            // Get the list of Status from sender
            List<StatusUI> lstStatuses = cboConnect.ItemsSource as List<StatusUI>;
            // Sets item with default option selected
            cboConnect.SelectedItem = lstStatuses.Find((status) => status.IsDefault);
        }

        private void dgAsuntosDia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAsuntosDia.SelectedItem != null)
            {
                btnEditarAsuntoDiario.IsEnabled = true;
                btnEliminarAsuntoDiario.IsEnabled = true;
            }
            else
            {
                btnEliminarAsuntoDiario.IsEnabled = false;
                btnEditarAsuntoDiario.IsEnabled = false;
            }
        }
        private void TbcMainControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbcMainControl.SelectedIndex == 1)
            {
                if (frmAsunto.NavigationService.Content == null)
                {
                    frmAsunto.NavigationService.Navigate(pagAsunto.TraerInstanciaPagina());
                }
            }
            else if (tbcMainControl.SelectedIndex == 0)
            {
                if (pagListadogeneral == null)
                {
                    pagListadogeneral = new PaginaListadoGeneral();
                    frmListaGeneral.NavigationService.Navigate(pagListadogeneral.Pagina);
                }
            }
            else if (tbcMainControl.SelectedIndex == 2)
            {
                if (frmActuacion.NavigationService.Content == null)
                {
                    frmActuacion.NavigationService.Navigate(pagActuacion.TraerInstancia());
                }
            }
            else if (tbcMainControl.SelectedIndex == 3)
            {
                if (frmReporte.NavigationService.Content == null)
                {
                    frmReporte.NavigationService.Navigate(pagReporte.TraerInstancia());
                }
            }
        }
        private void btnEliminarAsuntoDiario_Click(object sender, RoutedEventArgs e)
        {            
            // Consulta de eliminación
            if (Util.MsgBox.Consulta("¿Está seguro que desea eliminar el asunto " + (dgAsuntosDia.SelectedItem as Entidades.AsuntoDiario).Numero + "?") == true)
            {
                try
                {
                    // Generamos un nuevo objeto lógica de asunto
                    Logica.Asunto logAsunto = new Logica.Asunto();
                    // Generamos la entidad a eliminar
                    Entidades.Asunto entAsunto = new Entidades.Asunto()
                    {
                        Numero = (dgAsuntosDia.SelectedItem as Entidades.AsuntoDiario).Numero,
                        Oper = App.Current.Properties["user"] as Entidades.Operador
                    };
                    // Procesamos la entidad y la eliminamos
                    logAsunto.Eliminar(entAsunto);
                    // Cargamos los asuntos diarios nuevamente.
                    CargarAsuntosDiarios();
                    // Cargamos el listado general
                    pagListadogeneral.ActualizarListado();
                }
                catch (Exception ex)
                {
                    Util.MsgBox.Error("Ha ocurrido un error al intentar eliminar el asunto de la base : " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Carga los datos del asunto seleccionado en la ventana de asuntos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditarAsuntoDiario_Click(object sender, RoutedEventArgs e)
        {
            if (dgAsuntosDia.SelectedItem != null)
            {
                pagAsunto.GestionarModificacionAsunto((dgAsuntosDia.SelectedItem as Entidades.AsuntoDiario).Numero);
                tbcMainControl.SelectedIndex = 1;
            }
        }

        private void btnUnlog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(ConsultarCierreApp() == true)
            {
                Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }
        private void cboConnect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboConnect.SelectedItem != null) {
                // Convert the selected item on a specific object
                StatusUI newStatus = cboConnect.SelectedItem as StatusUI;
                // Run action related to the status
                runActionStatus(newStatus.Status);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ConsultarCierreApp() == false)
            {
                e.Cancel = true;
            }
        }
        #endregion

        #region background_workers
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


        #region service_helper_methods
        /// <summary>
        /// Informa gráficamente el estado de la conexión de proxy para que el usuario pueda saberlo
        /// </summary>
        /// <param name="proxyStatus"></param>
        public void runActionStatus(AvailabiltyStatus availStatus)
        {
            switch (availStatus) {
                case AvailabiltyStatus.Disconnected:
                    // if the current status is related to connected status
                    if (lstConnectedStatusOnly.Exists((status) => status.Status == currentStatus)) 
                    {
                        // Runs disconnection request 
                        disconnectService();
                    }
                    currentStatus = AvailabiltyStatus.Disconnected;
                    break;
                case AvailabiltyStatus.Connected:
                    // Launch a connection try with async method
                    connectService();                    
                    break;
                case AvailabiltyStatus.ReadyToReceive:
                    break;
                case AvailabiltyStatus.Break:
                    break;
                case AvailabiltyStatus.Bath:
                    break;
                case AvailabiltyStatus.SpecialTask:
                    break;
                case AvailabiltyStatus.Error:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sent petition to the service for connection. The request is asnyc type
        /// </summary>
        private async void connectService()
        {
            try
            {
                // Implements new Logica Operador object
                Logica.Operador logOperator = new Logica.Operador();
                // Gets operator from application variable
                Operador opLogged = App.Current.Properties["user"] as Operador;
                // Gets interface from current
                Entidades.Service.Interface.IServicioCallback callbackForUI = this as Entidades.Service.Interface.IServicioCallback;
                // Calls operation method to connect to the service
                if (await logOperator.ConnectOperatorToService(opLogged, callbackForUI))
                {
                    // Shows connection status on the UI
                    currentStatus = AvailabiltyStatus.ReadyToReceive;
                }
                else
                {
                    // If the connection has been rejected, shows on the UI the results
                    currentStatus = AvailabiltyStatus.Disconnected;
                }
            }
            catch (Exception ex)
            {
                Util.MsgBox.Error("Ha ocurrido un error al procesar la solicitud de conexión : " + ex.Message);
                currentStatus = AvailabiltyStatus.Error;
            }
        }

        private async void disconnectService() {
            try {
                // Generates a new logic operator object
                Logica.Operador logOperator = new Logica.Operador();
                // Encapsulates the operator from application
                Operador operatorLogged = App.Current.Properties["user"] as Operador;
                // Sent request to logic project for desconnect
                await logOperator.DisconnectFromService(operatorLogged);
            }
            catch (Exception ex) {
                Util.MsgBox.Error("Ha ocurrido un error al procesar la desconexión: " + ex.Message);
            }
        }
        #endregion

        #region service_implementation_methods
        public void Mensaje(string message)
        {
            
        }

        public void EnviarAsunto(Entidades.Asunto a)
        {
            
        }

        public void ForceDisconnect()
        {
            
        }

        public void ServiceChangeStatusRequest(AvailabiltyStatus paramNewStatus)
        {
            
        }
        #endregion

        private void btnTestChange_Click(object sender, RoutedEventArgs e) {
            cboConnect.ItemsSource = lstConnectedStatus;
        }
    }
}
