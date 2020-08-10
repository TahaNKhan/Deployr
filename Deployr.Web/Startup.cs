using Deployr.Web.Attributes;
using Deployr.Web.DataAccess;
using Deployr.Web.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Deployr.Web
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IDataContextFactory, DataContextFactory>();
			services.AddSingleton<IDeployLogic, DeployLogic>();
			services.AddSingleton<ILogsLogic, LogsLogic>();
			services.AddControllers(config => config.Filters.Add(typeof(WebExceptionTransformerAttribute)));
			services.AddSwaggerGen();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();
			

			app.UseHttpsRedirection();

			app.UseSwagger();

			app.UseSwaggerUI(s =>
			{
				s.SwaggerEndpoint("/swagger/v1/swagger.json", "Deployr API");
				s.RoutePrefix = "docs";
			});

			//Accept All HTTP Request Methods from all origins
			app.UseCors(builder =>
				builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
