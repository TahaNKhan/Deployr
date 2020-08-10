using Deployr.Web.Contracts.RequestContracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Deployr.Processor.Helpers
{
	public interface ILoggingQueue 
	{
		void Enqueue(AddLogRequest request);
		bool TryDequeue(out AddLogRequest request);
		int GetCount();
	}
	public class LoggingQueue : ILoggingQueue
	{
		private ConcurrentQueue<AddLogRequest> requests;

		public LoggingQueue()
		{
			requests = new ConcurrentQueue<AddLogRequest>();
		}

		public void Enqueue(AddLogRequest request)
		{
			requests.Enqueue(request);
		}

		public bool TryDequeue(out AddLogRequest request)
		{
			return requests.TryDequeue(out request);
		}

		public int GetCount() => requests.Count;
	}
}
