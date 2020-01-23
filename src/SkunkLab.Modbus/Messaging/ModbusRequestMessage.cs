using Microsoft.Extensions.Logging;

namespace SkunkLab.Modbus.Messaging
{
    public abstract class ModbusRequestMessage : ModbusMessage
    {
        public static ModbusRequestMessage Decode(byte[] message, ILogger logger = null)
        {
            return null;
        }

        public static ModbusRequestMessage Decode(string message)
        {
            return null;
        }

        //public abstract MessageType Type { get; set; }

        //public abstract MbapHeader Header { get; set; }

        //public abstract byte SlaveAddress { get; set; }

        //public abstract byte FunctionCode { get; set; }

        //public abstract byte[] Encode();

        //public abstract string Serialize();

        //public abstract byte[] ConvertToRtu();

        //public abstract byte[] ConvertToTcp(byte unitId, ushort transactionId, ushort protocolId = 0);
    }
}
