using Deployr.Web.Constants;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Deployr.Web.DataAccess
{
	public interface IDataContextFactory
	{
		Task<IDataContext> Construct();
	}
	public class DataContextFactory : IDataContextFactory
	{
		public async Task<IDataContext> Construct()
		{
			var connection = new SQLiteConnection($"Data Source={DatabaseConstants.DatabaseFileName}");
			return await DataContext.ConstructAsync(connection);
		}

		
	}
}
