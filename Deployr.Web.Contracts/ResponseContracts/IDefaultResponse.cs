using System;
using System.Collections.Generic;
using System.Text;

namespace Deployr.Web.Contracts.ResponseContracts
{
	public interface IDefaultResponse
	{
		bool WasSuccessful { get; }
		IEnumerable<string> ErrorMessages { get; }
	}
}
