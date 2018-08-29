using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Entidades
{
    [DataContract]
    public class BalanceHour
    {
        private string username;

        [DataMember]
        public string UserName {
            get { return username; }
            set { username = value; }
        }

        private int sevenHour;

        [DataMember]
        public int SevenHour {
            get { return sevenHour; }
            set { sevenHour = value; }
        }

        private int eightHour;

        [DataMember]
        public int EightHour {
            get { return eightHour; }
            set { eightHour = value; }
        }

        private int nineHour;

        [DataMember]
        public int NineHour {
            get { return nineHour; }
            set { nineHour = value; }
        }

        private int tenHour;

        [DataMember]
        public int TenHour {
            get { return tenHour; }
            set { tenHour = value; }
        }

        private int elevenHour;

        [DataMember]
        public int ElevenHour {
            get { return elevenHour; }
            set { elevenHour = value; }
        }

        private int twelveHour;

        [DataMember]
        public int TwelveHour {
            get { return twelveHour; }
            set { twelveHour = value; }
        }

        private int thirsteenHour;

        [DataMember]
        public int ThirsteenHour {
            get { return thirsteenHour; }
            set { thirsteenHour = value; }
        }

        private int fourteenHour;

        [DataMember]
        public int FourteenHour {
            get { return fourteenHour; }
            set { fourteenHour = value; }
        }

        private int fifteenHour;
        
        [DataMember]
        public int FifteenHour {
            get { return fifteenHour; }
            set { fifteenHour = value; }
        }

        private int sixteenHour;
        
        [DataMember]
        public int SixteenHour {
            get { return sixteenHour; }
            set { sixteenHour = value; }
        }

        private int seventeenHour;
        
        [DataMember]
        public int SeventeenHour {
            get { return seventeenHour; }
            set { seventeenHour = value; }
        }

        private int eighteenHour;
        
        [DataMember]
        public int EighteenHour {
            get { return eighteenHour; }
            set { eighteenHour = value; }
        }

        private int nineteenHour;

        [DataMember]
        public int NineteenHour {
            get { return nineteenHour; }
            set { nineteenHour = value; }
        }

        private int twentyHour;
        
        [DataMember]
        public int TwentyHour {
            get { return twentyHour; }
            set { twentyHour = value; }
        }

        private int twentyOneHour;

        [DataMember]
        public int TwentyOneHour {
            get { return twentyOneHour; }
            set { twentyOneHour = value; }
        }

        private int total;

        [DataMember]
        public int Total {
            get { return total; }
            set { total = value; }
        }


    }
}
