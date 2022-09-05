using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkunkLab.Modbus.Messaging;
using System;
using System.Linq;
using System.Text.Json;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class ReadHoldingRegistersResponseTests
    {
        [TestMethod]
        public void ReadHoldingRegistersResponseRtuTest()
        {
            string expected = "01-03-04-03-E8-13-88-77-15";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadHoldingRegistersResponse inputs = ReadHoldingRegistersResponse.Decode(message);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadHoldingRegistersResponseTcpTest()
        {
            string expected = "00-0F-00-00-00-07-01-03-04-03-E8-13-88";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadHoldingRegistersResponse inputs = ReadHoldingRegistersResponse.Decode(message);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadHoldingRegistersResponseConvertToRtuTest()
        {
            string expected = "01-03-04-03-E8-13-88-77-15";
            string loaded = "00-0F-00-00-00-07-01-03-04-03-E8-13-88";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadHoldingRegistersResponse inputs = ReadHoldingRegistersResponse.Decode(message);
            byte[] actualBytes = inputs.ConvertToRtu();
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadHoldingRegistersResponseConvertToTcpTest()
        {
            string loaded = "01-03-04-03-E8-13-88-77-15";
            string expected = "00-0F-00-00-00-07-01-03-04-03-E8-13-88";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadHoldingRegistersResponse inputs = ReadHoldingRegistersResponse.Decode(message);
            byte[] actualBytes = inputs.ConvertToTcp(1, 15, 0);
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadHoldingRegistersResponseSerializeRtuTest()
        {
            string expected = "01-03-04-03-E8-13-88-77-15";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            ReadHoldingRegistersResponse inputs = ReadHoldingRegistersResponse.Decode(message);
            string jsonString = inputs.Serialize();
            ReadHoldingRegistersResponse inputs2 = JsonSerializer.Deserialize<ReadHoldingRegistersResponse>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadHoldingRegistersResponseSerializeTcpTest()
        {
            string expected = "00-0F-00-00-00-07-01-03-04-03-E8-13-88";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            ReadHoldingRegistersResponse inputs = ReadHoldingRegistersResponse.Decode(message);
            string jsonString = inputs.Serialize();
            ReadHoldingRegistersResponse inputs2 = JsonSerializer.Deserialize<ReadHoldingRegistersResponse>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }
}
