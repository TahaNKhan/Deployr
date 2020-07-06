namespace Deployr.Web.Contracts
{
	public class CreateDeploymentRequest
	{
		public string PackageName { get; set; }
		public string Version { get; set; }
	}
}
