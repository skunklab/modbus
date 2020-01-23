using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

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
