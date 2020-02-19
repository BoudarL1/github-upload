using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace TaskTimeTracker.Controls.Screens
{
	/// <summary>
	/// Interaction logic for SettingsScreen.xaml
	/// </summary>
	public partial class SettingsScreen : UserControl
	{
		public SettingsScreen()
		{
			InitializeComponent();

			Refresh();
		}

		public void OnSave(object sender, EventArgs e)
		{
			Configuration.Configuration.SetUserAcronym(tbUserAcronym.Text);
			Refresh();
		}

		internal void Refresh()
		{
			tbUserAcronym.Text = ConfigurationManager.AppSettings[Configuration.Configuration.USER_ACRONYM_KEY];
		}
	}
}
