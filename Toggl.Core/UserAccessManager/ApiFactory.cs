using System.Net.Http;
using Toggl.Shared;
using Toggl.Networking;
using Toggl.Networking.Network;

namespace Toggl.Core.Login
{
    public sealed class ApiFactory : IApiFactory
    {
        private readonly HttpMessageHandler httpClientHandler;

        public UserAgent UserAgent { get; }
        public ApiEnvironment Environment { get; }

        public ApiFactory(ApiEnvironment apiEnvironment, UserAgent userAgent, HttpMessageHandler httpClientHandler)
        {
            Ensure.Argument.IsNotNull(userAgent, nameof(userAgent));

            UserAgent = userAgent;
            Environment = apiEnvironment;

            this.httpClientHandler = httpClientHandler;
        }

        public ITogglApi CreateApiWith(Credentials credentials)
        {
            var configuration = new ApiConfiguration(Environment, credentials, UserAgent);
            return TogglApiFactory.WithConfiguration(configuration, httpClientHandler);
        }
    }
}
