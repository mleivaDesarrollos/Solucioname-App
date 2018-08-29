using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using System.Data.SQLite;
using System.Data;

namespace Servicio_Principal.SQL
{
    public static class Balance
    {
        private class Total
        {
            public int Quarter { get; set; }

            public int Hour{ get; set; }

            public int General { get; set; }

            public Total(int prmQuart, int prmHour, int prmGeneral)
            {
                Quarter = prmQuart;
                Hour = prmHour;
                General = prmGeneral;
            }

            /// <summary>
            /// Increase all totals by one
            /// </summary>
            public void Increase()
            {
                Quarter += 1;
                Hour += 1;
                General += 1;
            }
        }

        static readonly Dictionary<int, string> hourTableReference = new Dictionary<int, string>()
        {
            {7, "seven"},
            {8, "eight"},
            {9, "nine"},
            {10, "ten"},
            {11, "eleven"},
            {12, "twelve"},
            {13, "thirsteen"},
            {14, "fourteen"},
            {15, "fifteen"},
            {16, "sixteen"},
            {17, "seventeen"},
            {18, "eighteen"},
            {19, "nineteen"},
            {20, "twenty"},
            {21, "twentyone"}
        };

        static readonly Dictionary<int, string> minuteTableReference = new Dictionary<int, string>()
        {
            { 15, "_qt" },
            { 30, "_half" },
            { 45, "_3_qt" }
        };

        static readonly string strFullHourTableReference = "_full";

        static readonly string strBalanceTotalTableReference = "total";
        /// <summary>
        /// Gets balance of operator from today reference to current day
        /// </summary>
        /// <returns></returns>
        public static List<BalanceHour> GetAllOfToday()
        {
            // Generate a new list to return on process
            List<BalanceHour> lstBalanceToReturn = new List<BalanceHour>();
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                c.Open();
                string sQueryBalanceOfDay = "SELECT * from view_total_asunto_today_by_operator";
                using (SQLiteCommand cmdQueryBalanceOfDay = new SQLiteCommand(sQueryBalanceOfDay, c)) {
                    using (SQLiteDataReader rdrQueryBalanceOfDay = cmdQueryBalanceOfDay.ExecuteReader()) {
                        while (rdrQueryBalanceOfDay.Read()) {
                            lstBalanceToReturn.Add(new BalanceHour()
                            {
                                UserName = rdrQueryBalanceOfDay["username"].ToString(),
                                SevenHour = Convert.ToInt16(rdrQueryBalanceOfDay["seven_hour"]),
                                EightHour = Convert.ToInt16(rdrQueryBalanceOfDay["eight_hour"]),
                                NineHour = Convert.ToInt16(rdrQueryBalanceOfDay["nine_hour"]),
                                TenHour = Convert.ToInt16(rdrQueryBalanceOfDay["ten_hour"]),
                                ElevenHour = Convert.ToInt16(rdrQueryBalanceOfDay["eleven_hour"]),
                                TwelveHour = Convert.ToInt16(rdrQueryBalanceOfDay["twelve_hour"]),
                                ThirsteenHour = Convert.ToInt16(rdrQueryBalanceOfDay["thirsteen_hour"]),
                                FourteenHour = Convert.ToInt16(rdrQueryBalanceOfDay["fourteen_hour"]),
                                FifteenHour = Convert.ToInt16(rdrQueryBalanceOfDay["fifteen_hour"]),
                                SixteenHour = Convert.ToInt16(rdrQueryBalanceOfDay["sixteen_hour"]),
                                SeventeenHour = Convert.ToInt16(rdrQueryBalanceOfDay["seventeen_hour"]),
                                EighteenHour = Convert.ToInt16(rdrQueryBalanceOfDay["eighteen_hour"]),
                                NineteenHour = Convert.ToInt16(rdrQueryBalanceOfDay["nineteen_hour"]),
                                TwentyHour = Convert.ToInt16(rdrQueryBalanceOfDay["twenty_hour"]),
                                TwentyOneHour = Convert.ToInt16(rdrQueryBalanceOfDay["twentyone_hour"]),
                                Total = Convert.ToInt16(rdrQueryBalanceOfDay["total"])
                            });
                        }
                    }
                }
            }
            // Return the processed list
            return lstBalanceToReturn;
        }

        /// <summary>
        /// Checks if the balance of today is generated, and creates if not exist based on list of connected today
        /// </summary>
        public static void CheckAndCreateBalanceCorrespondingToday(List<Entidades.Operador> prmOperatorConnectedList)
        {
            try {
                if (prmOperatorConnectedList == null) throw new Exception("the list of connected operator isn't correctly initialized.");
                using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                    c.Open();
                    string strQueryExistenceOfBalanceToday = "SELECT 1 from asuntos_balances_registry where registry_date = date('now', 'localtime')";
                    using (SQLiteCommand cmdQueryExistenceOfBalanceToday = new SQLiteCommand(strQueryExistenceOfBalanceToday, c)) {
                        bool balanceExist = false;
                        using (SQLiteDataReader rdrQueryExistenceOfBalanceToday = cmdQueryExistenceOfBalanceToday.ExecuteReader()) {
                            if (rdrQueryExistenceOfBalanceToday.Read()) {
                                balanceExist = true;
                            }
                        }
                        if (!balanceExist) {
                            using (SQLiteTransaction t = c.BeginTransaction()) {
                                Log.Info("BalanceCheckTask", "generating balance corresponding to the day...");
                                string strInsertBalanceOfDay = "INSERT INTO asuntos_balances_registry (username, registry_date) values (@User, date('now', 'localtime'))";
                                using (SQLiteCommand cmdInsertBalanceOfDay = new SQLiteCommand(strInsertBalanceOfDay, c, t)) {
                                    foreach (var operatorToday in prmOperatorConnectedList) {
                                        cmdInsertBalanceOfDay.Parameters.Agregar("@User", operatorToday.UserName);
                                        cmdInsertBalanceOfDay.ExecuteNonQuery();
                                    }
                                }
                                t.Commit();
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                Log.Error("BalanceCheckTask", ex.Message);
            }
        }

        /// <summary>
        /// Add new entry to balance. This methods process a add request to database on opened transaction
        /// </summary>
        /// <param name="prmAsunto">Asunto to add</param>
        /// <param name="conn">Connection to use for adding</param>
        /// <param name="tran">Transaction to use</param>
        public static void AddEntry(Entidades.Asunto prmAsunto, SQLiteConnection conn, SQLiteTransaction tran, DateTime prmDtmTimeProcess)
        {
            // Checks if connection is opend
            if(conn == null || tran == null) throw new Exception("transaction or connection has null values");
            if(!(conn.State == ConnectionState.Open)) throw new Exception("connection is not open");
            if (prmDtmTimeProcess == null) throw new Exception("datetime is not gived to process");
            // get hour and minutes from current dateTime
            int minutes = prmDtmTimeProcess.Minute;
            int hour = prmDtmTimeProcess.Hour;
            // Gets column name for adding to database
            string strHourName = getHourColumnName(hour);
            string strQuarterName = getQuarterColumnName(minutes);
            // Gets and load new total variable
            Total balanceTotal = getCurrentTotals(prmAsunto, conn, tran, prmDtmTimeProcess, strQuarterName, strHourName);
            // Prepare string for insertion
            updateBalance(prmAsunto, conn, tran, prmDtmTimeProcess, strHourName, strQuarterName, balanceTotal);
        }

        /// <summary>
        /// Process a update to balance registry
        /// </summary>
        /// <param name="prmAsuntoUpdate">Asunto with operator to update</param>
        /// <param name="conn">connection object to use in the process</param>
        /// <param name="tran">transaction operation to trace complete operation</param>
        /// <param name="prmCurrentTime">date on who the process has been initialized</param>
        /// <param name="prmColumnHourName">column hour name</param>
        /// <param name="prmQuarterColumnName">quarter column name</param>
        /// <param name="prmCurrentTotals">current obtained totals</param>
        private static void updateBalance(Entidades.Asunto prmAsuntoUpdate, SQLiteConnection conn, SQLiteTransaction tran, DateTime prmCurrentTime, string prmColumnHourName, string prmQuarterColumnName, Total prmCurrentTotals)
        {
            // Prepare string for update on database
            string strUpdateBalanceWithNewAdition = string.Format("UPDATE asuntos_balances_registry set {0}=@QuarterTotal, {1}=@HourTotal , {2}=@GeneralTotal where username=@User and registry_date=@DateNow", prmColumnHourName + prmQuarterColumnName, prmColumnHourName + strFullHourTableReference, strBalanceTotalTableReference);
            using (SQLiteCommand cmdUpdateBalanceWithNewAdition = new SQLiteCommand(strUpdateBalanceWithNewAdition, conn, tran)) {
                // Increase totals
                prmCurrentTotals.Increase();
                // Parametrize add
                cmdUpdateBalanceWithNewAdition.Parameters.Agregar("@QuarterTotal", prmCurrentTotals.Quarter);
                cmdUpdateBalanceWithNewAdition.Parameters.Agregar("@HourTotal", prmCurrentTotals.Hour);
                cmdUpdateBalanceWithNewAdition.Parameters.Agregar("@GeneralTotal", prmCurrentTotals.General);
                cmdUpdateBalanceWithNewAdition.Parameters.Agregar("@User", prmAsuntoUpdate.Oper.UserName);
                cmdUpdateBalanceWithNewAdition.Parameters.Agregar("@DateNow", prmCurrentTime.ToString("yyyy-MM-dd"));
                // Save result on a int variable
                int resultQuery = cmdUpdateBalanceWithNewAdition.ExecuteNonQuery();
                // Execute update
                if ( resultQuery != 1) throw new Exception(string.Format("error updating balance. No actions taken over {0} of operator {1}.",prmAsuntoUpdate.Numero, prmAsuntoUpdate.Oper.UserName));
            }
        }

        /// <summary>
        /// Checks on database for current saved totals and return processed balance to caller
        /// </summary>
        /// <param name="prmAsuntoTot">Asunto with operator loaded</param>
        /// <param name="conn">Connection to process request</param>
        /// <param name="tran">Transaction to trace operation</param>
        /// <param name="prmCurrentTime">current Time</param>
        /// <param name="prmQuarterName">Quarter column name</param>
        /// <param name="prmHourName">Hour column name</param>
        /// <returns></returns>
        private static Total getCurrentTotals(Entidades.Asunto prmAsuntoTot, SQLiteConnection conn, SQLiteTransaction tran, DateTime prmCurrentTime, string prmQuarterName, string prmHourName)
        {
            // Generates a new instance of total values
            Total totalFromDatabase;
            // Prepare string query for consulting
            string strQueryCurrentBalanceTotal = string.Format("SELECT {0}, {1}, {2} from asuntos_balances_registry where username=@User and registry_date = @DateNow", prmHourName + prmQuarterName , prmHourName + strFullHourTableReference, strBalanceTotalTableReference);
            using (SQLiteCommand cmdQueryCurrentBalanceTotal = new SQLiteCommand(strQueryCurrentBalanceTotal, conn, tran)) {
                // Parametrizes user name
                cmdQueryCurrentBalanceTotal.Parameters.Agregar("@User", prmAsuntoTot.Oper.UserName);
                cmdQueryCurrentBalanceTotal.Parameters.Agregar("@DateNow", prmCurrentTime.ToString("yyyy-MM-dd"));
                using (SQLiteDataReader rdrQueryCurrentBalanceTotal = cmdQueryCurrentBalanceTotal.ExecuteReader()) {
                    if (rdrQueryCurrentBalanceTotal.Read()) {
                        int quarter = Convert.ToInt32(rdrQueryCurrentBalanceTotal[prmHourName + prmQuarterName]);
                        int hour = Convert.ToInt16(rdrQueryCurrentBalanceTotal[prmHourName + strFullHourTableReference]);
                        int general = Convert.ToInt32(rdrQueryCurrentBalanceTotal[strBalanceTotalTableReference]);
                        totalFromDatabase = new Total(quarter, hour, general);
                    }
                    else {
                        throw new Exception(string.Format("the operator {0} don't have a balance loaded on database of day {1}.", prmAsuntoTot.Numero, prmCurrentTime.ToString("yyyy-MM-dd")));
                    }
                }
            }
            // Return processed value
            return totalFromDatabase;
        }


        /// <summary>
        /// Gets hour column name 
        /// </summary>
        /// <param name="prmHour">Hour to get column name</param>
        /// <returns></returns>
        private static string getHourColumnName(int prmHour)
        {
            // Dispose a variable for return
            string strColumnName = "";
            // Iterates over hours to get with correct related column name
            foreach (KeyValuePair<int, string> hourColumnName in hourTableReference) {
                // if match found adds content to return variable
                if(prmHour == hourColumnName.Key) {
                    strColumnName += hourColumnName.Value;
                    break;
                }
            }
            return strColumnName;
        }

        /// <summary>
        /// Calculate who a quarter correspond to minutes gived on parameter
        /// </summary>
        /// <param name="prmMinutes">Minutes to calculate corresponding quarter</param>
        /// <returns></returns>
        private static string getQuarterColumnName(int prmMinutes)
        {
            // Initialize quarter name
            string strQuarterNameResult = "";
            foreach (KeyValuePair<int,string> minuteQuarterName in minuteTableReference.OrderByDescending((minute) => minute.Key)) {
                if(prmMinutes >= minuteQuarterName.Key) {
                    strQuarterNameResult += minuteQuarterName.Value;
                    break;
                }
            }
            // Return processed result
            return strQuarterNameResult;
        }
    }
}
