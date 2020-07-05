using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Deployr.Web.DataAccess
{
	public interface IDataContext: IDisposable, IAsyncDisposable
	{
		IDeploymentDataBridge GetDeploymentDataBridge();
	}

	public class DataContext : IDataContext
	{
		private readonly DbConnection _dbConnection;
		private DataContext(DbConnection connection)
		{
			_dbConnection = connection;
		}

		/// <summary>
		/// .NET does not yet support async constructors so this is an alternative
		/// </summary>
		/// <param name="dbConnection"></param>
		/// <returns></returns>
		public static async Task<DataContext> ConstructAsync(DbConnection dbConnection)
		{
			await dbConnection.OpenAsync().ConfigureAwait(false);
			return new DataContext(dbConnection);
		}

		public IDeploymentDataBridge GetDeploymentDataBridge()
		{
			return new DeploymentDataBridge(_dbConnection);
		}

		public void Dispose()
		{
			DisposeAsync().GetAwaiter().GetResult();
		}

		public async ValueTask DisposeAsync()
		{
			await _dbConnection.CloseAsync().ConfigureAwait(false);
			await _dbConnection.DisposeAsync().ConfigureAwait(false);
		}
	}
}