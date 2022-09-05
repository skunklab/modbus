using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    [Serializable]

    public class WriteMultipleRegistersResponse : ModbusMessage
    {
        public WriteMultipleRegistersResponse()
        {
        }

        private const MessageType type = MessageType.WriteMultipleRegistersResponse;

        public static WriteMultipleRegistersResponse Decode(byte[] message, ILogger logger = null)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            try
            {
                MbapHeader header = MbapHeader.Decode(message);
                int index = 7;
                return new WriteMultipleRegistersResponse()
                {
                    Header = header,
                    SlaveAddress = header.UnitId,
                    FunctionCode = message[index++],
                    StartingAddress = (ushort)(message[index++] << 0x08 | message[index++]),
                    QuantityOfRegisters = (ushort)(message[index++] << 0x08 | message[index++]),
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

                return new WriteMultipleRegistersResponse()
                {
                    SlaveAddress = message[index++],
                    FunctionCode = message[index++],
                    StartingAddress = (ushort)(message[index++] << 0x08 | message[index++]),
                    QuantityOfRegisters = (ushort)(message[index++] << 0x08 | message[index++]),
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

        [JsonPropertyName("quantityOfRegisters")]
        public ushort QuantityOfRegisters { get; set; }

        [JsonPropertyName("checkSum")]
        public string CheckSum { get; set; }

        public override byte[] ConvertToRtu()
        {
            List<byte> frames = new List<byte>
            {
                SlaveAddress,
                FunctionCode,
                (byte)((StartingAddress >> 8) & 0x00FF),
                (byte)(StartingAddress & 0x00FF),
                (byte)((QuantityOfRegisters >> 8) & 0x00FF),
                (byte)(QuantityOfRegisters & 0x00FF)
            };

            byte[] crc = Crc.Compute(frames.ToArray());
            byte[] message = new byte[frames.Count + crc.Length];
            Buffer.BlockCopy(frames.ToArray(), 0, message, 0, frames.Count);
            Buffer.BlockCopy(crc, 0, message, frames.Count, crc.Length);

            return message;
        }

        public override byte[] ConvertToTcp(byte unitId, ushort transactionId, ushort protocolId = 0)
        {
            List<byte> frames = new List<byte>
            {
                FunctionCode,
                (byte)((StartingAddress >> 8) & 0x00FF),
                (byte)(StartingAddress & 0x00FF),
                (byte)((QuantityOfRegisters >> 8) & 0x00FF),
                (byte)(QuantityOfRegisters & 0x00FF)
            };

            MbapHeader header = new MbapHeader() { UnitId = unitId, TransactionId = transactionId, ProtocolId = protocolId, Length = (ushort)(frames.Count + 1) };
            byte[] headerBytes = header.Encode();
            byte[] message = new byte[headerBytes.Length + frames.Count];
            Buffer.BlockCopy(headerBytes, 0, message, 0, headerBytes.Length);
            Buffer.BlockCopy(frames.ToArray(), 0, message, headerBytes.Length, frames.Count);

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

        private byte[] EncodeTcp()
        {
            List<byte> frames = new List<byte>
            {
                FunctionCode,
                (byte)((StartingAddress >> 8) & 0x00FF),//MSB
                (byte)(StartingAddress & 0x00FF),//LSB
                (byte)((QuantityOfRegisters >> 8) & 0x00FF), //MSB
                (byte)(QuantityOfRegisters & 0x00FF) //LSB              
            };

            Header.Length = (ushort)(frames.Count + 1);
            byte[] header = Header.Encode();
            byte[] message = new byte[header.Length + frames.Count];
            Buffer.BlockCopy(header, 0, message, 0, header.Length);
            Buffer.BlockCopy(frames.ToArray(), 0, message, header.Length, frames.Count);

            return message;
        }

        private byte[] EncodeRtu()
        {
            List<byte> list = new List<byte>
            {
                SlaveAddress,
                FunctionCode,
                (byte)((StartingAddress >> 8) & 0x00FF),
                (byte)(StartingAddress & 0x00FF),
                (byte)((QuantityOfRegisters >> 8) & 0x00FF),
                (byte)(QuantityOfRegisters & 0x00FF)
            };


            byte[] crc = Crc.Compute(list.ToArray());
            byte[] message = new byte[list.Count + crc.Length];
            Buffer.BlockCopy(list.ToArray(), 0, message, 0, list.Count);
            Buffer.BlockCopy(crc, 0, message, list.Count, crc.Length);

            return message;
        }
    }
}
