using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameStore.API.Attributes;

public class CaseInsensitiveEnumListConverter<T> : JsonConverter<List<T>>
    where T : struct, Enum
{
    public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new List<T>();

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected start of array");
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Expected string");
            }

            var stringValue = reader.GetString();
            if (!Enum.TryParse<T>(stringValue, ignoreCase: true, out var parsed))
            {
                throw new JsonException($"Invalid value '{stringValue}' for enum '{typeof(T).Name}'");
            }

            result.Add(parsed);
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var item in value)
        {
            writer.WriteStringValue(item.ToString().ToLowerInvariant());
        }

        writer.WriteEndArray();
    }
}
