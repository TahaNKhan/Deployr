﻿using System.Collections.Generic;
using System.Linq;

namespace Deployr.Web.Contracts.ResponseContracts
{
	/// <summary>
	/// The "default" response.
	/// This should be used instead of an empty result body,
	/// which will allow us to extend the contract in the future.
	/// </summary>
	public class BasicResponse: DefaultResponse<string>
	{
		public BasicResponse()
		{
			
		}
		public BasicResponse(string identifier)
		{
			Identifier = identifier;
		}

		public BasicResponse(IEnumerable<string> errorMessages)
		{
			ErrorMessages = errorMessages;
		}
		public new bool WasSuccessful => (ErrorMessages?.Count() ?? 0) > 0 ? false : true;
	}
}
