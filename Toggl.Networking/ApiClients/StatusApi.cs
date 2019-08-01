using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Toggl.Networking.Helpers;
using Toggl.Networking.Network;
using Toggl.Shared;

namespace Toggl.Networking.ApiClients
{
    internal sealed class StatusApi : IStatusApi
    {
        private readonly StatusEndpoints endpoints;
        private readonly IApiClient apiClient;

        public StatusApi(Endpoints endpoints, IApiClient apiClient)
        {
            this.endpoints = endpoints.Status;
            this.apiClient = apiClient;
        }

        public async Task<Either<Unit, Exception>> IsAvailable()
        {
            try
            {
                var endpoint = endpoints.Get;
                var request = new Request("", endpoint.Url, Enumerable.Empty<HttpHeader>(), endpoint.Method);
                var response = await apiClient.Send(request).ConfigureAwait(false);

                if (response.IsSuccess)
                    return Either<Unit, Exception>.WithLeft(Unit.Default);

                var error = ApiExceptions.For(request, response);
                return Either<Unit, Exception>.WithRight(error);
            }
            catch (Exception exception)
            {
                return Either<Unit, Exception>.WithRight(exception);
            }
        }
    }
}
