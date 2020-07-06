using Dapper;
using Deployr.Web.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Deployr.Web.DataAccess
{
	public interface IDeploymentDataBridge
	{
		Task<int> CreateDeployment(CreateDeploymentRequest metadata);

		Task<DeploymentInformation> GetDeploymentMetadata(int id);

		Task<bool> UpdateDeployment(int id, string deploymentLocation, DeploymentStatus status);
		Task<IEnumerable<DeploymentInformation>> GetDeploymentsAsync(IEnumerable<DeploymentStatus> deploymentStatuses);
	}
	public class DeploymentDataBridge : IDeploymentDataBridge
	{
		private readonly DbConnection _connection;
		public DeploymentDataBridge(DbConnection connection)
		{
			_connection = connection;
		}

		public async Task<int> CreateDeployment(CreateDeploymentRequest metadata)
		{
			var insertCommand = "INSERT INTO deployments(package_name, version, status) VALUES (@PackageName, @Version, @Status);";
			await _connection.ExecuteAsync(insertCommand, new { metadata.PackageName, metadata.Version, Status = (byte)DeploymentStatus.Created }).ConfigureAwait(false);

			var getIdCommand = "SELECT id FROM deployments WHERE package_name = @PackageName AND version = @Version ORDER BY timestamp_utc DESC";

			var result = await _connection.QueryFirstAsync<int>(getIdCommand, new { metadata.PackageName, metadata.Version }).ConfigureAwait(false);

			return result;
		}

		public async Task<DeploymentInformation> GetDeploymentMetadata(int id)
		{
			var getIdCommand = "SELECT id, package_name as packagename, version, status as deploymentstatus, timestamp_utc as timestamp, artifact_location as deploymentlocation FROM deployments WHERE id = @id";
			var result = await _connection.QueryFirstAsync<DeploymentInformation>(getIdCommand, new { id }).ConfigureAwait(false);
			return result;
		}

		public async Task<IEnumerable<DeploymentInformation>> GetDeploymentsAsync(IEnumerable<DeploymentStatus> deploymentStatuses)
		{
			var getIdsCommand = "SELECT id, package_name as packagename, version, status as deploymentstatus, timestamp_utc as timestamp, artifact_location as deploymentlocation FROM deployments WHERE status IN @Statuses;";
			var result = await _connection.QueryAsync<DeploymentInformation>(getIdsCommand, new { Statuses = deploymentStatuses.Select(s => (byte)s) });
			return result;
		}

		public async Task<bool> UpdateDeployment(int id, string deploymentLocation, DeploymentStatus status)
		{
			var updateSql = "UPDATE deployments SET status = @status, artifact_location = @deploymentLocation WHERE id = @id;";
			var result = await _connection.ExecuteAsync(updateSql, new { status = (byte)status, deploymentLocation, id });
			return result == 1;
		}
		
		private static string ConvertIEnumerableToString<T>(IEnumerable<T> input)
		{
			var result = string.Join(",", input.ToList());
			return result;
		}
	}
}
