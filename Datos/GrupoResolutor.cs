using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class GrupoResolutor
    {
        /// <summary>
        /// Se trae los grupos resolutores desde la base de datos
        /// </summary>
        /// <returns>Un listado de grupos resolutores</returns>
        public List<Entidades.GrupoResolutor> TraerGrupos()
        {
            // Generamos la lista a devolver luego del proceso
            List<Entidades.GrupoResolutor> lstGrupoResolutores = new List<Entidades.GrupoResolutor>();
            // Generamos un objeto de conexión y lo dejamos disponible para el proceso
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                // Realizamos apertura de conexión
                c.Open();
                // Disponemos de la cadena que se utilizará para realizar la consulta
                String sConsultaGrupos = "SELECT id_grupo, descripcion, id_tipo, descripcion_tipo from VistaGruposResolutores";
                // Generamos el comando correspondiente para ejecutar la consulta sobre la base de datos
                using (SQLiteCommand cmdConsultaGrupos = new SQLiteCommand(sConsultaGrupos, c))
                {
                    // Generamos un lector de información
                    using (SQLiteDataReader rdrLectorGruposResolutores = cmdConsultaGrupos.ExecuteReader())
                    {
                        // Ejecutamos una lectura de la información recolectada
                        while (rdrLectorGruposResolutores.Read())
                        {
                            // Generamos una nueva entidad grupo para almacenar la información recolectada
                            Entidades.GrupoResolutor grupo = new Entidades.GrupoResolutor()
                            {
                                Id = Convert.ToInt32(rdrLectorGruposResolutores["id_grupo"]),
                                Descripcion = rdrLectorGruposResolutores["Descripcion"].ToString(),
                                Tipo = new Entidades.ActuacionTipo
                                {
                                    Id = Convert.ToInt32(rdrLectorGruposResolutores["id_tipo"]),
                                    Descripcion = rdrLectorGruposResolutores["descripcion_tipo"].ToString()

                                }
                            };
                            // Agregamos el elemento generado al listado nuevo
                            lstGrupoResolutores.Add(grupo);
                        }
                    }
                }
            }
            // Devolvemos la lista procesada
            return lstGrupoResolutores;
        }
    }
}
