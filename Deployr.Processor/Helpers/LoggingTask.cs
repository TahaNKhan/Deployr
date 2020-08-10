using Deployr.Web.Contracts.RequestContracts;
using Deployr.Web.Contracts.ResponseContracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Deployr.Processor.Helpers
{
	public interface ILoggingTask
	{
		Task Start();
	}
	public class LoggingTask: ILoggingTask
	{
		private readonly ILoggingQueue _loggingQueue;

		public CancellationTokenSource CancellationTokenSource { get; private set; }

		public LoggingTask(ILoggingQueue loggingQueue) 
		{
			_loggingQueue = loggingQueue;
		}

		public Task Start()
		{
			CancellationTokenSource = new CancellationTokenSource();
			var token = CancellationTokenSource.Token;
			return Task.Factory.StartNew(async _ =>
			{
				List<Task> tasks = new List<Task>();
				while (!CancellationTokenSource.IsCancellationRequested)
				{
					var currentLogCount = _loggingQueue.GetCount();
					for (var i = 0; i < currentLogCount; i++)
						if (_loggingQueue.TryDequeue(out var result))
							tasks.Add(ProcessLog(result));

					await Task.WhenAll(tasks.ToArray());
					Thread.Sleep(10000);
				}
			}, null, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
				
		}

		private async Task ProcessLog(AddLogRequest request)
		{
			try
			{
				// web request to Deployr.Web

				await Task.FromResult(1);
			}
			catch (Exception)
			{
				// on failure requeue
				_loggingQueue.Enqueue(request);
			}
		}

	}
}
