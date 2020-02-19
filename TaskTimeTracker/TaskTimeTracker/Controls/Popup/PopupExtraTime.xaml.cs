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
	/// Interaction logic for PopupExtraTime.xaml
	/// </summary>
	public partial class PopupExtraTime : UserControl
	{
		public PopupExtraTime()
		{
			InitializeComponent();
			KeyDown += OnKeyPressed;
		}
		private void OnKeyPressed(object sender, KeyEventArgs e)
		{
			//if (e.Key == Key.Escape)
			//{
			//	popup.IsOpen = false;
			//}
			if (e.Key == Key.Enter)
			{
				OnPopupOkCLick(null, null);
			}
		}

		public void OnPopupOkCLick(object sender, EventArgs e)
		{
			popup.IsOpen = false;
		}
	}
}
