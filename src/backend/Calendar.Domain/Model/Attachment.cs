using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonPolymorphic]
[JsonDerivedType(typeof(UriAttachment))]
[JsonDerivedType(typeof(BinaryAttachment))]
public abstract record Attachment
{
    public MediaType? MediaType { get; }
    
    protected Attachment(MediaType? mediaType = null)
    {
        MediaType = mediaType;
    }

    public record UriAttachment(Uri Value): Attachment;
    
    public record BinaryAttachment(byte[] Value) : Attachment;
}
