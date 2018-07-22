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

namespace UISolucioname
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

        public frmLogin()
        {
            InitializeComponent();
            ConfigurarCustomWindow();
            txtUsername.Focus();
        }

        private void btnLoguear_Click(object sender, RoutedEventArgs e)
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
                entOper = logOper.Ingresar(entOper);
                if (entOper != null)
                {
                    Operador = entOper;
                    DialogResult = true;
                }
                else
                {
                    Util.MsgBox.Error("El usuario o la clave es incorrecta.");
                }
            }
            catch (Exception ex)
            {
                Util.MsgBox.Error(ex.Message);
            }
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void btnReporte_Click(object sender, RoutedEventArgs e)
        {
            wpfReportePrueba wpfReportPrueba = new wpfReportePrueba();
            wpfReportPrueba.Show();
        }
    }
}
