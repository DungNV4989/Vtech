
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VTECHERP.Domain.Shared.Configurations.DateTimeConverters
{
    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        public CustomDateTimeConverter()
        { }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value == null)
            {
                return DateTime.MinValue;
            }
            return DateTime.Parse(value);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            writer.WriteStringValue(value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
        }
    }
}
