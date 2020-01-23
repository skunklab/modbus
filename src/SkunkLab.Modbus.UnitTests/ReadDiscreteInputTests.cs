using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SkunkLab.Modbus.Messaging;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class ReadDiscreteInputTests
    {
        [TestMethod]
        public void ReadDiscreteInputRtuTest()
        {
            string expected = "04-02-00-0A-00-0D-99-98";
            ReadDiscreteInputs inputs = ReadDiscreteInputs.Create(4, 10, 13);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadDiscreteInputTcpTest()
        {
            string expected = "00-02-00-00-00-06-04-02-00-0A-00-0D";
            ReadDiscreteInputs inputs = ReadDiscreteInputs.Create(4, 2, 0, 10, 13);
            byte[] msg = inputs.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadDiscreteInputConvertToRtuTest()
        {
            string expected = "04-02-00-0A-00-0D-99-98";
            ReadDiscreteInputs inputs = ReadDiscreteInputs.Create(4, 2, 0, 10, 13);
            byte[] msg = inputs.ConvertToRtu();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadDiscreteInputConvertToTcpTest()
        {
            string expected = "00-02-00-00-00-06-04-02-00-0A-00-0D";
            ReadDiscreteInputs inputs = ReadDiscreteInputs.Create(4, 10, 13);
            byte[] msg = inputs.ConvertToTcp(4, 2, 0);
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ReadDiscreteInputSerializeRtuTest()
        {
            string expected = "04-02-00-0A-00-0D-99-98";
            ReadDiscreteInputs inputs = ReadDiscreteInputs.Create(4, 10, 13);
            string jsonString = inputs.Serialize();
            ReadDiscreteInputs inputs2 = JsonConvert.DeserializeObject<ReadDiscreteInputs>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void ReadDiscreteInputSerializeTcpTest()
        {
            string expected = "00-02-00-00-00-06-04-02-00-0A-00-0D";
            ReadDiscreteInputs inputs = ReadDiscreteInputs.Create(4, 2, 0, 10, 13);
            string jsonString = inputs.Serialize();
            ReadDiscreteInputs inputs2 = JsonConvert.DeserializeObject<ReadDiscreteInputs>(jsonString);
            byte[] msg = inputs2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);

        }
    }
}
