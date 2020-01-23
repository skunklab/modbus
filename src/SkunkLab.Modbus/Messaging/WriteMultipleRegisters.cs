using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkunkLab.Modbus.Messaging
{
    [Serializable]
    [JsonObject]
    public class WriteMultipleRegisters : ModbusMessage
    {
        public WriteMultipleRegisters()
        {
        }

        private const MessageType type = MessageType.WriteMultpleRegisters;

        public static WriteMultipleRegisters Create(byte slaveAddress, ushort startingAddress, ushort[] data)
        {
            WriteMultipleRegisters request = new WriteMultipleRegisters()
            {
                SlaveAddress = slaveAddress,
                FunctionCode = 16,
                StartingAddress = startingAddress,
                QuantityOfRegisters = (ushort)(data.Length),
                ByteCount = (byte)(data.Length * 2),
                Data = data,
                Protocol = ProtocolType.RTU
            };

            byte[] rtuEncoded = request.Encode();
            return WriteMultipleRegisters.Decode(rtuEncoded);
        }

        public static WriteMultipleRegisters Create(byte unitId, ushort transactionId, ushort protocolId, ushort startingAddress, ushort[] data)
        {
            WriteMultipleRegisters request = new WriteMultipleRegisters()
            {
                Header = new MbapHeader() { ProtocolId = protocolId, TransactionId = transactionId, UnitId = unitId },
                SlaveAddress = unitId,
                FunctionCode = 16,
                StartingAddress = startingAddress,
                QuantityOfRegisters = (ushort)(data.Length),
                ByteCount = (byte)(data.Length * 2),
                Data = data,
                Protocol = ProtocolType.TCP
            };

            byte[] encoded = request.Encode();
            return WriteMultipleRegisters.Decode(encoded);
        }

        public static WriteMultipleRegisters Decode(byte[] message, ILogger logger = null)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            try
            {
                MbapHeader header = MbapHeader.Decode(message);
                int index = 7;
                return new WriteMultipleRegisters()
                {
                    Header = header,
                    SlaveAddress = header.UnitId,
                    FunctionCode = message[index++],
                    StartingAddress = (ushort)(message[index++] << 0x08 | message[index++]),
                    QuantityOfRegisters = (ushort)(message[index++] << 0x08 | message[index++]),
                    ByteCount = message[index++],
                    Data = GetValues(index, message[index - 1], message),
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

                return new WriteMultipleRegisters()
                {
                    SlaveAddress = message[index++],
                    FunctionCode = message[index++],
                    StartingAddress = (ushort)(message[index++] << 0x08 | message[index++]),
                    QuantityOfRegisters = (ushort)(message[index++] << 0x08 | message[index++]),
                    ByteCount = message[index++],
                    Data = GetValues(index, message[index - 1], message),
                    CheckSum = Convert.ToBase64String(checkSum),
                    Protocol = ProtocolType.RTU
                };
            }
        }

        public static WriteMultipleRegisters Decode(string message)
        {
            WriteMultipleRegisters response = JsonConvert.DeserializeObject<WriteMultipleRegisters>(message);
            byte[] msg = response.Encode();
            return Decode(msg);
        }


        [JsonProperty("messageType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public override MessageType Type
        {
            get { return type; }
            set { }
        }

        [JsonProperty("protocol")]
        public override ProtocolType Protocol { get; set; }

        [JsonProperty("header")]
        public override MbapHeader Header { get; set; }

        [JsonProperty("slaveAddress")]
        public override byte SlaveAddress { get; set; }

        [JsonProperty("code")]
        public override byte FunctionCode { get; set; }

        [JsonProperty("startingAddress")]
        public ushort StartingAddress { get; set; }

        [JsonProperty("quantityOfRegisters")]
        public ushort QuantityOfRegisters { get; set; }

        [JsonProperty("byteCount")]
        public byte ByteCount { get; set; }

        [JsonProperty("data")]
        public ushort[] Data { get; set; }

        [JsonProperty("checkSum")]
        public string CheckSum { get; set; }

        public override byte[] Encode()
        {
            return Protocol == ProtocolType.TCP ? EncodeTcp() : EncodeRtu();
        }

        public override byte[] ConvertToRtu()
        {
            List<byte> frames = new List<byte>();
            frames.Add(SlaveAddress);
            frames.Add(FunctionCode);
            frames.Add((byte)((StartingAddress >> 8) & 0x00FF));
            frames.Add((byte)(StartingAddress & 0x00FF));
            frames.Add((byte)((QuantityOfRegisters >> 8) & 0x00FF));
            frames.Add((byte)(QuantityOfRegisters & 0x00FF));
            frames.Add(ByteCount);

            for (int i = 0; i < Data.Length; i++)
            {
                frames.Add((byte)((Data[i] >> 8) & 0x00FF));
                frames.Add((byte)(Data[i] & 0x00FF));
            }

            byte[] crc = Crc.Compute(frames.ToArray());
            byte[] message = new byte[frames.Count + crc.Length];
            Buffer.BlockCopy(frames.ToArray(), 0, message, 0, frames.Count);
            Buffer.BlockCopy(crc, 0, message, frames.Count, crc.Length);

            return message;
        }

        public override byte[] ConvertToTcp(byte unitId, ushort transactionId, ushort protocolId = 0)
        {
            List<byte> frames = new List<byte>();
            frames.Add(FunctionCode);
            frames.Add((byte)((StartingAddress >> 8) & 0x00FF));
            frames.Add((byte)(StartingAddress & 0x00FF));
            frames.Add((byte)((QuantityOfRegisters >> 8) & 0x00FF));
            frames.Add((byte)(QuantityOfRegisters & 0x00FF));
            frames.Add(ByteCount);

            for (int i = 0; i < Data.Length; i++)
            {
                frames.Add((byte)((Data[i] >> 8) & 0x00FF));
                frames.Add((byte)(Data[i] & 0x00FF));
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
            return JsonConvert.SerializeObject(this);
        }

        private byte[] EncodeTcp()
        {
            List<byte> frames = new List<byte>();
            frames.Add(FunctionCode);
            frames.Add((byte)((StartingAddress >> 8) & 0x00FF));//MSB
            frames.Add((byte)(StartingAddress & 0x00FF));//LSB
            frames.Add((byte)((QuantityOfRegisters >> 8) & 0x00FF)); //MSB
            frames.Add((byte)(QuantityOfRegisters & 0x00FF)); //LSB  
            frames.Add(ByteCount);

            for (int i = 0; i < Data.Length; i++)
            {
                frames.Add((byte)((Data[i] >> 8) & 0x00FF));
                frames.Add((byte)(Data[i] & 0x00FF));
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
            List<byte> list = new List<byte>();
            list.Add(SlaveAddress);
            list.Add(FunctionCode);
            list.Add((byte)((StartingAddress >> 8) & 0x00FF));
            list.Add((byte)(StartingAddress & 0x00FF));
            list.Add((byte)((QuantityOfRegisters >> 8) & 0x00FF));
            list.Add((byte)(QuantityOfRegisters & 0x00FF));
            list.Add(ByteCount);
            for (int i = 0; i < Data.Length; i++)
            {
                list.Add((byte)((Data[i] >> 8) & 0x00FF));
                list.Add((byte)(Data[i] & 0x00FF));
            }

            byte[] crc = Crc.Compute(list.ToArray());                        
            byte[] message = new byte[list.Count + crc.Length];
            Buffer.BlockCopy(list.ToArray(), 0, message, 0, list.Count);
            Buffer.BlockCopy(crc, 0, message, list.Count, crc.Length);

            return message;
        }

        private static ushort[] GetValues(int index, byte byteCount, byte[] message)
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

        private static byte[] GetBitBlock(byte[] message, int index, byte byteCount)
        {
            byte[] block = new byte[byteCount];
            Buffer.BlockCopy(message, index, block, 0, byteCount);

            return block;
        }

    }
}
