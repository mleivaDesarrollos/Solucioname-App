﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace UIBackoffice
{
    public partial class frmAddBatchAsunto : Window
    {
        /// <summary>
        /// Clase Parcial que utilizamos para mantener separada la customización de la ventana de WPF, permite de esta forma mantener el codigo mas limpio. De esta forma tambien es mas escalable el codigo.
        /// </summary>
        private void ConfigurarCustomWindow()
        {
            SubscribirEventos();
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }
        private void SubscribirEventos()
        {
            btnCerrarWindow.Click += BtnCerrarWindow_Click;
            BarraTitulo.MouseDown += BarraTitulo_MouseDown;
        }
        

        private void BarraTitulo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) {
                DragMove();
            }
        }

        private void BtnMinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        

        private void BtnCerrarWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
    }
}
