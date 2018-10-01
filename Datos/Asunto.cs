using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos.Util;
using System.Data;
using Errors;
using Datos.ServiceOperation;

namespace Datos
{
    public partial class Asunto
    {
        private static readonly int INDEX_START_STRING = 0;
        /// <summary>
        /// Agrega un asunto a la base de datos
        /// Fecha de creación : 06/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pEntAsunto">Entidad con los datos cargados que necesitan persistencia</param>
        public void Add(Entidades.Asunto pEntAsunto)
        {
            // Generamos el objeto de conexión
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                // Abrimos la conexión
                c.Open();
                using (SQLiteTransaction t = c.BeginTransaction())
                {
                    // Disponemos de la cadena que se utilizará en el ingreso a la base de datos
                    String strIngresarAsunto = "INSERT INTO asuntos (numero, operador, descripcion_breve, grupo_derivado, reportable) values (@Numero, @Operador, @Descripcion_breve, @Grupo_Derivado, @Reportable)";
                    using (SQLiteCommand cmdIngresarAsunto = new SQLiteCommand(strIngresarAsunto, c, t))
                    {
                        // Parametrizamos los valores agregados
                        cmdIngresarAsunto.Parameters.Agregar("@Numero", pEntAsunto.Numero);
                        cmdIngresarAsunto.Parameters.Agregar("@Operador", pEntAsunto.Oper.UserName);
                        cmdIngresarAsunto.Parameters.Agregar("@Descripcion_breve", pEntAsunto.DescripcionBreve);
                        cmdIngresarAsunto.Parameters.Agregar("@Grupo_Derivado", pEntAsunto.GrupoDerivado);
                        cmdIngresarAsunto.Parameters.Agregar("@Reportable", pEntAsunto.Reportable ? 1 : 0);
                        // Ejecutamos el Query
                        cmdIngresarAsunto.ExecuteNonQuery();
                    }
                    if (pEntAsunto.Estados != null)
                    {
                        // Agregamos los estados que traiga el asunto en cuestión
                        EstadoAsunto.AddAllFromAsunto(pEntAsunto, c, t);
                    }
                    
                    
                    // Agregamos la o las actuaciones
                    if (pEntAsunto.Actuacion != null)
                    {
                        Actuacion.Agregar(pEntAsunto, c, t);
                    }                    
                    t.Commit();
                }
            }
        }
        
        /// <summary>
        /// Add a batch of asuntos
        /// </summary>
        /// <param name="lstA"></param>
        public void Add(List<Entidades.Asunto> lstA)
        {
            try {
                // Validates if the list is sented with data. If the list have one value, the petition is rejected because this method is for a batch of asuntos
                if (lstA == null || lstA.Count <= 1) throw new Exception("La lista de asuntos recibida esta vacía, es nula o es menor al minimo");
                using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                    c.Open();
                    using (SQLiteTransaction t = c.BeginTransaction()) {
                        string strInsertAsuntoOnDatabase = "INSERT INTO asuntos VALUES (@Number, @Operator, @ShortDescription, @DerivedGroup, @ForReport)";
                        using (SQLiteCommand cmdInsertAsuntoOnDatabase = new SQLiteCommand(strInsertAsuntoOnDatabase, c, t)) {
                            foreach (var asuntoToSave in lstA) {
                                cmdInsertAsuntoOnDatabase.Parameters.Agregar("@Number", asuntoToSave.Numero);
                                cmdInsertAsuntoOnDatabase.Parameters.Agregar("@Operator", asuntoToSave.Oper.UserName);
                                cmdInsertAsuntoOnDatabase.Parameters.Agregar("@ShortDescription", asuntoToSave.DescripcionBreve);
                                cmdInsertAsuntoOnDatabase.Parameters.Agregar("@DerivedGroup", asuntoToSave.GrupoDerivado.Id);
                                cmdInsertAsuntoOnDatabase.Parameters.Agregar("ForReport", asuntoToSave.Reportable);

                                if(asuntoToSave.Estados != null) {
                                    EstadoAsunto.AddAllFromAsunto(asuntoToSave, c, t);
                                }

                                if(asuntoToSave.Actuacion != null) {
                                    Actuacion.Agregar(asuntoToSave, c, t);
                                }

                                cmdInsertAsuntoOnDatabase.ExecuteNonQuery();
                            }
                        }

                        // When process is completed correctly, commit changes on database
                        t.Commit();
                    }
                }

            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Elimina un asunto de la base de datos
        /// Fecha de creación : 06/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pEntAsunto"></param>
        public void Remove(Entidades.Asunto pEntAsunto)
        {
            try
            {
                // Generamos un nuevo objeto de conexión
                using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
                {
                    // Realizamos la apertura de conexión
                    c.Open();
                    // Disponemos la transacción para ser utilizada en el transcurso de la operación
                    using (SQLiteTransaction t = c.BeginTransaction())
                    {
                        // Eliminamos los estados guardados en base de datos
                        EstadoAsunto.EliminarEstadosPorAsunto(pEntAsunto, c, t);
                        // Preparamos la eliminación del asunto
                        String strEliminarAsunto = "DELETE FROM ASUNTOS where numero=@Numero and operador=@Operador";
                        // Generamos el comando de eliminación
                        using (SQLiteCommand cmdEliminarAsunto = new SQLiteCommand(strEliminarAsunto, c, t))
                        {
                            // Agregamos los parametros al comando
                            cmdEliminarAsunto.Parameters.Agregar("@Numero", pEntAsunto.Numero);
                            cmdEliminarAsunto.Parameters.Agregar("@Operador", pEntAsunto.Oper.UserName);
                            // Ejecutamos la eliminación por comando
                            cmdEliminarAsunto.ExecuteNonQuery();
                        }
                        // Impactamos el Commit
                        t.Commit();
                    }                    
                }
            }
            catch (Exception)
            {
                throw new Exception("Ha ocurrido un error al eliminar el asunto");
            }
        }

        /// <summary>
        /// Actualiza un asunto en base de datos
        /// Fecha de creación : 06/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pEntAsunto"></param>
        public void Modify(Entidades.Asunto pEntAsunto)
        {
            try
            {
                // Generamos el objeto de conexión
                using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
                {
                    // Abrimos la conexión
                    c.Open();
                    // Iniciamos el sistema transaccional
                    using (SQLiteTransaction t = c.BeginTransaction())
                    {
                        // Generamos la cadena de caracteres que se utilizará para la actualización
                        String strActualizarAsunto = "UPDATE asuntos SET descripcion_breve=@DescripcionBreve, grupo_derivado=@GrupoDerivado WHERE operador=@Operador and numero=@Numero";
                        // Generamos el comando de actualización
                        using (SQLiteCommand cmdActualizarAsunto = new SQLiteCommand(strActualizarAsunto, c, t))
                        {
                            // Parametrizamo la consulta
                            cmdActualizarAsunto.Parameters.Agregar("@DescripcionBreve", pEntAsunto.DescripcionBreve);
                            cmdActualizarAsunto.Parameters.Agregar("@GrupoDerivado", pEntAsunto.GrupoDerivado);
                            cmdActualizarAsunto.Parameters.Agregar("@Operador", pEntAsunto.Oper.UserName);
                            cmdActualizarAsunto.Parameters.Agregar("@Reportable", pEntAsunto.Reportable ? 1: 0);
                            cmdActualizarAsunto.Parameters.Agregar("@Numero", pEntAsunto.Numero);
                            // Ejecutamos la actualización
                            cmdActualizarAsunto.ExecuteNonQuery();
                        }
                        // Ejecutamos de la interfaz expuesta por EstadoAsunto para actulizar los estados
                        EstadoAsunto.ActualizarEstadosPorAsunto(pEntAsunto, c, t);
                        // Ejecutamos la interfaz expuesta por actuación para actualizarlas o eliminarlas
                        if (pEntAsunto.Actuacion != null)
                        {
                            Actuacion.Modificar(pEntAsunto, c, t);
                        }
                        else // En el caso de que actuación venga como nulo, se eliminan todas las actuaciones relacionadas con el asunto (para evitar que queden actuaciones huerfanas)
                        {
                            Actuacion.Eliminar(pEntAsunto, c, t);
                        }                        
                        // Ejecutamos commit de la transacción
                        t.Commit();
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Ha ocurrido un error al ejecutar la actualización del asunto");
            }
            
        }

        /// <summary>
        /// Consulta a la base de datos si hay un asunto con los parametros que solicita
        /// Fecha de creación : 06/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pEntAsunto"></param>
        public Entidades.Asunto Get(Entidades.Asunto pEntAsunto)
        {
            // Generamos una entidad nueva que sera procesada
            Entidades.Asunto entAsunto = new Entidades.Asunto();
            try
            {
                // Generamos un nuevo objeto de conexión
                using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
                {
                    // Abrimos la conexión
                    c.Open();
                    String strConsultaAsunto = "SELECT descripcion_breve, grupo_derivado, reportable FROM asuntos where operador = @IdOperador and numero=@Numero";
                    using (SQLiteCommand cmdConsultaAsunto = new SQLiteCommand(strConsultaAsunto, c))
                    {
                        cmdConsultaAsunto.Parameters.Agregar("@IdOperador", pEntAsunto.Oper.UserName);
                        cmdConsultaAsunto.Parameters.Agregar("@Numero", pEntAsunto.Numero);
                        using (SQLiteDataReader rdrConsultaAsunto = cmdConsultaAsunto.ExecuteReader())
                        {
                            // Llemos los resultados obtenidos
                            if (rdrConsultaAsunto.Read())
                            {
                                if (!rdrConsultaAsunto.IsDBNull(1))
                                {
                                    entAsunto.GrupoDerivado.Id = rdrConsultaAsunto.GetInt32(1);
                                }
                                entAsunto.Numero = pEntAsunto.Numero;
                                entAsunto.Oper = pEntAsunto.Oper;                            
                                entAsunto.DescripcionBreve = rdrConsultaAsunto["descripcion_breve"].ToString();
                                entAsunto.Reportable = Convert.ToBoolean(rdrConsultaAsunto["reportable"]);
                                entAsunto.Estados = EstadoAsunto.TraerListaEstadosPorAsunto(pEntAsunto);
                                entAsunto.Actuacion = Actuacion.TraerActuaciones(entAsunto, c);
                            }                            
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Error en la recuperacion de información del asunto");
            }
            // Devolvemos la entidad procesada
            return entAsunto;
        }

        /// <summary>
        /// Método que determina si ya hay un asunto cargado en base que corresponda con el número de asunto y el operador
        /// </summary>
        /// <param name="pAsunto"></param>
        /// <returns></returns>
        public bool Exist(Entidades.Asunto pAsunto)
        {
            try
            {
                // Generamos un nuevo objeto de cadena de conexión
                using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
                {
                    c.Open();
                    String strConsultaAsunto = "SELECT 1 FROM asuntos WHERE numero=@Numero and operador=@Operador";
                    // Generamos el comando de consulta para la base de datos
                    using (SQLiteCommand cmdConsultaAsunto = new SQLiteCommand(strConsultaAsunto, c))
                    {
                        // Parametrizamos la consulta
                        cmdConsultaAsunto.Parameters.Agregar("@Numero", pAsunto.Numero);
                        cmdConsultaAsunto.Parameters.Agregar("@Operador", pAsunto.Oper.UserName);
                        // Ejecutamos la consulta con un lector
                        using (SQLiteDataReader rdrAsunto = cmdConsultaAsunto.ExecuteReader())
                        {
                            if (rdrAsunto.Read())
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw new Exception("Ha ocurrido un error al recolectar la información dede la base de datos");
            }
        }

        /// <summary>
        /// Retrieve filtered list to caller. The filter is based on non registered asunto on local base
        /// </summary>
        /// <param name="lstA"></param>
        /// <returns></returns>
        public List<Entidades.Asunto> GetNonDuplicatedAsuntosFromList(List<Entidades.Asunto> lstA)
        {
            // Generate a list to return in process
            List<Entidades.Asunto> lstFilteredAsuntos = new List<Entidades.Asunto>();
            try {
                // Validates if the list is sented with data. If the list have one value, the petition is rejected because this method is for a batch of asuntos
                if (lstA == null || lstA.Count <= 1) throw new Exception("La lista de asuntos recibida esta vacía, es nula o es menor al minimo");
                using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena)) {
                    c.Open();
                    string strQueryNonDuplicatedAsuntos = "SELECT numero FROM asuntos WHERE operador=@Operator AND numero NOT IN (@AsuntoList)";
                    using (SQLiteCommand cmdQueryNonDuplicatedAsuntos = new SQLiteCommand(strQueryNonDuplicatedAsuntos, c)) {
                        // Get Operator from the given list
                        Entidades.Operador operatorUsedToCheck = lstA[0].Oper;
                        cmdQueryNonDuplicatedAsuntos.Parameters.Agregar("@Operator", operatorUsedToCheck.UserName);
                        cmdQueryNonDuplicatedAsuntos.Parameters.Agregar("@AsuntoList", getNumberOfAsuntosOnlyFromListForDatabaseRead(lstA));
                        using (SQLiteDataReader rdrQueryNonDuplicatedAsuntos = cmdQueryNonDuplicatedAsuntos.ExecuteReader()) {
                            while (rdrQueryNonDuplicatedAsuntos.Read()) {
                                string asuntoNumber = rdrQueryNonDuplicatedAsuntos["numero"].ToString();
                                // Add the finded asunto to list of new asuntos
                                lstFilteredAsuntos.Add(lstA.Find(asunto => asunto.Numero == asuntoNumber));
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                throw ex;
            }
            // Return the list at the end of the process
            return lstFilteredAsuntos;
        }

        /// <summary>
        /// Por solicitud trae filtrado los registros cargados por mes y año
        /// </summary>
        /// <param name="iMonth">Mes de registro</param>
        /// <param name="iYear">Año de registro</param>
        /// <param name="pOper">Operador que ejecuta la consulta</param>
        public DataTable GetByPeriod(int iMonth, int iYear, Entidades.Operador pOper)
        {
            // Generamos una lista que devolveremos al final del recorrido
            DataTable dtResult = new DataTable();
            // Abrimos la consulta con control de errores
            try
            {
                _condicionalVistaEstadoAsunto = "ae.fechaHora BETWEEN @PeriodoInicio AND @PeriodoFin and a.operador = @Oper";
                // Modo con vistas, se deshabilita por ser mas performante el filtrado directo
                // String sConsultaEstados = "SELECT * FROM VistaEstadoAsuntosRecientes WHERE ultima_fecha BETWEEN @PeriodoInicio AND @PeriodoFin and oper = @Oper";
                using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(_consultaVistaEstadoAsunto, Conexion.Cadena))
                {
                    // Parametrizamos los datos a utilizar
                    DateTime dtPeriodoInicio = new DateTime(iYear, iMonth, 1).AddDays(-1);
                    DateTime dtPeriodoFinal = new DateTime(iYear, iMonth, 1).AddMonths(1);
                    dataAdapter.SelectCommand.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@PeriodoInicio",
                        Value = Util.SQLite.FormatearFecha(dtPeriodoInicio),
                        DbType = DbType.DateTime
                    });
                    dataAdapter.SelectCommand.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@PeriodoFin",
                        Value = Util.SQLite.FormatearFecha(dtPeriodoFinal),
                        DbType = DbType.DateTime
                    });
                    dataAdapter.SelectCommand.Parameters.Add(new SQLiteParameter()
                    {
                        ParameterName = "@Oper",
                        Value = pOper.UserName,
                        DbType = DbType.String
                    });
                    dataAdapter.Fill(dtResult);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Ha ocurrido un error al intentar traer los estados: " + e.Message);
            }
            return dtResult;
        }

        /// <summary>
        /// Consulta la base de datos filtrando según número de asunto
        /// Fecha de creación : 05/07/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pOper"></param>
        /// <param name="pTextoFiltro"></param>
        /// <returns></returns>
        public DataTable GetFilteredByNumber(Entidades.Operador pOper, string pTextoFiltro)
        {
            // Generamos la DataTable a procesar
            DataTable dtResultado = new DataTable();
            try
            {
                // Establecemos el valor del condicional de la vista
                _condicionalVistaEstadoAsunto = "num_asunto LIKE '3000" + pTextoFiltro + "%' and oper=@Oper";
                // Generamos el adaptador de datos para filtrar el asunto
                using (SQLiteDataAdapter adtListadoAsuntoFiltrado = new SQLiteDataAdapter(_consultaVistaEstadoAsunto, Conexion.Cadena))
                {
                    // Parametrizamos la consulta
                    adtListadoAsuntoFiltrado.SelectCommand.Parameters.Agregar("@Oper", pOper.UserName);
                    // Llenamos el DataTable con los datos indicados
                    adtListadoAsuntoFiltrado.Fill(dtResultado);
                }
            }
            catch (Exception)
            {
                throw new Exception("Error al procesar la consulta filtrada de asuntos.");
            }
            // Devolvemos la tabla procesada
            return dtResultado;
        }

        /// <summary>
        /// Consulta a la base de datos por los asuntos que corresponden al dia de la fecha
        /// Fecha de creación : 07/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <returns></returns>
        public List<Entidades.Asunto> GetCurrentDayList(Entidades.Operador pOper)
        {
            // Generamos la lista a devolver
            List<Entidades.Asunto> lstAsuntoDiario = new List<Entidades.Asunto>();
            // Generamos el objeto de conexión despachable
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                // Abrimos la conexión
                c.Open();
                // Creamos el comando despachable
                using (SQLiteCommand cmdConsultaAsuntosDia = new SQLiteCommand(_consultaAsuntosDiarios, c))
                {
                    // Cargamos el parametro de operador
                    cmdConsultaAsuntosDia.Parameters.Agregar("@Operador", pOper.UserName);
                    // Ejecutamos el lector de asuntos
                    using (SQLiteDataReader rdrLectorAsunto = cmdConsultaAsuntosDia.ExecuteReader())
                    {
                        // Leemos los resultados obtenidos del lector
                        while (rdrLectorAsunto.Read())
                        {
                            // Generamos una nueva entidad de asunto, donde almacenaremos los diferentes estados recolectados
                            Entidades.Asunto entAsuntoDiario = new Entidades.Asunto();
                            // Almacenamos el número y el operador sobre el asunto
                            entAsuntoDiario.Oper = pOper;
                            entAsuntoDiario.Numero = rdrLectorAsunto["numero"].ToString();
                            // Traemos los estados del asunto recorrido
                            entAsuntoDiario.Estados = EstadoAsunto.TraerListaEstadosPorAsunto(entAsuntoDiario);
                            // Consultamos si el asunto es reportable
                            using (SQLiteCommand cmdAsuntoReportable = new SQLiteCommand(_consultaReportable, c))
                            {
                                cmdAsuntoReportable.Parameters.Agregar("@Numero", rdrLectorAsunto["numero"].ToString());
                                cmdAsuntoReportable.Parameters.Agregar("@Operador", pOper.UserName);
                                // Ejecutamos el lector y averiguamos si es verdadera la consulta
                                using (SQLiteDataReader rdrReportable = cmdAsuntoReportable.ExecuteReader())
                                {
                                    // Si devuelve respuesta se asigna verdadero a reportable
                                    if (rdrReportable.Read())
                                    {
                                        entAsuntoDiario.Reportable = true;
                                    }
                                }
                            }
                            lstAsuntoDiario.Add(entAsuntoDiario);
                        }
                    }  
                }
            }
            // Devolvemos la lista procesada
            return lstAsuntoDiario;
        }

        /// <summary>
        /// Trae el mínimo año cargado en la base de datos
        /// Fecha de creación : 05/07/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <returns></returns>
        public int GetMinYear()
        {
            // Generamos el valor int a devolver, por defecto tomamos el año actual
            int iYear = DateTime.Now.Year;
            // Generamos el objeto de conexión que servirá para recolectar los datos de la base
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                // Abrimos la conexión de base de datos
                c.Open();
                // Preparamos la cadena de caracteres que se utilizará para realizar la consulta sobre la base de datos
                string sConsultaFecha = "SELECT strftime('%Y', fechaHora) FROM asuntos_estados order by fechaHora asc limit 1";
                // Generamos el comando a ejecutar sobre la base de datos
                using (SQLiteCommand cmdConsultaFecha = new SQLiteCommand(sConsultaFecha, c))
                {
                    // Ejecutamos el lector de sobre el comando
                    using (SQLiteDataReader rdrConsultaFecha = cmdConsultaFecha.ExecuteReader())
                    {
                        if (rdrConsultaFecha.Read())
                        {
                            // Almacenamos el año sobre la variable generada
                            iYear = Convert.ToInt32(rdrConsultaFecha[0]);
                        }
                    }
                }
            }
            // Devolvemos la variable procesada
            return iYear;
        }

        public int GetMaxYear()
        {
            // Generamos el valor int a devolver, por defecto tomamos el año actual
            int iYear = DateTime.Now.Year;
            // Generamos el objeto de conexión que servirá para recolectar los datos de la base
            using (SQLiteConnection c = new SQLiteConnection(Conexion.Cadena))
            {
                // Abrimos la conexión de base de datos
                c.Open();
                // Preparamos la cadena de caracteres que se utilizará para realizar la consulta sobre la base de datos
                string sConsultaFecha = "SELECT strftime('%Y', fechaHora) FROM asuntos_estados order by fechaHora desc limit 1";
                // Generamos el comando a ejecutar sobre la base de datos
                using (SQLiteCommand cmdConsultaFecha = new SQLiteCommand(sConsultaFecha, c))
                {
                    // Ejecutamos el lector de sobre el comando
                    using (SQLiteDataReader rdrConsultaFecha = cmdConsultaFecha.ExecuteReader())
                    {
                        if (rdrConsultaFecha.Read())
                        {
                            // Almacenamos el año sobre la variable generada
                            iYear = Convert.ToInt32(rdrConsultaFecha[0]);
                        }
                    }
                }
            }
            // Devolvemos la variable procesada
            return iYear;
        }

        /// <summary>
        /// Sent a client of the service a petition to add in queue al asunto
        /// </summary>
        /// <param name="prmBackofficeSender"></param>
        /// <param name="prmAsuntoToSent"></param>
        public async Task SentAsuntoToOperator(Entidades.Operador prmBackofficeSender, Entidades.Asunto prmAsuntoToSent)
        {
            try {
                await Client.Instance.SentAsuntoToOperator(prmBackofficeSender, prmAsuntoToSent);
            }
            catch (Exception ex) {
                Except.Throw(ex);
            }
        }


        /// <summary>
        /// Gets from service the list of asuntos unassigned
        /// </summary>
        /// <returns></returns>
        public async Task<List<Entidades.Asunto>> GetUnassignedAsuntos()
        {
            try {
                return await Client.Instance.GetListOfUnassignedAsuntos();
            }
            catch (Exception ex) {
                throw ex;
            }
        }


        /// <summary>
        /// Process a list of asuntos and save number only on a string with format for query on database
        /// </summary>
        /// <param name="lstAsuntosToParse"></param>
        /// <returns></returns>
        private string getNumberOfAsuntosOnlyFromListForDatabaseRead(List<Entidades.Asunto> lstAsuntosToParse)
        {
            // Get all numebers of asunto and save on array
            string[] unformattedListOfasuntos = lstAsuntosToParse.Select(asunto => asunto.Numero).ToArray();
            // Unify all numbers on one string, separating by ', '. This seperator is for query formatting purposes
            string strLstOfAsuntos = String.Join("', '", unformattedListOfasuntos);
            // The join don't adds ' on start and the end, manually adds for correct formatting
            strLstOfAsuntos.Insert(INDEX_START_STRING, "'");
            strLstOfAsuntos.Insert(strLstOfAsuntos.Length - 1, "'");
            // Return processed value
            return strLstOfAsuntos;
        }
    }
}
