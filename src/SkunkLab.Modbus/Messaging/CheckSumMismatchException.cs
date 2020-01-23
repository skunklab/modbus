using System;
using System.Runtime.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    public class CheckSumMismatchException : ModbusRtuException
    {
        public CheckSumMismatchException()
        {
        }

        public CheckSumMismatchException(string message)
            : base(message)
        {
        }

        public CheckSumMismatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CheckSumMismatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
