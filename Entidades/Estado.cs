using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Estado
    {   
        public DateTime FechaHora
        {
            get; set;
        }

        public String Detalle
        {
            get; set;
        }

        public int Ord
        {
            get; set;
        }

        public TipoEstado Tipo
        {
            get; set;
        }
    }
}
