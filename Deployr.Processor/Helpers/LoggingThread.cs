using Deployr.Web.Contracts.RequestContracts;
using Deployr.Web.Contracts.ResponseContracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Deployr.Processor.Helpers
{
	public class LoggingThread
	{
		private static readonly LoggingThread _instance;

		public static ConcurrentBag<AddLogRequest> LogRequests { get; } = new ConcurrentBag<AddLogRequest>();

		public static bool IsStopping = false;

		private LoggingThread() { }

		public static void Start()
		{
			List<Task> tasks = new List<Task>();
			while (!IsStopping)
			{
				var currentLogCount = LogRequests.Count;
				for (var i = 0; i < currentLogCount; i++)
					if (LogRequests.TryTake(out var result))
						tasks.Add(ProcessLog(result));

				Task.WaitAll(tasks.ToArray());
				Thread.Sleep(10000);
			}
		}

		public static async Task<BasicResponse> ProcessLog(AddLogRequest request)
		{
			// web request to Deployr.Web

			return await Task.FromResult(new BasicResponse());
		}

	}
}
