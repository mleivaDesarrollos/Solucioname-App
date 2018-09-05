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

        /// <summary>
        /// Agrega un asunto a la base de datos de respaldo
        /// </summary>
        /// <param name="asunto"></param>
        public static void AddToQueue(Entidades.Asunto asunto)
        {
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                c.Open();
                string strAddAsuntoToQueue = "INSERT INTO asuntos_pendiente_asignacion VALUES (@Numero, @Oper, @ShortDesc, @Reporting)";
                using (SQLiteCommand cmdAddAsuntoToQueue = new SQLiteCommand(strAddAsuntoToQueue, c))
                {
                    // Parametrizamos los valores del asunto cargado
                    cmdAddAsuntoToQueue.Parameters.Agregar("@Numero", asunto.Numero);
                    cmdAddAsuntoToQueue.Parameters.Agregar("@Oper", asunto.Oper.UserName);
                    cmdAddAsuntoToQueue.Parameters.Agregar("@ShortDesc", asunto.DescripcionBreve);
                    cmdAddAsuntoToQueue.Parameters.Agregar("@Reporting", asunto.Reportable);
                    // Ejecutamos el comando parametrizado
                    cmdAddAsuntoToQueue.ExecuteNonQuery();
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
                    string sDeleteFromPendientes = "DELETE FROM asuntos_pendiente_asignacion where numero=@Numero and operador=@Oper";
                    using (SQLiteCommand cmdDeleteFromPendientes = new SQLiteCommand(sDeleteFromPendientes, c, t)) {
                        cmdDeleteFromPendientes.Parameters.Agregar("@Numero", asunto.Numero);
                        cmdDeleteFromPendientes.Parameters.Agregar("@Oper", asunto.Oper.UserName);
                        cmdDeleteFromPendientes.ExecuteNonQuery();
                    }
                    // On delete go to add on assigned table
                    string sAddAssignedAsuntoToRegistry = "INSERT INTO asuntos_assigned_successful (number, operator, short_description, for_report, assignation_date) values (@Number, @Oper, @ShortDesc, @ForReport, @AssignDate)";

                    using (SQLiteCommand cmdInsertAssignedAsuntos = new SQLiteCommand(sAddAssignedAsuntoToRegistry, c, t)) {
                        cmdInsertAssignedAsuntos.Parameters.Agregar("@Number", asunto.Numero);
                        cmdInsertAssignedAsuntos.Parameters.Agregar("@Oper", asunto.Oper.UserName);
                        cmdInsertAssignedAsuntos.Parameters.Agregar("@ShortDesc", asunto.DescripcionBreve);
                        cmdInsertAssignedAsuntos.Parameters.Agregar("@ForReport", asunto.Reportable);
                        cmdInsertAssignedAsuntos.Parameters.Agregar("@AssignDate", asunto.AssignmentTime);

                        cmdInsertAssignedAsuntos.ExecuteNonQuery();
                    }
                    // When the registry is saved, proceed to calculate column to save
                    //Balance.AddEntry(asunto, c, t);
                    // if all operations are successful, commit changes over database
                    t.Commit();
                }
            }
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
                            SELECT apa.numero, apa.operador, apa.short_description, apa.for_report from asuntos_pendiente_asignacion as apa
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
                                    Reportable = Convert.ToBoolean(rdrDeliverPendingToday["for_report"])
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
                    string strQueryAsuntoOfCurrentDay = "SELECT * from asuntos_assigned_successful where assignation_date LIKE (date('now', 'localtime') || '%') ";
                    using (SQLiteCommand cmdQueryAsuntoOfCurrentDay = new SQLiteCommand(strQueryAsuntoOfCurrentDay, c)) {
                        using (SQLiteDataReader rdrQueryAsuntoOfCurrenDay = cmdQueryAsuntoOfCurrentDay.ExecuteReader()) {
                            while (rdrQueryAsuntoOfCurrenDay.Read()) {
                                lstAsuntoToday.Add(new Entidades.Asunto()
                                {
                                    Numero = rdrQueryAsuntoOfCurrenDay["number"].ToString(),
                                    Oper = new Entidades.Operador() { UserName = rdrQueryAsuntoOfCurrenDay["operator"].ToString() },
                                    DescripcionBreve = rdrQueryAsuntoOfCurrenDay["short_description"].ToString(),
                                    Reportable = Convert.ToBoolean(rdrQueryAsuntoOfCurrenDay["for_report"]),
                                    AssignmentTime = Convert.ToDateTime(rdrQueryAsuntoOfCurrenDay["assignation_date"])
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
    }
}
