using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TaskTimeTracker.Database;
using TaskTimeTracker.Notifications;
using TaskTimeTracker.Tracker.TaskHistory;

namespace TaskTimeTracker.Controls
{
	public interface IRowControlListener
	{
		void OnPlay(RowControl rowControl);
		void CollapseAll();
		void Delete(RowControl rowControl);
	}

	public enum TimerType
	{
		Normal,
		Free
	}

	/// <summary>
	/// Interaction logic for RowControl2.xaml
	/// </summary>
	public partial class RowControl : System.Windows.Controls.UserControl, IDisposable
	{
		public int Id => TaskHistory.Id;
		public double ExpectedTime => TaskHistory.ExpectedTime;
		Timer Timer;
		TaskHistory TaskHistory;
		List<IRowControlListener> m_Listeners;
		public TimerType m_TimerType;
		public bool IsPlaying { get; set; }

		private bool m_HasNotifyExpectedTime;

		public TimerType TimerType
		{
			get
			{
				return m_TimerType;
			}
			set
			{
				if (value == TimerType.Free)
				{
					lblId.Visibility = Visibility.Collapsed;
					deleteControl.Visibility = Visibility.Visible;
				}
				else
				{
					lblId.Visibility = Visibility.Visible;
					deleteControl.Visibility = Visibility.Collapsed;
				}
				TaskHistory.TimerType = value;
				m_TimerType = value;
			}
		}

		public RowControl()
		{
			try
			{
				SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
			}
			catch (Exception)
			{
				// silent error
			}

			Timer = new Timer();
			InitializeComponent();
			deleteControl.MouseDown += DeleteControl_MouseDown;
			popupDelete.popup.Closed += OnPopupDeleteClosed;
			m_Listeners = new List<IRowControlListener>();
		}

		private void OnPopupDeleteClosed(object sender, EventArgs e)
		{
			if (popupDelete.ShouldDelete)
			{
				foreach (var listener in m_Listeners)
				{
					listener.Delete(this);
				}
			}
		}

		private void DeleteControl_MouseDown(object sender, MouseButtonEventArgs e)
		{
			popupDelete.popup.IsOpen = true;
		}

		public RowControl(ClientToolsReference.Task task) : this()
		{
			TaskHistory taskHistory = new TaskHistory(task);
			SetHistory(taskHistory);
			lblId.Content = Id.ToString();
		}

		public RowControl(TaskHistory taskHistory) : this()
		{
			SetHistory(taskHistory);
			lblId.Content = Id.ToString();
			lblTime.Content = taskControl.historyControl.GetFormatedTimeStamp(taskControl.historyControl.TotalTime);
			TimerType = taskHistory.TimerType;
		}

		public void SetHistory(TaskHistory taskHistory)
		{
			TaskHistory = taskHistory;

			foreach (var historyItem in taskHistory.Histories)
			{
				taskControl.historyControl.UpdateHistory(historyItem);
			}

			if (taskHistory.Histories.Count > 0)
				taskControl.HideIcons();

			taskControl.SetTaskHistory(taskHistory);
		}

		public void AddListener(IRowControlListener listener)
		{
			if (listener != null)
				m_Listeners.Add(listener);
		}

		public void OnClick(Object sender, EventArgs e)
		{
			imagePlay.Visibility = !IsPlaying ? Visibility.Collapsed : Visibility.Visible;
			imageStop.Visibility = IsPlaying ? Visibility.Collapsed : Visibility.Visible;

			IsPlaying = !IsPlaying;

			HistoryItem historyItem = new HistoryItem();

			if (IsPlaying)
			{
				historyItem.Type = HistoryType.Start;
				historyItem.DateTime = DateTime.Now;

				Timer.Interval = 50;
				Timer.Tick += TimerTick;
				Timer.Start();
				var color = ColorConverter.ConvertFromString("#d9ae52");
				TimeRectangle.Fill = new SolidColorBrush((Color)color);
				ButonRect.Fill = new SolidColorBrush((Color)color);
			}
			else
			{
				var last = TaskHistory.Histories.Last(h => h.Type == HistoryType.Start);
				Timer.Stop();
				historyItem.Type = HistoryType.Stop;

				historyItem.DateTime = DateTime.Now;
				TimeRectangle.Fill = Brushes.DarkGray;
				ButonRect.Fill = Brushes.DarkGray;

				FindShelves(historyItem, last);

				taskControl.HideIcons();
			}

			TaskHistory.Histories.Add(historyItem);
			taskControl.UpdateHistory(historyItem);

			Save();

			ToggleListenersVisibility();
		}

		public void Save()
		{
			//if (TimerType != TimerType.Free)
				DatabaseAccess.Save(taskControl.TaskHistory);
		}

		internal void Collapse()
		{
			taskControl.HistoryControlVisibility(Visibility.Collapsed);
		}

		private void FindShelves(HistoryItem historyItem, HistoryItem last)
		{

#if DEBUG
			return;
#endif
			try
			{
				TfsTeamProjectCollection pc = new TfsTeamProjectCollection(new Uri(@"http://tfs.be.bvdep.net:8080/tfs/Defaultcollection"));
				VersionControlServer controlServer = pc.GetService<VersionControlServer>();
				var shelveSets = controlServer.QueryShelvesets("", Environment.UserName);

				List<Shelveset> listOfShelves = new List<Shelveset>();

				foreach (var shelve in shelveSets)
					if (shelve.CreationDate > last.DateTime && shelve.CreationDate < historyItem.DateTime && shelve.Name.Contains(TaskHistory.Id.ToString()))
						listOfShelves.Add(shelve);

				foreach (var shelve in listOfShelves)
				{
					taskControl.UpdateWithShelve(shelve, last);
					HistoryItem hShelve = new HistoryItem(HistoryType.Shelve);
					hShelve.DateTime = shelve.CreationDate;
					TaskHistory.Histories.Add(hShelve);
				}
			}
			catch (Exception)
			{

			}
		}

		public void Stop(Visibility visibility = Visibility.Collapsed)
		{
			if (IsPlaying)
			{
				OnClick(null, null);
			}
			taskControl.HistoryControlVisibility(visibility);
		}

		public void ToggleListenersVisibility()
		{
			foreach (IRowControlListener listener in m_Listeners)
			{
				if (IsPlaying)
					listener.OnPlay(this);

				listener.CollapseAll();
			}
			taskControl.HistoryControlVisibility(Visibility.Visible);
		}

		private void TimerTick(object sender, EventArgs e)
		{
			var time = taskControl.historyControl.CalculateTotalTime();

			lblTime.Content = taskControl.historyControl.RenderTime(time);

			if (time.TotalHours > ExpectedTime && ExpectedTime > 0 && !m_HasNotifyExpectedTime)
			{
				var notif = BaloonNotification.ShowNotificationIcon($"The timer '{Id}' has exceeded the expected time of {ExpectedTime} hours.");
				notif.Click += OnNotificationClicked;
				m_HasNotifyExpectedTime = true;
			}
		}

		private void OnNotificationClicked(object sender, EventArgs e)
		{
			System.Windows.Application.Current.MainWindow.Show();
		}

		void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			switch (e.Reason)
			{
				case SessionSwitchReason.SessionLogon:
				case SessionSwitchReason.SessionUnlock:
					break;

				case SessionSwitchReason.SessionLock:
				case SessionSwitchReason.SessionLogoff:
					Stop(Visibility.Collapsed);
					break;
			}
		}
		public void Dispose()
		{
			// windows log off
			try
			{
				SystemEvents.SessionSwitch -= new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
			}
			catch (Exception)
			{
				// silent error
			}
		}
	}
}
