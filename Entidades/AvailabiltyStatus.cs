using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Entidades
{    
    public enum AvailabiltyStatus
    {    
        Disconnected,
        Connected,
        ReadyToReceive,
        Break,
        Bath,
        SpecialTask,
        Error
    }
}
