using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UISolucioname
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Configuramos el apagado de la aplicacion, si se deja por defecto luego de cerrar el dialogo de logueo la aplicacion se cierra
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            // Disponemos de la instancia de la aplicación sin cargar ningun dato relacionado a usuario
            frmMainFrame MainApp = new frmMainFrame();
            // Establecemos como ventana principal la instaciación previa
            Current.MainWindow = MainApp;
            // Generamos la nueva instancia de la ventana de logueo
            frmLogin reqLogUser = new frmLogin();
            //  Confirmamos si el resultado del dialogo abierto da verdadero
            if (reqLogUser.ShowDialog() == true)
            {
                // Dejamos abierta la propiedad de operador para cualquier entidad que lo requiera
                App.Current.Properties["user"] = reqLogUser.Operador;
                MainApp.CargarInformacionUsuario();
                MainApp.Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}
