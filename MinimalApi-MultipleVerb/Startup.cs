using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Concurrent;
using System.Data.Common;

namespace MinimalApi_MultipleVerb
{
	record Fruit3(string Name, int stock);

	public class Startup
	{
		public IConfiguration configRoot
		{
			get;
		}
		public Startup(IConfiguration configuration)
		{
			configRoot = configuration;
		}
		public void ConfigureServices(IServiceCollection services)
		{
			//services.AddRazorPages();
			services.AddEndpointsApiExplorer();
			//services.AddHealthChecks();
			services.AddSwaggerGen();
		}
		public void Configure(WebApplication app, IWebHostEnvironment env)
		{
			if (!app.Environment.IsDevelopment())
			{
				//app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseExceptionHandler();
				app.UseHsts();
			}

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();

			/*
				#A Create a route group by calling MapGroup and providing a prefix
				#B Endpoints defined on the route group will have the group prefix prepended to the route
				#C You can create nested route groups with multiple prefixes
				#D You can add filters to the route group…
				#E …and the filter will be applied to all the endpoints defined on the route group.
			 */

			var _fruits = new ConcurrentDictionary<string, Fruit2>();

			RouteGroupBuilder fruitApi = app.MapGroup("/fruit"); //#A

			fruitApi.MapGet("/", () => _fruits); //#B

			var fruitApiWithValidation = fruitApi.MapGroup("/")		//#C
				.AddEndpointFilter(ValidationHelper.ValidateId);	//#D

			fruitApiWithValidation.MapGet("/{id}", (string id) => _fruits.TryGetValue(id, out var fruit2)	//#E
										? TypedResults.Ok(fruit2)
										: Results.Problem(statusCode: 404));

			app.Run();
		}
	}
}
