using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Json
{
    public class MobileConverter : JsonConverter<Mobile>
    {
        public override bool HandleNull => true;

        public override Mobile Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException("Mobile cannot be null. Use Mobile? for nullable.");
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected a JSON string for Mobile, got {reader.TokenType}.");
            return new(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, Mobile value, JsonSerializerOptions options)
        {
            var raw = (string)value;
            if (raw is null)
                throw new JsonException("Cannot serialize a default (uninitialized) Mobile.");
            writer.WriteStringValue(raw);
        }
    }
}
