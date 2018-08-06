using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio_Principal.SQL
{
    public sealed class AvailabilityStatus
    {
        private static readonly Lazy<AvailabilityStatus> lazy = new Lazy<AvailabilityStatus>(() => new AvailabilityStatus());

        private static AvailabilityStatus Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private AvailabilityStatus()
        {

        }
    }
}
