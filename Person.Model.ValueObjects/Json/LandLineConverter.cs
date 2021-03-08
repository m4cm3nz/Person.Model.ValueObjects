using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Json
{
    public class LandLineConverter : JsonConverter<LandLine?>
    {
        public override LandLine? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            PhoneNumberFactory.CreateLandLine(ref reader);

        public override void Write(Utf8JsonWriter writer, LandLine? value, JsonSerializerOptions options) =>
            PhoneNumberFactory.Write(writer, value);
    }
}
