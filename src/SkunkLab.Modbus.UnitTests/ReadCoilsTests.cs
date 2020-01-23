using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SkunkLab.Modbus.Messaging;
using System.Net.Http.Headers;

namespace SkunkLab.Modbus.UnitTests
{
    [TestClass]
    public class CoilsTests
    {
        #region code 1 tests
        [TestMethod]
        public void CoilsCode1RtuTest()
        {
            string expected = "04-01-00-0A-00-0D-DD-98";
            ReadCoils coils = ReadCoils.Create(4, 10, 13);
            //Coils coils = new Coils(1, 4, 10, 13);
            byte[] msg = coils.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }



        [TestMethod]
        public void CoilsCode1TcpTest()
        {
            string expected = "00-01-00-00-00-06-04-01-00-0A-00-0D";
            ReadCoils coils = ReadCoils.Create(4, 1, 0, 10, 13);
            //Coils coils = new Coils(4, 1, 0, 1, 10, 13);
            byte[] msg = coils.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void CoilsCode1ConvertToRtuTest()
        {
            string expected = "04-01-00-0A-00-0D-DD-98";
            ReadCoils coils = ReadCoils.Create(4, 1, 0, 10, 13);
            //Coils coils = new Coils(4, 1, 0, 1, 10, 13);
            byte[] m = coils.Encode();
            byte[] msg = coils.ConvertToRtu();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void CoilsCode1ConvertToTcpTest()
        {
            string expected = "00-01-00-00-00-06-04-01-00-0A-00-0D";
            ReadCoils coils = ReadCoils.Create(4, 10, 13);
            //Coils coils = new Coils(1, 4, 10, 13);
            byte[] msg = coils.ConvertToTcp(4, 1, 0);
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CoilsCode1SerializeRtuTest()
        {
            string expected = "04-01-00-0A-00-0D-DD-98";
            ReadCoils coils = ReadCoils.Create(4, 10, 13);
            //Coils coils = new Coils(1, 4, 10, 13);
            string jsonString = coils.Serialize();
            ReadCoils coils2 = JsonConvert.DeserializeObject<ReadCoils>(jsonString);
            //Coils coils2 = JsonConvert.DeserializeObject<Coils>(jsonString);
            byte[] msg = coils2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CoilsCode1SerializeTcpTest()
        {
            string expected = "00-01-00-00-00-06-04-01-00-0A-00-0D";
            ReadCoils coils = ReadCoils.Create(4, 1, 0, 10, 13);

            //Coils coils = new Coils(4, 1, 0, 1, 10, 13);
            string jsonString = coils.Serialize();
            ReadCoils coils2 = JsonConvert.DeserializeObject<ReadCoils>(jsonString);
            //Coils coils2 = JsonConvert.DeserializeObject<Coils>(jsonString);
            byte[] msg = coils2.Encode();
            string actual = System.BitConverter.ToString(msg);
            Assert.AreEqual(expected, actual);
        }

        #endregion

        
    }
}
