using Deployr.Web.Contracts.DataContracts;
using Deployr.Web.Contracts.RequestContracts;
using Deployr.Web.Contracts.ResponseContracts;
using Deployr.Web.DataAccess;
using Deployr.Web.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Deployr.Web.Logic
{
	public interface IDeployLogic
	{
		Task<BasicResponse> UploadPackageAsync(int deploymentId, IFormFile file);
		Task<BasicResponse> CreateDeployment(CreateDeploymentRequest metadata);
		Task<DeploymentInformation> GetDeploymentInformation(int id, bool includeLogs);
		Task<IEnumerable<DeploymentInformation>> GetDeploymentsAsync(IEnumerable<DeploymentStatus> deploymentStatuses);
		Task<bool> UpdateDeploymentStatus(int id, DeploymentStatus status);
	}
	public class DeployLogic : IDeployLogic
	{
		private readonly IDataContextFactory _dataContextFactory;

		public DeployLogic(IDataContextFactory dataContextFactory)
		{
			_dataContextFactory = dataContextFactory;
		}

		public async Task<BasicResponse> UploadPackageAsync(int deploymentId, IFormFile file)
		{
			var deploymentInformation = await GetDeploymentInformation(deploymentId, false);

			if (deploymentInformation == null)
				throw new WebException(400, "Deployment not found.");

			if (deploymentInformation.DeploymentStatus != DeploymentStatus.Created)
				return new BasicResponse(deploymentId.ToString());

			var deployLocation = GetDeployLocation(deploymentInformation.PackageName, deploymentInformation.Version);

			ValidateAndCleanDirectory(deployLocation, true);

			var artifactDirectory = CreateArtifactsDirectory(deployLocation);

			var artifactFileName = $"{deploymentInformation.PackageName}-{deploymentInformation.Version}.zip";
			var artifactFullFileName = Path.Join(artifactDirectory.ToString(), artifactFileName);
			await SaveFileToDiskAsync(file, artifactFullFileName);

			var updateResult = await UpdateDeployment(deploymentId, deployLocation, DeploymentStatus.Ready);

			if (!updateResult)
				throw new WebException(500, "Unable to update deploy location.");

			return new BasicResponse(deploymentId.ToString());
		}

		public async Task<BasicResponse> CreateDeployment(CreateDeploymentRequest metadata)
		{
			await using var dataContext = await _dataContextFactory.Construct();
			var dataBridge = dataContext.GetDeploymentDataBridge();
			var result = await dataBridge.CreateDeployment(metadata);
			return new BasicResponse(result.ToString());
		}

		public async Task<DeploymentInformation> GetDeploymentInformation(int id, bool includeLogs)
		{
			await using var dataContext = await _dataContextFactory.Construct();
			var dataBridge = dataContext.GetDeploymentDataBridge();
			var result = await dataBridge.GetDeploymentMetadata(id);
			if (includeLogs)
				result.Logs = await dataContext.GetLogsDataBridge().GetLogsAsync(id);
			
			return result;
		}

		public async Task<bool> UpdateDeployment(int id, string deploymentLocation, DeploymentStatus status)
		{
			await using var dataContext = await _dataContextFactory.Construct();
			var dataBridge = dataContext.GetDeploymentDataBridge();
			var result = await dataBridge.UpdateDeployment(id, deploymentLocation, status);
			return result;
		}

		public async Task<bool> UpdateDeploymentStatus(int id, DeploymentStatus status)
		{
			await using var dataContext = await _dataContextFactory.Construct();
			var dataBridge = dataContext.GetDeploymentDataBridge();
			var result = await dataBridge.UpdateDeploymentStatus(id, status);
			return result;
		}

		private static async Task SaveFileToDiskAsync(IFormFile file, string filePath)
		{
			await using var fileStream = new FileStream(filePath, FileMode.Create);
			await file.CopyToAsync(fileStream);
		}

		private DirectoryInfo CreateArtifactsDirectory(string deployLocation)
		{
			var directoryInfo = Directory.CreateDirectory(deployLocation);
			var artifactDirectory = directoryInfo.CreateSubdirectory("artifacts");
			return artifactDirectory;
		}

		private DirectoryInfo ValidateAndCleanDirectory(string deployLocation, bool forceDeploy)
		{
			var directoryExists = Directory.Exists(deployLocation);
			if (directoryExists && !forceDeploy)
				throw new WebException(400, "Package already deployed");

			// Delete old artifacts
			if (directoryExists)
				Directory.Delete(deployLocation, true);

			return Directory.CreateDirectory(deployLocation);

		}

		private static string GetDeployLocation(string packageName, string version)
		{
			return Path.Join(DetermineDefaultDeployLocation(), $"{packageName}-{version}");
		}

		private static string DetermineDefaultDeployLocation()
		{
			const string windowsDefaultDeployLocation = "C:\\CustomApps";
			const string linuxDefaultDeployLocation = "/usr/local/bin/CustomApps";
			
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return windowsDefaultDeployLocation;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return linuxDefaultDeployLocation;
			throw new WebException(500, "Unsupported platform");
		}

		public async Task<IEnumerable<DeploymentInformation>> GetDeploymentsAsync(IEnumerable<DeploymentStatus> deploymentStatuses)
		{
			await using var dataContext = await _dataContextFactory.Construct();
			var dataBridge = dataContext.GetDeploymentDataBridge();
			var result = await dataBridge.GetDeploymentsAsync(deploymentStatuses);
			return result;
		}
	}
}
