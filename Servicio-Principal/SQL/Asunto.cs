using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;


namespace Servicio_Principal.SQL
{
    public static class Asunto
    {

        public static readonly string CLASSNAME = "AsuntoDAO";

        public static readonly DateTime dayToCheckService = Config.TEST_MAILING_SERVICE ? new DateTime(2018, 09, 19) : DateTime.Today;

        public static List<AsuntoFilter> lstFiltersForMails;

        public static int INDEX_START_STRING = 0;

        private static readonly string QUERY_VALIDATE_ASSIGNED_ASUNTOS_BATCH = "SELECT number, operator from asuntos_assigned_successful where number in (@AsuntoList)";
        

        /// <summary>
        /// Agrega un asunto a la base de datos de respaldo
        /// </summary>
        /// <param name="asunto"></param>
        public static void AddToQueue(Entidades.Asunto asunto)
        {
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                c.Open();
                using (SQLiteTransaction t = c.BeginTransaction()) {
                    using (SQLiteCommand cmdAddAsuntoToQueue = getAddToPendingQueueCommand(c, t, asunto)) {
                        // Ejecutamos el comando parametrizado
                        cmdAddAsuntoToQueue.ExecuteNonQuery();
                    }
                    if (!asunto.isCreatedByBackoffice) {
                        using (SQLiteCommand cmdRemoveFromAsuntosAssigned = getDeleteAsuntoWithoutAssignationCommand(c, t, asunto)) {
                            // Ejecutamos la eliminación del asunto de los inasignados
                            cmdRemoveFromAsuntosAssigned.ExecuteNonQuery();
                        }
                    }
                    t.Commit();
                }
            }
        }

        /// <summary>
        /// Add a batch of asuntos to distribution pending database
        /// </summary>
        /// <param name="prmListOfPendingAsuntos"></param>
        public static void AddToQueue(List<Entidades.Asunto> prmListOfPendingAsuntos)
        {
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                c.Open();
                using (SQLiteTransaction t = c.BeginTransaction()) {
                    foreach (var asuntoToEnqueue in prmListOfPendingAsuntos) {
                        using (SQLiteCommand cmdAddAsuntoToQueue = getAddToPendingQueueCommand(c, t, asuntoToEnqueue)) {
                            // Ejecutamos el comando parametrizado
                            cmdAddAsuntoToQueue.ExecuteNonQuery();
                        }
                        if (!asuntoToEnqueue.isCreatedByBackoffice) {
                            using (SQLiteCommand cmdRemoveFromAsuntosAssigned = getDeleteAsuntoWithoutAssignationCommand(c, t, asuntoToEnqueue)) {
                                // Ejecutamos la eliminación del asunto de los inasignados
                                cmdRemoveFromAsuntosAssigned.ExecuteNonQuery();
                            }
                        }
                    }
                    t.Commit();
                }
            }
        }

        /// <summary>
        /// Remueve un asunto de la base de datos de respaldo
        /// </summary>
        /// <param name="asunto"></param>
        public static void RemoveFromQueueAndSaveHistoricData(Entidades.Asunto asunto)
        {
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                c.Open();
                using (SQLiteTransaction t = c.BeginTransaction()) {
                    using (SQLiteCommand cmdDeleteFromPendientes = getDeleteFromPendingCommand(c, t, asunto)) {
                        cmdDeleteFromPendientes.ExecuteNonQuery();
                    }
                    using (SQLiteCommand cmdInsertAssignedAsuntos = getInsertCommandToAsuntosAssignedSuccesful(c, t, asunto)) {                        
                        cmdInsertAssignedAsuntos.ExecuteNonQuery();
                    }
                    // When the registry is saved, proceed to calculate column to save
                    // if all operations are successful, commit changes over database
                    t.Commit();
                }
            }
        }

        public static void RemoveFromQueueAndSaveHistoricData(List<Entidades.Asunto> listOfAsuntosToRemove)
        {
            // If the list communicated not have minimal requirements, reject the process
            if (listOfAsuntosToRemove == null || listOfAsuntosToRemove.Count <= 1) throw new Exception("the list of asunto to remove from pending delivery is empty, null or have one register only.");
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                // Open connection for operate over database
                c.Open();
                using (SQLiteTransaction t = c.BeginTransaction()) {
                    // Iterates over all asuntos 
                    foreach (var asuntoToSuccessfulAssign in listOfAsuntosToRemove) {
                        // Remove from pending asuntos
                        using (SQLiteCommand cmdDeleteFromPending = getDeleteFromPendingCommand(c, t, asuntoToSuccessfulAssign)) {
                            cmdDeleteFromPending.ExecuteNonQuery();
                        }
                        using (SQLiteCommand cmdInsertAsuntoConfirmed = getInsertCommandToAsuntosAssignedSuccesful(c, t, asuntoToSuccessfulAssign)) {
                            cmdInsertAsuntoConfirmed.ExecuteNonQuery();
                        }
                    }                    
                    // Commit changes to database
                    t.Commit();
                }
            }
        }

        /// <summary>
        /// Dispose a command with all parameters added for adding to asunto_pendiente_asignacion table
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <param name="asuntoToEnqueue"></param>
        /// <returns></returns>
        private static SQLiteCommand getAddToPendingQueueCommand(SQLiteConnection conn, SQLiteTransaction tran, Entidades.Asunto asuntoToEnqueue)
        {
            // Generate a new SQliTe
            string strAddAsuntoToQueue = "INSERT INTO asuntos_pendiente_asignacion VALUES (@Numero, @Oper, @ShortDesc, @Reporting, @LoadedTime, @SendingDate)";
            SQLiteCommand cmdAddAsuntoToQueue = new SQLiteCommand(strAddAsuntoToQueue, conn, tran);
            // Parametrizamos los valores del asunto cargado
            cmdAddAsuntoToQueue.Parameters.Agregar("@Numero", asuntoToEnqueue.Numero);
            cmdAddAsuntoToQueue.Parameters.Agregar("@Oper", asuntoToEnqueue.Oper.UserName);
            cmdAddAsuntoToQueue.Parameters.Agregar("@ShortDesc", asuntoToEnqueue.DescripcionBreve);
            cmdAddAsuntoToQueue.Parameters.Agregar("@Reporting", asuntoToEnqueue.Reportable);
            cmdAddAsuntoToQueue.Parameters.Agregar("@SendingDate", asuntoToEnqueue.SendingDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmdAddAsuntoToQueue.Parameters.Agregar("@LoadedTime", asuntoToEnqueue.LoadedOnSolucionameDate.ToString("yyyy-MM-dd HH:mm:ss"));
            // Return processed command to caller
            return cmdAddAsuntoToQueue;
        }

        private static SQLiteCommand getDeleteAsuntoWithoutAssignationCommand(SQLiteConnection conn, SQLiteTransaction tran, Entidades.Asunto asuntoToRemove)
        {
            // Dispose string to query database
            string strRemoveAsuntoWithoutAssignation = "DELETE FROM asuntos_on_solucioname_without_assignation WHERE number=@Number";
            SQLiteCommand cmdRemoveAsuntoWithoutAssignation = new SQLiteCommand(strRemoveAsuntoWithoutAssignation, conn, tran);
            // Parametrize command
            cmdRemoveAsuntoWithoutAssignation.Parameters.Agregar("@Number", asuntoToRemove.Numero);
            // Return processed command
            return cmdRemoveAsuntoWithoutAssignation;
        }

        /// <summary>
        /// Return query with data for delete from asuntos pending register
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        private static SQLiteCommand getDeleteFromPendingCommand(SQLiteConnection conn, SQLiteTransaction tran, Entidades.Asunto asuntoToParametrize)
        {
            // Dispose a string for interact on database
            string strDeleteFromAssignationPending = "DELETE FROM asuntos_pendiente_asignacion WHERE numero=@Number and operador=@Operador";
            // Generate a new command to return in process
            SQLiteCommand cmdDeleteFromAssignationPending = new SQLiteCommand(strDeleteFromAssignationPending, conn, tran);
            // Assign parameters to command
            cmdDeleteFromAssignationPending.Parameters.Agregar("@Number", asuntoToParametrize.Numero);
            cmdDeleteFromAssignationPending.Parameters.Agregar("@Operador", asuntoToParametrize.Oper.UserName);
            // Return generated command to caller
            return cmdDeleteFromAssignationPending;            
        }

        /// <summary>
        /// Return a comand with data for loading asunto confirmed information
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        private static SQLiteCommand getInsertCommandToAsuntosAssignedSuccesful(SQLiteConnection conn, SQLiteTransaction tran, Entidades.Asunto asuntoToParametrize)
        {
            // Prepare string for command
            string strInsertAsuntoAssignedSuccesful = "INSERT INTO asuntos_assigned_successful (number, operator, short_description, for_report, assignation_date, date_of_register, sending_date) values (@Number, @Oper, @ShortDesc, @ForReport, @AssignDate, @LoadedTime, @SendingDate)";
            // Generate a new Command based on string
            SQLiteCommand cmdInsertAsuntoAssignedSuccesful = new SQLiteCommand(strInsertAsuntoAssignedSuccesful, conn, tran);
            // Parametrize new command
            cmdInsertAsuntoAssignedSuccesful.Parameters.Agregar("@Number", asuntoToParametrize.Numero);
            cmdInsertAsuntoAssignedSuccesful.Parameters.Agregar("@Oper", asuntoToParametrize.Oper.UserName);
            cmdInsertAsuntoAssignedSuccesful.Parameters.Agregar("@ShortDesc", asuntoToParametrize.DescripcionBreve);
            cmdInsertAsuntoAssignedSuccesful.Parameters.Agregar("@ForReport", asuntoToParametrize.Reportable);
            cmdInsertAsuntoAssignedSuccesful.Parameters.Agregar("@AssignDate", asuntoToParametrize.AssignmentDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmdInsertAsuntoAssignedSuccesful.Parameters.Agregar("@LoadedTime", asuntoToParametrize.LoadedOnSolucionameDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmdInsertAsuntoAssignedSuccesful.Parameters.Agregar("@SendingDate", asuntoToParametrize.SendingDate.ToString("yyyy-MM-dd HH:mm:ss"));
            // Return generated command
            return cmdInsertAsuntoAssignedSuccesful;
        }
        
        /// <summary>
        /// Obtiene un listado de asuntos almacenados pendientes de envío a clientes en la base de datos de respaldo
        /// </summary>
        /// <returns></returns>
        public static List<Entidades.Asunto> getQueue()
        {
            // Se crea el listado a procesar
            List<Entidades.Asunto> lstAsuntosPending = new List<Entidades.Asunto>();
            try
            {
                using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
                {
                    c.Open();
                    string strDeliverPendingToday = @"
                            SELECT apa.numero, apa.operador, apa.short_description, apa.for_report, apa.sending_date, apa.date_of_register from asuntos_pendiente_asignacion as apa
                            join operadores_horarios as oph on oph.operador = apa.operador
                            where oph.dayofweek = strftime('%w', 'now', 'localtime')";
                    using (SQLiteCommand cmdDeliverPendingToday = new SQLiteCommand(strDeliverPendingToday, c))
                    {
                        using (SQLiteDataReader rdrDeliverPendingToday = cmdDeliverPendingToday.ExecuteReader())
                        {
                            while (rdrDeliverPendingToday.Read())
                            {
                                // Agregamos el item recolectado al listado
                                lstAsuntosPending.Add(new Entidades.Asunto()
                                {
                                    Numero = rdrDeliverPendingToday["numero"].ToString(),
                                    Oper = new Entidades.Operador() { UserName = rdrDeliverPendingToday["operador"].ToString() },
                                    DescripcionBreve = rdrDeliverPendingToday["short_description"].ToString(),
                                    Reportable = Convert.ToBoolean(rdrDeliverPendingToday["for_report"]),
                                    SendingDate = Convert.ToDateTime(rdrDeliverPendingToday["sending_date"]),
                                    LoadedOnSolucionameDate = Convert.ToDateTime(rdrDeliverPendingToday["date_of_register"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("AsuntoPendingDeliverTask : Error getting information from database.");
            }
            // Se devuelve el listado procesado
            return lstAsuntosPending;            
        }

        /// <summary>
        /// Checks on database for asuntos saved today
        /// </summary>
        /// <returns></returns>
        public static List<Entidades.Asunto> GetAsuntosAssignedFromToday()
        {
            // Generate a new list instance of asunto
            List<Entidades.Asunto> lstAsuntoToday = new List<Entidades.Asunto>();
            try {
                using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                    c.Open();
                    string strQueryAsuntoOfCurrentDay = "SELECT number, operator, short_description, for_report, assignation_date, sending_date from asuntos_assigned_successful where assignation_date LIKE (date('now', 'localtime') || '%') ";
                    using (SQLiteCommand cmdQueryAsuntoOfCurrentDay = new SQLiteCommand(strQueryAsuntoOfCurrentDay, c)) {
                        using (SQLiteDataReader rdrQueryAsuntoOfCurrenDay = cmdQueryAsuntoOfCurrentDay.ExecuteReader()) {
                            while (rdrQueryAsuntoOfCurrenDay.Read()) {
                                lstAsuntoToday.Add(new Entidades.Asunto()
                                {
                                    Numero = rdrQueryAsuntoOfCurrenDay["number"].ToString(),
                                    Oper = new Entidades.Operador() { UserName = rdrQueryAsuntoOfCurrenDay["operator"].ToString() },
                                    DescripcionBreve = rdrQueryAsuntoOfCurrenDay["short_description"].ToString(),
                                    Reportable = Convert.ToBoolean(rdrQueryAsuntoOfCurrenDay["for_report"]),
                                    AssignmentDate = Convert.ToDateTime(rdrQueryAsuntoOfCurrenDay["assignation_date"]),
                                    SendingDate = Convert.ToDateTime(rdrQueryAsuntoOfCurrenDay["sending_date"])
                                });
                            }
                        }
                    }
                }
                return lstAsuntoToday;
            }
            catch (Exception ex) {
                throw ex;
            }
            
        }
        
        /// <summary>
        /// Check on service database for asuntos without assignation
        /// </summary>
        /// <returns></returns>
        public static List<Entidades.Asunto> GetAsuntosWithoutAssignation()
        {
            // Creates a list to return at the end of the process
            List<Entidades.Asunto> lstAsuntosWithoutAssignation = new List<Entidades.Asunto>();
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                // Opens the connection
                c.Open();
                // Dispose a string for query on database
                string strQueryAsuntosWithoutAssignation = "SELECT number, date_of_register, short_description from asuntos_on_solucioname_without_assignation";
                using (SQLiteCommand cmdQueryAsuntosWithoutAssignation = new SQLiteCommand(strQueryAsuntosWithoutAssignation, c)) {
                    using (SQLiteDataReader rdrQueryAsuntosAssignation = cmdQueryAsuntosWithoutAssignation.ExecuteReader()) {
                        // Adds to list a new register
                        while (rdrQueryAsuntosAssignation.Read()) {
                            lstAsuntosWithoutAssignation.Add(new Entidades.Asunto()
                            {
                                Numero = rdrQueryAsuntosAssignation["number"].ToString(),
                                DescripcionBreve = rdrQueryAsuntosAssignation["short_description"].ToString(),
                                LoadedOnSolucionameDate = Convert.ToDateTime(rdrQueryAsuntosAssignation["date_of_register"])
                            });
                        }
                    }
                }
            }
            // Return processed list to the caller
            return lstAsuntosWithoutAssignation;
        }

        public static bool CheckNewAsuntosOnSolucioname()
        {
            // Set result of update
            bool isUpdated = false;
            // Start connection object
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                // Start connection operation
                c.Open();
                // Init transaction using connection
                using (SQLiteTransaction t = c.BeginTransaction()) {
                    // Gets last check time
                    DateTime lastTimeChecked = getLastCheckTime(c, t);
                    // With date information checks on mail service for new asuntos
                    Mail.Service mailCheckingService = new Mail.Service();
                    // get list of new asuntos
                    List<Entidades.Asunto> lstNewAsuntos = mailCheckingService.GetLastAsuntosAdded(lastTimeChecked).GroupBy((asunto) => asunto.Numero).Select(group => group.First()).ToList();
                    // Check if are any new on the list
                    if(lstNewAsuntos.Count > 0) {
                        // Adds all asuntos to database
                        RegisterAsuntosFromSolucioname(lstNewAsuntos, c, t);
                        // Gets last datetime value
                        DateTime lastAsuntoAdded = lstNewAsuntos.OrderByDescending((asunto) => asunto.LoadedOnSolucionameDate).Select((asunto) => asunto.LoadedOnSolucionameDate).First();
                        // Establish a new las time check value
                        setLastCheckTime(lastTimeChecked, lastAsuntoAdded, c, t);
                        // Set update result to true;
                        isUpdated = true;
                        // Complete transaction
                        t.Commit();
                    }
                }
            }
            // Return process result
            return isUpdated;
        }

        /// <summary>
        /// Validates if the asunto sent exist on assigned history
        /// </summary>
        /// <param name="prmAsuntoToCheck"></param>
        /// <returns></returns>
        public static bool Validate(Entidades.Asunto prmAsuntoToCheck)
        {
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                c.Open();
                string sQueryCheckAsuntoExistence = "SELECT 1 from asuntos_assigned_successful where number=@Number and operator=@Operator";
                using (SQLiteCommand cmdQueryCheckAsuntoExistence = new SQLiteCommand(sQueryCheckAsuntoExistence, c)) {
                    cmdQueryCheckAsuntoExistence.Parameters.Agregar("@Number", prmAsuntoToCheck.Numero);
                    cmdQueryCheckAsuntoExistence.Parameters.Agregar("@Operator", prmAsuntoToCheck.Oper.UserName);
                    using (SQLiteDataReader rdrQueryCheckAsunto = cmdQueryCheckAsuntoExistence.ExecuteReader()) {
                        if (!rdrQueryCheckAsunto.Read()) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Validate a collection of asuntos. if one of asunto number is founded, reject petition
        /// </summary>
        /// <param name="lstAsuntos"></param>
        /// <returns></returns>
        public static bool Validate(List<Entidades.Asunto> lstAsuntos)
        {
            // Get list of duplicated asunto numbers
            List<Entidades.Asunto> lstDuplicatedAsuntos = getAssignedAndDuplicatedAsuntoList(lstAsuntos);
            if(lstDuplicatedAsuntos.Exists( asuntoDuplicated => 
                lstAsuntos.FindAll(asuntoInLoadList => asuntoInLoadList.Numero == asuntoDuplicated.Numero)
                    .Exists( asuntoMatch => asuntoMatch.Oper.UserName == asuntoDuplicated.Oper.UserName
                    ))) {
                return false;
            }                        
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstOfAsuntoTocheck"></param>
        /// <returns></returns>
        public static string GetDuplicatedConflictedAsuntoNumbers(List<Entidades.Asunto> lstOfAsuntoTocheck)
        {
            // Get duplicated asuntos from database
            List<Entidades.Asunto> lstDuplicatedAsuntos = getAssignedAndDuplicatedAsuntoList(lstOfAsuntoTocheck);
            string[] conflictedAsuntos = lstDuplicatedAsuntos.FindAll(
                asuntoDuplicated => lstOfAsuntoTocheck.Exists(
                    asuntoToLoad => asuntoDuplicated.Oper.UserName == asuntoToLoad.Oper.UserName && asuntoDuplicated.Numero == asuntoToLoad.Numero)).
                    Select(asuntoConflicted => asuntoConflicted.Numero).ToArray();
            // Return processed list to caller
            return String.Join(", ", conflictedAsuntos);
        }

        /// <summary>
        /// Loads from database a list of phrases who filters mails
        /// </summary>
        public static void LoadFiltersPhraseForMails()
        {
            lstFiltersForMails = new List<AsuntoFilter>();
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                c.Open();
                string strQueryFilterPhrases = "SELECT * from asunto_category_filter";
                using (SQLiteCommand cmdQueryFilterPhrases = new SQLiteCommand(strQueryFilterPhrases, c)) {
                    using (SQLiteDataReader rdrQueryFilterPhrases = cmdQueryFilterPhrases.ExecuteReader()) {
                        while (rdrQueryFilterPhrases.Read()) {
                            lstFiltersForMails.Add(new AsuntoFilter()
                            {
                                Id = Convert.ToInt16(rdrQueryFilterPhrases["id"]),
                                Phrase = rdrQueryFilterPhrases["phrase"].ToString(),
                                ShortDescription = rdrQueryFilterPhrases["short_description"].ToString()
                            });
                        }
                    }
                }
            }
            Log.Info(CLASSNAME, "Filter for mails loaded correctly...");
        }


        private static DateTime getLastCheckTime(SQLiteConnection conn, SQLiteTransaction tran)
        {
            // Create item to return in process
            DateTime dtmProcessed;
            // Generate string to query database
            string strQueryLastCheckTime = "SELECT time_of_register from last_check_asunto_on_solucioname where date_of_register=@DateRegister";
            using (SQLiteCommand cmdQueryLastCheckTime = new SQLiteCommand(strQueryLastCheckTime, conn, tran)) {
                cmdQueryLastCheckTime.Parameters.Agregar("@DateRegister", dayToCheckService.ToString("yyyy-MM-dd"));
                using (SQLiteDataReader rdrQueryLastCheckTime = cmdQueryLastCheckTime.ExecuteReader()) {
                    if (rdrQueryLastCheckTime.Read()) {
                        dtmProcessed = Convert.ToDateTime(rdrQueryLastCheckTime["time_of_register"]);
                        if (Config.TEST_MAILING_SERVICE) {
                            dtmProcessed = Convert.ToDateTime(dayToCheckService.
                                AddHours(dtmProcessed.Hour).
                                AddMinutes(dtmProcessed.Minute).
                                AddSeconds(dtmProcessed.Second));
                        }                        
                    }
                    else {
                        dtmProcessed = dayToCheckService;
                    }
                }
            }
            // Return processed item
            return dtmProcessed;
        }

        /// <summary>
        /// Set a new time based on last received asunto on system
        /// </summary>
        /// <param name="prmPreviousCheckTime"></param>
        /// <param name="prmLastTimeChecked"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        private static void setLastCheckTime(DateTime prmPreviousCheckTime, DateTime prmLastTimeChecked, SQLiteConnection conn, SQLiteTransaction tran)
        {
            // Set an update parameter
            string querySetCheckTime = "UPDATE last_check_asunto_on_solucioname set time_of_register=@TimeRegister where date_of_register=@DateRegister";
            // If previous time is equal to today, indicates there is no row loaded of today
            if (prmPreviousCheckTime == dayToCheckService) querySetCheckTime = "INSERT INTO last_check_asunto_on_solucioname values (@DateRegister,@TimeRegister)";
            using (SQLiteCommand cmdSetCheckTime = new SQLiteCommand(querySetCheckTime, conn, tran)) {
                // Parametrizes hour add
                cmdSetCheckTime.Parameters.Agregar("@DateRegister", prmLastTimeChecked.ToString("yyyy-MM-dd"));
                cmdSetCheckTime.Parameters.Agregar("@TimeRegister", prmLastTimeChecked.ToString("HH:mm:ss"));
                // Execute add operation
                cmdSetCheckTime.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Register new asuntos on service database
        /// </summary>
        private static void RegisterAsuntosFromSolucioname(List<Entidades.Asunto> lstAsuntosToRegister, SQLiteConnection conn, SQLiteTransaction tran)
        {
            // Prepare string for insertion
            string strInsertNewAsunto = "INSERT INTO asuntos_on_solucioname_without_assignation values (@Asunto, @RegisterTime, @ShortDescription)";
            using (SQLiteCommand cmdInsertNewAsunto = new SQLiteCommand(strInsertNewAsunto, conn, tran)) {
                // Iterates over all asuntos
                foreach (var asunto in lstAsuntosToRegister) {
                    // Parametrize insertion
                    cmdInsertNewAsunto.Parameters.Agregar("@Asunto", asunto.Numero);
                    cmdInsertNewAsunto.Parameters.Agregar("@RegisterTime", asunto.LoadedOnSolucionameDate);
                    cmdInsertNewAsunto.Parameters.Agregar("@ShortDescription", asunto.DescripcionBreve);
                    // Execute operation
                    cmdInsertNewAsunto.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Process a list of asuntos and save number only on a string with format for query on database
        /// </summary>
        /// <param name="lstAsuntosToParse"></param>
        /// <returns></returns>
        private static string getNumberOfAsuntosOnlyFromListForDatabaseRead(List<Entidades.Asunto> lstAsuntosToParse)
        {
            // Get all numebers of asunto and save on array
            string[] unformattedListOfasuntos = lstAsuntosToParse.Select(asunto => asunto.Numero).ToArray();
            // Unify all numbers on one string, separating by ', '. This seperator is for query formatting purposes
            string strLstOfAsuntos = String.Join("', '", unformattedListOfasuntos);
            // The join don't adds ' on start and the end, manually adds for correct formatting
            strLstOfAsuntos.Insert(INDEX_START_STRING, "'");
            strLstOfAsuntos.Insert(strLstOfAsuntos.Length - 1, "'");
            // Return processed value
            return strLstOfAsuntos;
        }

        /// <summary>
        /// Process a list passed on parameter and get from database coincidences
        /// </summary>
        /// <param name="lstAsuntoSource"></param>
        /// <returns></returns>
        private static List<Entidades.Asunto> getAssignedAndDuplicatedAsuntoList(List<Entidades.Asunto> lstAsuntoSource)
        {
            // Generate a new list to return in process
            List<Entidades.Asunto> lstDuplicatedAsuntos = new List<Entidades.Asunto>();
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                c.Open();
                // Validates existence of asunto in assigned successfull list
                string strQueryValidateAssignedAsuntoBatch = "SELECT number, operator from asuntos_assigned_successful where number in (@AsuntoList)";
                using (SQLiteCommand cmdQueryValidateAssignedAsuntoBatch = new SQLiteCommand(strQueryValidateAssignedAsuntoBatch, c)) {
                    cmdQueryValidateAssignedAsuntoBatch.Parameters.Agregar("@AsuntoList", getNumberOfAsuntosOnlyFromListForDatabaseRead(lstAsuntoSource));
                    using (SQLiteDataReader rdrQueryValidateAssignedAsuntoBatch = cmdQueryValidateAssignedAsuntoBatch.ExecuteReader()) {
                        while (rdrQueryValidateAssignedAsuntoBatch.Read()) {
                            Entidades.Asunto asuntoFinded = new Entidades.Asunto()
                            {
                                Numero = rdrQueryValidateAssignedAsuntoBatch["number"].ToString(),
                                Oper = new Entidades.Operador() { UserName = rdrQueryValidateAssignedAsuntoBatch["operator"].ToString() }
                            };
                            // Add founded duplicate on the list
                            lstDuplicatedAsuntos.Add(asuntoFinded);
                        }
                    }
                }
            }
            // Return processed list
            return lstDuplicatedAsuntos;
        }
    }
}
