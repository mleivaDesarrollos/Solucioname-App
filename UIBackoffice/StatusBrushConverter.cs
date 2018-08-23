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
    public class StatusBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert binded value to the class
            Entidades.AvailabiltyStatus statusOfConnection = (Entidades.AvailabiltyStatus)value;
            // Select correct brush for connection status
            switch (statusOfConnection) {
                case Entidades.AvailabiltyStatus.Disconnected: return Brushes.PaleVioletRed;
                case Entidades.AvailabiltyStatus.Connected: return Brushes.LightGreen;
                case Entidades.AvailabiltyStatus.ReadyToReceive: return Brushes.LightGreen;
                case Entidades.AvailabiltyStatus.Break: return Brushes.Wheat;
                case Entidades.AvailabiltyStatus.Bath: return Brushes.Wheat;
                case Entidades.AvailabiltyStatus.SpecialTask: return Brushes.Wheat;
                case Entidades.AvailabiltyStatus.Error: return Brushes.PaleVioletRed;
                default: return Brushes.White;
            }            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
