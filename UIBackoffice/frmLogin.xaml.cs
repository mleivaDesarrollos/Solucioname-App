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

namespace UIBackoffice
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmLogin : Window
    {
        public Entidades.Operador Operador
        {
            get;set;
        }

        private bool _logInPetition;

        private bool logInPetition
        {
            set
            {
                // Si la petición de ingreso ya fue hecha no se realizara ningun cambio
                if(_logInPetition != value)
                {
                    if(value)
                    {
                        // Activamos la pantalla de carga
                        grdAwaitingResponse.Visibility = Visibility.Visible;
                        // Desactivamos la pantalla de ingreso
                        grdLogInUI.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        // Activamos la visibilidad de los elementos de logueo
                        grdLogInUI.Visibility = Visibility.Visible;
                        // Desactivamos la pantalla de carga
                        grdAwaitingResponse.Visibility = Visibility.Hidden;
                    }
                    _logInPetition = value;
                }
            }
        }


        public frmLogin()
        {
            InitializeComponent();
            ConfigurarCustomWindow();
            txtUsername.Focus();
        }

        private void btnLoguear_Click(object sender, RoutedEventArgs e)
        {
            logInPetition = true;
            SendConnectOrder();
        }

        private async void SendConnectOrder()
        {
            try
            {
                // Generamos un objeto de logica operador
                Logica.Operador logOper = new Logica.Operador();
                // Disponemos de una entidad operador
                Entidades.Operador entOper = new Entidades.Operador()
                {
                    UserName = txtUsername.Text,
                    Password = txtPassword.Password
                };
                // Consultamos a las capas de negocio para validar el ingreso
                // TODO : Modificar el logueo para que se haga sobre el servicio.
                Entidades.Operador operResponse = await logOper.ConnectBackoffice(entOper);
                if (operResponse != null)
                {
                    Operador = operResponse;
                    DialogResult = true;
                }
                else
                {
                    Util.MsgBox.Error("El usuario o la clave es incorrecta.");
                    logInPetition = false;
                }
            }
            catch (Exception ex)
            {
                Util.MsgBox.Error(ex.Message);
                logInPetition = false;
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
