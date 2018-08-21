using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIBackoffice
{
    public class OperBackofficeList : ObservableCollection<OperBackoffice>
    {
        public void UpdateTimeLeftOnOperators()
        {

        }

        /// <summary>
        /// Converts from a list of operators to list operator type. Operators validation are on entity. Only validates if the list is not null. 
        /// </summary>
        /// <param name="prmLstOperator"></param>
        public void ClearListAndConvertFromOperator(List<Entidades.Operador> prmLstOperator)
        {
            // Checks if the list is null.Throws an exception on null case
            if (prmLstOperator == null) throw new Exception("La lista de operadores a convertir es nula.");
            // Clears current list
            // Iterates over the list
            // add a new operator converting to OperBackoffice
        }
    }
}
