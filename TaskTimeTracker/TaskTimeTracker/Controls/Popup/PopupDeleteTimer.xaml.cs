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

namespace TaskTimeTracker.Controls.Popup
{
	/// <summary>
	/// Interaction logic for PopupDeleteTimer.xaml
	/// </summary>
	public partial class PopupDeleteTimer : UserControl
	{
		public bool ShouldDelete = false;
		public PopupDeleteTimer()
		{
			InitializeComponent();
		}

		public void OnPopupYesCLick(object sender, EventArgs e)
		{
			ShouldDelete = true;
			popup.IsOpen = false;
		}
		public void OnPopupNoCLick(object sender, EventArgs e)
		{
			ShouldDelete = false;
			popup.IsOpen = false;
		}

	}
}
