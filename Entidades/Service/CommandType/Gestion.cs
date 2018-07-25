using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Service.CommandType
{
    public class Gestion : Command
    {
        public Asunto Asunto
        {
            get; set;
        }

        public Gestion() : base("gestion")
        {

        }

        /// <summary>
        /// Parametros relacionados con el comando
        /// Operador : nombre de usuario del operador que recibira el asunto
        /// Numero : Correspondiente al asunto a generar sobre la UI del operador
        /// Descripcion : Contenido de descripcion breve del asunto
        /// </summary>
        /// <param name="parameters"></param>
        internal override void loadAndCheckparameters(string[] parameters)
        {
            if (parameters.Length != 3)
            {

            }
        }
    }
}
