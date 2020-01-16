using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Usabilla
{
    /// <summary>
    /// An object that holds information about client and secret key
    /// </summary>
    public class Credentials
    {
        public string ClientKey { get; set; }
        public string SecretKey { get; set; }

        /// <summary>
        /// Initialize a Credentials instance
        /// </summary>
        /// <param name="clientKey"></param>
        /// <param name="secretKey"></param>
        public Credentials(string clientKey, string secretKey)
        {
            ClientKey = clientKey;
            SecretKey = secretKey;
        }

        /// <summary>
        /// Return the client and secret key
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetCredentials()
        {
            return new Dictionary<string, string>
            {
                { "client_key", ClientKey},
                { "secret_key", SecretKey}
            };
        }
    }

    /// <summary>
    /// GeneralError API exception
    /// </summary>
    public class GeneralError : Exception
    {
        public string Type { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Initialize a GeneralError exception
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public GeneralError(string type, string message)
        {
            Type = type;
            Message = message;
        }

        /// <summary>
        /// String representation of the exception
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Type} ({Message})";
        }
    }

    /// <summary>
    /// APIClient object
    /// </summary>
    public class APIClient
    {
        /**
         * For the key derivation functions see:
            http://docs.aws.amazon.com/general/latest/gr/signature-v4-examples.html#signature-v4-examples-python
         * **/

        /**
         * resources = {
                'scopes': {
                    'live': {
                        'products': {
                            'websites': {
                                'resources': {
                                    'button': '/button',
                                    'feedback': '/button/:id/feedback',
                                    'campaign': '/campaign',
                                    'campaign_result': '/campaign/:id/results',
                                    'campaign_stats': '/campaign/:id/stats',
                                    'inpage': '/inpage',
                                    'inpage_result': '/inpage/:id/feedback'
                                }
                            },
                            'email': {
                                'resources': {
                                    'button': '/button',
                                    'feedback': '/button/:id/feedback'
                                }
                            },
                            'apps': {
                                'resources': {
                                    'app': '',
                                    'feedback': '/:id/feedback',
                                    'campaign': '/campaign',
                                    'campaign_result': '/campaign/:id/results'
                                }
                            }
                        }
                    }
                }
            }
         * **/
        
        // Scope constants
        public static readonly string SCOPE_LIVE = "live";

        // Product constants
        public static readonly string PRODUCT_WEBSITES = "websites";
        public static readonly string PRODUCT_EMAIL = "email";
        public static readonly string PRODUCT_APPS = "apps";

        // Resource constants
        public static readonly string RESOURCE_FEEDBACK = "feedback";
        public static readonly string RESOURCE_APP = "app";
        public static readonly string RESOURCE_BUTTON = "button";
        public static readonly string RESOURCE_CAMPAIGN = "campaign";
        public static readonly string RESOURCE_CAMPAIGN_RESUTL = "campaign_result";
        public static readonly string RESOURCE_CAMPAIGN_STATS = "campaign_stats";
        public static readonly string RESOURCE_INPAGE = "inpage";
        public static readonly string RESOURCE_INPAGE_RESULT = "inpage_result";

        private string mMethod = "GET";
        private string mHost = "data.usabilla.com";
        private string mHostProtocol = "https://";
        public Credentials Credentials { get; set; }
        public string QueryParameters { get; set; }
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> Resource { get; set; }

        /// <summary>
        /// Initialize an APIClient object
        /// </summary>
        /// <param name="clientKey"></param>
        /// <param name="secretKey"></param>
        public APIClient(string clientKey, string secretKey)
        {
            QueryParameters = "";
            Credentials = new Credentials(clientKey, secretKey);

            // Construct scope, product and resource
            Resource = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>
            {
                { "live",
                    new Dictionary<string, Dictionary<string, string>>
                    {
                        { "websites",
                            new Dictionary<string, string>
                            {
                                { "button","/button"},
                                { "feedback","/button/:id/feedback"},
                                { "campaign","/campaign"},
                                { "campaign_result","/campaign/:id/results"},
                                { "campaign_stats","/campaign/:id/stats"},
                                { "inpage","/inpage"},
                                { "inpage_result","/inpage/:id/feedback"}
                            }
                        },
                        { "email",
                            new Dictionary<string, string>
                            {
                                { "button","/button"},
                                { "feedback","/button/:id/feedback"}
                            }
                        },
                        { "apps",
                            new Dictionary<string, string>
                            {
                                { "app",""},
                                { "feedback","/button/:id/feedback"},
                                { "campaign","/campaign"},
                                { "campaign_result","/campaign/:id/results"}
                            }
                        },
                    }
                }
            };
        }

        /// <summary>
        /// Get the digest of the message using the specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public byte[] Sign(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }

        /// <summary>
        /// Get the signature key
        /// </summary>
        /// <param name="datestamp"></param>
        /// <returns></returns>
        public byte[] GetSignatureKey(string key, string datestamp)
        {
            var kDate = Sign(Encoding.UTF8.GetBytes($"USBL1{key}"), Encoding.UTF8.GetBytes(datestamp));
            return Sign(kDate, Encoding.UTF8.GetBytes("usbl1_request"));
        }
        /// <summary>
        /// Set the query parameters
        /// </summary>
        /// <param name="parameters">A dict representing the query parameters to be used for the request</param>
        public void SetQueryParameters(Dictionary<string,string> parameters)
        {
            QueryParameters = string.Join("&", parameters
                                    .OrderBy(kvp => kvp.Key)
                                    .Select(kvp => string.Format("{0}={1}", HttpUtility.UrlEncode(kvp.Key), HttpUtility.UrlEncode(kvp.Value))));
        }

        public string GetQueryParameters()
        {
            return QueryParameters;
        }

        /// <summary>
        /// Send the signed request to the API
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public dynamic SendSignedRequest(string scope)
        {
            /**
             * The process is the following:
                1) Create a canonical request
                2) Create a string to sign
                3) Calculate the signature
                4) Sign the request
                5) Send the request
             * **/

            if (Credentials.ClientKey == null || Credentials.SecretKey == null)
                throw new GeneralError("Invalid Access Key.", "The Access Key supplied is invalid.");

            // Create a date for headers and the credentials string
            var t = DateTime.UtcNow;
            var usbldate = t.ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT";
            var datestamp = t.ToString("yyyyMMdd");
            var longDate = t.ToString("yyyyMMddTHHmmssZ");

            // Create canonical URI - the part of the URI from domain to query
            var canonicalUri = scope;

            // Create the canonical query string
            var canonicalQueryString = GetQueryParameters();

            // Create the canonical headers and signed headers
            var canonicalHeaders = $"date:{usbldate}\nhost:{mHost}\n";

            // Create the list of signed headers
            var signedHeaders = "date;host";

            // Create payload hash (hash of the request body content)
            var payloadHash = ComputeSha256Hash("");

            // Combine elements to create canonical request
            var canonicalRequest = $"{mMethod}\n{canonicalUri}\n{canonicalQueryString}\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";

            // Match the algorithm to the hashing algorithm you use
            var algorithm = "USBL1-HMAC-SHA256";
            var credentialScope = $"{datestamp}/usbl1_request";
            var string2Sign = $"{algorithm}\n{longDate}\n{credentialScope}\n{ComputeSha256Hash(canonicalRequest)}";

            // Create the signing key
            var signingKey = GetSignatureKey(Credentials.SecretKey, datestamp);

            // Sign the string2Sign using the signingKey
            var signature = HexDigest(Sign(signingKey, Encoding.UTF8.GetBytes(string2Sign)));

            // Construct the authorization header
            var authorizationHeader = $"{algorithm} Credential={Credentials.ClientKey}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";

            // Send the request
            var httpClient = new HttpClient();
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("date", usbldate);
            headers.TryAddWithoutValidation("Authorization", authorizationHeader);
            var requestUrl = $"{mHostProtocol}{mHost}{scope}?{canonicalQueryString}";
            var response = httpClient.GetAsync(requestUrl).Result;

            var body = response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject(body);
        }

        /// <summary>
        /// Checks whether the resource exists
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="product"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public string CheckResourceValidity(string scope, string product, string resource)
        {
            if (!Resource.Keys.Contains(scope))
                throw new GeneralError("invalid scope", "Invalid scope name");
            var foundScope = Resource[scope];
            if(!foundScope.Keys.Contains(product))
                throw new GeneralError("invalid product", "Invalid product name");
            var foundProduct = foundScope[product];
            if (!foundProduct.Keys.Contains(resource))
                throw new GeneralError("invalid resource", "Invalid resource name");
            var foundResource = foundProduct[resource];

            return $"/{scope}/{product}{foundResource}";
        }

        /// <summary>
        /// Replaces the :id pattern in the url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public string HandleId(string url, string resourceId)
        {
            if (resourceId != null)
            {
                if (resourceId == "")
                    throw new GeneralError("invalid id", "Invalid resource ID");
                if (resourceId == "*")
                    resourceId = "%2A";
                url = url.Replace(":id", resourceId);
            }
            return url;
        }

        /// <summary>
        /// Get items using an iterator
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> ItemIterator(string url)
        {
            bool hasMore = true;
            while (hasMore)
            {
                var results = SendSignedRequest(url);
                hasMore = results.hasMore;
                foreach (var item in results.items)
                {
                    yield return item;
                }
                SetQueryParameters(new Dictionary<string, string> { { "since", Convert.ToString(results.lastTimestamp) } });
            }
        }

        /// <summary>
        /// Retrieves resources of the specified type
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="product"></param>
        /// <param name="resource"></param>
        /// <param name="resourceId"></param>
        /// <param name="iterate"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetResource(string scope, string product, string resource, string resourceId=null, bool iterate=false)
        {
            var url = HandleId(CheckResourceValidity(scope, product, resource), resourceId);
            if (iterate)
                return ItemIterator(url);
            else
                return new List<dynamic> { SendSignedRequest(url) };
        }

        /// <summary>
        /// Equivalent to python api hashlib.sha256().hexdigest()
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                return HexDigest(bytes);
            }
        }

        static string HexDigest(byte[] bytes)
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