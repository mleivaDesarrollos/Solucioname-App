using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.Util;

namespace Datos
{
    /// <summary>
    /// Interfaz de datos para las actuaciones - Interactúa con el motor SQLITE para recuperar y almacenar información
    /// </summary>
    public class Actuacion
    {
        /// <summary>
        /// Gestiona el alta de una actuación en la base de datos
        /// </summary>
        /// <param name="pActuacion"></param>        
        public static void Agregar(Entidades.Asunto pAsunto, SQLiteConnection conn, SQLiteTransaction trans)
        {
            // Preparamos la cadena de caracteres que se utilizará para realizar el ingreso
            String strCmdAgregarActuacion = "INSERT INTO actuacion (numero, asunto_relacionado, operador, tipo, remedy_relacionado, grupo_asignado) values(@Numero, @AsuntoRelacionado, @Operador, @Tipo, @RemedyRelacionado, @GrupoAsignado)";
            // Disponemos de forma simplificada la variable actuación
            Entidades.Actuacion pActuacion = pAsunto.Actuacion;
            // Preparamos el comando a ejecutar
            using (SQLiteCommand cmdAgregarActuacion = new SQLiteCommand(strCmdAgregarActuacion, conn, trans))
            {
                // Parametrizamos los valores recibidos
                cmdAgregarActuacion.Parameters.Agregar("@Numero", pActuacion.Numero);
                cmdAgregarActuacion.Parameters.Agregar("@AsuntoRelacionado", pAsunto.Numero);
                cmdAgregarActuacion.Parameters.Agregar("@Operador", pActuacion.Operador.UserName);
                cmdAgregarActuacion.Parameters.Agregar("@Tipo", pActuacion.Tipo.Id);
                cmdAgregarActuacion.Parameters.Agregar("@RemedyRelacionado", pActuacion.RemedyRelacionado);
                cmdAgregarActuacion.Parameters.Agregar("@GrupoAsignado", pActuacion.Grupo.Id);
                // Ejecutamos el comando
                cmdAgregarActuacion.ExecuteNonQuery();
            }
            // Agregamos los estados de actuación a la base de datos
            EstadoActuacion.Agregar(pActuacion, conn, trans);
        }

        /// <summary>
        /// Procesa la baja de una o varias actuaciones
        /// </summary>
        /// <param name="pAsunto"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        public static void Eliminar(Entidades.Asunto pAsunto, SQLiteConnection conn, SQLiteTransaction trans)
        {
            // Parametrizamos el string que se utilizará para eliminar la actuación
            // Recolectamos los números de actuacion para poder eliminarlos también de actuaciones estados
            string sConsultaActuaciones = "SELECT numero FROM actuacion WHERE asunto_relacionado=@Asunto and operador=@Operador";
            // Generamos un comando para realizar la lectura total de actuaciones relacionadas con el asunto
            using (SQLiteCommand cmdEliminarActuacion = new SQLiteCommand(sConsultaActuaciones, conn, trans))
            {
                // Parametrizamos la consulta
                cmdEliminarActuacion.Parameters.Agregar("@Asunto", pAsunto.Numero);
                cmdEliminarActuacion.Parameters.Agregar("@Operador", pAsunto.Operador.UserName);
                // Ejecutamos el lector relacionado al comando
                using (SQLiteDataReader rdrActRelacionadas = cmdEliminarActuacion.ExecuteReader())
                {
                    // Generamos un listado de actuaciones donde almacenaremos los resultados recolectados
                    List<Entidades.Actuacion> lstAct = new List<Entidades.Actuacion>();
                    // Leemos los resultados obtenidos
                    while (rdrActRelacionadas.Read())
                    {
                        // Generamos una nueva entidad de actuacion
                        Entidades.Actuacion entAct = new Entidades.Actuacion() { Numero = rdrActRelacionadas["numero"].ToString(), Operador = pAsunto.Operador };
                        // Agregamos la entidad al listado
                        lstAct.Add(entAct);
                    }
                    // Recorremos el listado de actuaciones
                    foreach (Entidades.Actuacion entAct in lstAct)
                    {
                        // Eliminamos cada una de los estados de actuación relacionados
                        EstadoActuacion.Eliminar(entAct, conn, trans);
                    }
                }
            }
            string sCmdEliminarActuacion = "DELETE FROM actuacion where asunto_relacionado=@Numero and operador=@Operador";
            // Generamos un comando que permitirá eliminar la actuación de la base de datos
            using (SQLiteCommand cmdEliminarActuacion = new SQLiteCommand(sCmdEliminarActuacion, conn, trans))
            {
                // Parametrizamos la consulta
                cmdEliminarActuacion.Parameters.Agregar("@Numero", pAsunto.Numero);
                cmdEliminarActuacion.Parameters.Agregar("@Operador", pAsunto.Operador.UserName);
                // Ejecutamos el comando
                cmdEliminarActuacion.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Modifica los estados de actuación en la base de datos
        /// Como las actuaciones relacionadas con asuntos pueden eliminarse los estados, y esto afecta a las actuaciones, se deben eliminar en primera instancia las actuaciones relacionadas y 
        /// </summary>
        /// <param name="pAsunto"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        public static void Modificar(Entidades.Asunto pAsunto, SQLiteConnection conn, SQLiteTransaction trans)
        {
            // Disponemos la variable de actuación separada para mejorar la legibilidad del código
            Entidades.Actuacion pActuacion = pAsunto.Actuacion;
            // Tenemos que recolectar de la base de datos las actuaciones relacionadas con el asunto, para que asi podamos eliminarlas y poder hacer el ingreso sin dejar actuaciones huerfanas
            Eliminar(pAsunto, conn, trans);
            // Se agrega las actuaciones modificadas
            Agregar(pAsunto, conn, trans);
        }

        /// <summary>
        /// Trae una actuación desde la base de datos
        /// </summary>
        /// <param name="pAsunto"></param>
        /// <param name="conn"></param>
        public static Entidades.Actuacion TraerActuaciones(Entidades.Asunto pAsunto, SQLiteConnection conn)
        {
            // Generamos el listado que luego devolveremos
            Entidades.Actuacion entActuacion = null;
            // Preparamos la cadena de caracteres para que sea usada en la consulta
            string strCmdConsultaActuacion = "SELECT numero, tipo, remedy_relacionado, grupo_asignado FROM actuacion WHERE asunto_relacionado=@AsuntoRelacionado and operador=@Operador";
            // Disponemos del comando que ejecutara la consulta
            using (SQLiteCommand cmdConsultaActuacion = new SQLiteCommand(strCmdConsultaActuacion, conn))
            {
                // Parametrizamos la consulta a ejecutar
                cmdConsultaActuacion.Parameters.Agregar("@AsuntoRelacionado", pAsunto.Numero);
                cmdConsultaActuacion.Parameters.Agregar("@Operador", pAsunto.Operador.UserName);
                // Ejecutamos el lector de datos sobre el comando generado
                using (SQLiteDataReader rdrActuacion = cmdConsultaActuacion.ExecuteReader())
                {
                    // Realizamos lectura sobre todos los resultados
                    if (rdrActuacion.Read())
                    {
                        // Generamos la entidad a agregar en el listado
                        entActuacion = new Entidades.Actuacion()
                        {
                            Numero = rdrActuacion["numero"].ToString(),
                            Grupo = new Entidades.GrupoResolutor() { Id = Convert.ToInt32(rdrActuacion["grupo_asignado"]) },
                            Tipo = new Entidades.ActuacionTipo() { Id = Convert.ToInt32(rdrActuacion["tipo"]) },
                            RemedyRelacionado = rdrActuacion["remedy_relacionado"].ToString()                            
                        };
                        entActuacion.Operador = pAsunto.Operador;
                        // Traemos los estados de actuación y lo cargamos sobre la entidad
                        entActuacion.Estados = EstadoActuacion.TraerSegunActuacion(entActuacion, conn);
                    }
                }
            }
            // Devolvemos el listado procesado
            return entActuacion;
        }
    }
}
