using System;
using System.Runtime.Serialization;

namespace SkunkLab.Modbus.Messaging
{
    public class ModbusFunctionCodeMismatchException  : Exception
    {
        public ModbusFunctionCodeMismatchException()
        {
        }

        public ModbusFunctionCodeMismatchException(string message)
            : base(message)
        {
        }

        public ModbusFunctionCodeMismatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ModbusFunctionCodeMismatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
