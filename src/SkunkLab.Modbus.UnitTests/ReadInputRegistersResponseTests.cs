using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SkunkLab.Modbus.Messaging;
using System;
using System.Linq;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class ReadInputRegistersResponseTests
    {
        [TestMethod]
        public void ReadInputRegistersResponseRtuTest()
        {
            string expected = "01-04-04-27-10-C3-50-A0-39";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadInputRegistersResponse inputs = ReadInputRegistersResponse.Decode(message);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadInputRegistersResponseTcpTest()
        {
            string expected = "00-14-00-00-00-07-01-04-04-27-10-C3-50";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadInputRegistersResponse inputs = ReadInputRegistersResponse.Decode(message);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadInputRegistersResponseConvertToRtuTest()
        {
            string expected = "01-04-04-27-10-C3-50-A0-39";
            string loaded = "00-14-00-00-00-07-01-04-04-27-10-C3-50";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadInputRegistersResponse inputs = ReadInputRegistersResponse.Decode(message);
            byte[] actualBytes = inputs.ConvertToRtu();
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadInputRegistersResponseConvertToTcpTest()
        {
            string loaded = "01-04-04-27-10-C3-50-A0-39";
            string expected = "00-14-00-00-00-07-01-04-04-27-10-C3-50";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadInputRegistersResponse inputs = ReadInputRegistersResponse.Decode(message);
            byte[] actualBytes = inputs.ConvertToTcp(1, 20, 0);
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadInputRegistersResponseSerializeRtuTest()
        {
            string expected = "01-04-04-27-10-C3-50-A0-39";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            ReadInputRegistersResponse inputs = ReadInputRegistersResponse.Decode(message);
            string jsonString = inputs.Serialize();
            ReadInputRegistersResponse inputs2 = JsonConvert.DeserializeObject<ReadInputRegistersResponse>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadInputRegistersResponseSerializeTcpTest()
        {
            string expected = "00-14-00-00-00-07-01-04-04-27-10-C3-50";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            ReadInputRegistersResponse inputs = ReadInputRegistersResponse.Decode(message);
            string jsonString = inputs.Serialize();
            ReadInputRegistersResponse inputs2 = JsonConvert.DeserializeObject<ReadInputRegistersResponse>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }
}
