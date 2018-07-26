using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for pgeListadoGeneral.xaml
    /// </summary>
    public partial class pgeListadoGeneral : Page
    {

        private int _iYear = DateTime.Now.Year;

        private int _iMonth = DateTime.Now.Month;

        List<Mes> lstTotalMes = new List<Mes>();

        List<int> lstTotalYear = new List<int>();

        DataView dvListadoAsuntos = null;

        public pgeListadoGeneral()
        {
            InitializeComponent();
            CargarResumenTicketsMensuales();
            ConfigurarComboFecha();
            cboFiltro.SelectionChanged += CboFiltro_SelectionChanged;
        }
        

        private void ConfigurarComboFecha()
        {
            // Cargamos el combo de meses
            lstTotalMes = ComboFecha.CargarMeses();
            // Cargamos el ItemSource
            cboFiltroMes.ItemsSource = lstTotalMes;
            try
            {
                lstTotalYear = ComboFecha.CargarYear();
                // Cargamos el combo de años
                cboFiltroAno.ItemsSource = lstTotalYear;
                // Establecemos los indices iniciales de los filtros
                cboFiltroMes.SelectedIndex = DateTime.Now.Month - 1;
                cboFiltroAno.SelectedValue = DateTime.Now.Year;
                // Cargamos los campos con los datos filtrados
                CargarResumenTicketsMensuales();
                // Generamos los eventos cambio de combo
                cboFiltroMes.SelectionChanged += FiltrarPorFecha;
                cboFiltroAno.SelectionChanged += FiltrarPorFecha;
            }
            catch (Exception ex)
            {
                Util.MsgBox.Error("Ha ocurrido un error al cargar los combos de mes : " + ex.Message);
            }
        }

        private void FiltrarPorFecha(object sender, SelectionChangedEventArgs e)
        {
            if (stpPorFecha.Visibility == Visibility.Visible && cboFiltroMes.ItemsSource != null && cboFiltroAno.ItemsSource != null)
            {
                _iMonth = Convert.ToInt32(cboFiltroMes.SelectedValue);
                _iYear = Convert.ToInt32(cboFiltroAno.SelectedValue);
                CargarResumenTicketsMensuales();
            }
        }

        private void CboFiltro_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboFiltroPorFecha.IsSelected)
            {
                stpPorFecha.Visibility = Visibility.Visible;
                stpFiltroNumero.Visibility = Visibility.Hidden;
            }
            else if(cboFiltroPorAsunto.IsSelected)
            {
                stpPorFecha.Visibility = Visibility.Collapsed;
                stpFiltroNumero.Visibility = Visibility.Visible;                
            }
        }

        public void CargarResumenTicketsMensuales(DataView dvLoad = null)
        {
            try
            {
                if (dvLoad == null)
                {
                    // Generamos un nuevo objeto logica de estado de asuntos
                    Logica.Asunto logEAsunto = new Logica.Asunto();
                    // Cargamos el DataView con la información recolectada desde la base de                 
                    dvLoad = logEAsunto.TraerPorPeriodo(_iMonth, _iYear, App.Current.Properties["user"] as Entidades.Operador).AsDataView();
                }
                dvListadoAsuntos = dvLoad;
                // A modo de ejemplo practico, se utiliza el periodo actual y el usuario presente
                dgListadoGeneral.ItemsSource = dvListadoAsuntos;
                // Reordenamos el DataGrid segun fecha
                dgListadoGeneral.ReordenarDatagrid(3);
            }
            catch (Exception ex)
            {
                Util.MsgBox.Error(ex.Message);
            }
        }

        /// <summary>
        /// Controla si al hacer hacer click en modificar del menu contextual hay algún elemento seleccionado y elimina llegado el caso de confirmación
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniEliminarAsunto_Click(object sender, RoutedEventArgs e)
        {
            if (dgListadoGeneral.SelectedItem != null)
            {
                if (Util.MsgBox.Consulta("¿Estás seguro de que deseas eliminar el asunto " + (dgListadoGeneral.SelectedItem as DataRowView)["num_asunto"].ToString()) == true)
                {
                    try
                    {
                        // Generamos un objeto de lógica para procesar la solicitud de baja
                        Logica.Asunto logAsunto = new Logica.Asunto();
                        // Generamos un nueva entidad de asunto que será cargada con los datos requieridos para que la baja se procese
                        Entidades.Asunto entAsunto = new Entidades.Asunto() {
                            Numero = (dgListadoGeneral.SelectedItem as DataRowView)["num_asunto"].ToString(),
                            Oper = App.Current.Properties["user"] as Entidades.Operador
                        };
                        // Procesamos el pedido de baja utilizando el objeto de lógica
                        logAsunto.Eliminar(entAsunto);
                        // Informamos que la baja fue procesada
                        Util.MsgBox.Error("Se ha procesado la baja del asunto de manera correcta.");
                        // Recargamos el resumen de tickets mensuales
                        CargarResumenTicketsMensuales();
                        // Disponemos del objeto de ventana principal
                        frmMainFrame ventPrincip = App.Current.MainWindow as frmMainFrame;
                        // Actualizamos los asuntos diarios cargados
                        ventPrincip.CargarAsuntosDiarios();
                    }
                    catch (Exception ex)
                    {
                        Util.MsgBox.Error("No se ha podido completar la baja del asunto : " + ex.Message);
                    }
                }
            }
        }

        private void mniModificarAsunto_Click(object sender, RoutedEventArgs e)
        {
            ModificarAsuntoSeleccionado();
        }

        /// <summary>
        ///  Solicita a la capa de presentación que cargue los datos de un asunto para que puedan ser modificados en la pestaño de asuntos
        /// Fecha de creación : 17/07/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        private void ModificarAsuntoSeleccionado()
        {
            if (dgListadoGeneral.SelectedItem != null)
            {
                // Corroboramos que este correctamente cargado la variable MainWindow
                if (App.Current.MainWindow != null)
                {
                    // Disponemos del objeto de ventana principal
                    frmMainFrame ventPrincip = App.Current.MainWindow as frmMainFrame;
                    // Convertimos la variable hacia el tipo de ventana principal, para asi disponer de sus métodos
                    PaginaAsunto pgAsunto = ventPrincip.pagAsunto;
                    // Convertimos el número de asunto a String
                    String sNumAsunto = (dgListadoGeneral.SelectedItem as DataRowView)["num_asunto"].ToString();
                    // Si se completa la carga de campos de manera correcta, cambiamos la pestaña a la correspondiente
                    if (pgAsunto.GestionarModificacionAsunto(sNumAsunto))
                    {
                        // Cambiamos la ficha hacia la de asuntos para poder continuar con la edición
                        ventPrincip.tbcMainControl.SelectedIndex = 1;
                    }
                }
            }
        }

        private void FiltrarAsuntoListadoActual(object sender, TextChangedEventArgs e)
        {
            if (dvListadoAsuntos != null)
            {
                dvListadoAsuntos.RowFilter = "num_asunto LIKE '3000" + txtFiltroAsunto.Text + "%'";
            }            
        }

        private void FiltrarListadoGeneral(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Generamos un objeto de lógica disponible para utilzarlo en el procedimiento
                Logica.Asunto logAsunto = new Logica.Asunto();
                // Traemos el listado de asuntos filtrado según el dato cargado en el TextBox
                CargarResumenTicketsMensuales(logAsunto.TraerAsuntosFiltradoPorNumero(App.Current.Properties["user"] as Entidades.Operador, txtFiltroAsunto.Text).AsDataView());                
            }
            catch (Exception ex)
            {
                Util.MsgBox.Error(ex.Message);
            }
        }

        private void chkFiltrarListadoActual_Checked(object sender, RoutedEventArgs e)
        {
            txtFiltroAsunto.TextChanged += FiltrarAsuntoListadoActual;
            txtFiltroAsunto.TextChanged -= FiltrarListadoGeneral;
            txtFiltroAsunto.TextChanged -= FiltrarListadoGeneral;
        }

        private void chkFiltrarListadoActual_Unchecked(object sender, RoutedEventArgs e)
        {
            txtFiltroAsunto.TextChanged += FiltrarListadoGeneral;
            txtFiltroAsunto.TextChanged -= FiltrarAsuntoListadoActual;
        }

        private void stpPorFecha_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (stpPorFecha.Visibility == Visibility.Visible)
            {
                CargarResumenTicketsMensuales();
            }
        }

        /// <summary>
        /// Controla el evento de doble click en una celda y carga un asunto llegado el caso de que se cumplan las condiciones
        /// Fecha de creación : 17/07/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgListadoGeneral_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ModificarAsuntoSeleccionado();
            }
        }
    }
}
