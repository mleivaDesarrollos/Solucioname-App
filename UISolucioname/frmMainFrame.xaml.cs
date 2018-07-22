using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UISolucioname
{
    /// <summary>
    /// Interaction logic for frmMainFrame.xaml
    /// </summary>
    public partial class frmMainFrame : Window
    {
        public PaginaAsunto pgAsunto = null;

        public PaginaActuacion pgActuacion = null;       

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
                        Operador = App.Current.Properties["user"] as Entidades.Operador
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
        // Generamos una nueva entidad de tipo nuevo Dia
        //Util.AsuntoDiario nuevoDia = new Util.AsuntoDiario();
        //// Almacenamos el orden máximo
        //int iOrdMax = pEntAsunto.Estados.Select((ea) => ea.Ord).ToArray().Max();
        //// Cargamos los datos del asunto recibido
        //nuevoDia.Numero = pEntAsunto.Numero;
        //// Con el orden maximo buscamos el asunto
        //nuevoDia.UltimoEstado = pEntAsunto.Estados.Find((ea) => ea.Ord == iOrdMax).Tipo.Descripcion;
        //lstAsuntosDiarios.Add(nuevoDia);            
    }
}
