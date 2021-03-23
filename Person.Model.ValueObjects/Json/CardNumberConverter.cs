using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Json
{
    public class CardNumberConverter : JsonConverter<CardNumber>
    {
        public override CardNumber Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new CardNumber(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, CardNumber value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
