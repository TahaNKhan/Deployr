using Deployr.Web.Contracts.DataContracts;
namespace Deployr.Web.Contracts.RequestContracts
{
	public class UpdateDeploymentStatusRequest
	{
		public DeploymentStatus Status { get; set; }
	}
}
