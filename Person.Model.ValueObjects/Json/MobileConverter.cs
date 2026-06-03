using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Json
{
    public class MobileConverter : JsonConverter<Mobile>
    {
        public override Mobile Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                throw new JsonException("Mobile cannot be null. Use Mobile? for nullable.");
            return PhoneNumberFactory.CreateMobile(ref reader);
        }

        public override void Write(Utf8JsonWriter writer, Mobile value, JsonSerializerOptions options) =>
            PhoneNumberFactory.WriteString(writer, value.Raw);
    }
}
