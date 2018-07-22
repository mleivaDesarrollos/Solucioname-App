using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Entidades
{
    [DataContract]
    public class Asunto
    {
        [DataMember]
        public String Numero
        {
            get; set;
        }
        [DataMember]
        public Operador Operador
        {
            get; set;
        }
        [DataMember]
        public String DescripcionBreve
        {
            get; set;
        }
        [DataMember]
        public List<Estado> Estados
        {
            get; set;
        }

        [DataMember]
        public GrupoResolutor GrupoDerivado
        {
            get; set;
        }

        [DataMember]
        // TODO : Entidad - Medium Priority : Agregar soporte para listas de actuaciones y realizar las modificaciones correspondientes en el DAO, y en la capa de presentación para que admita esta funcionalidad aún no implementada
        public Actuacion Actuacion
        {
            get; set;
        }

        [DataMember]
        public bool Reportable
        {
            get; set;
        }
    }
}
