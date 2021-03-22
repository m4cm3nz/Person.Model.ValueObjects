using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Person.Model.ValueObjects.Json
{
    public class MonthOfYearConverter : JsonConverter<MonthOfYear>
    {
        public override MonthOfYear Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new MonthOfYear(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, MonthOfYear value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
