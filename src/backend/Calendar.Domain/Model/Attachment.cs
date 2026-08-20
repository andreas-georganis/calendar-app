
namespace Calendar.Domain.Model;

public sealed class Attachment
{
    public required Uri Uri { get; init; }

    public MediaType? MediaType { get; init; } 

}
