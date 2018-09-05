using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Entidades
{
    [DataContract]
    public class HourCount
    {
        /// <summary>
        /// Hour corresponding to balance
        /// </summary>
        [DataMember]
        public int Hour { get; set; }

        /// <summary>
        /// Counting minutes from 0 to 14
        /// </summary>
        [DataMember]
        public int Start { get; set; }

        /// <summary>
        /// Counting minutes from 15 to 29
        /// </summary>
        [DataMember]
        public int FirstQuarter { get; set; }

        /// <summary>
        /// Counting minutes from 30 to 44
        /// </summary>
        [DataMember]
        public int HalfHour { get; set; }

        /// <summary>
        /// Counting minutes from 45 to 59
        /// </summary>
        [DataMember]
        public int LastQuarter { get; set; }

        /// <summary>
        /// Get count on total Quarter
        /// </summary>
        [DataMember]
        public int Total {
            get {
                int totalQuarter = Start + FirstQuarter + HalfHour + LastQuarter;
                return totalQuarter;
            }
        }

        public HourCount(int prmHourToRegistry)
        {
            Hour = prmHourToRegistry;
        }
    }
}
