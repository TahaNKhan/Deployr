using Deployr.Web.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deployr.Web.Attributes
{
	public class WebExceptionTransformerAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext context)
		{
			if (!(context.Exception is WebException webEx)) return;

			var httpContext = context.HttpContext;
			httpContext.Response.StatusCode = webEx.ErrorCode;
			context.Result = new ObjectResult(webEx.DefaultResponse);
			context.Exception = null;
		}
	}
}
