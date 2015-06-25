using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsHistorical.Data
{
    public class WorkItemTimeCollection
    {
        private List<DateTime> _trackDates = new List<DateTime>();
        private ObservableCollection<WorkItemDay> _trackDays = new ObservableCollection<WorkItemDay>();

        /// <summary>
        /// Search all work items
        /// </summary>
        /// <param name="store"></param>
        /// <param name="iterationPath"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public WorkItemTimeCollection(WorkItemStore store, string iterationPath, DateTime startDate, DateTime endDate)
        {
            this.IterationPath = iterationPath;
            this.Data = new ObservableCollection<WorkItemTime>();

            // Sets a list of dates to compute
            _trackDates.Add(startDate.Date);
            for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                _trackDates.Add(date.AddHours(23).AddMinutes(59));
            }

            // Gets all work items for each dates
            foreach (DateTime asOfDate in _trackDates)
            {
                // Execute the query
                var wiCollection = store.Query(this.GetQueryString(asOfDate));

                // Iterate through all work items
                foreach (WorkItem wi in wiCollection)
                {                    
                    WorkItemTime time = new WorkItemTime(asOfDate, wi);
                    this.Data.Add(time);
                }
            }
        }

        /// <summary>
        /// Gets the Iteration path associated to workitems
        /// </summary>
        public string IterationPath { get; private set; }

        /// <summary>
        /// Gets work items sorted by days
        /// </summary>
        public ObservableCollection<WorkItemDay> TrackDays
        {
            get
            {
                if (_trackDays.Count <= 0)
                {
                    foreach (DateTime date in _trackDates)
                    {
                        _trackDays.Add(this.GetItemsForDay(date));
                    }
                }

                return _trackDays;
            }
        }

        /// <summary>
        /// Gets a list with all work items
        /// </summary>
        public ObservableCollection<WorkItemTime> Data { get; private set; }

        /// <summary>
        /// Returns a DataTable with work items
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            return this.ToDataTable(false);
        }

        /// <summary>
        /// Returns a DataTable with work items
        /// </summary>
        /// <param name="clearEmptyRows"></param>
        /// <returns></returns>
        public DataTable ToDataTable(bool clearEmptyRows)
        {
            DataTable table = new DataTable();
            const string dayFormat = "dd/MM/yyyy";

            // Columns
            if (this.TrackDays.Count > 1)
            {
                table.Columns.Add(new DataColumn("WorkItemID", typeof(int)));
                table.Columns.Add(new DataColumn("Title", typeof(string)));
                table.Columns.Add(new DataColumn("Type", typeof(string)));
                table.Columns.Add(new DataColumn("AssignedTo", typeof(string)));

                for (int i = 1; i < this.TrackDays.Count; i++)
                {
                    table.Columns.Add(new DataColumn(this.TrackDays[i].TrackDay.ToString(dayFormat), typeof(string)));
                }
            }

            WorkItemDay lastDayTracked = this.TrackDays[this.TrackDays.Count - 1];

            // Data
            foreach (WorkItemTime item in lastDayTracked.Items)
            {
                DataRow row = table.NewRow();
                bool isDataEmpty = true;

                row["WorkItemID"] = item.WorkItemID;
                row["Title"] = item.Title;
                row["Type"] = item.Type;
                row["AssignedTo"] = item.AssignedTo;

                for (int i = 1; i < this.TrackDays.Count; i++)
                {
                    double previous = this.TrackDays[i - 1].Items.FirstOrDefault(w => w.WorkItemID == item.WorkItemID).CompletedWork;
                    double current = this.TrackDays[i].Items.FirstOrDefault(w => w.WorkItemID == item.WorkItemID).CompletedWork;

                    if (isDataEmpty && (current - previous) > 0)
                    {
                        isDataEmpty = false;
                    }

                    row[this.TrackDays[i].TrackDay.ToString(dayFormat)] = (current - previous) != 0 ? Convert.ToString(current - previous) : string.Empty;
                }

                if (!clearEmptyRows || clearEmptyRows && !isDataEmpty)
                {
                    table.Rows.Add(row);
                }

            }

            return table;
        }

        /// <summary>
        /// Gets a list with all work items for a specifi track day
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private WorkItemDay GetItemsForDay(DateTime day)
        {
            return new WorkItemDay(this.Data, day);
        }

        /// <summary>
        /// Returns the TFS Query (WIQL) to gets all workitems at the specified date and time.
        /// See http://msdn.microsoft.com/en-us/library/ms194971.aspx
        /// </summary>
        /// <param name="asOfDate"></param>
        /// <returns></returns>
        private string GetQueryString(DateTime asOfDate)
        {
            StringBuilder queryText = new StringBuilder();

            queryText.AppendLine(" SELECT [System.Id], ");
            queryText.AppendLine("        [System.Title], ");
            queryText.AppendLine("        [System.WorkItemType], ");
            queryText.AppendLine("        [System.ChangedBy], ");
            queryText.AppendLine("        [System.State], ");
            queryText.AppendLine("        [Microsoft.VSTS.Scheduling.CompletedWork], ");
            queryText.AppendLine("        [Microsoft.VSTS.Scheduling.OriginalEstimate], ");
            queryText.AppendLine("        [Microsoft.VSTS.Scheduling.RemainingWork] ");
            queryText.AppendLine("   FROM WorkItems ");
            queryText.AppendLine("  WHERE  [System.IterationPath] UNDER '" + this.IterationPath + "' ");
            queryText.AppendLine("    AND (   [Microsoft.VSTS.Scheduling.CompletedWork] != 0 ");
            queryText.AppendLine("         OR [Microsoft.VSTS.Scheduling.OriginalEstimate] != 0 ");
            queryText.AppendLine("         OR [Microsoft.VSTS.Scheduling.RemainingWork] != 0 ");
            queryText.AppendLine("        ) ");
            queryText.AppendLine("   ASOF '" + asOfDate.ToString("yyyy-MM-dd HH:mm") + "'");

            return queryText.ToString();
        }

    }
}
