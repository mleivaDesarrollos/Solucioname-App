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
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Entidades.AvailabiltyStatus statusOfConnection = (Entidades.AvailabiltyStatus) value;
            switch (statusOfConnection) {
                case Entidades.AvailabiltyStatus.Disconnected:
                    return "Desconectado";
                case Entidades.AvailabiltyStatus.Connected:
                    return "Conectado";
                case Entidades.AvailabiltyStatus.ReadyToReceive:
                    return "Disponible";                    
                case Entidades.AvailabiltyStatus.Break:
                    return "Descanso";
                case Entidades.AvailabiltyStatus.Bath:
                    return "Baño";
                case Entidades.AvailabiltyStatus.SpecialTask:
                    return "Tareas";
                case Entidades.AvailabiltyStatus.Error:
                    return "Error";
                default:
                    return "Desconocido";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
