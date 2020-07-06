using System.Collections.Generic;
using System.Threading.Tasks;
using Deployr.Web.Contracts;
using Deployr.Web.Exceptions;
using Deployr.Web.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Deployr.Web.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DeploymentsController : ControllerBase
	{
		private readonly IDeployLogic _deployLogic;
		public DeploymentsController(IDeployLogic deployLogic)
		{
			_deployLogic = deployLogic;
		}

		[Route("")]
		[HttpPost]
		[ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateDeployment([FromBody]CreateDeploymentRequest metadata)
		{
			var result = await _deployLogic.CreateDeployment(metadata);
			return new JsonResult(result) { StatusCode = 201 };
		}
		
		[Route("{id}")]
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeploymentInformation))]
		public async Task<IActionResult> GetDeploymentInformation([FromRoute]int id)
		{
			var result = await _deployLogic.GetDeploymentInformation(id);
			return Ok(result);
		}


		[Route("")]
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DeploymentInformation>))]
		public async Task<IActionResult> GetDeployments([FromQuery]IEnumerable<DeploymentStatus> deploymentStatuses)
		{
			var result = await _deployLogic.GetDeploymentsAsync(deploymentStatuses);
			return Ok(result);
		}


		[Route("{id}/UploadPackage")]
		[HttpPost]
		[ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(DefaultResponse), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> DeployToMachine([FromRoute]int id, IFormFile formFile)
		{
			if (!ModelState.IsValid)
				throw new WebException(400, "Invalid input");
			var result = await _deployLogic.DeployToLocalMachineAsync(id, formFile);

			return Ok(result);
		}
	}
}