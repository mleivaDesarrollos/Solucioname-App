using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UIBackoffice.Util
{
    public static class MetodoExtension
    {
        /// <summary>
        /// Metodo de extensión para los datagrid, reordena y refresca el listado a solicitud
        /// </summary>
        /// <param name="dg"></param>
        /// <param name="iColIndex"></param>
        /// <param name="sortDirection"></param>
        public static void ReordenarDatagrid(this DataGrid dg, int iColIndex = 0, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            // Almacenamos la columna en una variable
            var columna = dg.Columns[iColIndex];
            // Limpiamos los ordenamientos actuales
            dg.Items.SortDescriptions.Clear();
            // Agregamos un ordenamiento
            dg.Items.SortDescriptions.Add(new SortDescription(columna.SortMemberPath, sortDirection));
            // Recorremos todas las columnas del datagrid y limpiamos si tienen sortdirections
            foreach (var col in dg.Columns)
            {
                col.SortDirection = null;
            }
            // Se establece la dirección de ordenamiento
            columna.SortDirection = sortDirection;
            // Refrescamos el listado
            dg.Items.Refresh();
        }
    }
}
