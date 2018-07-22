using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Actuacion
    {

        public String Numero
        {
            get; set;
        }

        public Operador Operador
        {
            get; set;
        }

        public ActuacionTipo Tipo
        {
            get; set;
        }

        public String RemedyRelacionado
        {
            get; set;
        }

        public List<Estado> Estados
        {
            get; set;
        }

        public GrupoResolutor Grupo
        {
            get; set;
        }
    }
}
