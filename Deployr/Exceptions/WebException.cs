using Deployr.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deployr.Web.Exceptions
{
	public class WebException : Exception
	{
		public int ErrorCode { get; }
		public string ErrorMessage { get; }

		public DefaultResponse DefaultResponse => new DefaultResponse(new[] { ErrorMessage });

		public WebException(int errorCode)
		{
			ErrorCode = errorCode;
			ErrorMessage = "Something went wrong";
		}

		public WebException(string errorMessage)
		{
			ErrorMessage = errorMessage;
			ErrorCode = 400;
		}

		public WebException(int errorCode, string errorMessage)
		{
			ErrorCode = errorCode;
			ErrorMessage = errorMessage;
		}
	}
}
