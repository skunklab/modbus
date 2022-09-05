namespace SkunkLab.Modbus.Messaging
{
    public abstract class ModbusMessage
    {
        public abstract MessageType Type { get; set; }
        public abstract ProtocolType Protocol { get; set; }

        public abstract MbapHeader Header { get; set; }

        public abstract byte SlaveAddress { get; set; }

        public abstract byte FunctionCode { get; set; }

        public abstract byte[] Encode();

        public abstract string Serialize();

        public abstract byte[] ConvertToRtu();

        public abstract byte[] ConvertToTcp(byte unitId, ushort transactionId, ushort protocolId = 0);
    }
}
