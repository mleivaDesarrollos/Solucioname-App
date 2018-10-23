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
        [DataContract(Name = "BackofficeType")]
        public enum BackofficeType
        {
            [EnumMember]
            Operator,
            [EnumMember]
            BackofficeAndOperator,
            [EnumMember]
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
        public WorkTime WorkingDayTime { get; set; }

        [DataMember]
        public List<WorkTime> Breaks { get; set; }

        public double TotalWorkTime {
            get {
                double totalTime = (WorkingDayTime.EndTime - WorkingDayTime.StartTime).TotalHours;
                if(Breaks != null) {
                    foreach (var breakTime in Breaks) {
                        totalTime -= (breakTime.EndTime - breakTime.StartTime).TotalHours;
                    }
                }
                return totalTime;
            }
        }

        public bool Equals(Operador operatorToCompare)
        {
            return UserName.ToLower() == operatorToCompare.UserName.ToLower();
        }

        public bool Equals(string operatorUserName)
        {
            return UserName.ToLower() == operatorUserName.ToLower();
        }

        public override string ToString()
        {
            return UserName;
        }
    }
}
