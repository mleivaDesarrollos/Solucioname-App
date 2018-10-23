using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Configuration;

namespace Servicio_Principal.SQL
{
    public static class Operador
    {
        private static readonly DateTime NO_TIME_LOADED = DateTime.MinValue;

        /// <summary>
        /// Valida con base de datos si el ingreso es correcto
        /// </summary>
        /// <param name="opIngresante"></param>
        /// <returns></returns>
        public static bool ValidarIngreso(Entidades.Operador opIngresante)
        {
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                c.Open();
                string sConsultaValidacion = "SELECT 1 FROM operadores WHERE username=@User COLLATE NOCASE and password=@Password";
                using (SQLiteCommand commValidacion = new SQLiteCommand(sConsultaValidacion, c))
                {
                    commValidacion.Parameters.Agregar("@User", opIngresante.UserName);
                    commValidacion.Parameters.Agregar("@Password", opIngresante.Password);
                    // Ejecutamos el lector de validacion
                    using (SQLiteDataReader rdrValidacion = commValidacion.ExecuteReader())
                    {
                        if (rdrValidacion.Read())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Valida si el operador informado cuenta con permisos de backoffice
        /// </summary>
        /// <param name="pOperator"></param>
        /// <returns>Operador con datos completos, nulo si no es encontrado o no tiene permisos</returns>
        public static Entidades.Operador ValidateBackofficeOperator(Entidades.Operador pOperator)
        {
            // El operador de backoffice es inicializado como nulo
            Entidades.Operador backofficeOperator = null;
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                c.Open();
                string strFillBackofficUserData = "SELECT nombre, apellido, dni from operadores where username=@Username and password=@Password and backoffice=1";
                using (SQLiteCommand cmdFillBackofficeUserData = new SQLiteCommand(strFillBackofficUserData, c))
                {
                    // Agregamos el parametro de usuario y contraseña
                    cmdFillBackofficeUserData.Parameters.Agregar("@Username", pOperator.UserName);
                    cmdFillBackofficeUserData.Parameters.Agregar("@Password", pOperator.Password);
                    using (SQLiteDataReader rdrBackofficeData = cmdFillBackofficeUserData.ExecuteReader())
                    {
                        // Read the possible results of query
                        if (rdrBackofficeData.Read())
                        {
                            // Load all properties related to operator
                            backofficeOperator = new Entidades.Operador()
                            {
                                UserName = pOperator.UserName,
                                Password = pOperator.Password,
                                Nombre = rdrBackofficeData["nombre"].ToString(),
                                Apellido = rdrBackofficeData["apellido"].ToString(),
                                DNI = rdrBackofficeData["dni"].ToString()
                            };
                        }
                    }
                }
            }
            // Return the operator value
            return backofficeOperator;
        }

        /// <summary>
        /// Check if the operator is loaded in database
        /// </summary>
        /// <param name="prmOperatorToValidate"></param>
        /// <returns></returns>
        public static bool Exist(Entidades.Operador prmOperatorToValidate)
        {
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                c.Open();
                string sQueryOperatorExistence = "SELECT 1 from operadores where UserName=@Operator";
                using (SQLiteCommand cmdQueryOperatorExistence = new SQLiteCommand(sQueryOperatorExistence, c)) {
                    cmdQueryOperatorExistence.Parameters.Agregar("@Operator", prmOperatorToValidate.UserName);
                    using (SQLiteDataReader rdrQueryOperatorExistence = cmdQueryOperatorExistence.ExecuteReader()) {
                        if (rdrQueryOperatorExistence.Read()) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieves a list of operators who logs on the system today
        /// </summary>
        /// <returns></returns>
        public static List<Entidades.Operador> getOperatorsOfTheDay()
        {
            try {
                // Create a list to return at the end of the process
                List<Entidades.Operador> lstTodayOperators = new List<Entidades.Operador>();
                using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                    c.Open();
                    // Get operators loaded today except who is loaded on exception today
                    string strCurrentDayOperator = @"select * from view_operator_current_day_activity";
                    if (Config.TEST) strCurrentDayOperator = "select * from view_operator_monday_day_activity";
                    using (SQLiteCommand cmdQueryOperatorOfTheDay = new SQLiteCommand(strCurrentDayOperator, c)) {
                        using (SQLiteDataReader rdrQueryOperatorOfTheDay = cmdQueryOperatorOfTheDay.ExecuteReader()) {
                            while (rdrQueryOperatorOfTheDay.Read()) {
                                getBreakList(rdrQueryOperatorOfTheDay);
                                lstTodayOperators.Add(new Entidades.Operador()
                                {
                                    UserName = rdrQueryOperatorOfTheDay["username"].ToString(),
                                    Nombre = rdrQueryOperatorOfTheDay["surname"].ToString(),
                                    Apellido = rdrQueryOperatorOfTheDay["lastname"].ToString(),
                                    WorkingDayTime = new Entidades.WorkTime()
                                    {
                                        StartTime = Convert.ToDateTime(rdrQueryOperatorOfTheDay["working_start"]),
                                        EndTime = Convert.ToDateTime(rdrQueryOperatorOfTheDay["working_end"])
                                    },
                                    Breaks = getBreakList(rdrQueryOperatorOfTheDay),
                                    Backoffice = (Entidades.Operador.BackofficeType) Convert.ToInt16(rdrQueryOperatorOfTheDay["backoffice"])
                                });
                            }
                        }
                    }
                    // Read exceptions for today
                    string strCurrentDayException = "SELECT * from view_operator_planification_exception_today";
                    using (SQLiteCommand cmdCurrentDayException = new SQLiteCommand(strCurrentDayException, c)) {
                        using (SQLiteDataReader rdrCurrentDayException = cmdCurrentDayException.ExecuteReader()) {
                            while (rdrCurrentDayException.Read()) {
                                // Get username of exception
                                string strUserName = rdrCurrentDayException["username"].ToString();
                                // Get Starting time for operator
                                DateTime dtmStartTime = Convert.ToDateTime(rdrCurrentDayException["working_start"]);
                                // If the operator dont have loadeed a ausent, implies that have change on working time or come on unprogramated date
                                if (dtmStartTime != Entidades.ExceptionDay.AUSENT_OPERATOR_TODAY) { 
                                    // Load Working end Time
                                    DateTime dtmEndingTime = Convert.ToDateTime(rdrCurrentDayException["working_end"]);
                                    // Validate loaded ending time
                                    if(dtmEndingTime == NO_TIME_LOADED) {
                                        Log.Error("OperatorExceptions", string.Format("the exception for operator {0} has not been loaded correctly. Lacks of end of working time."));
                                        continue;
                                    }
                                    // Get all breaks related to the exception
                                    List<Entidades.WorkTime> breaksForException = getBreakList(rdrCurrentDayException);
                                    // Get Surname and Lastname from corresponding exception
                                    string surName = rdrCurrentDayException["surname"].ToString();
                                    string lastName = rdrCurrentDayException["lastname"].ToString();
                                    Entidades.Operador.BackofficeType typeOfOperator = (Entidades.Operador.BackofficeType)Convert.ToInt16(rdrCurrentDayException["backoffice"]);
                                    // Adds the exception on the list
                                    lstTodayOperators.Add(new Entidades.Operador()
                                    {
                                        UserName = strUserName,
                                        Nombre = surName,
                                        Apellido = lastName,
                                        WorkingDayTime = new Entidades.WorkTime()
                                        {
                                            StartTime = dtmStartTime,
                                            EndTime = dtmEndingTime
                                        },
                                        Breaks = breaksForException,
                                        Backoffice = typeOfOperator
                                    });
                                }
                            }
                        }
                    }
                }
                // Reorder list based on username
                lstTodayOperators = lstTodayOperators.OrderBy(oper => oper.UserName).ToList();
                // Returns the processed list
                return lstTodayOperators;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Add an exception day for a specific operator
        /// </summary>
        /// <param name="operatorToLoadException"></param>
        public static void AddExceptionDay(Entidades.Operador operatorToAdd)
        {

        }

        /// <summary>
        /// Remove exception day for a specific operator
        /// </summary>
        /// <param name="operatorToRemoveException"></param>
        public static void RemoveExceptionDay(Entidades.Operador operatorToRemove)
        {

        }

        /// <summary>
        /// Update Exception day for specific operator
        /// </summary>
        /// <param name="operatorToUpdate"></param>
        public static void UpdateExceptionDay(Entidades.Operador operatorToUpdate)
        {

        }

        
        
        /// <summary>
        /// Calculates the starting hour and adds minutes to this date
        /// </summary>
        /// <param name="paramStart"></param>
        /// <param name="iMinutes"></param>
        /// <returns></returns>
        private static DateTime getEndBreak(string paramStart, int paramMinutes)
        {
            return Convert.ToDateTime(paramStart).AddMinutes(paramMinutes);
        }
        
        /// <summary>
        /// Calculates the starting hour and adds minutes to this date
        /// </summary>
        /// <param name="paramStart"></param>
        /// <param name="iMinutes"></param>
        /// <returns></returns>
        private static DateTime getEndBreak(DateTime dtmBreakStart, object paramMinutesInString)
        {
            int paramMinutes = 0;
            try {
                paramMinutes = Convert.ToInt16(paramMinutesInString);
            } finally {
                // Nothing really needed to do
            }
            return dtmBreakStart.AddMinutes(paramMinutes);

        }
        /// <summary>
        /// Get a break processed list by a DataReader
        /// </summary>
        /// <param name="rdrWithBreakData"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerHidden]
        private static List<Entidades.WorkTime> getBreakList(SQLiteDataReader rdrWithBreakData)
        {
            // Creates a new list of breaks
            List<Entidades.WorkTime> lstBreaks = new List<Entidades.WorkTime>();
            // Iterates over the reader to get all related breaks, dispose a flag to control
            bool hasNextBreak = true;
            // Value of current iteration
            int intBreakNumber = 1;
            // Concatenation items
            string strBreak = "break_";
            string strDuration = "duration_";
            while (hasNextBreak) {
                // Concatenates current iteration with common column name
                string breakIteration = strBreak + intBreakNumber;
                string durationIteration = strDuration + intBreakNumber;
                try {
                    // Checks if the current iteration has values. If the column value exceeds maximum of range reader object
                    if (rdrWithBreakData[breakIteration] != System.DBNull.Value && rdrWithBreakData[durationIteration] != System.DBNull.Value) {
                        lstBreaks.Add(new Entidades.WorkTime()
                        {
                            StartTime = Convert.ToDateTime(rdrWithBreakData[breakIteration]),
                            EndTime = getEndBreak(rdrWithBreakData[breakIteration].ToString(), Convert.ToInt32(rdrWithBreakData[durationIteration]))
                        });
                        // Plus a iteration
                        intBreakNumber++;
                    }
                    else {
                        hasNextBreak = false;
                    }
                }
                catch (IndexOutOfRangeException) {
                    hasNextBreak = false;
                }
                
            }            
            // Return the processed list
            return lstBreaks;
        }
    }
}
