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
    }
}
