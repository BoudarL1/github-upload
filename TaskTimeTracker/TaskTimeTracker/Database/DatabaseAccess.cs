using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTimeTracker.Controls;
using TaskTimeTracker.Tracker.TaskHistory;

namespace TaskTimeTracker.Database
{
	public static class DatabaseAccess
	{
		private const string DATABASE_NAME = "TTT.db";
		public static void Save(TaskHistory taskHistory)
		{
			using (var db = new LiteDatabase(DATABASE_NAME))
			{
				var tasks = db.GetCollection<TaskHistory>();
				//db.DropCollection("TaskHistory");

				var existingTask = tasks.FindById(taskHistory.Id);

				if (existingTask == null)
					tasks.Insert(taskHistory);
				else
					tasks.Update(taskHistory);
			}
		}

		public static TaskHistory Get(int id)
		{
			using (var db = new LiteDatabase(DATABASE_NAME))
			{
				var tasks = db.GetCollection<TaskHistory>();

				return tasks.FindById(id);
			}
		}

		public static IEnumerable<TaskHistory> GetAll()
		{
			using (var db = new LiteDatabase(DATABASE_NAME))
			{
				var tasks = db.GetCollection<TaskHistory>();

				return tasks.Find(Query.All());
			}
		}

		public static IEnumerable<TaskHistory> GetAllFree()
		{
			using (var db = new LiteDatabase(DATABASE_NAME))
			{
				var tasks = db.GetCollection<TaskHistory>();

				return tasks.Find(Query.EQ("TimerType", TimerType.Free.ToString()));
			}
		}

		public static bool DeleteFreeTimer(int id)
		{
			using (var db = new LiteDatabase(DATABASE_NAME))
			{
				var tasks = db.GetCollection<TaskHistory>();
				var t = tasks.FindById(id);

				if (t != null && t.TimerType == TimerType.Free)
				{
					tasks.Delete(id);
					return true;
				}
			}
			return false;
		}
	}
}
