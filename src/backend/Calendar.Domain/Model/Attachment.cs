
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

public sealed class Attachment
{
    public required Uri Uri { get; init; }

    public MediaType? MediaType { get; init; } 

}
