using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Deployr.Web.Controllers
{
    [Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        public async Task<IActionResult> RedirectToSwagger()
        {
            return await Task.FromResult<IActionResult>(Redirect("/docs"));
        }
    }
}