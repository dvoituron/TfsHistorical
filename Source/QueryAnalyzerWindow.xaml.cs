using System;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Windows;
using TfsHistorical.Data;
using Microsoft.Win32;
using System.Data;
using System.Text;

namespace TfsHistorical
{
    /// <summary>
    /// Interaction logic for QueryAnalyzerWindow.xaml
    /// </summary>
    public partial class QueryAnalyzerWindow : Window
    {
        private WorkItemStore _tfsStore = null;
        private string _fileName = string.Empty;

        /// <summary>
        /// Initializes a new instance of this window
        /// </summary>
        /// <param name="tfsproject">Reference to a selected project</param>
        public QueryAnalyzerWindow(Data.TfsProject tfsProject)
        {
            InitializeComponent();

            string path = tfsProject.Iterations.Items[0].Path;

            StringBuilder sql = new StringBuilder();
            sql.AppendLine(" SELECT [System.Id], ");
            sql.AppendLine("        [System.Title], ");
            sql.AppendLine("        [Microsoft.VSTS.Scheduling.CompletedWork], ");
            sql.AppendLine("        [Microsoft.VSTS.Scheduling.OriginalEstimate], ");
            sql.AppendLine("        [Microsoft.VSTS.Scheduling.RemainingWork] ");
            sql.AppendLine("  FROM WorkItems");
            sql.AppendLine(" WHERE  [System.IterationPath] UNDER '" + path + "' ");
            
            txtQuery.Text = sql.ToString();

            _tfsStore = (WorkItemStore)tfsProject.ProjectCollection.GetService(typeof(WorkItemStore));

        }

        /// <summary>
        /// Close the current window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Run the query
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Execute the query
                var wiCollection = _tfsStore.Query(txtQuery.Text);
                DataTable data = wiCollection.ToDataTable();
                dgvGrid.ItemsSource = data.DefaultView;
                txtCount.Text = String.Format("{0} record(s) found.", data.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
                       
        }

        /// <summary>
        /// Load the query from a text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".sql";
            dlg.Filter = "Work Items Query File (.sql)|*.sql";
            if (dlg.ShowDialog() == true)
            {
                _fileName = dlg.FileName;
                txtQuery.Text = System.IO.File.ReadAllText(_fileName);
            }
            
        }

        /// <summary>
        /// Catch F5
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F5)
            {
                btnPlay_Click(sender, null);
            }
        }

        /// <summary>
        /// Save the query
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.DefaultExt = ".sql";
                dlg.Filter = "Work Items Query File (.sql)|*.sql";
                if (dlg.ShowDialog() == true)
                {
                    _fileName = dlg.FileName;
                }
            }

            System.IO.File.WriteAllText(_fileName, txtQuery.Text);

        }
    }
}
