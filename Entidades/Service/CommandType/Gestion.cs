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
            // Si los parametros pasados por consola son menores a 3, se rechaza la generación del comando.
            if (parameters.Length < 3) throw new Exception("usage : gestion [operator] [asunto_number] [short_description]");
            // Subdividimos los parametros 
            string strOperator = parameters[0];
            string strNumero = parameters[1];
            StringBuilder sbDescription = new StringBuilder();
            // Almacenamos los valores desde la posicion 3 hacia adelante para la descripción breve
            foreach (var strWordShortDescription in parameters.Skip(2).ToArray())
            {
                // Agregamos cada palabra a la descripción breve
                sbDescription.Append(strWordShortDescription + " ");
            }
            // Generamos un nuevo asunto y almacenamos la información guardada
            Asunto = new Asunto() { Numero = strNumero, Oper = new Operador() { UserName = strOperator }, DescripcionBreve = sbDescription.ToString() };
        }
    }
}
