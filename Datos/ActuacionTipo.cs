using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class ActuacionTipo
    {
        /// <summary>
        /// Devuelve una lista de tipos de actuación        /// Fecha de creación : 15/06/2018        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <returns></returns>
        public List<Entidades.ActuacionTipo> TraerTipos()
        {
            // Generamos la lista a devolver
            List<Entidades.ActuacionTipo> lstTipoActuacion = new List<Entidades.ActuacionTipo>();
            // Generamos un objeto de conexión
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                // Abrimos la conexión
                c.Open();
                // Disponemos de la cadena de caracteres que realizará la consulta
                String sConsultaTiposActuacion = "SELECT id, descripcion FROM actuacion_tipo";
                // Generamos el comando y lo disponemos para su uso
                using (SQLiteCommand cmdConsulta = new SQLiteCommand(sConsultaTiposActuacion, c))
                {
                    // Generamos un lector de datos para poder recolectar la información del recorrido
                    using (SQLiteDataReader rdrTipoActuacion = cmdConsulta.ExecuteReader())
                    {
                        // Leemos los resultados obtenidos
                        while (rdrTipoActuacion.Read())
                        {
                            // Generamos una nueva entidad que almacenaremos dentro del listado resultante
                            Entidades.ActuacionTipo entActTipo = new Entidades.ActuacionTipo()
                            {
                                Id = Convert.ToInt32(rdrTipoActuacion["Id"]),
                                Descripcion = rdrTipoActuacion["Descripcion"].ToString()
                            };
                            // Agregamos la entidad al listado
                            lstTipoActuacion.Add(entActTipo);
                        }
                    }
                }
            }
            // Devolvemos la lista procesada
            return lstTipoActuacion;
        }
    }
}
