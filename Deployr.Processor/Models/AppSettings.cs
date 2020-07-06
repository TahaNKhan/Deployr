using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Deployr.Processor.Models
{
	public class AppSettings
	{
		private readonly IConfiguration _configuration;
		public AppSettings(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public string DeployrUrl => _configuration["DeployrUrl"];
	}
}
