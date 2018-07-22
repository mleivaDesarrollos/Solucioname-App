using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.Util;

namespace Datos
{
    public class EstadoActuacion
    {
        /// <summary>
        /// Agrega los estados de la actuación a la base de datos
        /// </summary>
        /// <param name="pActuacion"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        public static void Agregar(Entidades.Actuacion pActuacion, SQLiteConnection conn, SQLiteTransaction trans)
        {
            // Preparamos la cadena que se usara para la inserción
            String sAgregarEstadoActuacion = "INSERT INTO actuacion_estados (numero, operador, fechaHora, detalle, ord, tipo) VALUES (@Numero, @Operador, @FechaHora, @Detalle, @Ord, @Tipo)";
            using (SQLiteCommand cmdAgregarEstadoAct = new SQLiteCommand(sAgregarEstadoActuacion, conn, trans))
            {
                // Parametrizamos los valores que se repetiran en cada inserción a la base
                cmdAgregarEstadoAct.Parameters.Agregar("@Numero", pActuacion.Numero);
                cmdAgregarEstadoAct.Parameters.Agregar("@Operador", pActuacion.Operador.UserName);
                // Recorremos los estados almacenados
                foreach (var estado in pActuacion.Estados)
                {
                    // Parametrizamos los detalles especificos de cada orden
                    cmdAgregarEstadoAct.Parameters.Agregar("@Ord", estado.Ord);
                    cmdAgregarEstadoAct.Parameters.Agregar("@Tipo", estado.Tipo.Id);
                    cmdAgregarEstadoAct.Parameters.Agregar("@FechaHora", estado.FechaHora);
                    cmdAgregarEstadoAct.Parameters.Agregar("@Detalle", estado.Detalle);
                    // Una vez cargado todos los parametros procedemos a ejecutar el comando
                    cmdAgregarEstadoAct.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Procedimiento que elimina los estados de actuación almacenados en la base de datos
        /// </summary>
        /// <param name="pActuacion"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        public static void Eliminar(Entidades.Actuacion pActuacion, SQLiteConnection conn, SQLiteTransaction trans)
        {
            // Cadena de caracteres que se utilizará para realizar la baja de estados
            string sCmdEliminarEstados = "DELETE FROM actuacion_estados where numero=@Numero and operador=@Operador";
            // Comando que ejecutará la tarea de eliminación
            using (SQLiteCommand cmdEliminarEstados = new SQLiteCommand(sCmdEliminarEstados, conn, trans))
            {
                // Parametrizamos el comando
                cmdEliminarEstados.Parameters.Agregar("@Numero", pActuacion.Numero);
                cmdEliminarEstados.Parameters.Agregar("@Operador", pActuacion.Operador.UserName);
                // Ejecutamos el comando
                cmdEliminarEstados.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Modifica los estados actuación cargados en la entidad sobre la base de datos
        /// </summary>
        /// <param name="pActuacion"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        public static void Modificar(Entidades.Actuacion pActuacion, SQLiteConnection conn, SQLiteTransaction trans)
        {
            // Eliminamos en primera instancia todos los estados relacionados con la actuacio´n
            Eliminar(pActuacion, conn, trans);
            // Cargamos los estados que se encuentran almacenados dentro de la entidad
            Agregar(pActuacion, conn, trans);
        }

        /// <summary>
        /// Trae un listado completo de estados a través de una actuación
        /// </summary>
        /// <param name="entAct"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<Entidades.Estado> TraerSegunActuacion(Entidades.Actuacion entAct, SQLiteConnection conn)
        {
            // Generamos el listado de estados a devolver
            List<Entidades.Estado> lstEstados = new List<Entidades.Estado>();
            // Generamos la cadena de caracteres que se utilizara en la consulta
            string strCmdEstadosActuacion = "SELECT ord, fechaHora, tipo, detalle FROM actuacion_estados WHERE numero=@Numero and operador=@Operador";
            // Generamos el comando a ejecutar 
            using (SQLiteCommand cmdEstadosActuacion = new SQLiteCommand(strCmdEstadosActuacion, conn))
            {
                // Parametrizamos los comandos antes de ejecutar el lector
                cmdEstadosActuacion.Parameters.Agregar("@Numero", entAct.Numero);
                cmdEstadosActuacion.Parameters.Agregar("@Operador", entAct.Operador.UserName);
                // Ejecutamos el lector de estados
                using (SQLiteDataReader rdrLectorEstados = cmdEstadosActuacion.ExecuteReader())
                {
                    // Leemo todos los registros recolectados
                    while (rdrLectorEstados.Read())
                    {
                        // Generamos la entidad a agregar al listado de estados
                        Entidades.Estado entEstado = new Entidades.Estado()
                        {
                            Ord = Convert.ToInt32(rdrLectorEstados["ord"]),
                            FechaHora = Convert.ToDateTime(rdrLectorEstados["fechaHora"]),
                            Detalle = rdrLectorEstados["detalle"].ToString(),
                            Tipo = new Entidades.TipoEstado() { Id = Convert.ToInt32(rdrLectorEstados["tipo"])}
                        };
                        // Agregamos el estado al listado de estados
                        lstEstados.Add(entEstado);
                    }
                }

            }
            // Devolvemos el listado luego de ser procesado
            return lstEstados;
        }
    }
}
