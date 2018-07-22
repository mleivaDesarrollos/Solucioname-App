using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UISolucioname
{
    /// <summary>
    /// Valida si los ingresos registrados son nulos
    /// </summary>
    public class ValidarNulo : ValidationRule
    {
        public UIElement Element
        {
            get; set;
        }

        public bool Requiere = true;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (Requiere)
            {
                // Convertimos el valor a String para poder operarlo en la rutina
                string valor = (string)value;
                // Validamos y disponemos el control como TextBox
                TextBox txtContenedor = Element as TextBox;
                if (txtContenedor != null && txtContenedor.IsEnabled)
                {
                    // Consultamos si hay valores cargados dentro del string, si no hay nada cargado damos un rechazo
                    if (valor.Length <= 0)
                    {
                        return new ValidationResult(false, "El campo no puede permanecer vacío.");
                    }
                }
            }            
            return new ValidationResult(true, null);
        }
    }
}
