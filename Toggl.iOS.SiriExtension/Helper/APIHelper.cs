using System.Net.Http;
using Foundation;
using Toggl.iOS.ExtensionKit;
using Toggl.Networking;
using Toggl.Networking.Network;

namespace SiriExtension
{
    public class APIHelper
    {
        #if USE_PRODUCTION_API
        private const ApiEnvironment environment = ApiEnvironment.Production;
        #else
        private const ApiEnvironment environment = ApiEnvironment.Staging;
        #endif

        public static ITogglApi GetTogglAPI()
        {
            var apiToken = SharedStorage.instance.GetApiToken();
            if (apiToken == null)
            {
                return null;
            }

            var version = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();
            var userAgent = new UserAgent("Daneel", $"{version} SiriExtension");
            var apiConfiguration = new ApiConfiguration(environment, Credentials.WithApiToken(apiToken), userAgent);
            var httpClientHandler = new NSUrlSessionHandler(NSUrlSessionConfiguration.DefaultSessionConfiguration);
            return TogglApiFactory.WithConfiguration(apiConfiguration, httpClientHandler);
        }
    }
}
