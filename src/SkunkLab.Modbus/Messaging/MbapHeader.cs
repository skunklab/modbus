using System;
using System.Text.Json.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    [Serializable]

    public class MbapHeader
    {
        public MbapHeader()
        {
        }

        public static MbapHeader Decode(byte[] message)
        {
            if (message.Length < 7)
                throw new InvalidMbapHeaderException("MBAP header length less than 7 bytes.");

            MbapHeader header = new();

            int index = 0;

            header.TransactionId = (ushort)(message[index++] << 0x08 | message[index++]);
            header.ProtocolId = (ushort)(message[index++] << 0x08 | message[index++]);
            header.Length = (ushort)(message[index++] << 0x08 | message[index++]);
            header.UnitId = Convert.ToByte(message[index]);

            if (message.Length - 6 != header.Length)
                throw new MessageLengthMismatchException("MBAP header has invalid message length.");

            return header;
        }

        [JsonPropertyName("transactionId")]
        public ushort TransactionId { get; set; }

        [JsonPropertyName("protocolId")]
        public ushort ProtocolId { get; set; }

        [JsonPropertyName("unitId")]
        public byte UnitId { get; set; }

        [JsonPropertyName("length")]
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
