using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskTimeTracker.Configuration;

namespace TaskTimeTracker.Notifications
{
	static class BaloonNotification
	{
		public static NotifyIcon ShowNotificationIcon(string message)
		{
			NotifyIcon notifyIcon = new NotifyIcon();

			notifyIcon.Icon = new System.Drawing.Icon(@"Controls/Images/clock-2-32.ico");
			notifyIcon.Visible = true;
			notifyIcon.BalloonTipText = message;
			notifyIcon.BalloonTipTitle = Constants.TITLE;
			notifyIcon.ShowBalloonTip(7000);
			notifyIcon.BalloonTipClosed += OnNotificationClosed;
			notifyIcon.BalloonTipClicked += OnNotificationClosed;
			notifyIcon.BalloonTipClicked += OnNotificationClicked;
			
			return notifyIcon;
		}

		private static void OnNotificationClicked(object sender, EventArgs e)
		{
			System.Windows.Application.Current.MainWindow.Activate();
		}

		private static void OnNotificationClosed(object sender, EventArgs e)
		{
			var ni = sender as NotifyIcon;
			ni.Dispose();
		}
	}
}
