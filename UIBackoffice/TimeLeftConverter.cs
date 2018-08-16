using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UIBackoffice
{
    public class TimeLeftConverter : IMultiValueConverter
    {
        // string add for hour time
        static string strHr = " hr.";

        // string add for minute time
        static string strMin = " min.";

        static string strBrk = "Br. ";

        static string strEndWorkingDay = "Finalizado";

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int timeLeft = System.Convert.ToInt16(values[0]);
            int stoppedTimeLeft = System.Convert.ToInt16(values[1]);
            // Checks if status stopped
            if (stoppedTimeLeft != 0) {
                return strBrk + stoppedTimeLeft + strMin;
            }
            // To divide total hour and minutes, if the value is over an hour splits
            if (timeLeft >= 60) {
                // Sets a hour total
                int iHours = TimeSpan.FromMinutes(timeLeft).Hours;
                // Sets a minutes
                int iMinutes = timeLeft - (iHours * 60);
                // If minutes is 0, is an exact hour
                if (iMinutes <= 0) {
                    return iHours + strHr;
                }
                // else is hour plus minutes
                else {
                    string resultValue = iHours + strHr + " " + iMinutes + strMin;
                    return resultValue;
                }
            }
            else if (timeLeft < 0) {
                return strEndWorkingDay;
            }
            else {
                return timeLeft + strMin;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {/*
            string strValue = value.ToString();
            if (strValue.Contains(strHr)) {
                int iTotalMinutes = 0;
                string[] hourMinutes = strValue.Split(new[] { strHr, strMin, " " }, StringSplitOptions.RemoveEmptyEntries);
                iTotalMinutes = System.Convert.ToInt16(hourMinutes[0]) * 60;
                if (hourMinutes.Length > 2) {
                    iTotalMinutes = iTotalMinutes + System.Convert.ToInt16(hourMinutes[1]);
                }
                return iTotalMinutes;
            }
            else if (strValue.Contains(strMin)) {
                int intMin = System.Convert.ToInt16(strValue.Replace(strMin, ""));
                return intMin;
            }
            return null;*/
            throw new NotSupportedException();
        }
    }
}
