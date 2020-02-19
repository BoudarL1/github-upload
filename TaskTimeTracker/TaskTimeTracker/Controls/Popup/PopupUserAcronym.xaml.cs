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
	/// Interaction logic for PopupUserAcronym.xaml
	/// </summary>
	public partial class PopupUserAcronym : UserControl
	{
		public PopupUserAcronym()
		{
			InitializeComponent();
		}
		public void OnPopupOkCLick(object sender, EventArgs e)
		{
			if (tbValue.Text.Length < 5 && tbValue.Text.Length > 1)
			{
				popup.IsOpen = false;
			}
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
	}
}
