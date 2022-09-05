using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    public class ReadHoldingRegistersResponse : ModbusMessage
    {
        public ReadHoldingRegistersResponse()
        {
        }

        private const MessageType type = MessageType.ReadHoldingRegistersResponse;

        public static ReadHoldingRegistersResponse Decode(byte[] message, ILogger logger = null)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            try
            {
                MbapHeader header = MbapHeader.Decode(message);
                int index = 7;

                return new ReadHoldingRegistersResponse()
                {
                    SlaveAddress = header.UnitId,
                    Protocol = ProtocolType.TCP,
                    Header = header,
                    FunctionCode = message[index++],
                    ByteCount = message[index++],
                    RegisterValues = GetValues(message, index++, message[index - 1])
                };
            }
            catch (ModbusTcpException ex)
            {
                logger?.LogDebug(ex, "Modbus TCP header read fault.");
                byte[] data = new byte[message.Length - 2];
                Buffer.BlockCopy(message, 0, data, 0, data.Length);
                byte[] checkSum = Crc.Compute(data);

                if (message[message.Length - 2] != checkSum[0] || message[message.Length - 1] != checkSum[1])
                    throw new CheckSumMismatchException("Check sum mismatch.");

                int index = 0;

                return new ReadHoldingRegistersResponse()
                {
                    SlaveAddress = message[index++],
                    FunctionCode = message[index++],
                    ByteCount = message[index++],
                    RegisterValues = GetValues(message, index, message[index - 1]),
                    CheckSum = Convert.ToBase64String(checkSum),
                    Protocol = ProtocolType.RTU
                };
            }
        }

        public static ReadHoldingRegistersResponse Decode(string message)
        {
            ReadHoldingRegistersResponse response = JsonSerializer.Deserialize<ReadHoldingRegistersResponse>(message);
            byte[] msg = response.Encode();
            return Decode(msg);
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

        [JsonPropertyName("byteCount")]
        public byte ByteCount { get; set; }

        [JsonPropertyName("registerValues")]
        public ushort[] RegisterValues { get; set; }

        [JsonPropertyName("checkSum")]
        public string CheckSum { get; set; }

        public override byte[] Encode()
        {
            return Protocol == ProtocolType.TCP ? EncodeTcp() : EncodeRtu();
        }

        public override byte[] ConvertToRtu()
        {
            List<byte> byteArray = new List<byte>
            {
                SlaveAddress,
                FunctionCode,
                ByteCount
            };

            foreach (var item in RegisterValues)
            {
                byteArray.Add((byte)((item >> 8) & 0x00FF));
                byteArray.Add((byte)(item & 0x00FF));
            }

            byte[] crc = Crc.Compute(byteArray.ToArray());
            byte[] message = new byte[byteArray.Count + crc.Length];
            Buffer.BlockCopy(byteArray.ToArray(), 0, message, 0, byteArray.Count);
            Buffer.BlockCopy(crc, 0, message, byteArray.Count, crc.Length);
            return message;
        }

        public override byte[] ConvertToTcp(byte unitId, ushort transactionId, ushort protocolId = 0)
        {
            List<byte> list = new List<byte>
            {
                FunctionCode,
                ByteCount
            };
            foreach (var item in RegisterValues)
            {
                list.Add((byte)((item >> 8) & 0x00FF));
                list.Add((byte)(item & 0x00FF));
            }

            MbapHeader header = new MbapHeader() { UnitId = unitId, TransactionId = transactionId, ProtocolId = protocolId, Length = (ushort)(list.Count + 1) };
            byte[] headerBytes = header.Encode();
            byte[] message = new byte[headerBytes.Length + list.Count];
            Buffer.BlockCopy(headerBytes, 0, message, 0, headerBytes.Length);
            Buffer.BlockCopy(list.ToArray(), 0, message, headerBytes.Length, list.Count);
            return message;
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
                ByteCount
            };
            foreach (var item in RegisterValues)
            {
                frames.Add((byte)((item >> 8) & 0x00FF));
                frames.Add((byte)(item & 0x00FF));
            }
            Header.Length = (ushort)(frames.Count + 1);
            byte[] header = Header.Encode();
            byte[] message = new byte[header.Length + frames.Count];
            Buffer.BlockCopy(header, 0, message, 0, header.Length);
            Buffer.BlockCopy(frames.ToArray(), 0, message, header.Length, frames.Count);

            return message;
        }

        private byte[] EncodeRtu()
        {
            List<byte> frames = new List<byte>
            {
                SlaveAddress,
                FunctionCode,
                ByteCount
            };

            foreach (var item in RegisterValues)
            {
                frames.Add((byte)((item >> 8) & 0x00FF));
                frames.Add((byte)(item & 0x00FF));
            }

            byte[] crc = Crc.Compute(frames.ToArray());
            byte[] message = new byte[frames.Count + crc.Length];
            Buffer.BlockCopy(frames.ToArray(), 0, message, 0, frames.Count);
            Buffer.BlockCopy(crc, 0, message, frames.Count, crc.Length);

            return message;
        }

        private static byte[] GetBitBlock(byte[] message, int index, byte byteCount)
        {
            byte[] block = new byte[byteCount];
            Buffer.BlockCopy(message, index, block, 0, byteCount);

            return block;
        }

        private static ushort[] GetValues(byte[] message, int index, byte byteCount)
        {
            byte[] block = GetBitBlock(message, index, byteCount);
            int i = 0;
            List<ushort> list = new List<ushort>();
            while (i < block.Length)
            {
                list.Add((ushort)(message[index++] << 0x08 | message[index++]));
                i = i + 2;
            }

            return list.ToArray();
        }
    }
}
