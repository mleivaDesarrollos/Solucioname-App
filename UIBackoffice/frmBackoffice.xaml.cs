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
using Entidades;
using Entidades.Service.Interface;

namespace UIBackoffice
{
    /// <summary>
    /// Interaction logic for frmBackoffice.xaml
    /// </summary>
    public partial class frmBackoffice : Window, Entidades.Service.Interface.IServicioCallback
    {
        #region constructor
        public frmBackoffice()
        {
            InitializeComponent();
            ConfigurarCustomWindow();
            SetUpDate();   
        }
        #endregion

        #region event_subscription

        /// <summary>
        /// On closing window of any source, sents a confirmation dialog to front.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ConsultarCierreApp() == true) {
                e.Cancel = false;
            }
        }
        private void btnGetOperatorList_Click(object sender, RoutedEventArgs e)
        {
            PopulateOperatorList();
        }
        #endregion

        #region helper_methods
        /// <summary>
        /// Standarized message for closing application
        /// </summary>
        /// <returns>null, false or true</returns>
        private bool? ConsultarCierreApp()
        {
            return Util.MsgBox.Consulta("¿Estás seguro de que deseas salir de la aplicacion?");
        }
        /// <summary>
        /// Get and set operator information from the service
        /// </summary>
        private async void PopulateOperatorList()
        {
            try
            {
                // Generate a new Operator Logic object
                Logica.Operador logOper = new Logica.Operador();
                // Create a new list and save all operators on them
                List<Entidades.Operador> lstOperators = await logOper.GetFullOperatorList();
                // Get connected operators
                string strConnectedOperators = "";
                // Fetchs all operators and add short info to test results
                foreach (var operConnected in lstOperators)
                {
                    strConnectedOperators += operConnected.UserName + ", " + operConnected.Nombre + ", " + operConnected.Apellido + ".";
                }                // Generate a messagebox with short description of the user logged in
                Util.MsgBox.Error("Connected operators: " + strConnectedOperators);
            }
            catch (Exception ex)
            {
                // Shows error details on a message box
                Util.MsgBox.Error("Ha ocurrido un error al llenar el listado de operadores: " + ex.Message);   
            }
        }

        private void SetUpDate()
        {
            txtTodayDate.Text = DateTime.Now.ToShortDateString();
        }
        #endregion

        #region service_callback_implementation
        public void EnviarAsunto(Asunto a)
        {
            
        }

        public void ForceDisconnect()
        {
            
        }

        public void Mensaje(string message)
        {
            // Calls to show message on async thread
            Dispatcher.BeginInvoke((Action)(() =>
            {
                Util.MsgBox.Error("Mensaje proveniente del servicio: " + message);
            }));            
        }

        public void ServiceChangeStatusRequest(AvailabiltyStatus paramNewStatus)
        {
            
        }

        bool IServicioCallback.IsActive()
        {
            return true;
        }
        #endregion

    }
}
