using Deployr.Processor.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Deployr.Processor
{
	class Program
	{
		static async Task Main()
		{
			var serviceProvider = BuildServiceProvider();

			var processTask = serviceProvider.GetRequiredService<IProcessorTask>();

			while (true) 
			{
				await processTask.Process();

				// Sleep for 5 seconds
				Thread.Sleep(5000);
			}
		}

		private static IServiceProvider BuildServiceProvider()
		{
			var serviceCollection = new ServiceCollection();
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false)
				.Build();

			serviceCollection.AddSingleton<IConfiguration>(configuration);
			serviceCollection.AddSingleton<AppSettings>();
			serviceCollection.AddTransient<IProcessorTask, ProcessorTask>();
			return serviceCollection.BuildServiceProvider();
		}
	}
}
