using Dapper;
using Deployr.Web.Contracts.DataContracts;
using Deployr.Web.Contracts.RequestContracts;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Deployr.Web.DataAccess
{
	public interface ILogsDataBridge
	{
		Task AddLogAsync(AddLogRequest request);
		Task<IEnumerable<Log>> GetLogsAsync(int deploymentId);
	}

	public class LogsDataBridge: ILogsDataBridge
	{
		private readonly DbConnection _connection;
		public LogsDataBridge(DbConnection connection)
		{
			_connection = connection;
		}

		public async Task AddLogAsync(AddLogRequest request)
		{
			var insertCommand = "INSERT INTO logs(deployment_id, log) VALUES (@DeploymentId, @Log);";
			await _connection.ExecuteAsync(insertCommand, new { request.DeploymentId, request.Log }).ConfigureAwait(false);
		}

		public async Task<IEnumerable<Log>> GetLogsAsync(int deploymentId)
		{
			var getLogsCommand = "SELECT id, log as logtext, timestamp_utc as timestamp FROM logs WHERE deployment_id = @id";
			var result = await _connection.QueryAsync<Log>(getLogsCommand, new { id = deploymentId }).ConfigureAwait(false);
			return result.ToList();
		}
	}
}
