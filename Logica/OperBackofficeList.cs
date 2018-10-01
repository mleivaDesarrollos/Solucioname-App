using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Errors;

namespace Logica
{
    public class OperBackofficeList : ObservableCollection<Entidades.OperBackoffice>
    {
        Entidades.ConfigBackoffice configuration;

        /// <summary>
        /// On creation, we need pass information data of config File
        /// </summary>
        /// <param name="configData"></param>
        public OperBackofficeList(Entidades.ConfigBackoffice configData)
        {
            configuration = configData;
        }

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
                Add(new Entidades.OperBackoffice(operBack));
            }            
        }

        /// <summary>
        /// Get simple entity of operator and adds to a list
        /// </summary>
        /// <returns>A list with all operators</returns>
        public List<Entidades.Operador> GetOperatorList()
        {
            // Generate a new list to return at the end of the process
            List<Entidades.Operador> lstConnectedOperator = new List<Entidades.Operador>();
            // Check if the instance is null
            if (this == null) throw new Exception("El listado de operadores para el backoffice no fue inicializado correctamente.");
            // Iterates over all Operbackoffice
            foreach (var opBackoffice in this) {
                // checks if the operator instance is initialized
                if (opBackoffice.Operator == null) throw new Exception("Se ha encontrado un operador que no esta inicializado correctamente.");
                // Adds operator to list
                lstConnectedOperator.Add(opBackoffice.Operator);
            }
            // return the list processed operator
            return lstConnectedOperator;
        }

        /// <summary>
        /// Checks operator logged on for select specific operators ready for operation
        /// </summary>
        /// <returns></returns>
        public List<Entidades.Operador> GetOperatorListReadyToReceive()
        {
            // Create a new operator List on return
            List<Entidades.Operador> lstReadyOperators = new List<Entidades.Operador>();

            // Iterates over all operators saved on current list
            foreach (var operBo in this) {
                // Select only operators that currently are in operation time
                if (!isFinishedWork(operBo)) {
                    if(isInOffsetOfStartTime(operBo) || isWorkingReady(operBo)) {
                        lstReadyOperators.Add(operBo.Operator);
                    }
                }
            }
            // Return processed list to caller
            return lstReadyOperators;
        }

        /// <summary>
        /// Check if is offset of start time
        /// </summary>
        /// <param name="prmOperBo"></param>
        /// <returns></returns>
        private bool isInOffsetOfStartTime(Entidades.OperBackoffice prmOperBo)
        {
            if(prmOperBo.WorkStatus == Entidades.OperBackoffice.WorkingDayStatus.PreviousToStart) {
                if(prmOperBo.TimeLeftToNextEvent <= configuration.StartingMinutesOffset) {
                    return true;
                }
            }
            return false;
        }

        private bool isFinishedWork(Entidades.OperBackoffice prmOperBo)
        {
            return prmOperBo.WorkStatus == Entidades.OperBackoffice.WorkingDayStatus.Ended;
        }

        private bool isStartedWork(Entidades.OperBackoffice prmOperBo)
        {
            return prmOperBo.WorkStatus == Entidades.OperBackoffice.WorkingDayStatus.Started;
        }

        /// <summary>
        /// Working ready checks
        /// -Started to work
        /// -Break or end of work is not close
        /// -Break is close to end
        /// </summary>
        /// <param name="prmOperBo"></param>
        /// <returns></returns>
        private bool isWorkingReady(Entidades.OperBackoffice prmOperBo)
        {
            // Check is started to work
            if (isStartedWork(prmOperBo)) {
                // Check if time stop left is below offset
                if(prmOperBo.StoppedTimeLeft <= configuration.ReturningToWorkFromBreakInMinutesOffset) {
                    // Check if the break is not close to accomplish
                    if (prmOperBo.TimeLeftToNextEvent > configuration.StopProximityMinutesOffset) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
