using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace UISolucioname
{
    public class Validador
    {

        /// <summary>
        /// Valida todos los controles de una ventana que se pase por parametro. Si existe algún error de validación se remarca
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool EsValido(DependencyObject parent)
        {
            bool valido = true;
            LocalValueEnumerator localValues = parent.GetLocalValueEnumerator();
            while (localValues.MoveNext())
            {
                LocalValueEntry entry = localValues.Current;
                if (BindingOperations.IsDataBound(parent, entry.Property))
                {
                    Binding binding = BindingOperations.GetBinding(parent, entry.Property);
                    foreach (ValidationRule reglaValidacion in binding.ValidationRules)
                    {
                        ValidationResult result = reglaValidacion.Validate(parent.GetValue(entry.Property), null);
                        if (!result.IsValid)
                        {
                            BindingExpression expresion = BindingOperations.GetBindingExpression(parent, entry.Property);
                            System.Windows.Controls.Validation.MarkInvalid(expresion, new ValidationError(reglaValidacion, expresion, result.ErrorContent, null));                            
                            valido = false;
                        }
                    }
                }
            }
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!EsValido(child)) { valido = false; }
            }
            return valido;
        }

        /// <summary>
        /// Recorre todas las validaciones cargadas en el pariente para desactivarlas
        /// </summary>
        /// <param name="parent"></param>
        public static void LimpiarValidaciones(DependencyObject parent)
        {
            LocalValueEnumerator localValues = parent.GetLocalValueEnumerator();
            while (localValues.MoveNext())
            {
                LocalValueEntry entry = localValues.Current;
                if (BindingOperations.IsDataBound(parent, entry.Property))
                {
                    Binding binding = BindingOperations.GetBinding(parent, entry.Property);
                    foreach (ValidationRule reglaValidacion in binding.ValidationRules)
                    {
                        ValidationResult result = reglaValidacion.Validate(parent.GetValue(entry.Property), null);
                        if (!result.IsValid)
                        {
                            BindingExpression expresion = BindingOperations.GetBindingExpression(parent, entry.Property);
                            System.Windows.Controls.Validation.ClearInvalid(expresion);
                        }
                    }
                }
            }
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                LimpiarValidaciones(child);
            }
        }
    }
}
