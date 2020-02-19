using System;
using System.Collections.Generic;
using System.Globalization;
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
using Microsoft.TeamFoundation.VersionControl.Client;
using TaskTimeTracker.Tracker.TaskHistory;

namespace TaskTimeTracker.Controls
{
    /// <summary>
    /// Interaction logic for HistoryControl.xaml
    /// </summary>
    public partial class HistoryControl : UserControl
    {
        public List<HistoryItem> Histories;
        public TimeSpan TotalTime = new TimeSpan();
        public int numberOfItems;
        public HistoryControl()
        {
            InitializeComponent();
            Histories = new List<HistoryItem>();
        }
		

        public void UpdateHistory(HistoryItem history)
        {
            if (history == null)
                return;

			Histories.Add(history);

			if (history.Type == HistoryType.Start)
            {
                HistoryStartItem hstart = new HistoryStartItem(this, history);
                hstart.Time = TotalTime;
                hstart.lblStartTime.Content = GetFormatedTimeStamp(TotalTime);
				if (history.ExtraTime > 0)
					hstart.SetLabelValue(history.ExtraTime);

                AddRow();
                MainGrid.Children.Add(hstart);
                Grid.SetRow(hstart, numberOfItems);
            }
            else if (history.Type == HistoryType.Stop)
            {
				var last = Histories.Last(h => h.Type == HistoryType.Start);
                TotalTime += history.DateTime - last.DateTime + TimeSpan.FromMinutes(last.ExtraTime);

                HistoryStopItem hstop = new HistoryStopItem(history);
                hstop.Time = TotalTime;
                hstop.lblStopTime.Content = GetFormatedTimeStamp(TotalTime);

                AddRow();
                MainGrid.Children.Add(hstop);
                Grid.SetRow(hstop, numberOfItems);
            }
			else
			{
				var last = Histories.Last(h => h.Type == HistoryType.Start);

				TimeSpan time = TotalTime + (history.DateTime - last.DateTime);
				HistoryShelveControl hsc = new HistoryShelveControl(history.DateTime);
				hsc.lblShelveTime.Content = GetFormatedTimeStamp(time);

				AddRow();
				MainGrid.Children.Add(hsc);
				Grid.SetRow(hsc, numberOfItems);
			}
            numberOfItems++;
        }

		internal void HideIcons()
		{
			foreach (var hstart in MainGrid.Children.OfType<HistoryStartItem>())
			{
				hstart.plusExtraTime.Visibility = Visibility.Hidden;
			}
		}

		internal void UpdateWithShelve(DateTime shelveCreationDate, DateTime startDateTime)
		{
			TimeSpan time = TotalTime + (shelveCreationDate - startDateTime);
			HistoryShelveControl hsc = new HistoryShelveControl(shelveCreationDate);
			hsc.lblShelveTime.Content = GetFormatedTimeStamp(time);
			
			AddRow();
			MainGrid.Children.Add(hsc);
			Grid.SetRow(hsc, numberOfItems);
			numberOfItems++;
		}


		public TimeSpan CalculateTotalTime()
        {
            return TotalTime + (DateTime.Now - Histories.Last().DateTime) + TimeSpan.FromMinutes(Histories.Last().ExtraTime);
        }

		public string RenderTime(TimeSpan time)
		{
			return GetFormatedTimeStamp(time);
		}

        public string GetFormatedTimeStamp(TimeSpan timeSpan)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }


        private void AddRow()
        {
            var rd = new RowDefinition();
            var gridHeight = new GridLength(30);
            rd.Height = gridHeight;
            MainGrid.RowDefinitions.Add(rd);
        }


	}
}
