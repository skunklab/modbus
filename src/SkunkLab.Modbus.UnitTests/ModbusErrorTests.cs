using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SkunkLab.Modbus.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class ModbusErrorTests
    {
        [TestMethod]
        public void ErrorRtuTest()
        {
            string expected = "0A-81-02-B0-53";
            ModbusError error = ModbusError.Create(10, 1, ModbusErrorCode.IllegalDataAddress);
            byte[] msg = error.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ErrorTcpTest()
        {
            string expected = "00-05-00-00-00-03-0A-81-02";
            ModbusError error = ModbusError.Create(10, 5, 0, 1, ModbusErrorCode.IllegalDataAddress);
            byte[] msg = error.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ErrorConvertToRtuTest()
        {
            string expected = "0A-81-02-B0-53";
            ModbusError error = ModbusError.Create(10, 5, 0, 1, ModbusErrorCode.IllegalDataAddress);
            byte[] msg = error.ConvertToRtu();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void ErrorConvertToTcpTest()
        {
            string expected = "00-05-00-00-00-03-0A-81-02";
            ModbusError error = ModbusError.Create(10, 1, ModbusErrorCode.IllegalDataAddress);
            byte[] msg = error.ConvertToTcp(10, 5, 0);
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ErrorSerializeRtuTest()
        {
            string expected = "0A-81-02-B0-53";
            ModbusError error = ModbusError.Create(10, 1, ModbusErrorCode.IllegalDataAddress);
            string jstring = JsonConvert.SerializeObject(error);
            ModbusError error2 = JsonConvert.DeserializeObject<ModbusError>(jstring);
            string actual = System.BitConverter.ToString(error2.Encode());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ErrorSerializeTcpTest()
        {
            string expected = "00-05-00-00-00-03-0A-81-02";
            ModbusError error = ModbusError.Create(10, 5, 0, 1, ModbusErrorCode.IllegalDataAddress);
            string jstring = JsonConvert.SerializeObject(error);
            ModbusError error2 = JsonConvert.DeserializeObject<ModbusError>(jstring);
            string actual = System.BitConverter.ToString(error2.Encode());
            Assert.AreEqual(expected, actual);

        }
    }
}
