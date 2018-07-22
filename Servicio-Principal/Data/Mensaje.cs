using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Entidades;

namespace Servicio_Principal
{
    [DataContract]
    public class Mensaje
    {
        [DataMember]
        public string Contenido 
        {
            get; set;
        }

        [DataMember]
        public Operador Remitente
        {
            get; set;
        }

    }
}
