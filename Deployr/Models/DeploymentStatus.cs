using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deployr.Web.Models
{
	public enum DeploymentStatus
	{
		Created = 1,
		Ready = 2,
		Unzipping = 3,
		RunningScript = 4,
		Done = 5,
		Failed = 6
	}
}
