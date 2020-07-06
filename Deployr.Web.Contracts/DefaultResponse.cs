using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Deployr.Web.Contracts
{
	public class DefaultResponse
	{
		public DefaultResponse(string commonIdentifier)
		{
			CommonIdentifier = commonIdentifier;
		}

		public DefaultResponse(IEnumerable<string> errorMessages)
		{
			ErrorMessages = errorMessages;
		}
		public bool WasSuccessful => (ErrorMessages?.Count() ?? 0) > 0 ? false : true;
		public IEnumerable<string> ErrorMessages { get; set; }
		public string CommonIdentifier { get; set; }
	}
}
