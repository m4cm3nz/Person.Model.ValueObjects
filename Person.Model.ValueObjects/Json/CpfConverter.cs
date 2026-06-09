using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Json
{
    public class CpfConverter : JsonConverter<CPF>
    {
        public override CPF Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException("CPF cannot be null. Use CPF? for nullable.");
            return new(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, CPF value, JsonSerializerOptions options) =>
            writer.WriteStringValue((string)value);
    }
}
