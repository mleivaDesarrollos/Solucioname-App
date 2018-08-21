using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Errors
{
    public static class Assert
    {
        private static IAssertion _interface { get; set; }

        private static IAssertion Interface{
            get {
                if (_interface == null) throw new Exception("Fatal error: there is no interface related to Assertion class");
                return _interface;
            }
            set {
                _interface = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prmAssertionInterface"></param>
        public static void LoadInterface(IAssertion prmAssertionInterface)
        {
            // Checks if the interface passed on parameter is correct
            if (prmAssertionInterface == null) throw new Exception("Fatal error : Assertion interface is null");
            // Load interface on variable
            Interface = prmAssertionInterface;
        }


        /// <summary>
        /// Public interface to manage a Exception. 
        /// </summary>
        /// <param name="prmExcept"></param>
        public static void Throw(Exception prmExcept)
        {
            // Checks if exception is correctly loaded
            if (prmExcept == null) throw new Exception("Fatal error: Exception passed on parameter is null");
            // Send a message to notify assert and close application
            Interface.NotifyAndClose(prmExcept.Message);
        }
    }
}
