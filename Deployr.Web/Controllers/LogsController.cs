using System.Threading.Tasks;
using Deployr.Web.Contracts.RequestContracts;
using Deployr.Web.Contracts.ResponseContracts;
using Deployr.Web.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Deployr.Web.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
		private readonly ILogsLogic _logsLogic;
		public LogsController(ILogsLogic logsLogic)
		{
			_logsLogic = logsLogic;
		}

		[Route("")]
		[HttpPost]
		[ProducesResponseType(typeof(BasicResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(BasicResponse), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> AddLog([FromBody]AddLogRequest addLogRequest)
		{
			if (!ModelState.IsValid)
				throw new Exceptions.WebException(400, "Invalid input");

			await _logsLogic.AddLog(addLogRequest);

			return Ok(new BasicResponse());
		}
	}
}