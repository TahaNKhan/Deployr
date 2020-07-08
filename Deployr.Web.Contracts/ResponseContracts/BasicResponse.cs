using System.Collections.Generic;
using System.Linq;

namespace Deployr.Web.Contracts.ResponseContracts
{
	public class BasicResponse: IDefaultResponse
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
		public bool WasSuccessful => (ErrorMessages?.Count() ?? 0) > 0 ? false : true;

		public IEnumerable<string> ErrorMessages { get; set; }
		public string CommonIdentifier { get; set; }
	}
}
