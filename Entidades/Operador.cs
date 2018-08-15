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
        [DataContract]
        public class Break
        {
            [DataMember]
            public DateTime Start { get; set; }
            [DataMember]
            public DateTime End { get; set; }
        }

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

        [DataMember]
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public List<Break> Breaks { get; set; }

        public override string ToString()
        {
            return UserName;
        }
    }
}
