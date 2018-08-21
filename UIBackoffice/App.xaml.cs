using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UIBackoffice
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {                
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Configuramos el modo de cierre del aplicativo, asi es posible mantenerlo activo hasta que la ventana principal se cierre
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            // Generate a new instance of backoffice window hidden until the login is approved
            frmBackoffice frmMainApp = new frmBackoffice();
            // Set MainWindow properties of application
            Application.Current.MainWindow = frmMainApp;
            // Disponemos de un objeto de login
            frmLogin frmLogInBack = new frmLogin();
            // Activamos temporalmente la ventana de dialogo
            if (frmLogInBack.ShowDialog() == true)
            {
                // Set on application properties the current user logged on system
                App.Current.Properties["user"] = frmLogInBack.Operador;
                // Show the main app dialog on screen
                frmMainApp.Show();
            }
            else
            {
                // if the dialog result is false the application has to be closed
                Application.Current.Shutdown();
            }
        }
    }
}
