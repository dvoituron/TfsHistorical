using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.TeamFoundation.Framework.Client.Catalog.Objects;
using Microsoft.TeamFoundation.Server;
using System.Xml;

namespace TfsHistorical
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Data.TfsProject _tfsProject = new Data.TfsProject();

        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            dtpStart.SelectedDate = DateTime.Today.AddDays(-1);
            dtpEnd.SelectedDate = DateTime.Today;
            chkChangesOnly.IsChecked = true;

            this.Title += String.Format(" v.{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(4));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;

            _tfsProject.ShowDialog();
            if (String.IsNullOrEmpty(_tfsProject.ProjectName))
                this.Close();

            cboIterations.ItemsSource = _tfsProject.Iterations.Items;
            cboIterations.SelectedIndex = 0;
            this.Cursor = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;

            // Get the work item service
            WorkItemStore store = (WorkItemStore)_tfsProject.ProjectCollection.GetService(typeof(WorkItemStore));
                
            DateTime startDate = dtpStart.SelectedDate == null ? DateTime.Today : dtpStart.SelectedDate.Value;
            DateTime endDate = dtpStart.SelectedDate == null ? DateTime.Today : dtpEnd.SelectedDate.Value;

            Data.IterationItem iteration = cboIterations.SelectedValue as Data.IterationItem;
            Data.IterationItem[] allIterations = cboIterations.ItemsSource as Data.IterationItem[];
            Data.WorkItemTimeCollection wi = new Data.WorkItemTimeCollection(store, allIterations[0].Path, startDate, endDate);

            dgvWorkItems.ItemsSource = wi.ToDataTable(chkChangesOnly.IsChecked == true).DefaultView;
            
            this.Cursor = null;
        }

        /// <summary>
        /// Display iteration Dates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboIterations_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Data.IterationItem iteration = cboIterations.SelectedValue as Data.IterationItem;
            if (iteration != null && iteration.StartDate.HasValue && iteration.EndDate.HasValue)
            {
                dtpStart.SelectedDate = iteration.StartDate;
                dtpEnd.SelectedDate = iteration.EndDate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            _tfsProject.ProjectCollection.Dispose();
        }

        /// <summary>
        /// Export data to the ClipBoard in HTML format
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            System.Data.DataView view = dgvWorkItems.ItemsSource as System.Data.DataView;

            if (view != null)
            {
                Data.Export export = new Data.Export(view.Table);
                export.CopyToClipBoard();
            }
            else
            {
                MessageBox.Show("No data found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Open the Query Analyser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            QueryAnalyzerWindow frmQuery = new QueryAnalyzerWindow(_tfsProject);
            frmQuery.Show();
        }

    }
}
