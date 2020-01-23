using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SkunkLab.Modbus.Messaging;
using System;
using System.Linq;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class WriteMultipleRegistersResponseTest 
    {
        [TestMethod]
        public void WriteMultipleRegistersResponseRtuTest()
        {
            string expected = "1C-10-00-64-00-02-03-9A";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            WriteMultipleRegistersResponse inputs = WriteMultipleRegistersResponse.Decode(message);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleRegistersResponseTcpTest()
        {
            string expected = "00-23-00-00-00-06-1C-10-00-64-00-02";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            WriteMultipleRegistersResponse inputs = WriteMultipleRegistersResponse.Decode(message);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleRegistersResponseConvertToRtuTest()
        {
            string expected = "1C-10-00-64-00-02-03-9A";
            string loaded = "00-23-00-00-00-06-1C-10-00-64-00-02";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            WriteMultipleRegistersResponse inputs = WriteMultipleRegistersResponse.Decode(message);
            byte[] actualBytes = inputs.ConvertToRtu();
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleRegistersResponseConvertToTcpTest()
        {
            string loaded = "1C-10-00-64-00-02-03-9A";
            string expected = "00-23-00-00-00-06-1C-10-00-64-00-02";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            WriteMultipleRegistersResponse inputs = WriteMultipleRegistersResponse.Decode(message);
            byte[] actualBytes = inputs.ConvertToTcp(28, 35, 0);
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleRegistersResponseSerializeRtuTest()
        {
            string expected = "1C-10-00-64-00-02-03-9A";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            WriteMultipleRegistersResponse inputs = WriteMultipleRegistersResponse.Decode(message);
            string jsonString = inputs.Serialize();
            WriteMultipleRegistersResponse inputs2 = JsonConvert.DeserializeObject<WriteMultipleRegistersResponse>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleRegistersResponseSerializeTcpTest()
        {
            string expected = "00-23-00-00-00-06-1C-10-00-64-00-02";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            WriteMultipleRegistersResponse inputs = WriteMultipleRegistersResponse.Decode(message);
            string jsonString = inputs.Serialize();
            WriteMultipleRegistersResponse inputs2 = JsonConvert.DeserializeObject<WriteMultipleRegistersResponse>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }
}
