using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Concurrent;
using System.Data.Common;

namespace MinimalApi_MultipleVerb
{
	record Fruit2(string Name, int stock);

	public class Startup2
	{
		public IConfiguration configRoot
		{
			get;
		}
		public Startup2(IConfiguration configuration)
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

			app.Use(async (context, next) =>
			{
				if (context.Request.Path.Value.Contains("invalid"))
					throw new Exception("ERROR");

				await next();
			}

			);
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();
			//app.UseAuthorization();
			//app.MapRazorPages();

			/*
				#A Using a concurrent dictionary to make the API thread-safe
				#B Try to get the fruit from the dictionary. If the ID exists in the dictionary this returns true…
				#C … and we return a 200 OK response, serializing the fruit in the body as JSON
				#D If the ID does not exist, we return a 404 Not Found response.
				#E Try to add the fruit to the dictionary. If the ID has not yet been added this returns true…
				#F … and we return a 201 response with a JSON body, and set the Location header to the given path
				#G If the ID already exists, we return a 400 Bad Request response with an error message
				#H After adding or replacing the fruit, we return a 204 No Content response.
				#I After deleting the fruit, always return a 204 No Content response.
			 */

			var _fruits = new ConcurrentDictionary<string, Fruit2>();

			//Lambda expressions are the simplest but least descriptive way to create a handler
			app.MapGet("/fruit", () => _fruits); //--> endpoint without a parameter --> returns all fruits

			//app.MapGet("/fruit/{id1}", (string id) => _fruits.TryGetValue(id, out var fruit2)
			//							? TypedResults.Ok(fruit2)
			//							: Results.NotFound()); //--> endpoint with 1 parameter (id) --> returns desired fruit.

			//validation using endpointfilter to avoid clogging.
			//app.MapGet("/fruit/{id}", (string id) => _fruits.TryGetValue(id, out var fruit2)
			//							? TypedResults.Ok(fruit2)
			//							: Results.Problem(statusCode: 404))

			////using validations
			//app.MapGet("/fruit/{id}", (string id) =>
			//{
			//	if (string.IsNullOrEmpty(id) || !id.StartsWith("f"))
			//	{
			//		return Results.ValidationProblem(new Dictionary<string, string[]>
			//		{
			//			{"id", new[] {"Invalid format. Id must start with 'f'"} }
			//		});
			//	}
			//	return _fruits.TryGetValue(id, out var fruit2)
			//							? TypedResults.Ok(fruit2)
			//							: Results.Problem(statusCode: 404);
			//}); //--> endpoint with 1 parameter (id) --> returns desired fruit.

			//validation using endpointfilter to avoid clogging.
			//app.MapGet("/fruit/{id}", (string id) => _fruits.TryGetValue(id, out var fruit2)
			//							? TypedResults.Ok(fruit2)
			//							: Results.Problem(statusCode: 404))
			//	.AddEndpointFilter(ValidationHelper.ValidateId);

			//app.MapGet("/fruit/{id}", (string id) => _fruits.TryGetValue(id, out var fruit2)
			//							? TypedResults.Ok(fruit2)
			//							: Results.Problem(statusCode: 404))
			//	.AddEndpointFilter(ValidationHelper.ValidateId)
			//	.AddEndpointFilter(async (context, next) =>
			//	{
			//		app.Logger.LogInformation("Executing filter...");
			//		var result = await next(context);
			//		app.Logger.LogInformation($"Handler result: {result}");
			//		return result;
			//	});

			app.MapGet("/fruit/{id}", (string id) => _fruits.TryGetValue(id, out var fruit2)
										? TypedResults.Ok(fruit2)
										: Results.Problem(statusCode: 404))
				.AddEndpointFilter<IdValidationFilter>();

			//var ok1 = Results.Ok(_fruits["f1"]);
			//var ok = TypedResults.Ok(_fruits["f2"]);

			//Handlers can be static methods in any class
			app.MapPost("/fruit/{id}", (string id, Fruit2 fruit) => _fruits.TryAdd(id, fruit)
										? TypedResults.Created($"/fruit/{id}", fruit)
										: Results.BadRequest(new { id = id,
											error = "A fruit with this id already exists" }));
			//up - endpoint with 2 parameters (id and fruit) --> add fruit to collection.

			app.MapPut("/fruit/{id}", (string id, Fruit2 fruit) =>
			{
				_fruits[id] = fruit;
				return Results.NoContent();
			});

			//  After deleting the fruit, always return a 204 No Content response.
			app.MapDelete("/fruit/{id}", (string id) =>
			{
				_fruits.TryRemove(id, out _);
				return Results.NoContent();
			});

			//app.MapGet("/", void () => throw new Exception("Error"));

			app.Run();
		}
	}
}
