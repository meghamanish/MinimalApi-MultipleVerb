namespace MinimalApi_MultipleVerb
{
	public class ValidationHelper
	{
		internal static async ValueTask<object?> ValidateId(EndpointFilterInvocationContext context, 
			EndpointFilterDelegate next)
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
