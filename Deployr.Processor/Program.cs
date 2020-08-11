using Deployr.Processor.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Deployr.Processor.Helpers;

namespace Deployr.Processor
{
    class Program
    {
        
        static async Task Main()
        {
            var serviceProvider = BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger>();
            var backgroundDeployrTask = serviceProvider.GetService<IBackgroundDeployrTask>();
            var backgroundLogsTask = serviceProvider.GetService<ILoggingTask>().Start();
            try
            {
                await backgroundDeployrTask.ListenerTask;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed running background task");
            }
            finally
            {
                backgroundDeployrTask.CancellationTokenSource.Cancel();
            }
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(cfg => cfg.AddConsole())
                .Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Trace);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            // TODO: Setup/add logger
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddSingleton<AppSettings>();
            serviceCollection.AddSingleton<ILoggingQueue, LoggingQueue>();
            serviceCollection.AddSingleton<IBackgroundDeployrTask, BackgroundDeployrTask>();
            serviceCollection.AddSingleton<ILoggingTask, LoggingTask>();
            serviceCollection.AddTransient<IProcessorTask, ProcessorTask>();
            

            return serviceCollection.BuildServiceProvider();
        }
    }
}