using System;
using System.Linq;
using Usabilla;

namespace ExampleEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an API client with access key and secret key
            var api = new APIClient("YOUR-ACCESS-KEY", "YOUR-SECRET-KEY");

            // Get all email widgets for this account
            var widgets = api.GetResource(APIClient.SCOPE_LIVE, APIClient.PRODUCT_EMAIL, APIClient.RESOURCE_BUTTON);
            var firstWidget = widgets.First();
            Console.WriteLine(firstWidget.name);

            // Get the feedback of the first email widget
            string resourceId = firstWidget.id;
            var feedback = api.GetResource(APIClient.SCOPE_LIVE, APIClient.PRODUCT_EMAIL, APIClient.RESOURCE_FEEDBACK, resourceId, true);
            Console.WriteLine(feedback.Count());

            Console.ReadKey();
        }
    }
}