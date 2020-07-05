namespace Deployr.Web.Models
{
	public class CreateDeploymentRequest
	{
		public string PackageName { get; set; }
		public string Version { get; set; }
	}
}
