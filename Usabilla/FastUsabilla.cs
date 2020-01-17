using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Usabilla
{
    public class FastUsabilla
    {
        private string mClientKey;
        private string mSecretKey;

        public FastUsabilla(string clientKey, string secretKey)
        {
            mClientKey = clientKey;
            mSecretKey = secretKey;
        }

        public string GetResource(string url)
        {
            var uri = new Uri(url);
            var t = DateTime.UtcNow;
            var usbldate = t.ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT";
            var datestamp = t.ToString("yyyyMMdd");
            var longDate = t.ToString("yyyyMMddTHHmmssZ");

            // step1 create a canonical request
            var method = "GET";
            var cUri = uri.AbsolutePath;
            var cHeaders = $"date:{usbldate}\nhost:{uri.Host}\n";
            var signedHeaders = "date;host";
            var payloadHash = ComputeSha256Hash("");
            var cRequest = $"{method}\n{cUri}\n{uri.Query.Replace("?", "")}\n{cHeaders}\n{signedHeaders}\n{payloadHash}";

            // step2 create a string to sign
            var algorithm = "USBL1-HMAC-SHA256";
            var credentialScope = $"{datestamp}/usbl1_request";
            var string2Sign = $"{algorithm}\n{longDate}\n{credentialScope}\n{ComputeSha256Hash(cRequest)}";

            // step3 calculate the signature
            var signingKey = GetSignatureKey(datestamp);
            var signature = ByteArrayToHexString(HashHMAC(signingKey, Encoding.UTF8.GetBytes(string2Sign)));

            // step4 signing the request
            var authHeader = $"{algorithm} Credential={mClientKey}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";

            var httpClient = new HttpClient();
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("date", usbldate);
            headers.TryAddWithoutValidation("Authorization", authHeader);

            var response = httpClient.GetAsync(uri).Result;

            return response.Content.ReadAsStringAsync().Result;
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                return ByteArrayToHexString(bytes);
            }
        }

        private byte[] HashHMAC(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }

        private byte[] GetSignatureKey(string datestamp)
        {
            var kDate = HashHMAC(Encoding.UTF8.GetBytes($"USBL1{mSecretKey}"), Encoding.UTF8.GetBytes(datestamp));
            return HashHMAC(kDate, Encoding.UTF8.GetBytes("usbl1_request"));
        }

        private string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
