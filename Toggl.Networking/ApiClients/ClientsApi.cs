using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    internal sealed class ClientsApi : BaseApi, IClientsApi
    {
        private readonly ClientEndpoints endPoints;

        public ClientsApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.Clients;
        }

        public Task<List<IClient>> GetAll()
            => SendRequest<Client, IClient>(endPoints.Get, AuthHeader);

        public Task<List<IClient>> GetAllSince(DateTimeOffset threshold)
            => SendRequest<Client, IClient>(endPoints.GetSince(threshold), AuthHeader);

        public Task<IClient> Create(IClient client)
        {
            var endPoint = endPoints.Post(client.WorkspaceId);
            var clientCopy = client as Client ?? new Client(client);
            return SendRequest(endPoint, AuthHeader, clientCopy, SerializationReason.Post)
                .Upcast<IClient, Client>();
        }
    }
}
