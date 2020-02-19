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
	/// Interaction logic for PopupUserControl.xaml
	/// </summary>
	public partial class PopupUserControl : UserControl
	{
		public ICommand CmdEscape { get { return null; } }
		public PopupUserControl()
		{
			InitializeComponent();
			Focus();
			PreviewKeyDown += OnKeyPressed;
		}

		public void OnEscape(object sender, KeyEventArgs e)
		{

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
