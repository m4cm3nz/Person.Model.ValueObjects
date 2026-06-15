using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Json
{
    public class CnhConverter : JsonConverter<CNH>
    {
        public override bool HandleNull => true;

        public override CNH Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException("CNH cannot be null. Use CNH? for nullable.");
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected a JSON string for CNH, got {reader.TokenType}.");
            return new(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, CNH value, JsonSerializerOptions options)
        {
            var raw = (string)value;
            if (raw is null)
                throw new JsonException("Cannot serialize a default (uninitialized) CNH.");
            writer.WriteStringValue(raw);
        }
    }
}
