using System;

namespace SkunkLab.Modbus.Messaging
{
    public abstract class ModbusResponseFactory
    {
        public static ModbusMessage Decode(byte[] message)
        {
            try
            {
                MbapHeader header = MbapHeader.Decode(message);
                int index = 7;
                byte code = message[index++];
                return GetDecodedMessage(code, message);
            }
            catch
            {
                int index = 0;
                index++;
                byte code = message[index++];
                return GetDecodedMessage(code, message);
            }
        }

        private static ModbusMessage GetDecodedMessage(byte code, byte[] message)
        {
            if (code == 1)
                return ReadCoilsResponse.Decode(message);
            if (code == 2)
                return ReadDiscreteInputsResponse.Decode(message);
            if (code == 3)
                return ReadHoldingRegistersResponse.Decode(message);
            if (code == 4)
                return ReadInputRegistersResponse.Decode(message);
            if (code == 5)
                return WriteSingleCoilResponse.Decode(message);
            if (code == 6)
                return WriteSingleRegisterResponse.Decode(message);
            if (code == 15)
                return WriteMultipleCoilsResponse.Decode(message);
            if (code == 16)
                return WriteMultipleCoilsResponse.Decode(message);

            throw new IndexOutOfRangeException("Function code out of range.");
        }
    }
}
