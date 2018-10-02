using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Entidades
{
    [DataContract]
    public class Asunto
    {
        private static readonly DateTime NULLDATE = Convert.ToDateTime("01/01/0001 0:00:00");

        private string _numero;

        [DataMember]
        public String Numero
        {
            get
            {
                return _numero;
            }
            set
            {
                // Generamos un regex para corroborar que el valor pasado cumple con los parametros de generación
                Regex rgxCheckAsunto = new Regex(@"^3000\d{6}$");
                if (!rgxCheckAsunto.IsMatch(value))
                    throw new Exception("Error : Wrong asunto number. The asunto need to start with 3000 and finishes with 6 numbers.");
                _numero = value;
            }
        }
        [DataMember]
        public Operador Oper
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

        [DataMember]
        public DateTime SendingDate { get; set; }

        [DataMember]
        public DateTime AssignmentDate { get; set; }


        [DataMember]
        public DateTime LoadedOnSolucionameDate { get; set; }

        public bool isAssigned {
            get {
                if(AssignmentDate == NULLDATE) {
                    return false;
                }
                return true;
            }
        }

        public bool isCreatedByBackoffice {
            get {
                if(SendingDate == LoadedOnSolucionameDate) {
                    return true;
                }
                return false;
            }
            set {
                if (value) {
                    DateTime dtmRegister = DateTime.Now;
                    SendingDate = dtmRegister;
                    LoadedOnSolucionameDate = dtmRegister;
                }
            }
        }

        public Asunto()
        {

        }        

        public Asunto(Asunto duplicateAsunto)
        {
            Numero = duplicateAsunto.Numero;
            Oper = duplicateAsunto.Oper;
            Actuacion = duplicateAsunto.Actuacion;
            Reportable = duplicateAsunto.Reportable;
            GrupoDerivado = duplicateAsunto.GrupoDerivado;
            DescripcionBreve = duplicateAsunto.DescripcionBreve;
            Estados = duplicateAsunto.Estados;
        }

        public bool Equals(Asunto asuntoToCompare)
        {
            if(Numero == asuntoToCompare.Numero && Oper.UserName == asuntoToCompare.Oper.UserName) {
                return true;
            }
            return false;
        }
    }
}
