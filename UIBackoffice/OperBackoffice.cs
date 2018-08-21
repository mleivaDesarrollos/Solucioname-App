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
        internal enum WorkingDayStatus
        {
            PreviousToStart,
            Started,
            Ended
        }

        public string FullName {
            get {
                return Apellido + ", " + Nombre;
            }
        }

        private DateTime nextEvent { get; set; }

        /// <summary>
        /// On modify Next Event, notify to binded WPF items for update UI
        /// </summary>
        public DateTime NextEvent {
            get {
                return nextEvent;
            }
            set {
                if (nextEvent != value) {
                    nextEvent = value;
                    NotifyPropertyChanged("NextEvent");
                }
            }
        }

        private int stoppedTimeLeft { get; set; }


        /// <summary>
        /// On modify StoppedTimeLeft, notify to binded WPF items for update UI
        /// </summary>
        public int StoppedTimeLeft {
            get {
                return stoppedTimeLeft;
            }
            set {
                if (stoppedTimeLeft != value) {
                    stoppedTimeLeft = value;
                    NotifyPropertyChanged("StoppedTimeLeft");
                }
            }
        }

        private int timeLeftToNextEvent;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// On modify TimeLeftEvent, notify to binded WPF items for update UI
        /// </summary>
        public int TimeLeftToNextEvent {
            get {
                return timeLeftToNextEvent;
            }
            set {
                if (timeLeftToNextEvent != value) {
                    timeLeftToNextEvent = value;
                    NotifyPropertyChanged("TimeLeftToNextEvent");
                }
            }
        }

        private WorkingDayStatus WorkStatus {
            get {
                DateTime currentTime = DateTime.Now;
                if (currentTime <= StartTime) {
                    return WorkingDayStatus.PreviousToStart;
                }
                else if (currentTime >= EndTime) {
                    return WorkingDayStatus.Ended;
                }
                else {
                    return WorkingDayStatus.Started;
                }
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) {
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

        /// <summary>
        /// Gets and set the next event on operator day
        /// </summary>
        private void CalculateNextEvent()
        {
            switch (WorkStatus) {
                case WorkingDayStatus.PreviousToStart:
                    NextEvent = StartTime;
                    break;
                case WorkingDayStatus.Started:
                    NextEvent = calculateNextEventOnBreak();
                    break;
                case WorkingDayStatus.Ended:
                    NextEvent = EndTime;
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// Iterates on all breaks to get next break
        /// </summary>
        /// <returns></returns>
        private DateTime calculateNextEventOnBreak()
        {
            foreach (var breakTime in Breaks) {
                if(breakTime.Start >= DateTime.Now) {
                    return breakTime.Start;
                }
            }
            // if are not a break on the list returns end of working day
            return EndTime;
        }

        /// <summary>
        /// Display a interface for update data corresponding to time left to the user
        /// </summary>
        internal void CalculateTimeLeft()
        {
            CalculateNextEvent();
            TimeSpan differenceWithCurrentTime = NextEvent - DateTime.Now;
            TimeLeftToNextEvent = Convert.ToInt32(differenceWithCurrentTime.TotalMinutes);
            CalculateStoppedTimeLeft();
        }

        /// <summary>
        /// Calculates time left on stopped status
        /// </summary>
        private void CalculateStoppedTimeLeft()
        {
            // Checks all breaks
            foreach (var breakTime in Breaks) {
                // if now is between break range
                if (DateTime.Now >= breakTime.Start && DateTime.Now <= breakTime.End) {
                    // Sets timespan
                    TimeSpan diffOnBreak = breakTime.End - DateTime.Now;
                    // Save minutes difference on variable
                    StoppedTimeLeft = diffOnBreak.Minutes;
                    break;
                }
            }
        }
    }
}
