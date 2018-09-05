using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.ServiceOperation;

namespace Datos
{
    public class Balance
    {
        /// <summary>
        /// Gets from service a list of balance today
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entidades.Asunto>> GetAsuntoAssignedOfCurrentDay()
        {
            try {
                return await Client.Instance.GetListOfAsuntosAssignedToday();
            }
            catch (Exception ex) {
                throw ex;
            }
        }

    }
}
