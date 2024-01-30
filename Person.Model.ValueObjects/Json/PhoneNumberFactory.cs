using System.Text.Json;

namespace Person.Model.ValueObjects.Json
{
    public class PhoneNumberFactory
    {
        public static Mobile CreateMobile(ref Utf8JsonReader reader)
        {
            return new Mobile(Create(ref reader));
        }
        public static LandLine? CreateLandLine(ref Utf8JsonReader reader)
        {
            return new LandLine?(Create(ref reader));
        }

        public static void Write(Utf8JsonWriter writer, IPhoneNumber value)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(value.Raw), value.Raw);
            writer.WriteString(nameof(value.CountryCode), value.CountryCode);
            writer.WriteString(nameof(value.AreaCode), value.AreaCode);
            writer.WriteString(nameof(value.Number), value.Number);
            writer.WriteEndObject();
        }

        static string Create(ref Utf8JsonReader reader)
        {
            string phoneNumber = null;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.EndObject:
                        return phoneNumber;

                    case JsonTokenType.PropertyName:
                        if (reader.GetString() == "Raw")
                        {
                            reader.Read();
                            phoneNumber = reader.GetString();
                        }
                        break;
                }
            }

            throw new JsonException();
        }
    }
}
