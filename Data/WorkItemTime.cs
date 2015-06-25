using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsHistorical.Data
{
    [DebuggerDisplay("{TrackDay} - {WorkItemID} - {CompletedWork}")]
    public class WorkItemTime
    {
        /// <summary>
        /// Initializes a new instance of WorkItemTime
        /// </summary>
        /// <param name="trackDay"></param>
        private WorkItemTime(DateTime trackDay)
        {
            this.TrackDay = trackDay;
        }

        /// <summary>
        /// Initializes a new instance of WorkItemTime base on day associated to these values
        /// and based on values included in specified work item.
        /// </summary>
        /// <param name="trackDay"></param>
        /// <param name="workItem"></param>
        public WorkItemTime(DateTime trackDay, WorkItem workItem) : this(trackDay)
        {
            this.WorkItemID = workItem.Id;
            this.Title = workItem.Title;
            this.Type = workItem.Type.Name;
            //this.AssignedTo = workItem.ChangedBy;
            this.AssignedTo = workItem.Fields.Contains("Assigned To") ? Convert.ToString(workItem.Fields["Assigned To"].Value) : String.Empty;
            this.OriginalEstimate = workItem.Fields.Contains("Original Estimate") ? Convert.ToDouble(workItem.Fields["Original Estimate"].Value) : 0;
            this.RemainingWork = workItem.Fields.Contains("Remaining Work") ? Convert.ToDouble(workItem.Fields["Remaining Work"].Value) : 0;
            this.CompletedWork = workItem.Fields.Contains("Completed Work") ? Convert.ToDouble(workItem.Fields["Completed Work"].Value) : 0;
        }

        /// <summary>
        /// Initializes a new instance of WorkItemTime base on day associated to these values
        /// and based on values included in specified work item.
        /// </summary>
        /// <param name="trackDay"></param>
        /// <param name="item"></param>
        public WorkItemTime(DateTime trackDay, WorkItemTime item) : this(trackDay)
        {
            this.WorkItemID = item.WorkItemID;
            this.Title = item.Title;
            this.Type = item.Type;
            this.AssignedTo = item.AssignedTo;
            this.OriginalEstimate = 0;
            this.RemainingWork = 0;
            this.CompletedWork = 0;
        }

        public DateTime TrackDay { get; set; }

        public int WorkItemID { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string AssignedTo { get; set; }

        public double OriginalEstimate { get; set; }

        public double CompletedWork { get; set; }

        public double RemainingWork { get; set; }

    }
}
