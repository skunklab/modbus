using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkunkLab.Modbus.Messaging;
using System.Collections;
using System.Text.Json;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class WriteMultipleCoilsResponseTest
    {
        [TestMethod]
        public void WriteMultipleCoilsResponseRtuTest()
        {
            string expected = "11-0F-00-13-00-0A-26-99";
            WriteMultipleCoilsResponse coils = WriteMultipleCoilsResponse.Create(17, 19, 10);
            byte[] msg = coils.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleCoilsResponseTcpTest()
        {
            string expected = "00-19-00-00-00-06-11-0F-00-13-00-0A";
            WriteMultipleCoilsResponse coils = WriteMultipleCoilsResponse.Create(17, 25, 0, 19, 10);
            byte[] msg = coils.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleCoilsResponseConvertToRtuTest()
        {
            string expected = "11-0F-00-13-00-0A-26-99";
            WriteMultipleCoilsResponse coils = WriteMultipleCoilsResponse.Create(17, 25, 0, 19, 10);
            byte[] msg = coils.ConvertToRtu();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleCoilsResponseConvertToTcpTest()
        {
            string expected = "00-19-00-00-00-06-11-0F-00-13-00-0A";
            WriteMultipleCoilsResponse coils = WriteMultipleCoilsResponse.Create(17, 19, 10);
            byte[] msg = coils.ConvertToTcp(17, 25, 0);
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void WriteMultipleCoilsResponseSerializeRtuTest()
        {
            string expected = "11-0F-00-13-00-0A-26-99";
            WriteMultipleCoilsResponse coils = WriteMultipleCoilsResponse.Create(17, 19, 10);
            string jsonString = coils.Serialize();
            WriteMultipleCoilsResponse coil2 = JsonSerializer.Deserialize<WriteMultipleCoilsResponse>(jsonString);
            byte[] msg = coil2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleCoilsResponseSerializeTcpTest()
        {
            string expected = "00-19-00-00-00-06-11-0F-00-13-00-0A";
            BitArray array = new(new bool[] { true, false, true, true, false, false, true, true, true, false });
            WriteMultipleCoilsResponse coils = WriteMultipleCoilsResponse.Create(17, 25, 0, 19, 10);
            string jsonString = coils.Serialize();
            WriteMultipleCoilsResponse coil2 = JsonSerializer.Deserialize<WriteMultipleCoilsResponse>(jsonString);
            byte[] msg = coil2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }
}
