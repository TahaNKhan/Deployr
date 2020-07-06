using Deployr.Web.Contracts;
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
		Task<DefaultResponse> DeployToLocalMachineAsync(int deploymentId, IFormFile file);
		Task<DefaultResponse> CreateDeployment(CreateDeploymentRequest metadata);
		Task<DeploymentInformation> GetDeploymentInformation(int id);
		Task<IEnumerable<DeploymentInformation>> GetDeploymentsAsync(IEnumerable<DeploymentStatus> deploymentStatuses);
	}
	public class DeployLogic : IDeployLogic
	{
		private readonly IDataContextFactory _dataContextFactory;

		public DeployLogic(IDataContextFactory dataContextFactory)
		{
			_dataContextFactory = dataContextFactory;
		}

		public async Task<DefaultResponse> DeployToLocalMachineAsync(int deploymentId, IFormFile file)
		{
			var deploymentInformation = await GetDeploymentInformation(deploymentId);

			if (deploymentInformation == null)
				throw new WebException(400, "Deployment not found.");

			if (deploymentInformation.DeploymentStatus != DeploymentStatus.Created)
				return new DefaultResponse(deploymentId.ToString());

			var deployLocation = GetDeployLocation(deploymentInformation.PackageName, deploymentInformation.Version);

			ValidateAndCleanDirectory(deployLocation, true);

			var artifactDirectory = CreateArtifactsDirectory(deployLocation);

			var artifactFileName = $"{deploymentInformation.PackageName}-{deploymentInformation.Version}.zip";
			var artifactFullFileName = $"{artifactDirectory}\\{artifactFileName}";
			await SaveFileToDiskAsync(file, artifactFullFileName);

			var updateResult = await UpdateDeployment(deploymentId, deployLocation, DeploymentStatus.Ready);

			if (!updateResult)
				throw new WebException(500, "Unable to update deploy location.");

			return new DefaultResponse(deploymentId.ToString());
		}

		public async Task<DefaultResponse> CreateDeployment(CreateDeploymentRequest metadata)
		{
			using var dataContext = await _dataContextFactory.Construct();
			var dataBridge = dataContext.GetDeploymentDataBridge();
			var result = await dataBridge.CreateDeployment(metadata);
			return new DefaultResponse(result.ToString());
		}

		public async Task<DeploymentInformation> GetDeploymentInformation(int id)
		{
			using var dataContext = await _dataContextFactory.Construct();
			var dataBridge = dataContext.GetDeploymentDataBridge();
			var result = await dataBridge.GetDeploymentMetadata(id);
			return result;
		}

		public async Task<bool> UpdateDeployment(int id, string deploymentLocation, DeploymentStatus status)
		{
			using var dataContext = await _dataContextFactory.Construct();
			var dataBridge = dataContext.GetDeploymentDataBridge();
			var result = await dataBridge.UpdateDeployment(id, deploymentLocation, status);
			return result;
		}

		private static async Task SaveFileToDiskAsync(IFormFile file, string filePath)
		{
			using var fileStream = new FileStream(filePath, FileMode.Create);
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
			if (directoryExists && forceDeploy)
				Directory.Delete(deployLocation, true);

			return Directory.CreateDirectory(deployLocation);

		}

		private static string GetDeployLocation(string packageName, string version)
		{
			return $"{DetermineDefaultDeployLocation()}\\{packageName}-{version}";
		}

		private static string DetermineDefaultDeployLocation()
		{
			string WindowsDefaultDeployLocation = "C:\\CustomApps";
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return WindowsDefaultDeployLocation;
			throw new WebException(500, "Unsupported platform");
		}

		public async Task<IEnumerable<DeploymentInformation>> GetDeploymentsAsync(IEnumerable<DeploymentStatus> deploymentStatuses)
		{
			using var dataContext = await _dataContextFactory.Construct();
			var dataBridge = dataContext.GetDeploymentDataBridge();
			var result = await dataBridge.GetDeploymentsAsync(deploymentStatuses);
			return result;
		}
	}
}
