using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Errors
{
    public static class Except
    {
        #region property
        private static IException _interface { get; set; }

        private static IException Interface {
            get {
                if (_interface == null) throw new Exception("Fatal error: there is no interface related to Exception class");
                return _interface;
            }
            set {
                _interface = value;
            }
        }
        #endregion

        #region public_interface
        /// <summary>
        /// Expose a method to comunicate interface with error process
        /// </summary>
        /// <param name="prmInterfaceException"></param>
        public static void LoadInterface(IException prmInterfaceException)
        {
            // Check if parameter is valid
            if (prmInterfaceException == null) throw new Exception("Fatal Error : the Exception interface has become empty.");
            // Load parameter in static value            
            Interface = prmInterfaceException;
        }
        #endregion
    }
}
