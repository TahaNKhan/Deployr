using Deployr.Web.Initialize;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Deployr.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			DatabaseInitializer.Initialize();
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>()
					.UseKestrel(options => 
						// 1 GB in bytes
						options.Limits.MaxRequestBodySize = 1073741824
					);
				});
		}
	}
}
