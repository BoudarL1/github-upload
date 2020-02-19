using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskTimeTracker.ClientToolsReference;
using TaskTimeTracker.Controls;

namespace TaskTimeTracker.Tracker.TaskHistory
{
    public class TaskHistory
    {
		public int Id { get; set; }
		public string Summary { get; set; }
		public double ExpectedTime { get; set; }
        public List<HistoryItem> Histories { get; set; }
		public TimerType TimerType { get; set; }

		public TaskHistory()
		{
			Histories = new List<HistoryItem>();
		}

		public TaskHistory(Task task):this()
		{
			Id = task.Id;
			Summary = task.Summary;
			ExpectedTime = task.ExpectedTime;
		}
    }
}
