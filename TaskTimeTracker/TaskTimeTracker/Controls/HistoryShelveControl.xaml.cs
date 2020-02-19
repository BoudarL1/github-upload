using Microsoft.TeamFoundation.VersionControl.Client;
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

namespace TaskTimeTracker.Controls
{
	/// <summary>
	/// Interaction logic for HistoryShelveControl.xaml
	/// </summary>
	public partial class HistoryShelveControl : UserControl
	{

		public HistoryShelveControl()
		{
			InitializeComponent();
		}

		public HistoryShelveControl(DateTime shelveCreationDate) : this()
		{
			lblShelveDate.Content = shelveCreationDate.ToString(Configuration.Configuration.DATETIME_FORMAT);
		}
	}
}
