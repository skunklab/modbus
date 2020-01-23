using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace SkunkLab.Modbus.Messaging
{
    [Serializable]
    [JsonObject]
    public class ReadCoils : ModbusMessage
    {
        public ReadCoils()
        {                
        }

        private const MessageType type = MessageType.ReadCoils;

        public static ReadCoils Create(byte slaveId, ushort startingAddress, ushort quantity)
        {
            ReadCoils request = new ReadCoils()
            {
                SlaveAddress = slaveId,
                FunctionCode = 1,
                StartingAddress = (ushort)(startingAddress),
                QuantityOfCoils = quantity,
                Protocol = ProtocolType.RTU
            };

            byte[] encoded = request.Encode();
            return ReadCoils.Decode(encoded);            
        }

        public static ReadCoils Create(byte unitId, ushort transactionId, ushort protocolId, ushort startingAddress, ushort quantity)
        {
            ReadCoils request = new ReadCoils()
            {
                Header = new MbapHeader() { ProtocolId = protocolId, TransactionId = transactionId, UnitId = unitId },
                SlaveAddress = unitId,
                FunctionCode = 1,
                StartingAddress = (ushort)(startingAddress),
                QuantityOfCoils = quantity,
                Protocol = ProtocolType.TCP
            };
            
            byte[] rtuEncoded = request.Encode();
            return ReadCoils.Decode(rtuEncoded);
        }

        public static ReadCoils Decode(byte[] message, ILogger logger = null)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            
            try
            {
                MbapHeader header = MbapHeader.Decode(message);
                int index = 7;
                return new ReadCoils()
                {
                    Header = header,
                    SlaveAddress = header.UnitId,
                    FunctionCode = message[index++],
                    StartingAddress = (ushort)(message[index++] << 0x08 | message[index++]),
                    QuantityOfCoils = (ushort)(message[index++] << 0x08 | message[index++]),                    
                    Protocol = ProtocolType.TCP
                };
            }
            catch(Exception ex)
            {
                logger?.LogDebug(ex, "Modbus TCP header read fault.");
                byte[] data = new byte[message.Length - 2];
                Buffer.BlockCopy(message, 0, data, 0, data.Length);
                byte[] checkSum = Crc.Compute(data);

                if (message[message.Length - 2] != checkSum[0] || message[message.Length - 1] != checkSum[1])
                    throw new CheckSumMismatchException("Check sum mismatch.");
                int index = 0;

                return new ReadCoils()
                {
                    SlaveAddress = message[index++],
                    FunctionCode = message[index++],
                    StartingAddress = (ushort)(message[index++] << 0x08 | message[index++]),
                    QuantityOfCoils = (ushort)(message[index++] << 0x08 | message[index++]),
                    CheckSum = Convert.ToBase64String(checkSum),
                    Protocol = ProtocolType.RTU
                };
            }

        }

        public static ReadCoils Decode(string message)
        {
            return JsonConvert.DeserializeObject<ReadCoils>(message);
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

        [JsonProperty("header")]
        public override MbapHeader Header { get; set; }

        [JsonProperty("slaveAddress")]
        public override byte SlaveAddress { get; set; }

        [JsonProperty("function")]
        public override byte FunctionCode { get; set; }

        [JsonProperty("startingAddress")]
        public ushort StartingAddress { get; set; }

        [JsonProperty("quantity")]
        public ushort QuantityOfCoils { get; set; }

        [JsonProperty("checkSum")]
        public string CheckSum { get; set; }

        public override byte[] Encode()
        {
            return Protocol == ProtocolType.TCP ? EncodeTcp() : EncodeRtu();
        }

        public override byte[] ConvertToRtu()
        {
            int index = 0;
            byte[] frames = new byte[6];
            frames[index++] = SlaveAddress;
            frames[index++] = FunctionCode;
            frames[index++] = (byte)((StartingAddress >> 8) & 0x00FF); //MSB
            frames[index++] = (byte)(StartingAddress & 0x00FF); //LSB
            frames[index++] = (byte)((QuantityOfCoils >> 8) & 0x00FF); //MSB
            frames[index++] = (byte)(QuantityOfCoils & 0x00FF); //LSB

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
            frames[index++] = (byte)((QuantityOfCoils >> 8) & 0x00FF); //MSB
            frames[index++] = (byte)(QuantityOfCoils & 0x00FF); //LSB
            MbapHeader header = new MbapHeader() { UnitId = unitId, TransactionId = transactionId, ProtocolId = protocolId, Length = (ushort)(frames.Length + 1) };
            byte[] headerBytes = header.Encode();
            byte[] message = new byte[headerBytes.Length + frames.Length];
            Buffer.BlockCopy(headerBytes, 0, message, 0, headerBytes.Length);
            Buffer.BlockCopy(frames, 0, message, headerBytes.Length, frames.Length);
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
            frames.Add((byte)((QuantityOfCoils >> 8) & 0x00FF)); //MSB
            frames.Add((byte)(QuantityOfCoils & 0x00FF)); //LSB                                                         
            Header.Length = (ushort)(frames.Count + 1);
            byte[] header = Header.Encode();
            byte[] message = new byte[header.Length + frames.Count];
            Buffer.BlockCopy(header, 0, message, 0, header.Length);
            Buffer.BlockCopy(frames.ToArray(), 0, message, header.Length, frames.Count);

            return message;
        }

        private byte[] EncodeRtu()
        {
            int index = 0;
            byte[] frames = new byte[6];
            frames[index++] = SlaveAddress;
            frames[index++] = FunctionCode;
            frames[index++] = (byte)((StartingAddress >> 8) & 0x00FF); //MSB
            frames[index++] = (byte)(StartingAddress & 0x00FF); //LSB
            frames[index++] = (byte)((QuantityOfCoils >> 8) & 0x00FF); //MSB
            frames[index++] = (byte)(QuantityOfCoils & 0x00FF); //LSB
            byte[] crc = Crc.Compute(frames);
            byte[] message = new byte[frames.Length + crc.Length];
            Buffer.BlockCopy(frames, 0, message, 0, frames.Length);
            Buffer.BlockCopy(crc, 0, message, frames.Length, crc.Length);

            return message;
        }



    }
}
