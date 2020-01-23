using Newtonsoft.Json;
using System;

namespace SkunkLab.Modbus.Messaging
{
    [Serializable]
    [JsonObject]
    public class MbapHeader
    {
        public MbapHeader()
        {                
        }

        public static MbapHeader Decode(byte[] message)
        {
            if (message.Length < 7)
                throw new InvalidMbapHeaderException("MBAP header length less than 7 bytes.");

            MbapHeader header = new MbapHeader();

            int index = 0;

            header.TransactionId = (ushort)(message[index++] << 0x08 | message[index++]);
            header.ProtocolId = (ushort)(message[index++] << 0x08 | message[index++]);
            header.Length = (ushort)(message[index++] << 0x08 | message[index++]);
            header.UnitId = Convert.ToByte(message[index]);

            if (message.Length - 6 != header.Length)
                throw new MessageLengthMismatchException("MBAP header has invalid message length.");

            return header;
        }

        [JsonProperty("transactionId")]
        public ushort TransactionId { get; set; }

        [JsonProperty("protocolId")]
        public ushort ProtocolId { get; set; }

        [JsonProperty("unitId")]
        public byte UnitId { get; set; }

        [JsonProperty("length")]
        public ushort Length { get; set; }        

        public byte[] Encode()
        {
            int index = 0;
            byte[] header = new byte[7];
            header[index++] = (byte)((TransactionId >> 8) & 0x00FF); //MSB
            header[index++] = (byte)(TransactionId & 0x00FF); //LSB
            header[index++] = (byte)((ProtocolId >> 8) & 0x00FF); //MSB
            header[index++] = (byte)(ProtocolId & 0x00FF); //LSB
            header[index++] = (byte)((Length >> 8) & 0x00FF); //MSB
            header[index++] = (byte)(Length & 0x00FF); //LSB
            header[index++] = UnitId;

            return header;
        }
    }
}
