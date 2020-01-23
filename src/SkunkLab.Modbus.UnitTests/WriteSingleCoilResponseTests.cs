using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SkunkLab.Modbus.Messaging;
using System;
using System.Linq;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class WriteSingleCoilResponseTests
    {
        [TestMethod]
        public void WriteSingleCoilResponseRtuTest()
        {
            string expected = "01-05-00-64-FF-00-CD-E5";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            WriteSingleCoilResponse inputs = WriteSingleCoilResponse.Decode(message);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleCoilResponseTcpTest()
        {
            string expected = "00-19-00-00-00-06-01-05-00-64-FF-00";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            WriteSingleCoilResponse inputs = WriteSingleCoilResponse.Decode(message);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleCoilResponseConvertToRtuTest()
        {
            string expected = "01-05-00-64-FF-00-CD-E5";
            string loaded = "00-19-00-00-00-06-01-05-00-64-FF-00";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            WriteSingleCoilResponse inputs = WriteSingleCoilResponse.Decode(message);
            byte[] actualBytes = inputs.ConvertToRtu();
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleCoilResponseConvertToTcpTest()
        {
            string loaded = "01-05-00-64-FF-00-CD-E5";
            string expected = "00-19-00-00-00-06-01-05-00-64-FF-00";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            WriteSingleCoilResponse inputs = WriteSingleCoilResponse.Decode(message);
            byte[] actualBytes = inputs.ConvertToTcp(1, 25, 0);
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleCoilResponseSerializeRtuTest()
        {
            string expected = "01-05-00-64-FF-00-CD-E5";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            WriteSingleCoilResponse inputs = WriteSingleCoilResponse.Decode(message);
            string jsonString = inputs.Serialize();
            WriteSingleCoilResponse inputs2 = JsonConvert.DeserializeObject<WriteSingleCoilResponse>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteSingleCoilResponseSerializeTcpTest()
        {
            string expected = "00-19-00-00-00-06-01-05-00-64-FF-00";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            WriteSingleCoilResponse inputs = WriteSingleCoilResponse.Decode(message);
            string jsonString = inputs.Serialize();
            WriteSingleCoilResponse inputs2 = JsonConvert.DeserializeObject<WriteSingleCoilResponse>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }
}
