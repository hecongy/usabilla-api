using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Usabilla;

namespace Tests
{
    [TestClass]
    public class GeneralErrorTest
    {
        private GeneralError mError;
        public GeneralErrorTest()
        {
            mError = new GeneralError("type", "message");
        }

        [TestMethod]
        public void TestStr()
        {
            Assert.Equals(mError.ToString(), "type (message)");
        }
    }
}