using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkunkLab.Modbus.Messaging;
using System;
using System.Linq;
using System.Text.Json;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class ReadDiscreteInputsResponseTests
    {
        [TestMethod]
        public void ReadDiscreteInputsResponseRtuTest()
        {
            string expected = "01-02-02-05-00-BA-E8";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadDiscreteInputsResponse inputs = ReadDiscreteInputsResponse.Decode(message);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadDiscreteInputsResponseTcpTest()
        {
            string expected = "00-0A-00-00-00-05-01-02-02-05-00";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadDiscreteInputsResponse inputs = ReadDiscreteInputsResponse.Decode(message);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadDiscreteInputsResponseConvertToRtuTest()
        {
            string expected = "01-02-02-05-00-BA-E8";
            string loaded = "00-0A-00-00-00-05-01-02-02-05-00";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadDiscreteInputsResponse inputs = ReadDiscreteInputsResponse.Decode(message);
            byte[] actualBytes = inputs.ConvertToRtu();
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadDiscreteInputsResponseConvertToTcpTest()
        {
            string loaded = "01-02-02-05-00-BA-E8";
            string expected = "00-0A-00-00-00-05-01-02-02-05-00";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadDiscreteInputsResponse inputs = ReadDiscreteInputsResponse.Decode(message);
            byte[] actualBytes = inputs.ConvertToTcp(1, 10, 0);
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadDiscreteInputsResponseSerializeRtuTest()
        {
            string expected = "01-02-02-05-00-BA-E8";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            ReadDiscreteInputsResponse inputs = ReadDiscreteInputsResponse.Decode(message);
            string jsonString = inputs.Serialize();
            ReadDiscreteInputsResponse inputs2 = JsonSerializer.Deserialize<ReadDiscreteInputsResponse>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadDiscreteInputsResponseSerializeTcpTest()
        {
            string expected = "00-0A-00-00-00-05-01-02-02-05-00";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            ReadDiscreteInputsResponse inputs = ReadDiscreteInputsResponse.Decode(message);
            string jsonString = inputs.Serialize();
            ReadDiscreteInputsResponse inputs2 = JsonSerializer.Deserialize<ReadDiscreteInputsResponse>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }
}
