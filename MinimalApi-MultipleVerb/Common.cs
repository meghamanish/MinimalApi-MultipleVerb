namespace MinimalApi_MultipleVerb
{
	public record Fruit1(string Name, int Stock)
	{
		public static readonly Dictionary<string, Fruit1> All = new();
	}

	public class Handlers
	{
		public void ReplaceFruit(string id, Fruit1 fruit) 
		{
			Fruit1.All[id] = fruit;
		}

		public static void AddFruit(string id, Fruit1 fruit)
		{
			Fruit1.All.Add(id, fruit);
		}

		public static async Task<IResult> AddFruit2(string id, Fruit1 fruit)
		{
			Fruit1.All.Add(id, fruit);
			return await Task.FromResult(TypedResults.Ok(fruit));
		}
	}

	public class IdValidationFilter : IEndpointFilter
	{
		public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
		{
			var id = context.GetArgument<string>(0);
			if (string.IsNullOrEmpty(id) || !id.StartsWith("f"))
			{
				return Results.ValidationProblem(new Dictionary<string, string[]>
					{
						{"id", new[] {"Invalid format. Id must start with 'f'"} }
					});
			}

			return await next(context);
		}
	}
}
