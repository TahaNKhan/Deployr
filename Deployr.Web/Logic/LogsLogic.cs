using Deployr.Web.Contracts.RequestContracts;
using Deployr.Web.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deployr.Web.Logic
{
	public interface ILogsLogic
	{
		Task AddLog(AddLogRequest request);
	}

	public class LogsLogic : ILogsLogic
	{
		private readonly IDataContextFactory _dataContextFactory;
		public LogsLogic(IDataContextFactory dataContextFactory)
		{
			_dataContextFactory = dataContextFactory;
		}

		public async Task AddLog(AddLogRequest request)
		{
			await using var dataContext = await _dataContextFactory.Construct();
			var logsDataBridge = dataContext.GetLogsDataBridge();
			await logsDataBridge.AddLogAsync(request);
		}
	}
}
