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
using TaskTimeTracker.Controls.Popup;
using TaskTimeTracker.Converter;
using TaskTimeTracker.Tracker.TaskHistory;

namespace TaskTimeTracker.Controls
{

	/// <summary>
	/// Interaction logic for HistoryStartItem.xaml
	/// </summary>
	public partial class HistoryStartItem : UserControl
	{
		public DateTime DateTime { get; set; }

		public TimeSpan Time { get; set; }

        public HistoryItem History { get; set; }

        public HistoryControl ParentControl { get; set; }

        public HistoryStartItem(HistoryControl control, HistoryItem history)
		{
			InitializeComponent();

            History = history;
            ParentControl = control;
			plusExtraTime.MouseDown += OnImgAddExtraTime_MouseDown;
			plusDescription.MouseDown += OnPlusDescription_MouseDown;
			popupExtraTime.popup.Closed += OnPopupExtraTimeClosed;
			popupDescription.popup.Closed += OnPopupDescriptionClosed;
			DateTime = history.DateTime;
			lblDescription.Content = history.Description;

			lblStartDate.Content = history.DateTime.ToString(Configuration.Configuration.DATETIME_FORMAT);
		}

		private void OnPlusDescription_MouseDown(object sender, MouseButtonEventArgs e)
		{
			popupDescription.popup.IsOpen = true;
		}

		private void OnPopupDescriptionClosed(object sender, EventArgs e)
		{
			History.Description = popupDescription.tbValue.Text;
			lblDescription.Content = History.Description;
		}

		private void OnPopupExtraTimeClosed(object sender, EventArgs e)
		{
			int extraTime;
			if (Int32.TryParse(popupExtraTime.tbExtraTime.Text, out extraTime))
			{
				if (extraTime == 0)
				{
					lblExtraTime.Content = "";
				}
				else
				{
					SetLabelValue(extraTime);
					History.ExtraTime = extraTime;
				}
			}
		}

		public void SetLabelValue(int extraTime)
		{
			var value = extraTime.ToString();
			if (value != "")
				value += " (min)";
			lblExtraTime.Content = value;
		}

		private void OnImgAddExtraTime_MouseDown(object sender, MouseEventArgs e)
		{
			popupExtraTime.popup.IsOpen = true;
		}
	}
}
