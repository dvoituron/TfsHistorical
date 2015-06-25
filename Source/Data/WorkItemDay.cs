using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TfsHistorical.Data
{
    public class WorkItemDay
    {
        /// <summary>
        /// Initializes a new instance of a list of work items for a specific track day
        /// </summary>
        /// <param name="allItems"></param>
        /// <param name="trackDay"></param>
        internal WorkItemDay(ObservableCollection<WorkItemTime> allItems, DateTime trackDay)
        {
            this.TrackDay = trackDay;

            List<WorkItemTime> result = new List<WorkItemTime>();
            foreach (WorkItemTime item in allItems.Where(i => i.TrackDay == trackDay).OrderBy(i => i.WorkItemID))
            {
                result.Add(item);
            }

            foreach(var item in allItems.Where(i => !result.Exists(r => r.WorkItemID == i.WorkItemID)))
            {
                result.Add(new WorkItemTime(trackDay, item));
            }

            this.Items = new ObservableCollection<WorkItemTime>(result.ToArray());
        }

        /// <summary>
        /// Gets the track day
        /// </summary>
        public DateTime TrackDay { get; private set; }

        /// <summary>
        /// Gets all work items for the current track day
        /// </summary>
        public ObservableCollection<WorkItemTime> Items { get; set; }
    }
}
