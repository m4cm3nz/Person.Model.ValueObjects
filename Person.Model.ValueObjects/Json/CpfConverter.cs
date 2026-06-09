using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Json
{
    public class CpfConverter : JsonConverter<CPF>
    {
        public override bool HandleNull => true;

        public override CPF Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException("CPF cannot be null. Use CPF? for nullable.");
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected a JSON string for CPF, got {reader.TokenType}.");
            return new(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, CPF value, JsonSerializerOptions options)
        {
            var raw = (string)value;
            if (raw is null)
                throw new JsonException("Cannot serialize a default (uninitialized) CPF.");
            writer.WriteStringValue(raw);
        }
    }
}
