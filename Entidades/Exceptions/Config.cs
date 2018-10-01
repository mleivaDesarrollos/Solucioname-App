using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Exceptions
{
    public class ConfigException : Exception
    {
        public ConfigException(string pPropertyName) : base("Error obteniendo las propiedades de configuración. Propiedad: " + pPropertyName) {  }
    }
}
