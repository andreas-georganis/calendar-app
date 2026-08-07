using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<MediaType>))]
public readonly record struct MediaType : IParsable<MediaType>
{
    public static MediaType OctetStream { get; } = new("application/octet-stream");
    
    private readonly ContentType _contentType;

    //private readonly StringSegment _mediaType;

    public MediaType(string mediaType)
    {
        _contentType = new ContentType(mediaType);
        //_mediaType = new StringSegment(mediaType);
        //var _ = MediaTypeHeaderValue.Parse(mediaType);
    }
    
    public string Value => _contentType.MediaType;

    public static MediaType Parse(string s, IFormatProvider? provider)
    => TryParse(s, provider, out var result) ? result : throw new FormatException($"Invalid media type: '{s}'.");

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out MediaType result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = OctetStream;
            return true;
        }

        try
        {
            result = new MediaType(s);
            return true;
        }
        catch (FormatException)
        {
            result = default;
            return false;
        }
    }

    // public static ValueTask<MediaType> BindAsync(HttpContext context, ParameterInfo parameter)
    // {
    //     throw new NotImplementedException();
    // }
}
