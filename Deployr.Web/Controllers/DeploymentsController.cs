﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Deployr.Web.Contracts.DataContracts;
using Deployr.Web.Contracts.RequestContracts;
using Deployr.Web.Contracts.ResponseContracts;
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
		[ProducesResponseType(typeof(BasicResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(typeof(BasicResponse), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateDeployment([FromBody]CreateDeploymentRequest metadata)
		{
			var result = await _deployLogic.CreateDeployment(metadata);
			return new JsonResult(result) { StatusCode = 201 };
		}
		
		[Route("{id}")]
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeploymentInformation))]
		public async Task<IActionResult> GetDeploymentInformation([FromRoute]int id, bool? includeLogs)
		{
			var result = await _deployLogic.GetDeploymentInformation(id, includeLogs.HasValue ? includeLogs.Value : false);
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
		[ProducesResponseType(typeof(BasicResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BasicResponse), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UploadPackage([FromRoute]int id, IFormFile formFile)
		{
			if (!ModelState.IsValid)
				throw new WebException(400, "Invalid input");
			var result = await _deployLogic.UploadPackageAsync(id, formFile);

			return Ok(result);
		}

		[Route("{id}/status")]
		[HttpPut]
		[ProducesResponseType(typeof(BasicResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BasicResponse), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateStatus([FromRoute]int id, [FromBody]UpdateDeploymentStatusRequest statusUpdateRequest)
		{
			if (!ModelState.IsValid)
				throw new WebException(400, "Invalid input");
			var result = await _deployLogic.UpdateDeploymentStatus(id, statusUpdateRequest.Status);
			if (!result)
				throw new WebException(400, "Unable to update status");
			return Ok(new BasicResponse());
		}
	}
}