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
        public async Task GetBalanceOfTodayOperator(List<Entidades.Balance> prmListToProcess)
        {
            try {
                // Gets asuntos assigned of the day form the service
                List<Entidades.Asunto> lstAsuntoAssigned = await datbalance.GetAsuntoAssignedOfCurrentDay();
                // Iterates over asuntos assigned and count plus one on selected time
                foreach (var asuntoAssign in lstAsuntoAssigned) {
                    // Find balance corresponding to operator looking by operator username
                    Entidades.Balance balanceForModify = prmListToProcess.Find((balance) => balance.UserName == asuntoAssign.Oper.UserName);
                    // Increase balance by date
                    balanceForModify.Increment(asuntoAssign.AssignmentTime);
                }

            }
            catch (Exception ex) {
                throw ex;
            }
        }
        
        /// <summary>
        /// Create a list of balance by a list of operator 
        /// </summary>
        /// <param name="prmListOfOperatorsToCreate"></param>
        /// <returns></returns>
        public List<Entidades.Balance> CreateList(List<Entidades.Operador> prmListOfOperatorsToCreate)
        {
            // Create a new List for return on process
            List<Entidades.Balance> lstNewBalance = new List<Entidades.Balance>();
            // Check if input parameters are correct
            if (prmListOfOperatorsToCreate == null) throw new Exception("La lista no ha sido comunicada o esta vacía");
            // Iterates over the list of operators to generate the list
            foreach (var operatorLogged in prmListOfOperatorsToCreate) {
                // Generate a new instance of balance and save on the list
                lstNewBalance.Add(new Entidades.Balance()
                {
                    UserName = operatorLogged.UserName
                });
            }
            // Return the list proceseed
            return lstNewBalance;
        }       
    }
}
