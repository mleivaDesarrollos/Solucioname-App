using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UISolucioname
{
    public class ValidarActuacion : ValidationRule
    {
        public UIElement Element
        {
            get; set;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string valor = (string)value;
            // Diponemos del TextBox para poder utilizarlo en consultas posteriores
            TextBox txtAct = Element as TextBox;
            // Consultamos si el TextBox es nulo y si esta habilitado
            if (txtAct != null && txtAct.IsEnabled)
            {
                if (!ComprobarInicio(valor) || valor.Length == 0)
                {
                    return new ValidationResult(false, "El asunto debe comenzar con el número 5000 o 6000");
                }
                if (!ComprobarFinalizacion(valor))
                {
                    return new ValidationResult(false, "El asunto debe finalizar con 6 digitos posterior al 5000 o 6000");
                }
            }            
            return new ValidationResult(true, null);
        }

        private bool ComprobarInicio(string pStrValor)
        {
            Regex rgxComprobInicio = new Regex(@"^5000");
            if (rgxComprobInicio.IsMatch(pStrValor))
            {
                return true;
            }
            rgxComprobInicio = new Regex(@"^6000");
            if (rgxComprobInicio.IsMatch(pStrValor))
            {
                return true;
            }
            return false;
        }

        private bool ComprobarFinalizacion(string pStrValor)
        {
            Regex rgxComprobacionFin = new Regex(@"5000\d{6}$");
            if (rgxComprobacionFin.IsMatch(pStrValor))
            {
                return true;
            }
            rgxComprobacionFin = new Regex(@"6000\d{6}$");
            if (rgxComprobacionFin.IsMatch(pStrValor))
            {
                return true;
            }
            return false;
        }
    }
}
