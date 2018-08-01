using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace UISolucioname
{
    /// <summary>
    /// Interaction logic for frmMainFrame.xaml
    /// </summary>
    public partial class frmMainFrame : Window
    {
        public PaginaAsunto pgAsunto = null;

        public PaginaActuacion pgActuacion = null;

        private ServiceOperation.OperatorService _serviceInterface = null;

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


        public frmMainFrame()
        {
            InitializeComponent();
            ConfigurarCustomWindow();
            SubscribirEventosBotonesDiario();
            ConfiguraComandosVentana();
        }

        public void CargarInformacionUsuario()
        {            
            CargarAsuntosDiarios();
            ConfigurarTabControl();
            EstablecerNombreUser();
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

        private void BtnNuevoAsuntoDiario_Click(object sender, RoutedEventArgs e)
        {
            tbcMainControl.SelectedIndex = 1;
            pagAsunto.GestionarNuevoAsunto();  
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

        private bool? ConsultarCierreApp()
        {
            return Util.MsgBox.Consulta("¿Estás seguro de que deseas salir de la aplicacion?");
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(ConsultarCierreApp() == false)
            {
                e.Cancel = true;
            }
        }
               
        /// <summary>
        /// Interfaz abierta para que el servicio pueda establecer comunicacion con la capa de UI
        /// </summary>
        public void NuevoAsuntoDesdeServicio(Entidades.Asunto pAsuntoRecibido)
        {
            // Se dispone de una accion asincronica para modificar la UI
            Dispatcher.BeginInvoke((Action)(() => 
            {
                Util.MsgBox.Error("Has recibido un nuevo asunto: " + pAsuntoRecibido.Numero);
            }));
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

        private void cboConnect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (optConnected.IsSelected)
            {
                // Controlamos si la variable de interface es nula
                if (_serviceInterface == null)
                {
                    // Inicializamos una nueva instancia del proxy
                    _serviceInterface = new ServiceOperation.OperatorService(this);                    
                }
                // Intentamos conexion con interface
                _serviceInterface.ConnectService();
            }
            else if (optNotConnected.IsSelected)
            {
                if (_serviceInterface != null)
                {
                    _serviceInterface.DisconnectService();                    
                }
            }
        }

        /// <summary>
        /// Informa gráficamente el estado de la conexión de proxy para que el usuario pueda saberlo
        /// </summary>
        /// <param name="proxyStatus"></param>
        public void setConnectionStatus(CommunicationState proxyStatus)
        {
            switch (proxyStatus)
            {
                case CommunicationState.Created:
                    break;
                case CommunicationState.Opening:
                    break;
                case CommunicationState.Opened:
                    imgConnectStatus.Source = new BitmapImage(new Uri(@"/Images/greenstatus.png", UriKind.Relative));
                    break;
                case CommunicationState.Closing:
                    break;
                case CommunicationState.Closed:
                    imgConnectStatus.Source = new BitmapImage(new Uri(@"/Images/idlestatus.png", UriKind.Relative));
                    break;
                case CommunicationState.Faulted:
                    imgConnectStatus.Source = new BitmapImage(new Uri(@"/Images/redstatus.png", UriKind.Relative));
                    break;
                default:
                    break;
            }
        }
    }
}
