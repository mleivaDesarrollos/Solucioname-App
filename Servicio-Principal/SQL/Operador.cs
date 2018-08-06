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


        /// <summary>
        /// Valida si el operador informado cuenta con permisos de backoffice
        /// </summary>
        /// <param name="pOperator"></param>
        /// <returns>Operador con datos completos, nulo si no es encontrado o no tiene permisos</returns>
        public Entidades.Operador ValidateBackofficeOperator(Entidades.Operador pOperator)
        {
            // El operador de backoffice es inicializado como nulo
            Entidades.Operador backofficeOperator = null;
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                c.Open();
                string strFillBackofficUserData = "SELECT nombre, apellido, dni from operadores where username=@Username and password=@Password and backoffice=1";
                using (SQLiteCommand cmdFillBackofficeUserData = new SQLiteCommand(strFillBackofficUserData, c))
                {
                    // Agregamos el parametro de usuario y contraseña
                    cmdFillBackofficeUserData.Parameters.Agregar("@Username", pOperator.UserName);
                    cmdFillBackofficeUserData.Parameters.Agregar("@Password", pOperator.Password);
                    using (SQLiteDataReader rdrBackofficeData = cmdFillBackofficeUserData.ExecuteReader())
                    {
                        // Read the possible results of query
                        if (rdrBackofficeData.Read())
                        {
                            // Load all properties related to operator
                            backofficeOperator = new Entidades.Operador()
                            {
                                UserName = pOperator.UserName,
                                Password = pOperator.Password,
                                Nombre = rdrBackofficeData["nombre"].ToString(),
                                Apellido = rdrBackofficeData["apellido"].ToString(),
                                DNI = rdrBackofficeData["dni"].ToString()
                            };
                        }
                    }
                }
            }
            // Return the operator value
            return backofficeOperator;
        }        
    }
}
