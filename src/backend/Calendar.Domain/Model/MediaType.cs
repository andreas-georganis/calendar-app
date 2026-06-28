using System.Net.Mime;

namespace Calendar.Domain.Model;

public sealed record MediaType
{
    private readonly ContentType _contentType;

    public MediaType(string mediaType)
    {
        _contentType = new ContentType(mediaType);
        //var _ = MediaTypeHeaderValue.Parse(mediaType);
    }
    
    public string Value => _contentType.MediaType;

}
