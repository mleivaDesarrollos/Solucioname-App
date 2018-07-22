using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    /// <summary>
    /// Clase publica que almacena los estados posibles, sean de actuación o asuntos
    /// </summary>
    public class TipoEstado
    {
        /// <summary>
        /// Devuelve un listado con tipos de estado de asunto
        /// </summary>
        /// <returns>Listado de tipos de estado de asunto</returns>
        public List<Entidades.TipoEstado> TraerTipoEstado()
        {
            // Preparamos el objeto a devolver
            List<Entidades.TipoEstado> lstTipoEstado = new List<Entidades.TipoEstado>();

            // Generamos el objeto de conexión
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                // Abrimos la conexión
                c.Open();
                String sConsultaTipoEstado = @" SELECT id,descripcion, requiere_detalle, inicio_habilitado, unico,                                    corte_ciclo, requiere_actuacion, gestion
                                                FROM estado_tipo
                                                ";
                // Disponemos del comando de lectura
                using (SQLiteCommand cmdLecturaTipoEstado = new SQLiteCommand(sConsultaTipoEstado, c))
                {
                    // Disponemos del objeto de lecutra para recorrer los elementos obtenido de base de datos
                    using (SQLiteDataReader rdrLecturaTipoEstado = cmdLecturaTipoEstado.ExecuteReader())
                    {
                        // Leemos los resultados obtenidos
                        while (rdrLecturaTipoEstado.Read())
                        {
                            // Generamos la entidad que utilizaremos durante el transcurso del aplicativo
                            Entidades.TipoEstado entTipoEstado = new Entidades.TipoEstado();
                            entTipoEstado.Id = Convert.ToInt32(rdrLecturaTipoEstado["id"]);
                            entTipoEstado.Descripcion = rdrLecturaTipoEstado["descripcion"].ToString();
                            entTipoEstado.RequiereDetalle = Convert.ToBoolean(rdrLecturaTipoEstado["requiere_detalle"]);
                            entTipoEstado.InicioHabilitado = Convert.ToBoolean(rdrLecturaTipoEstado["inicio_habilitado"]);
                            entTipoEstado.CorteCiclo = Convert.ToBoolean(rdrLecturaTipoEstado["corte_ciclo"]);
                            entTipoEstado.RequiereActuacion = ObtenerRequerimientoActuacion(rdrLecturaTipoEstado["requiere_actuacion"].ToString());
                            entTipoEstado.Unico = Convert.ToBoolean(rdrLecturaTipoEstado["unico"]);
                            List<Entidades.TipoEstado> lstRelacionesTipo = new List<Entidades.TipoEstado>();
                            #region almacenamiento_relaciones_tipo
                            // Nota:  Es posible que haya que refactorizar esta porción de código. Si la logica de negocio va a requerir mas información que solo el número de ID y la descripción, es posible que tipos de estados deban ser cargados completamente dependiendo de la necesidad de la aplicación.
                            // Una refactorización posible sería unicamente cagar el listado de estados desde la logica, y luego logica procese un pedido a la base de datos solicitando unicamente los ID destino de cada foreach de tipo revisado. Una vez recolectado los IDs se recorre el listado de estados cargado y se almacenan las entidades dentro del listado de permitidos

                            // Se debe recorrer las relaciones vinculadas y permitidas con cada uno de los tipos de estados
                            String sRelacionesAsunto = "SELECT tipo_destino_id, tipo_destino_descripcion FROM VistaTiposRelacionesAdmitidas WHERE tipo_origen_id=@Id";
                            using (SQLiteCommand cmdLecturaRelacionesEstado = new SQLiteCommand(sRelacionesAsunto, c))
                            {
                                // Parametrizamos el comando
                                cmdLecturaRelacionesEstado.Parameters.Add(new SQLiteParameter()
                                {
                                    ParameterName = "@Id",
                                    Value = entTipoEstado.Id,
                                    DbType = System.Data.DbType.Int32
                                });
                                // Preparamos el lector de relaciones vinculadas con el ID actual
                                using (SQLiteDataReader rdrLectorRelaciones = cmdLecturaRelacionesEstado.ExecuteReader())
                                {
                                    while (rdrLectorRelaciones.Read())
                                    {
                                        // Generamos una entidad que almacenara solo el id y la descripción del id
                                        Entidades.TipoEstado entRelTipo = new Entidades.TipoEstado()
                                        {
                                            Id = Convert.ToInt32(rdrLectorRelaciones["tipo_destino_id"]),
                                            Descripcion = rdrLectorRelaciones["tipo_destino_descripcion"].ToString()
                                        };
                                        // Almacenamos el ID dentro del listado de relaciones
                                        lstRelacionesTipo.Add(entRelTipo);
                                    }
                                }
                            }
                            #endregion
                            entTipoEstado.RelacionesPermitidas = lstRelacionesTipo;
                            entTipoEstado.Gestion = new Entidades.Gestion()
                            {
                                Id = Convert.ToInt32(rdrLecturaTipoEstado["gestion"])
                            };
                            lstTipoEstado.Add(entTipoEstado);
                        }
                    }
                }
            }
            // Rellenamos el listado de relaciones de cada uno de los tipos de estados con sus datos completos
            foreach (Entidades.TipoEstado tipoEstado in lstTipoEstado)
            {
                for (int i = 0; i < tipoEstado.RelacionesPermitidas.Count; i++)
                {
                    tipoEstado.RelacionesPermitidas[i] = lstTipoEstado.Find((te) => te.Id == tipoEstado.RelacionesPermitidas[i].Id);
                }
            }
            // Lo devolvemos luego de procesarlo
            return lstTipoEstado;
        }

        /// <summary>
        /// Convierte el string obtenido de la base de datos a una enumeración aceptada por el tipo de dato
        /// </summary>
        /// <param name="pDatoDesdeBase"></param>
        /// <returns></returns>
        private Entidades.TipoEstado.SolicitaActuacion ObtenerRequerimientoActuacion(string pDatoDesdeBase)
        {
            switch (Convert.ToInt32(pDatoDesdeBase))
            {
                case 0:
                    return Entidades.TipoEstado.SolicitaActuacion.No;
                case 1:
                    return Entidades.TipoEstado.SolicitaActuacion.Si;
                case 2:
                    return Entidades.TipoEstado.SolicitaActuacion.SiPredeterminado;
                case 3:
                    return Entidades.TipoEstado.SolicitaActuacion.Obligatoria;
                default:
                    throw new Exception("El dato recolectado del requerimiento actuación no es valido, verificar la base de datos.");
            }
        }

    }
}
