using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
using UISolucioname.Util;

namespace UISolucioname
{

    /// <summary>
    /// Interaction logic for pgeActuacion.xaml
    /// </summary>
    public partial class pgeActuacion : Page
    {
        public pgeActuacion()
        {
            InitializeComponent();
            operarDetalles = false;
            operarEstados = false;
            this.DataContext = this;
        }

        private List<Entidades.GrupoResolutor> lstGruposResolutoresTotales;

        private bool _bOperDetalles = false;

        private bool _bOperEstados = false;

        private bool _modificandoEstados = false;

        private Entidades.Actuacion entActuacion = null;

        public Entidades.Actuacion ActCargada
        {
            get
            {
                if (entActuacion != null)
                {
                    entActuacion.Tipo = cboTipoActuacion.SelectedItem as Entidades.ActuacionTipo;
                    entActuacion.RemedyRelacionado = txtRemedy.Text;
                    entActuacion.Numero = txtNumActuacion.Text;
                    entActuacion.Grupo = cboGrupos.SelectedItem as Entidades.GrupoResolutor;
                }
                return entActuacion;
            }
            set
            {
                if (value != null)
                {
                    if (cboTipoActuacion.ItemsSource == null)
                    {
                        CargarCamposActuacion();
                    }
                    txtNumActuacion.Text = value.Numero;
                    txtRemedy.Text = value.RemedyRelacionado;
                    cboTipoActuacion.SelectedItem = (cboTipoActuacion.ItemsSource as List<Entidades.ActuacionTipo>).FindAll((x) => x.Id == value.Tipo.Id).First();
                    cboGrupos.SelectedItem = (cboGrupos.ItemsSource as List<Entidades.GrupoResolutor>).FindAll((x) => x.Id == value.Grupo.Id).First();
                    dgListadoActuacionEstados.ItemsSource = value.Estados;
                    operarDetalles = true;
                    txtNumActuacion.IsEnabled = false;
                }
                else
                {
                    ResetearCamposActuacion();
                    operarEstados = false;
                    operarDetalles = false;
                }
                entActuacion = value;
            }
        }
        
        /// <summary>
        /// Controla la disponiblidad de los controles de detalles de actuación
        /// Fecha de creación : 14/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        private bool operarDetalles
        {
            get
            {
                return _bOperDetalles;
            }
            set
            {
                if (value)
                {
                    txtNumActuacion.IsEnabled = true;
                    txtRemedy.IsEnabled = true;
                    cboTipoActuacion.IsEnabled = true;
                    cboGrupos.IsEnabled = true;
                    btnNuevoEstadoActuacion.IsEnabled = true;
                }
                else
                {
                    txtNumActuacion.IsEnabled = false;
                    txtRemedy.IsEnabled = false;
                    cboTipoActuacion.IsEnabled = false;
                    cboGrupos.IsEnabled = false;
                    btnNuevoEstadoActuacion.IsEnabled = false;
                }
                _bOperDetalles = value;
            }
        }

        /// <summary>
        /// Controla la disponibilidad de los controles de estados
        /// </summary>
        private bool operarEstados
        {
            get
            {
                return _bOperEstados;
            }
            set
            {
                if (value)
                {
                    txtFechaEstadoActuacion.IsEnabled = true;
                    txtHoraEstadoActuacion.IsEnabled = true;
                    cboOrdEstadoActuacion.IsEnabled = true;
                    cboTipoEstadoActuacion.IsEnabled = true;
                    txtDetalleEstadoActuacion.IsEnabled = true;
                    btnDescartarCambios.IsEnabled = true;
                    btnGuardarCambios.IsEnabled = true;
                }
                else
                {
                    txtFechaEstadoActuacion.IsEnabled = false;
                    txtHoraEstadoActuacion.IsEnabled = false;
                    cboOrdEstadoActuacion.IsEnabled = false;
                    cboTipoEstadoActuacion.IsEnabled = false;
                    txtDetalleEstadoActuacion.IsEnabled = false;
                    btnModificarEstadoActuacion.IsEnabled = false;
                    btnEliminarEstadoActuacion.IsEnabled = false;
                    btnDescartarCambios.IsEnabled = false;
                    btnGuardarCambios.IsEnabled = false;                    
                    _modificandoEstados = false;
                }
                _bOperEstados = value;
            }
        }

        public void GenerarNuevo()
        {
            operarDetalles = true;
            CargarCamposActuacion();
            InicializarActuacion();
        }        

        private void InicializarActuacion()
        {
            // Generamos una nueva entidad actuación
            entActuacion = new Entidades.Actuacion();
            // Cargamos los datos correspondientes de la actuación
            entActuacion.Operador = App.Current.Properties["user"] as Entidades.Operador;
            // Generamos una nueva lista de entidades estado
            List<Entidades.Estado> lstEntEstado = new List<Entidades.Estado>();
            // Generamos un estado y lo agregamos al listado generado
            Entidades.Estado entEstado = new Entidades.Estado()
            {
                Ord = 1,
                Detalle = "Nueva actuacion",
                Tipo = Logica.TipoEstado.TraerEstadoActuacionInicialNormal(),
                FechaHora = DateTime.Now
            };
            // Agregamos el estado nuevo al listado de estados
            lstEntEstado.Add(entEstado);
            // Establecemos la lista como miembro de la entidad actuación
            entActuacion.Estados = lstEntEstado;
            // Vinculamos el listado de estados al DataGrid
            dgListadoActuacionEstados.ItemsSource = entActuacion.Estados;
            // Activamos la modificación de la nueva actuación en la interfaz gráfica
            ModificarEstado(entEstado);
        }        

        /// <summary>
        /// Inhabilita todos los campos para que no puedan ser modificados
        /// </summary>
        public void InhabilitarTodosCampos()
        {
            operarDetalles = false;
            operarEstados = false;
        }

        /// <summary>
        /// Resetea a los valores por defecto de los campos genéricos
        /// </summary>
        public void ResetearCamposActuacion()
        {
            txtNumActuacion.Text = "";
            txtRemedy.Text = "";
            cboGrupos.ItemsSource = null;
            cboTipoActuacion.ItemsSource = null;
            dgListadoActuacionEstados.ItemsSource = null;
            entActuacion = null;
        }

        /// <summary>
        /// Resetea a los valores defecto de los campos de estado
        /// </summary>
        public void ResetearCamposEstadoActuacion()
        {
            txtFechaEstadoActuacion.Text = "";
            txtHoraEstadoActuacion.Text = "";
            txtDetalleEstadoActuacion.Text = "";
            cboOrdEstadoActuacion.ItemsSource = null;
            cboTipoEstadoActuacion.ItemsSource = null;
        }

        /// <summary>
        /// Controla la disponibilidad de botones de edicion y eliminacion de estados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgListadoActuacionEstados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgListadoActuacionEstados.SelectedItem != null)
            {
                btnEliminarEstadoActuacion.IsEnabled = true;
                btnModificarEstadoActuacion.IsEnabled = true;
            }
            else
            {
                btnEliminarEstadoActuacion.IsEnabled = false;
                btnModificarEstadoActuacion.IsEnabled = false;
            }
        }

        private void btnModificarEstadoActuacion_Click(object sender, RoutedEventArgs e)
        {
            // Averiguamos si hay algun item seleccionado en el DataGrid
            if (dgListadoActuacionEstados.SelectedItem != null)
            {
                // Convertimos el item Selecionado en estado
                Entidades.Estado entEstado = dgListadoActuacionEstados.SelectedItem as Entidades.Estado;
            }
        }

        private void btnEliminarEstadoActuacion_Click(object sender, RoutedEventArgs e)
        {
            // Consultamos si el objeto es nulo
            if (dgListadoActuacionEstados.SelectedItem != null)
            {
                // Convertimos la entidad seleccionada al tipo de objeto tipo estado
                Entidades.Estado entEstado = dgListadoActuacionEstados.SelectedItem as Entidades.Estado;
                // Solicitamos a la capa UI que procese la modificación
                EliminarEntidadEstado(entEstado);
            }
        }

        private void btnNuevoEstadoActuacion_Click(object sender, RoutedEventArgs e)
        {
            if (DescartarModificacionesEstado())
            {
                // Activamos los campos de detalles para gestionar la edicion
                operarEstados = true;
                // Configuramos la fecha y hora
                EstablecerFecha(DateTime.Now.AddMinutes(1));
                // Definimos el contenido del combo de orden
                DefinirContenidoOrden();
                // Luego de cargar el contenido de orden se carga el combo de tipo de estados
                DefinirContenidoEstados();
            }
        }

        /// <summary>
        /// Establece fecha y hora en los campos según lo trasladado por parametro
        /// </summary>
        /// <param name="dtHorario"></param>
        private void EstablecerFecha(DateTime dtHorario)
        {
            txtFechaEstadoActuacion.Text = dtHorario.ToString("dd-MM-yyyy");
            txtHoraEstadoActuacion.Text = dtHorario.ToString("hh:mm:ss");
        }

        /// <summary>
        /// Prepara la interfaz gráfica para modificar un estado        /// Fecha de creación : 17/07/2018        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pEntEstado"></param>
        private void EliminarEntidadEstado(Entidades.Estado pEntEstado)
        {
            // Averiguamos si es nulo
            if (pEntEstado != null)
            {
                // Si el estado obtenido es nulo realizamos la consulta a la capa de datos
                if (Util.MsgBox.Consulta("¿Esta seguro de que desea eliminar la orden " + pEntEstado.Ord + "?") == true)
                {
                    // Removemos la actuación
                    entActuacion.Estados.Remove(pEntEstado);
                    // Reseteamos los campos
                    ResetearCamposEstadoActuacion();
                    // Desactivamos la edicion de campos actuación
                    operarEstados = false;
                    // Reordenamos el DataGrid
                    dgListadoActuacionEstados.ReordenarDatagrid();
                }
            }
        }

        /// <summary>
        /// Procesa en la capa de presentación el pedido de modificación de un estado actuación        /// Fecha de creación : 17/07/2018        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pEntEstado"></param>
        private void ModificarEstado(Entidades.Estado pEntEstado)
        {

            // Si se logra localizar el item
            if (pEntEstado != null)
            {
                // Descartamos las modificaciones 
                if (DescartarModificacionesEstado())
                {
                    txtFechaEstadoActuacion.Text = pEntEstado.FechaHora.ToString("dd-MM-yyyy");
                    txtHoraEstadoActuacion.Text = pEntEstado.FechaHora.ToString("hh:mm:ss");
                    txtDetalleEstadoActuacion.Text = pEntEstado.Detalle;
                    DefinirContenidoOrden(pEntEstado.Ord);
                    DefinirContenidoEstados();
                    operarEstados = true;
                    _modificandoEstados = true;
                }
            }
        }

        /// <summary>
        /// Se procede a la carga de campos genéricos de actuación
        /// Fecha de creación : 15/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        private void CargarCamposActuacion()
        {
            try
            {
                // Generamos un objeto de logicas necesarias para cargar los combos
                Logica.ActuacionTipo logActTipo = new Logica.ActuacionTipo();
                Logica.GrupoResolutor logGrpRes = new Logica.GrupoResolutor();
                // Cargamos los combos
                lstGruposResolutoresTotales = logGrpRes.TraerGrupos();
                ActualizarOrigenCombo(cboTipoActuacion, logActTipo.TraerTipos());
            }
            catch (Exception ex)
            {
                Util.MsgBox.Error("Ha ocurrido un error al cargar los campos de actuación: " + ex.Message);
            }
        }

        private void ActualizarOrigenCombo(ComboBox ctrl, IEnumerable origenDatos)
        {
            // Actualizamos el combo
            ctrl.ItemsSource = origenDatos;
            // Establecemos como primera opción el indice del combo
            ctrl.SelectedIndex = 0;
        }

        private void cboTipoActuacion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {     
            // Si el listado de grupos resolutores se encuentra cargado                   
            if (lstGruposResolutoresTotales != null && cboTipoActuacion.ItemsSource != null)
            {
                // Se llena el combo de grupos con el filtro
                ActualizarOrigenCombo(cboGrupos, lstGruposResolutoresTotales.FindAll((x) => x.Tipo.Id == (cboTipoActuacion.SelectedItem as Entidades.ActuacionTipo).Id));
            }
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
                cboOrdEstadoActuacion.ItemsSource = lstOrdenesMostrar;
                // Establecemos el item Seleccionado al primero
                cboOrdEstadoActuacion.SelectedItem = pIOrdParam;
            }
            else
            {
                int maxOrd = 0;
                if (!(entActuacion.Estados.Count == 0))
                {
                    // Utilizando LINQ obtenemos el número de orden máximo
                    maxOrd = entActuacion.Estados.Select(p => p.Ord).ToArray().Max();
                    // Generamos un Array de bool para contener los positivos
                    bool[] ordenesExistentes = new bool[maxOrd];
                    foreach (Entidades.Estado entEstado in entActuacion.Estados)
                    {
                        ordenesExistentes[entEstado.Ord - 1] = true;
                    }

                    // Agregamos las ordenes faltantes en el proceso
                    for (int i = 0; i < ordenesExistentes.Length; i++)
                    {
                        if (!ordenesExistentes[i]) lstOrdenesMostrar.Add(i + 1);
                    }
                }
                // Agregamos la orden consecuente a las ordenes obtenidas por proceso de detección de faltantes
                lstOrdenesMostrar.Add(maxOrd + 1);
                // Disponemos el ItemSource del Combo de ordenes con la información recolectada
                cboOrdEstadoActuacion.ItemsSource = lstOrdenesMostrar;
                // Dejamos Seleccionado el ultimo item
                cboOrdEstadoActuacion.SelectedItem = maxOrd + 1;
            }
        }


        /// <summary>
        /// Define los estados posibles según la orden seleccionada
        /// </summary>
        private void DefinirContenidoEstados()
        {
            try
            {
                // Cargamos el ItemSource del combo de estado con el resultado filtrado desde TipoEstado                
                cboTipoEstadoActuacion.ItemsSource = Logica.TipoEstado.TraerEstadosPermitidos(entActuacion.Estados, (int)cboOrdEstadoActuacion.SelectedValue, true);
                // Establecemos por defecto la primera opcion que aparezca en el listado cargado
                cboTipoEstadoActuacion.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnDescartarCambios_Click(object sender, RoutedEventArgs e)
        {
            DescartarModificacionesEstado();
        }

        private void btnGuardarCambios_Click(object sender, RoutedEventArgs e)
        {
            // Se continua si la comprobación se cumple
            if (ComprobarCargaEstado())
            {

                if (_modificandoEstados)
                {
                    entActuacion.Estados.RemoveAll((ea) => ea.Ord == Convert.ToInt32(cboOrdEstadoActuacion.SelectedItem));
                }
                // Generamos una nueva entidad de estado para almacenar en la colección
                Entidades.Estado entEstado = new Entidades.Estado();
                // Cargamos la entidad con los datos generados
                entEstado.Detalle = txtDetalleEstadoActuacion.Text;
                entEstado.FechaHora = Util.Tiempo.ComponerDateTimeEstadoAsunto(txtFechaEstadoActuacion.Text, txtHoraEstadoActuacion.Text);
                entEstado.Ord = Convert.ToInt32(cboOrdEstadoActuacion.SelectedItem);
                entEstado.Tipo = (Entidades.TipoEstado)cboTipoEstadoActuacion.SelectedItem;
                // Agregamos el estado en la actuación
                entActuacion.Estados.Add(entEstado);
                // Actualizamos el listado de items
                dgListadoActuacionEstados.ReordenarDatagrid();
                // Reseteamos los campos
                ResetearCamposEstadoActuacion();
                // Desactivamos la operación de estados
                operarEstados = false;
            }
            
        }

        /// <summary>
        /// En este método se ejecutaran todas las comprobaciones pertinentes al estado gestionado
        /// </summary>
        /// <returns></returns>
        private bool ComprobarCargaEstado()
        {
            return true;
        }

        /// <summary>
        /// Bajo este método se comprobarán los campos totales de la actuación para constatar de que todo este cargado correctamente
        /// </summary>
        /// <returns></returns>
        public bool ComprobarCargaActuacion()
        {
            // Valida todas las reglas de validación cargadas en la página
            if (Validador.EsValido(this))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Descarta las modificaciones realizadas sobre los estados
        /// </summary>
        private bool DescartarModificacionesEstado()
        {
            if (operarEstados)
            {
                if (MsgBox.Consulta("¿Está seguro de que desea descartar las modificaciones realizadas sobre el estado?") == true)
                {
                    ResetearCamposEstadoActuacion();
                    operarEstados = false;
                    return true;
                }
                return false;
            }
            else
            {
                ResetearCamposEstadoActuacion();
                operarEstados = false;
                return true;
            }
        }
    }

}
