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

namespace UISolucioname
{
    /// <summary>
    /// Interaction logic for wpfReportePrueba.xaml
    /// </summary>
    public partial class wpfReportePrueba : Window
    {
        private bool isReportLoaded = false;

        public wpfReportePrueba()
        {
            InitializeComponent();
            rptPruebaReporte.Load += RptPruebaReporte_Load;
        }

        private void RptPruebaReporte_Load(object sender, EventArgs e)
        {
            if (!isReportLoaded)
            {
                Microsoft.Reporting.WinForms.ReportDataSource rptDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                dsReportesAsuntos dataSet = new dsReportesAsuntos();

                dataSet.BeginInit();

                rptDataSource.Name = "DataSetReportes";
                rptDataSource.Value = dataSet.AsuntosPorOperadorSegunMes;
                rptPruebaReporte.LocalReport.DataSources.Add(rptDataSource);
                rptPruebaReporte.LocalReport.ReportEmbeddedResource = "UISolucioname.Reports.rptAsuntosOperador.rdlc";

                dsReportesAsuntosTableAdapters.AsuntosPorOperadorSegunMesTableAdapter adaptadorTabla = new dsReportesAsuntosTableAdapters.AsuntosPorOperadorSegunMesTableAdapter();
                adaptadorTabla.Fill(dataSet.AsuntosPorOperadorSegunMes, "mleiva", new DateTime(2018, 06, 01), new DateTime(2018, 06, 30));
                
                dataSet.EndInit();

                rptPruebaReporte.RefreshReport();
                isReportLoaded = true;
            }
        }
    }
}
