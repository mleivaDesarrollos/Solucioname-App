using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using Datos.Util;

namespace Datos
{
    public partial class EstadoAsunto
    {
        

        /// <summary>
        /// Consulta sobre la base de datos por el listado de estados asuntos cargados segun el asunto pasado por parametro
        /// Fecha de creación : 06/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pEntAsunto"></param>
        /// <returns></returns>
        public static List<Entidades.Estado> TraerListaEstadosPorAsunto(Entidades.Asunto pEntAsunto)
        {
            // Generamos la lista vacía a devolver
            List<Entidades.Estado> lstEstadoAsunto = new List<Entidades.Estado>();
            // Generamos un nuevo objeto de conexión
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                // Abrimos la conexión de la base de datos
                c.Open();
                // Disponemos de la cadena que realizará la consulta
                String strConsultaEstados = "SELECT fechaHora, detalle, ord, tipo FROM asuntos_estados WHERE numero=@Numero and operador=@Operador order by ord ASC";
                using (SQLiteCommand cmdConsultaEstados = new SQLiteCommand(strConsultaEstados, c))
                {
                    // Parametrizamos la consulta
                    cmdConsultaEstados.Parameters.Agregar("@Numero", pEntAsunto.Numero);
                    cmdConsultaEstados.Parameters.Agregar("@Operador", pEntAsunto.Oper.UserName);
                    // Leemos los resultados obtenidos
                    using (SQLiteDataReader rdrAsuntoEstados = cmdConsultaEstados.ExecuteReader())
                    {
                        while (rdrAsuntoEstados.Read())
                        {
                            Entidades.Estado estadoLeido = new Entidades.Estado();
                            Entidades.TipoEstado estadoLeidoTipo = new Entidades.TipoEstado();
                            estadoLeido.FechaHora = Convert.ToDateTime(rdrAsuntoEstados["fechaHora"]);
                            estadoLeido.Detalle = rdrAsuntoEstados["detalle"].ToString();
                            estadoLeido.Ord = Convert.ToInt32(rdrAsuntoEstados["ord"]);
                            estadoLeidoTipo.Id = Convert.ToInt32(rdrAsuntoEstados["tipo"]);
                            estadoLeido.Tipo = estadoLeidoTipo;
                            lstEstadoAsunto.Add(estadoLeido);
                        }
                    }
                }

            }
            // Devolvemos la lista procesada.
            return lstEstadoAsunto;            
        }
                
        /// <summary>
        /// Agrega los estados cargados sobre el asunto a la base de datos
        /// </summary>
        /// <param name="pAsunto"></param>
        /// <param name="c"></param>
        /// <param name="t"></param>
        public static void AgregarEstadoPorAsunto(Entidades.Asunto pAsunto, SQLiteConnection c, SQLiteTransaction t)
        {
            String strIngresarEstados = "INSERT INTO asuntos_estados (numero, operador, fechaHora, detalle, ord, tipo) values (@Numero, @Operador, @FechaHora, @Detalle, @Ord, @Tipo)";
            // Preparamos el comando a ejecutar para ingresar los estados
            using (SQLiteCommand cmdIngresarEstados = new SQLiteCommand(strIngresarEstados, c, t))
            {
                // Agregamos los valores repetidos
                cmdIngresarEstados.Parameters.Agregar("@Numero", pAsunto.Numero);
                cmdIngresarEstados.Parameters.Agregar("@Operador", pAsunto.Oper.UserName);
                // Recorremos los estados almacenados
                foreach (var estados in pAsunto.Estados)
                {
                    // Parametrizamos el estado
                    cmdIngresarEstados.Parameters.Agregar("@FechaHora", SQLite.FormatearFechaHora(estados.FechaHora));
                    cmdIngresarEstados.Parameters.Agregar("@Detalle", estados.Detalle);
                    cmdIngresarEstados.Parameters.Agregar("@Ord", estados.Ord);
                    cmdIngresarEstados.Parameters.Agregar("@Tipo", estados.Tipo.Id);
                    // Ejecutamos la consulta sobre la base de datos
                    cmdIngresarEstados.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Ejecutamos una solicitud de eliminación de estados. Este metodo es de interface para conectar con métodos de otra clase
        /// </summary>
        /// <param name="pAsunto"></param>
        public static void EliminarEstadosPorAsunto(Entidades.Asunto pAsunto, SQLiteConnection conn, SQLiteTransaction trans)
        {
            // Preparamos el primer comando de eliminación
            String strDeleteEstadosAsunto = "DELETE FROM asuntos_estados where numero=@Numero and operador=@Operador";
            // Generamos el comando de eliminación para resolver los estados en primera instancia
            using (SQLiteCommand cmdDeleteEstadosAsuntos = new SQLiteCommand(strDeleteEstadosAsunto, conn, trans))
            {
                // Parametrizamos la consulta
                cmdDeleteEstadosAsuntos.Parameters.Agregar("@Numero", pAsunto.Numero);
                cmdDeleteEstadosAsuntos.Parameters.Agregar("@Operador", pAsunto.Oper.UserName);
                // Ejecutamos el Query
                cmdDeleteEstadosAsuntos.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Actualiza la base de datos bajo la entidad comunicada
        /// </summary>
        /// <param name="pAsunto"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        public static void ActualizarEstadosPorAsunto(Entidades.Asunto pAsunto, SQLiteConnection conn, SQLiteTransaction trans)
        {
            // Eliminamos los estados de asunto
            EliminarEstadosPorAsunto(pAsunto, conn, trans);
            // Agregamos los estados cargados del asunto
            AgregarEstadoPorAsunto(pAsunto, conn, trans);
        }
        
    }
}
