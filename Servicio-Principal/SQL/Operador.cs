using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Servicio_Principal.SQL
{
    public class Operador
    {
        /// <summary>
        /// Valida con base de datos si el ingreso es correcto
        /// </summary>
        /// <param name="opIngresante"></param>
        /// <returns></returns>
        public bool ValidarIngreso(Entidades.Operador opIngresante)
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
        public Entidades.Operador ValidateBackofficeOperator(Entidades.Operador pOperator)
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
        /// Retrieves a list of operators who logs on the system today
        /// </summary>
        /// <returns></returns>
        public List<Entidades.Operador> getOperatorOfTheDay()
        {
            try {
                // Create a list to return at the end of the process
                List<Entidades.Operador> lstTodayOperators = new List<Entidades.Operador>();
                using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                    c.Open();
                    string strCurrentDayOperator = "select * from view_operator_current_day_activity";
                    using (SQLiteCommand cmdQueryOperatorOfTheDay = new SQLiteCommand(strCurrentDayOperator, c)) {
                        using (SQLiteDataReader rdrQueryOperatorOfTheDay = cmdQueryOperatorOfTheDay.ExecuteReader()) {
                            while (rdrQueryOperatorOfTheDay.Read()) {
                                lstTodayOperators.Add(new Entidades.Operador()
                                {
                                    UserName = rdrQueryOperatorOfTheDay["username"].ToString(),
                                    Nombre = rdrQueryOperatorOfTheDay["surname"].ToString(),
                                    Apellido = rdrQueryOperatorOfTheDay["lastname"].ToString(),
                                    StartTime = Convert.ToDateTime(rdrQueryOperatorOfTheDay["working_start"]),
                                    EndTime = Convert.ToDateTime(rdrQueryOperatorOfTheDay["working_end"]),
                                    Breaks = getBreakList(rdrQueryOperatorOfTheDay)
                                });
                            }
                        }
                    }
                }
                // Returns the processed list
                return lstTodayOperators;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        
        /// <summary>
        /// Calculates the starting hour and adds minutes to this date
        /// </summary>
        /// <param name="paramStart"></param>
        /// <param name="iMinutes"></param>
        /// <returns></returns>
        private DateTime getEndBreak(string paramStart, int paramMinutes)
        {
            return Convert.ToDateTime(paramStart).AddMinutes(paramMinutes);
        }

        /// <summary>
        /// Get a break processed list by a DataReader
        /// </summary>
        /// <param name="rdrWithBreakData"></param>
        /// <returns></returns>
        private List<Entidades.Operador.Break> getBreakList(SQLiteDataReader rdrWithBreakData)
        {
            // Creates a new list of breaks
            List<Entidades.Operador.Break> lstBreaks = new List<Entidades.Operador.Break>();
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
                        lstBreaks.Add(new Entidades.Operador.Break()
                        {
                            Start = Convert.ToDateTime(rdrWithBreakData[breakIteration]),
                            End = getEndBreak(rdrWithBreakData[breakIteration].ToString(), Convert.ToInt32(rdrWithBreakData[durationIteration]))
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
