using System;
using System.Collections.Generic;
using System.Text;

namespace Deployr.Web.Contracts.ResponseContracts
{
	/// <summary>
	/// All reponse contracts should inherit from this.
	/// Makes it easier for the consumers to check if the web request was successful
	/// </summary>
	public abstract class DefaultResponse
	{
		public bool WasSuccessful { get; set;  }
		public IEnumerable<string> ErrorMessages { get; set; }
	}

	/// <summary>
	/// All reponse contracts should inherit from this.
	/// Makes it easier for the consumers to check if the web request was successful
	/// </summary>
	public abstract class DefaultResponse<T> : DefaultResponse
	{
		public T CommonIdentifier { get; set; }
	}
}
