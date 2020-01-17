using Microsoft.VisualStudio.TestTools.UnitTesting;
using Usabilla;

namespace Tests
{
    [TestClass]
    public class CredentialsTest
    {
        private string mClientKey, mSecretKey;
        public CredentialsTest()
        {
            mClientKey = "ACCESS-KEY";
            mSecretKey = "SECRET-KEY";
        }

        [TestMethod]
        public void TestCredentials()
        {
            var credentials = new Credentials(mClientKey, mSecretKey);
            Assert.IsInstanceOfType(credentials, typeof(Credentials));
        }

        [TestMethod]
        public void TestGetCredentials()
        {
            var credentials = new Credentials(mClientKey, mSecretKey);
            var clientCredentials = credentials.GetCredentials();
            var clientKey = clientCredentials["client_key"];
            var secretKey = clientCredentials["secret_key"];
            Assert.Equals(mClientKey, clientKey);
            Assert.Equals(mSecretKey, secretKey);
        }
    }
}