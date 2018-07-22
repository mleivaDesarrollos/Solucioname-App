using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UISolucioname
{
    public class ValidarAsunto : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string valor = (string)value;
            if (!ComprobarInicio(valor) || valor.Length == 0)
            {
                return new ValidationResult(false, "El asunto debe comenzar con el número 3000");
            }
            if (!ComprobarFinalizacion(valor))
            {
                return new ValidationResult(false, "El asunto debe finalizar con 6 digitos posterior al 3000");
            }
            return new ValidationResult(true, null);
        }

        private bool ComprobarInicio(string pStrValor)
        {
            Regex rgxComprobInicio = new Regex(@"^3000");
            if (rgxComprobInicio.IsMatch(pStrValor))
            {
                return true;
            }
            return false;
        }

        private bool ComprobarFinalizacion(string pStrValor)
        {
            Regex rgxComprobacionFin = new Regex(@"3000\d{6}$");
            if (rgxComprobacionFin.IsMatch(pStrValor))
            {
                return true;
            }
            return false;
        }
        
    }
}
