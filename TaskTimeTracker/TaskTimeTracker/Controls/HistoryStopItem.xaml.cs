using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskTimeTracker.Controls
{
	/// <summary>
	/// Interaction logic for HistoryStopItem.xaml
	/// </summary>
	public partial class HistoryStopItem : UserControl
	{
		public DateTime DateTime { get; set; }

		public TimeSpan Time { get; set; }

		public HistoryStopItem(Tracker.TaskHistory.HistoryItem history)
		{
			InitializeComponent();
			DateTime = history.DateTime;

			lblStopDate.Content = history.DateTime.ToString(Configuration.Configuration.DATETIME_FORMAT);
		}
	}
}
