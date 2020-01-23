using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SkunkLab.Modbus.Messaging;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class WriteSingleCoilTests
    {
        [TestMethod]
        public void WriteSingleCoilRtuTest()
        {
            string expected = "01-05-00-64-FF-00-CD-E5";
            WriteSingleCoil coils = WriteSingleCoil.Create(1, 100, 65280);
            byte[] msg = coils.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleCoilTcpTest()
        {
            string expected = "00-19-00-00-00-06-01-05-00-64-FF-00";
            WriteSingleCoil coils = WriteSingleCoil.Create(1, 25, 0, 100, 65280);
            byte[] msg = coils.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleCoilConvertToRtuTest()
        {
            string expected = "01-05-00-64-FF-00-CD-E5";
            WriteSingleCoil coils = WriteSingleCoil.Create(1, 25, 0, 100, 65280);
            byte[] msg = coils.ConvertToRtu();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleCoilConvertToTcpTest()
        {
            string expected = "00-19-00-00-00-06-01-05-00-64-FF-00";
            WriteSingleCoil coils = WriteSingleCoil.Create(1, 100, 65280);
            byte[] msg = coils.ConvertToTcp(1, 25, 0);
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleCoilSerializeRtuTest()
        {
            string expected = "01-05-00-64-FF-00-CD-E5";
            WriteSingleCoil coils = WriteSingleCoil.Create(1, 100, 65280);
            string jsonString = coils.Serialize();
            WriteSingleCoil coils2 = JsonConvert.DeserializeObject<WriteSingleCoil>(jsonString);
            byte[] msg = coils2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleCoilSerializeTcpTest()
        {
            string expected = "00-19-00-00-00-06-01-05-00-64-FF-00";
            WriteSingleCoil coils = WriteSingleCoil.Create(1, 25, 0, 100, 65280);
            string jsonString = coils.Serialize();
            WriteSingleCoil coils2 = JsonConvert.DeserializeObject<WriteSingleCoil>(jsonString);
            byte[] msg = coils2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }
}
