using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Json
{
    public class CardNumberConverter : JsonConverter<CardNumber>
    {
        public override bool HandleNull => true;

        public override CardNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException("CardNumber cannot be null. Use CardNumber? for nullable.");
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected a JSON string for CardNumber, got {reader.TokenType}.");
            return new(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, CardNumber value, JsonSerializerOptions options)
        {
            var raw = (string)value;
            if (raw is null)
                throw new JsonException("Cannot serialize a default (uninitialized) CardNumber.");
            writer.WriteStringValue(raw);
        }
    }
}
