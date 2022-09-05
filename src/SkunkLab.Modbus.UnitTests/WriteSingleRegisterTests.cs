using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkunkLab.Modbus.Messaging;
using System.Text.Json;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class WriteSingleRegisterTests
    {
        [TestMethod]
        public void WriteSingleRegisterRtuTest()
        {
            string expected = "11-06-00-01-00-03-9A-9B";
            WriteSingleRegister register = WriteSingleRegister.Create(17, 1, 3);
            byte[] msg = register.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleRegisterTcpTest()
        {
            string expected = "00-1E-00-00-00-06-11-06-00-01-00-03";
            WriteSingleRegister register = WriteSingleRegister.Create(17, 30, 0, 1, 3);
            byte[] msg = register.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleRegisterConvertToRtuTest()
        {
            string expected = "11-06-00-01-00-03-9A-9B";
            WriteSingleRegister register = WriteSingleRegister.Create(17, 30, 0, 1, 3); //Create(17, 30, 0, 1, 3);
            byte[] msg = register.ConvertToRtu();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleRegisterConvertToTcpTest()
        {
            string expected = "00-1E-00-00-00-06-01-06-00-01-00-03";
            WriteSingleRegister register = WriteSingleRegister.Create(17, 1, 3);
            byte[] msg = register.ConvertToTcp(1, 30, 0);
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleRegisterSerializeRtuTest()
        {
            string expected = "11-06-00-01-00-03-9A-9B";
            WriteSingleRegister register = WriteSingleRegister.Create(17, 1, 3);
            string jsonString = register.Serialize();
            WriteSingleRegister register2 = JsonSerializer.Deserialize<WriteSingleRegister>(jsonString);
            byte[] msg = register2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleRegisterSerializeTcpTest()
        {
            string expected = "00-1E-00-00-00-06-11-06-00-01-00-03";
            WriteSingleRegister register = WriteSingleRegister.Create(17, 30, 0, 1, 3);
            string jsonString = register.Serialize();
            WriteSingleRegister register2 = JsonSerializer.Deserialize<WriteSingleRegister>(jsonString);
            byte[] msg = register2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }
}
