using System.Net.Http;
using Toggl.Shared;
using Toggl.Networking;
using Toggl.Networking.Network;

namespace Toggl.Core.Login
{
    public sealed class ApiFactory : IApiFactory
    {
        private readonly HttpMessageHandler httpHandler;

        public UserAgent UserAgent { get; }
        public ApiEnvironment Environment { get; }

        public ApiFactory(ApiEnvironment apiEnvironment, UserAgent userAgent, HttpMessageHandler httpHandler)
        {
            Ensure.Argument.IsNotNull(userAgent, nameof(userAgent));
            Ensure.Argument.IsNotNull(httpHandler, nameof(httpHandler));

            UserAgent = userAgent;
            Environment = apiEnvironment;

            this.httpHandler = httpHandler;
        }

        public ITogglApi CreateApiWith(Credentials credentials)
        {
            var configuration = new ApiConfiguration(Environment, credentials, UserAgent);
            return TogglApiFactory.WithConfiguration(configuration, httpHandler);
        }
    }
}
