using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.TeamFoundation.VersionControl.Client;
using TaskTimeTracker.ClientToolsReference;
using TaskTimeTracker.Tracker.TaskHistory;

namespace TaskTimeTracker.Controls
{
    /// <summary>
    /// Interaction logic for TaskControl.xaml
    /// </summary>
    public partial class TaskControl : UserControl
    {
		public TaskHistory TaskHistory { get; set; }

        public TaskControl()
        {
            InitializeComponent();
			historyControl.Visibility = Visibility.Collapsed;
		}

		public void SetTaskHistory(TaskHistory taskHistory)
		{
			TaskHistory = taskHistory;
			TaskName.Content = TaskHistory.Summary;
        }
		

		public void UpdateHistory(HistoryItem taskHistory)
		{
			this.historyControl.UpdateHistory(taskHistory);
		}

		public void OnCollapse(object sender, EventArgs e)
		{
			if (historyControl.numberOfItems > 0)
			{
                Visibility visibility = historyControl.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                HistoryControlVisibility(visibility);
            }
		}

        public void HistoryControlVisibility(Visibility visibility)
        {
            historyControl.Visibility = visibility;
        }

        internal void UpdateWithShelve(Shelveset shelve, HistoryItem startHistoryItem)
        {
            historyControl.UpdateWithShelve(shelve.CreationDate, startHistoryItem.DateTime);
        }

		internal void HideIcons()
		{
			historyControl.HideIcons();
		}
	}
}
