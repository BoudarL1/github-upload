using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTimeTracker.Configuration
{
	public class Configuration
	{
		public const string DATETIME_FORMAT = "dd/MM/yyyy - HH:mm";
		public const string USER_ACRONYM_KEY = "UserAcronym";

		public static void SetUserAcronym(string acronym)
		{
			var a = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			a.AppSettings.Settings.Remove("UserAcronym");
			a.AppSettings.Settings.Add("UserAcronym", acronym);
			a.Save();

			ConfigurationManager.RefreshSection("appSettings");
		}
	}
}
