using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTimeTracker.Tracker.TaskHistory;

namespace TaskTimeTracker.Converter
{
    public class TimeConverter
    {
        public static int ConvertToMinutes(HistoryItem history)
        {
            if (history == null || history.DateTime == DateTime.MinValue)
                return 0;

            TimeSpan startTicks = new TimeSpan(history.DateTime.Ticks);
            TimeSpan endTicks = new TimeSpan(history.DateTime.Ticks);

            int time = Convert.ToInt32(((endTicks.TotalSeconds - startTicks.TotalSeconds) / 60));
            if (history.ExtraTime > 0)
                time += history.ExtraTime;

            return time;
        }
    }
}
