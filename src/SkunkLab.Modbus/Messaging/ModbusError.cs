using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace SkunkLab.Modbus.Messaging
{
    public class ModbusError : ModbusMessage
    {
        public static ModbusError Create(byte slaveId, byte functionCode, ModbusErrorCode errorCode)
        {
            ModbusError request = new ModbusError()
            {
                SlaveAddress = slaveId,
                FunctionCode = functionCode,
                ErrorCode = errorCode,
                Protocol = ProtocolType.RTU
            };

            byte[] encoded = request.Encode();
            return ModbusError.Decode(encoded);
        }

        public static ModbusError Create(byte unitId, ushort transactionId, ushort protocolId, byte functionCode, ModbusErrorCode errorCode)
        {
            ModbusError request = new ModbusError()
            {
                Header = new MbapHeader() { ProtocolId = protocolId, TransactionId = transactionId, UnitId = unitId },
                SlaveAddress = unitId,
                FunctionCode = functionCode,
                ErrorCode = errorCode,
                Protocol = ProtocolType.TCP
            };

            byte[] rtuEncoded = request.Encode();
            return ModbusError.Decode(rtuEncoded);
        }

        public static ModbusError Decode(byte[] message, ILogger logger = null)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            try
            {
                MbapHeader header = MbapHeader.Decode(message);
                int index = 7;
                return new ModbusError()
                {
                    Header = header,
                    SlaveAddress = header.UnitId,
                    FunctionCode = (byte)(message[index++] & 0x000F),
                    ErrorCode = (ModbusErrorCode)(message[index] & 0x000F),
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

                return new ModbusError()
                {
                    SlaveAddress = message[index++],
                    FunctionCode = (byte)(message[index++] & 0x000F),
                    ErrorCode = (ModbusErrorCode)(message[index] & 0x000F),
                    CheckSum = Convert.ToBase64String(checkSum),
                    Protocol = ProtocolType.RTU
                };
            }

        }

        public static ReadCoils Decode(string message)
        {
            return JsonConvert.DeserializeObject<ReadCoils>(message);
        }

        private const MessageType type = MessageType.Error;

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

        [JsonProperty("errorCode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ModbusErrorCode ErrorCode { get; set; }

        [JsonProperty("checkSum")]
        public string CheckSum { get; set; }

        public override byte[] ConvertToRtu()
        {
            int index = 0;
            byte[] frames = new byte[3];
            frames[index++] = SlaveAddress;
            frames[index++] = (byte)((1 << 7) | FunctionCode);
            frames[index++] = (byte)Convert.ToByte(ErrorCode);

            byte[] crc = Crc.Compute(frames);
            byte[] message = new byte[frames.Length + crc.Length];
            Buffer.BlockCopy(frames, 0, message, 0, frames.Length);
            Buffer.BlockCopy(crc, 0, message, frames.Length, crc.Length);
            return message;
        }

        public override byte[] ConvertToTcp(byte unitId, ushort transactionId, ushort protocolId = 0)
        {
            int index = 0;
            byte[] frames = new byte[2];
            frames[index++] = (byte)((1 << 7) | FunctionCode);
            frames[index++] = (byte)Convert.ToByte(ErrorCode);
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
            return JsonConvert.SerializeObject(this);
        }

        private byte[] EncodeTcp()
        {
            List<byte> frames = new List<byte>();
            frames.Add((byte)((1 << 7) | FunctionCode));
            frames.Add((byte)Convert.ToByte(ErrorCode));                                                    
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
            byte[] frames = new byte[3];
            frames[index++] = SlaveAddress;
            frames[index++] = (byte)((1 << 7) | FunctionCode);
            frames[index++] = (byte)Convert.ToByte(ErrorCode);

            byte[] crc = Crc.Compute(frames);
            byte[] message = new byte[frames.Length + crc.Length];
            Buffer.BlockCopy(frames, 0, message, 0, frames.Length);
            Buffer.BlockCopy(crc, 0, message, frames.Length, crc.Length);
            return message;
        }
    }
}
