using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    [Serializable]

    public class ReadCoilsResponse : ModbusResponseMessage
    {
        private const MessageType type = MessageType.ReadCoils;

        public static new ReadCoilsResponse Decode(byte[] message, ILogger logger = null)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            try
            {
                MbapHeader header = MbapHeader.Decode(message);
                int index = 7;
                return new ReadCoilsResponse()
                {
                    Header = header,
                    SlaveAddress = header.UnitId,
                    Protocol = ProtocolType.TCP,
                    FunctionCode = message[index++],
                    ByteCount = message[index++],
                    Data = GetValues(index, message[index - 1], message)
                };
            }
            catch (ModbusTcpException ex)
            {
                logger?.LogDebug(ex, "Modbus TCP header read fault.");
                byte[] data = new byte[message.Length - 2];
                Buffer.BlockCopy(message, 0, data, 0, data.Length);
                byte[] checkSum = Crc.Compute(data);

                if (message[^2] != checkSum[0] || message[^1] != checkSum[1])
                    throw new CheckSumMismatchException("Check sum mismatch.");

                int index = 0;

                return new ReadCoilsResponse()
                {
                    SlaveAddress = message[index++],
                    FunctionCode = message[index++],
                    ByteCount = message[index++],
                    Data = GetValues(index, message[index - 1], message),  //GetBitBlock(message, index, message[index-1]),
                    CheckSum = Convert.ToBase64String(checkSum),
                    Protocol = ProtocolType.RTU
                };
            }
        }

        public static new ReadCoilsResponse Decode(string message)
        {
            return JsonSerializer.Deserialize<ReadCoilsResponse>(message);
        }

        private static BitArray GetValues(int index, byte byteCount, byte[] message)
        {
            byte[] block = new byte[byteCount];
            Buffer.BlockCopy(message, index, block, 0, block.Length);
            BitArray array = new(block);
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
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public override ProtocolType Protocol { get; set; }

        [JsonPropertyName("mbapHeader")]
        public override MbapHeader Header { get; set; }

        [JsonPropertyName("slaveAddress")]
        public override byte SlaveAddress { get; set; }

        [JsonPropertyName("function")]
        public override byte FunctionCode { get; set; }

        [JsonPropertyName("byteCount")]
        public byte ByteCount { get; set; }

        //[JsonPropertyName("bitBlock")]
        //public byte[] BitBlock { get; set; }

        [JsonPropertyName("data")]
        [JsonConverter(typeof(BitArrayConverter))]
        public BitArray Data { get; set; }

        [JsonPropertyName("checkSum")]
        public string CheckSum { get; set; }

        public override byte[] Encode()
        {
            return Protocol == ProtocolType.TCP ? EncodeTcp() : EncodeRtu();
        }

        public override string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        public override byte[] ConvertToRtu()
        {
            List<byte> byteArray = new()
            {
                SlaveAddress,
                FunctionCode,
                ByteCount
            };


            byte[] data = new byte[ByteCount];
            Data.CopyTo(data, 0);

            foreach (var item in data)
            {
                byteArray.Add(item);
            }
            //for (int i = 0; i < BitBlock.Length; i++)
            //    byteArray.Add(BitBlock[i]);


            byte[] crc = Crc.Compute(byteArray.ToArray());
            byte[] message = new byte[byteArray.Count + crc.Length];
            Buffer.BlockCopy(byteArray.ToArray(), 0, message, 0, byteArray.Count);
            Buffer.BlockCopy(crc, 0, message, byteArray.Count, crc.Length);
            return message;
        }

        public override byte[] ConvertToTcp(byte unitId, ushort transactionId, ushort protocolId = 0)
        {
            List<byte> list = new()
            {
                FunctionCode,
                ByteCount
            };

            byte[] data = new byte[ByteCount];
            Data.CopyTo(data, 0);

            foreach (var item in data)
            {
                list.Add(item);
            }

            //foreach(var item in BitBlock)
            //{
            //    list.Add(item);
            //}

            MbapHeader header = new() { UnitId = unitId, TransactionId = transactionId, ProtocolId = protocolId, Length = (ushort)(list.Count + 1) };
            byte[] headerBytes = header.Encode();
            byte[] message = new byte[headerBytes.Length + list.Count];
            Buffer.BlockCopy(headerBytes, 0, message, 0, headerBytes.Length);
            Buffer.BlockCopy(list.ToArray(), 0, message, headerBytes.Length, list.Count);
            return message;
        }

        private byte[] EncodeTcp()
        {
            List<byte> frames = new()
            {
                FunctionCode,
                ByteCount
            };

            byte[] data = new byte[ByteCount];
            Data.CopyTo(data, 0);

            foreach (var item in data)
            {
                frames.Add(item);
            }

            //foreach (var item in BitBlock)
            //{
            //    frames.Add(item);
            //}
            Header.Length = (ushort)(frames.Count + 1);
            byte[] header = Header.Encode();
            byte[] message = new byte[header.Length + frames.Count];
            Buffer.BlockCopy(header, 0, message, 0, header.Length);
            Buffer.BlockCopy(frames.ToArray(), 0, message, header.Length, frames.Count);

            return message;
        }

        private byte[] EncodeRtu()
        {
            List<byte> frames = new()
            {
                SlaveAddress,
                FunctionCode,
                ByteCount
            };

            byte[] data = new byte[ByteCount];
            Data.CopyTo(data, 0);

            foreach (var item in data)
            {
                frames.Add(item);
            }

            //for (int i = 0; i < ByteCount; i++)
            //{
            //    frames.Add(BitBlock[i]);
            //}

            byte[] crc = Crc.Compute(frames.ToArray());
            byte[] message = new byte[frames.Count + crc.Length];
            Buffer.BlockCopy(frames.ToArray(), 0, message, 0, frames.Count);
            Buffer.BlockCopy(crc, 0, message, frames.Count, crc.Length);

            return message;
        }

        //private static byte[] GetBitBlock(byte[] message, int index, byte byteCount)
        //{
        //    byte[] block = new byte[byteCount];
        //    Buffer.BlockCopy(message, index, block, 0, byteCount);
        //    return block;
        //}




    }
}
