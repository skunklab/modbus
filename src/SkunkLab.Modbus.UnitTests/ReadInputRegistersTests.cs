using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SkunkLab.Modbus.Messaging;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class ReadInputRegistersTests
    {
        [TestMethod]
        public void ReadInputRegistersRtuTest()
        {
            string expected = "01-04-00-00-00-02-71-CB";
            ReadInputRegisters registers = ReadInputRegisters.Create(1, 0, 2);
            byte[] msg = registers.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadInputRegistersTcpTest()
        {
            string expected = "00-04-00-00-00-06-01-04-00-00-00-02";
            ReadInputRegisters registers = ReadInputRegisters.Create(1, 4, 0, 0, 2);
            byte[] msg = registers.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadInputRegistersConvertToRtuTest()
        {
            string expected = "01-04-00-00-00-02-71-CB";
            ReadInputRegisters registers = ReadInputRegisters.Create(1, 4, 0, 0, 2);
            byte[] msg = registers.ConvertToRtu();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadInputRegistersConvertToTcpTest()
        {
            string expected = "00-04-00-00-00-06-01-04-00-00-00-02";
            ReadInputRegisters registers = ReadInputRegisters.Create(1, 0, 2);
            byte[] msg = registers.ConvertToTcp(1, 4, 0);
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadInputRegistersSerializeRtuTest()
        {
            string expected = "01-04-00-00-00-02-71-CB";
            ReadInputRegisters registers = ReadInputRegisters.Create(1, 0, 2);
            string jsonString = registers.Serialize();
            ReadInputRegisters registers2 = JsonConvert.DeserializeObject<ReadInputRegisters>(jsonString);
            byte[] msg = registers2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadInputRegistersSerializeTcpTest()
        {
            string expected = "00-04-00-00-00-06-01-04-00-00-00-02";
            ReadInputRegisters registers = ReadInputRegisters.Create(1, 4, 0, 0, 2);
            string jsonString = registers.Serialize();
            ReadInputRegisters registers2 = JsonConvert.DeserializeObject<ReadInputRegisters>(jsonString);
            byte[] msg = registers2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }
}
