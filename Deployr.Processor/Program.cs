using System;

namespace Deployr.Processor
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			while (true)
			{
				// Make a call to the deployr web service to get deployments in ready state

				// Update database status to unzipping, Unzip artifacts

				// Update deployment status to running script, Run the provided setup script

				// Dump script logs to the webservice

				// Update the status to failed/succeeded.
				
				// Sleep for ~5 seconds
			}
		}
	}
}
