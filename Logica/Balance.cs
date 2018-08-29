using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class Balance
    {
        Datos.Balance datbalance = new Datos.Balance();

        /// <summary>
        /// Calls data layer to get data from the service
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entidades.BalanceHour>> GetBalanceOfTodayOperator()
        {
            try {
                return await datbalance.GetBalanceOfTodayOperator();
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
