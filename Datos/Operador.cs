using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.Util;
using System.Data.SQLite;

namespace Datos
{
    public class Operador
    {
        /// <summary>
        /// Consulta a la base de datos con los datos brindados
        /// </summary>
        /// <param name="pUser"></param>
        /// <param name="pPass"></param>
        /// <returns>Nulo si los datos ingresados no concuerdan con los registros de la base de datos</returns>
        public Entidades.Operador Ingresar(Entidades.Operador pOperador)
        {
            // Generamos la entidad con valor nulo
            Entidades.Operador entOper = null;
            try
            {
                // Generamos el objeto de conexión
                using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
                {
                    // Abrimos la conexión
                    c.Open();
                    // Generamos el string que se usará en la consulta
                    string sConsultaUsuario = "SELECT nombre, apellido, DNI FROM operadores where username=@Operador and password=@Password";
                    // Preparamos el comando a ejecutar
                    using (SQLiteCommand cmdConsultaUsuario = new SQLiteCommand(sConsultaUsuario, c))
                    {
                        // Parametrizamos el comando
                        cmdConsultaUsuario.Parameters.Agregar("@Operador", pOperador.UserName);
                        cmdConsultaUsuario.Parameters.Agregar("@Password", pOperador.Password);
                        // Ejecutamos el lector
                        using (SQLiteDataReader rdrLectorUsuario = cmdConsultaUsuario.ExecuteReader())
                        {
                            // Si hay resultado se carga el objeto operador
                            if (rdrLectorUsuario.Read())
                            {
                                // Generamos una nueva entidad operador
                                entOper = new Entidades.Operador()
                                {
                                    UserName = pOperador.UserName,
                                    Nombre = rdrLectorUsuario["nombre"].ToString(),
                                    Password = pOperador.Password,
                                    Apellido = rdrLectorUsuario["apellido"].ToString(),
                                    DNI = rdrLectorUsuario["DNI"].ToString()
                                };

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ha ocurrido un error al recolectar la información en la base de datos: " + ex.Message);
            }            
            // Devolvemos el elemento procesado
            return entOper;
        }
    }
}
