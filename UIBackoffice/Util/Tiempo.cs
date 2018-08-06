using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIBackoffice.Util
{
    public static class Tiempo
    {
        /// <summary>
        /// Se arma un objeto DateTime a partir de strings pasados por parametro
        /// </summary>
        /// <param name="sFecha"></param>
        /// <param name="sHora"></param>
        /// <returns></returns>
        public static DateTime ComponerDateTimeEstadoAsunto(string sFecha, string sHora)
        {
            int[] fechaDescompuesta = Array.ConvertAll(sFecha.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
            int[] horaDescompuesta = Array.ConvertAll(sHora.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries), s => int.Parse(s));
            return new DateTime(fechaDescompuesta[2], fechaDescompuesta[1], fechaDescompuesta[0], horaDescompuesta[0], horaDescompuesta[1], horaDescompuesta[2]);
        }
    }
}
