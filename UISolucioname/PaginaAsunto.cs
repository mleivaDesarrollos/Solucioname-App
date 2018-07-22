using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISolucioname
{
    public class PaginaAsunto
    {
        private pgeAsunto paginaAsunto;

        private bool unicoAcceso = false;

        public PaginaAsunto()
        {
            paginaAsunto = new pgeAsunto();
        }
                
        /// <summary>
        /// Informa a la página de asuntos por una solicitud de ingreso
        /// </summary>
        public void GestionarNuevoAsunto()
        {
            paginaAsunto.GestionarNuevoAsunto();
        }

        /// <summary>
        /// Informa a la página de asuntos por una solicitud de modificación de asunto
        /// </summary>
        /// <param name="pSNumero"></param>
        public bool GestionarModificacionAsunto(String pSNumero)
        {
            return paginaAsunto.GestionarModificacionAsunto(pSNumero);
        }

        /// <summary>
        /// Se dispone de la instancia de pagina si es que ya no fue solicitada. Solo se permite un ingreso para encapsular la implementación e impedir que esta instancia se le puedan modificar sus propiedades desde afuera
        /// </summary>
        /// <returns></returns>
        public pgeAsunto TraerInstanciaPagina()
        {
            if (unicoAcceso)
            {
                return null;
            }
            return paginaAsunto;
        }
    }
}
