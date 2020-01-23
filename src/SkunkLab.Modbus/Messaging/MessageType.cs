namespace SkunkLab.Modbus.Messaging
{
    public enum MessageType
    {
        ReadCoils = 1,
        ReadDiscreteInputs = 2,
        ReadHoldingRegisters = 3,
        ReadInputRegisters = 4,
        WriteSingleCoil = 5,
        WriteSingleRegister = 6,
        Error = 8,
        WriteMultipleCoilsResponse = 15,
        WriteMultipleRegistersResponse = 16,
        ReadCoilsResponse = 101,
        ReadDiscreteInputsResponse = 102,
        ReadHoldingRegistersResponse = 103,
        ReadInputRegistersResponse = 104,
        WriteSingleCoilResponse = 105,
        WriteSingleRegisterResponse = 106,
        WriteMultipleCoils = 115,
        WriteMultpleRegisters = 116
    }
}
