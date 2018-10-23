using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Entidades
{
    [DataContract]
    public class WorkTime
    {
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public DateTime EndTime { get; set; }

        public override string ToString()
        {
            return "StartTime : " + StartTime.ToString() + ". EndTime : " + EndTime.ToString();
        }
    }
}
