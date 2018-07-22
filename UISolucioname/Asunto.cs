using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISolucioname
{
    public static class Asunto
    {
        private static frmMainFrame MainWindow
        {
            get
            {
                return App.Current.MainWindow as frmMainFrame;
            }
        }

    }
}
