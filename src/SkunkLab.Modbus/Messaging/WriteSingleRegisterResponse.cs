using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    [Serializable]

    public class WriteSingleRegisterResponse : ModbusMessage
    {
        private const MessageType type = MessageType.WriteSingleRegisterResponse;


        public static WriteSingleRegisterResponse Decode(byte[] message, ILogger logger = null)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            try
            {
                MbapHeader header = MbapHeader.Decode(message);
                int index = 7;
                return new WriteSingleRegisterResponse()
                {
                    Header = header,
                    SlaveAddress = header.UnitId,
                    FunctionCode = message[index++],
                    StartingAddress = (ushort)(message[index++] << 0x08 | message[index++]),
                    Data = (ushort)(message[index++] << 0x08 | message[index++]),
                    Protocol = ProtocolType.TCP
                };
            }
            catch (Exception ex)
            {
                logger?.LogDebug(ex, "Modbus TCP header read fault.");
                byte[] data = new byte[message.Length - 2];
                Buffer.BlockCopy(message, 0, data, 0, data.Length);
                byte[] checkSum = Crc.Compute(data);

                if (message[message.Length - 2] != checkSum[0] || message[message.Length - 1] != checkSum[1])
                    throw new CheckSumMismatchException("Check sum mismatch.");
                int index = 0;

                return new WriteSingleRegisterResponse()
                {
                    SlaveAddress = message[index++],
                    FunctionCode = message[index++],
                    StartingAddress = (ushort)(message[index++] << 0x08 | message[index++]),
                    Data = (ushort)(message[index++] << 0x08 | message[index++]),
                    CheckSum = Convert.ToBase64String(checkSum),
                    Protocol = ProtocolType.RTU
                };
            }

        }

        public static WriteSingleRegisterResponse Decode(string message)
        {
            return JsonSerializer.Deserialize<WriteSingleRegisterResponse>(message);
        }

        [JsonPropertyName("messageType")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public override MessageType Type
        {
            get { return type; }
            set { }
        }

        [JsonPropertyName("protocol")]
        public override ProtocolType Protocol { get; set; }

        [JsonPropertyName("header")]
        public override MbapHeader Header { get; set; }

        [JsonPropertyName("slaveAddress")]
        public override byte SlaveAddress { get; set; }

        [JsonPropertyName("code")]
        public override byte FunctionCode { get; set; }

        [JsonPropertyName("startingAddress")]
        public ushort StartingAddress { get; set; }

        [JsonPropertyName("data")]
        public ushort Data { get; set; }

        [JsonPropertyName("checkSum")]
        public string CheckSum { get; set; }

        public override byte[] ConvertToRtu()
        {
            int index = 0;
            byte[] frames = new byte[6];
            frames[index++] = SlaveAddress;
            frames[index++] = FunctionCode;
            frames[index++] = (byte)((StartingAddress >> 8) & 0x00FF); //MSB
            frames[index++] = (byte)(StartingAddress & 0x00FF); //LSB
            frames[index++] = (byte)((Data >> 8) & 0x00FF); //MSB
            frames[index++] = (byte)(Data & 0x00FF); //LSB

            byte[] crc = Crc.Compute(frames);
            byte[] message = new byte[frames.Length + crc.Length];
            Buffer.BlockCopy(frames, 0, message, 0, frames.Length);
            Buffer.BlockCopy(crc, 0, message, frames.Length, crc.Length);
            return message;
        }

        public override byte[] ConvertToTcp(byte unitId, ushort transactionId, ushort protocolId = 0)
        {
            int index = 0;
            byte[] frames = new byte[5];
            frames[index++] = FunctionCode;
            frames[index++] = (byte)((StartingAddress >> 8) & 0x00FF); //MSB
            frames[index++] = (byte)(StartingAddress & 0x00FF); //LSB
            frames[index++] = (byte)((Data >> 8) & 0x00FF); //MSB
            frames[index++] = (byte)(Data & 0x00FF); //LSB
            MbapHeader header = new MbapHeader() { UnitId = unitId, TransactionId = transactionId, ProtocolId = protocolId, Length = (ushort)(frames.Length + 1) };
            byte[] headerBytes = header.Encode();
            byte[] message = new byte[headerBytes.Length + frames.Length];
            Buffer.BlockCopy(headerBytes, 0, message, 0, headerBytes.Length);
            Buffer.BlockCopy(frames, 0, message, headerBytes.Length, frames.Length);
            return message;
        }

        public override byte[] Encode()
        {
            return Protocol == ProtocolType.TCP ? EncodeTcp() : EncodeRtu();
        }

        public override string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        private byte[] EncodeRtu()
        {
            int index = 0;
            byte[] frames = new byte[6];
            frames[index++] = SlaveAddress;
            frames[index++] = FunctionCode;
            frames[index++] = (byte)((StartingAddress >> 8) & 0x00FF); //MSB
            frames[index++] = (byte)(StartingAddress & 0x00FF); //LSB
            frames[index++] = (byte)((Data >> 8) & 0x00FF); //MSB
            frames[index++] = (byte)(Data & 0x00FF); //LSB
            byte[] crc = Crc.Compute(frames);
            byte[] message = new byte[frames.Length + crc.Length];
            Buffer.BlockCopy(frames, 0, message, 0, frames.Length);
            Buffer.BlockCopy(crc, 0, message, frames.Length, crc.Length);

            return message;
        }

        private byte[] EncodeTcp()
        {
            List<byte> frames = new List<byte>
            {
                FunctionCode,
                (byte)((StartingAddress >> 8) & 0x00FF),//MSB
                (byte)(StartingAddress & 0x00FF),//LSB
                (byte)((Data >> 8) & 0x00FF), //MSB
                (byte)(Data & 0x00FF) //LSB                                                         
            };
            Header.Length = (ushort)(frames.Count + 1);
            byte[] header = Header.Encode();
            byte[] message = new byte[header.Length + frames.Count];
            Buffer.BlockCopy(header, 0, message, 0, header.Length);
            Buffer.BlockCopy(frames.ToArray(), 0, message, header.Length, frames.Count);

            return message;
        }
    }
}
