using System;
using System.Runtime.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    public class InvalidMbapHeaderException : ModbusTcpException
    {
        public InvalidMbapHeaderException()
        {
        }

        public InvalidMbapHeaderException(string message)
            : base(message)
        {
        }

        public InvalidMbapHeaderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidMbapHeaderException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
