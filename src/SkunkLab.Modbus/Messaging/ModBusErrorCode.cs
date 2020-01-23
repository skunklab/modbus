using System;
using System.Collections.Generic;
using System.Text;

namespace SkunkLab.Modbus.Messaging
{
    public enum ModbusErrorCode
    {
        IllegalFunction = 1,
        IllegalDataAddress = 2,
        IllegalDataValue = 3,
        SlaveDeviceFailure = 4,
        Acknowledge = 5,
        SlaveDeviceBusy = 6,
        NegativeAcknowledge = 7,
        MemoryParityError = 8,
        GatewayPathUnavailable = 10,
        GatewayTargetDeviceNoResponse = 11
    }
}
