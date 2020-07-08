using System.Collections.Generic;
using System.Linq;

namespace Deployr.Web.Contracts.ResponseContracts
{
	public class BasicResponse: DefaultResponse<string>
	{
		public BasicResponse()
		{
			
		}
		public BasicResponse(string commonIdentifier)
		{
			CommonIdentifier = commonIdentifier;
		}

		public BasicResponse(IEnumerable<string> errorMessages)
		{
			ErrorMessages = errorMessages;
		}
		public new bool WasSuccessful => (ErrorMessages?.Count() ?? 0) > 0 ? false : true;
	}
}
