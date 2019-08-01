using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Networking.ApiClients
{
    internal sealed class TimeEntriesApi : BaseApi, ITimeEntriesApi
    {
        private readonly UserAgent userAgent;
        private readonly TimeEntryEndpoints endPoints;

        public TimeEntriesApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer,
            Credentials credentials, UserAgent userAgent)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            this.userAgent = userAgent;
            this.endPoints = endPoints.TimeEntries;
        }

        public Task<List<ITimeEntry>> GetAll()
            => SendRequest<TimeEntry, ITimeEntry>(endPoints.Get, AuthHeader);

        public Task<List<ITimeEntry>> GetAll(DateTimeOffset start, DateTimeOffset end)
        {
            if (start > end)
                throw new InvalidOperationException($"Start date ({start}) must be earlier than the end date ({end}).");

            return SendRequest<TimeEntry, ITimeEntry>(endPoints.GetBetween(start, end), AuthHeader)
                .ContinueWith(t => t.Result ?? new List<ITimeEntry>());
        }

        public Task<List<ITimeEntry>> GetAllSince(DateTimeOffset threshold)
            => SendRequest<TimeEntry, ITimeEntry>(endPoints.GetSince(threshold), AuthHeader);

        public Task<ITimeEntry> Create(ITimeEntry timeEntry)
            => pushTimeEntry(endPoints.Post(timeEntry.WorkspaceId), timeEntry, SerializationReason.Post);

        public Task<ITimeEntry> Update(ITimeEntry timeEntry)
            => pushTimeEntry(endPoints.Put(timeEntry.WorkspaceId, timeEntry.Id), timeEntry, SerializationReason.Default);

        public Task Delete(ITimeEntry timeEntry)
            => SendRequest<ITimeEntry>(endPoints.Delete(timeEntry.WorkspaceId, timeEntry.Id), AuthHeader);

        private Task<ITimeEntry> pushTimeEntry(Endpoint endPoint, ITimeEntry timeEntry, SerializationReason reason)
        {
            var timeEntryCopy = timeEntry as TimeEntry ?? new TimeEntry(timeEntry);
            if (reason == SerializationReason.Post)
            {
                timeEntryCopy.CreatedWith = userAgent.ToString();
            }

            return SendRequest(endPoint, AuthHeader, timeEntryCopy, reason)
                .Upcast<ITimeEntry, TimeEntry>();
        }
    }
}
