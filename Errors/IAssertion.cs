using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Errors
{
    public interface IAssertion
    {
        void Notify(string prmMessage);

        void NotifyAndClose(string prmMessage);
    }
}
