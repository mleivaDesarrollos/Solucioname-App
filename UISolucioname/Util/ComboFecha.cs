using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISolucioname.Util
{
    public static class ComboFecha
    {

        /// <summary>
        /// Por solicitud se cargan un listado de meses, el cual generalmente es utilizado para los combos de meses
        /// </summary>
        /// <returns></returns>
        public static List<Mes> CargarMeses()
        {
            // Generamos un listado nuevo de meses
            List<Mes> lstMes = new List<Mes>();
            lstMes.Add(new Mes() { Numero = 1, Descripcion = "Enero" });
            lstMes.Add(new Mes() { Numero = 2, Descripcion = "Febrero" });
            lstMes.Add(new Mes() { Numero = 3, Descripcion = "Marzo" });
            lstMes.Add(new Mes() { Numero = 4, Descripcion = "Abril" });
            lstMes.Add(new Mes() { Numero = 5, Descripcion = "Mayo" });
            lstMes.Add(new Mes() { Numero = 6, Descripcion = "Junio" });
            lstMes.Add(new Mes() { Numero = 7, Descripcion = "Julio" });
            lstMes.Add(new Mes() { Numero = 8, Descripcion = "Agosto" });
            lstMes.Add(new Mes() { Numero = 9, Descripcion = "Septiembre" });
            lstMes.Add(new Mes() { Numero = 10, Descripcion = "Octubre" });
            lstMes.Add(new Mes() { Numero = 11, Descripcion = "Noviembre" });
            lstMes.Add(new Mes() { Numero = 12, Descripcion = "Diciembre" });
            // Lo devolvemos luego de procesarlo
            return lstMes;
        }

        /// <summary>
        /// Utilizando los enlaces a lógica se realiza una consulta de años cargados en base de datos para que sean utilizados en los combos
        /// </summary>
        /// <returns></returns>
        public static List<int> CargarYear()
        {
            // Generamos un listado de años para procesar
            List<int> lstYears = new List<int>();

            // Generamos el objeto logica asunto para traer la información de la base de datos
            Logica.Asunto logAsunto = new Logica.Asunto();
            // Traemos el año minimo cargado en la base de datos
            int iYearMinimo = logAsunto.getMinYear();
            int iYearMaximo = logAsunto.getMaxYear();
            // Diferencia entre año maximo y minimo
            for (int i = 0; i <= iYearMaximo - iYearMinimo; i++)
            {
                // Agregamos el año al listado cargado
                lstYears.Add(iYearMinimo + i);
            }
            // Devolvemos el listado de años procesado
            return lstYears;
        }
    }
}
