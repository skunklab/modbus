using System;
using System.Collections;

namespace SkunkLab.Modbus.Messaging
{
    public abstract class ModbusRequestFactory
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
                return ReadCoils.Decode(message);
            if (code == 2)
                return ReadDiscreteInputs.Decode(message);
            if (code == 3)
                return ReadHoldingRegisters.Decode(message);
            if (code == 4)
                return ReadInputRegisters.Decode(message);
            if (code == 5)
                return WriteSingleCoil.Decode(message);
            if (code == 6)
                return WriteSingleRegister.Decode(message);
            if (code == 15)
                return WriteMultipleCoils.Decode(message);
            if (code == 16)
                return WriteMultipleCoils.Decode(message);

            throw new IndexOutOfRangeException("Function code out of range.");
        }

        public static ModbusMessage Create(MessageType type, byte slaveId, ushort startingAddress, ushort quantity)
        {
            int num = (int)type;

            if (num == 1)
                return ReadCoils.Create(slaveId, startingAddress, quantity);
            if (num == 2)
                return ReadDiscreteInputs.Create(slaveId, startingAddress, quantity);
            if (num == 3)
                return ReadHoldingRegisters.Create(slaveId, startingAddress, quantity);
            if (num == 4)
                return ReadInputRegisters.Create(slaveId, startingAddress, quantity);

            throw new ModbusFunctionCodeMismatchException("Message type out of range, not 1-4.");
        }

        public static ModbusMessage Create(MessageType type, byte unitId, ushort transactionId, ushort protocolId, byte slaveId, ushort startingAddress, ushort quantity)
        {
            int num = (int)type;

            if (num == 1)
                return ReadCoils.Create(unitId, transactionId, protocolId, startingAddress, quantity);
            if (num == 2)
                return ReadDiscreteInputs.Create(unitId, transactionId, protocolId, startingAddress, quantity);
            if (num == 3)
                return ReadHoldingRegisters.Create(unitId, transactionId, protocolId, startingAddress, quantity);
            if (num == 4)
                return ReadInputRegisters.Create(unitId, transactionId, protocolId, startingAddress, quantity);

            throw new ModbusFunctionCodeMismatchException("Message type out of range, not 1-4.");
        }

        public static ModbusMessage Create(byte slaveId, ushort startingAddress, ushort data, MessageType type)
        {
            //5,6
            int num = (int)type;

            if (num == 5)
                return WriteSingleCoil.Create(slaveId, startingAddress, data);
            if (num == 6)
                return WriteSingleRegister.Create(slaveId, startingAddress, data);

            throw new ModbusFunctionCodeMismatchException("Message type out of range, not 5 or 6.");
        }

        public static ModbusMessage Create(byte unitId, ushort transactionId, ushort protocolId, MessageType type, ushort startingAddress, ushort data)
        {
            //5,6
            int num = (int)type;

            if (num == 5)
                return WriteSingleCoil.Create(unitId, transactionId, protocolId, startingAddress, data);
            if (num == 6)
                return WriteSingleRegister.Create(unitId, transactionId, protocolId, startingAddress, data);

            throw new ModbusFunctionCodeMismatchException("Message type out of range, not 5 or 6.");
        }

        public static ModbusMessage Create(byte slaveId, ushort startingAddress, BitArray coilValues)
        {
            //15
            return WriteMultipleCoils.Create(slaveId, startingAddress, coilValues);
        }
        public static ModbusMessage Create(byte unitId, ushort transactionId, ushort protocolId, ushort startingAddress, BitArray coilValues)
        {
            return WriteMultipleCoils.Create(unitId, transactionId, protocolId, startingAddress, coilValues);
        }

        public static ModbusMessage Create(byte slaveId, ushort startingAddress, ushort[] data)
        {
            //16
            return WriteMultipleRegisters.Create(slaveId, startingAddress, data);
        }

        public static ModbusMessage Create(byte unitId, ushort transactionId, ushort protocolId, ushort startingAddress, ushort[] data)
        {
            return WriteMultipleRegisters.Create(unitId, transactionId, protocolId, startingAddress, data);
        }
    }
}
