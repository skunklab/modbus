using System;
using System.Runtime.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    public class MessageLengthMismatchException : ModbusTcpException
    {
        public MessageLengthMismatchException()
        {
        }

        public MessageLengthMismatchException(string message)
            : base(message)
        {
        }

        public MessageLengthMismatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MessageLengthMismatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
