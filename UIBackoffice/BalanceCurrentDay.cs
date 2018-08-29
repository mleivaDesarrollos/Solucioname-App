using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using Errors;

namespace UIBackoffice
{
    public class BalanceCurrentDay
    {
        private List<BalanceHour> _lstBalance;

        public List<BalanceHour> List {
            get {
                return _lstBalance;                
            }
            set {
                _lstBalance = value;
            }
        }

        public BalanceCurrentDay()
        {
            List = new List<BalanceHour>();            
        }

        public async Task RefreshBalance()
        {
            try {
                // Instanciation of Logic Balance object
                Logica.Balance logBalance = new Logica.Balance();
                // Get the list from the service
                List = await logBalance.GetBalanceOfTodayOperator();
            }
            catch (Exception ex) {
                Except.Throw(ex);
            }
        }
    }
}
