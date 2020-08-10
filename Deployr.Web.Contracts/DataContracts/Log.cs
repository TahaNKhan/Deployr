using System;
using System.Collections.Generic;
using System.Text;

namespace Deployr.Web.Contracts.DataContracts
{
	public class Log
	{
		public int Id { get; set; }
		public string LogText { get; set; }
		public DateTimeOffset Timestamp { get; set; }
	}
}
