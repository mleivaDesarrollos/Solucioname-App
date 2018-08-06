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

namespace UIBackoffice.Util
{
    /// <summary>
    /// Interaction logic for CustomMsgBox.xaml
    /// </summary>
    public partial class MsgBox : Window
    {
        public MsgBox()
        {
            InitializeComponent();
            ConfigurarCustomWindow();
            if (App.Current.MainWindow != null && App.Current.MainWindow.IsActive) Owner = App.Current.MainWindow;
            btnSi.IsDefault = true;
            btnNo.IsCancel = true;
            btnOk.IsDefault = true;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public static bool? Consulta(String pStrConsulta)
        {
            MsgBox MsgBox = new MsgBox();
            MsgBox.btnOk.Visibility = Visibility.Hidden;
            MsgBox.dkpYesNo.Visibility = Visibility.Visible;
            MsgBox.txtMensaje.Text = pStrConsulta;
            MsgBox.dkpYesNo.IsEnabled = true;
            MsgBox.Width = 250;
            MsgBox.Height = 140;
            MsgBox.grdContenedor.Margin = new Thickness(0);                        
            return MsgBox.ShowDialog();
        }

        public static void Error(String pStrConsulta)
        {
            MsgBox MsgBox = new MsgBox();
            MsgBox.txtMensaje.Text = pStrConsulta;
            MsgBox.Show();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnSi_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
