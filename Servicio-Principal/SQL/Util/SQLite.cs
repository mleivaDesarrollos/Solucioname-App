using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio_Principal.SQL
{
    internal static class SQLite
    {
        /// <summary>
        /// Formatea la fecha a un formato que no brindará problema en la interactuación con SQLITE
        /// </summary>
        /// <param name="dtPFecha">Fecha en formato DateTime</param>
        /// <returns></returns>
        internal static String FormatearFecha(DateTime dtPFecha)
        {
            return dtPFecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formatea la fecha y hora a un formato que no brindará problema en la interactuación con SQLITE
        /// </summary>
        /// <param name="dtPFecha">Fecha en formato DateTime</param>
        /// <returns></returns>
        internal static String FormatearFechaHora(DateTime dtPFecha)
        {
            return dtPFecha.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
        
        [DebuggerStepThrough]
        internal static void Agregar(this SQLiteParameterCollection cmdParam, String pStrParametro, object pValor)
        {
            // Generamos un nuevo parametro
            SQLiteParameter nuevoParametro = new SQLiteParameter();
            // Agregamos los valores a utilizar
            nuevoParametro.ParameterName = pStrParametro;
            if (pValor != null)
            {
                nuevoParametro.Value = pValor;
                nuevoParametro.DbType = pValor.TraerTipoBase();
            }
            else
            {
                nuevoParametro.Value = DBNull.Value;
                nuevoParametro.DbType = System.Data.DbType.Object;
            }
            // Agregamos el parametro al comando
            cmdParam.Add(nuevoParametro);
        }


    }
}
