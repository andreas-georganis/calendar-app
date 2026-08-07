namespace CalendarApp.API.ViewOnly;

internal sealed class ViewOnlyPropertyMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ViewOnlyPropertyException ex) when (!context.Response.HasStarted)
        {
            var errors = new Dictionary<string, string[]>
            {
                [ex.PropertyName] = [$"The property '{ex.PropertyName}' is view-only and cannot be set by the client."]
            };

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await TypedResults.ValidationProblem(
                    errors,
                    title: "Invalid request body")
                .ExecuteAsync(context);
        }
    }
}
