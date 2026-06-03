using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Json
{
    public class CardNumberConverter : JsonConverter<CardNumber>
    {
        public override CardNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException("CardNumber cannot be null. Use CardNumber? for nullable.");
            return new(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, CardNumber value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value);
    }
}
