using Newtonsoft.Json;
using System;
using System.Collections;

namespace SkunkLab.Modbus.Messaging
{
    public class BitArrayConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BitArray);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var bytes = serializer.Deserialize<bool[]>(reader);
            return bytes == null ? null : new BitArray(bytes);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, ((BitArray)value).BitArrayToBoolArray());
        }
    }
}
