using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UIBackoffice
{
    public class TimeLeftConverter : IValueConverter
    {
        // string add for hour time
        static string strHr = " hr.";

        // string add for minute time
        static string strMin = " min.";

        static string strEndWorkingDay = "Finalizado";


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = System.Convert.ToInt16(value);
            // To divide total hour and minutes, if the value is over an hour splits
            if (intValue >= 60) {
                // Sets a hour total
                int iHours = TimeSpan.FromMinutes(intValue).Hours;
                // Sets a minutes
                int iMinutes = intValue - (iHours * 60);
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
            else if (intValue < 0) {
                return strEndWorkingDay;
            }
            else{
                return intValue + strMin;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value.ToString();
            if (strValue.Contains(strHr)) {
                int iTotalMinutes = 0;
                string[] hourMinutes = strValue.Split(new[]{ strHr, strMin , " "}, StringSplitOptions.RemoveEmptyEntries);
                iTotalMinutes = System.Convert.ToInt16(hourMinutes[0]) * 60;
                if (hourMinutes.Length > 2) {
                    iTotalMinutes = iTotalMinutes + System.Convert.ToInt16(hourMinutes[1]);
                }
                return iTotalMinutes;
            }
            else if (strValue.Contains(strMin)){
                int intMin = System.Convert.ToInt16(strValue.Replace(strMin, ""));
                return intMin;
            }
            return null;            
        }
    }
}
