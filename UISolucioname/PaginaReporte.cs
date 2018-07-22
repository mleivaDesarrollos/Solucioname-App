using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISolucioname
{
    public class PaginaReporte
    {
        private pgeReporte paginaReporte;

        private pgeReporte pagReporte
        {
            get
            {
                if (paginaReporte == null)
                {
                    paginaReporte = new pgeReporte();
                    return paginaReporte;
                }
                else
                {
                    throw new Exception("La instancia de la pagina puede ser consulta una unica vez");
                }
            }
        }

        public pgeReporte TraerInstancia()
        {
            return pagReporte;
        }

    }
}
