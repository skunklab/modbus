using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkunkLab.Modbus.Messaging;
using System.Text.Json;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class WriteMultipleRegistersTest
    {

        [TestMethod]
        public void WriteMultipleRegistersRtuTest()
        {
            string expected = "1C-10-00-64-00-02-04-03-E8-07-D8-19-02";
            WriteMultipleRegisters registers = WriteMultipleRegisters.Create(28, 100, new ushort[] { 1000, 2008 });
            byte[] msg = registers.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleRegistersTcpTest()
        {
            string expected = "00-23-00-00-00-0B-1C-10-00-64-00-02-04-03-E8-07-D8";
            WriteMultipleRegisters registers = WriteMultipleRegisters.Create(28, 35, 0, 100, new ushort[] { 1000, 2008 });
            byte[] msg = registers.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleRegistersConvertToRtuTest()
        {
            string expected = "1C-10-00-64-00-02-04-03-E8-07-D8-19-02";
            WriteMultipleRegisters registers = WriteMultipleRegisters.Create(28, 35, 0, 100, new ushort[] { 1000, 2008 });
            byte[] msg = registers.ConvertToRtu();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleRegistersConvertToTcpTest()
        {
            string expected = "00-23-00-00-00-0B-1C-10-00-64-00-02-04-03-E8-07-D8";
            WriteMultipleRegisters registers = WriteMultipleRegisters.Create(28, 100, new ushort[] { 1000, 2008 });
            byte[] msg = registers.ConvertToTcp(28, 35, 0);
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleRegistersSerializeRtuTest()
        {
            string expected = "1C-10-00-64-00-02-04-03-E8-07-D8-19-02";
            WriteMultipleRegisters registers = WriteMultipleRegisters.Create(28, 100, new ushort[] { 1000, 2008 });
            string jsonString = registers.Serialize();
            WriteMultipleRegisters registers2 = JsonSerializer.Deserialize<WriteMultipleRegisters>(jsonString);
            byte[] msg = registers2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleRegistersSerializeTcpTest()
        {
            string expected = "00-23-00-00-00-0B-1C-10-00-64-00-02-04-03-E8-07-D8";
            WriteMultipleRegisters registers = WriteMultipleRegisters.Create(28, 35, 0, 100, new ushort[] { 1000, 2008 });
            string jsonString = registers.Serialize();
            WriteMultipleRegisters registers2 = JsonSerializer.Deserialize<WriteMultipleRegisters>(jsonString);
            byte[] msg = registers2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }
}
