using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace UIBackoffice
{
    public class TimeLeftBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int timeleft = System.Convert.ToInt16(values[0]);
            int stoppedTimeLeft = System.Convert.ToInt16(values[1]);
            DateTime startTime = System.Convert.ToDateTime(values[2]);

            // if the journey is not started
            if (DateTime.Now <= startTime) {
                return Brushes.PaleVioletRed;
            }

            if (stoppedTimeLeft != 0) {
                return Brushes.CornflowerBlue;
            }
            if (timeleft <= 0) {
                return Brushes.PaleVioletRed;
            }
            else if (timeleft < 15) {
                return Brushes.Wheat;
            }
            else if (timeleft < 30) {
                return Brushes.GreenYellow;
            }
            return Brushes.LightGreen;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }        
    }
}
