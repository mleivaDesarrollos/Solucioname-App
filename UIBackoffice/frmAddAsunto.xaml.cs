using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Errors;

namespace UIBackoffice
{
    /// <summary>
    /// Interaction logic for frmAddAsunto.xaml
    /// </summary>
    public partial class frmAddAsunto : Window
    {
        Entidades.Operador operatorToSent;

        public Entidades.Asunto confirmedNewAsunto;

        public frmAddAsunto(Entidades.Operador prmOperatorToSent)
        {
            InitializeComponent();
            ConfigurarCustomWindow();
            operatorToSent = prmOperatorToSent;
            txtOperator.Text = operatorToSent.Nombre + " " + operatorToSent.Apellido;
        }

        #region event_subscription
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try {
                // Prepares new entity to travel to service
                Entidades.Asunto newAsunto = new Entidades.Asunto()
                {
                    Numero = txtAsuntoNumber.Text,
                    DescripcionBreve = txtShortDescription.Text,
                    Oper = operatorToSent,
                    isCreatedByBackoffice = true
                };
                // Generates a new logic asunto object
                Logica.Asunto logAsunto = new Logica.Asunto();
                // Gets operator logged on application
                Entidades.Operador backofficeOperator = App.Current.Properties["user"] as Entidades.Operador;
                // Calls a sent method
                logAsunto.SentAsuntoToOperator(backofficeOperator, newAsunto);
                // Set property on public property
                confirmedNewAsunto = newAsunto;
                // Sets result to true
                DialogResult = true;
            }
            catch (Exception ex) {
                Except.Throw(ex);   
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}
