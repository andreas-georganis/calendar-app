using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using NodaTime;

namespace Calendar.API.OpenApi;

public class NodaTimeTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type == typeof(LocalDate))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "date";
            schema.Description = "ISO-8601 date pattern yyyy'-'MM'-'dd";
        }

        if (context.JsonTypeInfo.Type == typeof(Instant))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "date-time";
            schema.Description = "an ISO-8601 pattern extended to handle fractional seconds: yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFFF'Z'";
        }
        
        if (context.JsonTypeInfo.Type == typeof(LocalDateTime))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "date-time";
            schema.Description = "SO-8601 date/time pattern with no time zone specifier, extended to handle fractional seconds: yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFFF";
        }
        
        if (context.JsonTypeInfo.Type == typeof(DateTimeZone))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "timezone-id";
            schema.Description = "The ID - IANA time zone identifier, e.g. 'America/New_York'- , as a string.";
        }
        
        if (context.JsonTypeInfo.Type == typeof(Duration))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "duration";
            schema.Description = "A duration pattern of -H:mm:ss.FFFFFFFFF (like the standard round-trip pattern, but starting with hours instead of days)";
        }
        
        if (context.JsonTypeInfo.Type == typeof(Period))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "period";
            schema.Description = "The round-trip period pattern;a period pattern of PnYnMnDTnHnMnS";
        }
        
        if (context.JsonTypeInfo.Type == typeof(OffsetDateTime))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "date-time";
            schema.Description = "RFC3339 pattern: yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<Z+HH:mm>; note that the offset always includes hours and minutes, to conform with ECMA-262. It does not support round-tripping offsets with sub-minute components.";
        }
        
        if (context.JsonTypeInfo.Type == typeof(ZonedDateTime))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = "date-time";
            schema.Description = "As OffsetDateTime, but with a time zone ID at the end: uuuu'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFFo<G> z";
        }
        
        return Task.CompletedTask;
    }
}
