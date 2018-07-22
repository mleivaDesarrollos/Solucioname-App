using Microsoft.Reporting.WinForms;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UISolucioname
{
    /// <summary>
    /// Interaction logic for pgeReporte.xaml
    /// </summary>
    public partial class pgeReporte : Page
    {
        private int iMes = DateTime.Now.Month;
        private int iYear = DateTime.Now.Year;

        dsReportesAsuntos dataset = null;

        dsReportesAsuntosTableAdapters.VistaReporteDiarioTableAdapter adaptadorReportes = null;   

        public pgeReporte()
        {
            try
            {
                InitializeComponent();
                InicializarReporte();
                CargarCombosFecha();
            }
            catch (Exception ex)
            {
                Util.MsgBox.Error("Error al inicializar reportes: " + ex.StackTrace);                
            }
        }

        private void CargarCombosFecha()
        {
            // Cargamos el combo de meses con los datos correspondientes
            cboMes.ItemsSource = Util.ComboFecha.CargarMeses();
            cboYear.ItemsSource = Util.ComboFecha.CargarYear();
            // Seteamos los valores de indices
            cboMes.SelectedIndex = iMes - 1;
            cboYear.SelectedValue = iYear;
            // Configuramos los eventos de filtrado
            cboMes.SelectionChanged += FiltrarReporte;
            cboYear.SelectionChanged += FiltrarReporte;
            // Recargamos el listado
            Actualizar();
        }

        private void InicializarReporte()
        {
            ReportDataSource rptDataSource = new ReportDataSource();
            dataset = new dsReportesAsuntos();

            dataset.BeginInit();
            rptDataSource.Name = "DataSetReportes";
            rptDataSource.Value = dataset.VistaReporteDiario;
            rptAsuntosTotales.LocalReport.DataSources.Add(rptDataSource);
            rptAsuntosTotales.LocalReport.ReportEmbeddedResource = "UISolucioname.Reports.rptAsuntosOperador.rdlc";
            adaptadorReportes = new dsReportesAsuntosTableAdapters.VistaReporteDiarioTableAdapter();
            adaptadorReportes.ClearBeforeFill = true;
            dataset.EnforceConstraints = false;
            dataset.EndInit();

            rptAsuntosTotales.RefreshReport();
        }

        private void Actualizar()
        {
            String DatoFecha = (cboMes.SelectedItem as Util.Mes).Descripcion + "-" + cboYear.SelectedValue.ToString();
            ReportParameter paramFecha = new ReportParameter("DatoFecha", DatoFecha);
            rptAsuntosTotales.LocalReport.SetParameters(new ReportParameter[] { paramFecha });
            adaptadorReportes.Fill(dataset.VistaReporteDiario, (App.Current.Properties["user"] as Entidades.Operador).UserName, new DateTime(iYear, iMes, 01), new DateTime(iYear, iMes, DateTime.DaysInMonth(iYear, iMes)));
            rptAsuntosTotales.RefreshReport();
        }

        private void FiltrarReporte(object sender, SelectionChangedEventArgs e)
        {
            if (iYear != 0 && iMes != 0)
            {
                iMes = (cboMes.SelectedItem as Util.Mes).Numero;
                iYear = Convert.ToInt32(cboYear.SelectedItem);
                Actualizar();
            }
        }
    }
}
