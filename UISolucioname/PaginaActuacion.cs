using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISolucioname
{
    public class PaginaActuacion
    {
        pgeActuacion paginaActuacion = null;

        bool unicaConsulta = false;

        public PaginaActuacion()
        {
            paginaActuacion = new pgeActuacion();
        }

        /// <summary>
        /// Devuelve una instancia de la pagina contenida y la hace disponible solo para el actual llamado, no es posible llamar en dos ocasiones a la misma instancia.
        /// Esto lo hacemos para encapsular la implementación de la pagina
        /// </summary>
        /// <returns></returns>
        public pgeActuacion TraerInstancia()
        {
            if (!unicaConsulta)
            {
                unicaConsulta = true;
                return paginaActuacion;
            }
            return null;
        }

        public void GenerarNuevo()
        {
            paginaActuacion.GenerarNuevo();
        }

        public bool ComprobarCamposCargados()
        {
            return paginaActuacion.ComprobarCargaActuacion();
        }

        /// <summary>
        /// Devuelve la entidad actuación cargada desde la página de actuación
        /// Fecha de creación : 28/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        /// <returns></returns>
        public Entidades.Actuacion TraerEntidadCargada()
        {
            return paginaActuacion.ActCargada;
        }

        /// <summary>
        /// Méotodo que inhabilita los campos de la página de actuación
        /// </summary>
        public void InhabilitarCampos()
        {
            paginaActuacion.InhabilitarTodosCampos();
        }

        /// <summary>
        /// Metodo disponible para que desde otras secciones de la navegaación WPF puedan resetear los campos
        /// </summary>
        public void ResetearCampos()
        {
            paginaActuacion.ResetearCamposEstadoActuacion();
            paginaActuacion.ResetearCamposActuacion();
        }

        public void CargarActuacion(Entidades.Actuacion entAct)
        {
            paginaActuacion.ActCargada = entAct;
        }
    }
}
