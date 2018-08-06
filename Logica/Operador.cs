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

        /// <summary>
        /// Procesa una solicitud de conexión. Si el proceso es correcto la entidad operador viene completa
        /// </summary>
        /// <param name="pOperator"></param>
        /// <returns></returns>
        public async Task<Entidades.Operador> ConnectBackoffice(Entidades.Operador pOperator)
        {
            try
            {
                return await datOper.LogOnServiceBackoffice(pOperator);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Call service methods to get a list with service logged operator
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entidades.Operador>> GetFullOperatorList()
        {
            try
            {
                return await datOper.GetFullOperatorList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
