using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SkunkLab.Modbus.Messaging;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class ReadHoldingRegistersTests
    {
        [TestMethod]
        public void ReadHoldingRegistersRtuTest()
        {
            string expected = "01-03-00-00-00-02-C4-0B";
            ReadHoldingRegisters registers = ReadHoldingRegisters.Create(1, 0, 2);
            byte[] msg = registers.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadHoldingRegistersTcpTest()
        {
            string expected = "00-03-00-00-00-06-01-03-00-00-00-02";
            ReadHoldingRegisters registers = ReadHoldingRegisters.Create(1, 3, 0, 0, 2);
            byte[] msg = registers.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadHoldingRegistersConvertToRtuTest()
        {
            string expected = "01-03-00-00-00-02-C4-0B";
            ReadHoldingRegisters registers = ReadHoldingRegisters.Create(1, 3, 0, 0, 2);
            byte[] msg = registers.ConvertToRtu();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadHoldingRegistersConvertToTcpTest()
        {
            string expected = "00-03-00-00-00-06-01-03-00-00-00-02";
            ReadHoldingRegisters registers = ReadHoldingRegisters.Create(1, 0, 2);
            byte[] msg = registers.ConvertToTcp(1, 3, 0);
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadHoldingRegistersSerializeRtuTest()
        {
            string expected = "01-03-00-00-00-02-C4-0B";
            ReadHoldingRegisters registers = ReadHoldingRegisters.Create(1, 0, 2);
            string jsonString = registers.Serialize();
            ReadHoldingRegisters registers2 = JsonConvert.DeserializeObject<ReadHoldingRegisters>(jsonString);
            byte[] msg = registers2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadHoldingRegistersSerializeTcpTest()
        {
            string expected = "00-03-00-00-00-06-01-03-00-00-00-02";
            ReadHoldingRegisters registers = ReadHoldingRegisters.Create(1, 3, 0, 0, 2);
            string jsonString = registers.Serialize();
            ReadHoldingRegisters registers2 = JsonConvert.DeserializeObject<ReadHoldingRegisters>(jsonString);
            byte[] msg = registers2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }
}
