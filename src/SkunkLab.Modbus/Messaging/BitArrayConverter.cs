using System;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    public class BitArrayConverter : JsonConverter<BitArray>
    {
        public override BitArray Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var bytes = JsonSerializer.Deserialize<bool[]>(ref reader, options);
            return bytes == null ? null : new BitArray(bytes);
        }

        public override void Write(Utf8JsonWriter writer, BitArray value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
