using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIBackoffice
{
    public class OperBackoffice : Entidades.Operador, INotifyPropertyChanged
    {
        public string FullName {
            get {
                return Apellido + ", " + Nombre;
            }
        }

        public DateTime NextEvent {
            get {
                // Sets current time
                DateTime currentTime = DateTime.Now;
                // Iterates all breaks
                for (int i = 0; i < Breaks.Count; i++) {
                    if (Breaks[i].Start > currentTime) {
                        return Breaks[i].Start;
                    }
                }
                return EndTime;
            }
        }

        private int timeLeftToNextEvent;

        public event PropertyChangedEventHandler PropertyChanged;

        public int TimeLeftToNextEvent {
            get {
                return timeLeftToNextEvent;
            }
            set {
                if(timeLeftToNextEvent != value) {
                    timeLeftToNextEvent = value;
                    NotifyPropertyChanged("TimeLeftToNextEvent");
                }                
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public OperBackoffice(Entidades.Operador paramOperator)
        {
            UserName = paramOperator.UserName;
            Nombre = paramOperator.Nombre;
            Apellido = paramOperator.Apellido;
            Status = paramOperator.Status;
            Breaks = paramOperator.Breaks;
            StartTime = paramOperator.StartTime;
            EndTime = paramOperator.EndTime;
        }
    }
}
