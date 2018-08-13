using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Entidades
{
    [DataContract]
    public class Operador
    {
        public enum BackofficeType
        {
            Operator,
            BackofficeAndOperator,
            OnlyBackoffice
        }

        [DataMember]
        public String UserName
        {
            get; set;
        }

        [DataMember]
        public String Password
        {
            get; set;
        }

        [DataMember]
        public String Nombre
        {
            get; set;
        }

        [DataMember]
        public String Apellido
        {
            get; set;
        }

        [DataMember]
        public String DNI
        {
            get; set;
        }       
        
        [DataMember]
        public BackofficeType Backoffice
        {
            get; set;
        }

        [DataMember]
        public AvailabiltyStatus Status
        {
            get; set;
        }

        public override string ToString()
        {
            return UserName;
        }
    }
}
