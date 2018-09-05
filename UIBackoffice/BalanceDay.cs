using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIBackoffice
{
    public class BalanceDay
    {
        Logica.Balance logBalance;

        private List<Entidades.Balance> _list;

        public List<Entidades.Balance> List {
            get {                
                return _list;
            }
            set {
                _list = value;
            }
        }

        /// <summary>
        /// Generate a new balance list for reporting purposes
        /// </summary>
        /// <param name="prmOperatorListToGenerate">List of operators to process</param>
        public async Task Generate(List<Entidades.Operador> prmOperatorListToGenerate)
        {
            // Generate a new logic object
            logBalance = new Logica.Balance();
            // Get a new list passing a operator list
            List = logBalance.CreateList(prmOperatorListToGenerate);
            // Loads starting values
            await GetStartingValues();
        }
        
        /// <summary>
        /// Increment by 1 by asunto passed on parameter
        /// </summary>
        /// <param name="prmAsuntoToFetch"></param>
        public void Increment(Entidades.Asunto prmAsuntoToFetch)
        {
            // Checks if the asunto is loaded with needeed values
            if (prmAsuntoToFetch.Oper.UserName == "" || prmAsuntoToFetch.AssignmentTime == null) throw new Exception("the informed asunto is missing of datetime or username");
            // Check if the list is started correctly
            if (List == null) throw new Exception("the list is not started correctly");
            try {
                // Find specific balance on the list
                Entidades.Balance balanceToIncrement = List.Find((balance) => balance.UserName == prmAsuntoToFetch.Oper.UserName);
                // Increment balance finded
                balanceToIncrement.Increment(prmAsuntoToFetch.AssignmentTime);
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Update list connecting to the service for retrieve 
        /// </summary>
        private async Task GetStartingValues()
        {
            // Gets base values of balance from the service
            await logBalance.GetBalanceOfTodayOperator(List);
            // Nullifies the entity logica balance
            logBalance = null;
        }
    }
}
