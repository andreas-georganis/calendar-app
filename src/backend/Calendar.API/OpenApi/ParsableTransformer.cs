using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Calendar.API.OpenApi;

public sealed class ParsableTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (IsIParsable(context.JsonTypeInfo.Type))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "string";
        }

        return Task.CompletedTask;
    }

    private static bool IsIParsable(Type type)
    {
        // Check if the type implements IParsable<T> for any T
        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IParsable<>));
    }
}