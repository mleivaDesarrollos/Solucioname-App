using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica
{
    public class ActuacionTipo
    {
        // Generamos una instancia de objeto actuación tipo
        Datos.ActuacionTipo datActTipo = new Datos.ActuacionTipo();

        public List<Entidades.ActuacionTipo> TraerTipos()
        {
            try
            {
                return datActTipo.TraerTipos();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
