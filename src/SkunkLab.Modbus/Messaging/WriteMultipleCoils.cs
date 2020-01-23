using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SkunkLab.Modbus.Messaging
{
    [Serializable]
    [JsonObject]
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
            WriteMultipleCoils response = JsonConvert.DeserializeObject<WriteMultipleCoils>(message);
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
                    Data = GetValues(index, message[index - 1], message, (ushort)(message[index-3] << 0x08 | message[index-2])),
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
            BitArray array = new BitArray(block);
            array.Length = length;
            return array;
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

        [JsonProperty("quantityOfCoils")]
        public ushort QuantityOfCoils { get; set; }

        [JsonProperty("byteCount")]       
        public byte ByteCount { get; set; }

        [JsonProperty("data")]
        [JsonConverter(typeof(BitArrayConverter))]
        public BitArray Data { get; set; }

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
            frames.Add((byte)((QuantityOfCoils >> 8) & 0x00FF));
            frames.Add((byte)(QuantityOfCoils & 0x00FF));
            frames.Add(ByteCount);

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
            List<byte> frames = new List<byte>();
            frames.Add(FunctionCode);
            frames.Add((byte)((StartingAddress >> 8) & 0x00FF));
            frames.Add((byte)(StartingAddress & 0x00FF));
            frames.Add((byte)((QuantityOfCoils >> 8) & 0x00FF));
            frames.Add((byte)(QuantityOfCoils & 0x00FF));
            frames.Add(ByteCount);

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
            return JsonConvert.SerializeObject(this);
        }

        private byte[] EncodeRtu()
        {
            List<byte> list = new List<byte>();
            list.Add(SlaveAddress);
            list.Add(FunctionCode);
            list.Add((byte)((StartingAddress >> 8) & 0x00FF));
            list.Add((byte)(StartingAddress & 0x00FF));
            list.Add((byte)((QuantityOfCoils >> 8) & 0x00FF));
            list.Add((byte)(QuantityOfCoils & 0x00FF));
            list.Add(ByteCount);

            byte[] data = new byte[ByteCount];
            Data.CopyTo(data, 0);

            foreach(var item in data)
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
            List<byte> frames = new List<byte>();
            frames.Add(FunctionCode);
            frames.Add((byte)((StartingAddress >> 8) & 0x00FF));//MSB
            frames.Add((byte)(StartingAddress & 0x00FF));//LSB
            frames.Add((byte)((QuantityOfCoils >> 8) & 0x00FF)); //MSB
            frames.Add((byte)(QuantityOfCoils & 0x00FF)); //LSB  
            frames.Add((byte)ByteCount);

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
