using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicio_Principal
{
    public static class Log
    {
        public static string GetFullShortDateTime
        {
            get
            {
                return DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            }
        }
        /// <summary>
        /// Manages a error
        /// </summary>
        /// <param name="classSource">Class source of the error</param>
        /// <param name="message">error message</param>
        public static void Error(string classSource, string message)
        {
            Console.WriteLine(GetFullShortDateTime + " : [Error] " + classSource + " - " + message);
        }

        /// <summary>
        /// Information log
        /// </summary>
        /// <param name="classSource">class source of the error</param>
        /// <param name="message"></param>
        public static void Info(string classSource, string message)
        {
            Console.WriteLine(GetFullShortDateTime + " : [Info] " + classSource + " - " + message);
        }
    }
}
