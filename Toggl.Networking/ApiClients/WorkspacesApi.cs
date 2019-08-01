using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Networking.Helpers;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    internal sealed class WorkspacesApi : BaseApi, IWorkspacesApi
    {
        private readonly WorkspaceEndpoints endPoints;

        private readonly IJsonSerializer serializer;

        public WorkspacesApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer,
            Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.Workspaces;
            this.serializer = serializer;
        }

        public Task<List<IWorkspace>> GetAll()
            => SendRequest<Workspace, IWorkspace>(endPoints.Get, AuthHeader);

        public Task<IWorkspace> GetById(long id)
            => SendRequest<Workspace>(endPoints.GetById(id), AuthHeader)
                .Upcast<IWorkspace, Workspace>();

        public async Task<IWorkspace> Create(IWorkspace workspace)
        {
            var dto = new UserApi.WorkspaceParameters { Name = workspace.Name, InitialPricingPlan = PricingPlans.Free };
            var json = serializer.Serialize(dto, SerializationReason.Post);

            var response = await SendRequest<Workspace>(endPoints.Post, AuthHeader, json);
            return response;
        }
    }
}
