using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public static class TipoEstado
    {
        // Disponemos de un elemento de tipo estado que se utilizará en toda la aplicación
        static Datos.TipoEstado datTipoEstado = new Datos.TipoEstado();

        // Por decisión de diseño, la listado de tipos de estados se cargara una vez a petición. Es una variable de tipo estatica que por naturaleza del aplicativo no se modifica practicamente
        private static List<Entidades.TipoEstado> listadoTiposEstados;

        public static List<Entidades.TipoEstado> _listadoTiposEstados
        {
            get
            {
                if (listadoTiposEstados == null)
                {
                    // Generamos un nuevo listado de tipos de estado
                    listadoTiposEstados = TraerTipoEstado();
                                        
                }
                return listadoTiposEstados;
            }
        }
        

        /// <summary>
        /// Aplica la logica de negocio y trae los listado de tipos
        /// Fecha de creación : 30/05/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <returns></returns>
        private static List<Entidades.TipoEstado> TraerTipoEstado()
        {
            try
            {
                return datTipoEstado.TraerTipoEstado();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Devuelve un tipo de estado generico de inicio de asuntos
        /// Fecha de creación : 30/05/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <returns>Tipo Estado inicio de asunto</returns>
        public static Entidades.TipoEstado TraerEstadoAsuntoInicialNormal()
        {
            return (from ltp in _listadoTiposEstados
                   where ltp.InicioHabilitado && ltp.Descripcion == "Nuevo"
                   select ltp).First();
        }

        /// <summary>
        /// Devuelve el primer estado actución por defecto cuando se genera una nueva actuacion
        /// </summary>
        /// <returns></returns>
        public static Entidades.TipoEstado TraerEstadoActuacionInicialNormal()
        {
            return (from ltp in _listadoTiposEstados
                    where ltp.InicioHabilitado && ltp.Descripcion == "Nueva actuacion"
                    select ltp).First();
        }

        public static List<Entidades.TipoEstado> TraerEstadosPermitidos(List<Entidades.Estado> pLstEstado, int pIntOrdenSeleccionada, bool actuacion = false)
        {
            // Generamos la entidad listado
            List<Entidades.TipoEstado> lstTipoEstadoPermitido = new List<Entidades.TipoEstado>();

            // Comprobamos si el listado viene cargado con información o si por el contrario esta nulo
            if (pLstEstado != null)
            {
                if (pIntOrdenSeleccionada == 1)
                {
                    lstTipoEstadoPermitido.AddRange(_listadoTiposEstados.FindAll((tpe) => tpe.InicioHabilitado));
                }
                else
                {
                    // Obtenemos el estado previo si es que existe y Recolectamos el TipoEstado del listado proveniente de la base para corroborar de que este correctamente cargado
                    Entidades.TipoEstado tpePrevio = pLstEstado.Find((tipoEstado) => tipoEstado.Ord == pIntOrdenSeleccionada - 1).Tipo;
                    tpePrevio = _listadoTiposEstados.Find((previo) => previo.Id == tpePrevio.Id);
                    // Para evitar errores de nulos en caso de que no haya un estado posterior, filtramos el resultado
                    if (pLstEstado.Select((ea) => ea.Ord).ToArray().Max() <= pIntOrdenSeleccionada)
                    {
                        lstTipoEstadoPermitido = tpePrevio.RelacionesPermitidas;
                    }
                    else
                    {
                        // Obtenemos el estado posterior si es que existe y Recolectamos el TipoEstado del listado proveniente de la base para corroborar de que este correctamente cargado
                        Entidades.TipoEstado tpePosterior = pLstEstado.Find((tipoEstado) => tipoEstado.Ord == pIntOrdenSeleccionada + 1).Tipo;
                        tpePosterior = _listadoTiposEstados.Find((posterior) => tpePosterior.Id == posterior.Id);
                        // Dejamos una variable disponible para ir almacenando los permitidos posteriores
                        List<Entidades.TipoEstado> lstTiposPosterioresPermitidos = new List<Entidades.TipoEstado>();
                        // Recorremos los resultados almacenados para encontrar el tipo estado que sea aceptado por estado posterior
                        foreach (var tiposEstados in _listadoTiposEstados)
                        {
                            if (tiposEstados.RelacionesPermitidas.Exists((tpe) => tpe.Id == tpePosterior.Id))
                            {
                                lstTiposPosterioresPermitidos.Add(tiposEstados);
                            }
                        }
                        // Teniendo en cuenta el estado previo completamos el listado de tipos
                        lstTipoEstadoPermitido.AddRange(tpePrevio.RelacionesPermitidas);
                        // Tenemos que remover del listado los estados que no sean permitidos por el estado siguiente
                        foreach (Entidades.TipoEstado estadoRelacionado in tpePrevio.RelacionesPermitidas)
                        {
                            if (!lstTiposPosterioresPermitidos.Exists((tipoPosterior) => tipoPosterior.Id == estadoRelacionado.Id))
                            {
                                lstTipoEstadoPermitido.Remove(estadoRelacionado);
                            }

                        }
                    }
                }
            }
            if (actuacion)
            {
                // Recorremos el listado de estados permitidos para determinar cuales son actuacion
                lstTipoEstadoPermitido = lstTipoEstadoPermitido.FindAll((tpe) => tpe.Gestion.Id == 2);
            }
            else
            {
                lstTipoEstadoPermitido = lstTipoEstadoPermitido.FindAll((tpe) => tpe.Gestion.Id == 1);
            }
            // Luego de procesar el listado lo devolvemos
            return lstTipoEstadoPermitido;
        }

        /// <summary>
        /// Devuelve una descripción según el id pasado por parametro
        /// Fecha de creación : 08/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <returns></returns>
        public static String TraerDescripcion(int pIdTipo)
        {
            return _listadoTiposEstados.Find((tipoEstado) => tipoEstado.Id == pIdTipo).Descripcion;
        }
        /// <summary>
        /// Devuelve si el estado pasado por parametros es de tipo corte de ciclo
        /// Fecha de creación : 08/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <param name="entTipoEstado"></param>
        /// <returns></returns>
        public static bool EsCorteCiclo(Entidades.TipoEstado entTipoEstado)
        {
            return _listadoTiposEstados.Find((tipoEstado) => tipoEstado.Id == entTipoEstado.Id).CorteCiclo;
        }

        /// <summary>
        /// Devuelve un tipo de estado con todas sus propiedades cargadas correctamente en base a un id del tipo de estado
        /// </summary>
        /// <para><param name="pTipoEstado"> : Tipo estado con propiedades faltantes de cargar </param></para>
        /// <returns>Se carga completamente la entidad y se la devuelve con toda su información cargada</returns>
        public static Entidades.TipoEstado TraerCompleto(Entidades.TipoEstado pTipoEstado)
        {
            return _listadoTiposEstados.Find((tipEst) => tipEst.Id == pTipoEstado.Id);
        }
        
    }
}
