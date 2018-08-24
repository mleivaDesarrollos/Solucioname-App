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
        public static void RemoveFromQueue(Entidades.Asunto asunto)
        {            
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                c.Open();
                string strRemoveAsuntoFromQueue = "DELETE FROM asuntos_pendiente_asignacion where numero=@Numero and operador=@Oper";

                using (SQLiteCommand cmdRemoveAsuntoFromQueue = new SQLiteCommand(strRemoveAsuntoFromQueue,c))
                {
                    // Parametrizamos las variables pasadas por parametro
                    cmdRemoveAsuntoFromQueue.Parameters.Agregar("@Numero", asunto.Numero);
                    cmdRemoveAsuntoFromQueue.Parameters.Agregar("@Oper", asunto.Oper.UserName);
                    // Ejecutamos el comando
                    cmdRemoveAsuntoFromQueue.ExecuteNonQuery();
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
