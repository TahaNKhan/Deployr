using Deployr.Processor.Models;
using Deployr.Web.Contracts;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deployr.Processor
{
	public interface IProcessorTask
	{
		Task Process();
	}
	public class ProcessorTask: IProcessorTask
	{
		private readonly AppSettings _appSettings;
		public ProcessorTask(AppSettings appSettings)
		{
			_appSettings = appSettings;
		}

		public async Task Process()
		{

			// Make a call to the deployr web service to get deployments in ready state
			var deployments = await GetReadyDeployments();
			var processTasks = new List<Task<bool>>();
			foreach (var deployment in deployments)
				processTasks.Add(ProcessDeployment(deployment));

			var results = await Task.WhenAll(processTasks);

			if (!results.All(s => s))
			{
				Console.WriteLine("Something failed");
			}
		}

		private async Task<bool> ProcessDeployment(DeploymentInformation deployment)
		{
			// Update database status to unzipping, Unzip artifacts

			// Update deployment status to running script, Run the provided setup script

			// Dump script logs to the webservice

			// Update the status to failed/succeeded.
			return true;
		}

		private static void UnzipArtifacts(string artifactFullFileName, string deployLocation)
		{
			try
			{
				System.IO.Compression.ZipFile.ExtractToDirectory(artifactFullFileName, deployLocation);
			}
			catch (Exception)
			{
				// Clean directory
				Directory.Delete(deployLocation, true);
				throw new InvalidOperationException("Unable to unzip file");
			}
		}

		private async Task<IEnumerable<Web.Contracts.DeploymentInformation>> GetReadyDeployments()
		{
			var client = new RestClient(_appSettings.DeployrUrl);
			var request = new RestRequest("/api/Deployments", Method.GET);
			request.AddQueryParameter("deploymentStatuses", Web.Contracts.DeploymentStatus.Ready.ToString());
			var result = await client.ExecuteAsync(request);

			return result.StatusCode == System.Net.HttpStatusCode.OK 
				? JsonConvert.DeserializeObject<IEnumerable<Web.Contracts.DeploymentInformation>>(result.Content)
				: null;
		}
	}
}
