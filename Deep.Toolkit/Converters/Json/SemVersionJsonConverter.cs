using System.Text.Json;
using System.Text.Json.Serialization;
using Semver;

namespace Deep.Toolkit.Converters.Json;

public class SemVersionJsonConverter : JsonConverter<SemVersion>
{
    public override SemVersion Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        return SemVersion.Parse(reader.GetString()!, SemVersionStyles.Strict);
    }

    public override void Write(
        Utf8JsonWriter writer,
        SemVersion value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}