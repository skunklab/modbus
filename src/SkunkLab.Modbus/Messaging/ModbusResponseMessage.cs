using Microsoft.Extensions.Logging;

namespace SkunkLab.Modbus.Messaging
{
    public abstract class ModbusResponseMessage : ModbusMessage
    {
        public static ModbusResponseMessage Decode(byte[] message, ILogger logger = null)
        {
            return null;
        }

        public static ModbusResponseMessage Decode(string message)
        {
            return null;
        }



    }
}
