using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Json
{
    public class CnpjConverter : JsonConverter<CNPJ>
    {
        public override CNPJ Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException("CNPJ cannot be null. Use CNPJ? for nullable.");
            return new(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, CNPJ value, JsonSerializerOptions options) =>
            writer.WriteStringValue((string)value);
    }
}
