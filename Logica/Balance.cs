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
        
        private List<Entidades.Balance> _list;

        private List<Entidades.Asunto> _lstOfTemporalyCountedAsuntos = new List<Entidades.Asunto>();

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
            // Get a new list passing a operator list
            List = CreateList(prmOperatorListToGenerate);
            // Loads starting values
            await GetStartingValues();
            // Update average on start
            UpdateAverage();
        }


        /// <summary>
        /// Create a list of balance by a list of operator 
        /// </summary>
        /// <param name="prmListOfOperatorsToCreate"></param>
        /// <returns></returns>
        private List<Entidades.Balance> CreateList(List<Entidades.Operador> prmListOfOperatorsToCreate)
        {
            // Create a new List for return on process
            List<Entidades.Balance> lstNewBalance = new List<Entidades.Balance>();
            // Check if input parameters are correct
            if (prmListOfOperatorsToCreate == null) throw new Exception("La lista no ha sido comunicada o esta vacía");
            // Iterates over the list of operators to generate the list
            foreach (var operatorLogged in prmListOfOperatorsToCreate) {
                // Generate a new balance entity
                Entidades.Balance newBalance = new Entidades.Balance()
                {
                    UserName = operatorLogged.UserName,
                    FirstName = operatorLogged.Nombre,
                    LastName = operatorLogged.Apellido,
                    StartTime = operatorLogged.StartTime,
                    EndTime = operatorLogged.EndTime,
                    TotalWorkTime = operatorLogged.TotalWorkTime          
                };
                if (operatorLogged.Breaks.Count >= 1) {
                    newBalance.BreakOneStart = operatorLogged.Breaks[0].Start;
                    newBalance.BreakOneEnd = operatorLogged.Breaks[0].End;
                }
                if (operatorLogged.Breaks.Count >= 2) {
                    newBalance.BreakTwoStart = operatorLogged.Breaks[1].Start;
                    newBalance.BreakTwoEnd = operatorLogged.Breaks[1].End;
                }
                if (operatorLogged.Breaks.Count >= 3) {
                    newBalance.BreakThreeStart = operatorLogged.Breaks[2].Start;
                    newBalance.BreakThreeEnd = operatorLogged.Breaks[2].End;
                }
                // Generate a new instance of balance and save on the list
                lstNewBalance.Add(newBalance);
            }
            // Return the list proceseed
            return lstNewBalance;
        }

        /// <summary>
        /// Increment by 1 by asunto passed on parameter
        /// </summary>
        /// <param name="prmAsuntoToFetch"></param>
        public void Increment(Entidades.Asunto prmAsuntoToFetch, bool refreshAverageAsuntoByHour = true)
        {
            try {
                validateInput(prmAsuntoToFetch);
                // Find specific balance on the list
                Entidades.Balance balance = getBalanceByAsunto(prmAsuntoToFetch);
                // DateTime to process in increment
                DateTime dateToIncrement;
                // If the asunto is assigned correctly
                if (prmAsuntoToFetch.isAssigned) {
                    // Set date of increment on date of assignment
                    dateToIncrement = prmAsuntoToFetch.AssignmentDate;
                    // If the asunto is previously saved on temporaly list
                    if (isLoadedOnTemporalyList(prmAsuntoToFetch)) {
                        // Decrement value on temporaly location saved
                        balance.Decrement(prmAsuntoToFetch.SendingDate);
                    }
                } else {
                    // If is not assigned means probably change status on program execution                    
                    _lstOfTemporalyCountedAsuntos.Add(prmAsuntoToFetch);
                    // Set va
                    dateToIncrement = prmAsuntoToFetch.SendingDate;
                }
                // Increment balance finded                
                balance.Increment(dateToIncrement);
            } catch (Exception ex) {
                throw ex;
            }
        }
        
        private Entidades.Balance getBalanceByAsunto(Entidades.Asunto prmAsuntoToFetch)
        {
            return List.Find((balance) => balance.UserName == prmAsuntoToFetch.Oper.UserName);
        }

        private bool isLoadedOnTemporalyList(Entidades.Asunto prmAsuntoToCheck)
        {
            return _lstOfTemporalyCountedAsuntos.Exists(asunto => asunto.Numero == prmAsuntoToCheck.Numero);
        }

        private void validateInput(Entidades.Asunto prmAsuntoToFetch)
        {
            // Checks if the asunto is loaded with needeed values
            if (prmAsuntoToFetch.Oper.UserName == "" || prmAsuntoToFetch.AssignmentDate == null) throw new Exception("the informed asunto is missing of datetime or username");
            // Check if the list is started correctly
            if (List == null) throw new Exception("the list is not started correctly");
        }

        /// <summary>
        /// Update list connecting to the service for retrieve 
        /// </summary>
        private async Task GetStartingValues()
        {
            // Gets base values of balance from the service
            List<Entidades.Asunto> lstAsuntoFromToday = await getAssignedAsuntosFromToday();
            foreach (var asunto in lstAsuntoFromToday) {
                Increment(asunto);
            }
        }

        /// <summary>
        /// On request, update Average asunto by hour
        /// </summary>
        public void UpdateAverage()
        {
            List.ForEach(balance => {
                balance.CalculateAverageAsuntoByHour();                
                });
        }


        /// <summary>
        /// Calls data layer to get data from the service
        /// </summary>
        /// <returns></returns>
        private async Task<List<Entidades.Asunto>> getAssignedAsuntosFromToday()
        {
            try {
                // Gets asuntos assigned of the day form the service
                return await datbalance.GetAsuntoAssignedOfCurrentDay();

            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}
