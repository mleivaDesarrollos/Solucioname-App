using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.Util;
using System.Data.SQLite;
using Datos.ServiceOperation;
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

        /// <summary>
        /// Procesa una solicitud al servicio para poder obtener las credenciales completas
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<Entidades.Operador> LogOnServiceBackoffice(Entidades.Operador operatorToLog, Entidades.Service.Interface.IServicioCallback paramCallback)
        {
            // Generamos un operador para procesar
            Entidades.Operador operatorProcess = await Client.Instance.ConnectBackoffice(operatorToLog, paramCallback);
            // Devolvemos el valor procesado
            return operatorProcess;
        }

        /// <summary>
        /// Sent a request to service for connection to the service
        /// </summary>
        /// <param name="paramOperator"></param>
        /// <param name="paramCallback"></param>
        /// <returns>True if the connections succeds. False when connection is rejected</returns>
        public async Task<bool> ConnectOperatorToService(Entidades.Operador paramOperator, Entidades.Service.Interface.IServicioCallback paramCallback)
        {
            try
            {
                // Returns the getted value from service
                return await Client.Instance.ConnectOperator(paramOperator, paramCallback);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sent petition for desconnection from service
        /// </summary>
        /// <param name="pOperator">Operator to disconnect</param>
        public async Task DisconnectFromService(Entidades.Operador pOperator)
        {
            try {
                // Calls to disconnect from the service
                await Client.Instance.DisconnectOperator(pOperator);
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Sent a request to service for changing current status on the service
        /// </summary>
        /// <param name="pOperator"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public async Task ChangeCurrentStatus(Entidades.Operador prmOper, Entidades.AvailabiltyStatus newStatus)
        {
            try {
                // Sent to proxy interface to change status
                await Client.Instance.ChangeCurrentStatus(prmOper, newStatus);
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Call to service methods to obtain full data of connected operators
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entidades.Operador>> GetFullOperatorList()
        {
            try
            {
                // Returns processed list
                return await Client.Instance.GetFullOperatorList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el listado de operadores completos: " + ex.Message);
            }
        }

        /// <summary>
        /// Sent to service layer a petition for retrieve all operator list of today
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entidades.Operador>> GetOperatorWorkingToday()
        {
            try {
                return await Client.Instance.GetOperatorWorkingToday();
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
