using Deployr.Processor.Models;
using Deployr.Web.Contracts.DataContracts;
using Deployr.Web.Contracts.RequestContracts;
using Deployr.Web.Contracts.ResponseContracts;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
			// TODO: Handle failure better
			if (deployments == null)
				return;
			if (!deployments.Any())
				return;
			var processTasks = new List<Task<bool>>();
			foreach (var deployment in deployments.Take(_appSettings.MaxConcurrentDeployments))
				processTasks.Add(ProcessDeployment(deployment));

			var results = await Task.WhenAll(processTasks);

			if (!results.All(s => s))
				Console.WriteLine("Something failed");
		}

		private async Task<bool> ProcessDeployment(DeploymentInformation deployment)
		{
			try
			{
				var artifactLocation = $"{deployment.DeploymentLocation}\\artifacts";
				var artifactFilePath = $"{artifactLocation}\\{deployment.PackageName}-{deployment.Version}.zip";
				// Update database status to unzipping, Unzip artifacts
				var updateStatusResult = await UpdateDeploymentStatus(deployment.Id, DeploymentStatus.Unzipping);
				if (!updateStatusResult.WasSuccessful) { /** Log something... once we have a logger */ }
				await UnzipArtifacts(artifactFilePath, deployment.DeploymentLocation);

				// Update deployment status to running script, Run the provided setup script
				updateStatusResult = await UpdateDeploymentStatus(deployment.Id, DeploymentStatus.RunningScript);
				if (!updateStatusResult.WasSuccessful) { /** Log something... once we have a logger */ }

				// Dump script logs to the webservice

				// Update the status to failed/succeeded.
				updateStatusResult = await UpdateDeploymentStatus(deployment.Id, DeploymentStatus.Done);
				if (!updateStatusResult.WasSuccessful) { /** Log something... once we have a logger */ }

				return true;
			}
			catch (Exception) 
			{
				await UpdateDeploymentStatus(deployment.Id, DeploymentStatus.Failed);

				return false;
			}
		}

		private static Task UnzipArtifacts(string artifactFullFileName, string deployLocation)
		{
			return Task.Run(() =>
			{
				try
				{
					System.IO.Compression.ZipFile.ExtractToDirectory(artifactFullFileName, deployLocation);
				}
				catch (Exception)
				{
					throw new InvalidOperationException("Unable to unzip file");
				}
			});

		
		}

		private async Task<DefaultResponse> UpdateDeploymentStatus(int id, DeploymentStatus status)
		{
			var client = CreateDeployrRestClient();
			var restRequest = new RestRequest($"api/Deployments/{id}/status", Method.PUT);
			var requestBody = new UpdateDeploymentStatusRequest { Status = status };
			restRequest.AddJsonBody(requestBody);
			var result = await client.ExecuteAsync(restRequest);

			return result.StatusCode == System.Net.HttpStatusCode.OK
				? JsonConvert.DeserializeObject<BasicResponse>(result.Content)
				: null;
		}

		private async Task<IEnumerable<DeploymentInformation>> GetReadyDeployments()
		{
			var client = CreateDeployrRestClient();
			var request = new RestRequest("/api/Deployments", Method.GET);
			request.AddQueryParameter("deploymentStatuses", DeploymentStatus.Ready.ToString());
			var result = await client.ExecuteAsync(request);

			return result.StatusCode == System.Net.HttpStatusCode.OK 
				? JsonConvert.DeserializeObject<IEnumerable<DeploymentInformation>>(result.Content)
				: null;
		}

		private IRestClient CreateDeployrRestClient()
		{
			if (string.IsNullOrWhiteSpace(_appSettings.DeployrUrl))
				throw new InvalidOperationException("Deployr Web URL is not set.");
			return new RestClient(_appSettings.DeployrUrl);
		}
	}
}
