using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Util
{
    internal static class TipoDatos
    {
        /// <summary>
        /// Devuelve el tipo de dato de base para cualquier objeto, util para determinar rapidamente el DBType
        /// </summary>
        /// <param name="objetoNativo"></param>
        /// <returns></returns>
        internal static System.Data.DbType TraerTipoBase(this object objetoNativo)
        {
            System.Data.DbType dbReturn;
            switch (objetoNativo.GetType().Name)
            {
                case "String":
                    dbReturn = System.Data.DbType.String;
                    break;
                case "Int32":
                    dbReturn = System.Data.DbType.Int32;
                    break;
                case "Double":
                    dbReturn = System.Data.DbType.Double;
                    break;
                case "DateTime":
                    dbReturn = System.Data.DbType.DateTime;
                    break;
                case "Boolean":
                    dbReturn = System.Data.DbType.Boolean;
                    break;
                default:
                    throw new Exception("Una entidad tiene un tipo de datos no reconocido: " + objetoNativo.GetType().Name);
            }
            return dbReturn;
        }
    }
}
