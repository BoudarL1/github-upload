using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTimeTracker.Tracker.TaskHistory
{
	public enum HistoryType
	{
		Start,
		Stop,
		Shelve
	}

	public class HistoryItem
    {
		public HistoryType Type { get; set; }
        public DateTime DateTime { get; set; }
        public int ExtraTime { get; set; }
        public string Description { get; set; }

		public HistoryItem()
		{

		}

		public HistoryItem(HistoryType type)
		{
			this.Type = type;
		}
	}
}
