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

        static readonly int ZERO_QUARTER_POS = 0;

        static readonly int FIRST_QUARTER_POS = 1;

        static readonly int HALF_QUARTER_POS = 2;

        static readonly int LAST_QUARTER_POS = 3;

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

        public string FirstName { get; set; }

        public string LastName { get; set; }
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

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime BreakOneStart { get; set; }
        public DateTime BreakOneEnd { get; set; }
        public DateTime BreakTwoStart { get; set; }
        public DateTime BreakTwoEnd { get; set; }
        public DateTime BreakThreeStart { get; set; }
        public DateTime BreakThreeEnd { get; set; }
        public double TotalWorkTime { get; set; }
        public double CurrentWorkTime { get; set; }
        public double AverageAsuntoByHour { get; set; }
        public double SimulationAverageAsuntoByHour { get; set; }
        public int SimulationTotal { get; set;}
        

        /// <summary>
        /// Increment by 1 balance count filtering by hour and minute
        /// </summary>
        /// <param name="prmDtmReference"></param>
        public void Increment(DateTime prmDtmReference)
        {
            // Validates if input parameter is correct
            if (prmDtmReference == null) throw new Exception("BalanceEntity: datetime passed by parameter is null");
            // Increment hour value on selected position of array
            QuarterCount[getTotalPosition(prmDtmReference)]++;
        }

        /// <summary>
        /// Decrement by balance count filtering by hour and minutes
        /// </summary>
        /// <param name="prmDtmReference"></param>
        public void Decrement(DateTime prmDtmReference)
        {
            // Validates if parameter input is correctly loaded. Because is a public interface, we need validate values
            if (prmDtmReference == null) throw new Exception("BalanceEntity: datetime passed by parameter is null");
            QuarterCount[getTotalPosition(prmDtmReference)]--;
        }

        /// <summary>
        /// Initalize variables related to simulation
        /// </summary>
        public void InitSimulation()
        {
            SimulationAverageAsuntoByHour = AverageAsuntoByHour;
            SimulationTotal = Total;
        }

        /// <summary>
        /// Simulate a average keeping unaffected current average
        /// </summary>
        /// <param name="dtmDestination"></param>
        public void SimulateAverageIncrementingByOne()
        {

            // In case of calculation is requested out of labor time, break task
            if (DateTime.Now > EndTime)
                throw new Exception("SimulationAverageAsunto : Cannot simulate a balance on operator off of work time");
            // Increment by one total of simulation
            SimulationTotal++;
            // Calculate average on simulation
            CalculateSimulationAverageAsuntoByHour();
        }

        private int getTotalPosition(DateTime prmDtmReferenced)
        {
            // Subdivide hour and minutes
            int intMinutes = prmDtmReferenced.Minute;
            int intHour = prmDtmReferenced.Hour;
            // Get position of incremental values
            int posMinutes = GetMinutesCurrentPos(intMinutes);
            int posHour = GetPosOfHourArray(intHour);
            return posMinutes + posHour;
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

        /// <summary>
        /// Calculates average total by hour
        /// </summary>
        public void CalculateAverageAsuntoByHour()
        {
            // Select period to calculate average
            if (DateTime.Now <= StartTime) {
                // If the hour is previous to StartTime sent average to zero
                AverageAsuntoByHour = 0;
            } else if (DateTime.Today.AddHours(DateTime.Now.Hour + 1) >= EndTime) {
                // The calculation of average is total of asuntos divided all hours
                if (TotalWorkTime != 0) AverageAsuntoByHour = Total / TotalWorkTime;
            } else {
                AverageAsuntoByHour = CalculateAverageAsuntoOnWorkingActive(false);
            }
        }

        /// <summary>
        /// Simulate Calculation for current balance
        /// </summary>
        private void CalculateSimulationAverageAsuntoByHour()
        {
            // Calculate asunto average
            SimulationAverageAsuntoByHour = CalculateAverageAsuntoOnWorkingActive(true);
        }

        private double CalculateAverageAsuntoOnWorkingActive(bool isSimulation)
        {
            // Get current Time plus one hour
            DateTime currentTime = DateTime.Today.AddHours(DateTime.Now.Hour + 1);
            // Get Base work time
            CurrentWorkTime = (currentTime - StartTime).TotalHours;
            if(BreakOneStart != null && currentTime > BreakOneStart) {
                // Set up a variable for endtime break calculation
                DateTime endTimeCalculation;
                // if the currentime is upper end break, set up end break
                if(currentTime > BreakOneEnd) {
                    endTimeCalculation = BreakOneEnd;
                // else indicates the break is in the middle of previous and next hour
                } else {
                    endTimeCalculation = currentTime;
                }
                // The difference is substracted from total worktime
                CurrentWorkTime -= (endTimeCalculation - BreakOneStart).TotalHours;
            }
            if (BreakTwoStart != null && currentTime > BreakTwoStart) {
                // Set up a variable for endtime break calculation
                DateTime endTimeCalculation;
                // if the currentime is upper end break, set up end break
                if (currentTime > BreakTwoEnd) {
                    endTimeCalculation = BreakTwoEnd;
                    // else indicates the break is in the middle of previous and next hour
                } else {
                    endTimeCalculation = currentTime;
                }
                // The difference is substracted from total worktime
                CurrentWorkTime -= (endTimeCalculation - BreakTwoStart).TotalHours;
            }
            if (BreakThreeStart != null && currentTime > BreakThreeStart) {
                // Set up a variable for endtime break calculation
                DateTime endTimeCalculation;
                // if the currentime is upper end break, set up end break
                if (currentTime > BreakThreeEnd) {
                    endTimeCalculation = BreakThreeEnd;
                    // else indicates the break is in the middle of previous and next hour
                } else {
                    endTimeCalculation = currentTime;
                }
                // The difference is substracted from total worktime
                CurrentWorkTime -= (endTimeCalculation - BreakThreeStart).TotalHours;
            }
            // Return in process
            if (isSimulation) return SimulationTotal / CurrentWorkTime;
            else return Total / CurrentWorkTime;
        }

    }
}
