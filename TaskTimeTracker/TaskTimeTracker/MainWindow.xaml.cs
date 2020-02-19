using LiteDB;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using TaskTimeTracker.ClientToolsReference;
using TaskTimeTracker.Controls;
using TaskTimeTracker.ServiceConsumer;
using TaskTimeTracker.Tracker.TaskHistory;
using Microsoft.Win32;
using TaskTimeTracker.Notifications;
using TaskTimeTracker.Configuration;
using TaskTimeTracker.Database;
using System.Windows.Input;
using System.Collections.Generic;
using System.Configuration;

namespace TaskTimeTracker
{

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IRowControlListener
	{
		Timer Timer;
		int row_number = 1;

		public MainWindow()
		{
			Title = Constants.TITLE;
			InitializeComponent();

			InitializeEvents();
			
			var a = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			a.AppSettings.Settings.Remove("UserAcronym");
			a.AppSettings.Settings.Add("UserAcronym", "lala");
			//a.Save();

			try
			{
				SystemEvents.SessionSwitch += new SessionSwitchEventHandler(OnSessionSwitch);
			}
			catch (Exception)
			{
				// silent error
			}
			var user = ConfigurationManager.AppSettings[Configuration.Configuration.USER_ACRONYM_KEY];
			if (string.IsNullOrEmpty(user))
			{
				Timer timer = new Timer();
				timer.Interval = 100;
				timer.Tick += Timer_AcronymPopup;
				timer.Start();
			}
			else
				Initialize();
		}

		private void Initialize()
		{
#if DEBUG
			InitialiseDebug();
#endif
#if !DEBUG

			FillRowFromTask(TaskStatus.InProgress);
			FillRowFromTask(TaskStatus.InReview);
			FillRowFromTask(TaskStatus.Feedback);
			FillRowFromTask(TaskStatus.Suspended);
			FillRowFromTask(TaskStatus.Assigned);
#endif
			var freeTimers = DatabaseAccess.GetAllFree();

			foreach (var freeTimer in freeTimers)
			{
				AddFreeTimerRow(freeTimer);
			}
			LaunchTimer();
		}

		/// <summary>
		/// Timer to avoid the bug of the popup in the left corner because the window is not initialized yet
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Timer_AcronymPopup(object sender, EventArgs e)
		{
			var user = ConfigurationManager.AppSettings[Configuration.Configuration.USER_ACRONYM_KEY];

			if (string.IsNullOrEmpty(user))
				userAcronymPopup.popup.IsOpen = true;

			Timer timer = sender as Timer;
			timer.Stop();
		}

		private void InitializeEvents()
		{
			freeTimerPlus.MouseUp += OnFreeTimerPlusClick;
			rctHeaderTask.MouseUp += OnHeaderTaskClick;
			lblHeaderTask.MouseUp += OnHeaderTaskClick;
			freeTimerPopup.popup.Closed += OnPopupOkCLick;
			userAcronymPopup.popup.Closed += OnPopupUserAcronymClosed;
			settingsScreen.btSave.Click += OnSettingsSaved;
		}

		private void OnSettingsSaved(object sender, RoutedEventArgs e)
		{
			Initialize();
		}

		private void OnPopupUserAcronymClosed(object sender, EventArgs e)
		{
			Configuration.Configuration.SetUserAcronym(userAcronymPopup.tbValue.Text);

			Initialize();
		}

		private void OnHeaderTaskClick(object sender, MouseButtonEventArgs e)
		{
			foreach (var item in TasksGrid.Children.OfType<RowControl>())
				item.Collapse();
			foreach (var item in FreeTimers.Children.OfType<RowControl>())
				item.Collapse();
		}

		private void OnFreeTimerPlusClick(object sender, MouseButtonEventArgs e)
		{
			if (FreeTimers.Children.Count < 5)
			{
				freeTimerPopup.tbTaskName.Text = "";
				freeTimerPopup.popup.IsOpen = true;
				freeTimerPopup.popup.Focus();
				if (FreeTimers.Children.Count == 4)
					freeTimerPlus.Visibility = Visibility.Collapsed;
			}
		}

		private void AddFreeTimerRow(int id)
		{
			TaskHistory th = new TaskHistory();
			th.ExpectedTime = 10000000;
			th.Id = id;
			th.Summary = freeTimerPopup.tbTaskName.Text;
			RowControl rowControl = new RowControl(th);
			rowControl.TimerType = TimerType.Free;

			rowControl.AddListener(this);
			FreeTimers.Children.Add(rowControl);
		}
		private void AddFreeTimerRow(TaskHistory th)
		{
			RowControl rowControl = new RowControl(th);

			rowControl.AddListener(this);
			FreeTimers.Children.Add(rowControl);
		}

		private void InitialiseDebug()
		{
			var tasks = DatabaseAccess.GetAll();
			int i = 0;
			foreach (var task in tasks)
			{
				if (task.TimerType == TimerType.Free)
					continue;

				var t = new Task();
				t.Id = task.Id;
				t.Summary = task.Summary;
				t.ExpectedTime = task.ExpectedTime;
				CreateRow(t);

				if (i++ == 5)
					break;
			}
		}

		private void LaunchTimer()
		{
			Timer = new Timer();
			Timer.Interval = 30000;

			Timer.Tick += UpdateTaskList;
			Timer.Tick += Save;
			Timer.Start();
		}

		private void Save(object sender, EventArgs e)
		{
			foreach (var rowControl in TasksGrid.Children.OfType<RowControl>())
			{
				rowControl.Save();
			}

			foreach (var rowControl in FreeTimers.Children.OfType<RowControl>())
			{
				rowControl.Save();
			}
		}

		private void UpdateTaskList(object sender, EventArgs e)
		{
			FillRowFromTask(TaskStatus.InProgress, true);
			FillRowFromTask(TaskStatus.InReview, true);
			FillRowFromTask(TaskStatus.Feedback, true);
			FillRowFromTask(TaskStatus.Suspended, true);
			FillRowFromTask(TaskStatus.Assigned, true);

			RemoveResolvedTasks();
		}

		private void RemoveResolvedTasks()
		{
			foreach (var rowControl in TasksGrid.Children.OfType<RowControl>().ToList())
			{
				try
				{
					Task task = DevNet_ClientTools.GetTask(rowControl.Id);
					if (task.Status == TaskStatus.Resolved)
						TasksGrid.Children.Remove(rowControl);
				}
				catch (Exception)
				{
					
				}
			}
		}

		private void FillRowFromTask(TaskStatus taskStatus, bool shoowNotif = false)
		{
			Task[] tasks = DevNet_ClientTools.GetTasks(taskStatus);
			foreach (var task in tasks)
			{
				CreateRow(task, shoowNotif);
			}
		}

		private void CreateRow(Task task, bool showNotif = false)
		{
			if (!TaskGridContains(task))
			{
				var taskHistory = DatabaseAccess.Get(task.Id);

				RowControl rowControl = null;

				if (taskHistory != null)
				{
					taskHistory.ExpectedTime = task.ExpectedTime; // override to make sure there is no bug
					rowControl = new RowControl(taskHistory);
				}
				else
				{
					rowControl = new RowControl(task);
				}

				rowControl.AddListener(this);
				TasksGrid.Children.Add(rowControl);
				Grid.SetColumn(rowControl, 0);
				Grid.SetRow(rowControl, row_number);
				AddRowTasksGrid();
				row_number++;

				if (showNotif)
					BaloonNotification.ShowNotificationIcon($"You Received a new Task: '{task.Summary}'");
			}
		}

		private bool TaskGridContains(Task task)
		{
			foreach (var row in TasksGrid.Children.OfType<RowControl>())
				if (task.Id == row.Id)
					return true;
			return false;
		}

		private void AddRowTasksGrid()
		{
			var rd = new RowDefinition();
			var gridHeight = new GridLength(30, GridUnitType.Auto);
			rd.Height = gridHeight;
			TasksGrid.RowDefinitions.Add(rd);
		}


		public void OnPlay(RowControl rowControl)
		{
			foreach (UIElement control in TasksGrid.Children)
			{
				RowControl row = control as RowControl;

				if (row != null && row != rowControl)
				{
					row.Stop();
				}
			}
			foreach (UIElement control in FreeTimers.Children)
			{
				RowControl row = control as RowControl;

				if (row != null && row != rowControl)
				{
					row.Stop();
				}
			}
		}

		public void CollapseAll()
		{
			foreach (UIElement control in TasksGrid.Children)
			{
				RowControl row = control as RowControl;
				if (row != null)
					row.Collapse();
			}

			foreach (UIElement control in FreeTimers.Children)
			{
				RowControl row = control as RowControl;
				if (row != null)
					row.Collapse();
			}
		}

		void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			switch (e.Reason)
			{
				case SessionSwitchReason.SessionLogon:
				case SessionSwitchReason.SessionUnlock:
					BaloonNotification.ShowNotificationIcon("All Timer are stopped");
					break;

				case SessionSwitchReason.SessionLock:
				case SessionSwitchReason.SessionLogoff:

					break;
			}
		}

		public void On_Close(object sender, EventArgs e)
		{
			OnPlay(null);
		}

		public void Dispose()
		{
			// windows lock
			try
			{
				SystemEvents.SessionSwitch -= new SessionSwitchEventHandler(OnSessionSwitch);
			}
			catch (Exception)
			{
				// silent error
			}
		}

		public void OnPopupOkCLick(object sender, EventArgs e)
		{
			var last = FreeTimers.Children.Cast<RowControl>().LastOrDefault();
			AddFreeTimerRow(last == null ? 1 : last.Id + 1);
		}

		public void OnTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			System.Windows.Controls.TabControl tb = sender as System.Windows.Controls.TabControl;
			var tabItem = tb.SelectedItem as TabItem;
			var header = tabItem.Name as string;
			switch (header)
			{
				case "tabItemSettings":
					settingsScreen.Refresh();
					break;

				default:
					break;
			}
		}

		public void Delete(RowControl rowControl)
		{
			FreeTimers.Children.Remove(rowControl);

			DatabaseAccess.DeleteFreeTimer(rowControl.Id);
		}
	}
	
}
