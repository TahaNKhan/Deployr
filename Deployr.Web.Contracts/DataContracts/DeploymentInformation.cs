using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deployr.Web.Contracts.DataContracts
{
	public class DeploymentInformation
	{
		public int Id { get; set; }
		public string PackageName { get; set; }
		public string Version { get; set; }
		public DeploymentStatus DeploymentStatus { get; set; }
		public string DeploymentLocation { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public IEnumerable<Log> Logs { get; set; }
	}
}
