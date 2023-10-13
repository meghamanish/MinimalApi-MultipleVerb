using MinimalApi_MultipleVerb;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup2(builder.Configuration);
startup.ConfigureServices(builder.Services); // calling ConfigureServices method
builder.Services.AddProblemDetails();
var app = builder.Build();
startup.Configure(app, builder.Environment); // calling Configure method


