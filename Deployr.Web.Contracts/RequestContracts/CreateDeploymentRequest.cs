namespace Deployr.Web.Contracts.RequestContracts
{
	public class CreateDeploymentRequest
	{
		public string PackageName { get; set; }
		public string Version { get; set; }
	}
}
