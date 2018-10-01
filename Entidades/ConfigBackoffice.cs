using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Entidades.Exceptions;

namespace Entidades
{
    /// <summary>
    /// Entity who saves all configuration related to backoffice operation
    /// </summary>
    public class ConfigBackoffice
    {       

        private int returningToWorkFromBreakInMinutesOffset;

        public int ReturningToWorkFromBreakInMinutesOffset {
            get { return returningToWorkFromBreakInMinutesOffset; }
            private set {
                if (value < 0) {
                    throw new ConfigException("ReturningToWorkFromBreakInMinutesOffset");
                }
                returningToWorkFromBreakInMinutesOffset = value;
            }
        }

        private int startingMinutesOffset;

        public int StartingMinutesOffset {
            get { return startingMinutesOffset; }
            set {
                if(value < 0) {
                    throw new ConfigException("StartingMinutesOffset");
                }
                startingMinutesOffset = value;
            }
        }

        private int stopProximityMinutesOffset;

        public int StopProximityMinutesOffset {
            get { return stopProximityMinutesOffset; }
            set {
                if (value < 0) {
                    throw new ConfigException("BreakProximityMinutesOffset");
                }
                stopProximityMinutesOffset = value;
            }
        }        
        public ConfigBackoffice()
        {
            // Proceed to load basic of the files
            ReturningToWorkFromBreakInMinutesOffset = Convert.ToInt16(ConfigurationManager.AppSettings.GetValues("RETURNING_BREAK_ACTIVITY_OFFSET_MINUTES")[0]);
            StartingMinutesOffset = Convert.ToInt16(ConfigurationManager.AppSettings.GetValues("STARTING_ACTIVITY_OFFSET_MINUTES")[0]);
            StopProximityMinutesOffset = Convert.ToInt16(ConfigurationManager.AppSettings.GetValues("STOP_PROXIMITY_OFFSET_MINUTES")[0]);
        }
    }
}
