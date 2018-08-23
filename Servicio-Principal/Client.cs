using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using Entidades.Service.Interface;

namespace Servicio_Principal
{
    public class Client
    {
        public Operador Operator {
            get; set;
        }

        public IServicioCallback Callback {
            get; set;
        }
    }
}
