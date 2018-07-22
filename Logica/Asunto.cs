using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class Asunto
    {
        Datos.Asunto datAsunto = new Datos.Asunto();

        /// <summary>
        ///  Agregamos un asunto a la base de datos aplicando previamente las reglas de negocio
        /// Fecha de creación : 06/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pAsunto"></param>
        public void Agregar(Entidades.Asunto pAsunto)
        {
            try
            {
                // Hacemos persistir la información
                datAsunto.Agregar(pAsunto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Elimina un asunto de la base de datos aplicando reglas de negocio
        /// Fecha de creación : 06/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pAsunto"></param>
        public void Eliminar(Entidades.Asunto pAsunto)
        {
            try
            {
                datAsunto.Eliminar(pAsunto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Modifica un asunto en la persistencia de la base de datos
        /// Fecha de creación : 12/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pAsunto"></param>
        public void Modificar(Entidades.Asunto pAsunto)
        {
            try
            {
                datAsunto.Modificar(pAsunto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Trae un asunto desde la persistencia hacia la capa superior
        /// Fecha de creación : 06/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pNumeroAsunto"></param>
        public Entidades.Asunto TraerAsunto(Entidades.Asunto pAsunto)
        {
            try
            {
                // Recolectamos el asunto desde la base de datos
                Entidades.Asunto asuntoRecolectado = datAsunto.TraerAsunto(pAsunto);
                // El asunto viene con faltante de datos relativos a las propiedades especificas de los estados. Por tanto se carga
                asuntoRecolectado.Estados = TraerEstadosCompletos(asuntoRecolectado.Estados);
                // Procedemos de la misma forma con los estados de actuación
                if (asuntoRecolectado.Actuacion != null) asuntoRecolectado.Actuacion.Estados = TraerEstadosCompletos(asuntoRecolectado.Actuacion.Estados);
                // Devolvemos el asunto cargado
                return asuntoRecolectado;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Determina si el asunto pasado por parametro ya esta cargado en base de datos
        /// </summary>
        /// <param name="pAsunto"></param>
        /// <returns></returns>
        public bool ExisteAsunto(Entidades.Asunto pAsunto)
        {
            try
            {
                return datAsunto.ExisteAsunto(pAsunto);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Trae asuntos por periodo
        /// </summary>
        /// <param name="iMes"></param>
        /// <param name="iAno"></param>
        /// <param name="pOper"></param>
        /// <returns></returns>
        public DataTable TraerPorPeriodo(int iMes, int iAno, Entidades.Operador pOper)
        {
            try
            {
                // Regresamos el valro obtenido
                return datAsunto.TraerPorPeriodo(iMes, iAno, pOper);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Trae los asuntos del día y los procesa para que la capa de presentación pueda disponer de ellos
        /// Fecha de creación : 07/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pOper">Operador que quiere recolectar sus asuntos del día</param>
        /// <returns></returns>
        public List<Entidades.AsuntoDiario> TraerAsuntosDelDia(Entidades.Operador pOper)
        {
            try
            {
                // Generamos un listado de asuntos diarios que sera devuelto en el proceso
                List<Entidades.AsuntoDiario> lstAsuntoDiario = new List<Entidades.AsuntoDiario>();
                // Traemos el listado de asuntos del día
                List<Entidades.Asunto> lstAsuntos = datAsunto.TraerAsuntosDelDia(pOper);
                #region proceso_asuntos_tiempo
                // Recorremos el listado de asuntos
                foreach (Entidades.Asunto asunto in lstAsuntos)
                {
                    // Generamos un nuevo asunto diario
                    Entidades.AsuntoDiario asunto_diario = new Entidades.AsuntoDiario();
                    // Almacenamos el número de asunto sobre la entidad
                    asunto_diario.Numero = asunto.Numero;
                    // Obtenemos el maximo orden del listado
                    int iOrdMax = asunto.Estados.Select((asEst) => asEst.Ord).ToArray().Max();
                    // Obtenemos el tipo de estado almacenado con el orden máximo
                    Entidades.TipoEstado tipoEstadoMaxOrd = TipoEstado.TraerCompleto(asunto.Estados.Find((et) => et.Ord == iOrdMax).Tipo);
                    // Obtenemos el string correspondiente del tipo de estado
                    String strDescripcionEstado = tipoEstadoMaxOrd.Descripcion;
                    // Almacenamos el nombre del estado con ordenMaximo
                    asunto_diario.UltimoEstado = strDescripcionEstado;
                    if (asunto.Reportable && !tipoEstadoMaxOrd.CorteCiclo)
                    {
                        // Disponemos un acumulador de minutos para almacenar los datos de reportables
                        int iMinutos = 0;
                        for (int i = 0; i < asunto.Estados.Count; i++)
                        {
                            bool bCorte = false;
                            DateTime dtInicio = asunto.Estados[i].FechaHora;
                            for (int j = i + 1; j < asunto.Estados.Count && !bCorte; j++)
                            {
                                // Si la suma de i + 1 supera el conteo total de estados no se procesa
                                // Determinamos si el tipo de estado cargado en el asunto es de tipo corte
                                if (j < asunto.Estados.Count && TipoEstado.EsCorteCiclo(asunto.Estados[j].Tipo))
                                {
                                    // Detenemos el bucle posterior cambiando el estado de corte
                                    bCorte = true;
                                    // Seteamos el valor del tiempo almacenado en el estado indicado
                                    DateTime dtCorte = asunto.Estados[j].FechaHora;
                                    // Obtenemos la diferencia en tiempo
                                    TimeSpan dtDiff = dtCorte.Subtract(dtInicio);
                                    // Almacenamos la diferencia en minutos
                                    iMinutos = iMinutos + Convert.ToInt16(dtDiff.TotalMinutes);
                                    // Asignamos el valor siguiente de I al valor de J sumado uno para continuar ciclando
                                    i = j;
                                }
                            }
                            if(i+1 == asunto.Estados.Count)
                            {
                                // TimeSpan de tiempo relacionado con el actual
                                TimeSpan diffTiempoActual = DateTime.Now.Subtract(dtInicio);
                                iMinutos = iMinutos + Convert.ToInt16(diffTiempoActual.TotalMinutes);
                            }                          
                        }                        
                        // Agregamos el procesamiento de minutos a la entidad generada
                        asunto_diario.TiempoReportable = iMinutos;
                    }
                    // Agregamos la entidad generada al listado de asuntos diarios
                    lstAsuntoDiario.Add(asunto_diario);
                }
                #endregion
                // Devolvemos el listado de asuntos diarios procesados
                return lstAsuntoDiario;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Trae un listado de asuntos filtrado segun el texto que haya sido pasado por parametro
        /// Fecha de creación : 05/07/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="pOper"></param>
        /// <param name="pTextoFiltro"></param>
        /// <returns></returns>
        public DataTable TraerAsuntosFiltradoPorNumero(Entidades.Operador pOper, string pTextoFiltro)
        {
            try
            {
                return datAsunto.TraerAsuntosFiltradoPorNumero(pOper, pTextoFiltro);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Trae desde la base de datos la información del año minimo de la consulta
        /// </summary>
        /// <returns>Entero con el año procesado</returns>
        public int TraerYearMinimo()
        {
            try
            {
                return datAsunto.TraerYearMinimo();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Trae desde la base de datos la información del año minimo de la consulta
        /// </summary>
        /// <returns>Entero con el año procesado</returns>
        public int TraerYearMaximo()
        {
            try
            {
                return datAsunto.TraerYearMaximo();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Recibe un listado de asuntos parcialmente cargado, y lo procesa cargandoló en su totalidad
        /// </summary>
        /// <param name="pAsunto"></param>
        /// <returns>Lista con todos los detalles cargados</returns>
        private List<Entidades.Estado> TraerEstadosCompletos(List<Entidades.Estado> lstEstadosIncompleto)
        {
            // Generamos la lista a procesar
            List<Entidades.Estado> lstEstados = new List<Entidades.Estado>();
            // Recorremos el listado y lo agregamos
            foreach (Entidades.Estado estado in lstEstadosIncompleto)
            {
                estado.Tipo = Logica.TipoEstado.TraerCompleto(estado.Tipo);
                lstEstados.Add(estado);
            }
            // Devolvemos el listado procesado
            return lstEstados;
        }
    }
}
