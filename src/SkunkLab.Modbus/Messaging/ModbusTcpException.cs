using System;
using System.Runtime.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    public class ModbusTcpException : ModbusException
    {
        public ModbusTcpException()
        {
        }

        public ModbusTcpException(string message)
            : base(message)
        {
        }

        public ModbusTcpException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ModbusTcpException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
