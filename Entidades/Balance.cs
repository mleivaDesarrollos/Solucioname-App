using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Entidades
{
    [DataContract]
    public class Balance
    {
        static readonly int QUARTER_TOTAL_COUNT = 60;
        
        static readonly int SEVEN_POS = 0;
        
        static readonly int EIGHT_POS = 4;

        static readonly int NINE_POS = 8;

        static readonly int TEN_POS = 12;

        static readonly int ELEVEN_POS = 16;

        static readonly int TWELVE_POS = 20;

        static readonly int THIRSTEEN_POS = 24;

        static readonly int FOURTEEN_POS = 28;

        static readonly int FIFTEEN_POS = 32;

        static readonly int SIXTEEN_POS = 36;

        static readonly int SEVENTEEN_POS = 40;

        static readonly int EIGHTEEN_POS = 44;

        static readonly int NINETEEN_POS = 48;

        static readonly int TWENTY_POS = 52;

        static readonly int TWENTY_ONE_POS = 56;

        static readonly int FIRST_QUARTER_POS = 1;

        static readonly int HALF_QUARTER_POS = 2;

        static readonly int LAST_QUARTER_POS = 3;

        static readonly int ZERO_QUARTER_POS = 0;

        static readonly Reference refLastQuarter = new Reference(45, LAST_QUARTER_POS);

        static readonly Reference refHalfHour = new Reference(30, HALF_QUARTER_POS);

        static readonly Reference refFirstQuarter = new Reference(15, FIRST_QUARTER_POS);

        static readonly Reference refStartQuarter = new Reference(0, ZERO_QUARTER_POS);

        static readonly List<Reference> lstHourAndPositionReference = new List<Reference>()
        {
            new Reference(7, SEVEN_POS),
            new Reference(8, EIGHT_POS),
            new Reference(9, NINE_POS),
            new Reference(10, TEN_POS),
            new Reference(11, ELEVEN_POS),
            new Reference(12, TWELVE_POS),
            new Reference(13, THIRSTEEN_POS),
            new Reference(14, FOURTEEN_POS),
            new Reference(15, FIFTEEN_POS),
            new Reference(16, SIXTEEN_POS),
            new Reference(17, SEVENTEEN_POS),
            new Reference(18, EIGHTEEN_POS),
            new Reference(19, NINETEEN_POS),
            new Reference(20, TWENTY_POS),
            new Reference(21, TWENTY_ONE_POS),

        };

        private class Reference
        {
            public int RealTimeValue {
                get; set;
            }

            public int Position {
                get; set;
            }

            public Reference(int prmRealHourTime, int prmHourArrayPosition)
            {
                RealTimeValue = prmRealHourTime;
                Position = prmHourArrayPosition;
            }
        }

        /// <summary>
        /// User corresponding to the balance
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// by 15 minutes save quantity of asuntos assigned. From 07:00, 07:15...21:30, 21:45
        /// </summary>
        [DataMember]
        public int[] QuarterCount = new int[QUARTER_TOTAL_COUNT];
        
        /// <summary>
        /// Total asuntos counted
        /// </summary>
        [DataMember]
        public int Total {
            get {
                return SevenTotal + EightTotal + NineTotal + TenTotal + ElevenTotal + TwelveTotal + ThirsteenTotal + 
                    FourteenTotal + FifteenTotal + SixteenTotal + SeventeenTotal + EighteenTotal + NineteenTotal +
                    TwentyTotal + TwentyOneTotal;
            }
        }



        public int SevenStart {
            get {
                return QuarterCount[SEVEN_POS];
            }
        }
        public int SevenFirstQuarter {
            get {
                return QuarterCount[SEVEN_POS + FIRST_QUARTER_POS];
            }
        }
        public int SevenHalf {
            get {
                return QuarterCount[SEVEN_POS + HALF_QUARTER_POS];
            }
        }
        public int SevenLastQuarter {
            get {
                return QuarterCount[SEVEN_POS + LAST_QUARTER_POS];
            }
        }

        public int EightStart {
            get {
                return QuarterCount[EIGHT_POS];
            }
        }
        public int EightFirstQuarter {
            get {
                return QuarterCount[EIGHT_POS + FIRST_QUARTER_POS];
            }
        }
        public int EightHalf {
            get {
                return QuarterCount[EIGHT_POS + HALF_QUARTER_POS];
            }
        }
        public int EightLastQuarter {
            get {
                return QuarterCount[EIGHT_POS + LAST_QUARTER_POS];
            }
        }

        public int NineStart {
            get {
                return QuarterCount[NINE_POS];
            }
        }
        public int NineFirstQuarter {
            get {
                return QuarterCount[NINE_POS + FIRST_QUARTER_POS];
            }
        }
        public int NineHalf {
            get {
                return QuarterCount[NINE_POS + HALF_QUARTER_POS];
            }
        }
        public int NineLastQuarter {
            get {
                return QuarterCount[NINE_POS + LAST_QUARTER_POS];
            }
        }

        public int TenStart {
            get {
                return QuarterCount[TEN_POS];
            }
        }
        public int TenFirstQuarter {
            get {
                return QuarterCount[TEN_POS + FIRST_QUARTER_POS];
            }
        }
        public int TenHalf {
            get {
                return QuarterCount[TEN_POS + HALF_QUARTER_POS];
            }
        }
        public int TenLastQuarter {
            get {
                return QuarterCount[TEN_POS + LAST_QUARTER_POS];
            }
        }

        public int ElevenStart {
            get {
                return QuarterCount[ELEVEN_POS];
            }
        }
        public int ElevenFirstQuarter {
            get {
                return QuarterCount[ELEVEN_POS + FIRST_QUARTER_POS];
            }
        }
        public int ElevenHalf {
            get {
                return QuarterCount[ELEVEN_POS + HALF_QUARTER_POS];
            }
        }
        public int ElevenLastQuarter {
            get {
                return QuarterCount[ELEVEN_POS + LAST_QUARTER_POS];
            }
        }

        public int TwelveStart {
            get {
                return QuarterCount[TWELVE_POS];
            }
        }
        public int TwelveFirstQuarter {
            get {
                return QuarterCount[TWELVE_POS + FIRST_QUARTER_POS];
            }
        }
        public int TwelveHalf {
            get {
                return QuarterCount[TWELVE_POS + HALF_QUARTER_POS];
            }
        }
        public int TwelveLastQuarter {
            get {
                return QuarterCount[TWELVE_POS + LAST_QUARTER_POS];
            }
        }

        public int ThirsteenStart {
            get {
                return QuarterCount[THIRSTEEN_POS];
            }
        }
        public int ThirsteenFirstQuarter {
            get {
                return QuarterCount[THIRSTEEN_POS + FIRST_QUARTER_POS];
            }
        }
        public int ThirsteenHalf {
            get {
                return QuarterCount[THIRSTEEN_POS + HALF_QUARTER_POS];
            }
        }
        public int ThirsteenLastQuarter {
            get {
                return QuarterCount[THIRSTEEN_POS + LAST_QUARTER_POS];
            }
        }

        public int FourteenStart {
            get {
                return QuarterCount[FOURTEEN_POS];
            }
        }
        public int FourteenFirstQuarter {
            get {
                return QuarterCount[FOURTEEN_POS + FIRST_QUARTER_POS];
            }
        }
        public int FourteenHalf {
            get {
                return QuarterCount[FOURTEEN_POS + HALF_QUARTER_POS];
            }
        }
        public int FourteenLastQuarter {
            get {
                return QuarterCount[FOURTEEN_POS + LAST_QUARTER_POS];
            }
        }

        public int FifteenStart {
            get {
                return QuarterCount[FIFTEEN_POS];
            }
        }
        public int FifteenFirstQuarter {
            get {
                return QuarterCount[FIFTEEN_POS + FIRST_QUARTER_POS];
            }
        }
        public int FifteenHalf {
            get {
                return QuarterCount[FIFTEEN_POS + HALF_QUARTER_POS];
            }
        }
        public int FifteenLastQuarter {
            get {
                return QuarterCount[FIFTEEN_POS + LAST_QUARTER_POS];
            }
        }

        public int SixteenStart {
            get {
                return QuarterCount[SIXTEEN_POS];
            }
        }
        public int SixteenFirstQuarter {
            get {
                return QuarterCount[SIXTEEN_POS + FIRST_QUARTER_POS];
            }
        }
        public int SixteenHalf {
            get {
                return QuarterCount[SIXTEEN_POS + HALF_QUARTER_POS];
            }
        }
        public int SixteenLastQuarter {
            get {
                return QuarterCount[SIXTEEN_POS + LAST_QUARTER_POS];
            }
        }

        public int SeventeenStart {
            get {
                return QuarterCount[SEVENTEEN_POS];
            }
        }
        public int SeventeenFirstQuarter {
            get {
                return QuarterCount[SEVENTEEN_POS + FIRST_QUARTER_POS];
            }
        }
        public int SeventeenHalf {
            get {
                return QuarterCount[SEVENTEEN_POS + HALF_QUARTER_POS];
            }
        }
        public int SeventeenLastQuarter {
            get {
                return QuarterCount[SEVENTEEN_POS + LAST_QUARTER_POS];
            }
        }

        public int EighteenStart {
            get {
                return QuarterCount[EIGHTEEN_POS];
            }
        }
        public int EighteenFirstQuarter {
            get {
                return QuarterCount[EIGHTEEN_POS + FIRST_QUARTER_POS];
            }
        }
        public int EighteenHalf {
            get {
                return QuarterCount[EIGHTEEN_POS + HALF_QUARTER_POS];
            }
        }
        public int EighteenLastQuarter {
            get {
                return QuarterCount[EIGHTEEN_POS + LAST_QUARTER_POS];
            }
        }

        public int NineteenStart {
            get {
                return QuarterCount[NINETEEN_POS];
            }
        }
        public int NineteenFirstQuarter {
            get {
                return QuarterCount[NINETEEN_POS + FIRST_QUARTER_POS];
            }
        }
        public int NineteenHalf {
            get {
                return QuarterCount[NINETEEN_POS + HALF_QUARTER_POS];
            }
        }
        public int NineteenLastQuarter {
            get {
                return QuarterCount[NINETEEN_POS + LAST_QUARTER_POS];
            }
        }

        public int TwentyStart {
            get {
                return QuarterCount[TWENTY_POS];
            }
        }
        public int TwentyFirstQuarter {
            get {
                return QuarterCount[TWENTY_POS + FIRST_QUARTER_POS];
            }
        }
        public int TwentyHalf {
            get {
                return QuarterCount[TWENTY_POS + HALF_QUARTER_POS];
            }
        }
        public int TwentyLastQuarter {
            get {
                return QuarterCount[TWENTY_POS + LAST_QUARTER_POS];
            }
        }

        public int TwentyOneStart {
            get {
                return QuarterCount[TWENTY_ONE_POS];
            }
        }
        public int TwentyOneFirstQuarter {
            get {
                return QuarterCount[TWENTY_ONE_POS + FIRST_QUARTER_POS];
            }
        }
        public int TwentyOneHalf {
            get {
                return QuarterCount[TWENTY_ONE_POS + HALF_QUARTER_POS];
            }
        }
        public int TwentyOneLastQuarter {
            get {
                return QuarterCount[TWENTY_ONE_POS + LAST_QUARTER_POS];
            }
        }
        public int SevenTotal {
            get {
                return SevenStart + SevenFirstQuarter + SevenHalf + SevenLastQuarter;
            }
        }
        public int EightTotal {
            get {
                return EightStart + EightFirstQuarter + EightHalf + EightLastQuarter;
            }
        }
        public int NineTotal {
            get {
                return NineStart + NineFirstQuarter + NineHalf + NineLastQuarter;
            }
        }
        public int TenTotal {
            get {
                return TenStart + TenFirstQuarter + TenHalf + TenLastQuarter;
            }
        }
        public int ElevenTotal {
            get {
                return ElevenStart + ElevenFirstQuarter + ElevenHalf + ElevenLastQuarter;
            }
        }
        public int TwelveTotal {
            get {
                return TwelveStart + TwelveFirstQuarter + TwelveHalf + TwelveLastQuarter;
            }
        }
        public int ThirsteenTotal {
            get {
                return ThirsteenStart + ThirsteenFirstQuarter + ThirsteenHalf + ThirsteenLastQuarter;
            }
        }
        public int FourteenTotal {
            get {
                return FourteenStart + FourteenFirstQuarter + FourteenHalf + FourteenLastQuarter;
            }
        }
        public int FifteenTotal {
            get {
                return FifteenStart + FifteenFirstQuarter + FifteenHalf + FifteenLastQuarter;
            }
        }
        public int SixteenTotal {
            get {
                return SixteenStart + SixteenFirstQuarter + SixteenHalf + SixteenLastQuarter;
            }
        }
        public int SeventeenTotal {
            get {
                return SeventeenStart + SeventeenFirstQuarter + SeventeenHalf + SeventeenLastQuarter;
            }
        }
        public int EighteenTotal {
            get {
                return EighteenStart + EighteenFirstQuarter + EighteenHalf + EighteenLastQuarter;
            }
        }
        public int NineteenTotal {
            get {
                return NineteenStart + NineteenFirstQuarter + NineteenHalf + NineteenLastQuarter;
            }
        }
        public int TwentyTotal {
            get {
                return TwentyStart + TwentyFirstQuarter + TwentyHalf + TwentyLastQuarter;
            }
        }
        public int TwentyOneTotal {
            get {
                return TwentyOneStart + TwentyOneFirstQuarter + TwentyOneHalf + TwentyOneLastQuarter;
            }
        }

        /// <summary>
        /// Increment by 1 balance count filtering by hour and minute
        /// </summary>
        /// <param name="prmDtmReference"></param>
        public void Increment(DateTime prmDtmReference)
        {
            // Validates if input parameter is correct
            if (prmDtmReference == null) throw new Exception("BalanceEntity: datetime passed by parameter is null");
            // Subdivide hour and minutes
            int intMinutes = prmDtmReference.Minute;
            int intHour = prmDtmReference.Hour;
            // Get position of incremental values
            int posMinutes = GetMinutesCurrentPos(intMinutes);
            int posHour = GetPosOfHourArray(intHour);
            // Increment hour value on selected position of array
            QuarterCount[posHour + posMinutes]++;
        }

        /// <summary>
        /// Return position of array when try to get a specific hour
        /// </summary>
        /// <param name="prmHourToGet"></param>
        /// <returns></returns>
        private int GetPosOfHourArray(int prmHourToGet)
        {
            foreach (var hourRef in lstHourAndPositionReference) {
                if (hourRef.RealTimeValue == prmHourToGet) return hourRef.Position;
            }
            throw new Exception(string.Format("{0} hour is out of service range.", prmHourToGet));
        }

        /// <summary>
        /// Get Current Position of array by minutes
        /// </summary>
        /// <param name="prmMinuteToGet"></param>
        /// <returns></returns>
        private int GetMinutesCurrentPos(int prmMinuteToGet)
        {
            if(prmMinuteToGet >= refLastQuarter.RealTimeValue) {
                return refLastQuarter.Position;
            }
            else if(prmMinuteToGet >= refHalfHour.RealTimeValue) {
                return refHalfHour.Position;
            }
            else if (prmMinuteToGet >= refFirstQuarter.RealTimeValue) {
                return refFirstQuarter.Position;
            }
            else {
                return refStartQuarter.Position;
            }
        }
        
    }
}
