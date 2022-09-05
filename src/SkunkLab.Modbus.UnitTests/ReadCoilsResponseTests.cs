using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkunkLab.Modbus.Messaging;
using System;
using System.Linq;
using System.Text.Json;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class ReadCoilsResponseTests
    {
        [TestMethod]
        public void ReadCoilsResponseRtuTest()
        {
            string expected = "11-01-05-CD-6B-B2-0E-1B-45-E6";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadCoilsResponse coils = ReadCoilsResponse.Decode(message);
            byte[] msg = coils.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }



        [TestMethod]
        public void ReadCoilsResponseTcpTest()
        {
            string expected = "00-01-00-00-00-08-04-01-05-CD-6B-B2-0E-1B";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadCoilsResponse coils = ReadCoilsResponse.Decode(message);
            byte[] msg = coils.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadCoilsResponseConvertToRtuTest()
        {
            string expected = "11-01-05-CD-6B-B2-0E-1B-45-E6";
            string loaded = "00-01-00-00-00-08-11-01-05-CD-6B-B2-0E-1B";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadCoilsResponse coils = ReadCoilsResponse.Decode(message);
            byte[] actualBytes = coils.ConvertToRtu();
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadCoilsResponseConvertToTcpTest()
        {
            string loaded = "11-01-05-CD-6B-B2-0E-1B-45-E6";
            string expected = "00-01-00-00-00-08-04-01-05-CD-6B-B2-0E-1B";
            string hex = loaded.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();

            ReadCoilsResponse coils = ReadCoilsResponse.Decode(message);
            byte[] actualBytes = coils.ConvertToTcp(4, 1, 0);
            string actual = System.BitConverter.ToString(actualBytes);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadCoilsResponseSerializeRtuTest()
        {
            string expected = "11-01-05-CD-6B-B2-0E-1B-45-E6";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            ReadCoilsResponse coils = ReadCoilsResponse.Decode(message);
            string jsonString = coils.Serialize();
            ReadCoilsResponse coils2 = JsonSerializer.Deserialize<ReadCoilsResponse>(jsonString);
            byte[] msg = coils2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadCoilsResponseSerializeTcpTest()
        {
            string expected = "00-01-00-00-00-08-04-01-05-CD-6B-B2-0E-1B";
            string hex = expected.Replace("-", "");
            byte[] message = Enumerable.Range(0, hex.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                     .ToArray();
            ReadCoilsResponse coils = ReadCoilsResponse.Decode(message);
            string jsonString = coils.Serialize();
            ReadCoilsResponse coils2 = JsonSerializer.Deserialize<ReadCoilsResponse>(jsonString);
            byte[] msg = coils2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }
}
