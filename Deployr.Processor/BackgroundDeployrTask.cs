using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Deployr.Processor
{
    /// <summary>
    /// Defines the long-running background task and cancellation token
    /// </summary>
    public interface IBackgroundDeployrTask
    {
        CancellationTokenSource CancellationTokenSource { get; }

        Task ListenerTask { get; }
    }

    public class BackgroundDeployrTask : IBackgroundDeployrTask
    {
        /// <summary>
        /// Wait period in Millisecond
        /// </summary>
        private const int WaitMillisecond = 5000;

        public CancellationTokenSource CancellationTokenSource { get; }

        public Task ListenerTask { get; }

        public BackgroundDeployrTask(IProcessorTask processorTask, ILogger<BackgroundDeployrTask> logger)
        {
            CancellationTokenSource = new CancellationTokenSource();

            var token = CancellationTokenSource.Token;

            ListenerTask = Task.Factory.StartNew(_ =>
            {
                try
                {
                    while (!CancellationTokenSource.IsCancellationRequested)
                    {
                        logger.LogTrace("Starting to run listener task");

                        processorTask.Process().Wait();

                        logger.LogTrace("Finished running listener task");

                        logger.LogTrace($"Starting to wait {WaitMillisecond}ms before the next run");

                        Thread.Sleep(WaitMillisecond);

                        logger.LogTrace($"Finished Waiting {WaitMillisecond}ms for the next run");
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Background task loop failed");
                }
            }, null, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }
}