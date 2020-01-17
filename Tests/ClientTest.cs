using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Usabilla;

namespace Tests
{
    [TestClass]
    public class ClientTest
    {
        private string mClientKey, mSecretKey;
        private Credentials mCredentials;
        private APIClient mClient;
        public ClientTest()
        {
            mClientKey = "ACCESS-KEY";
            mSecretKey = "SECRET-KEY";
            mCredentials = new Credentials(mClientKey, mSecretKey);
            mClient = new APIClient(mClientKey, mSecretKey);
        }

        [TestMethod]
        public void TestSignKey()
        {
            var signedKey = mClient.Sign(Encoding.UTF8.GetBytes(mSecretKey), Encoding.UTF8.GetBytes("usbl1_request"));
            Assert.Equals(signedKey, new byte[] { });
        }

        [TestMethod]
        public void TestGetSignatureKey()
        {
            var datestamp = "20150115";
            var signingKey = mClient.GetSignatureKey(mSecretKey, datestamp);
            Assert.Equals(signingKey, new byte[] { });
        }

        [TestMethod]
        public void TestQueryParameters()
        {
            var parameters = new Dictionary<string, string> { { "limit", "1" } };
            mClient.SetQueryParameters(parameters);
            Assert.Equals(mClient.GetQueryParameters(), "limit=1");

            parameters = new Dictionary<string, string> { { "limit", "1" }, { "since", "1235454" } };
            mClient.SetQueryParameters(parameters);
            Assert.Equals(mClient.GetQueryParameters(), "limit=1&since=1235454");
        }
    }
}