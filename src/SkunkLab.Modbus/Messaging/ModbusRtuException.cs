using System;
using System.Runtime.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    public class ModbusRtuException : ModbusException
    {
        public ModbusRtuException()
        {
        }

        public ModbusRtuException(string message)
            : base(message)
        {
        }

        public ModbusRtuException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ModbusRtuException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
