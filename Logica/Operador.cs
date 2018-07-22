using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class Operador
    {
        // Disponemos de un objeto datos operador
        Datos.Operador datOper  = new Datos.Operador();

        /// <summary>
        /// Valida los datos pasados por parametros sobre la base de datos
        /// </summary>
        /// <param name="pUser"></param>
        /// <param name="pPass"></param>
        /// <returns></returns>
        public Entidades.Operador Ingresar(Entidades.Operador pOperador)
        {
            try
            {
                return datOper.Ingresar(pOperador);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
