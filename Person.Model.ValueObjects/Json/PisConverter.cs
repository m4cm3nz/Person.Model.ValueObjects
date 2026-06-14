using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Json
{
    public class PisConverter : JsonConverter<PIS>
    {
        public override bool HandleNull => true;

        public override PIS Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException("PIS cannot be null. Use PIS? for nullable.");
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected a JSON string for PIS, got {reader.TokenType}.");
            return new(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, PIS value, JsonSerializerOptions options)
        {
            var raw = (string)value;
            if (raw is null)
                throw new JsonException("Cannot serialize a default (uninitialized) PIS.");
            writer.WriteStringValue(raw);
        }
    }
}
