using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace UISolucioname
{
    public partial class frmMainFrame : Window
    {
        /// <summary>
        /// Clase Parcial que utilizamos para mantener separada la customización de la ventana de WPF, permite de esta forma mantener el codigo mas limpio. De esta forma tambien es mas escalable el codigo.
        /// </summary>
        private void ConfigurarCustomWindow()
        {
            if (esMaximizado())
            {
                imgBtnDimensionar.Source = new BitmapImage(new Uri(@"/Images/RestoreButton.png", UriKind.Relative));
            }
            SubscribirEventos();
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }
        private void SubscribirEventos()
        {
            btnCerrarWindow.Click += BtnCerrarWindow_Click;
            btnDimensionarWindow.Click += BtnDimensionarWindow_Click;
            btnMinimizeWindow.Click += BtnMinimizeWindow_Click;
            BarraTitulo.MouseDown += BarraTitulo_MouseDown;
            BarraTitulo.MouseLeftButtonDown += BarraTitulo_MouseLeftButtonDown;
        }

        private void BarraTitulo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (esMaximizado())
                {
                    WindowState = WindowState.Normal;
                }
                else
                {
                    WindowState = WindowState.Maximized;
                }
            }
        }

        private void BarraTitulo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void BtnMinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnDimensionarWindow_Click(object sender, RoutedEventArgs e)
        {
            if (esMaximizado())
            {
                WindowState = WindowState.Normal;
                imgBtnDimensionar.Source = new BitmapImage(new Uri(@"/Images/MaximizeButton.png", UriKind.Relative));
            }
            else
            {
                WindowState = WindowState.Maximized;
                imgBtnDimensionar.Source = new BitmapImage(new Uri(@"/Images/RestoreButton.png", UriKind.Relative));
            }
        }

        private void BtnCerrarWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private Boolean esMaximizado()
        {
            if (WindowState == WindowState.Maximized)
            {
                return true;
            }
            return false;
        }
    }
}
