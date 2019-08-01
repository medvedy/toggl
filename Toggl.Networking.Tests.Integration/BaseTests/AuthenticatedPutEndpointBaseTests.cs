using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Exceptions;

namespace Toggl.Networking.Tests.Integration.BaseTests
{
    public abstract class AuthenticatedPutEndpointBaseTests<T> : AuthenticatedEndpointBaseTests<T>
    {
        protected sealed override async Task<T> CallEndpointWith(ITogglApi api)
        {
            try
            {
                var entityToUpdate = await PrepareForCallingUpdateEndpoint(ValidApi);
                return await CallUpdateEndpoint(api, entityToUpdate);
            }
            catch (ApiException e)
            {
                throw new InvalidOperationException("Preparation for calling the update endpoint itself failed.", e);
            }
        }

        protected abstract Task<T> PrepareForCallingUpdateEndpoint(ITogglApi api);

        protected abstract Task<T> CallUpdateEndpoint(ITogglApi api, T entityToUpdate);
    }
}
