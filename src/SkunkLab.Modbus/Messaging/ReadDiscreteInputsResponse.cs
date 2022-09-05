using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    [Serializable]

    public class ReadDiscreteInputsResponse : ModbusMessage
    {
        public ReadDiscreteInputsResponse()
        {
        }

        private const MessageType type = MessageType.ReadDiscreteInputsResponse;

        public static ReadDiscreteInputsResponse Decode(byte[] message, ILogger logger = null)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            try
            {
                MbapHeader header = MbapHeader.Decode(message);
                int index = 7;
                return new ReadDiscreteInputsResponse()
                {
                    Header = header,
                    SlaveAddress = header.UnitId,
                    Protocol = ProtocolType.TCP,
                    FunctionCode = message[index++],
                    ByteCount = message[index++],
                    BitBlock = GetBitBlock(message, index, message[index - 1])
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

                return new ReadDiscreteInputsResponse()
                {
                    SlaveAddress = message[index++],
                    FunctionCode = message[index++],
                    ByteCount = message[index++],
                    BitBlock = GetBitBlock(message, index, message[index - 1]),
                    CheckSum = Convert.ToBase64String(checkSum),
                    Protocol = ProtocolType.RTU
                };
            }
        }

        public static ReadDiscreteInputsResponse Decode(string message)
        {
            ReadDiscreteInputsResponse response = JsonSerializer.Deserialize<ReadDiscreteInputsResponse>(message);
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

        [JsonPropertyName("bitBlock")]
        public byte[] BitBlock { get; set; }

        [JsonIgnore]
        public BitArray InputStatus
        {
            get { return new BitArray(BitBlock); }
        }

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

            for (int i = 0; i < BitBlock.Length; i++)
                byteArray.Add(BitBlock[i]);


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
            foreach (var item in BitBlock)
            {
                list.Add(item);
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
            //int index = 0;
            //byte[] header = Header.Encode();
            //byte[] frames = new byte[Header.Length - 1];
            //frames[index++] = FunctionCode;
            //frames[index++] = ByteCount;
            //foreach (var item in BitBlock)
            //{
            //    frames[index++] = item;
            //}

            //byte[] message = new byte[header.Length + frames.Length];
            //Buffer.BlockCopy(header, 0, message, 0, header.Length);
            //Buffer.BlockCopy(frames, 0, message, header.Length, frames.Length);

            //return message;

            List<byte> frames = new List<byte>
            {
                FunctionCode,
                ByteCount
            };
            foreach (var item in BitBlock)
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

        private byte[] EncodeRtu()
        {
            List<byte> frames = new List<byte>
            {
                SlaveAddress,
                FunctionCode,
                ByteCount
            };

            for (int i = 0; i < ByteCount; i++)
            {
                frames.Add(BitBlock[i]);
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
    }
}
