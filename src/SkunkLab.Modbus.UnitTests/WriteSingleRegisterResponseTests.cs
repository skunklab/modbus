using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SkunkLab.Modbus.Messaging;
using System;
using System.Linq;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class WriteSingleRegisterResponseTests
    {
        [TestMethod]
        public void WriteSingleRegisterResponseRtuTest()
        {
            string expected = "11-06-00-01-00-03-9A-9B";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            WriteSingleRegisterResponse inputs = WriteSingleRegisterResponse.Decode(message);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleRegisterResponseTcpTest()
        {
            string expected = "00-1E-00-00-00-06-11-06-00-01-00-03";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            WriteSingleRegisterResponse inputs = WriteSingleRegisterResponse.Decode(message);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleRegisterResponseConvertToRtuTest()
        {
            string expected = "11-06-00-01-00-03-9A-9B";
            string loaded = "00-1E-00-00-00-06-11-06-00-01-00-03";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            WriteSingleRegisterResponse inputs = WriteSingleRegisterResponse.Decode(message);
            byte[] actualBytes = inputs.ConvertToRtu();
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleRegisterResponseConvertToTcpTest()
        {
            string loaded = "11-06-00-01-00-03-9A-9B";
            string expected = "00-1E-00-00-00-06-01-06-00-01-00-03";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            WriteSingleRegisterResponse inputs = WriteSingleRegisterResponse.Decode(message);
            byte[] actualBytes = inputs.ConvertToTcp(1, 30, 0);
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleRegisterResponseSerializeRtuTest()
        {
            string expected = "11-06-00-01-00-03-9A-9B";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            WriteSingleRegisterResponse inputs = WriteSingleRegisterResponse.Decode(message);
            string jsonString = inputs.Serialize();
            WriteSingleRegisterResponse inputs2 = JsonConvert.DeserializeObject<WriteSingleRegisterResponse>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleRegisterResponseSerializeTcpTest()
        {
            string expected = "00-1E-00-00-00-06-11-06-00-01-00-03";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            WriteSingleRegisterResponse inputs = WriteSingleRegisterResponse.Decode(message);
            string jsonString = inputs.Serialize();
            WriteSingleRegisterResponse inputs2 = JsonConvert.DeserializeObject<WriteSingleRegisterResponse>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }
}
