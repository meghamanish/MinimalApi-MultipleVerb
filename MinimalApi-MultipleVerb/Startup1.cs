using Microsoft.AspNetCore.Builder;

namespace MinimalApi_MultipleVerb
{
	public class Startup1
	{
		public IConfiguration configRoot
		{
			get;
		}
		public Startup1(IConfiguration configuration)
		{
			configRoot = configuration;
		}
		public void ConfigureServices(IServiceCollection services)
		{
			//services.AddRazorPages();
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
		}
		public void Configure(WebApplication app, IWebHostEnvironment env)
		{
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
			//app.UseAuthorization();
			//app.MapRazorPages();

			//Lambda expressions are the simplest but least descriptive way to create a handler
			app.MapGet("/fruit", () => Fruit1.All);

			//Storing the lambda expression as a variable means you can name it, getFruit in this case
			var getFruit = (string id) => Fruit1.All[id];
			app.MapGet("/fruit/{id}", getFruit);

			//Handlers can be static methods in any class
			app.MapPost("/fruit/{id}", Handlers.AddFruit);

			//Handlers can also be instance methods
			Handlers handlers = new();
			app.MapPut("/fruit/{id}", handlers.ReplaceFruit);

			// You can also use local functions, introduced in C# 7.0, as handler methods
			app.MapDelete("/fruit/{id}", DeleteFruit);
			
			app.Run();
		}

		void DeleteFruit(string id)
		{
			Fruit1.All.Remove(id);
		}
	}
}
