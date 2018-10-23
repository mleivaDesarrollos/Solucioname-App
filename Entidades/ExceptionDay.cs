using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Entidades
{
    [DataContract]
    public class ExceptionDay
    {
        public static readonly DateTime AUSENT_OPERATOR_TODAY = DateTime.MinValue;

        [DataMember]
        public Operador Operator { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public WorkTime WorkingDayTime { get; set; }
        [DataMember]
        public List<WorkTime> Breaks { get; set; }
        [DataMember]
        public bool IsAusent { get; set; }

        public override string ToString()
        {
            return String.Format("Operator: {0}, Date: {1}", Operator.UserName, Date.ToString());
        }
    }
}
