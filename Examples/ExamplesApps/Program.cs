using System;
using System.Collections.Generic;
using System.Linq;
using Usabilla;

namespace ExamplesApps
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an API client with access key and secret key
            var api = new APIClient("YOUR-ACCESS-KEY", "YOUR-SECRET-KEY");

            // Set the limit of buttons to retrieve to 1
            if (false)
                api.SetQueryParameters(new Dictionary<string, string> { { "limit", "1" } });

            // Set a limit for last 7 days
            // NB: You might need to convert the since_unix calculation from scientific notation or set the since parameter manually
            if (false)
            {
                var epoch = new DateTime(1970, 1, 1);
                var since = new TimeSpan(7, 0, 0, 0);
                var sinceUnix = (DateTime.UtcNow - since - epoch).TotalMilliseconds;
                api.SetQueryParameters(new Dictionary<string, string> { { "since", sinceUnix.ToString() } });
            }

            // Get all apps forms for this account
            var forms = api.GetResource(APIClient.SCOPE_LIVE, APIClient.PRODUCT_APPS, APIClient.RESOURCE_APP);
            var firstForm = forms.First();
            Console.WriteLine(firstForm.name);

            // Get the feedback of the first app form
            string resourceId = firstForm.id;
            var feedbackItems = api.GetResource(APIClient.SCOPE_LIVE, APIClient.PRODUCT_APPS, APIClient.RESOURCE_FEEDBACK, resourceId, true);
            Console.WriteLine(feedbackItems.Count());

            // Campaigns for apps
            var appCampaigns = api.GetResource(APIClient.SCOPE_LIVE, APIClient.PRODUCT_APPS, APIClient.RESOURCE_CAMPAIGN);
            var firstCampaign = appCampaigns.First();
            resourceId = firstCampaign.id;
            var responses = api.GetResource(APIClient.SCOPE_LIVE, APIClient.PRODUCT_WEBSITES, APIClient.RESOURCE_CAMPAIGN_RESUTL, resourceId, true);
            Console.WriteLine(responses.Count());

            Console.ReadKey();
        }
    }
}