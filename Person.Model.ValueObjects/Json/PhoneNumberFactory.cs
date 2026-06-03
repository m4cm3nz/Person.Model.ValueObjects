using System.Text.Json;

namespace Person.Model.ValueObjects.Json
{
    public class PhoneNumberFactory
    {
        public static Mobile CreateMobile(ref Utf8JsonReader reader) =>
            new(reader.GetString()!);

        public static LandLine? CreateLandLine(ref Utf8JsonReader reader)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            return new LandLine(reader.GetString()!);
        }

        public static void WriteString(Utf8JsonWriter writer, string raw) =>
            writer.WriteStringValue(raw);
    }
}
