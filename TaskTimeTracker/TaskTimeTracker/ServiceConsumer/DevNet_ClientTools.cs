using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TaskTimeTracker.ClientToolsReference;
using Task = TaskTimeTracker.ClientToolsReference.Task;
using TaskStatus = TaskTimeTracker.ClientToolsReference.TaskStatus;

namespace TaskTimeTracker.ServiceConsumer
{
    public class DevNet_ClientTools
    {
        private static ClientTools m_Service = null;

        public static ClientTools DevNetService
        {
            get
            {
                if(m_Service == null)
                {
                    m_Service = new ClientTools();
                    m_Service.Url = string.Concat("http://custo-internal.be.bvdep.net/DevNet/Services/ClientTools.asmx");
                    m_Service.Credentials = CredentialCache.DefaultNetworkCredentials;
                    m_Service.Timeout = 2000;
                }

                return m_Service;
            }
        }

        public static string User
        {
            get { return ConfigurationManager.AppSettings[Configuration.Configuration.USER_ACRONYM_KEY]; }
        }

        public static Task[] GetTasks()
        {
            return DevNetService.GetTaskList(User, new TaskStatus[] { TaskStatus.Assigned, TaskStatus.Feedback, TaskStatus.InProgress, TaskStatus.InReview, TaskStatus.Suspended });
        }
		public static Task GetTask(int id)
		{
			return DevNetService.GetTask(id);
		}
		public static Task[] GetTasks(TaskStatus taskStatus)
		{
			try
			{
				return DevNetService.GetTaskList(User, new TaskStatus[] { taskStatus });
			}
			catch (Exception)
			{

			}
			return new Task[] { };
		}
	}
}
