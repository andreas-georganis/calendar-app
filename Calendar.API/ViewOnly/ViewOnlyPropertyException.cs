using System.Text.Json;

namespace CalendarApp.API.ViewOnly;

internal sealed class ViewOnlyPropertyException(string propertyName)
    : JsonException($"The property '{propertyName}' is view-only and cannot be set by the client.")
{
    public string PropertyName { get; } = propertyName;
}
