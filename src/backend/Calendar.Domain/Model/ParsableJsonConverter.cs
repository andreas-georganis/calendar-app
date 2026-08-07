using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace Calendar.Domain.Model;

internal sealed class ParsableJsonConverter<T> : JsonConverter<T> where T : IParsable<T>
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value is null)
        {
            return default;
        }

        return T.Parse(value, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
