using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISolucioname
{

    /// <summary>
    /// Interfaz para la página de listado general
    /// </summary>
    public class PaginaListadoGeneral
    {
        private pgeListadoGeneral pageLstGeneral;

        public pgeListadoGeneral Pagina
        {
            get
            {
                if (pageLstGeneral == null)
                {
                    pageLstGeneral = new pgeListadoGeneral();
                    return pageLstGeneral;
                }
                else
                {
                    throw new Exception("Solo es posible acceder al objeto en primera llamada");
                }
            }
        }

        public void ActualizarListado()
        {
            pageLstGeneral.CargarResumenTicketsMensuales();
        }

        
    }
}
