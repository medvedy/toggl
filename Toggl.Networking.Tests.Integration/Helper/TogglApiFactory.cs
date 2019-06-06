using System.Net.Http;
using Toggl.Networking.Network;
using static System.Net.DecompressionMethods;

namespace Toggl.Networking.Tests.Integration.Helper
{
    internal static class TogglApiFactory
    {
        public static ITogglApi TogglApiWith(Credentials credentials)
        {
            var apiConfiguration = configurationFor(credentials);
            var managedHandler = new HttpClientHandler { AutomaticDecompression = GZip | Deflate };
            var apiClient = Networking.TogglApiFactory.CreateDefaultApiClient(apiConfiguration.UserAgent, managedHandler);
            var retryingApiClient = new RetryingApiClient(apiClient);

            return new TogglApi(apiConfiguration, retryingApiClient);
        }

        private static ApiConfiguration configurationFor(Credentials credentials)
            => new ApiConfiguration(ApiEnvironment.Staging, credentials, Configuration.UserAgent);
    }
}
