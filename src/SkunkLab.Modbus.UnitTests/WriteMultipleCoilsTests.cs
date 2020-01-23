using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SkunkLab.Modbus.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class WriteMultipleCoilsTests
    {
        [TestMethod]
        public void WriteMultipleCoilsRtuTest()
        {
            string expected = "11-0F-00-13-00-0A-02-CD-01-BF-0B";
            BitArray array = new BitArray(new bool[] { true, false, true, true, false, false, true, true, true, false });
            WriteMultipleCoils coils = WriteMultipleCoils.Create(17, 19, array);
            byte[] msg = coils.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleCoilsTcpTest()
        {
            string expected = "00-19-00-00-00-09-01-0F-00-13-00-0A-02-CD-01";
            BitArray array = new BitArray(new bool[] { true, false, true, true, false, false, true, true, true, false });
            WriteMultipleCoils coils = WriteMultipleCoils.Create(1, 25, 0, 19, array);
            byte[] msg = coils.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleCoilsConvertToRtuTest()
        {
            string expected = "11-0F-00-13-00-0A-02-CD-01-BF-0B";
            BitArray array = new BitArray(new bool[] { true, false, true, true, false, false, true, true, true, false });
            WriteMultipleCoils coils = WriteMultipleCoils.Create(17, 25, 0, 19, array);
            byte[] msg = coils.ConvertToRtu();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleCoilsConvertToTcpTest()
        {
            string expected = "00-19-00-00-00-09-01-0F-00-13-00-0A-02-CD-01";
            BitArray array = new BitArray(new bool[] { true, false, true, true, false, false, true, true, true, false });
            WriteMultipleCoils coils = WriteMultipleCoils.Create(17, 19, array);
            byte[] msg = coils.ConvertToTcp(1, 25, 0);
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleCoilsSerializeRtuTest()
        {
            string expected = "11-0F-00-13-00-0A-02-CD-01-BF-0B";
            BitArray array = new BitArray(new bool[] { true, false, true, true, false, false, true, true, true, false });
            WriteMultipleCoils coils = WriteMultipleCoils.Create(17, 19, array);
            string jsonString = coils.Serialize();
            WriteMultipleCoils coil2 = JsonConvert.DeserializeObject<WriteMultipleCoils>(jsonString);
            byte[] msg = coil2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WriteMultipleCoilsSerializeTcpTest()
        {
            string expected = "00-19-00-00-00-09-01-0F-00-13-00-0A-02-CD-01";
            BitArray array = new BitArray(new bool[] { true, false, true, true, false, false, true, true, true, false });
            WriteMultipleCoils coils = WriteMultipleCoils.Create(1, 25, 0, 19, array);
            string jsonString = coils.Serialize();
            WriteMultipleCoils coil2 = JsonConvert.DeserializeObject<WriteMultipleCoils>(jsonString);
            byte[] msg = coil2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }
    }

}
