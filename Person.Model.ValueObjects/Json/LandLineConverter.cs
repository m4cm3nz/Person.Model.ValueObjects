using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Json
{
    // HandleNull defaults to false: STJ returns null for LandLine? without calling Read,
    // and skips Write for null LandLine? values — no null-handling needed here.
    public class LandLineConverter : JsonConverter<LandLine>
    {
        public override LandLine Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected a JSON string for LandLine, got {reader.TokenType}.");
            return new(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, LandLine value, JsonSerializerOptions options)
        {
            var raw = (string)value;
            if (raw is null)
                throw new JsonException("Cannot serialize a default (uninitialized) LandLine.");
            writer.WriteStringValue(raw);
        }
    }
}
