using System;
using System.Threading;
using System.Windows.Controls;

namespace UISolucioname.ThreadManage
{
    public static class Tooltip
    {
        private static ToolTip _tooltipModif;

        private static Page _page;

        public static void Limpiar(ToolTip parTool, Page parPage)
        {
            _tooltipModif = parTool;
            _page = parPage;
            Thread Limpiar = new Thread(TareaLimpiar);
            Limpiar.IsBackground = true;
            Limpiar.Start();
        }

        private static void TareaLimpiar()
        {
            Thread.Sleep(5000);
            _page.Dispatcher.BeginInvoke((Action)(() =>
          {
              _tooltipModif.IsOpen = false;
              _tooltipModif.Visibility = System.Windows.Visibility.Hidden;
          }
            ));
        }


    }
}
