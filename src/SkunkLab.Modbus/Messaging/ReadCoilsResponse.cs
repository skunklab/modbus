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
    public class ReadCoilsResponse : ModbusResponseMessage
    {
        private const MessageType type = MessageType.ReadCoils;

        new public static ReadCoilsResponse Decode(byte[] message, ILogger logger = null)
        {
            if (message == null)
                throw new ArgumentNullException("message");

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
                    Data = GetValues(index, message[index-1], message)
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

        new public static ReadCoilsResponse Decode(string message)
        {
            return JsonConvert.DeserializeObject<ReadCoilsResponse>(message);
        }

        private static BitArray GetValues(int index, byte byteCount, byte[] message)
        {
            byte[] block = new byte[byteCount];
            Buffer.BlockCopy(message, index, block, 0, block.Length);
            BitArray array = new BitArray(block);
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
        [JsonConverter(typeof(StringEnumConverter))]
        public override ProtocolType Protocol { get; set; }

        [JsonProperty("mbapHeader")]
        public override MbapHeader Header { get; set; }

        [JsonProperty("slaveAddress")]
        public override byte SlaveAddress { get; set; }
        
        [JsonProperty("function")]
        public override byte FunctionCode { get; set; }

        [JsonProperty("byteCount")]
        public byte ByteCount { get; set; }

        //[JsonProperty("bitBlock")]
        //public byte[] BitBlock { get; set; }

        [JsonProperty("data")]
        [JsonConverter(typeof(BitArrayConverter))]
        public BitArray Data { get; set; }

        [JsonProperty("checkSum")]
        public string CheckSum { get; set; }

        public override byte[] Encode()
        {
            return Protocol == ProtocolType.TCP ? EncodeTcp() : EncodeRtu();
        }

        public override string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override byte[] ConvertToRtu()
        {
            List<byte> byteArray = new List<byte>();
            byteArray.Add(SlaveAddress);
            byteArray.Add(FunctionCode);
            byteArray.Add(ByteCount);


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
            List<byte> list = new List<byte>();
            list.Add(FunctionCode);
            list.Add(ByteCount);

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

            MbapHeader header = new MbapHeader() { UnitId = unitId, TransactionId = transactionId, ProtocolId = protocolId, Length = (ushort)(list.Count + 1) };
            byte[] headerBytes = header.Encode();
            byte[] message = new byte[headerBytes.Length + list.Count];
            Buffer.BlockCopy(headerBytes, 0, message, 0, headerBytes.Length);
            Buffer.BlockCopy(list.ToArray(), 0, message, headerBytes.Length, list.Count);
            return message;
        }

        private byte[] EncodeTcp()
        {
            List<byte> frames = new List<byte>();
            frames.Add(FunctionCode);
            frames.Add(ByteCount);

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
            List<byte> frames = new List<byte>();

            frames.Add(SlaveAddress);
            frames.Add(FunctionCode);
            frames.Add(ByteCount);

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
