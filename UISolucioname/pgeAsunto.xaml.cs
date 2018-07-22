using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Entidades;
using UISolucioname.Util;
using System.Windows.Threading;

namespace UISolucioname
{
    /// <summary>
    /// Interaction logic for pgeAsunto.xaml
    /// </summary>
    public partial class pgeAsunto : Page
    {
        Entidades.Asunto entAsunto = null;

        private bool bActuacionIncluida = false;

        private bool bOperandoEstados = false;

        private bool bModificando = false;
        
        private frmMainFrame VentanaPadre
        {
            get
            {
                foreach (Window Ventana in Application.Current.Windows)
                {
                    if (Ventana is frmMainFrame)
                    {
                        return Ventana as frmMainFrame;
                    }
                }
                throw new Exception("Ventana Principal no encontrada");
            }
        }

        private ToolTip muestreoMensajeValidacion = new ToolTip() { Content = "Existen errores de validación, favor de corroborar.", Visibility = Visibility.Hidden };

        public bool ModificandoAsunto
        {
            get
            {
                return bModificando;
            }
            set
            {
                txtNumAsunto.IsEnabled = !value;
                bModificando = value;
            }
        }

        public bool OperarEstados
        {
            get
            {
                return bOperandoEstados;
            }
            set
            {
                if (value)
                {
                    txtFechaEstadoAsunto.IsEnabled = true;
                    txtHoraEstadoAsunto.IsEnabled = true;
                    txtDetallesEstadoAsunto.IsEnabled = true;
                    btnGuardarEstadoAsunto.IsEnabled = true;
                    btnDescartarEstadoAsunto.IsEnabled = true;
                    cboOrdenEstadoAsunto.IsEnabled = true;
                    cboTipoEstadoAsunto.IsEnabled = true;
                    bOperandoEstados = true;
                }
                else
                {
                    txtFechaEstadoAsunto.IsEnabled = false;
                    txtHoraEstadoAsunto.IsEnabled = false;
                    txtDetallesEstadoAsunto.IsEnabled = false;
                    btnGuardarEstadoAsunto.IsEnabled = false;
                    btnDescartarEstadoAsunto.IsEnabled = false;
                    cboOrdenEstadoAsunto.IsEnabled = false;
                    cboTipoEstadoAsunto.IsEnabled = false;
                    grdChkAct.Visibility = Visibility.Collapsed;
                    bOperandoEstados = false;
                }
            }
        }
        public pgeAsunto()
        {
            InitializeComponent();
            InhabilitarOpcionesAsunto();
            ConfigurarComandosAsunto();
            OperarEstados = false;
        }

        /// <summary>
        ///  En la carga de la nueva ventana se configuran los comandos a utilizar
        /// </summary>
        private void ConfigurarComandosAsunto()
        {
            // Configuración del comando para generar un nuevo estado
            RoutedCommand rcmdNuevoEstadoAsunto = new RoutedCommand();
            // Asociamos el comando CTRL + SHIFT + N
            rcmdNuevoEstadoAsunto.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control | ModifierKeys.Shift));
            // Asociamos la acción a ejecutar bajo el comando
            VentanaPadre.CommandBindings.Add(new CommandBinding(rcmdNuevoEstadoAsunto, btnNuevoEstadoAsunto_Click));

            // Configuramos el comando para guardar un nuevo estado
            RoutedCommand rcmdGuardarEstadoNuevo = new RoutedCommand();
            // Establecemos la configuración de teclas que se utilizará para el comadno
            rcmdGuardarEstadoNuevo.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift));
            // Asociamos el comando con la acción a ejecutar
            VentanaPadre.CommandBindings.Add(new CommandBinding(rcmdGuardarEstadoNuevo, btnGuardarEstadoAsunto_Click));

            // Control de comando para el guardado de asunto
            RoutedCommand rcmdGuardarAsunto = new RoutedCommand();
            // Configuramos las teclas que se utilizaran para el comando de guardado
            rcmdGuardarAsunto.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            // Asociamos el comando a ejecutar
            VentanaPadre.CommandBindings.Add(new CommandBinding(rcmdGuardarAsunto, btnGuardarAsunto_Click));
        }

        private void HabilitarCampos()
        {
            txtDescBreveAsuntoEstado.IsEnabled = true;
            txtNumAsunto.IsEnabled = true;
            chkEstadoReportable.IsEnabled = true;
            btnNuevoEstadoAsunto.IsEnabled = true;
            btnGuardarAsunto.IsEnabled = true;
            dgListadoEstados.IsEnabled = true;
        }

        private void InhabilitarOpcionesAsunto()
        {
            txtDescBreveAsuntoEstado.IsEnabled = false;
            txtNumAsunto.IsEnabled = false;
            chkEstadoReportable.IsEnabled = false;
            btnEliminarEstadoAsunto.IsEnabled = false;
            btnModificarEstadoAsunto.IsEnabled = false;
            btnNuevoEstadoAsunto.IsEnabled = false;
            btnGuardarAsunto.IsEnabled = false;
            dgListadoEstados.IsEnabled = false;
            VentanaPadre.pagActuacion.InhabilitarCampos();
        }
        

        public void GestionarNuevoAsunto()
        {
            if (DescartarModificacionesAsunto())
            {
                HabilitarCampos();
                OperarEstados = true;
                EstablecerTabOrderNuevoAsunto();
                EstadoAsuntoFechaHora(DateTime.Now.AddMinutes(1));
                GenerarNuevaEntidad();
                DefinirContenidoOrden();
                DefinirContenidoEstados();
                SeleccionarPredeterminadoNuevo();
                EnfocarElemento(txtNumAsunto);
            }
        }

        /// <summary>
        /// Interfaz abierta para poder recibir peticiones de modificación
        /// </summary>
        /// <param name="pSNumero"></param>
        public bool GestionarModificacionAsunto(String pSNumero)
        {
            try
            {
                // Si el procedimiento llega a completarse correctamente se procesa el descartado del asunto
                if (DescartarModificacionesAsunto())
                {
                    HabilitarCampos();
                    ModificandoAsunto = true;
                    // Generamos un objeto lógica asunto para procesar la extraccion de información del asunto
                    Logica.Asunto logAsunto = new Logica.Asunto();
                    // Recuperamos la información del asunto y la almacenamos en la entidad
                    entAsunto = new Entidades.Asunto()
                    {
                        Numero = pSNumero,
                        Operador = App.Current.Properties["user"] as Entidades.Operador
                    };
                    entAsunto = logAsunto.TraerAsunto(entAsunto);
                    // Cargamos los campos correspondientes
                    CargarCamposAsunto(entAsunto);
                    // Cargamos los datos correspondientes a la actuación si la entidad viene cargada
                    if (entAsunto.Actuacion != null)
                    {
                        VentanaPadre.pagActuacion.CargarActuacion(entAsunto.Actuacion);
                    }
                    // Devolvemos una respuesta positiva al proceso
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Util.MsgBox.Error("Ha ocurrido un error al intentar recuperar el asunto " + pSNumero + ". Detalle : " + ex.Message);
                return false;
            }
        }

        private void CargarCamposAsunto(Entidades.Asunto entAsunto)
        {
            txtNumAsunto.Text = entAsunto.Numero;
            txtDescBreveAsuntoEstado.Text = entAsunto.DescripcionBreve;
            dgListadoEstados.ItemsSource = entAsunto.Estados;
            chkEstadoReportable.IsChecked = entAsunto.Reportable;
        }

        private void SeleccionarPredeterminadoNuevo()
        {
            // TODO: UI - Low Priority : Es posible especificar bajo archivo de configuración que elemento puede tratarse como predeterminado dependiendo del previo, del inicio, o si esta en el medio de entre dos estados
            // Esto debe poder configurarlo el usuario 
            cboTipoEstadoAsunto.SelectedItem = (cboTipoEstadoAsunto.ItemsSource as List<Entidades.TipoEstado>).Find((tipoEstado) => tipoEstado.Descripcion == "Cerrado");
        }

        /// <summary>
        /// Rellena el combo de orden segun lo que se encuentre en listado de estados
        /// </summary>
        private void DefinirContenidoOrden(int pIOrdParam = 0)
        {
            // Creamos una lista de ordenes
            List<int> lstOrdenesMostrar = new List<int>();

            if (pIOrdParam != 0)
            {
                // Agregamos unicamente el id de la orden a modificar, por decisión, no se permite modificar el número de orden, si se desea hacer algo asi se debe eliminar el registro y generarlo
                lstOrdenesMostrar.Add(pIOrdParam);
                // Recargamos el ItemSource del combo con el listado de ordenes cargados
                cboOrdenEstadoAsunto.ItemsSource = lstOrdenesMostrar;
                // Establecemos el item Seleccionado al primero
                cboOrdenEstadoAsunto.SelectedItem = pIOrdParam;
            }
            else
            {
                // Iniciamos la orden maxima en 1
                var maxOrd = 0;
                // Utilizando LINQ obtenemos el número de orden máximo si es que hay contenido en el listado de estados
                if (entAsunto.Estados.Count != 0)
                {
                    maxOrd = entAsunto.Estados.Select(p => p.Ord).ToArray().Max();
                }
                // Generamos un Array de bool para contener los positivos
                bool[] ordenesExistentes = new bool[maxOrd];
                foreach (Entidades.Estado entEstado in entAsunto.Estados)
                {
                    ordenesExistentes[entEstado.Ord - 1] = true;
                }

                // Agregamos las ordenes faltantes en el proceso
                for (int i = 0; i < ordenesExistentes.Length; i++)
                {
                    if (!ordenesExistentes[i]) lstOrdenesMostrar.Add(i + 1);
                }
                // Agregamos la orden consecuente a las ordenes obtenidas por proceso de detección de faltantes
                lstOrdenesMostrar.Add(maxOrd + 1);
                // Disponemos el ItemSource del Combo de ordenes con la información recolectada
                cboOrdenEstadoAsunto.ItemsSource = lstOrdenesMostrar;
                // Dejamos Seleccionado el ultimo item
                cboOrdenEstadoAsunto.SelectedItem = maxOrd + 1;
            }
        }
        /// <summary>
        /// Define los estados posibles según la orden seleccionada
        /// </summary>
        private bool DefinirContenidoEstados()
        {
            try
            {   
                // Cargamos una lista de tipos de estados
                List<TipoEstado> lstTipoEstado = Logica.TipoEstado.TraerEstadosPermitidos(entAsunto.Estados, (int)cboOrdenEstadoAsunto.SelectedValue);
                if (lstTipoEstado.Count > 0)
                {
                    // Cargamos el ItemSource del combo de estado con el resultado filtrado desde TipoEstado                
                    cboTipoEstadoAsunto.ItemsSource = lstTipoEstado;
                    // Establecemos por defecto la primera opcion que aparezca en el listado cargado
                    cboTipoEstadoAsunto.SelectedIndex = 0;
                    // Se confirma que hay valores definiendo el valor de retorno
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Si no hay una entidad generada, la generamos para que se pueda utilizar durante la aplicación
        private void GenerarNuevaEntidad()
        {
            try
            {
                // Generamos la nueva entidad
                entAsunto = new Entidades.Asunto();
                entAsunto.Operador = App.Current.Properties["user"] as Entidades.Operador;
                entAsunto.Estados = new List<Entidades.Estado>();
                Entidades.Estado entEstadoAsunto = new Entidades.Estado()
                {
                    Detalle = "Nuevo asunto",
                    FechaHora = DateTime.Now,
                    Ord = 1,
                    Tipo = Logica.TipoEstado.TraerEstadoAsuntoInicialNormal()
                };
                entAsunto.Estados.Add(entEstadoAsunto);
                dgListadoEstados.ItemsSource = entAsunto.Estados;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Este método es requerido para poder dar el foco correctamente. El tab control genera un comportamiento incorrecto para los TabControl y la forma de normalizarlo es utilizando el BeginInvoke
        /// </summary>
        private void EnfocarElemento(Control ctrlFoco)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                ctrlFoco.Focus();
            }
            ));
        }

        private void ResetearCamposAsuntos()
        {
            txtNumAsunto.Text = "";
            txtDescBreveAsuntoEstado.Text = "";
            chkEstadoReportable.IsChecked = false;
            dgListadoEstados.ItemsSource = null;
            bModificando = false;
            dgListadoEstados.Items.Refresh();
            entAsunto = null;
            VentanaPadre.pagActuacion.ResetearCampos();
        }

        private void ResetearDetallesEstadoAsunto()
        {
            txtDetallesEstadoAsunto.Text = "";
            txtFechaEstadoAsunto.Text = "";
            txtHoraEstadoAsunto.Text = "";
            cboOrdenEstadoAsunto.ItemsSource = null;
            cboTipoEstadoAsunto.ItemsSource = null;
            cboOrdenEstadoAsunto.Items.Refresh();
            cboTipoEstadoAsunto.Items.Refresh();
        }

        private void EstablecerTabOrderNuevoAsunto()
        {
            KeyboardNavigation.SetTabIndex(txtNumAsunto, 0);
            KeyboardNavigation.SetTabIndex(txtDescBreveAsuntoEstado, 1);
            KeyboardNavigation.SetTabIndex(txtDetallesEstadoAsunto, 2);
            KeyboardNavigation.SetTabIndex(btnGuardarAsunto, 3);
        }

        private void cboOrdenEstadoAsunto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboOrdenEstadoAsunto.ItemsSource != null)
            {
                DefinirContenidoEstados();
            }
        }

        private void btnGuardarEstadoAsunto_Click(object sender, RoutedEventArgs e)
        {
            // Se ejecuta si las comprobaciones son validas
            if (!Validador.EsValido(gpbDetallesEstados)) return;
            // Se encapsula el ID de la orden
            int iOrdOperacion = Convert.ToInt32(cboOrdenEstadoAsunto.SelectedValue);
            // Se comprueba si el número de orden ya esta cargado en el listado de estados (caso modificación)
            if (entAsunto.Estados.Exists((estAs) => estAs.Ord == iOrdOperacion))
            {
                // Se elimina la orden si es que esta cargada
                entAsunto.Estados.RemoveAll((ea) => ea.Ord == iOrdOperacion);
            }
            // Se agrega el nuevo estado en el listado de estados de asuntos
            AgregarEstado();            
            // Se reordena el Grid
            dgListadoEstados.ReordenarDatagrid();
            // Se deshabilita la edición de los campos de detalles
            OperarEstados = false;
            // Se vacía el campo
            ResetearDetallesEstadoAsunto();
            // Si la pagina debe cambiarse para actuacións, se informa en esta capa
            if (bActuacionIncluida)
            {
                VentanaPadre.tbcMainControl.SelectedIndex = 2;
            }
        }

        private void btnGuardarAsunto_Click(object sender, RoutedEventArgs e)
        {
            if (Validador.EsValido(this) && EmpaquetarAsunto())
            {
                try
                {
                    if (ModificandoAsunto)
                    {
                        ModificarAsunto(entAsunto);
                    }
                    else
                    {
                        CargarAsunto(entAsunto);
                    }
                    OperarEstados = false;
                    InhabilitarOpcionesAsunto();
                    ResetearCamposAsuntos();
                    ResetearDetallesEstadoAsunto();      
                    Validador.LimpiarValidaciones(this);
                }
                catch (Exception ex)
                {
                    Util.MsgBox.Error("Ha ocurrido un problema al cargar el asunto: " + ex.Message);
                }
            }
            else
            {
                if(muestreoMensajeValidacion.Visibility == Visibility.Hidden)
                {                    
                    muestreoMensajeValidacion.IsOpen = true;
                    muestreoMensajeValidacion.Visibility = Visibility.Visible;
                    ThreadManage.Tooltip.Limpiar(muestreoMensajeValidacion, this);
                }             
            }         
        }

        /// <summary>
        /// Agrega un estado a la entidad cargada
        /// Fecha de creación : 06/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        private void AgregarEstado()
        {
            // Si las comprobaciones no son exitosas, se cancela el agregado
            if (!ComprobarCamposEstados()) return;
            Entidades.Estado estadoAsunto = new Entidades.Estado()
            {
                FechaHora = Util.Tiempo.ComponerDateTimeEstadoAsunto(txtFechaEstadoAsunto.Text, txtHoraEstadoAsunto.Text),
                Detalle = txtDetallesEstadoAsunto.Text,
                Ord = Convert.ToInt32(cboOrdenEstadoAsunto.SelectedValue),
                Tipo = cboTipoEstadoAsunto.SelectedItem as Entidades.TipoEstado
            };
            if (chkActuacion.IsChecked == true)
            {
                VentanaPadre.pagActuacion.GenerarNuevo();
                bActuacionIncluida = true;
            }
            entAsunto.Estados.Add(estadoAsunto);
        }

        /// <summary>
        /// Empaqueta la entidad para que sea guardada en listado diario o en la base de datos
        /// Fecha de creación : 01/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        private bool EmpaquetarAsunto()
        {
            if (ComprobarCamposAsunto())
            {
                entAsunto.DescripcionBreve = txtDescBreveAsuntoEstado.Text;
                entAsunto.Numero = txtNumAsunto.Text;
                entAsunto.Reportable = chkEstadoReportable.IsChecked == true ? true : false;
                if (ExisteActuacion())
                {
                    entAsunto.Actuacion = VentanaPadre.pagActuacion.TraerEntidadCargada();
                }
                if (OperarEstados)
                {
                    AgregarEstado();
                }
                return true;
            }
            return false;
        }

        private void EstadoAsuntoFechaHora(DateTime dtHorario)
        {
            txtFechaEstadoAsunto.Text = dtHorario.ToString("dd-MM-yyyy");
            txtHoraEstadoAsunto.Text = dtHorario.ToString("hh:mm:ss");
        }
        /// <summary>
        /// Comprueba si los campos cargados fueron llenados correctamente
        /// Fecha de creación : 01/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        private bool ComprobarCamposAsunto()
        {
            // Si existe algún tipo de estado que requiera una actuacón vinculada se solicita comprobación de campos en actuacion
            if (ExisteActuacion())
            {            
                // Comprobamos si los campos cargados de actuación estan correctamente seteados
                if (!VentanaPadre.pagActuacion.ComprobarCamposCargados())
                {
                    return false;
                }
            }
            return true;
            // Gestionar las comprobaciones pertinentes de carga para que los datos sean mas confiables
        }

        private bool ExisteActuacion()
        {
            return entAsunto.Estados.Exists((ea) => ea.Tipo.RequiereActuacion == TipoEstado.SolicitaActuacion.Si || ea.Tipo.RequiereActuacion == TipoEstado.SolicitaActuacion.Obligatoria);
        }

        /// <summary>
        /// Comprobaciones a realizar
        /// 1- Chequear fechas y horas cargadas en las ordenes. Una orden menor no puede tener un horario mayor a la siguiente
        /// 2- Si el listado de ordenes de asunto cargado contiene algun estado que requiera actuacion, las actuaciones deben estar correctamente cargadas para que se pueda procesar el guardado del asunto
        /// 3- La misma actuación debe tener una comprobacion de estados que compruebe lo anterior
        /// </summary>
        /// <returns></returns>
        private bool ComprobarCamposEstados()
        {
            return true;
            // Metodo que gestiona las comprobaciones sobre los campos cargados
        }

        private void dgListadoEstados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgListadoEstados.SelectedItem != null)
            {
                btnModificarEstadoAsunto.IsEnabled = true;
                btnEliminarEstadoAsunto.IsEnabled = true;
                DescartarModificacionesEstado();
                if (!OperarEstados)
                {
                    CargarInformacionEstado();
                }
            }
        }

        private void btnDescartarEstadoAsunto_Click(object sender, RoutedEventArgs e)
        {            
            DescartarModificacionesEstado();
        }

        private void btnNuevoEstadoAsunto_Click(object sender, RoutedEventArgs e)
        {
            DescartarModificacionesEstado();
            DefinirContenidoOrden();
            if (DefinirContenidoEstados())
            {
                EstadoAsuntoFechaHora(DateTime.Now.AddMinutes(1));
                OperarEstados = true;
            }
            else
            {
                ResetearDetallesEstadoAsunto();
                Util.MsgBox.Error("No se disponen de estados para ser anexados al asunto. Favor de verificar el ultimo estado, de ser necesario un nuevo estado eliminelo y generé otro que se adecue a sus necesidades.");         
            }
        }

        private void DescartarModificacionesEstado()
        {

            if (OperarEstados)
            {
                if (MsgBox.Consulta("¿Esta seguro de que quiere descartar los cambios") == true)
                {
                    ResetearDetallesEstadoAsunto();
                    OperarEstados = false;
                }
            }
            else
            {
                ResetearDetallesEstadoAsunto();
                OperarEstados = false;
            }
        }

        private bool DescartarModificacionesAsunto()
        {

            if (entAsunto != null)
            {
                if (MsgBox.Consulta("¿Está seguro que desea descartar las modificaciones sobre el asunto?") == true)
                {
                    ResetearCamposAsuntos();
                    ResetearDetallesEstadoAsunto();
                    OperarEstados = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            OperarEstados = false;
            return true;
        }

        private void btnEliminarEstadoAsunto_Click(object sender, RoutedEventArgs e)
        {
            if (dgListadoEstados.SelectedItem != null)
            {
                // Convertimos el valor seleccionado a tipo Entidad.Estado
                Entidades.Estado entEstado = dgListadoEstados.SelectedItem as Entidades.Estado;
                
                if (MsgBox.Consulta("¿Esta seguro que desea eliminar el estado " + entEstado.Ord.ToString() + "?") == true)
                {
                    if (entEstado.Tipo.RequiereActuacion != TipoEstado.SolicitaActuacion.No)
                    {
                        entAsunto.Actuacion = null;
                        VentanaPadre.pagActuacion.CargarActuacion(null);
                    }
                    entAsunto.Estados.Remove(entEstado);
                    dgListadoEstados.ReordenarDatagrid();
                }
            }
        }

        private void btnModificarEstadoAsunto_Click(object sender, RoutedEventArgs e)
        {
            // Consultamos si hay alguna operación previa ejecutandose
            DescartarModificacionesEstado();
            if (!OperarEstados)
            {
                CargarInformacionEstado();
                OperarEstados = true;
            }
        }

        private void CargarInformacionEstado()
        {
            if (dgListadoEstados.SelectedItem != null)
            {
                Entidades.Estado entEstAsunto = dgListadoEstados.SelectedItem as Entidades.Estado;
                DefinirContenidoOrden(entEstAsunto.Ord);
                EstadoAsuntoFechaHora(entEstAsunto.FechaHora);
                txtDetallesEstadoAsunto.Text = (entEstAsunto.Detalle);
            }
        }

        /// <summary>
        /// Carga un asunto en el listado de asuntos
        /// </summary>
        /// <param name="pEntAsunto"></param>
        public void CargarAsunto(Entidades.Asunto pEntAsunto)
        {
            try
            {
                // Generamos un objeto de logica asunto
                Logica.Asunto logAsunto = new Logica.Asunto();
                // Procesamos el pedido de alta
                logAsunto.Agregar(pEntAsunto);
                // Mostramos un mensaje de exito en solicitud
                Util.MsgBox.Error("Se ha cargado el asunto de manera correcta");
                // Actualizamos el listado de asuntos cargados
                VentanaPadre.pagListadogeneral.ActualizarListado();
                // Actualizamos el listado de asuntos diarios
                VentanaPadre.CargarAsuntosDiarios();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Modifica un asunto en el listado de asuntos
        /// </summary>
        /// <param name="entAsunto"></param>
        public void ModificarAsunto(Entidades.Asunto entAsunto)
        {
            try
            {
                // Generamos un objeto de lógica de asunto
                Logica.Asunto logAsunto = new Logica.Asunto();
                // Modificamos el asunto procesado
                logAsunto.Modificar(entAsunto);
                // Enviamos el mensaje de actualización
                Util.MsgBox.Error("Se ha actualizado un asunto de manera correcta.");
                // Actualizamos el listado de asuntos
                VentanaPadre.pagListadogeneral.ActualizarListado();
                // Actualizamos los asuntos diarios
                VentanaPadre.CargarAsuntosDiarios();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (chkActuacion.IsEnabled && grdChkAct.Visibility == Visibility.Visible)
            {
                chkActuacion.IsChecked = chkActuacion.IsChecked == true ? false : true;
            }
            
        }

        private void cboTipoEstadoAsunto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Se comprueba si el tipo de estado es nulo
            if (cboTipoEstadoAsunto.SelectedItem != null)
            {
                Entidades.TipoEstado tipoEstado = cboTipoEstadoAsunto.SelectedItem as Entidades.TipoEstado;
                switch (tipoEstado.RequiereActuacion)
                {
                    case TipoEstado.SolicitaActuacion.No:
                        grdChkAct.Visibility = Visibility.Collapsed;
                        chkActuacion.IsChecked = false;
                        break;
                    case TipoEstado.SolicitaActuacion.Si:
                        grdChkAct.Visibility = Visibility.Visible;
                        chkActuacion.IsChecked = false;
                        chkActuacion.IsEnabled = true;
                        break;
                    case TipoEstado.SolicitaActuacion.SiPredeterminado:
                        grdChkAct.Visibility = Visibility.Visible;
                        chkActuacion.IsEnabled = true;
                        chkActuacion.IsChecked = true;
                        break;
                    case TipoEstado.SolicitaActuacion.Obligatoria:
                        grdChkAct.Visibility = Visibility.Visible;
                        chkActuacion.IsChecked = true;
                        chkActuacion.IsEnabled = false;
                        break;
                    default:
                        break;
                }
                // Obtenemos la validación de estado
                Binding bind = BindingOperations.GetBinding(txtDetallesEstadoAsunto, TextBox.TextProperty);
                // Recorremos los validadores cargados
                foreach (var validation in bind.ValidationRules)
                {
                    // Si el validador es de tipo ValidarNulo
                    if (((ValidarNulo)validation) != null)
                    {
                        // Establecemos la opción de requiere en el valor que venga cargado requiere detalle
                        ((ValidarNulo)validation).Requiere = tipoEstado.RequiereDetalle;
                    }
                }
            }
        }
        
        private void txtNumAsunto_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Si se comprueba que el número de asunto tiene el largo maximo del textbox (indica que esta completo)
            if (txtNumAsunto.Text.Length == txtNumAsunto.MaxLength && txtNumAsunto.IsEnabled)
            {
                // Comprobamos que ls validaciones sean correctas para poder avanzar con la consulta a la base de datos
                if (Validador.EsValido(txtNumAsunto))
                {
                    try
                    {
                        // Generamos un objeto de logica asunto para averiguar si el asunto está cargado
                        Logica.Asunto logAsunto = new Logica.Asunto();
                        // Generamos un asunto que contenga solo este número cargado
                        Entidades.Asunto entAsunto = new Entidades.Asunto() { Numero = txtNumAsunto.Text, Operador = App.Current.Properties["user"] as Entidades.Operador };
                        // Ejecutamos la consulta a la base de datos con el valor del campo cargado
                        if (logAsunto.ExisteAsunto(entAsunto))
                        {
                            if (Util.MsgBox.Consulta("El asunto " + txtNumAsunto.Text + " se encuentra cargado. ¿Deseas traer el asunto guardado?") == true)
                            {
                                // Traemos el asunto y todos sus campos en la página de asuntos.
                                GestionarModificacionAsunto(txtNumAsunto.Text);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Util.MsgBox.Error("Ha ocurrido un error al traer la información de la base de datos: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Si se realiza un doble click sobre un estado y se confirma el descartado se carga el asunto para que sea modificado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgListadoEstados_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                // Consultamos si hay alguna operación previa ejecutandose
                DescartarModificacionesEstado();
                if (!OperarEstados)
                {
                    CargarInformacionEstado();
                    OperarEstados = true;
                }
            }
        }
    }
}
