using System;
using System.Linq;
using Usabilla;

namespace ExamplesWebsites
{
    /// <summary>
    /// Examples Usabilla for websites
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Create an API client with access key and secret key
            var api = new APIClient("YOUR-ACCESS-KEY", "YOUR-SECRET-KEY");

            // Get all buttons for this account
            var buttons = api.GetResource(APIClient.SCOPE_LIVE, APIClient.PRODUCT_WEBSITES, APIClient.RESOURCE_BUTTON);
            var firstButton = buttons.First();
            Console.WriteLine(firstButton.name);

            // Get the feedback items for the first button of the list
            string resourceId = firstButton.id;
            var feedbackItems = api.GetResource(APIClient.SCOPE_LIVE, APIClient.PRODUCT_WEBSITES, APIClient.RESOURCE_FEEDBACK, resourceId, true);
            Console.WriteLine(feedbackItems.Count());


            // ------------------------------------------
            // Get all campaigns for this account
            var campaigns = api.GetResource(APIClient.SCOPE_LIVE, APIClient.PRODUCT_WEBSITES, APIClient.RESOURCE_CAMPAIGN);
            var firstCampaign = campaigns.First();
            Console.WriteLine(firstCampaign.name);

            // Get the response of the first campaign
            resourceId = firstCampaign.id;
            var responses = api.GetResource(APIClient.SCOPE_LIVE, APIClient.PRODUCT_WEBSITES, APIClient.RESOURCE_CAMPAIGN_RESUTL, resourceId, true);
            Console.WriteLine(responses.Count());

            // Get the stats of the first campaign
            var stats = api.GetResource(APIClient.SCOPE_LIVE, APIClient.PRODUCT_WEBSITES, APIClient.RESOURCE_CAMPAIGN_STATS, resourceId);
            Console.WriteLine(stats);


            // ------------------------------------------
            // in-page is not yet available
            // Get all In-Page Widgets for this account
            var widgets = api.GetResource(APIClient.SCOPE_LIVE, APIClient.PRODUCT_WEBSITES, APIClient.RESOURCE_INPAGE);
            Console.WriteLine(widgets);

            // Get one specific widget
            var specificWidget = widgets.ElementAt(0);
            Console.WriteLine(specificWidget);

            Console.ReadKey();
        }
    }
}
