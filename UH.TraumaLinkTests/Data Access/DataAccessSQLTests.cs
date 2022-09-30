using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCMLib.Context;

namespace UH.TraumaLink.Tests
{
    [TestClass()]
    public class DataAccessSQLTests
    {
        private DataAccessSQL _Access;
        public CustomContextObj _CustomContext;

        [TestInitialize]
        public void Initialize()
        {
            _Access = new DataAccessSQL();
            _CustomContext = CustomContextObj.GetInstance();
        }

        [TestMethod()]
        public void DataAccessSQLTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetSettingsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetLinkedTraumaPatientsTest()
        {
            Assert.Fail();
        }
    }
}