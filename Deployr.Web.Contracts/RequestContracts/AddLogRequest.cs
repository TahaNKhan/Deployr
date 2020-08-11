using System;
using System.Collections.Generic;
using System.Text;

namespace Deployr.Web.Contracts.RequestContracts
{
	public class AddLogRequest
	{
		public int DeploymentId { get; set; }
		public string Log { get; set; }
	}
}
