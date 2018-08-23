using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Errors;

namespace UIBackoffice
{
    public class OperBackofficeList : ObservableCollection<OperBackoffice>
    {
        /// <summary>
        /// Process time left update on all operator in the list
        /// </summary>
        public void UpdateTimeLeftOnOperators()
        {
            // because this is a public interface, we need to check if the object is initialized
            if (this == null) Assert.Throw(new Exception("El listado no ha sido inicializado. No se puede actualizar tiempo restante"));
            // iterates over all elements on the list
            foreach (var operBack in this) {
                // update operator with specific methods
                operBack.CalculateTimeLeft();
            }
        }

        /// <summary>
        /// Converts from a list of operators to list operator type. Operators validation are on entity. Only validates if the list is not null. 
        /// </summary>
        /// <param name="prmLstOperator"></param>
        public void ClearListAndConvertFromOperator(List<Entidades.Operador> prmLstOperator)
        {
            // Checks if the list is null.Throws an exception on null case
            if (prmLstOperator == null) Assert.Throw(new Exception("El listado de operadores de backoffice ingreso como nulo, siendo imposible esta alternativa"));
            // Clears current list
            Clear();
            // Iterates over the list
            foreach (var operBack in prmLstOperator) {
                // add a new operator converting to OperBackoffice
                Add(new OperBackoffice(operBack));
            }            
        }
    }
}
