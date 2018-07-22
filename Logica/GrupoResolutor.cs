using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class GrupoResolutor
    {
        // Generamos un objeto de grupo resolución disponible para toda la clase
        Datos.GrupoResolutor datGrpRes = new Datos.GrupoResolutor();

        /// <summary>
        /// Solicita a la capa de datos el listado de grupos resolutores cargados
        /// </summary>
        /// <returns>Una coleccion de listado de grupos resolutores</returns>
        public List<Entidades.GrupoResolutor> TraerGrupos()
        {
            try
            {
                return datGrpRes.TraerGrupos();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
