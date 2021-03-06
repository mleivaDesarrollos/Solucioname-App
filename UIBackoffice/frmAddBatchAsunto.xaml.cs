﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using Errors;
using System.Windows.Media;

namespace UIBackoffice
{
    /// <summary>
    /// Interaction logic for frmAddBatchAsunto.xaml
    /// </summary>
    public partial class frmAddBatchAsunto : Window
    {
        bool bllSelectedOnly = false;

        private class OperatorAsuntoDistribute
        {
            public Entidades.Operador Operator { get; set; }
            
            public string FullName {
                get {
                    if(Operator == null) {
                        return "<Elegir>";
                    } else {
                        return Operator.Apellido + ", " + Operator.Nombre;
                    }
                    
                }
            }
            public bool Equals(Entidades.Operador operToCompare)
            {
                return Operator.UserName == operToCompare.UserName;
            }

        }

        List<OperatorAsuntoDistribute> lstOperatorForAssign;

        Logica.Balance currentBalanceFromOperators;

        ObservableCollection<Entidades.Asunto> lstAsuntoToAssign;

        public frmAddBatchAsunto(List<Entidades.Operador> prmListOfOperatorToAssign, List<Entidades.Asunto> prmListOfAsuntosToAssign, Logica.Balance logBalanceProcessing)
        {
            InitializeComponent();
            ConfigurarCustomWindow();
            lstOperatorForAssign = getOperatorListForCombo(prmListOfOperatorToAssign);
            lstAsuntoToAssign = ConvertFromListAsuntoToObservableCollection(prmListOfAsuntosToAssign);
            currentBalanceFromOperators = logBalanceProcessing;
            dgAsuntosWithoutAssignation.ItemsSource = lstAsuntoToAssign;
        }

        private List<OperatorAsuntoDistribute> getOperatorListForCombo(List<Entidades.Operador> prmSourceList)
        {
            // Generate a new list to return in process
            List<OperatorAsuntoDistribute> lstOADToShow = new List<OperatorAsuntoDistribute>();
            // Generate a new OAD for selection show
            OperatorAsuntoDistribute noSelectionOperator = new OperatorAsuntoDistribute();
            // Add the no selection operator
            lstOADToShow.Add(noSelectionOperator);
            // Iterate over all operback
            foreach (var operatorToAdd in prmSourceList) {
                lstOADToShow.Add(new OperatorAsuntoDistribute() { Operator = operatorToAdd });
            }
            // Return processed list
            return lstOADToShow;
        }


        private ObservableCollection<Entidades.Asunto> ConvertFromListAsuntoToObservableCollection(List<Entidades.Asunto> prmListAsunto)
        {
            // Generate a new instance of ObservableCollection
            ObservableCollection<Entidades.Asunto> lstObsrvCol = new ObservableCollection<Entidades.Asunto>();
            // Iterates over all asuntos communicated
            foreach (var asunto in prmListAsunto) {
                lstObsrvCol.Add(asunto);
            }
            // Return processed collection
            return lstObsrvCol;
        }

        private async void btnLoadBatchAsuntos_Click(object sender, RoutedEventArgs e)
        {

            try {
                List<Entidades.Asunto> lstToDistribute;
                if (!bllSelectedOnly) {
                    CheckLoadedUsersInAsunto();
                    lstToDistribute = lstAsuntoToAssign.ToList();
                } else {
                    isOneOperatorLoaded();
                    lstToDistribute = lstAsuntoToAssign.Where(asunto => asunto.Oper != null).ToList();
                }
                // Set sending time to current
                DateTime dtSendingTime = DateTime.Now;
                // On all filtered asuntos save sending date
                lstToDistribute.ForEach(asunto => asunto.SendingDate = dtSendingTime);
                // Generate a new logic asunto object
                Logica.Asunto logAsunto = new Logica.Asunto();
                Entidades.Operador currBackOffice = App.Current.Properties["user"] as Entidades.Operador;
                // Call for asunto distribution in batch
                await logAsunto.SentBatchAsuntoToOperators(currBackOffice, lstToDistribute);
                // Update balance with asuntos sented
                lstToDistribute.ForEach(asuntoToDeliver => currentBalanceFromOperators.Increment(asuntoToDeliver));
                currentBalanceFromOperators.UpdateAverage();
                // Refresh report of caller method
                (Owner as frmBackoffice).RefreshReportBalanceCurrentDay();
                Util.MsgBox.Error("Completado correctamente");
            }
            catch (Exception ex) {
                Except.Throw(ex);
            }
        }

        private void btnCancelLoad_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void cboOperatorToLoad_Loaded(object sender, RoutedEventArgs e)
        {
            // Get Sender Object and set in combo
            ComboBox operatorCombo = sender as ComboBox;
            // Save itemsource to combobox
            operatorCombo.ItemsSource = lstOperatorForAssign;
            // Get asunto related to combo
            Entidades.Asunto asuntoRelated = getAsuntoByCombo(operatorCombo);
            if(asuntoRelated.Oper == null) {
                // Configure selected index to first
                operatorCombo.SelectedIndex = 0;
            } else {
                // If the asunto have already selected operator
                foreach (var operInList in lstOperatorForAssign) {
                    if (operInList.Operator != null && operInList.Equals(asuntoRelated.Oper))
                        operatorCombo.SelectedItem = operInList;
                }
            }
            // Loads Selection Changed
            operatorCombo.SelectionChanged += cboOperatorToLoad_SelectionChanged;
        }

        private void cboOperatorToLoad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obtenemos el valor del combo
            ComboBox operatorCombo = sender as ComboBox;
            // Gets current value of combo and cast to operator
            OperatorAsuntoDistribute oadSelected = operatorCombo.SelectedItem as OperatorAsuntoDistribute;
            // Gets selected file of dataGrid
            Entidades.Asunto selectedAsunto = dgAsuntosWithoutAssignation.SelectedItem as Entidades.Asunto;
            if(oadSelected.Operator != null) {
                // Save operator to selected
                selectedAsunto.Oper = oadSelected.Operator;
            }
            else {
                // Unselect operator
                selectedAsunto.Oper = null;
            }
        }

        private void CheckLoadedUsersInAsunto()
        {
            // Iterates over all asuntos
            foreach (var asunto in lstAsuntoToAssign) {
                if (asunto.Oper == null) {
                    throw new Exception(string.Format("El asunto {0} no se le ha asignado operador.", asunto.Numero));
                }
            }
        }

        private void isOneOperatorLoaded()
        {
            // Iterates over all asuntos assigned
            foreach (var asunto in lstAsuntoToAssign) {
                if (asunto.Oper != null) return;
            }
            throw new Exception("Para enviar solo los asuntos seleccionados debe haber al menos un operador cargado.");
        }

        private void chkSentSelectedOnly_Checked(object sender, RoutedEventArgs e)
        {
             bllSelectedOnly = true;
        }

        private void chkSentSelectedOnly_Unchecked(object sender, RoutedEventArgs e)
        {
            bllSelectedOnly = false;
        }

        private void chkAutomaticAssignment_Checked(object sender, RoutedEventArgs e)
        {
            try {
                // Nullfies itemsource of datagrid
                dgAsuntosWithoutAssignation.ItemsSource = null;
                // Process simulation of automatic assignment
                currentBalanceFromOperators.GetAutomaticAsuntoAssignationByAverageHour(lstAsuntoToAssign.ToList(), lstOperatorForAssign.Select(oad => oad.Operator).Where(oper => oper != null).ToList());
                // Refresh list
                dgAsuntosWithoutAssignation.ItemsSource = lstAsuntoToAssign;

            } catch (Exception ex) {
                Except.Throw(ex);                
            }
        }

        private void chkAutomaticAssignment_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        
        private Entidades.Asunto getAsuntoByCombo(ComboBox comboSource)
        {
            // Get parent DataGridRow 
            DataGridRow dgrSelected = FindParent<DataGridRow>(comboSource);
            int indexOfRow = dgrSelected.GetIndex();
            return dgAsuntosWithoutAssignation.Items.GetItemAt(indexOfRow) as Entidades.Asunto;
        }

        private Parent FindParent<Parent>(DependencyObject child) where Parent : DependencyObject
        {
            DependencyObject parentObject = child;
            while (!((parentObject is System.Windows.Media.Visual) || (parentObject is System.Windows.Media.Media3D.Visual3D))) {
                if(parentObject is Parent || parentObject == null) {
                    return parentObject as Parent;
                } else {
                    parentObject = (parentObject as FrameworkContentElement).Parent;
                }
            }
            parentObject = VisualTreeHelper.GetParent(parentObject);
            if(parentObject is Parent || parentObject == null) {
                return parentObject as Parent;
            } else {
                return FindParent<Parent>(parentObject);
            }
        }

        /// <summary>
        /// Configures event on closing Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // When windows is closing set all operator to null
            lstAsuntoToAssign.ToList().ForEach(asunto => asunto.Oper = null);
        }
    }
}
