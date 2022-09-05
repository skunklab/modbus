using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    [Serializable]

    public class WriteMultipleCoils : ModbusMessage
    {

        public WriteMultipleCoils()
        {
        }


        private const MessageType type = MessageType.WriteMultipleCoils;

        public static WriteMultipleCoils Create(byte slaveId, ushort startingAddress, BitArray coilValues)
        {
            WriteMultipleCoils request = new WriteMultipleCoils()
            {
                SlaveAddress = slaveId,
                FunctionCode = 15,
                StartingAddress = startingAddress,
                QuantityOfCoils = (ushort)coilValues.Length,
                ByteCount = (coilValues.Length % 8) == 0 ? (byte)(coilValues.Length / 8) : (byte)((coilValues.Length / 8) + 1),
                Data = coilValues,
                Protocol = ProtocolType.RTU
            };

            byte[] rtuEncoded = request.Encode();
            return WriteMultipleCoils.Decode(rtuEncoded);
        }

        public static WriteMultipleCoils Create(byte unitId, ushort transactionId, ushort protocolId, ushort startingAddress, BitArray coilValues)
        {
            WriteMultipleCoils request = new WriteMultipleCoils()
            {
                Header = new MbapHeader() { ProtocolId = protocolId, TransactionId = transactionId, UnitId = unitId },
                SlaveAddress = unitId,
                FunctionCode = 15,
                StartingAddress = startingAddress,
                QuantityOfCoils = (ushort)coilValues.Length,
                ByteCount = (coilValues.Length % 8) == 0 ? (byte)(coilValues.Length / 8) : (byte)((coilValues.Length / 8) + 1),
                Data = coilValues,
                Protocol = ProtocolType.TCP
            };

            byte[] encoded = request.Encode();
            return WriteMultipleCoils.Decode(encoded);
        }

        public static WriteMultipleCoils Decode(string message)
        {
            WriteMultipleCoils response = JsonSerializer.Deserialize<WriteMultipleCoils>(message);
            byte[] msg = response.Encode();
            return Decode(msg);
        }

        public static WriteMultipleCoils Decode(byte[] message, ILogger logger = null)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            try
            {
                MbapHeader header = MbapHeader.Decode(message);
                int index = 7;
                return new WriteMultipleCoils()
                {
                    Header = header,
                    SlaveAddress = header.UnitId,
                    FunctionCode = message[index++],
                    StartingAddress = (ushort)(message[index++] << 0x08 | message[index++]),
                    QuantityOfCoils = (ushort)(message[index++] << 0x08 | message[index++]),
                    ByteCount = message[index++],
                    Data = GetValues(index, message[index - 1], message, (ushort)(message[index - 3] << 0x08 | message[index - 2])),
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

                return new WriteMultipleCoils()
                {
                    SlaveAddress = message[index++],
                    FunctionCode = message[index++],
                    StartingAddress = (ushort)(message[index++] << 0x08 | message[index++]),
                    QuantityOfCoils = (ushort)(message[index++] << 0x08 | message[index++]),
                    ByteCount = message[index++],
                    Data = GetValues(index, message[index - 1], message, (ushort)(message[index - 3] << 0x08 | message[index - 2])),
                    CheckSum = Convert.ToBase64String(checkSum),
                    Protocol = ProtocolType.RTU
                };
            }
        }

        private static BitArray GetValues(int index, byte byteCount, byte[] message, ushort length)
        {
            byte[] block = new byte[byteCount];
            Buffer.BlockCopy(message, index, block, 0, block.Length);
            BitArray array = new BitArray(block)
            {
                Length = length
            };
            return array;
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

        [JsonPropertyName("quantityOfCoils")]
        public ushort QuantityOfCoils { get; set; }

        [JsonPropertyName("byteCount")]
        public byte ByteCount { get; set; }

        [JsonPropertyName("data")]
        [JsonConverter(typeof(BitArrayConverter))]
        public BitArray Data { get; set; }

        [JsonPropertyName("checkSum")]
        public string CheckSum { get; set; }

        public override byte[] Encode()
        {
            return Protocol == ProtocolType.TCP ? EncodeTcp() : EncodeRtu();
        }

        public override byte[] ConvertToRtu()
        {
            List<byte> frames = new List<byte>
            {
                SlaveAddress,
                FunctionCode,
                (byte)((StartingAddress >> 8) & 0x00FF),
                (byte)(StartingAddress & 0x00FF),
                (byte)((QuantityOfCoils >> 8) & 0x00FF),
                (byte)(QuantityOfCoils & 0x00FF),
                ByteCount
            };

            byte[] data = new byte[ByteCount];
            Data.CopyTo(data, 0);

            foreach (var item in data)
            {
                frames.Add(item);
            }

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
                (byte)((QuantityOfCoils >> 8) & 0x00FF),
                (byte)(QuantityOfCoils & 0x00FF),
                ByteCount
            };

            byte[] data = new byte[ByteCount];
            Data.CopyTo(data, 0);

            foreach (var item in data)
            {
                frames.Add(item);
            }

            MbapHeader header = new MbapHeader() { UnitId = unitId, TransactionId = transactionId, ProtocolId = protocolId, Length = (ushort)(frames.Count + 1) };
            byte[] headerBytes = header.Encode();
            byte[] message = new byte[headerBytes.Length + frames.Count];
            Buffer.BlockCopy(headerBytes, 0, message, 0, headerBytes.Length);
            Buffer.BlockCopy(frames.ToArray(), 0, message, headerBytes.Length, frames.Count);

            return message;
        }


        public override string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        private byte[] EncodeRtu()
        {
            List<byte> list = new List<byte>
            {
                SlaveAddress,
                FunctionCode,
                (byte)((StartingAddress >> 8) & 0x00FF),
                (byte)(StartingAddress & 0x00FF),
                (byte)((QuantityOfCoils >> 8) & 0x00FF),
                (byte)(QuantityOfCoils & 0x00FF),
                ByteCount
            };

            byte[] data = new byte[ByteCount];
            Data.CopyTo(data, 0);

            foreach (var item in data)
            {
                list.Add(item);
            }

            byte[] crc = Crc.Compute(list.ToArray());
            byte[] message = new byte[list.Count + crc.Length];
            Buffer.BlockCopy(list.ToArray(), 0, message, 0, list.Count);
            Buffer.BlockCopy(crc, 0, message, list.Count, crc.Length);

            return message;
        }

        private byte[] EncodeTcp()
        {
            List<byte> frames = new List<byte>
            {
                FunctionCode,
                (byte)((StartingAddress >> 8) & 0x00FF),//MSB
                (byte)(StartingAddress & 0x00FF),//LSB
                (byte)((QuantityOfCoils >> 8) & 0x00FF), //MSB
                (byte)(QuantityOfCoils & 0x00FF), //LSB  
                ByteCount
            };

            byte[] data = new byte[ByteCount];
            Data.CopyTo(data, 0);

            foreach (var item in data)
            {
                frames.Add(item);
            }

            Header.Length = (ushort)(frames.Count + 1);
            byte[] header = Header.Encode();
            byte[] message = new byte[header.Length + frames.Count];
            Buffer.BlockCopy(header, 0, message, 0, header.Length);
            Buffer.BlockCopy(frames.ToArray(), 0, message, header.Length, frames.Count);

            return message;
        }


    }
}
