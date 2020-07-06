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

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
