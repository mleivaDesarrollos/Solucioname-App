using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    /// <summary>
    /// Por decision de diseño las vistas no van a estar almacenadas en la base de datos para asi lograr una mejor performance en las consultas. Para alivianar la lectura dividimos el archivo en varios archivos.
    /// </summary>
    public partial class Asunto
    {

        // Por decisión de diseño y por optimización de consultas, evitamos el uso de vistas y disponemos de las consultas directamente
        private static String _consultaVistaEstadoAsunto
        {
            get
            {
                return @"SELECT * FROM ( 
                            SELECT  a.numero as 'num_asunto', 
                                    a.operador as 'oper', 
                                    a.descripcion_breve as 'descripcion_breve', 
                                    ae.fechaHora as 'Ultima_Fecha', 
                                    et.descripcion as 'Ultimo_Estado', 
                                    ae.ord as 'ord' 
                            FROM asuntos_estados as ae
                                join asuntos as a on a.numero = ae.numero 
                                join estado_tipo as et on ae.tipo = et.id
                            " + _condicionalVistaEstadoAsunto + @"
                            ORDER BY ae.ord ASC ) 
                        GROUP BY num_asunto";
            }
        }
        /// <summary>
        /// Consulta a la base de datos por los asuntos del día, dentro del comando ya tiene el filtro cargado de fecha.
        /// <para> </para>
        /// <paramref name="@Operador"/>: Variable de operador
        /// </summary>
        private static String _consultaAsuntosDiarios =
            @"SELECT
                ae.numero
            from asuntos_estados as ae
                where ae.fechaHora >= date('now', 'localtime', 'start of day')
                and ae.fechaHora <= date('now', 'localtime', '+1 day', 'start of day')
                and ae.operador = @Operador
            group by ae.numero
            ";

        /// <summary>
        /// Consulta si el asunto del operador es de tipo reportable
        /// <para> </para>
        /// <paramref name="@Operador"/>: Username del operador
        /// <para> </para>
        /// <paramref name="@Numero"/>: Numero del asunto procesado
        /// <para> </para>
        /// Fecha de creación : 08/06/2018
        /// Autor : Maximiliano Leiva
        /// </summary>
        private static String _consultaReportable = 
            @"SELECT 1 from asuntos where operador = @Operador and numero = @Numero and reportable=1";

        // Condicional aplicable y variable la vista
        private static String _condicionalVEAsunto;

        /// <summary>
        /// Si se desea almacenar un condicional para esta consulta se debe escribir lo que se desea filtrar luego del WHERE_ (espacio)
        /// </summary>
        private static String _condicionalVistaEstadoAsunto
        {
            get
            {
                return _condicionalVEAsunto;
            }
            set
            {
                _condicionalVEAsunto = "WHERE " + value;
            }
        }
    }
}
