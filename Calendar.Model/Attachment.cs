
namespace Calendar.API.Model;

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
